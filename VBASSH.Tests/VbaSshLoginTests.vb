Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports VBASSH

<TestClass>
Public NotInheritable Class VbaSshLoginTests

    <TestMethod>
    Public Sub DefaultPort_Is22()
        Dim login As New VbaSshLogin()
        Assert.AreEqual(22, login.Port)
    End Sub

    <TestMethod>
    Public Sub ClearSecrets_ClearsPasswordKeyPathAndPassphrase()
        Dim login As New VbaSshLogin()
        login.Password = "x"
        login.PrivateKeyFilePath = "C:\key"
        login.PrivateKeyPassphrase = "p"
        login.ClearSecrets()
        Assert.AreEqual("", login.Password)
        Assert.AreEqual("", login.PrivateKeyFilePath)
        Assert.AreEqual("", login.PrivateKeyPassphrase)
    End Sub

    <TestMethod>
    Public Sub Password_SetNothing_BecomesEmpty()
        Dim login As New VbaSshLogin()
        login.Password = Nothing
        Assert.AreEqual("", login.Password)
    End Sub

End Class
