# Sistema de Licencias de Tweaker

## Descripción General

Este documento describe el sistema completo de licencias implementado en Tweaker, que permite activar el programa con una llave única vinculada al hardware de la computadora.

## Componentes del Sistema

### 1. Generación de Hardware Fingerprint (`HardwareFingerprint.cs`)

El sistema genera un identificador único del hardware basado en:
- **UUID de la placa madre**: Identifica la motherboard
- **Número de serie del procesador**: Identifica el CPU
- **Dirección MAC**: De la interfaz de red principal
- **Número de serie del disco**: Del disco duro/SSD

Estos componentes se combinan y se genera un hash SHA-256 para crear un fingerprint único de 32 caracteres.

**Métodos principales:**
- `GetFingerprint()`: Obtiene el fingerprint completo
- `GetDisplayFingerprint()`: Obtiene los primeros 16 caracteres para mostrar al usuario

### 2. Datos de Licencia (`LicenseData.cs`)

Clase que representa una licencia con:
- Hardware fingerprint autorizado
- Fecha de expiración (opcional, puede ser perpetua)
- Fecha de creación
- Propiedades de validación (IsExpired, IsValid, IsPerpetual)

### 3. Generador de Llaves (`LicenseKeyGenerator.cs`)

**⚠️ HERRAMIENTA ADMINISTRATIVA - NO DISTRIBUIR**

Genera llaves de licencia válidas en formato `XXXXX-XXXXX-XXXXX-XXXXX` (20 caracteres en grupos de 5).

La llave contiene:
- Información encriptada del hardware fingerprint autorizado
- Fecha de expiración (si aplica)
- Checksum para validación

Usa encriptación AES-256 para proteger la información.

**Uso:**
```csharp
var licenseKey = LicenseKeyGenerator.GenerateKey(hardwareFingerprint, expirationDate);
```

### 4. Validador de Licencias (`LicenseValidator.cs`)

Valida llaves de licencia verificando:
1. Formato correcto (XXXXX-XXXXX-XXXXX-XXXXX)
2. Checksum válido
3. Desencriptación exitosa
4. Coincidencia de hardware fingerprint
5. Fecha de expiración (si aplica)

**Uso:**
```csharp
var licenseData = LicenseValidator.ValidateLicenseKey(licenseKey, currentHardwareFingerprint);
if (licenseData != null && licenseData.IsValid)
{
    // Licencia válida
}
```

### 5. Almacenamiento Seguro (`LicenseStorage.cs`)

Guarda y recupera licencias de forma segura:
- **Ubicación**: `%AppData%\Tweaker\.tweaker.lic`
- **Protección**: Archivo cifrado con AES
- **Integridad**: Firma digital para detectar modificaciones

**Métodos principales:**
- `SaveLicense()`: Guarda una licencia activada
- `LoadLicense()`: Carga la licencia guardada
- `DeleteLicense()`: Elimina la licencia
- `HasSavedLicense()`: Verifica si existe una licencia guardada

### 6. Ventana de Activación (`ActivationWindow.xaml/cs`)

Interfaz gráfica para activación:
- Muestra el Hardware ID actual (para soporte)
- Campo para ingresar llave de licencia
- Auto-formateo con guiones
- Validación en tiempo real
- Mensajes de estado claros

### 7. Gestor de Licencias (`LicenseManager.cs`)

Componente central que coordina todo el sistema:
- `ValidateLicenseOnStartup()`: Valida al iniciar la aplicación
- `ShowActivationWindow()`: Muestra diálogo de activación
- `EnsureValidLicense()`: Asegura que hay licencia válida
- `DeactivateLicense()`: Desactiva la licencia actual
- `GetLicenseInfo()`: Obtiene información para mostrar

## Flujo de Usuario

### Primera Ejecución

1. Usuario inicia Tweaker
2. `App.xaml.cs` llama a `LicenseManager.ValidateLicenseOnStartup()`
3. No se detecta licencia válida
4. Se muestra diálogo de activación con Hardware ID
5. Usuario introduce llave proporcionada
6. Sistema valida y activa
7. Licencia se guarda en archivo cifrado
8. Programa continúa normalmente

### Ejecuciones Posteriores

1. Usuario inicia Tweaker
2. Sistema carga y valida licencia automáticamente
3. Verifica que hardware fingerprint coincida
4. Si es válida, programa continúa
5. Si falló validación, solicita reactivación

## Herramienta de Generación de Llaves

### KeyGenerator (Consola)

Herramienta administrativa separada para generar llaves.

**Ubicación**: `KeyGenerator/Program.cs`

**Modo Interactivo:**
```bash
dotnet run --project KeyGenerator
```

Menú interactivo con opciones:
1. Generar llave perpetua
2. Generar llave con fecha de expiración
3. Salir

