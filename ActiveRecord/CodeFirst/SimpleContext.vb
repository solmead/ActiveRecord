
Imports System.Data.Entity
Imports Microsoft.VisualBasic.CompilerServices

Namespace CodeFirst
    Public Class SimpleContext
        Inherits DbContext
        Implements IContext(Of SimpleContext)

        Public Sub New()
            MyBase.New()
            Extension = New ContextExtension(Of SimpleContext)(Me)
        End Sub
        Public Sub New(nameOrConnectionString As String)
            MyBase.New(nameOrConnectionString)
            Extension = New ContextExtension(Of SimpleContext)(Me)
        End Sub

        Public Property Extension As ContextExtension(Of SimpleContext) Implements IContext(Of SimpleContext).Extension

        Public Function MyBaseSaveChanges() As Integer Implements IContext(Of SimpleContext).MyBaseSaveChanges
            Return MyBase.SaveChanges()
        End Function

        Public Overrides Function SaveChanges() As Integer
            Return Extension.SaveChanges()
        End Function

    End Class
End Namespace