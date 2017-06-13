


Public interface IActiveEventHandler
    Sub HandleDeleteBefore(db As IHandlerContext,  item As object)
    Sub HandleDeleteAfter(db As IHandlerContext,  item As object)
    Sub HandleSaveBefore(db As IHandlerContext,  item As object)
    Sub HandleSaveAfter(db As IHandlerContext,  item As object)
    'Sub HandleDeleteCalledStart(ByVal db As DbContext)
    'Sub HandleSaveCalledStart(ByVal db As DbContext)
    'Sub HandleDeleteCalledEnd(ByVal db As DbContext)
    'Sub HandleSaveCalledEnd(ByVal db As DbContext)
    'Sub HandleLoadCalledEnd(ByVal db As DbContext)
        
        
End interface