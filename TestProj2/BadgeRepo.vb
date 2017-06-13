Imports ActiveRecord.Repos
Imports TestProj1

Public Class BadgeRepo
    Inherits RepoRecord(Of Badge, DataContext)
    Implements IBadgeRepo

    sub New()
        MyBase.New(new DataContext())

    End sub

    Public Sub SavePartial2(entity As Badge) Implements IBadgeRepo.SavePartial
        SavePartial(entity)
    End Sub

    Public Sub Save2(entity As Badge) Implements IBadgeRepo.Save
        Save(entity)
    End Sub

    Public Sub DeletePartial2(entity As Badge) Implements IBadgeRepo.DeletePartial
        DeletePartial2(entity)
    End Sub

    Public Sub Delete2(entity As Badge) Implements IBadgeRepo.Delete
        Delete(entity)
    End Sub

    Public Sub Delete2(id As Integer) Implements IBadgeRepo.Delete
        Delete(id)
    End Sub

    Public Function GetKeyName2() As String Implements IBadgeRepo.GetKeyName
        Return GetKeyName()
    End Function

    Public Function GetKeyValue2(entity As Badge) As Object Implements IBadgeRepo.GetKeyValue
        Return GetKeyValue(entity)
    End Function

    Public Function GetList2() As IQueryable(Of Badge) Implements IBadgeRepo.GetList
        Return GetList()
    End Function

    Public Function Load2(id As Integer, Optional returnNothing As Boolean = False) As Badge Implements IBadgeRepo.Load
        Return Load(id, returnNothing)
    End Function
End Class
