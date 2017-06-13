
Imports System.Data.Entity
Imports System.Text
Imports System.Data.Entity.Infrastructure
Imports HttpObjectCaching
Imports System.Data.Entity.Core.Objects
Imports ActiveRecord.Base
Imports PocoPropertyData.Extensions
Imports ActiveRecord.Repository

Namespace CodeFirst
    Public Class ContextExtension(Of TT As {IContext(Of TT), DbContext})

        Private bContext As TT

        Public Sub New(baseContext As TT)
            baseContext.Extension = Me
            bContext = baseContext
        End Sub

        Public ReadOnly Property ObjectContext As ObjectContext
            Get
                'return ((IObjectContextAdapter)this).ObjectContext;
                Return (CType(bContext, IObjectContextAdapter).ObjectContext)
            End Get
        End Property

        Public Shared Sub ClearContext()
            Dim context As TT
            Cache.SetItem(CacheArea.Request, "DataContext", context)
        End Sub
        Public Shared Function Current() As TT
            Return Cache.GetItem(Of TT)(CacheArea.Request, "DataContext", Function()
                                                                              Dim tp As Type = GetType(TT)
                                                                              Return CType(tp.Assembly.CreateInstance(tp.FullName), TT)
                                                                          End Function)
        End Function
        Public Shared Function Current(createFunction As Func(Of TT)) As TT
            Return Cache.GetItem(Of TT)(CacheArea.Request, "DataContext", createFunction)
        End Function
        Public Shared Function Current(newContext As TT) As TT
            Return Cache.GetItem(Of TT)(CacheArea.Request, "DataContext", newContext)
        End Function


        public Sub AddActiveHandler(of ItemTT)(handler as IActiveEventHAndler)

        end Sub

        Public Function GetHandler(entity As Object) As IActiveEventHandler
            
        End Function

        Private Function CheckChanges(savedObjs As List(Of Object), deletedObjs As List(Of Object)) As Boolean
            Dim changed = False
            bContext.ChangeTracker.DetectChanges()
            Dim changedList = bContext.ChangeTracker.Entries().ToList
            For Each EntityEntry In changedList
                Dim entity = EntityEntry.Entity
                Dim item As IActiveEventHandler = TryCast(entity, IActiveEventHandler)
                If item Is Nothing Then
                    item = GetHandler(entity)
                End If
                If (EntityEntry.State = EntityState.Added OrElse EntityEntry.State = EntityState.Modified) AndAlso Not savedObjs.Contains(entity) Then
                    item.HandleSaveBefore(bContext, entity)
                    savedObjs.Add(entity)
                    changed = True
                ElseIf EntityEntry.State = EntityState.Deleted AndAlso Not deletedObjs.Contains(entity) Then
                    EntityEntry.State = EntityState.Modified
                    For Each op In EntityEntry.OriginalValues.PropertyNames
                        Dim ir = CType(item, IRecord)
                        ir.SetValue(op, EntityEntry.OriginalValues(op))
                    Next
                    bContext.ChangeTracker.DetectChanges()
                    item.HandleDeleteBefore(bContext, entity)
                    EntityEntry.State = EntityState.Deleted
                    deletedObjs.Add(entity)
                    changed = True
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
                Dim sb As New StringBuilder()
                'Logger.GlobalLog.DebugMessage("SaveChanges: Error")
                For Each e In bContext.GetValidationErrors()
                    sb.AppendLine(e.Entry.Entity.GetType().ToString & " failed validation")
                    Debug.WriteLine(e.Entry.Entity.GetType().ToString & " failed validation")
                    For Each se In e.ValidationErrors
                        sb.AppendLine("     [" & se.PropertyName & "] - [" & se.ErrorMessage & "]")
                        Debug.WriteLine("     [" & se.PropertyName & "] - [" & se.ErrorMessage & "]")
                    Next
                Next
                'Dim i2 As Integer = 1

                Throw New Exception("Save Changes Error, See inner exception - " & sb.ToString, ex)
            End Try
            For Each item In savedObjs
                Dim itm As IActiveEventHandler = TryCast(item, IActiveEventHandler)
                If itm Is Nothing Then
                    itm = GetHandler(item)
                End If
                itm.HandleSaveAfter(bContext, item)
            Next
            For Each item In deletedObjs
                Dim itm As IActiveEventHandler = TryCast(item, IActiveEventHandler)
                If itm Is Nothing Then
                    itm = GetHandler(item)
                End If
                itm.HandleDeleteAfter(bContext, item)
            Next

            Return I
        End Function
    End Class
End Namespace