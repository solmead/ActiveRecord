
Imports System.Data.Entity
Imports System.Runtime.CompilerServices
Imports ActiveRecord.Base
Imports ActiveRecord.CodeFirst
Imports ActiveRecord.Repos


    Public Module Extensions
    
        <Extension()> 
        Public function GetList(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT) As IQueryable(of TT)
            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            Return rr.GetList()
        End function
    
        <Extension()> 
        Public function Load(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT, ByVal id As Integer, Optional returnNothing As Boolean = False) As TT

            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            Return rr.Load(id, returnNothing)
        End function
        <Extension()> 
        Public Sub Save(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT)
            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            rr.Save(entity)
        End Sub
        <Extension()> 
        Public Sub SavePartial(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT)
            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            rr.SavePartial(entity)
        End Sub
        <Extension()> 
        Public Sub Delete(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT, id As integer)
            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            rr.Delete(id)
        End Sub
        <Extension()> 
        Public Sub Delete(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT)
            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            rr.Delete(entity)
        End Sub
        <Extension()> 
        Public Sub DeletePartial(Of TT As IRecord, ContextTT As SimpleContext(Of ContextTT))(ByVal entity As TT, db As ContextTT)
            Dim rr As IRepoRecord(of TT) = new RepoRecord(Of TT, ContextTT)(db)
            rr.DeletePartial(entity)
        End Sub

    End Module