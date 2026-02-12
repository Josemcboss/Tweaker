# Manual de Usuario - Activación de Tweaker

## 🔐 Activación de Tweaker

Tweaker ahora requiere activación mediante una llave de licencia única para proteger el software y garantizar que solo usuarios autorizados puedan usarlo.

## 📋 Proceso de Activación

### Paso 1: Primera Ejecución

Cuando inicies Tweaker por primera vez (o sin licencia válida), verás automáticamente la ventana de activación:

![Ventana de Activación](docs/activation_window.png)

### Paso 2: Obtener tu Hardware ID

En la ventana de activación verás tu **Hardware ID** (ID del Hardware):

```
ID del Hardware (para soporte):
┌──────────────────┐
│  ABC123DEF456    │
└──────────────────┘
```

Este ID es **único para tu computadora** y está basado en los componentes de hardware.

### Paso 3: Solicitar tu Llave de Licencia

1. **Copia tu Hardware ID** del campo mostrado
2. **Contacta a soporte** o al vendedor autorizado:
   - Envía tu Hardware ID
   - Proporciona tu información de compra/registro
3. **Espera tu llave de licencia** por email o sistema de soporte

### Paso 4: Ingresar la Llave

Una vez que recibas tu llave de licencia:

1. **Copia la llave** (formato: `XXXXX-XXXXX-XXXXX-XXXXX`)
2. **Pégala en el campo** "Llave de Licencia" en la ventana de activación
3. El formato se ajustará automáticamente con guiones
4. Haz clic en **"✅ Activar"**

### Paso 5: Confirmación

Si la activación es exitosa, verás un mensaje:

```
✅ ¡Licencia activada exitosamente!

Licencia Perpetua ♾️
(o)
Válida hasta: 2025-12-31

Tweaker está ahora activado en este equipo.
```

¡Listo! Ahora puedes usar Tweaker normalmente.

## 🔄 Activaciones Posteriores

Una vez activado:
- **No necesitas volver a activar** al iniciar Tweaker
- La licencia se **valida automáticamente** en cada inicio
- Tu licencia está **guardada de forma segura** en tu computadora

## ⚠️ Errores Comunes

### "Llave de licencia inválida"

**Causas posibles:**
- La llave fue copiada incorrectamente (verifica espacios extra)
- La llave no corresponde a tu Hardware ID
- La llave ha expirado (si es temporal)

**Solución:**
1. Verifica que copiaste la llave completa
2. Asegúrate que es la llave correcta para tu Hardware ID
3. Contacta a soporte si el problema persiste

### "Formato de llave inválido"

**Causa:**
El formato de la llave no es correcto.

**Solución:**
Verifica que la llave tenga el formato:
```
XXXXX-XXXXX-XXXXX-XXXXX
```
- 20 caracteres alfanuméricos
- 4 grupos de 5 caracteres
- Separados por guiones (-)

### "Hardware no coincide"

**Causa:**
Esta llave fue generada para otra computadora.

**Solución:**
1. Verifica que estás usando la llave correcta
2. Si es así, obtén tu Hardware ID actual
3. Solicita una nueva llave con tu Hardware ID correcto

## 🖥️ Cambios de Hardware

### ¿Qué sucede si cambio componentes de mi PC?

Si realizas cambios significativos en tu hardware:
- Cambio de placa madre
- Cambio de procesador
- Cambio de disco duro
- Cambio de tarjeta de red

Tu licencia **dejará de funcionar** porque el Hardware ID habrá cambiado.

### ¿Cómo reactivar después de cambiar hardware?

1. Inicia Tweaker (verás la ventana de activación)
2. Anota tu **nuevo Hardware ID**
3. Contacta a soporte con:
   - Tu información de licencia anterior
   - Tu nuevo Hardware ID
   - Descripción del cambio de hardware
4. Soporte generará una **nueva llave** para tu nuevo hardware
5. Ingresa la nueva llave para reactivar

## 🔒 Seguridad

### ¿Es seguro mi Hardware ID?

