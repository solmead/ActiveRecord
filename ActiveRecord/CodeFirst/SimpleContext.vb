
Imports System.Data.Entity
Imports ActiveRecord.Base

Namespace CodeFirst
    Public MustInherit Class SimpleContext(Of TT As {SimpleContext(Of TT)})
        Inherits DbContext
        Implements IContext(Of TT)
        Implements IHandlerContext

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
        
        public Sub AddActiveHandler(of ItemTT)(handler as IActiveEventHAndler)
            Extension.AddActiveHandler(of ItemTT)(handler)
        end Sub

        Public Function GetHandler(entity As Object) As IActiveEventHandler Implements IHandlerContext.GetHandler
            Return Extension.GetHandler(entity)
        End Function
    End Class
End Namespace