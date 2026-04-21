Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports VBASSH

<TestClass>
Public NotInheritable Class VbaSSHPropertyTests

    <TestMethod>
    Public Sub ShellCommandTimeoutSeconds_ClampedToMinimum1()
        Dim ssh As New VbaSSH()
        ssh.ShellCommandTimeoutSeconds = 0
        Assert.AreEqual(1, ssh.ShellCommandTimeoutSeconds)
        ssh.ShellCommandTimeoutSeconds = -5
        Assert.AreEqual(1, ssh.ShellCommandTimeoutSeconds)
    End Sub

    <TestMethod>
    Public Sub ShellCommandTimeoutSeconds_ClampedToMaximum86400()
        Dim ssh As New VbaSSH()
        ssh.ShellCommandTimeoutSeconds = 200000
        Assert.AreEqual(86400, ssh.ShellCommandTimeoutSeconds)
    End Sub

    <TestMethod>
    Public Sub UsePersistentShell_DefaultTrue()
        Dim ssh As New VbaSSH()
        Assert.IsTrue(ssh.UsePersistentShell)
    End Sub

    <TestMethod>
    Public Sub Close_WhenNeverOpened_DoesNotThrow()
        Dim ssh As New VbaSSH()
        ssh.Close()
        ssh.Close()
    End Sub

End Class
