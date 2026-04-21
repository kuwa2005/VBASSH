# 手動 QA チェックリスト

改造・リリース前に、可能な範囲で順に確認してください。環境が無い項目はスキップで構いません。

## 1. ビルド

- [ ] Visual Studio または `scripts\build-release.cmd` で **Release** が成功する。
- [ ] `VBASSH\bin\Release\` に **`VbaSSHLibrary.dll`** と、**依存の各 `*.dll`**（`Renci.SshNet.dll`、`BouncyCastle.Cryptography.dll` 等）が揃っている。

## 2. NuGet 復元（クリーン確認）

- [ ] `packages` を削除したうえで `nuget restore VBASSH.sln`（または VS で開いて復元）後、再度 Release ビルドが通る。

## 3. COM 登録（管理者環境）

- [ ] 管理者コマンドプロンプトで、`VbaSSHLibrary.dll` があるディレクトリに移動できる。
- [ ] `RegAsm.exe` で `VbaSSHLibrary.dll` と **`/tlb:`** 付き登録がエラーなく完了する（依存 DLL が同ディレクトリにあること）。

## 4. Excel VBA

- [ ] VBE の **ツール → 参照** で `VbaSSHLibrary` にチェックが付く。
- [ ] 標準モジュールで次が動く（接続先はテスト用の安全なホストに置き換え）。

```vb
Sub QA_Smoke()
    Dim s As New VbaSSHLibrary.VbaSSH
    s.Open "ホスト", 22, "ユーザー", "パスワード"
    Debug.Print s.Execute("echo ok")
    s.Close
End Sub
```

- [ ] （可能なら）**公開鍵認証**: `OpenWithPrivateKey` でテスト用鍵を指定し、`Execute` まで通る。
- [ ] **誤ったパスワード**で `Open` したとき、期待どおり失敗する（将来、エラーメッセージの改善が入る場合はこの項目を更新）。

## 5. ドキュメント

- [ ] `README.md` の手順（ビルド・RegAsm・配布 DLL）が現状の出力と矛盾していない。

## 6. CI（リポジトリ管理者）

- [ ] GitHub Actions の **build** ワークフローが緑になる（`master` への push または PR）。
