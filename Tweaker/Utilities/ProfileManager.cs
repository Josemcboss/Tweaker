using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Gestor de perfiles de configuración
    /// Permite guardar, cargar y gestionar diferentes configuraciones de tweaks
    /// </summary>
    public class ProfileManager
    {
        private static ProfileManager? _instance;
        private readonly string _profilesDirectory;
        private const string DEFAULT_EXTENSION = ".tweakerprofile";

        public static ProfileManager Instance => _instance ??= new ProfileManager();

        private ProfileManager()
        {
            _profilesDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Tweaker",
                "Profiles"
            );

            Directory.CreateDirectory(_profilesDirectory);
            EnsureDefaultProfiles();
            
            Debug.WriteLine($"?? Profiles directory: {_profilesDirectory}");
        }

        /// <summary>
        /// Crea los perfiles predefinidos si no existen
        /// </summary>
        private void EnsureDefaultProfiles()
        {
            CreateDefaultProfile_MaxPerformance();
            CreateDefaultProfile_Balanced();
            CreateDefaultProfile_Streaming();
            CreateDefaultProfile_CompetitiveGaming();
            CreateDefaultProfile_WorkStation();
        }

        /// <summary>
        /// Perfil: MAXIMUM PERFORMANCE (Para gaming extremo)
        /// </summary>
        private void CreateDefaultProfile_MaxPerformance()
        {
            string profilePath = Path.Combine(_profilesDirectory, "MaxPerformance" + DEFAULT_EXTENSION);
            
            if (File.Exists(profilePath)) return;

            var profile = new TweakerProfile
            {
                Name = "Maximum Performance",
                Description = "Configuración extrema para máximo rendimiento en gaming. Sacrifica eficiencia energética por FPS.",
                Category = "Gaming",
                CreatedDate = DateTime.Now,
                Author = "Tweaker Team",
                Tweaks = new Dictionary<string, bool>
                {
                    // Input & Visuals
                    {"MouseAcceleration", true},
                    {"Keyboard", true},
                    {"VisualEffects", true},
                    {"MemoryOptimization", true},
                    
                    // Sistema & GPU
                    {"SystemProfile", true},
                    {"GameDVR", true},
                    {"GpuScheduling", false}, // Controversial, por defecto OFF
                    {"SystemResponsiveness", true},
                    {"HighPerformance", true},
                    {"PowerThrottling", true},
                    {"CoreParking", true},
                    
                    // Limpieza
                    {"Hibernation", true},
                    {"WindowsSearch", true},
                    {"SysMain", true},
                    {"Telemetry", true},
                    {"DiagTrack", true},
                    
                    // GHOST Pack
                    {"MPO", true},
                    {"UltimatePower", true},
                    {"GameBar", true},
                    {"CoreIsolation", true},
                    {"HPET", true},
                    {"HyperV", false}, // OFF por defecto (muchos usan Docker)
                    
                    // Network
                    {"NetworkOptimization", true},
                    
                    // Advanced
                    {"SpectreMeltdown", false} // OFF por seguridad
                }
            };

            SaveProfile(profile);
        }

        /// <summary>
        /// Perfil: BALANCED (Equilibrio entre rendimiento y eficiencia)
        /// </summary>
        private void CreateDefaultProfile_Balanced()
        {
            string profilePath = Path.Combine(_profilesDirectory, "Balanced" + DEFAULT_EXTENSION);
            
            if (File.Exists(profilePath)) return;

            var profile = new TweakerProfile
            {
                Name = "Balanced",
                Description = "Configuración equilibrada. Mejoras de rendimiento sin sacrificar funcionalidad del sistema.",
                Category = "General",
                CreatedDate = DateTime.Now,
                Author = "Tweaker Team",
                Tweaks = new Dictionary<string, bool>
                {
                    // Input & Visuals
                    {"MouseAcceleration", true},
                    {"Keyboard", true},
                    {"VisualEffects", false}, // Mantener efectos
                    {"MemoryOptimization", true},
                    
                    // Sistema & GPU
                    {"SystemProfile", true},
                    {"GameDVR", true},
                    {"GpuScheduling", false},
                    {"SystemResponsiveness", true},
                    {"HighPerformance", true},
                    {"PowerThrottling", false},
                    {"CoreParking", true},
                    
                    // Limpieza
                    {"Hibernation", true},
                    {"WindowsSearch", false}, // Mantener búsqueda
                    {"SysMain", false},
                    {"Telemetry", true},
                    {"DiagTrack", true},
                    
                    // GHOST Pack
                    {"MPO", true},
                    {"UltimatePower", false}, // Usar High Performance en lugar de Ultimate
                    {"GameBar", true},
                    {"CoreIsolation", false}, // Mantener seguridad
                    {"HPET", true},
                    {"HyperV", false},
                    
                    // Network
                    {"NetworkOptimization", true},
                    
                    // Advanced
                    {"SpectreMeltdown", false}
                }
            };

            SaveProfile(profile);
        }

        /// <summary>
        /// Perfil: STREAMING (Para streamers y creadores de contenido)
        /// </summary>
        private void CreateDefaultProfile_Streaming()
        {
            string profilePath = Path.Combine(_profilesDirectory, "Streaming" + DEFAULT_EXTENSION);
            
            if (File.Exists(profilePath)) return;

            var profile = new TweakerProfile
            {
                Name = "Streaming",
                Description = "Optimizado para streaming. Balance entre rendimiento del juego y calidad de transmisión.",
                Category = "Content Creation",
                CreatedDate = DateTime.Now,
                Author = "Tweaker Team",
                Tweaks = new Dictionary<string, bool>
                {
                    // Input & Visuals
                    {"MouseAcceleration", true},
                    {"Keyboard", true},
                    {"VisualEffects", false}, // Mantener efectos para mejor UI en stream
                    {"MemoryOptimization", true},
                    
                    // Sistema & GPU
                    {"SystemProfile", true},
                    {"GameDVR", false}, // NO deshabilitar (necesario para algunos overlays)
                    {"GpuScheduling", true}, // Ayuda con encoding
                    {"SystemResponsiveness", true},
                    {"HighPerformance", true},
                    {"PowerThrottling", true},
                    {"CoreParking", true},
                    
                    // Limpieza
                    {"Hibernation", true},
                    {"WindowsSearch", false},
                    {"SysMain", true},
                    {"Telemetry", true},
                    {"DiagTrack", true},
                    
                    // GHOST Pack
                    {"MPO", false}, // Puede causar problemas con OBS
                    {"UltimatePower", true},
                    {"GameBar", false}, // Mantener para shortcuts
                    {"CoreIsolation", false},
                    {"HPET", true},
                    {"HyperV", false},
                    
                    // Network
                    {"NetworkOptimization", true},
                    
                    // Advanced
                    {"SpectreMeltdown", false}
                }
            };

            SaveProfile(profile);
        }

        /// <summary>
        /// Perfil: COMPETITIVE GAMING (Para e-sports y juegos competitivos)
        /// </summary>
        private void CreateDefaultProfile_CompetitiveGaming()
        {
            string profilePath = Path.Combine(_profilesDirectory, "CompetitiveGaming" + DEFAULT_EXTENSION);
            
            if (File.Exists(profilePath)) return;

            var profile = new TweakerProfile
            {
                Name = "Competitive Gaming",
                Description = "Configuración para gaming competitivo. Minimiza latencia y maximiza consistencia de FPS.",
                Category = "Gaming",
                CreatedDate = DateTime.Now,
                Author = "Tweaker Team",
                Tweaks = new Dictionary<string, bool>
                {
                    // Input & Visuals - TODO activado para mínima latencia
                    {"MouseAcceleration", true},
                    {"Keyboard", true},
                    {"VisualEffects", true},
                    {"MemoryOptimization", true},
                    
                    // Sistema & GPU - Enfocado en latencia
                    {"SystemProfile", true},
                    {"GameDVR", true},
                    {"GpuScheduling", false}, // OFF para menor latencia
                    {"SystemResponsiveness", true},
                    {"HighPerformance", true},
                    {"PowerThrottling", true},
                    {"CoreParking", true},
                    
                    // Limpieza - Máxima
                    {"Hibernation", true},
                    {"WindowsSearch", true},
                    {"SysMain", true},
                    {"Telemetry", true},
                    {"DiagTrack", true},
                    
                    // GHOST Pack - TODO activado
                    {"MPO", true},
                    {"UltimatePower", true},
                    {"GameBar", true},
                    {"CoreIsolation", true},
                    {"HPET", true},
                    {"HyperV", false}, // OFF para menor latencia
                    
                    // Network - Crítico
                    {"NetworkOptimization", true},
                    
                    // Advanced
                    {"SpectreMeltdown", false} // OFF por defecto, usuario decide
                }
            };

            SaveProfile(profile);
        }

        /// <summary>
        /// Perfil: WORKSTATION (Para trabajo y productividad)
        /// </summary>
        private void CreateDefaultProfile_WorkStation()
        {
            string profilePath = Path.Combine(_profilesDirectory, "WorkStation" + DEFAULT_EXTENSION);
            
            if (File.Exists(profilePath)) return;

            var profile = new TweakerProfile
            {
                Name = "WorkStation",
                Description = "Para uso productivo. Mantiene funcionalidad mientras optimiza rendimiento.",
                Category = "Work",
                CreatedDate = DateTime.Now,
                Author = "Tweaker Team",
                Tweaks = new Dictionary<string, bool>
                {
                    // Input & Visuals
                    {"MouseAcceleration", false}, // Mantener aceleración para trabajo
                    {"Keyboard", false},
                    {"VisualEffects", false}, // Mantener efectos
                    {"MemoryOptimization", true},
                    
                    // Sistema & GPU
                    {"SystemProfile", false},
                    {"GameDVR", false},
                    {"GpuScheduling", false},
                    {"SystemResponsiveness", true},
                    {"HighPerformance", true},
                    {"PowerThrottling", false},
                    {"CoreParking", false},
                    
                    // Limpieza - Mínima
                    {"Hibernation", false}, // Mantener hibernación
                    {"WindowsSearch", false}, // MANTENER búsqueda
                    {"SysMain", false},
                    {"Telemetry", true},
                    {"DiagTrack", true},
                    
                    // GHOST Pack - Conservador
                    {"MPO", false},
                    {"UltimatePower", false},
                    {"GameBar", false},
                    {"CoreIsolation", false}, // Mantener seguridad
                    {"HPET", false},
                    {"HyperV", false}, // Mantener para Docker/VMs
                    
                    // Network
                    {"NetworkOptimization", true},
                    
                    // Advanced
                    {"SpectreMeltdown", false} // Mantener seguridad
                }
            };

            SaveProfile(profile);
        }

        /// <summary>
        /// Guarda un perfil en disco
        /// </summary>
        private void SaveProfile(TweakerProfile profile)
        {
            try
            {
                string fileName = profile.Name.Replace(" ", "") + DEFAULT_EXTENSION;
                string filePath = Path.Combine(_profilesDirectory, fileName);

                string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);
                Debug.WriteLine($"? Perfil guardado: {fileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error guardando perfil: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los perfiles disponibles
        /// </summary>
        public List<TweakerProfile> GetAllProfiles()
        {
            var profiles = new List<TweakerProfile>();

            try
            {
                var files = Directory.GetFiles(_profilesDirectory, "*" + DEFAULT_EXTENSION);

                foreach (var file in files)
                {
                    try
                    {
                        string json = File.ReadAllText(file);
                        var profile = JsonSerializer.Deserialize<TweakerProfile>(json);
                        
                        if (profile != null)
                        {
                            profiles.Add(profile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"? Error leyendo perfil {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error obteniendo perfiles: {ex.Message}");
            }

            return profiles.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Carga un perfil específico
        /// </summary>
        public TweakerProfile? LoadProfile(string profileName)
        {
            try
            {
                string fileName = profileName.Replace(" ", "") + DEFAULT_EXTENSION;
                string filePath = Path.Combine(_profilesDirectory, fileName);

                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"? Perfil no encontrado: {fileName}");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<TweakerProfile>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error cargando perfil: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Aplica un perfil (activa/desactiva los tweaks según el perfil)
        /// </summary>
        public void ApplyProfile(TweakerProfile profile)
        {
            try
            {
                Debug.WriteLine($"?? Aplicando perfil: {profile.Name}");
                
                // TODO: Implementar lógica de aplicación de tweaks
                // Por ahora solo logging
                
                foreach (var tweak in profile.Tweaks)
                {
                    Debug.WriteLine($"  {tweak.Key}: {(tweak.Value ? "ON" : "OFF")}");
                }
                
                Debug.WriteLine($"? Perfil aplicado: {profile.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error aplicando perfil: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un perfil personalizado
        /// </summary>
        public bool DeleteProfile(string profileName)
        {
            try
            {
                string fileName = profileName.Replace(" ", "") + DEFAULT_EXTENSION;
                string filePath = Path.Combine(_profilesDirectory, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.WriteLine($"? Perfil eliminado: {fileName}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error eliminando perfil: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Clase que representa un perfil de configuración
    /// </summary>
    public class TweakerProfile
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public Dictionary<string, bool> Tweaks { get; set; } = new();
    }
}
