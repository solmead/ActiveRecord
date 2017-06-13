Imports ActiveRecord.Base

Public Class BadgeHandler
    Implements IActiveEventHandler

    Public Sub HandleDeleteBefore(db As IHandlerContext, item As Object) Implements IActiveEventHandler.HandleDeleteBefore
        'Throw New NotImplementedException
    End Sub

    Public Sub HandleDeleteAfter(db As IHandlerContext, item As Object) Implements IActiveEventHandler.HandleDeleteAfter
        'Throw New NotImplementedException
    End Sub

    Public Sub HandleSaveBefore(db As IHandlerContext, item As Object) Implements IActiveEventHandler.HandleSaveBefore
        'Throw New NotImplementedException
    End Sub

    Public Sub HandleSaveAfter(db As IHandlerContext, item As Object) Implements IActiveEventHandler.HandleSaveAfter
        'Throw New NotImplementedException
    End Sub

End Class
