# 手動 QA チェックリスト

改造・リリース前に、可能な範囲で順に確認してください。環境が無い項目はスキップで構いません。

## 1. ビルド

- [ ] Visual Studio または `scripts\build-release.cmd` で **Release** が成功する（スクリプトは **ビルド後に `dotnet test`** まで実行する）。
- [ ] `dotnet test VBASSH.Tests\VBASSH.Tests.vbproj -c Release --no-build`（またはビルド直後に `--no-build` なし）で **単体テストがすべて成功**する。
- [ ] `VBASSH\bin\Release\` に **`VbaSSHLibrary.dll`** と、**依存の各 `*.dll`**（`Renci.SshNet.dll`、`BouncyCastle.Cryptography.dll` 等）が揃っている。

## 2. NuGet 復元（クリーン確認）

- [ ] `bin` / `obj` を削除するかプロジェクトをクリーンしたうえで、`msbuild VBASSH.sln /t:Restore` または `nuget restore VBASSH.sln`（または VS で開いて復元）後、再度 Release ビルドが通る。

## 3. COM 登録（管理者環境）

- [ ] 管理者コマンドプロンプトで、`VbaSSHLibrary.dll` があるディレクトリに移動できる。
- [ ] `RegAsm.exe` で `VbaSSHLibrary.dll` と **`/tlb:`** 付き登録がエラーなく完了する（依存 DLL が同ディレクトリにあること）。

## 4. Excel VBA

- [ ] VBE の **ツール → 参照** で `VbaSSHLibrary` にチェックが付く。
- [ ] 標準モジュールで次が動く（接続先はテスト用の安全なホストに置き換え）。

```vb
Sub QA_Smoke()
    Dim L As New VbaSSHLibrary.VbaSshLogin
    L.Host = "ホスト"
    L.Port = 22
    L.UserName = "ユーザー"
    L.Password = "パスワード"

    Dim s As New VbaSSHLibrary.VbaSSH
    If Not s.Open(L) Then
        Debug.Print s.LastError
        Exit Sub
    End If
    Debug.Print s.Execute("echo ok")
    s.Close
End Sub
```

- [ ] （可能なら）**秘密鍵認証**: `VbaSshLogin` に `PrivateKeyFilePath` を設定し、`Open` から `Execute` まで通る。
- [ ] **対話シェル**: `Execute "cd …"` のあとに `Execute "pwd"` し、カレントが変わっていること（既定 `UsePersistentShell=True`）。
- [ ] **誤ったパスワード**で `Open` したとき **`False`** を返し、**`LastError`** に内容が入ること。

## 5. ドキュメント

- [ ] `README.md` の手順（ビルド・RegAsm・配布 DLL）が現状の出力と矛盾していない。

## 6. CI（リポジトリ管理者）

- [ ] GitHub Actions の **build** ワークフローが緑になる（`master` への push または PR）。
