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
Imports System.Reflection
Imports System.Data.Linq
Imports System.ComponentModel.DataAnnotations
Imports System.Collections.Specialized

Namespace Linq2Sql

    Public MustInherit Class Record(Of TT As Class)
        Inherits IRecord
        Implements IValidatableObject

        Public Shared LastIDSaved As Object

        'Private Shared m_InternalDB As DataContext
        'Private Shared Function DataContext() As DataContext
        '    If m_InternalDB Is Nothing Then
        '        m_InternalDB = BaseData.CVP_DataContext
        '    End If
        '    Return m_InternalDB
        'End Function
        Public Property OverrideValidation As Boolean

        Public ReadOnly Property IsValid() As Boolean
            Get
                If OverrideValidation Then
                    Return True
                End If
                Dim list = GetRuleViolations
                Dim tp As Type = GetType(TT)
                For Each I In list
                    Debug.WriteLine("Type:" & tp.Name & " Prop:" & I.PropertyName & " Prob:" & I.ErrorMessage)
                Next
                Return list.Count = 0
            End Get
        End Property

        Public Overridable Function GetRuleViolations() As List(Of RuleViolation)
            Return New List(Of RuleViolation)
        End Function

        Private Shared Function GetKeyProperty() As String
            Dim tp As Type = GetType(TT)

            'tp.GetProperties((BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy))
            Dim mainCa = (From p In tp.GetProperties((BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)) From ca In p.GetCustomAttributes(False) Where ca.GetType Is GetType(Data.Linq.Mapping.ColumnAttribute) AndAlso CType(ca, Data.Linq.Mapping.ColumnAttribute).IsPrimaryKey Select ca, p.Name).FirstOrDefault

            Dim idString = mainCa.Name
            If idString = "" Then idString = "ID"
            Return idString
        End Function

        Protected Function GetKeyValue() As Object
            Dim obj As Object = Me.GetType.GetProperty(GetKeyProperty).GetValue(Me, Nothing)
            Return obj
        End Function
        Public Overridable Sub HandleDeleteBefore(db As Data.Linq.DataContext)

        End Sub
        Public Overridable Sub HandleDeleteAfter(db As Data.Linq.DataContext)

        End Sub
        Public Overridable Sub HandleSaveBefore(db As Data.Linq.DataContext)

        End Sub
        Public Overridable Sub HandleSaveAfter(db As Data.Linq.DataContext)

        End Sub
        Protected Overridable Sub HandleDeleteCalledStart(ByVal db As Data.Linq.DataContext)

        End Sub
        Protected Overridable Sub HandleSaveCalledStart(ByVal db As Data.Linq.DataContext)

        End Sub
        Protected Overridable Sub HandleDeleteCalledEnd(ByVal db As Data.Linq.DataContext)

        End Sub
        Protected Overridable Sub HandleSaveCalledEnd(ByVal db As Data.Linq.DataContext)

        End Sub
        Protected Overridable Sub HandleLoadCalledEnd(ByVal db As Data.Linq.DataContext)

        End Sub


        Private Shared Function Convert(ByVal record As Record(Of TT)) As TT
            Return CType(CType(record, Object), TT)
        End Function
        Private Shared Function Convert(ByVal entity As TT) As Record(Of TT)
            Return CType(CType(Entity, Object), Record(Of TT))
        End Function
        'Public Shared Function GetList() As IQueryable(Of t)
        '    Return GetList(DataContext)
        'End Function
        Public Shared Function GetList(ByVal db As DataContext) As IQueryable(Of TT)
            If DB IsNot Nothing Then
                Return From I As TT In DB.GetTable(GetType(TT)) Select I
            Else
                Dim l As New List(Of TT)
                Return l.AsQueryable
            End If
        End Function
        'Public Shared Function Load(ByVal ID As Integer) As t

        'End Function
        Public Shared Function Load(ByVal db As DataContext, ByVal id As Object) As TT
            Dim tp As Type = GetType(TT)
            If db IsNot Nothing Then
                'Dim p = Tp.GetProperty("ID")
                Dim colName As String = GetKeyProperty()
                Dim tableName As String = ""
                'Dim PropCAs = From CA In p.GetCustomAttributes(False) Where CA.GetType Is GetType(System.Data.Linq.Mapping.ColumnAttribute) Select CA
                'If PropCAs.Count > 0 Then
                ' Dim ca As System.Data.Linq.Mapping.ColumnAttribute = PropCAs.First
                'ColName = ca.Name
                'End If
                'If ColName = "" Then
                '    ColName = "ID"
                'End If


                Dim objCAs = From ca In tp.GetCustomAttributes(False) Where ca.GetType Is GetType(Data.Linq.Mapping.TableAttribute) Select ca
                If objCAs.Count > 0 Then
                    Dim ca As Data.Linq.Mapping.TableAttribute = objCAs.First
                    tableName = ca.Name
                End If

                Dim l As New List(Of TT)

                'Dim st As String = List.ToString & " where [t0].[" & ColName & "]={0}"
                'Dim st As String = List.ToString & " where [t0].[ID]={0}"
                Try

                    Dim st As String = "Select * from " & tableName & " where " & colName & "={0}"
                    Dim l2 = db.ExecuteQuery(Of TT)(st, id.ToString)
                    l.AddRange(l2)

                Catch ex As Exception

                End Try
                If l.Count > 0 Then
                    Dim item = Convert(l.First)
                    item.HandleLoadCalledEnd(db)
                    Return Convert(item)
                Else
                    Dim o As Object = tp.Assembly.CreateInstance(tp.FullName, True)

                    Return CType(o, TT)
                End If
            Else

                Dim o As Object = tp.Assembly.CreateInstance(tp.FullName, True)

                Return CType(o, TT)
            End If
        End Function
        'Public Shared Sub Save(ByVal entity As t)

        'End Sub
        Public Shared Sub Save(ByVal db As DataContext, ByVal entity As TT)
            Dim eOrg As Record(Of TT) = Convert(entity)
            Dim id As Object = eOrg.GetKeyValue()
            If ((TypeOf (id) Is Integer AndAlso id = 0) OrElse (id Is Nothing)) Then
                eOrg.Save(DB)
            Else
                Dim e1 As Record(Of TT) = Convert(Load(db, id))
                'Dim e2 As Record(Of TT) = Convert(e1)
                eOrg.CopyInto(e1)
                e1.Save(db)
            End If
        End Sub
        'Public Sub DeletePartial()

        'End Sub
        Public Sub DeletePartial(ByVal db As DataContext)
            If DB IsNot Nothing Then
                Dim tp As Type = Me.GetType()
                Try
                    DB.GetTable(tp).Attach(Me, True)
                Catch ex As Exception

                End Try
                DB.GetTable(tp).DeleteOnSubmit(Me)
                HandleDeleteCalledStart(DB)
            End If
        End Sub
        'Public Sub SavePartial()

        'End Sub
        Public Sub SavePartial(ByVal db As DataContext)
            If DB IsNot Nothing Then
                Dim id As Object = GetKeyValue()
                If ((TypeOf (id) Is Integer AndAlso id = 0) OrElse (id Is Nothing) OrElse (TypeOf (id) Is Guid AndAlso id = Guid.Empty)) Then
                    Dim tp As Type = Me.GetType()
                    db.GetTable(tp).InsertOnSubmit(Me)
                Else
                    Try
                        Dim tp As Type = Me.GetType()
                        db.GetTable(tp).Attach(Me)
                    Catch ex As Exception
                        ' Dim a As Integer = 0
                    End Try
                End If
                HandleSaveCalledStart(DB)
            End If
        End Sub
        'Public Shared Sub Delete(ByVal entity As t)

        'End Sub
        Public Shared Sub Delete(ByVal db As DataContext, ByVal entity As TT)
            Dim id As Integer = CInt(entity.GetType.GetProperty("ID").GetValue(entity, Nothing))
            Dim e As Record(Of TT) = Convert(Load(db, id))
            e.Delete(db)
        End Sub
        'Public Sub Save()

        'End Sub
        Public Sub Save(ByVal db As DataContext)
            If db IsNot Nothing Then
                CType(db, Object).HAS.clear()
                SavePartial(db)
                db.SubmitChanges()
                LastIDSaved = GetKeyValue()
                HandleSaveCalledEnd(db)
                For Each i As Object In CType(db, Object).HAS
                    i.HandleSaveAfter(db)
                Next
            Else
                If Not IsValid Then Throw New Exception("Rule violations prevent saving")
            End If
        End Sub
        'Public Sub Delete()

        'End Sub
        Public Sub Delete(ByVal db As DataContext)
            If DB IsNot Nothing Then
                'Try
                DeletePartial(DB)
                DB.SubmitChanges()
                HandleDeleteCalledEnd(DB)
                'Catch ex As Exception
                '    Dim a As Integer = 1
                'End Try
            End If
        End Sub
        

        Public Overridable Function ValidateObject(ByVal validationContext As System.ComponentModel.DataAnnotations.ValidationContext) As System.Collections.Generic.IEnumerable(Of System.ComponentModel.DataAnnotations.ValidationResult)
            Dim VRList As New List(Of ValidationResult)

            For Each RV In GetRuleViolations()
                VRList.Add(New ValidationResult(RV.ErrorMessage, {RV.PropertyName}.ToList))
            Next

            Return VRList
        End Function
        Public Function Validate(ByVal validationContext As System.ComponentModel.DataAnnotations.ValidationContext) As System.Collections.Generic.IEnumerable(Of System.ComponentModel.DataAnnotations.ValidationResult) Implements System.ComponentModel.DataAnnotations.IValidatableObject.Validate
            Return ValidateObject(validationContext)
        End Function
    End Class

End Namespace