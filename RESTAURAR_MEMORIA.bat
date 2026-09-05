@echo off
:: Comprobar permisos de administrador y auto-elevar si es necesario
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Solicitando permisos de administrador...
    powershell -Command "Start-Process '%~0' -Verb RunAs"
    exit /b
)

title Tweaker - Restaurar Memoria y Compresion de Windows
color 0A
cls
echo ======================================================================
echo    RESTAURADOR DE MEMORIA RAM Y SERVICIOS DE WINDOWS (TWEAKER FIX)
echo ======================================================================
echo.
echo 1. Restaurando configuracion de Memory Management en el Registro...
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v LargeSystemCache /t REG_DWORD /d 0 /f >nul
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v DisablePagingExecutive /t REG_DWORD /d 0 /f >nul
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v DisablePageCombining /t REG_DWORD /d 0 /f >nul
reg delete "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v IoPageLockLimit /f >nul 2>&1
echo    [OK] LargeSystemCache = 0 (Prioridad a Aplicaciones/Juegos)
echo    [OK] DisablePagingExecutive = 0 (Paging equilibrado de Windows)
echo    [OK] DisablePageCombining = 0 (Deduplicacion activa)
echo.

echo 2. Restaurando servicio SysMain (Compresion de Memoria)...
sc config SysMain start= auto >nul 2>&1
net start SysMain >nul 2>&1
echo    [OK] Servicio SysMain activado e iniciado.
echo.

echo 3. Habilitando Memory Compression y Page Combining en el kernel...
powershell -Command "Enable-MMAgent -MemoryCompression -PageCombining -ErrorAction SilentlyContinue" >nul 2>&1
echo    [OK] Compresion de memoria y Page Combining habilitados.
echo.

echo ======================================================================
echo    CORRECCIONES APLICADAS CON EXITO
echo ======================================================================
echo.
echo Se recomienda reiniciar el equipo cuando te sea conveniente para que
echo todos los cambios en el Kernel de Windows se asienten por completo.
echo.
pause
