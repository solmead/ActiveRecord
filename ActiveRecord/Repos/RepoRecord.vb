

Imports System.Data.Entity
Imports System.Reflection
Imports ActiveRecord.Base
Imports ActiveRecord.CodeFirst
Imports ActiveRecord.Repository

Namespace Repos
    Public Class RepoRecord(Of TT As Class, ContextTT As {SimpleContext(Of ContextTT)})
        Implements IRepoRecord(Of TT)

        public Property db As ContextTT

        Public Sub New(dbContext As ContextTT)
            db = dbContext
        End Sub


        Private Function GetHandler(entity As TT) As IActiveEventHandler
            Dim item = TryCast(entity, IActiveEventHandler)
            If item Is Nothing Then
                item = db.GetHandler(entity)
            End If
            Return item
        End Function

        ''' <summary>
        ''' Marks that this item should be inserted/updated, but does not trigger SaveChanges on context
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should be used when you want to insert an item or trigger an item should be saved, the protected handle function should only use this</remarks>
        Public Sub SavePartial(entity As TT)
            SavePartial(entity, GetHandler(entity))
        End Sub

        ''' <summary>
        ''' Marks an item should be deleted then triggers all changes on the context should be done to the database
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should not be used from inside the protected handle functions</remarks>
        Public Sub DeletePartial(entity As TT)
            DeletePartial(entity, GetHandler(entity))
        End Sub

        ''' <summary>
        ''' Marks that this item should be inserted/updated, but does not trigger SaveChanges on context
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should be used when you want to insert an item or trigger an item should be saved, the protected handle function should only use this</remarks>
        Public Sub Save(entity As TT)
            Save(entity, GetHandler(entity))
        End Sub

        ''' <summary>
        ''' Marks an item should be deleted then triggers all changes on the context should be done to the database
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should not be used from inside the protected handle functions</remarks>
        Public Sub Delete(entity As TT)
            Delete(entity, GetHandler(entity))
        End Sub

        Public Function GetKeyName() As String
            Dim objectContext = CType(db, Entity.Infrastructure.IObjectContextAdapter).ObjectContext
            Dim objectSet As Core.Objects.ObjectSet(Of TT) = objectContext.CreateObjectSet(Of TT)()
            Dim keyName = (From m In objectSet.EntitySet.ElementType.KeyMembers() Select m.Name).ToList().First()
            Return keyName
        End Function
        Public Function GetKeyValue(entity As TT) As Object
            Dim name = GetKeyName()
            Dim tp As Type = GetType(TT)
            Dim props = tp.GetProperties((BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy))
            Return (From p In props Where p.Name = name Select p).FirstOrDefault.GetValue(entity, Nothing)
        End Function


        ''' <summary>
        ''' Get Queryable entry for current type
        ''' </summary>
        ''' <param name="db">Data Context</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetList() As IQueryable(Of TT)
            If db IsNot Nothing Then
                Dim l = From I In db.Set(GetType(TT)).AsQueryable() Select CType(I, TT)
                Return l
            Else
                Dim l As New List(Of TT)
                Return l.AsQueryable
            End If
        End Function
        ''' <summary>
        ''' Gets requested item, uses find on DbSet
        ''' </summary>
        ''' <param name="db">Code First Data context</param>
        ''' <param name="id">Id of item to find</param>
        ''' <param name="returnNothing">If item is not found return null or nothing</param>
        ''' <returns>Returns item found, by default if item is not found returns empty item</returns>
        ''' <remarks></remarks>
        Public Function Load(ByVal id As Integer, Optional returnNothing As Boolean = False) As TT
            Dim tp As Type = GetType(TT)
            If db IsNot Nothing Then
                'Dim L As New List(Of TT)
                Dim l2 = CType(db.Set(tp).Find({id}), TT)

                If l2 IsNot Nothing Then
                    'Convert(l2).HandleLoadCalledEnd(db)
                    Return l2
                ElseIf Not returnNothing Then
                    Dim o As Object = tp.Assembly.CreateInstance(tp.FullName, True)

                    Return CType(o, TT)
                Else
                    Return Nothing
                End If
            ElseIf Not returnNothing Then
                Dim o As Object = tp.Assembly.CreateInstance(tp.FullName, True)

                Return CType(o, TT)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' Marks that this item should be deleted, but does not trigger save changes in the context
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should be used when you want to delete an item but not trigger an additional save changes, the protected handle functions should only use this for other objects</remarks>
        Public Sub DeletePartial(entity As TT, eventHandler As IActiveEventHandler)
            If db IsNot Nothing Then
                db.ChangeTracker.DetectChanges()
                If (eventHandler IsNot Nothing) Then
                    eventHandler.HandleDeleteBefore(db, entity)
                End If
                'Dim tp As Type = Me.GetType()
                Try
                    If db.Entry(entity).State <> EntityState.Deleted Then
                        db.Entry(entity).State = EntityState.Deleted
                        'db.Set(tp).Remove(Me)
                    End If
                Catch ex As Exception

                End Try
            End If
        End Sub

        ''' <summary>
        ''' Marks that this item should be inserted/updated, but does not trigger SaveChanges on context
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should be used when you want to insert an item or trigger an item should be saved, the protected handle function should only use this</remarks>
        Public Sub SavePartial(entity As TT, eventHandler As IActiveEventHandler)
            If db IsNot Nothing Then
                db.ChangeTracker.DetectChanges()
                If (eventHandler IsNot Nothing) Then
                    eventHandler.HandleSaveBefore(db, entity)
                End If
                Dim id As Object = GetKeyValue(entity)
                If ((TypeOf (id) Is Integer AndAlso id = 0) OrElse (id Is Nothing) OrElse (TypeOf (id) Is Guid AndAlso id = Guid.Empty)) Then
                    Dim tp As Type = entity.GetType()
                    If db.Entry(entity).State = EntityState.Detached Then
                        db.Set(tp).Add(entity)
                    End If
                    'DB.GetTable(Tp).InsertOnSubmit(Me)
                Else
                    Try
                        Dim tp As Type = entity.GetType()
                        If db.Entry(entity).State = EntityState.Detached Then
                            db.Set(tp).Attach(entity)
                            db.Entry(entity).State = EntityState.Modified
                        End If
                    Catch ex As Exception
                        Dim a As Integer = 0
                        Throw
                    End Try
                End If
            End If
        End Sub
        ''' <summary>
        ''' Marks item should be saved, and triggers SaveCanges to submit all changes on the current context
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should not be used from inside the protected handle functions</remarks>
        Public Sub Save(entity As TT, eventHandler As IActiveEventHandler)
            If db IsNot Nothing Then
                'CType(db, Object).HAS.clear()
                SavePartial(entity, eventHandler)
                db.SaveChanges()
                'LastIdSaved = GetKeyValue(db)
                If (eventHandler IsNot Nothing) Then
                    eventHandler.HandleSaveAfter(db, entity)
                End If
                'HandleSaveCalledEnd(db)
                'Try
                '    Dim Has As List(Of Object) = CType(db, Object).Has
                '    For Each i In Has
                '        i.HandleAfterSave(db)
                '    Next
                'Catch ex As Exception

                'End Try
            Else
                'If Not Me.IsValid Then Throw New Exception("Rule violations prevent saving")
            End If
        End Sub
        ''' <summary>
        ''' Marks an item by it's id should be deleted then triggers all changes on the context should be done to the database
        ''' </summary>
        ''' <param name="db"></param>
        ''' <param name="id">Id of item to delete</param>
        ''' <remarks>This should not be used from inside the protected handle functions. also this triggers a load from the db first</remarks>
        Public Sub Delete(id As Integer)
            Dim item = me.Load(id, True)
            If item IsNot Nothing Then
                Delete(item)
            End If
        End Sub

        ''' <summary>
        ''' Marks an item by it's id should be deleted then triggers all changes on the context should be done to the database
        ''' </summary>
        ''' <param name="db"></param>
        ''' <param name="id">Id of item to delete</param>
        ''' <remarks>This should not be used from inside the protected handle functions. also this triggers a load from the db first</remarks>
        Public Sub Delete(id As Integer, eventHandler As IActiveEventHandler)
            Dim item = Load(id, True)
            If item IsNot Nothing Then
                Delete(item, eventHandler)
            End If
        End Sub


        ''' <summary>
        ''' Marks an item should be deleted then triggers all changes on the context should be done to the database
        ''' </summary>
        ''' <param name="db"></param>
        ''' <remarks>This should not be used from inside the protected handle functions</remarks>
        Public Sub Delete(entity As TT, eventHandler As IActiveEventHandler)
            If db IsNot Nothing Then
                'Try
                DeletePartial(entity, eventHandler)
                db.SaveChanges()
                If (eventHandler IsNot Nothing) Then
                    eventHandler.HandleDeleteAfter(db, entity)
                End If
                'HandleDeleteCalledEnd(db)
                'Catch ex As Exception
                '    Dim a As Integer = 1
                'End Try
            End If
        End Sub

        Private Sub IRepoRecord_SavePartial(entity As TT) Implements IRepoRecord(Of TT).SavePartial
            SavePartial(entity)
        End Sub

        Private Sub IRepoRecord_Save(entity As TT) Implements IRepoRecord(Of TT).Save
            Save(entity)
        End Sub

        Private Sub IRepoRecord_DeletePartial(entity As TT) Implements IRepoRecord(Of TT).DeletePartial
            DeletePartial(entity)
        End Sub

        Private Sub IRepoRecord_Delete(entity As TT) Implements IRepoRecord(Of TT).Delete
            Delete(entity)
        End Sub

        Private Sub IRepoRecord_Delete1(id As Integer) Implements IRepoRecord(Of TT).Delete
            Delete(id)
        End Sub

        Private Function IRepoRecord_GetKeyName() As String Implements IRepoRecord(Of TT).GetKeyName
            Return GetKeyName()
        End Function

        Private Function IRepoRecord_GetKeyValue(entity As TT) As Object Implements IRepoRecord(Of TT).GetKeyValue
            Return GetKeyValue(entity)
        End Function

        Private Function IRepoRecord_GetList() As IQueryable(Of TT) Implements IRepoRecord(Of TT).GetList
            return GetList()
        End Function

        Private Function IRepoRecord_Load(id As Integer, Optional returnNothing As Boolean = False) As TT Implements IRepoRecord(Of TT).Load
            Return Load(id, returnNothing)
        End Function


        'Public Overridable Function ValidateObject(ByVal validationContext As ValidationContext) As IEnumerable(Of ValidationResult)
        '    Dim vrList As New List(Of ValidationResult)

        '    'For Each RV In GetRuleViolations()
        '    '    VRList.Add(New ValidationResult(RV.ErrorMessage, {RV.PropertyName}.ToList))
        '    'Next

        '    Return vrList
        'End Function
        'Public Function Validate(ByVal validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate
        '    Return ValidateObject(validationContext)
        'End Function
        'Public Sub SavePartial(entity As TT) Implements IRepoRecord(Of TT).SavePartial
        '    Dim cdb = CType(db, ContextTT)
        '    SavePartial(cdb, entity)
        'End Sub


        'Public Sub Save(entity As TT) Implements IRepoRecord(Of TT).Save
        '    Dim cdb = CType(db, ContextTT)
        '    Save(cdb, entity)
        'End Sub


        'Public Sub DeletePartial(entity As TT) Implements IRepoRecord(Of TT).DeletePartial
        '    Dim cdb = CType(db, ContextTT)
        '    DeletePartial(cdb, entity)
        'End Sub


        'Public Sub Delete(entity As TT) Implements IRepoRecord(Of TT).Delete
        '    Dim cdb = CType(db, ContextTT)
        '    Delete(cdb, entity)
        'End Sub


        'Public Sub Delete(id As Integer) Implements IRepoRecord(Of TT).Delete
        '    Dim cdb = CType(db, ContextTT)
        '    Delete(cdb, id)
        'End Sub


        'Public Function GetKeyName() As String Implements IRepoRecord(Of TT).GetKeyName
        '    Dim cdb = CType(db, ContextTT)
        '    Return GetKeyName(cdb)
        'End Function

        'Public Function GetKeyValue(entity As TT) As Object Implements IRepoRecord(Of TT).GetKeyValue
        '    Dim cdb = CType(db, ContextTT)
        '    Return GetKeyValue(cdb, entity)
        'End Function

        'Public Function GetList() As IQueryable(Of TT) Implements IRepoRecord(Of TT).GetList
        '    Dim cdb = CType(db, ContextTT)
        '    Return GetList(cdb)
        'End Function

        'Public Function Load(id As Integer, Optional returnNothing As Boolean = False) As TT Implements IRepoRecord(Of TT).Load
        '    Dim cdb = CType(db, ContextTT)
        '    Return Load(cdb, id, returnNothing)
        'End Function
    End Class
End Namespace