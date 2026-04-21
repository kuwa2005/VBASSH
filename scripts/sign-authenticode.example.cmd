@echo off
setlocal EnableExtensions
rem =============================================================================
rem Authenticode（コード署名）の例 — 本ファイルを sign-authenticode.cmd にコピーし、
rem 値を埋めてから実行してください。リポジトリには秘密（PFX パスワード等）を含めないこと。
rem 使用ツール: Windows SDK の signtool.exe（Visual Studio インストールに含まれる）
rem =============================================================================

set SIGNTOOL="C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\signtool.exe"
if not exist %SIGNTOOL% (
  echo signtool のパスを環境に合わせて修正してください。
  exit /b 1
)

rem コード署名用 PFX（公開鍵証明書で取得したファイル）
set PFX_PATH=C:\path\to\codesign.pfx

rem タイムスタンプ（失効後も署名が検証できる）
set TIMESTAMP_URL=http://timestamp.digicert.com

rem 署名対象（ビルド出力に合わせる）
set DLL=%~dp0..\VBASSH\bin\Release\VbaSSHLibrary.dll

if not exist "%DLL%" (
  echo 先に Release ビルドを実行してください: %DLL%
  exit /b 1
)

rem パスワードは環境変数で渡す（setx や CI のシークレット推奨）
if "%SIGN_PFX_PASSWORD%"=="" (
  echo 環境変数 SIGN_PFX_PASSWORD に PFX のパスワードを設定してから再実行してください。
  exit /b 1
)

%SIGNTOOL% sign /fd SHA256 /tr %TIMESTAMP_URL% /td SHA256 /f "%PFX_PATH%" /p "%SIGN_PFX_PASSWORD%" "%DLL%"
if errorlevel 1 exit /b 1

echo 検証:
%SIGNTOOL% verify /pa /v "%DLL%"
exit /b 0
