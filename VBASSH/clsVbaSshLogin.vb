Imports System
Imports System.Runtime.InteropServices


''' <summary>
''' SSH 接続のログイン情報。COM から <c>New VbaSSHLibrary.VbaSshLogin</c> で生成し、
''' プロパティを設定したうえで <see cref="VbaSSH.Open"/> に渡します。
''' </summary>
<ComClass(VbaSshLogin.ClassId, VbaSshLogin.InterfaceId, VbaSshLogin.EventsId)>
Public Class VbaSshLogin

    Public Const ClassId As String = "8F2A4B1C-3D5E-4678-9ABC-DEF012345678"
    Public Const InterfaceId As String = "9E3B5C2D-4F6A-4789-BCDE-F0123456789A"
    Public Const EventsId As String = "AF4C6D3E-5A7B-4890-CDEF-0123456789AB"

    Private _host As String
    Private _port As Integer
    Private _userName As String
    Private _password As String
    Private _privateKeyFilePath As String
    Private _privateKeyPassphrase As String

    Public Sub New()
        _port = 22
        _password = ""
        _privateKeyFilePath = ""
        _privateKeyPassphrase = ""
    End Sub

    Public Property Host() As String
        Get
            Return _host
        End Get
        Set(value As String)
            _host = value
        End Set
    End Property

    Public Property Port() As Integer
        Get
            Return _port
        End Get
        Set(value As Integer)
            _port = value
        End Set
    End Property

    Public Property UserName() As String
        Get
            Return _userName
        End Get
        Set(value As String)
            _userName = value
        End Set
    End Property

    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(value As String)
            _password = If(value, "")
        End Set
    End Property

    Public Property PrivateKeyFilePath() As String
        Get
            Return _privateKeyFilePath
        End Get
        Set(value As String)
            _privateKeyFilePath = If(value, "")
        End Set
    End Property

    Public Property PrivateKeyPassphrase() As String
        Get
            Return _privateKeyPassphrase
        End Get
        Set(value As String)
            _privateKeyPassphrase = If(value, "")
        End Set
    End Property

End Class
