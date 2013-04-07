'/*
' * Copyright (C) 2009-2012 Solmead Productions
' *
' * == BEGIN LICENSE ==
' *
' * Licensed under the terms of any of the following licenses at your
' * choice:
' *
' *  - GNU General Public License Version 2 or later (the "GPL")
' *    http://www.gnu.org/licenses/gpl.html
' *
' *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
' *    http://www.gnu.org/licenses/lgpl.html
' *
' *  - Mozilla Public License Version 1.1 or later (the "MPL")
' *    http://www.mozilla.org/MPL/MPL-1.1.html
' *
' * == END LICENSE ==
' */

Imports System
Imports System.Reflection
Imports System.ComponentModel.DataAnnotations
Imports System.Collections.Specialized
Imports System.Data.Entity

Namespace CodeFirst

    Public MustInherit Class Record(Of TT As Class)
        Inherits IRecord
        Implements IValidatableObject

        Public Shared LastIdSaved As Object



        Public Sub HandleDeleteBeforeEvent(db As DbContext)
            HandleDeleteBefore(db)
        End Sub
        Public Sub HandleDeleteAfterEvent(db As DbContext)
            HandleDeleteAfter(db)
        End Sub
        Public Sub HandleSaveBeforeEvent(db As DbContext)
            HandleSaveBefore(db)
        End Sub
        Public Sub HandleSaveAfterEvent(db As DbContext)
            HandleSaveAfter(db)
        End Sub

        Protected Overridable Sub HandleDeleteBefore(db As DbContext)

        End Sub
        Protected Overridable Sub HandleDeleteAfter(db As DbContext)

        End Sub
        Protected Overridable Sub HandleSaveBefore(db As DbContext)

        End Sub
        Protected Overridable Sub HandleSaveAfter(db As DbContext)

        End Sub
        Protected Overridable Sub HandleDeleteCalledStart(ByVal db As DbContext)

        End Sub
        Protected Overridable Sub HandleSaveCalledStart(ByVal db As DbContext)

        End Sub
        Protected Overridable Sub HandleDeleteCalledEnd(ByVal db As DbContext)

        End Sub
        Protected Overridable Sub HandleSaveCalledEnd(ByVal db As DbContext)

        End Sub
        Protected Overridable Sub HandleLoadCalledEnd(ByVal db As DbContext)

        End Sub
        Public Function GetKeyName(db As DbContext) As String
            Dim objectContext = CType(db, Entity.Infrastructure.IObjectContextAdapter).ObjectContext
            Dim objectSet As Objects.ObjectSet(Of TT) = objectContext.CreateObjectSet(Of TT)()
            Dim keyName = (From m In objectSet.EntitySet.ElementType.KeyMembers() Select m.Name).ToList().First()
            Return keyName
        End Function
        Public Function GetKeyValue(db As DbContext) As Object
            Dim name = GetKeyName(db)

            Dim tp As Type = Me.GetType()
            Dim props = tp.GetProperties((BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy))
            Return (From p In props Where p.Name = name Select p).FirstOrDefault.GetValue(Me, Nothing)
        End Function


        Private Shared Function Convert(ByVal entity As Record(Of TT)) As TT
            Return CType(CType(entity, Object), TT)
        End Function
        Private Shared Function Convert(ByVal entity As TT) As Record(Of TT)
            Return CType(CType(entity, Object), Record(Of TT))
        End Function
        Public Shared Function GetList(ByVal db As DbContext) As IQueryable(Of TT)
            If db IsNot Nothing Then
                Dim l = From I In db.Set(GetType(TT)).AsQueryable() Select CType(I, TT)
                Return l
            Else
                Dim l As New List(Of TT)
                Return l.AsQueryable
            End If
        End Function
        Public Shared Function Load(ByVal db As DbContext, ByVal id As Integer, Optional returnNothing As Boolean = False) As TT
            Dim tp As Type = GetType(TT)
            If db IsNot Nothing Then
                'Dim L As New List(Of TT)
                Dim l2 = CType(db.Set(tp).Find({id}), TT)

                If l2 IsNot Nothing Then
                    Convert(l2).HandleLoadCalledEnd(db)
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
        Public Sub DeletePartial(ByVal db As DbContext)
            If db IsNot Nothing Then
                db.ChangeTracker.DetectChanges()
                HandleDeleteCalledStart(db)
                'Dim tp As Type = Me.GetType()
                Try
                    If db.Entry(Me).State <> EntityState.Deleted Then
                        db.Entry(Me).State = EntityState.Deleted
                        'db.Set(tp).Remove(Me)
                    End If
                Catch ex As Exception

                End Try
            End If
        End Sub

        Public Sub SavePartial(ByVal db As DbContext)
            If db IsNot Nothing Then
                db.ChangeTracker.DetectChanges()
                HandleSaveCalledStart(db)
                Dim id As Object = GetKeyValue(db)
                If ((TypeOf (id) Is Integer AndAlso id = 0) OrElse (id Is Nothing) OrElse (TypeOf (id) Is Guid AndAlso id = Guid.Empty)) Then
                    Dim tp As Type = Me.GetType()
                    If db.Entry(Me).State = EntityState.Detached Then
                        db.Set(tp).Add(Me)
                    End If
                    'DB.GetTable(Tp).InsertOnSubmit(Me)
                Else
                    Try
                        Dim tp As Type = Me.GetType()
                        If db.Entry(Me).State = EntityState.Detached Then
                            db.Set(tp).Attach(Me)
                        End If
                    Catch ex As Exception
                        'Dim a As Integer = 0
                    End Try
                End If
            End If
        End Sub

        Public Sub Save(ByVal db As DbContext)
            If db IsNot Nothing Then
                CType(db, Object).HAS.clear()
                SavePartial(db)
                db.SaveChanges()
                LastIdSaved = GetKeyValue(db)
                HandleSaveCalledEnd(db)
                Try
                    Dim Has As List(Of Object) = CType(db, Object).Has
                    For Each i In Has
                        i.HandleAfterSave(db)
                    Next
                Catch ex As Exception

                End Try
            Else
                'If Not Me.IsValid Then Throw New Exception("Rule violations prevent saving")
            End If
        End Sub
        Public Shared Sub Delete(ByVal db As DbContext, id As Integer)
            Dim item = Convert(Load(db, id, True))
            If item IsNot Nothing Then
                item.Delete(db)
            End If
        End Sub

        Public Sub Delete(ByVal db As DbContext)
            If db IsNot Nothing Then
                'Try
                DeletePartial(db)
                db.SaveChanges()
                HandleDeleteCalledEnd(db)
                'Catch ex As Exception
                '    Dim a As Integer = 1
                'End Try
            End If
        End Sub

        
        Public Overridable Function ValidateObject(ByVal validationContext As ValidationContext) As IEnumerable(Of ValidationResult)
            Dim vrList As New List(Of ValidationResult)

            'For Each RV In GetRuleViolations()
            '    VRList.Add(New ValidationResult(RV.ErrorMessage, {RV.PropertyName}.ToList))
            'Next

            Return vrList
        End Function
        Public Function Validate(ByVal validationContext As ValidationContext) As IEnumerable(Of ValidationResult) Implements IValidatableObject.Validate
            Return ValidateObject(validationContext)
        End Function
    End Class


End Namespace