# 変更履歴

このファイルの形式は [Keep a Changelog](https://keepachangelog.com/ja/1.0.0/) を参考にしています。

## [2.2.0] - 2026-04-21

### 追加

- `VbaSSH.LastError` … 直近の `Open` / `Execute` の失敗メッセージ
- `VbaSSH.LastExitStatus` … 非永続シェル（`UsePersistentShell = False`）の `Execute` における終了コード
- `VbaSshLogin.ClearSecrets()` … パスワード・鍵パス・パスフレーズを空に戻す
- **`VBASSH.Tests`**（MSTest・.NET Framework 4.8）と、CI および `scripts\build-release.cmd` での **`dotnet test`**
- `CONTRIBUTING.md` … `gh issue comment` と PowerShell に関する注意（本文の文字化け防止）

### 変更（破壊的を含む）

- **`VbaSSH.Open` の戻り値を `Boolean` に変更**（成功 `True` / 失敗 `False`）。`login` が `Nothing` のときのみ `ArgumentNullException`
- NuGet を **`packages.config` から `PackageReference`（SSH.NET 2025.1.0）へ移行**
- アセンブリ・ファイル版を **2.2.0.0** に更新

### 修正・運用

- `vcvars` 由来の `Platform` 環境変数により **`dotnet test` の出力パスがずれる**問題への対処（スクリプト内の変数クリア、CI の `env`）
- 配布用 README（Release 同梱 DLL 一覧）、セキュリティ（`ClearSecrets`）、API サンプル（`If Not ssh.Open(...)`）の整備

## [2.0.x 以前]

- v1 系の `Open(host, …)` 形式は **2.0 で廃止**済みです。詳細は `README.md` の移行節を参照してください。
- タグ **`v1.0.0`** は改造前の基準点として記録されています。
