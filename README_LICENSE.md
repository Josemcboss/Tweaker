# Sistema de Licencias Tweaker

## 📋 Resumen

Sistema completo de licencias implementado para Tweaker que permite:
- ✅ Activación con llave única vinculada al hardware
- ✅ Licencias perpetuas o temporales
- ✅ Validación automática al inicio
- ✅ Almacenamiento seguro cifrado
- ✅ Herramienta administrativa para generar llaves

## 🚀 Características

### Para Usuarios
- Ventana de activación intuitiva
- Muestra Hardware ID para soporte
- Auto-formato de llave con guiones
- Mensajes claros de error/éxito
- Validación automática en cada inicio

### Para Administradores
- Herramienta CLI para generar llaves
- Soporte para licencias perpetuas y temporales
- Modo interactivo y modo batch
- Documentación completa

## 📁 Estructura del Proyecto

```
Tweaker/
├── License/                          # Módulo de licencias
│   ├── HardwareFingerprint.cs       # Generación de fingerprint del hardware
│   ├── LicenseData.cs               # Modelo de datos de licencia
│   ├── LicenseKeyGenerator.cs       # Generador de llaves (para admins)
│   ├── LicenseValidator.cs          # Validador de llaves
│   ├── LicenseStorage.cs            # Almacenamiento seguro
│   ├── ActivationWindow.xaml        # UI de activación
│   ├── ActivationWindow.xaml.cs     # Lógica de activación
│   └── LicenseManager.cs            # Gestor central

KeyGenerator/                         # ⚠️ Herramienta CONFIDENCIAL
├── Program.cs                        # CLI para generar llaves
└── KeyGenerator.csproj

LicenseTest/                          # Tests del sistema
├── Program.cs                        # Suite de pruebas
└── LicenseTest.csproj

LICENSE_SYSTEM_DOCUMENTATION.md       # Documentación técnica completa
ADMIN_LICENSE_GUIDE.md               # Guía para administradores
README_LICENSE.md                     # Este archivo
```

## 🔧 Uso

### Para Desarrolladores

**Integración en la aplicación:**

La integración ya está completa en `App.xaml.cs`. Al iniciar Tweaker:
1. Se valida la licencia automáticamente
2. Si no hay licencia válida, se muestra la ventana de activación
3. El usuario debe activar antes de usar el programa

**Probar el sistema:**

```bash
cd LicenseTest
dotnet run
```

Esto ejecutará una suite completa de tests que valida:
- Generación de hardware fingerprint
- Generación de llaves
- Validación de llaves
- Almacenamiento y recuperación
- Gestión con LicenseManager

### Para Administradores

**Generar llaves:**

```bash
cd KeyGenerator
dotnet run
```

Sigue el menú interactivo para generar llaves.

**Ver documentación completa:**
- `ADMIN_LICENSE_GUIDE.md` - Guía paso a paso
- `LICENSE_SYSTEM_DOCUMENTATION.md` - Documentación técnica

## 🔐 Seguridad

### Implementaciones de Seguridad

✅ **Encriptación AES-256** para llaves y almacenamiento
✅ **Hash SHA-256** para hardware fingerprint
✅ **Checksum** en llaves para detectar modificación
✅ **Firma digital** en archivo de licencia
✅ **Validación estricta** de formato y coincidencia de hardware

### Recomendaciones Adicionales

Para producción se recomienda:
- Ofuscar el código con herramientas especializadas
- Proteger la clave secreta con key derivation
- Implementar detección anti-tampering
- Usar compilación optimizada

## 📝 Formato de Llave

```
XXXXX-XXXXX-XXXXX-XXXXX
```

- 20 caracteres alfanuméricos
- 4 grupos de 5 caracteres
- Separados por guiones
- Mayúsculas

Ejemplo: `AB12C-D34EF-G56HI-J78KL`

## 🔄 Flujo de Activación

1. **Usuario inicia Tweaker**
2. **Sistema verifica licencia**
   - Si existe y es válida → Continuar
   - Si no existe o es inválida → Mostrar ventana de activación
3. **Usuario obtiene Hardware ID** (mostrado en ventana)
4. **Usuario contacta soporte** y envía Hardware ID
5. **Admin genera llave** con KeyGenerator
6. **Usuario ingresa llave** en Tweaker
7. **Sistema valida y activa**
8. **Licencia guardada** cifrada en disco
9. **Tweaker activado** ✅

## 💾 Almacenamiento

**Ubicación del archivo de licencia:**
```
%AppData%\Tweaker\.tweaker.lic
```

**Formato:** JSON cifrado con AES-256

**Contenido:**
- Llave de licencia
- Hardware fingerprint
- Fecha de expiración (si aplica)
- Firma digital

## 🛠️ Mantenimiento

### Cambio de Hardware

Si el usuario cambia componentes significativos:
- La licencia dejará de funcionar
- Sistema mostrará nuevo Hardware ID
- Usuario debe solicitar nueva licencia con nuevo Hardware ID

### Renovación de Licencia Temporal

1. Verificar fecha de expiración en sistema de gestión
2. Generar nueva llave con nueva fecha
3. Usuario ingresa nueva llave (reemplaza anterior)

## 📚 Documentación

- **[LICENSE_SYSTEM_DOCUMENTATION.md](LICENSE_SYSTEM_DOCUMENTATION.md)** - Documentación técnica completa del sistema
- **[ADMIN_LICENSE_GUIDE.md](ADMIN_LICENSE_GUIDE.md)** - Guía para administradores (CONFIDENCIAL)

## ⚠️ Importante

- **NO DISTRIBUIR** KeyGenerator a usuarios finales
- **MANTENER PRIVADA** la documentación administrativa
- **REGISTRAR** todas las licencias generadas
- **VERIFICAR IDENTIDAD** antes de generar llaves

## 🧪 Testing

```bash
# Ejecutar tests del sistema de licencias
cd LicenseTest
dotnet run

# Generar llave de prueba
cd KeyGenerator
dotnet run
```

## 📞 Soporte

Para problemas con el sistema de licencias:
1. Verificar Hardware ID del cliente
2. Verificar formato de llave
3. Revisar fecha de expiración
4. Consultar logs de activación
5. Generar nueva llave si es necesario

---

**Versión:** 1.0
**Última actualización:** 2024-02-12
