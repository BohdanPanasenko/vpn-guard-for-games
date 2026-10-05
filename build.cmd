@echo off
setlocal

set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set "OUT_DIR=%~dp0bin"
set "INSTALL_DIR=%LOCALAPPDATA%\vpn-guard-for-games"

if not exist "%CSC%" (
  echo [build] C# compiler not found at %CSC%
  exit /b 1
)

if not exist "%OUT_DIR%" mkdir "%OUT_DIR%"
"%CSC%" /nologo /target:winexe /optimize+ /out:"%OUT_DIR%\vpnguard.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll "%~dp0src\*.cs" || exit /b 1

if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
copy /y "%OUT_DIR%\vpnguard.exe" "%INSTALL_DIR%\vpnguard.exe" >nul || (
  echo [build] Could not copy to %INSTALL_DIR%: vpnguard.exe is still running.
  echo [build] Close its dialog or the game it launched, or run: Stop-Process -Name vpnguard
  exit /b 1
)

if not exist "%INSTALL_DIR%\config.ini" copy "%~dp0config.ini" "%INSTALL_DIR%\config.ini" >nul

echo.
echo [build] Installed to %INSTALL_DIR%\vpnguard.exe
echo [build] Settings: %INSTALL_DIR%\config.ini
echo [build] Steam launch options:
echo   "%INSTALL_DIR%\vpnguard.exe" %%command%%
