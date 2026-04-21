Imports System
Imports System.IO
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


    ''' <summary>
    ''' <paramref name="Login"/> の内容に従い接続します。
    ''' <see cref="VbaSshLogin.PrivateKeyFilePath"/> が空でない場合は秘密鍵認証、そうでなければパスワード認証です。
    ''' </summary>
    Public Function Open(Login As VbaSshLogin) As String
        If Login Is Nothing Then
            Throw New ArgumentNullException(NameOf(Login))
        End If
        If String.IsNullOrWhiteSpace(Login.Host) Then
            Throw New ArgumentException("Host is required.", NameOf(Login))
        End If
        If Login.Port < 1 OrElse Login.Port > 65535 Then
            Throw New ArgumentOutOfRangeException(NameOf(Login), "Port must be between 1 and 65535.")
        End If
        If String.IsNullOrWhiteSpace(Login.UserName) Then
            Throw New ArgumentException("UserName is required.", NameOf(Login))
        End If

        DisposeConnection()

        Dim useKey As Boolean = Not String.IsNullOrWhiteSpace(Login.PrivateKeyFilePath)

        If useKey Then
            If Not File.Exists(Login.PrivateKeyFilePath) Then
                Throw New FileNotFoundException("Private key file not found.", Login.PrivateKeyFilePath)
            End If

            Dim pkf As PrivateKeyFile
            If String.IsNullOrEmpty(Login.PrivateKeyPassphrase) Then
                pkf = New PrivateKeyFile(Login.PrivateKeyFilePath)
            Else
                pkf = New PrivateKeyFile(Login.PrivateKeyFilePath, Login.PrivateKeyPassphrase)
            End If

            Dim auth As New PrivateKeyAuthenticationMethod(Login.UserName, pkf)
            Dim ci As New ConnectionInfo(Login.Host, Login.Port, Login.UserName, auth)
            SSHConnection = New SshClient(ci)
        Else
            Dim ci As New PasswordConnectionInfo(host:=Login.Host,
                                                 port:=Login.Port,
                                                 username:=Login.UserName,
                                                 password:=If(Login.Password, ""))
            SSHConnection = New SshClient(ci)
        End If

        SSHConnection.Connect()
        Return SSHConnection.IsConnected.ToString()
    End Function


    Public Function Execute(Command As String) As String
        If SSHConnection Is Nothing OrElse Not SSHConnection.IsConnected Then
            Throw New InvalidOperationException("SSH session is not connected. Call Open after setting up VbaSshLogin.")
        End If

        Dim CmdWork As SshCommand = SSHConnection.CreateCommand(Command)
        Return CmdWork.Execute()
    End Function


    Public Sub Close()
        DisposeConnection()
    End Sub

End Class
