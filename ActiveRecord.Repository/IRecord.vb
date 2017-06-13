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
Imports PocoPropertyData.Extensions

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable>
Public MustInherit Class IRecord
    

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

End Class
