namespace Tweaker.Utilities
{
    /// <summary>
    /// RegistryPaths - Centraliza todas las rutas de registro para evitar duplicación
    /// </summary>
    public static class RegistryPaths
    {
        // ────────────────────────────────────────────?
        // SYSTEM REGISTRY PATHS (HKEY_LOCAL_MACHINE)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Rutas relacionadas con TCP/IP y red
        /// </summary>
        public static class Network
        {
            public const string TcpIpInterfaces = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
            public const string TcpIpParameters = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";
            public const string SystemProfile = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
            public const string DnsCache = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";
        }

        /// <summary>
        /// Rutas relacionadas con energía y CPU
        /// </summary>
        public static class Power
        {
            public const string PowerThrottling = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling";
            public const string PriorityControl = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
            public const string PowerSettings = @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings";
            public const string CoreParking = @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583";
        }

        /// <summary>
        /// Rutas relacionadas con GPU y gráficos
        /// </summary>
        public static class Graphics
        {
            public const string GraphicsDrivers = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers";
            public const string DwmRegistry = @"Software\Microsoft\Windows\DWM";
            public const string SystemProfileTasksGames = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games";
        }

        /// <summary>
        /// Rutas relacionadas con servicios del sistema
        /// </summary>
        public static class Services
        {
            public const string UsbService = @"SYSTEM\CurrentControlSet\Services\USB";
            public const string WindowsSearch = @"SYSTEM\CurrentControlSet\Services\WSearch";
            public const string SysMain = @"SYSTEM\CurrentControlSet\Services\SysMain";
            public const string DiagTrack = @"SYSTEM\CurrentControlSet\Services\DiagTrack";
        }

        /// <summary>
        /// Rutas relacionadas con políticas del sistema
        /// </summary>
        public static class Policies
        {
            public const string System = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
            public const string WindowsUpdate = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate";
            public const string GameDvrPolicy = @"SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR";
        }

        // ────────────────────────────────────────────?
        // USER REGISTRY PATHS (HKEY_CURRENT_USER)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Rutas relacionadas con efectos visuales y tema
        /// </summary>
        public static class UserVisual
        {
            public const string VisualEffects = @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects";
            public const string PersonalizeThemes = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            public const string DwmUser = @"Software\Microsoft\Windows\DWM";
        }

        /// <summary>
        /// Rutas relacionadas con gaming y Game Bar
        /// </summary>
        public static class UserGaming
        {
            public const string GameConfigStore = @"System\GameConfigStore";
            public const string GameDvr = @"Software\Microsoft\Windows\CurrentVersion\GameDVR";
            public const string GameBar = @"Software\Microsoft\GameBar";
        }

        /// <summary>
        /// Rutas relacionadas con input (mouse, teclado)
        /// </summary>
        public static class UserInput
        {
            public const string Mouse = @"Control Panel\Mouse";
            public const string KeyboardParameters = @"SYSTEM\CurrentControlSet\Services\kbdclass\Parameters";
            public const string MouseParameters = @"SYSTEM\CurrentControlSet\Services\mouclass\Parameters";
            public const string AccessibilityStickyKeys = @"Control Panel\Accessibility\StickyKeys";
            public const string AccessibilityKeyboardResponse = @"Control Panel\Accessibility\Keyboard Response";
        }

        /// <summary>
        /// Rutas relacionadas con FTH (Fault Tolerant Heap)
        /// </summary>
        public static class UserSystem
        {
            public const string Fth = @"SOFTWARE\Microsoft\FTH";
        }
    }

    /// <summary>
    /// RegistryValues - Centraliza nombres de valores comunes del registro
    /// </summary>
    public static class RegistryValues
    {
        // ────────────────────────────────────────────?
        // NETWORK VALUES
        // ────────────────────────────────────────────?
        public const string TcpAckFrequency = "TcpAckFrequency";
        public const string TCPNoDelay = "TCPNoDelay";
        public const string TcpDelAckTicks = "TcpDelAckTicks";
        public const string TcpWindowSize = "TcpWindowSize";
        public const string NetworkThrottlingIndex = "NetworkThrottlingIndex";
        public const string SystemResponsiveness = "SystemResponsiveness";

        // ────────────────────────────────────────────?
        // POWER VALUES
        // ────────────────────────────────────────────?
        public const string PowerThrottlingOff = "PowerThrottlingOff";
        public const string Win32PrioritySeparation = "Win32PrioritySeparation";

        // ────────────────────────────────────────────?
        // GRAPHICS VALUES
        // ────────────────────────────────────────────?
        public const string HwSchMode = "HwSchMode";
        public const string GpuPriority = "GPU Priority";
        public const string Priority = "Priority";
        public const string SchedulingCategory = "Scheduling Category";
        public const string EnableTransparency = "EnableTransparency";
        public const string EnableAeroPeek = "EnableAeroPeek";
        public const string VisualFxSetting = "VisualFXSetting";

        // ────────────────────────────────────────────?
        // GAMING VALUES
        // ────────────────────────────────────────────?
        public const string GameDvrEnabled = "GameDVR_Enabled";
        public const string AppCaptureEnabled = "AppCaptureEnabled";
        public const string AllowGameDVR = "AllowGameDVR";

        // ────────────────────────────────────────────?
        // INPUT VALUES
        // ────────────────────────────────────────────?
        public const string MouseSpeed = "MouseSpeed";
        public const string MouseThreshold1 = "MouseThreshold1";
        public const string MouseThreshold2 = "MouseThreshold2";
        public const string MouseDataQueueSize = "MouseDataQueueSize";
        public const string KeyboardDataQueueSize = "KeyboardDataQueueSize";
        public const string DisableSelectiveSuspend = "DisableSelectiveSuspend";

        // ────────────────────────────────────────────?
        // SYSTEM VALUES
        // ────────────────────────────────────────────?
        public const string FthEnabled = "Enabled";
        public const string StickyKeysFlags = "Flags";

        // ────────────────────────────────────────────?
        // SERVICE VALUES
        // ────────────────────────────────────────────?
        public const string ServiceStart = "Start";
    }
}