Sí. Tu Hardware ID:
- ✅ Es un hash criptográfico (SHA-256)
- ✅ No contiene información personal
- ✅ Solo identifica tu hardware de forma anónima
- ✅ No puede ser usado para rastrearte

### ¿Dónde se guarda mi licencia?

Tu licencia se guarda de forma segura en:
```
%AppData%\Tweaker\.tweaker.lic
```

El archivo está:
- 🔐 Cifrado con AES-256
- 🔏 Protegido con firma digital
- 🛡️ Solo funciona en tu computadora

### ¿Puedo compartir mi llave?

**NO.** Cada llave está vinculada a un Hardware ID específico:
- ❌ No funcionará en otras computadoras
- ❌ Compartir llaves viola los términos de uso
- ⚠️ Puede resultar en revocación de licencia

## 📞 Soporte

### ¿Necesitas ayuda?

Si tienes problemas con la activación:

1. **Verifica esta guía** para errores comunes
2. **Prepara esta información**:
   - Tu Hardware ID (en la ventana de activación)
   - Mensaje de error exacto (captura de pantalla)
   - Descripción del problema
3. **Contacta a soporte**:
   - Email: [soporte@ejemplo.com]
   - Sistema de tickets: [url]
   - Chat en vivo: [url]

### Información útil para soporte:

```
Hardware ID: [Tu ID aquí]
Versión de Tweaker: [Versión]
Sistema Operativo: [Tu OS]
Mensaje de error: [Descripción]
```

## 📝 Tipos de Licencias

### Licencia Perpetua ♾️

- **Duración**: Sin fecha de expiración
- **Uso**: Puedes usar Tweaker indefinidamente
- **Restricción**: Solo en la computadora para la que fue activada

### Licencia Temporal 📅

- **Duración**: Hasta una fecha específica
- **Uso**: Puedes usar Tweaker hasta la fecha de expiración
- **Renovación**: Contacta a soporte para renovar o actualizar

Verás el tipo de licencia al activar:
```
✅ ¡Licencia activada exitosamente!

Licencia Perpetua ♾️
```
o
```
✅ ¡Licencia activada exitosamente!

Válida hasta: 2025-12-31
```

## ❓ Preguntas Frecuentes

### ¿Puedo usar Tweaker en múltiples computadoras?

No. Cada llave está vinculada a un Hardware ID específico. Para usar Tweaker en múltiples computadoras necesitas una licencia para cada una.

### ¿Qué pasa si reinstalo Windows?

Reinstalar Windows generalmente **NO** cambia el Hardware ID (a menos que formatees con cambios de hardware). Tu licencia debería seguir funcionando.

### ¿Puedo transferir mi licencia a otra computadora?

Contacta a soporte. Dependiendo de la política de licenciamiento, puede ser posible transferir tu licencia a nuevo hardware.

### ¿Necesito internet para activar?

**No** para activar localmente. Una vez que tengas tu llave:
- La activación se hace localmente
- No requiere conexión a internet
- La validación es offline

Pero **SÍ** necesitas internet para:
- Contactar a soporte
- Recibir tu llave de licencia

### ¿Qué pasa si pierdo mi llave?

Contacta a soporte con:
- Tu información de compra/registro
- Tu Hardware ID actual

Soporte puede regenerar tu llave.

### ¿La licencia expira?

Depende del tipo:
- **Perpetua**: Nunca expira
- **Temporal**: Expira en la fecha indicada

Verás tu tipo de licencia al activar.

## ✅ Checklist de Activación

- [ ] Inicié Tweaker y vi la ventana de activación
- [ ] Copié mi Hardware ID
- [ ] Contacté a soporte/vendedor con mi Hardware ID
- [ ] Recibí mi llave de licencia
- [ ] Copié la llave completa (20 caracteres + guiones)
- [ ] Pegué la llave en Tweaker
- [ ] Hice clic en "Activar"
- [ ] Vi el mensaje de éxito
- [ ] Tweaker está funcionando normalmente

---

**¡Gracias por usar Tweaker!** 🚀

Si tienes preguntas o problemas, no dudes en contactar a soporte.
