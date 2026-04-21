# VbaSSHLibrary（VBASSH）

Excel（VBA）や VBScript から SSH 接続を行うための **COM コンポーネント**です。`VbaSSH` インスタンスに対して **`Open`（接続）→ `Execute`（コマンド実行）→ `Close`（切断）** の順で呼び出します。

![複数ホストへ接続するイメージ](docs/images/image-826.png)

## 要件

- Windows
- .NET Framework 4.7.2 以降
- Visual Studio（ソースからビルドする場合）

## ビルド

1. リポジトリをクローンする。
2. `VBASSH.sln` を Visual Studio で開く。
3. NuGet パッケージを復元する（`packages/` はリポジトリに含まれないため、初回ビルド前に復元が必要です）。
4. **Release** または **Debug** でビルドする。  
   生成物は `VBASSH\bin\<構成>\` に出力されます（`VbaSSHLibrary.dll` など）。

## COM としての登録（RegAsm）

ActiveX 形式のため、利用前に **`RegAsm.exe`** で登録します。管理者権限のコマンドプロンプトで、**`VbaSSHLibrary.dll` があるディレクトリ**に移動してから実行してください。

![管理者コマンドプロンプトの例](docs/images/image-365.png)

作業ディレクトリの例:

```bat
cd C:\path\to\VbaSSHLibrary
```

![カレントディレクトリの例](docs/images/image-397.png)

登録コマンドの例（32 ビットの .NET Framework 4.x）:

```bat
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe" VbaSSHLibrary.dll /tlb:VbaSSHLibrary.tlb /codebase
```

![RegAsm 実行例](docs/images/image-398.png)

64 ビット用の RegAsm が必要な場合は `Framework64` 配下を使います。ビルド構成（AnyCPU / x64 など）に合わせて選んでください。

.NET Framework が未導入の場合は [4.7.2 オフライン インストーラー](https://support.microsoft.com/ja-jp/help/4054530/microsoft-net-framework-4-7-2-offline-installer-for-windows) を参照してください。

### `RegAsm : error RA0000 : 入力アセンブリ 'VbaSSHLibrary.dll' またはその依存関係の 1 つが見つかりません。`

`VbaSSHLibrary.dll` と同じフォルダに **`Renci.SshNet.dll`**（SSH.NET）など依存 DLL があるか、**カレントディレクトリが DLL のある場所か**を確認してください。

![エラー表示の例](docs/images/image-406.png)

![DLL 所在フォルダで再実行](docs/images/image-408.png)

## Excel VBA での参照設定

VBE で **ツール → 参照** を開き、`VbaSSHLibrary.tlb` を追加し、**VbaSSHLibrary** にチェックが付いていることを確認します。

![参照設定の例](docs/images/image-399.png)

## API の使い方（例）

```vb
Public Sub Example()
    Dim ssh As New VbaSSHLibrary.VbaSSH
    ssh.Open "192.168.0.10", 22, "user", "password"
    Debug.Print ssh.Execute("uname -a")
    ssh.Close
End Sub
```

複数接続の例:

```vb
Public Sub MultipleSessions()
    Dim s1 As New VbaSSHLibrary.VbaSSH
    Dim s2 As New VbaSSHLibrary.VbaSSH

    s1.Open "192.168.0.100", 22, "user1", "pass1"
    s2.Open "192.168.0.101", 22, "user2", "pass2"

    Debug.Print s1.Execute("cd /; ls -la")
    Debug.Print s2.Execute("tar cvf backup.tar *.php")

    s1.Close
    s2.Close
End Sub
```

`Open` の第 2 引数は **ポート番号（整数）** です。

## `Execute` が期待どおり動かないとき

手動の SSH クライアントでは成功するが、本ライブラリ経由では失敗する場合は、リモートの **ログインシェル・環境変数・非対話実行の制限**などが原因になることがあります。

## 技術スタック

- Visual Basic .NET（.NET Framework 4.7.2）
- [SSH.NET](https://github.com/sshnet/SSH.NET)（`Renci.SshNet`）

## ドキュメント用画像

README 用の画面キャプチャは `docs/images/` に置いています。
