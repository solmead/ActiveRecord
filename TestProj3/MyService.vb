Imports TestProj1

Public Class MyService

    Private Property _badgeRepo As IBadgeRepo
    Private Property _personRepo As IPersonRepo


    public sub New(badgerepo As IBadgeRepo,
        personrepo As IPersonRepo)
        _badgeRepo = badgerepo
        _personRepo = personrepo

    End sub



    public sub DoSomething()

        'Dim p = _personRepo.Load(5)
        'p.Name="Test"

        '_personRepo.Save(p)
        
        Dim b = _badgeRepo.Load(5)
        b.Name="Test"

        _badgeRepo.Save(b)




    End sub

End Class
