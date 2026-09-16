@echo off
:: Batch para ejecutar los tweaks de sistema como Administrador
echo ============================================================
echo   Optimizando Fortnite: Prioridad CPU + Modo MSI GPU
echo ============================================================
echo.

:: Solicitar elevacion si no es administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Solicitando permisos de administrador...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

:: 1. Prioridad de CPU Alta para el ejecutable de Fortnite
echo [1/3] Configurando prioridad de CPU en el Registro...
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FortniteClient-Win64-Shipping.exe\PerfOptions" /v "CpuPriorityClass" /t REG_DWORD /d 3 /f >nul
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FortniteClient-Win64-Shipping.exe\PerfOptions" /v "IoPriority" /t REG_DWORD /d 2 /f >nul

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FortniteLauncher.exe\PerfOptions" /v "CpuPriorityClass" /t REG_DWORD /d 3 /f >nul
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FortniteLauncher.exe\PerfOptions" /v "IoPriority" /t REG_DWORD /d 2 /f >nul
echo       -> Prioridad de CPU Alta aplicada a Fortnite.

:: 2. Limpieza de cache de shaders de DirectX y NVIDIA
echo [2/3] Limpiando cache de shaders antigua...
del /f /q /s "%LOCALAPPDATA%\D3DSCache\*" >nul 2>&1
del /f /q /s "%LOCALAPPDATA%\NVIDIA\DXCache\*" >nul 2>&1
del /f /q /s "%LOCALAPPDATA%\NVIDIA\GLCache\*" >nul 2>&1
del /f /q /s "%LOCALAPPDATA%\FortniteGame\Saved\D3D12\*" >nul 2>&1
echo       -> Cache de shaders purgada.

:: 3. Configurar MSI Mode para la RTX 5060
echo [3/3] Configurando MSI Mode (Message Signaled Interrupts) para la RTX 5060...
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$gpus = Get-PnpDevice -Class 'Display' -PresentOnly | Where-Object { $_.FriendlyName -like '*NVIDIA*' };" ^
  "foreach ($g in $gpus) {" ^
  "  $id = $g.InstanceId;" ^
  "  $msi = 'HKLM:\SYSTEM\CurrentControlSet\Enum\' + $id + '\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties';" ^
  "  $aff = 'HKLM:\SYSTEM\CurrentControlSet\Enum\' + $id + '\Device Parameters\Interrupt Management\Affinity Policy';" ^
  "  if (-not (Test-Path $msi)) { New-Item -Path $msi -Force | Out-Null };" ^
  "  Set-ItemProperty -Path $msi -Name 'MSISupported' -Value 1 -Type DWord -Force;" ^
  "  if (-not (Test-Path $aff)) { New-Item -Path $aff -Force | Out-Null };" ^
  "  Set-ItemProperty -Path $aff -Name 'DevicePriority' -Value 3 -Type DWord -Force;" ^
  "  Write-Host '      -> RTX 5060 configurada en MSI Mode (Prioridad Alta).' -ForegroundColor Green;" ^
  "}"

echo.
echo ============================================================
echo   Listo! Todos los tweaks fueron aplicados con exito.
echo ============================================================
pause
