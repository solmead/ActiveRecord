Imports ActiveRecord
Imports ActiveRecord.CodeFirst
Imports TestProj1

Public Class Runner


    public sub main()
        Dim db As New DataContext()
        db.AddActiveHandler(Of Person)(New PersonHandler())
        db.AddActiveHandler(Of Badge)(New BadgeHandler())

        Dim c As New Person()
        
        c.Save(db)

        Dim c2 = c.Load(db, 1)

        'Person.Load(db, 1)

        Dim repo As IPersonRepo = new PersonRepo()


        c2 = repo.Load(1)

        repo.Save(c2)


        
        Dim b As New Badge()
        Dim repo2 As IBadgeRepo = new BadgeRepo()
        repo2.Save(b)

        Dim b2 = repo2.Load(2)



    End sub
End Class
