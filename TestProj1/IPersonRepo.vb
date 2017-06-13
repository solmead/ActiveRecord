Imports ActiveRecord.Base

Public Interface IPersonRepo
    'inherits IRepoRecord(of Person)
    
    Sub SavePartial(entity As Person)
    Sub Save(entity As Person)
    Sub DeletePartial(entity As Person)
    Sub Delete(entity As Person)
    Sub Delete(id As Integer)
    Function GetKeyName() As String
    Function GetKeyValue(entity As Person) As Object
    Function GetList() As IQueryable(Of Person)
    Function Load(ByVal id As Integer, Optional returnNothing As Boolean = False) As Person


End Interface
