


Public Interface IRepoRecord(Of TT As Class)
    Sub SavePartial(entity As TT)
    Sub Save(entity As TT)
    Sub DeletePartial(entity As TT)
    Sub Delete(entity As TT)
    Sub Delete(id As Integer)
    Function GetKeyName() As String
    Function GetKeyValue(entity As TT) As Object
    Function GetList() As IQueryable(Of TT)
    Function Load(ByVal id As Integer, Optional returnNothing As Boolean = False) As TT
End Interface