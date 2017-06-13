Imports ActiveRecord.Base

Public Interface IBadgeRepo
    'inherits IRepoRecord(of Badge)
    
    Sub SavePartial(entity As Badge)
    Sub Save(entity As Badge)
    Sub DeletePartial(entity As Badge)
    Sub Delete(entity As Badge)
    Sub Delete(id As Integer)
    Function GetKeyName() As String
    Function GetKeyValue(entity As Badge) As Object
    Function GetList() As IQueryable(Of Badge)
    Function Load(ByVal id As Integer, Optional returnNothing As Boolean = False) As Badge

End Interface
