# Notas de Seguridad - Sistema de Licencias

## ⚠️ IMPORTANTE: Preparación para Producción

Este documento describe las consideraciones de seguridad adicionales que deben implementarse antes de desplegar el sistema de licencias en producción.

## 🔒 Hardcoded Secret Keys (Prioridad ALTA)

### Problema Actual

Las claves secretas para encriptación AES están hardcoded en:
- `Tweaker/License/LicenseKeyGenerator.cs` (línea 13)
- `Tweaker/License/LicenseValidator.cs` (línea 14)

```csharp
private const string SECRET_KEY = "TweakerLic2024SecretKey9876543";
```

### Riesgos

✗ Cualquiera con acceso al código puede ver la clave
✗ Ingeniería inversa puede exponer la clave
✗ No hay rotación de claves
✗ Compromiso de la clave compromete todas las licencias

### Soluciones Recomendadas

#### Opción 1: Key Derivation Function (KDF)

Usar PBKDF2 o Argon2 para derivar la clave desde una seed:

```csharp
using System.Security.Cryptography;

public static byte[] DeriveKey(string seed, byte[] salt)
{
    using (var deriveBytes = new Rfc2898DeriveBytes(seed, salt, 100000, HashAlgorithmName.SHA256))
    {
        return deriveBytes.GetBytes(32); // 256 bits
    }
}
```

#### Opción 2: Azure Key Vault / AWS KMS

Para producción enterprise:
- Almacenar clave en Azure Key Vault o AWS Key Management Service
- Recuperar clave en runtime con autenticación
- Habilitar rotación automática de claves

#### Opción 3: Ofuscación de Código

Mínimo viable para MVP:
- Usar ConfuserEx, .NET Reactor, o Dotfuscator
- Ofuscar constantes y strings
- Dificulta (pero no previene) la ingeniería inversa

```bash
# Ejemplo con ConfuserEx
ConfuserEx.Cli.exe -n Tweaker.csproj
```

#### Opción 4: Implementación Actual Mejorada

Para el deployment inicial:

```csharp
// En lugar de:
private const string SECRET_KEY = "TweakerLic2024SecretKey9876543";

// Usar:
private static readonly byte[] SECRET_KEY_BYTES = new byte[] 
{ 
    0x54, 0x77, 0x65, 0x61, 0x6B, 0x65, 0x72, 0x4C, 
    0x69, 0x63, 0x32, 0x30, 0x32, 0x34, 0x53, 0x65,
    // ... continuar con bytes
};
```

Esto dificulta ligeramente la extracción pero no es seguridad real.

## 🛡️ Ofuscación de Código

### Herramientas Recomendadas

#### ConfuserEx (Gratis, Open Source)
```bash
dotnet tool install --global ConfuserEx.CLI
confuserex-cli Tweaker.csproj
```

Características:
- Ofuscación de nombres
- Control flow obfuscation
- Anti-tampering
- Anti-debugging

#### .NET Reactor (Comercial)
- Mejor protección
- Native compilation
- Code virtualization
- License manager integrado

#### SmartAssembly (Comercial)
- Ofuscación avanzada
- Error reporting
- Feature usage analytics

### Configuración Mínima

```xml
<!-- ConfuserEx Configuration -->
<project>
  <module path="Tweaker.exe">
    <rule pattern="true" inherit="false">
      <protection id="anti tamper" />
      <protection id="anti debug" />
      <protection id="constants" />
      <protection id="ctrl flow" />
      <protection id="rename" />
    </rule>
  </module>
</project>
```

## 🔍 Anti-Tampering

### Verificación de Integridad

Agregar verificación de que el assembly no ha sido modificado:

```csharp
public static class IntegrityChecker
{
    public static bool VerifyAssembly()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var originalHash = "EXPECTED_HASH_HERE";
        
        using (var sha = SHA256.Create())
        {
            using (var stream = File.OpenRead(assembly.Location))
            {
                var hash = Convert.ToBase64String(sha.ComputeHash(stream));
                return hash == originalHash;
            }
        }
    }
}
```

### Detección de Debuggers

```csharp
public static class DebugDetection
{
    public static bool IsDebuggerAttached()
    {
        return System.Diagnostics.Debugger.IsAttached;
    }
    
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    public static extern bool IsDebuggerPresent();
}
```

Llamar en startup:
```csharp
if (DebugDetection.IsDebuggerAttached() || DebugDetection.IsDebuggerPresent())
{
    // Acción: cerrar app, generar alerta, etc.
}
```

