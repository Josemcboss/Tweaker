using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Tweaker.Models;
using Tweaker.Optimizations;

namespace Tweaker.ViewModels
{
    /// <summary>
    /// ViewModel para la ventana Debloat Wizard
    /// </summary>
    public class DebloatViewModel : INotifyPropertyChanged
    {
        private int _totalItemsSelected;
        private int _estimatedRamFreed;
        private int _estimatedDiskFreed;
        private bool _isExecuting;
        private string _statusMessage = string.Empty;

        public ObservableCollection<DebloatItem> DebloatItems { get; set; }

        public int TotalItemsSelected
        {
            get => _totalItemsSelected;
            set
            {
                _totalItemsSelected = value;
                OnPropertyChanged(nameof(TotalItemsSelected));
            }
        }

        public int EstimatedRamFreed
        {
            get => _estimatedRamFreed;
            set
            {
                _estimatedRamFreed = value;
                OnPropertyChanged(nameof(EstimatedRamFreed));
            }
        }

        public int EstimatedDiskFreed
        {
            get => _estimatedDiskFreed;
            set
            {
                _estimatedDiskFreed = value;
                OnPropertyChanged(nameof(EstimatedDiskFreed));
            }
        }

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                OnPropertyChanged(nameof(IsExecuting));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public DebloatViewModel()
        {
            DebloatItems = new ObservableCollection<DebloatItem>();
            InitializeDebloatItems();
            UpdateStats();
        }

        private void InitializeDebloatItems()
        {
            // CATEGOR�A: BLOATWARE PURE
            DebloatItems.Add(new DebloatItem
            {
                Id = "cortana",
                Name = "Eliminar Cortana",
                Description = "Asistente de voz en desuso. Ya no es funcional en Windows 11.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.549981C3F5F10 | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Bloatware",
                EstimatedRamFreedMB = 150,
                EstimatedDiskFreedMB = 200,
                IsRecommended = true
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "games",
                Name = "Eliminar Juegos Pre-instalados",
                Description = "Candy Crush, Bubble Witch, Solitaire y otros juegos basura.",
                PowerShellCommand = "Get-AppxPackage -allusers *CandyCrush*, king.com*, *BubbleWitch*, Microsoft.MicrosoftSolitaireCollection | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Bloatware",
                EstimatedRamFreedMB = 50,
                EstimatedDiskFreedMB = 500,
                IsRecommended = true
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "weather",
                Name = "Eliminar Bing Weather",
                Description = "Widget del clima. No afecta funcionalidad del sistema.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.BingWeather | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Bloatware",
                EstimatedRamFreedMB = 30,
                EstimatedDiskFreedMB = 100,
                IsRecommended = true
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "news",
                Name = "Eliminar Microsoft News",
                Description = "Widget de noticias. App decorativa sin utilidad real.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.BingNews | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Bloatware",
                EstimatedRamFreedMB = 40,
                EstimatedDiskFreedMB = 120,
                IsRecommended = true
            });

            // CATEGOR�A: PRIVACY & TELEMETRY
            DebloatItems.Add(new DebloatItem
            {
                Id = "telemetry",
                Name = "Eliminar Apps de Telemetr�a",
                Description = "Feedback Hub, Get Help, Get Started. No afecta funciones esenciales.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.Windows.Feedback*, Microsoft.GetHelp, Microsoft.Getstarted | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Privacy",
                EstimatedRamFreedMB = 100,
                EstimatedDiskFreedMB = 150,
                IsRecommended = true
            });

            // CATEGOR�A: OPTIONAL APPS
            DebloatItems.Add(new DebloatItem
            {
                Id = "maps",
                Name = "Eliminar Windows Maps",
                Description = "Aplicaci�n de mapas integrada. Solo eliminar si no la usas.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.WindowsMaps | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Optional",
                EstimatedRamFreedMB = 80,
                EstimatedDiskFreedMB = 300,
                IsRecommended = false
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "3dapps",
                Name = "Eliminar Apps 3D",
                Description = "3D Builder, Paint 3D, Print 3D. Apps legacy sin soporte.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.3DBuilder, Microsoft.MSPaint, Microsoft.Print3D | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Optional",
                EstimatedRamFreedMB = 60,
                EstimatedDiskFreedMB = 250,
                IsRecommended = false
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "skype",
                Name = "Eliminar Skype Pre-instalado",
                Description = "Versi�n UWP de Skype. Puedes reinstalar desde Microsoft Store.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.SkypeApp | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Optional",
                EstimatedRamFreedMB = 70,
                EstimatedDiskFreedMB = 180,
                IsRecommended = false
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "mixedreality",
                Name = "Eliminar Mixed Reality Portal",
                Description = "Portal de realidad virtual/aumentada. Solo si no usas VR/AR.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.MixedReality.Portal | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Optional",
                EstimatedRamFreedMB = 100,
                EstimatedDiskFreedMB = 400,
                IsRecommended = false
            });

            DebloatItems.Add(new DebloatItem
            {
                Id = "todo",
                Name = "Eliminar Microsoft To Do",
                Description = "App de tareas. Reinstalable desde Store si la necesitas.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.Todos | Remove-AppxPackage",
                Risk = RiskLevel.Safe,
                Category = "Optional",
                EstimatedRamFreedMB = 50,
                EstimatedDiskFreedMB = 150,
                IsRecommended = false
            });

            // CATEGOR�A: MODERATE RISK
            DebloatItems.Add(new DebloatItem
            {
                Id = "onedrive",
                Name = "Eliminar OneDrive",
                Description = "?? Servicio de nube de Microsoft. NO eliminar si sincronizas archivos.",
                PowerShellCommand = "taskkill /f /im OneDrive.exe; ...", // Comando completo en WindowsDebloat.cs
                Risk = RiskLevel.Moderate,
                Category = "Cloud Services",
                EstimatedRamFreedMB = 200,
                EstimatedDiskFreedMB = 100,
                IsRecommended = false
            });

            // CATEGOR�A: ADVANCED
            DebloatItems.Add(new DebloatItem
            {
                Id = "xbox",
                Name = "Eliminar Servicios de Xbox",
                Description = "?? AVANZADO: Elimina Game Bar y todos los servicios Xbox. Solo si ya deshabilitaste Game Bar.",
                PowerShellCommand = "Get-AppxPackage -allusers Microsoft.Xbox*, Microsoft.GamingApp | Remove-AppxPackage",
                Risk = RiskLevel.Advanced,
                Category = "Gaming Services",
                EstimatedRamFreedMB = 250,
                EstimatedDiskFreedMB = 500,
                IsRecommended = false
            });

            // Suscribir eventos para actualizar estad�sticas
            foreach (var item in DebloatItems)
            {
                item.PropertyChanged += OnItemSelectionChanged;
            }
        }