**Modo Batch:**
```bash
# Llave perpetua
dotnet run --project KeyGenerator -- --batch ABC123DEF456

# Llave con expiración
dotnet run --project KeyGenerator -- --batch ABC123DEF456 2025-12-31
```

### Proceso de Generación para Administradores

1. Cliente solicita licencia
2. Cliente ejecuta Tweaker y obtiene su Hardware ID
3. Cliente envía Hardware ID al administrador
4. Administrador ejecuta KeyGenerator
5. Ingresa Hardware ID del cliente
6. Selecciona tipo de licencia (perpetua o con fecha)
7. KeyGenerator genera llave única
8. Administrador envía llave al cliente
9. Cliente ingresa llave en Tweaker

## Manejo de Cambios de Hardware

Si el hardware cambia significativamente:
- La licencia dejará de funcionar
- Sistema muestra mensaje explicativo
- Muestra nuevo Hardware ID
- Usuario debe solicitar nueva licencia

**Tolerancia:**
El sistema actualmente NO permite cambios de hardware. Cualquier cambio invalidará la licencia. Esto puede modificarse en el futuro para permitir X cambios menores.

## Seguridad

### Implementadas

1. **Encriptación AES-256** de llaves y archivo de licencia
2. **Hash SHA-256** para hardware fingerprint
3. **Checksum** en llaves para detectar modificación
4. **Firma digital** en archivo de licencia guardada
5. **Validación estricta** de formato y datos

### Recomendaciones Adicionales

Para mayor seguridad en producción:

1. **Ofuscar código**: Usar herramientas como ConfuserEx o .NET Reactor
2. **Proteger clave secreta**: Usar key derivation o almacenamiento seguro
3. **Detección anti-tampering**: Detectar debuggers y modificaciones
4. **PyInstaller/compilación**: Compilar con opciones de ofuscación

## Logging

El sistema registra eventos importantes:
- Intentos de activación (exitosos y fallidos)
- Validaciones de licencia
- Cambios de hardware detectados

**Ubicación de logs**: Se puede implementar en el futuro.

## Consideraciones de Desarrollo

### Para Modificar el Sistema

1. **Cambiar algoritmo de encriptación**: Modificar `LicenseKeyGenerator.cs` y `LicenseValidator.cs`
2. **Agregar tolerancia a cambios de hardware**: Implementar sistema de puntos en `HardwareFingerprint.cs`
3. **Modo trial**: Agregar lógica en `LicenseManager.cs`
4. **Servidor de activación**: Crear API REST para validación en línea

### Testing

Actualmente no hay tests unitarios implementados. Se recomienda agregar tests para:
- Generación de fingerprints
- Generación y validación de llaves
- Encriptación/desencriptación
- Almacenamiento y recuperación

## Troubleshooting

### Usuario reporta "Licencia inválida"

1. Verificar que el Hardware ID coincide
2. Verificar formato de llave (XXXXX-XXXXX-XXXXX-XXXXX)
3. Verificar fecha de expiración
4. Re-generar llave si es necesario

### Licencia funciona pero deja de funcionar

1. Verificar cambios de hardware
2. Verificar archivo de licencia no corrupto (`%AppData%\Tweaker\.tweaker.lic`)
3. Solicitar nueva licencia si hardware cambió

### No se puede guardar licencia

1. Verificar permisos de escritura en `%AppData%`
2. Verificar que directorio Tweaker existe
3. Verificar antivirus no bloquea archivo `.lic`

## Archivos del Sistema

```
Tweaker/
├── License/
│   ├── HardwareFingerprint.cs      # Generación de fingerprint
│   ├── LicenseData.cs               # Modelo de datos
│   ├── LicenseKeyGenerator.cs       # Generador de llaves
│   ├── LicenseValidator.cs          # Validador de llaves
│   ├── LicenseStorage.cs            # Almacenamiento seguro
│   ├── ActivationWindow.xaml        # UI de activación
│   ├── ActivationWindow.xaml.cs     # Lógica de UI
│   └── LicenseManager.cs            # Gestor central
├── App.xaml.cs                      # Integración en startup
└── ...

KeyGenerator/
├── Program.cs                       # Herramienta admin
└── KeyGenerator.csproj              # Proyecto consola
```

## Ubicación de Datos

- **Archivo de licencia**: `%AppData%\Tweaker\.tweaker.lic`
- **Formato**: JSON cifrado con AES
- **Permisos**: Usuario actual

## Soporte

Para problemas con licencias:
1. Obtener Hardware ID del cliente
2. Verificar licencia en sistema de administración
3. Generar nueva llave si es necesario
4. Logs de activación pueden ayudar a diagnosticar

---

**Última actualización**: 2024-02-12
**Versión del sistema**: 1.0