## 📝 Logging de Seguridad

### Registrar Eventos de Seguridad

```csharp
public static class SecurityLogger
{
    private static readonly string LOG_PATH = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Tweaker", "security.log"
    );
    
    public static void LogActivationAttempt(bool success, string hardwareId)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                   $"Activation attempt: {(success ? "SUCCESS" : "FAILED")} " +
                   $"Hardware ID: {hardwareId}";
        File.AppendAllText(LOG_PATH, entry + Environment.NewLine);
    }
    
    public static void LogTamperingDetected(string details)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                   $"SECURITY ALERT: Tampering detected - {details}";
        File.AppendAllText(LOG_PATH, entry + Environment.NewLine);
        
        // Opcionalmente: enviar alerta remota
    }
}
```

Integrar en LicenseManager:

```csharp
// En ValidateLicenseOnStartup()
var result = /* validación */;
SecurityLogger.LogActivationAttempt(result, fingerprint);
```

## 🌐 Validación en Línea (Opcional)

Para mayor seguridad, considerar validación en servidor:

```csharp
public static async Task<bool> ValidateOnlineAsync(string licenseKey, string fingerprint)
{
    using (var client = new HttpClient())
    {
        var payload = new { LicenseKey = licenseKey, Fingerprint = fingerprint };
        var response = await client.PostAsJsonAsync("https://api.ejemplo.com/validate", payload);
        return response.IsSuccessStatusCode;
    }
}
```

Ventajas:
- Control centralizado de licencias
- Revocación remota
- Analytics en tiempo real
- Detección de abusos

Desventajas:
- Requiere conectividad
- Infraestructura adicional
- Costos de servidor

## 🔐 Rotación de Claves

### Implementar Versionado de Claves

```csharp
public class KeyVersion
{
    public int Version { get; set; }
    public byte[] Key { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

public static class KeyManager
{
    private static readonly Dictionary<int, KeyVersion> KEYS = new()
    {
        { 1, new KeyVersion { Version = 1, Key = /* v1 key */, ValidFrom = new DateTime(2024, 1, 1) } },
        { 2, new KeyVersion { Version = 2, Key = /* v2 key */, ValidFrom = new DateTime(2024, 6, 1) } },
    };
    
    public static byte[] GetCurrentKey()
    {
        return KEYS.Values.OrderByDescending(k => k.Version).First().Key;
    }
    
    public static byte[] GetKeyForVersion(int version)
    {
        return KEYS[version].Key;
    }
}
```

## 📋 Checklist Pre-Producción

### Mínimo Viable (MVP)

- [ ] Cambiar SECRET_KEY a un valor único y aleatorio
- [ ] Ofuscar código con ConfuserEx o similar
- [ ] Implementar logging de activaciones
- [ ] Probar en múltiples máquinas reales
- [ ] Documentar proceso de recuperación de licencias

### Recomendado

- [ ] Implementar key derivation (PBKDF2/Argon2)
- [ ] Anti-debugging básico
- [ ] Verificación de integridad del assembly
- [ ] Sistema de registro de licencias generadas
- [ ] Backup seguro del código de KeyGenerator

### Óptimo

- [ ] Azure Key Vault / AWS KMS para claves
- [ ] Validación online opcional
- [ ] Rotación de claves
- [ ] Telemetría y analytics
- [ ] Detección avanzada de tampering
- [ ] Code virtualization

## 🚀 Plan de Deployment

### Fase 1: Testing (1-2 semanas)
- Probar en ambiente controlado
- Validar con usuarios beta
- Ajustar según feedback

### Fase 2: Soft Launch (2-4 semanas)
- Deploy con MVP security
- Monitorear activaciones
- Soporte dedicado

### Fase 3: Hardening (ongoing)
- Implementar mejoras de seguridad
- Responder a reportes
- Actualizar según amenazas

## 📞 Contacto

Para preguntas sobre seguridad:
- Security lead: [nombre]
- Email: security@ejemplo.com
- Reportar vulnerabilidades: security-reports@ejemplo.com

## 📚 Referencias

- [OWASP .NET Security Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/DotNet_Security_Cheat_Sheet.html)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/security/)
- [ConfuserEx Documentation](https://yck1509.github.io/ConfuserEx/)
- [NIST Key Management](https://csrc.nist.gov/publications/detail/sp/800-57-part-1/rev-5/final)

---

**Última actualización**: 2024-02-12
**Versión**: 1.0
**Clasificación**: INTERNO