        private void OnItemSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DebloatItem.IsSelected))
            {
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            var selectedItems = DebloatItems.Where(x => x.IsSelected).ToList();

            TotalItemsSelected = selectedItems.Count;
            EstimatedRamFreed = selectedItems.Sum(x => x.EstimatedRamFreedMB);
            EstimatedDiskFreed = selectedItems.Sum(x => x.EstimatedDiskFreedMB);
        }

        public void SelectRecommended()
        {
            foreach (var item in DebloatItems)
            {
                item.IsSelected = item.IsRecommended;
            }

            StatusMessage = $"? {TotalItemsSelected} elementos recomendados seleccionados";
        }

        public void ExecuteDebloat(Action<string> progressCallback)
        {
            if (TotalItemsSelected == 0)
            {
                StatusMessage = "?? No has seleccionado ning�n elemento";
                return;
            }

            IsExecuting = true;
            StatusMessage = "?? Iniciando limpieza...";

            int successCount = 0;
            int failCount = 0;

            foreach (var item in DebloatItems.Where(x => x.IsSelected))
            {
                progressCallback?.Invoke($"Eliminando: {item.Name}...");

                bool success = false;

                // Ejecutar el m�todo correspondiente
                switch (item.Id)
                {
                    case "cortana":
                        success = WindowsDebloat.RemoveCortana();
                        break;
                    case "games":
                        success = WindowsDebloat.RemoveGames();
                        break;
                    case "weather":
                        success = WindowsDebloat.RemoveBingWeather();
                        break;
                    case "news":
                        success = WindowsDebloat.RemoveNews();
                        break;
                    case "telemetry":
                        success = WindowsDebloat.RemoveTelemetry();
                        break;
                    case "maps":
                        success = WindowsDebloat.RemoveMaps();
                        break;
                    case "3dapps":
                        success = WindowsDebloat.Remove3DApps();
                        break;
                    case "skype":
                        success = WindowsDebloat.RemoveSkype();
                        break;
                    case "mixedreality":
                        success = WindowsDebloat.RemoveMixedReality();
                        break;
                    case "todo":
                        success = WindowsDebloat.RemoveToDo();
                        break;
                    case "onedrive":
                        success = WindowsDebloat.RemoveOneDrive();
                        break;
                    case "xbox":
                        success = WindowsDebloat.RemoveXboxServices();
                        break;
                }

                if (success)
                {
                    successCount++;
                    progressCallback?.Invoke($"? {item.Name} eliminado");
                }
                else
                {
                    failCount++;
                    progressCallback?.Invoke($"? Error al eliminar {item.Name}");
                }

                // Peque�a pausa para que el usuario vea el progreso
                System.Threading.Thread.Sleep(500);
            }

            IsExecuting = false;
            StatusMessage = $"? Limpieza completada: {successCount} exitosos, {failCount} fallidos";

            progressCallback?.Invoke($"\n?? RAM liberada estimada: ~{EstimatedRamFreed} MB");
            progressCallback?.Invoke($"?? Espacio en disco liberado estimado: ~{EstimatedDiskFreed} MB");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
