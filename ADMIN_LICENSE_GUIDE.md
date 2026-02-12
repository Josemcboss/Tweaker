# Guía de Administración - Sistema de Licencias Tweaker

## 🔐 CONFIDENCIAL - Solo para Administradores

Esta guía es para personal autorizado que genera y administra licencias de Tweaker.

## ⚠️ Importante

**NO DISTRIBUIR** la herramienta KeyGenerator ni esta documentación a usuarios finales. Solo el equipo de administración debe tener acceso.

## Requisitos Previos

- .NET 10.0 SDK instalado
- Acceso al código fuente de Tweaker
- Permisos para ejecutar la herramienta KeyGenerator

## Instalación de KeyGenerator

1. Abrir terminal en el directorio del proyecto
2. Navegar a la carpeta KeyGenerator:
   ```bash
   cd KeyGenerator
   ```
3. Compilar el proyecto:
   ```bash
   dotnet build -c Release
   ```

## Uso de KeyGenerator

### Modo Interactivo (Recomendado)

1. Ejecutar la herramienta:
   ```bash
   dotnet run
   ```

2. Verás el menú:
   ```
   ════════════════════════════════════════════════════════
      TWEAKER - GENERADOR DE LLAVES DE LICENCIA
      HERRAMIENTA ADMINISTRATIVA - CONFIDENCIAL
   ════════════════════════════════════════════════════════

   Opciones:
   1. Generar llave perpetua
   2. Generar llave con fecha de expiración
   3. Salir
   ```

3. Seleccionar opción (1 o 2)

4. Ingresar el **Hardware Fingerprint** del cliente
   - El cliente debe ejecutar Tweaker y obtener su Hardware ID
   - Este ID aparece en la ventana de activación

5. Si seleccionaste opción 2, ingresar fecha de expiración en formato `yyyy-MM-dd`
   - Ejemplo: `2025-12-31`

6. La herramienta generará y mostrará la llave:
   ```
   ════════════════════════════════════════════════════════
   ✅ LLAVE GENERADA EXITOSAMENTE
   ════════════════════════════════════════════════════════

   Hardware Fingerprint: ABC123DEF456
   Tipo:                 Licencia Perpetua ♾️

   LLAVE DE LICENCIA:
   ╔══════════════════════════════╗
   ║  XXXXX-XXXXX-XXXXX-XXXXX  ║
   ╚══════════════════════════════╝
   ```

7. Copiar la llave y enviarla al cliente

### Modo Batch (Para automatización)

Útil para scripts o generación masiva:

**Licencia Perpetua:**
```bash
dotnet run -- --batch ABC123DEF456
```

**Licencia con Expiración:**
```bash
dotnet run -- --batch ABC123DEF456 2025-12-31
```

**Salida:**
```
XXXXX-XXXXX-XXXXX-XXXXX
```

## Proceso Completo de Activación

### 1. Cliente Solicita Licencia

Cliente contacta a soporte/ventas solicitando activar Tweaker.

### 2. Cliente Obtiene Hardware ID

Instruir al cliente para:
1. Ejecutar Tweaker
2. Si no tiene licencia, verá la ventana de activación automáticamente
3. En la ventana aparece "ID del Hardware (para soporte)"
4. Copiar y enviar ese ID (ejemplo: `ABC123DEF456GHIJ`)

### 3. Administrador Genera Llave

1. Recibir Hardware ID del cliente
2. Ejecutar KeyGenerator
3. Ingresar Hardware ID del cliente
4. Seleccionar tipo de licencia:
   - **Perpetua**: Para clientes pagados completo
   - **Con fecha**: Para trials o suscripciones temporales
5. Copiar la llave generada

### 4. Enviar Llave al Cliente

Enviar la llave al cliente por:
- Email
- Chat de soporte
- Sistema de tickets

**Formato de envío:**
```
Tu llave de activación de Tweaker:

XXXXX-XXXXX-XXXXX-XXXXX

Instrucciones:
1. Abre Tweaker
2. Ingresa la llave en el campo de licencia
3. Click en "Activar"
4. ¡Listo! Tweaker está activado
```

### 5. Cliente Activa

Cliente:
1. Copia la llave
2. La pega en Tweaker
3. Click en "Activar"
4. Sistema valida y activa
5. Cliente puede usar Tweaker normalmente

## Tipos de Licencias

### Licencia Perpetua (♾️)

- **Duración**: Sin fecha de expiración
- **Uso**: Clientes que compraron licencia completa
- **Características**:
  - No expira nunca
  - Vinculada al hardware
  - Una sola activación por máquina

### Licencia Temporal (📅)

