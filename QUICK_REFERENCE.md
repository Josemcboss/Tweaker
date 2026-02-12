# Quick Reference - Sistema de Licencias Tweaker

## 🎯 Para Usuarios

### Activar Tweaker
1. Ejecuta Tweaker
2. Copia tu Hardware ID
3. Solicita llave a soporte
4. Ingresa llave en Tweaker
5. Click "Activar"

### Hardware ID
- Se muestra en ventana de activación
- Ejemplo: `ABC123DEF456`
- Único para tu PC

### Formato de Llave
```
XXXXX-XXXXX-XXXXX-XXXXX
```

### Cambio de Hardware
1. Obtén nuevo Hardware ID
2. Contacta soporte
3. Recibe nueva llave
4. Reactiva

---

## 🔧 Para Administradores

### Generar Llave Perpetua
```bash
cd KeyGenerator
dotnet run
# Opción 1 → Ingresar Hardware ID
```

### Generar Llave Temporal
```bash
cd KeyGenerator
dotnet run
# Opción 2 → Ingresar Hardware ID + Fecha
```

### Modo Batch
```bash
# Perpetua
dotnet run -- --batch ABC123DEF456

# Temporal
dotnet run -- --batch ABC123DEF456 2025-12-31
```

### Flujo de Generación
```
Cliente → Hardware ID → Admin → KeyGenerator → Llave → Cliente
```

---

## 💻 Para Desarrolladores

### Estructura
```
Tweaker/License/           # Módulo de licencias
KeyGenerator/              # Herramienta admin
LicenseTest/              # Tests
```

### Clases Principales
- `HardwareFingerprint`: Genera fingerprint del HW
- `LicenseKeyGenerator`: Genera llaves
- `LicenseValidator`: Valida llaves
- `LicenseStorage`: Almacenamiento seguro
- `LicenseManager`: Gestor central
- `ActivationWindow`: UI de activación

### Integración
```csharp
// En App.xaml.cs OnStartup()
if (!LicenseManager.ValidateLicenseOnStartup())
{
    LicenseManager.ShowActivationWindow(showCancelOption: false);
}
```

### Testing
```bash
cd LicenseTest
dotnet run
```

---

## 📁 Ubicaciones

### Archivo de Licencia
```
%AppData%\Tweaker\.tweaker.lic
```

### Proyectos
```
Tweaker/Tweaker/           # App principal + License/
Tweaker/KeyGenerator/      # Generador admin
Tweaker/LicenseTest/       # Suite de tests
```

### Documentación
```
LICENSE_SYSTEM_DOCUMENTATION.md    # Docs técnicas
ADMIN_LICENSE_GUIDE.md            # Guía admin
USER_ACTIVATION_GUIDE.md          # Guía usuario
README_LICENSE.md                 # README general
QUICK_REFERENCE.md                # Esta hoja
```

---

## 🔐 Seguridad

### Encriptación
- **AES-256**: Llaves y storage
- **SHA-256**: Hardware fingerprint
- **Checksum**: Detección de modificación
- **Firma digital**: Integridad del archivo

### Clave Secreta
```csharp
// LicenseKeyGenerator.cs y LicenseValidator.cs
private const string SECRET_KEY = "TweakerLic2024SecretKey9876543";
```

⚠️ **En producción**: Cambiar y ofuscar

---

## 🐛 Troubleshooting Rápido

### "Llave inválida"
- Verifica formato
- Verifica Hardware ID correcto
- Verifica fecha de expiración

### "Hardware no coincide"
- Hardware cambió
- Generar nueva llave con nuevo HW ID

### KeyGenerator no compila
```bash
cd Tweaker
dotnet build
cd ../KeyGenerator
dotnet build
```

### Tests fallan
```bash
dotnet clean
dotnet build
cd LicenseTest
dotnet run
```

---

## 📊 Componentes del Sistema

### Hardware Fingerprint
```
Placa Madre + CPU + MAC + Disco → SHA-256 → Fingerprint
```

### Llave de Licencia
```
[Fingerprint + Fecha + Metadata] → AES-256 → Llave + Checksum
```

### Validación
```
Llave → Decrypt → Verificar Fingerprint → Verificar Fecha → ✅/❌
```

### Almacenamiento
```
Llave + Datos → JSON → AES-256 → Firma → .tweaker.lic
```

---

## 🔄 Estados de Licencia

| Estado | Descripción | Acción |
|--------|-------------|--------|
| ✅ Válida | Todo OK | Permitir uso |
| ❌ Inválida | No coincide HW | Mostrar activación |
| ⏰ Expirada | Fecha pasada | Solicitar renovación |
| 🚫 Sin licencia | No existe | Mostrar activación |
| ⚠️ Corrupta | Archivo dañado | Solicitar reactivación |

---

## 📞 Contactos de Soporte

### Para Usuarios
- Email: soporte@ejemplo.com
- Web: www.ejemplo.com/soporte

### Para Admins/Devs
- Equipo de desarrollo interno
- Documentación técnica: `LICENSE_SYSTEM_DOCUMENTATION.md`

---

**Versión**: 1.0
**Última actualización**: 2024-02-12
