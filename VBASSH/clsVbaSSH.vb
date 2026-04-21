Imports System
Imports System.Net
Imports Renci.SshNet.ConnectionInfo


' クラスをCOM経由でアクセス可能にする
<ComClass(VbaSSH.ClassId, VbaSSH.InterfaceId, VbaSSH.EventsId)>
Public Class VbaSSH

    ' COM用のGUID値
    Public Const ClassId As String = "66FDD47A-3EF0-4A5E-AC14-ADE72015859B"
    Public Const InterfaceId As String = "B516B2E3-3A71-4E4A-A1D0-2B0864FC1C45"
    Public Const EventsId As String = "27A844CC-D1B0-45DF-9E15-AC2BCBE029AF"

    Dim ConnectInfo As Renci.SshNet.PasswordConnectionInfo
    Dim SSHConnection As Renci.SshNet.SshClient



    ' VBAから利用できるメソッド
    Public Function Open(ByVal Host As String, ByVal Port As Integer, ByVal UserName As String, Password As String) As String


        ConnectInfo = New Renci.SshNet.PasswordConnectionInfo(host:=Host,
                                                              port:=Port,
                                                              username:=UserName,
                                                              password:=Password)

        SSHConnection = New Renci.SshNet.SshClient(ConnectInfo)
        SSHConnection.Connect()
        Return SSHConnection.IsConnected

    End Function

    Public Function Execute(Command As String) As String
        Dim CmdWork As Renci.SshNet.SshCommand
        Dim Result As String

        CmdWork = SSHConnection.CreateCommand(Command)
        Result = CmdWork.Execute
        Return Result
    End Function

    ' VBAから利用できるメソッド
    Public Sub Close()
        SSHConnection.Disconnect()
    End Sub

End Class