- **Duración**: Hasta fecha específica
- **Uso**: Trials, suscripciones, demos
- **Características**:
  - Expira en fecha indicada
  - Después de expirar, Tweaker pide reactivación
  - Renovable generando nueva llave

## Gestión de Casos Especiales

### Cliente Cambió de Hardware

**Problema**: La licencia deja de funcionar porque el hardware cambió.

**Solución**:
1. Cliente obtiene nuevo Hardware ID
2. Verificar que es el mismo cliente (email, nombre, compra original)
3. Generar nueva llave con el nuevo Hardware ID
4. Enviar nueva llave al cliente

**Política recomendada**: 
- Permitir 1-2 cambios de hardware por licencia
- Registrar cambios en sistema de gestión
- Para cambios frecuentes, investigar posible compartir licencia

### Cliente Perdió la Llave

**Solución**:
1. Verificar identidad del cliente
2. Cliente obtiene Hardware ID actual
3. Generar nueva llave con el mismo Hardware ID
4. Enviar nueva llave

**Nota**: Como la llave depende del hardware, generar otra llave con el mismo Hardware ID es equivalente a recuperar la original.

### Licencia No Funciona

**Diagnóstico**:

1. **Verificar formato**: Debe ser `XXXXX-XXXXX-XXXXX-XXXXX`
   - 20 caracteres alfanuméricos
   - 4 grupos de 5 caracteres
   - Separados por guiones

2. **Verificar Hardware ID coincide**:
   - Pedir al cliente su Hardware ID actual
   - Comparar con el usado para generar la llave
   - Si son diferentes, hardware cambió

3. **Verificar fecha de expiración**:
   - Si es licencia temporal, verificar no ha expirado
   - Revisar fecha del sistema del cliente

4. **Re-generar llave si es necesario**:
   - Usar Hardware ID actual del cliente
   - Generar nueva llave
   - Enviar al cliente

### Licencia Compartida (Abuso)

**Detección**: 
- Múltiples reportes de "licencia inválida" del mismo cliente
- Cambios frecuentes de hardware

**Acción**:
- Investigar si cliente está compartiendo licencia
- Revisar términos de servicio
- Tomar acción según política de empresa

## Seguridad y Mejores Prácticas

### ✅ Hacer

- Mantener KeyGenerator privado y seguro
- Registrar todas las licencias generadas
- Verificar identidad de clientes antes de generar llaves
- Usar sistema de gestión para rastrear licencias
- Hacer backup del código fuente de KeyGenerator

### ❌ No Hacer

- NO compartir KeyGenerator con usuarios finales
- NO compartir esta documentación públicamente
- NO generar llaves sin verificar identidad
- NO usar el mismo Hardware ID para múltiples clientes
- NO almacenar llaves en texto plano sin protección

## Sistema de Registro (Opcional)

Para mejor gestión, mantener registro de:

| Hardware ID | Cliente | Email | Tipo Licencia | Fecha Generación | Fecha Expiración | Estado |
|-------------|---------|-------|---------------|------------------|------------------|--------|
| ABC123...   | Juan P. | juan@email.com | Perpetua | 2024-01-15 | - | Activa |
| DEF456...   | María G. | maria@email.com | Temporal | 2024-01-16 | 2024-12-31 | Activa |

Esto permite:
- Rastrear licencias generadas
- Detectar abusos
- Gestionar renovaciones
- Soporte más eficiente

## Troubleshooting Administrador

### KeyGenerator no compila

```bash
# Verificar .NET SDK instalado
dotnet --version

# Debe ser 10.0 o superior
# Limpiar y recompilar
dotnet clean
dotnet build
```

### Error "No se puede encontrar Tweaker.License"

```bash
# Asegurarse que el proyecto principal está compilado
cd ../Tweaker
dotnet build
cd ../KeyGenerator
dotnet build
```

### Llave generada no funciona

1. Verificar que `SECRET_KEY` es la misma en:
   - `LicenseKeyGenerator.cs`
   - `LicenseValidator.cs`

2. No modificar el código de encriptación sin actualizar ambos lados

3. Recompilar proyecto completo:
   ```bash
   dotnet clean
   dotnet build
   ```

## Actualizaciones del Sistema

Si se modifica el algoritmo de encriptación o generación:

1. Actualizar documentación
2. Notificar a equipo de soporte
3. Considerar retrocompatibilidad con llaves antiguas
4. Probar exhaustivamente antes de desplegar

## Contacto y Soporte

Para problemas con la herramienta de administración:
- Contactar al equipo de desarrollo
- Revisar documentación técnica completa
- Verificar logs de errores

---

**Última actualización**: 2024-02-12
**Versión**: 1.0
**Clasificación**: CONFIDENCIAL
