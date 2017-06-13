
Imports ActiveRecord.Repos
Imports TestProj1

Public Class PersonRepo
    Inherits RepoRecord(Of Person, DataContext)
    Implements IPersonRepo

    sub New()
        MyBase.New(new DataContext())

    End sub


End Class
