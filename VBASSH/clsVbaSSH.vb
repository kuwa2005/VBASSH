Imports System
Imports System.IO
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports Renci.SshNet
Imports Renci.SshNet.Common


' クラスをCOM経由でアクセス可能にする
<ComClass(VbaSSH.ClassId, VbaSSH.InterfaceId, VbaSSH.EventsId)>
Public Class VbaSSH

    ' COM用のGUID値
    Public Const ClassId As String = "66FDD47A-3EF0-4A5E-AC14-ADE72015859B"
    Public Const InterfaceId As String = "B516B2E3-3A71-4E4A-A1D0-2B0864FC1C45"
    Public Const EventsId As String = "27A844CC-D1B0-45DF-9E15-AC2BCBE029AF"

    Private Const DefaultShellTimeoutSeconds As Integer = 120

    Dim SSHConnection As SshClient
    Dim _shell As ShellStream
    Private _usePersistentShell As Boolean = True
    Private _shellCommandTimeoutSeconds As Integer = DefaultShellTimeoutSeconds
    Private _lastError As String = ""
    Private _lastExitStatus As Integer = -1


    ''' <summary>直近の <see cref="Open"/> または <see cref="Execute"/> で失敗したときのメッセージ（成功時は空）。</summary>
    Public ReadOnly Property LastError() As String
        Get
            Return If(_lastError, "")
        End Get
    End Property


    ''' <summary>
    ''' 直近の <see cref="Execute"/> の終了コード。<see cref="UsePersistentShell"/> が <c>True</c> のときは取得できないため <c>-1</c>。
    ''' 非対話 exec（<c>False</c>）のときはリモートの終了コード（0 が成功）。
    ''' </summary>
    Public ReadOnly Property LastExitStatus() As Integer
        Get
            Return _lastExitStatus
        End Get
    End Property


    ''' <summary>
    ''' <c>True</c>（既定）のとき、複数の <see cref="Execute"/> は同一対話シェル上で実行され、
    ''' <c>cd</c> などの状態が引き継がれます。<c>False</c> のときは従来どおりコマンドごとに独立した exec です。
    ''' </summary>
    Public Property UsePersistentShell() As Boolean
        Get
            Return _usePersistentShell
        End Get
        Set(value As Boolean)
            If Not value AndAlso _shell IsNot Nothing Then
                DisposeShellOnly()
            End If
            _usePersistentShell = value
        End Set
    End Property


    ''' <summary>対話シェル経由の <see cref="Execute"/> の待ち合わせ秒数（既定 120）。</summary>
    Public Property ShellCommandTimeoutSeconds() As Integer
        Get
            Return _shellCommandTimeoutSeconds
        End Get
        Set(value As Integer)
            If value < 1 Then value = 1
            If value > 86400 Then value = 86400
            _shellCommandTimeoutSeconds = value
        End Set
    End Property


    Private Sub DisposeShellOnly()
        If _shell IsNot Nothing Then
            Try
                _shell.Dispose()
            Catch
            End Try
            _shell = Nothing
        End If
    End Sub


    Private Sub DisposeConnection()
        DisposeShellOnly()
        If SSHConnection IsNot Nothing Then
            If SSHConnection.IsConnected Then
                SSHConnection.Disconnect()
            End If
            SSHConnection.Dispose()
            SSHConnection = Nothing
        End If
    End Sub


    Private Sub ResetLastResult()
        _lastError = ""
        _lastExitStatus = -1
    End Sub


    Private Sub SetLastErrorFromException(ex As Exception)
        If ex Is Nothing Then Return
        _lastError = ex.GetType().Name & ": " & ex.Message
    End Sub


    ''' <summary>
    ''' <paramref name="Login"/> の内容に従い接続します。
    ''' 失敗時は <see cref="LastError"/> を設定し <c>False</c> を返します（<paramref name="Login"/> が <c>Nothing</c> のときのみ例外）。
    ''' </summary>
    Public Function Open(Login As VbaSshLogin) As Boolean
        ResetLastResult()

        If Login Is Nothing Then
            Throw New ArgumentNullException(NameOf(Login))
        End If
        If String.IsNullOrWhiteSpace(Login.Host) Then
            _lastError = "Host is required."
            Return False
        End If
        If Login.Port < 1 OrElse Login.Port > 65535 Then
            _lastError = "Port must be between 1 and 65535."
            Return False
        End If
        If String.IsNullOrWhiteSpace(Login.UserName) Then
            _lastError = "UserName is required."
            Return False
        End If

        Try
            DisposeConnection()

            Dim useKey As Boolean = Not String.IsNullOrWhiteSpace(Login.PrivateKeyFilePath)

            If useKey Then
                If Not File.Exists(Login.PrivateKeyFilePath) Then
                    _lastError = "Private key file not found: " & Login.PrivateKeyFilePath
                    Return False
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
            Return SSHConnection.IsConnected
        Catch ex As SshAuthenticationException
            SetLastErrorFromException(ex)
            DisposeConnection()
            Return False
        Catch ex As SshConnectionException
            SetLastErrorFromException(ex)
            DisposeConnection()
            Return False
        Catch ex As SocketException
            SetLastErrorFromException(ex)
            DisposeConnection()
            Return False
        Catch ex As ProxyException
            SetLastErrorFromException(ex)
            DisposeConnection()
            Return False
        Catch ex As Exception
            SetLastErrorFromException(ex)
            DisposeConnection()
            Return False
        End Try
    End Function


    Public Function Execute(Command As String) As String
        If SSHConnection Is Nothing OrElse Not SSHConnection.IsConnected Then
            _lastError = "SSH session is not connected. Call Open after setting up VbaSshLogin."
            Throw New InvalidOperationException(_lastError)
        End If

        _lastError = ""
        _lastExitStatus = -1

        Try
            If UsePersistentShell Then
                Return ExecuteOnPersistentShell(If(Command, ""))
            End If

            Using cmd As SshCommand = SSHConnection.CreateCommand(If(Command, ""))
                Dim stdout As String = cmd.Execute()
                _lastExitStatus = cmd.ExitStatus
                If cmd.ExitStatus <> 0 Then
                    If Not String.IsNullOrEmpty(cmd.Error) Then
                        _lastError = "Exit " & cmd.ExitStatus.ToString() & ": " & cmd.Error
                    Else
                        _lastError = "Remote command exited with code " & cmd.ExitStatus.ToString()
                    End If
                End If
                Return stdout
            End Using
        Catch ex As Exception
            SetLastErrorFromException(ex)
            Throw
        End Try
    End Function


    Private Function ExecuteOnPersistentShell(command As String) As String
        Try
            EnsureShellStream()
            Dim marker As String = "__VBASSH_" & Guid.NewGuid().ToString("N") & "__"
            _shell.Write(command & vbLf)
            _shell.Write("echo " & marker & vbLf)
            Return ReadUntilMarker(marker, TimeSpan.FromSeconds(ShellCommandTimeoutSeconds))
        Catch ex As TimeoutException
            SetLastErrorFromException(ex)
            Throw
        End Try
    End Function


    Private Sub EnsureShellStream()
        If _shell IsNot Nothing Then Return
        _shell = SSHConnection.CreateShellStream("dumb", 80, 24, 800, 600, 65536)
        DrainInitialShellOutput()
    End Sub


    Private Sub DrainInitialShellOutput()
        Thread.Sleep(250)
        For i = 1 To 200
            If _shell.DataAvailable Then
                _shell.Read()
                Thread.Sleep(30)
            Else
                Exit For
            End If
        Next
    End Sub


    Private Function ReadUntilMarker(marker As String, timeout As TimeSpan) As String
        Dim sb As New StringBuilder()
        Dim deadline = DateTime.UtcNow.Add(timeout)
        While DateTime.UtcNow < deadline
            If _shell.DataAvailable Then
                Dim chunk As String = _shell.Read()
                If Not String.IsNullOrEmpty(chunk) Then
                    sb.Append(chunk)
                    Dim s As String = sb.ToString()
                    Dim idx As Integer = s.IndexOf(marker, StringComparison.Ordinal)
                    If idx >= 0 Then
                        Return s.Substring(0, idx).TrimEnd()
                    End If
                End If
            Else
                Thread.Sleep(15)
            End If
        End While
        Throw New TimeoutException("Shell command did not complete before timeout (" & CInt(timeout.TotalSeconds) & "s). Output so far: " & sb.ToString())
    End Function


    Public Sub Close()
        DisposeConnection()
        ResetLastResult()
    End Sub

End Class
