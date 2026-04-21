@echo off
setlocal
rem Release 成果物の SHA256 を表示する（GitHub Releases の本文に貼る用）
pushd "%~dp0..\VBASSH\bin\Release"
if not exist "VbaSSHLibrary.dll" (
  echo VbaSSHLibrary.dll が見つかりません。先に Release ビルドを実行してください。
  popd
  exit /b 1
)
echo === SHA256 (certutil) ===
for %%F in (*.dll) do (
  echo.
  echo %%F
  certutil -hashfile "%%F" SHA256
)
popd
exit /b 0
