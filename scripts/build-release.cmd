@echo off
setlocal
rem リポジトリルート（本ファイルの親の親）
pushd "%~dp0.."
if not exist "VBASSH.sln" (
  echo VBASSH.sln が見つかりません。スクリプトの配置を確認してください。
  popd
  exit /b 1
)

rem ビルド環境（x64）。インストール先が異なる場合はパスを読み替えてください。
call "C:\Program Files\Microsoft Visual Studio\18\Community\VC\Auxiliary\Build\vcvars64.bat"
if errorlevel 1 (
  echo vcvars64.bat の読み込みに失敗しました。
  popd
  exit /b 1
)

rem COM のレジストリ登録はビルド時に行わない（管理者権限不要）。配布時は RegAsm を別途実行。
rem vcvars64 により既定の Platform が x64 になることがあるため、ソリューションの「Any CPU」を明示
"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "VBASSH.sln" /t:Restore,Build /p:Configuration=Release /p:Platform="Any CPU" /p:RegisterForComInterop=false /v:m
set ERR=%ERRORLEVEL%
if not %ERR%==0 (
  popd
  exit /b %ERR%
)

where dotnet >nul 2>nul
if errorlevel 1 (
  echo [エラー] dotnet コマンドが PATH にありません。.NET SDK を含む環境で実行するか、PATH を通してください。
  popd
  exit /b 1
)

rem vcvars が Platform=x64 を付けると、MSBuild（Any CPU）の出力パスと dotnet test の解決がずれるため一旦クリアする
set "Platform="
set "PlatformName="

dotnet test "VBASSH.Tests\VBASSH.Tests.vbproj" -c Release --no-build --verbosity minimal
set ERR=%ERRORLEVEL%
popd
exit /b %ERR%
