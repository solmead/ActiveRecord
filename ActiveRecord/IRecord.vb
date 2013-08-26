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
Imports System.Collections.Specialized

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class IRecord

    'Protected Overridable Function GetKeyValue() As Object
    '    Return Nothing
    'End Function

    Public Function GetSubJSON() As Object
        Return Me.GetJSON
    End Function

    Public Overridable Function GetJSON() As Object
        Dim Obj = New With {.ID = ""}

        Return Obj
    End Function
    Public Overridable Function GetJSONForDataTable() As Object
        Return GetJSON()
    End Function

    Public Function Clone() As IRecord
        Dim tp As Type = Me.GetType()
        Dim newItem As IRecord = CType(tp.Assembly.CreateInstance(tp.FullName), IRecord)
        Me.CopyInto(newItem)
        Return newItem
    End Function
    Public Sub CloneFrom(ByVal oldItem As IRecord)
        oldItem.CopyInto(Me)
    End Sub
    Public Sub CopyInto(ByVal newItem As IRecord)
        For Each p In Me.GetPropertyNames(onlyBaseTypes:=True, onlyWritable:=True)
            newItem.SetValue(p, GetValue(p))
        Next
    End Sub
    Public Overridable Function AsCollection() As NameValueCollection
        Dim f As New NameValueCollection
        For Each p In Me.GetPropertyNames(onlyBaseTypes:=True)
            f(p) = Me.GetValue(p)
        Next
        Return f
    End Function

    Public Overrides Function ToString() As String
        Dim sb As New Text.StringBuilder
        For Each p In Me.GetPropertyNames(onlyBaseTypes:=True, onlyWritable:=True)
            Dim v = Me.GetValue(p)
            If v Is Nothing Then
                v = ""
            End If

            sb.AppendLine(p & ": " & v.ToString)
        Next

        Return sb.ToString
    End Function
    Public Overridable Function GetPropertyType(propertyName As String) As Type
        Dim tp As Type = Me.GetType()
        Dim prop = tp.GetProperty(propertyName)

        If (prop IsNot Nothing) Then
            Return prop.PropertyType
        End If
        Return GetType(String)
    End Function
    Public Overridable Sub SetValue(propertyName As String, value As Object)

        Dim tp As Type = Me.GetType()
        Dim prop = tp.GetProperty(propertyName)

        If (prop IsNot Nothing) Then
            prop.SetValue(Me, value, Nothing)
        End If
    End Sub
    Public Overridable Function GetValue(propertyName As String) As Object
        Dim retVal As Object = Nothing
        Dim tp As Type = Me.GetType()
        Dim prop = tp.GetProperty(propertyName)
        If (prop IsNot Nothing) Then
            retVal = prop.GetValue(Me, Nothing)
        End If

        Return retVal
    End Function
    Public Overridable Function DoesPropertyExist(propertyName As String) As Boolean
        ' Dim retVal As Object = Nothing
        Dim tp As Type = Me.GetType()
        Dim prop = tp.GetProperty(propertyName)
        Return (prop IsNot Nothing)
    End Function
    Public Overridable Function GetPropertyNames(Optional onlyWritable As Boolean = True, Optional onlyBaseTypes As Boolean = False) As List(Of String)
        Dim tp As Type = Me.GetType()
        Dim props = tp.GetProperties((BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)).ToList

        If onlyWritable Then
            props = (From p In props Where p.CanWrite Select p).ToList
        End If
        If onlyBaseTypes Then
            Try
                props = (From p In props
                         Where Not (p.PropertyType.FullName.Contains("Record") OrElse
                         p.PropertyType.FullName.Contains("Set") OrElse
                         p.PropertyType.FullName.Contains("EntitySet") OrElse
                         (p.PropertyType.BaseType IsNot Nothing AndAlso
                          (p.PropertyType.BaseType.FullName.Contains("Record") OrElse
                         p.PropertyType.BaseType.FullName.Contains("Set") OrElse
                         p.PropertyType.BaseType.FullName.Contains("EntitySet"))))).ToList
            Catch ex As Exception

            End Try
        End If

        Return (From p In props Select p.Name).ToList
    End Function

End Class
