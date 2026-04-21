Imports System
Imports System.IO
Imports System.Net
Imports Renci.SshNet


' クラスをCOM経由でアクセス可能にする
<ComClass(VbaSSH.ClassId, VbaSSH.InterfaceId, VbaSSH.EventsId)>
Public Class VbaSSH

    ' COM用のGUID値
    Public Const ClassId As String = "66FDD47A-3EF0-4A5E-AC14-ADE72015859B"
    Public Const InterfaceId As String = "B516B2E3-3A71-4E4A-A1D0-2B0864FC1C45"
    Public Const EventsId As String = "27A844CC-D1B0-45DF-9E15-AC2BCBE029AF"

    Dim SSHConnection As SshClient


    Private Sub DisposeConnection()
        If SSHConnection IsNot Nothing Then
            If SSHConnection.IsConnected Then
                SSHConnection.Disconnect()
            End If
            SSHConnection.Dispose()
            SSHConnection = Nothing
        End If
    End Sub


    ''' <summary>パスワード認証で接続します。</summary>
    Public Function Open(ByVal Host As String, ByVal Port As Integer, ByVal UserName As String, Password As String) As String
        DisposeConnection()

        Dim ci As New PasswordConnectionInfo(host:=Host,
                                             port:=Port,
                                             username:=UserName,
                                             password:=Password)

        SSHConnection = New SshClient(ci)
        SSHConnection.Connect()
        Return SSHConnection.IsConnected.ToString()
    End Function


    ''' <summary>秘密鍵ファイル（OpenSSH / PEM / PuTTY 等）で接続します。パスフレーズ不要のときは空文字列を渡してください。</summary>
    Public Function OpenWithPrivateKey(ByVal Host As String,
                                       ByVal Port As Integer,
                                       ByVal UserName As String,
                                       ByVal PrivateKeyFilePath As String,
                                       ByVal PrivateKeyPassphrase As String) As String
        DisposeConnection()

        If String.IsNullOrWhiteSpace(PrivateKeyFilePath) Then
            Throw New ArgumentException("Private key file path is required.", NameOf(PrivateKeyFilePath))
        End If
        If Not File.Exists(PrivateKeyFilePath) Then
            Throw New FileNotFoundException("Private key file not found.", PrivateKeyFilePath)
        End If

        Dim pkf As PrivateKeyFile
        If String.IsNullOrEmpty(PrivateKeyPassphrase) Then
            pkf = New PrivateKeyFile(PrivateKeyFilePath)
        Else
            pkf = New PrivateKeyFile(PrivateKeyFilePath, PrivateKeyPassphrase)
        End If

        Dim auth As New PrivateKeyAuthenticationMethod(UserName, pkf)
        Dim ci As New ConnectionInfo(Host, Port, UserName, auth)

        SSHConnection = New SshClient(ci)
        SSHConnection.Connect()
        Return SSHConnection.IsConnected.ToString()
    End Function


    Public Function Execute(Command As String) As String
        If SSHConnection Is Nothing OrElse Not SSHConnection.IsConnected Then
            Throw New InvalidOperationException("SSH session is not connected. Call Open or OpenWithPrivateKey first.")
        End If

        Dim CmdWork As SshCommand = SSHConnection.CreateCommand(Command)
        Return CmdWork.Execute()
    End Function


    Public Sub Close()
        DisposeConnection()
    End Sub

End Class
