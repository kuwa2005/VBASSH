# メンテナ向け: GitHub Release の作り方

利用者向けの説明は **`README.md`**。ここでは **リリース担当者**が GitHub 上で版を切るときの手順をまとめます。

## 1. バージョンの決定

- **`VBASSH\My Project\AssemblyInfo.vb`** の `AssemblyVersion` / `AssemblyFileVersion` を更新する。
- **`CHANGELOG.md`** に日付付きで節を追加する。

## 2. ビルドと検証

1. **`scripts\build-release.cmd`** を実行する（Release ビルドのあと **`dotnet test`** まで走る）。
2. 可能なら **`docs\QA-CHECKLIST.md`** の手動項目も確認する。

## 3. 配布 ZIP（任意・推奨）

リポジトリの **`.gitignore` で `*.zip` を除外**しているため、ZIP は Git にコミットしません。ローカルで作り、Release に添付します。

1. **`VBASSH\bin\Release`** に、`README.md` の「Release フォルダに同梱するファイル」に挙げた **各 `*.dll`** が揃っていることを確認する。
2. **リポジトリのルート**で、次のように **同梱物だけ**を ZIP にまとめる（例: PowerShell）。

```powershell
$rel = "VBASSH\bin\Release"
$out = Join-Path (Get-Location) "VbaSSHLibrary-2.2.0-bin.zip"
if (Test-Path $out) { Remove-Item $out }
$items = @(Get-ChildItem "$rel\*.dll" | ForEach-Object { $_.FullName })
$items += (Join-Path (Resolve-Path $rel) "VbaSSHLibrary.xml")
Compress-Archive -LiteralPath $items -DestinationPath $out
```

（リポジトリ**ルート**で実行。PowerShell 7 以降では `-Path` に複数パスを渡すと失敗することがあるため **`-LiteralPath` に配列**を渡す。バージョン番号はタグ名に合わせて読み替えてください。）

## 4. SHA256（Release 本文用）

```bat
scripts\compute-release-hashes.cmd
```

表示された **SHA256 を Release 本文**に貼り付けると、利用者が改ざん検知しやすくなります（`README.md` の SmartScreen 節も参照）。

## 5. Git に反映してタグを打つ

```bat
git add -A
git commit -m "Release 2.2.0: changelog and docs"
git push origin master
git tag -a v2.2.0 -m "VbaSSHLibrary 2.2.0"
git push origin v2.2.0
```

（タグ名は **`CHANGELOG.md` / アセンブリ版と一致**させる。）

## 6. GitHub Release の作成

事前に **`gh auth login`** 済みであること。

```bat
gh release create v2.2.0 --title "VbaSSHLibrary 2.2.0" --notes-file RELEASE_NOTES.md VbaSSHLibrary-2.2.0-bin.zip
```

- **`RELEASE_NOTES.md`** … `CHANGELOG.md` の該当版をコピーし、必要なら SHA256 ブロックを追記したファイル（リポジトリに残さず一時ファイルでもよい）。
- ZIP を付けない場合は、ファイル引数を省略する。

既に同名タグ／Release がある場合は **`gh release delete` / `git tag -d`** で削除してからやり直すか、パッチ版 **`v2.2.1`** として切り直してください。
