# ⚠️ SOLUCIÓN DE PROBLEMAS - Compilación

## 🔴 SI BUILD_AND_ZIP.BAT FALLA

### Usa la versión simple:
```
BUILD_SIMPLE.bat
```

Esta versión es más robusta y evita problemas con PowerShell.

---

## ❌ ERRORES COMUNES

### Error 1: "dotnet no se reconoce como comando"

**Problema:** No tienes .NET SDK instalado

**Solución:**
1. Descarga .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
2. Instala
3. Reinicia el terminal/CMD
4. Verifica: `dotnet --version`

---

### Error 2: "Missing closing '}' in statement block"

**Problema:** Error de sintaxis en PowerShell (BUILD_AND_ZIP.bat)

**Solución:** Usa `BUILD_SIMPLE.bat` en su lugar

---

### Error 3: "No se encuentra Tweaker.csproj"

**Problema:** Estás ejecutando el script desde el lugar incorrecto

**Solución:**
1. Abre CMD en la raíz del proyecto (donde está Tweaker\)
2. O ejecuta con doble click (debe estar en raíz)

---

### Error 4: "Compilación fallida" o errores de código

**Problema:** Hay errores en el código fuente

**Solución:**
1. Abre el proyecto en Visual Studio
2. Compila desde VS para ver los errores
3. Corrige los errores
4. Intenta de nuevo

---

### Error 5: "Acceso denegado" al crear archivos

**Problema:** Permisos insuficientes

**Solución:**
1. Cierra Visual Studio si está abierto
2. Ejecuta el .bat como Administrador:
   - Click derecho → "Ejecutar como administrador"

---

### Error 6: El ZIP no se crea

**Problema:** PowerShell está bloqueado

**Solución Rápida:** Usa `BUILD_SIMPLE.bat`

**Solución Completa:**
```powershell
# Ejecuta esto en PowerShell como Administrador:
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## 🎯 SCRIPTS DISPONIBLES

### ✅ BUILD_SIMPLE.bat (RECOMENDADO si hay problemas)
- Versión simplificada y robusta
- Evita problemas de PowerShell
- Crea ejecutable + ZIP
- Funciona en la mayoría de casos

### 🔧 BUILD_AND_ZIP.bat (Completa)
- Versión completa con más detalles
- Puede fallar en algunos sistemas
- Usa BUILD_SIMPLE.bat si falla

### 🧪 TEST_COMPILACION.bat
- Verifica que todo esté listo
- No compila, solo prueba
- Útil para diagnosticar problemas

### 📦 BUILD_RELEASE.bat
- Solo compila, no crea ZIP
- Útil si solo necesitas el ejecutable

---

## 📝 PASOS DE DIAGNÓSTICO

Si sigues teniendo problemas:

### 1. Verifica .NET SDK
```cmd
dotnet --version
```
Debe mostrar `10.x.x`

### 2. Verifica ubicación
```cmd
dir Tweaker\Tweaker.csproj
```
Debe mostrar el archivo

### 3. Prueba compilación manual
```cmd
dotnet build Tweaker\Tweaker.csproj
```
Si falla, hay errores de código

### 4. Limpia y reinicia
```cmd
dotnet clean
BUILD_SIMPLE.bat
```

---

## 🆘 SI NADA FUNCIONA

### Plan B: Compilar desde Visual Studio

1. Abre el proyecto en Visual Studio
2. Click derecho en proyecto "Tweaker"
3. Selecciona "Publish..."
4. Configura:
   - Target: Folder
   - Configuration: Release
   - Target Runtime: win-x64
   - Deployment Mode: Self-contained
   - Produce single file: Yes
5. Click "Publish"

El ejecutable estará en: `bin\Release\net10.0-windows\win-x64\publish\`

---

## 💡 CONSEJOS

### Para evitar problemas:

✅ Cierra Visual Studio antes de ejecutar scripts
✅ Ejecuta desde CMD (no PowerShell) si hay problemas
✅ Usa BUILD_SIMPLE.bat si BUILD_AND_ZIP.bat falla
✅ Verifica que tienes .NET 10 SDK instalado
✅ Ejecuta desde la raíz del proyecto

---

## 📞 AYUDA ADICIONAL

Si los problemas persisten:

1. **Lee:** `Release\GUIA_COMPILACION_Y_DISTRIBUCION_v2.5.0.md`
2. **Revisa:** Este archivo completo
3. **Prueba:** BUILD_SIMPLE.bat (versión robusta)
4. **GitHub:** https://github.com/Josemcboss/Tweaker/issues

---

## 🎯 RESUMEN RÁPIDO

```
┌─────────────────────────────────────┐
│  ¿QUÉ SCRIPT USAR?                 │
├─────────────────────────────────────┤
│                                     │
│  BUILD_SIMPLE.bat                   │
│  ↓                                  │
│  Versión simple y robusta           │
│  ⭐ ÚSALA SI HAY PROBLEMAS          │
│                                     │
│  BUILD_AND_ZIP.bat                  │
│  ↓                                  │
│  Versión completa con más info      │
│  ⚠️ Puede fallar en algunos casos   │
│                                     │
│  TEST_COMPILACION.bat               │
│  ↓                                  │
│  Solo verifica, no compila          │
│  🧪 Para diagnóstico                │
│                                     │
└─────────────────────────────────────┘
```

---

**Recomendación: Usa BUILD_SIMPLE.bat si tienes cualquier problema.**

Es la versión más confiable y funciona en casi todos los casos.
