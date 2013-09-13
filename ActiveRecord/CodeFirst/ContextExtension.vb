
Imports System.Data.Entity
Imports System.Runtime.InteropServices
Imports System.CodeDom
Imports System.Text

Namespace CodeFirst
    Public Class ContextExtension(Of TT As {IContext(Of TT), DbContext})

        Private bContext As TT

        Public Sub New(baseContext As TT)
            baseContext.Extension = Me
            bContext = baseContext
        End Sub


        Private Function CheckChanges(savedObjs As List(Of Object), deletedObjs As List(Of Object)) As Boolean
            Dim changed = False
            bContext.ChangeTracker.DetectChanges()
            Dim changedList = bContext.ChangeTracker.Entries().ToList
            For Each EntityEntry In changedList
                Dim item As Object = TryCast(EntityEntry.Entity, IRecord)
                If item IsNot Nothing Then
                    If (EntityEntry.State = EntityState.Added OrElse EntityEntry.State = EntityState.Modified) AndAlso Not savedObjs.Contains(item) Then
                        item.HandleSaveBeforeEvent(Me)
                        savedObjs.Add(item)
                        changed = True
                    ElseIf EntityEntry.State = EntityState.Deleted AndAlso Not deletedObjs.Contains(item) Then
                        EntityEntry.State = EntityState.Modified
                        For Each op In EntityEntry.OriginalValues.PropertyNames
                            Dim ir = CType(item, IRecord)
                            ir.SetValue(op, EntityEntry.OriginalValues(op))
                        Next
                        bContext.ChangeTracker.DetectChanges()
                        item.HandleDeleteBeforeEvent(Me)
                        EntityEntry.State = EntityState.Deleted
                        deletedObjs.Add(item)
                        changed = True
                    End If
                End If
            Next
            Return changed
        End Function


        Public Function SaveChanges() As Integer
            bContext.ChangeTracker.DetectChanges()
            Dim I As Integer
            Dim savedObjs As New List(Of Object)
            Dim deletedObjs As New List(Of Object)

            While CheckChanges(savedObjs, deletedObjs)

            End While
            Try
                'Logger.GlobalLog.DebugMessage("SaveChanges: Saving changes count=" & savedObjs.Count)
                I = bContext.MyBaseSaveChanges()
                'Logger.GlobalLog.DebugMessage("SaveChanges: Saved changes")
            Catch ex As Exception
                Dim SB As New StringBuilder()
                'Logger.GlobalLog.DebugMessage("SaveChanges: Error")
                For Each e In bContext.GetValidationErrors()
                    SB.AppendLine(e.Entry.Entity.GetType().ToString & " failed validation")
                    Debug.WriteLine(e.Entry.Entity.GetType().ToString & " failed validation")
                    For Each se In e.ValidationErrors
                        SB.AppendLine("     [" & se.PropertyName & "] - [" & se.ErrorMessage & "]")
                        Debug.WriteLine("     [" & se.PropertyName & "] - [" & se.ErrorMessage & "]")
                    Next
                Next
                'Dim i2 As Integer = 1

                Throw New Exception("Save Changes Error, See inner exception - " & SB.ToString, ex)
            End Try
            For Each item In savedObjs
                item.HandleSaveAfterEvent(Me)
            Next
            For Each item In deletedObjs
                item.HandleDeleteAfterEvent(Me)
            Next

            Return I
        End Function
    End Class
End Namespace