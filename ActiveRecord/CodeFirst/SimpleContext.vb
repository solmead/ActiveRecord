
Imports System.Data.Entity
Imports Microsoft.VisualBasic.CompilerServices

Namespace CodeFirst
    Public MustInherit Class SimpleContext(Of TT As {SimpleContext(Of TT)})
        Inherits DbContext
        Implements IContext(Of TT)

        Public Sub New()
            MyBase.New()
            Extension = New ContextExtension(Of TT)(Me)
        End Sub
        Public Sub New(nameOrConnectionString As String)
            MyBase.New(nameOrConnectionString)
            Extension = New ContextExtension(Of TT)(Me)
        End Sub

        Public Property Extension As ContextExtension(Of TT) Implements IContext(Of TT).Extension

        Public Function MyBaseSaveChanges() As Integer Implements IContext(Of TT).MyBaseSaveChanges
            Return MyBase.SaveChanges()
        End Function

        Public Overrides Function SaveChanges() As Integer
            Return Extension.SaveChanges()
        End Function

    End Class
End Namespace