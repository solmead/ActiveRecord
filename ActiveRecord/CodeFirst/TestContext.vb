
Imports System.Data.Entity
Imports Microsoft.VisualBasic.CompilerServices

Namespace CodeFirst
    Public Class TestContext
        Inherits DbContext
        Implements IContext(Of TestContext)

        Public Sub New()
            MyBase.New()
            Extension = New ContextExtension(Of TestContext)(Me)
        End Sub
        Public Sub New(nameOrConnectionString As String)
            MyBase.New(nameOrConnectionString)
            Extension = New ContextExtension(Of TestContext)(Me)
        End Sub

        Public Property Extension As ContextExtension(Of TestContext) Implements IContext(Of TestContext).Extension

        Public Function MyBaseSaveChanges() As Integer Implements IContext(Of TestContext).MyBaseSaveChanges
            Return MyBase.SaveChanges()
        End Function

        Public Overrides Function SaveChanges() As Integer
            Return Extension.SaveChanges()
        End Function

    End Class
End Namespace