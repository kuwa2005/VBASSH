Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports VBASSH

<TestClass>
Public NotInheritable Class VbaSSHOpenValidationTests

    <TestMethod>
    Public Sub Open_NothingLogin_ThrowsArgumentNullException()
        Dim ssh As New VbaSSH()
        Assert.ThrowsException(Of ArgumentNullException)(
            Sub() ssh.Open(Nothing))
    End Sub

    <TestMethod>
    Public Sub Open_EmptyHost_ReturnsFalse()
        Dim ssh As New VbaSSH()
        Dim login As New VbaSshLogin()
        login.Host = ""
        login.UserName = "u"
        Assert.IsFalse(ssh.Open(login))
        Assert.IsTrue(ssh.LastError.IndexOf("Host", StringComparison.OrdinalIgnoreCase) >= 0)
    End Sub

    <TestMethod>
    Public Sub Open_InvalidPort_ReturnsFalse()
        Dim ssh As New VbaSSH()
        Dim login As New VbaSshLogin()
        login.Host = "127.0.0.1"
        login.Port = 0
        login.UserName = "u"
        Assert.IsFalse(ssh.Open(login))
        Assert.IsTrue(ssh.LastError.IndexOf("Port", StringComparison.OrdinalIgnoreCase) >= 0)
    End Sub

    <TestMethod>
    Public Sub Open_EmptyUserName_ReturnsFalse()
        Dim ssh As New VbaSSH()
        Dim login As New VbaSshLogin()
        login.Host = "127.0.0.1"
        login.UserName = "  "
        Assert.IsFalse(ssh.Open(login))
        Assert.IsTrue(ssh.LastError.IndexOf("User", StringComparison.OrdinalIgnoreCase) >= 0)
    End Sub

    <TestMethod>
    Public Sub Execute_WithoutOpen_ThrowsInvalidOperationException()
        Dim ssh As New VbaSSH()
        Assert.ThrowsException(Of InvalidOperationException)(
            Sub() ssh.Execute("echo"))
    End Sub

End Class
