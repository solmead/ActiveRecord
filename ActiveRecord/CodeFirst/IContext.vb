
Imports System.Data.Entity

Namespace CodeFirst
    Public Interface IContext(Of TT As {IContext(Of TT), DbContext})
        Property Extension As ContextExtension(Of TT)

        Function MyBaseSaveChanges() As Integer

    End Interface
End Namespace