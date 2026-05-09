# Ghost Optimizer - Modernización Completa

## Estrategia
Modernización incremental de .NET 8 → .NET 10 LTS con actualización conservadora de dependencias

## Preferencias
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task
- **Pace**: Standard
- **Target Framework**: net10.0-windows
- **Conservative Updates**: Usar versiones estables, no preview (excepto LibreHardwareMonitorLib que es pre-release)

## Decisiones
- Actualizar a .NET 10 LTS (soporte hasta 2028) - 2025-01-29
- Mantener formato SDK-style (ya optimizado) - 2025-01-29
- Actualizar LibreHardwareMonitorLib de forma conservadora - 2025-01-29

## Custom Instructions
<!-- Para tareas específicas -->
- **Para 01-upgrade-dotnet-framework**: Validar que .NET 10.0 SDK está instalado antes de cambiar
- **Para 03-fix-api-breaking-changes**: Revisar principalmente System.ServiceProcess y System.Management
- **Para 04-validate-build-and-tests**: Si existen tests en Tweaker.Tests/, ejecutarlos también
