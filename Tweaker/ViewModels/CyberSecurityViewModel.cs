using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tweaker.Models;
using Tweaker.Optimizations;
using Tweaker.Services;

namespace Tweaker.ViewModels
{
    public class CyberSecurityViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly SecurityDiagnosticsService _diagnosticsService;
        private string _statusMessage = "Listo";
        private string _hostsStatusSummary = "Sin analizar";
        private bool _isHostsClean = true;
        private bool _isScanning = false;

        public ObservableCollection<NetworkConnectionItem> ActiveConnections { get; } = new();
        public ObservableCollection<PersistenceItem> PersistenceItems { get; } = new();
        public ObservableCollection<HostsEntry> HostsEntries { get; } = new();

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string HostsStatusSummary
        {
            get => _hostsStatusSummary;
            set => SetProperty(ref _hostsStatusSummary, value);
        }

        public bool IsHostsClean
        {
            get => _isHostsClean;
            set => SetProperty(ref _isHostsClean, value);
        }

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        public ICommand RefreshConnectionsCommand { get; }
        public ICommand RefreshPersistenceCommand { get; }
        public ICommand CheckHostsCommand { get; }
        public ICommand RestoreHostsCommand { get; }
        public ICommand ApplyDnsCommand { get; }
        public ICommand ResetDnsCommand { get; }
        public ICommand EnableAsrCommand { get; }
        public ICommand DisableAsrCommand { get; }
        public ICommand EnableTelemetryBlockCommand { get; }
        public ICommand RevertTelemetryBlockCommand { get; }

        public CyberSecurityViewModel()
        {
            _diagnosticsService = new SecurityDiagnosticsService();

            RefreshConnectionsCommand = new RelayCommand(async _ => await LoadConnectionsAsync());
            RefreshPersistenceCommand = new RelayCommand(async _ => await LoadPersistenceAsync());
            CheckHostsCommand = new RelayCommand(_ => CheckHostsFile());
            RestoreHostsCommand = new RelayCommand(_ => RestoreHostsFile());

            ApplyDnsCommand = new RelayCommand(provider => ApplyDns(provider?.ToString() ?? "Cloudflare"));
            ResetDnsCommand = new RelayCommand(_ => ResetDns());

            EnableAsrCommand = new RelayCommand(async _ => await Task.Run(() =>
            {
                StatusMessage = "Aplicando reglas ASR de Windows Defender...";
                bool ok = CyberSecurityOptimization.EnableDefenderAsrRules();
                StatusMessage = ok ? "Reglas ASR de Defender activadas con éxito." : "Error al aplicar reglas ASR.";
            }));

            DisableAsrCommand = new RelayCommand(async _ => await Task.Run(() =>
            {
                StatusMessage = "Restableciendo reglas ASR...";
                bool ok = CyberSecurityOptimization.DisableDefenderAsrRules();
                StatusMessage = ok ? "Reglas ASR restablecidas a predeterminado." : "Error al restablecer reglas ASR.";
            }));

            EnableTelemetryBlockCommand = new RelayCommand(async _ => await Task.Run(() =>
            {
                StatusMessage = "Bloqueando telemetría invasiva...";
                bool ok = CyberSecurityOptimization.ApplyTelemetryHardening();
                StatusMessage = ok ? "Telemetría y rastreadores bloqueados." : "Error aplicando bloqueo de telemetría.";
            }));

            RevertTelemetryBlockCommand = new RelayCommand(async _ => await Task.Run(() =>
            {
                StatusMessage = "Restaurando telemetría de Windows...";
                bool ok = CyberSecurityOptimization.RevertTelemetryHardening();
                StatusMessage = ok ? "Telemetría restaurada a valores normales." : "Error restaurando telemetría.";
            }));

            // Carga inicial
            Task.Run(async () =>
            {
                CheckHostsFile();
                await LoadPersistenceAsync();
                await LoadConnectionsAsync();
            });
        }

        public async Task LoadConnectionsAsync()
        {
            IsScanning = true;
            StatusMessage = "Escaneando sockets y puertos activos...";

            await Task.Run(() =>
            {
                var list = _diagnosticsService.GetActiveConnections();
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    ActiveConnections.Clear();
                    foreach (var item in list)
                    {
                        ActiveConnections.Add(item);
                    }
                });
            });

            IsScanning = false;
            StatusMessage = $"Monitor de red: {ActiveConnections.Count} conexiones inspeccionadas.";
        }

        public async Task LoadPersistenceAsync()
        {
            await Task.Run(() =>
            {
                var list = _diagnosticsService.GetPersistenceItems();
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    PersistenceItems.Clear();
                    foreach (var item in list)
                    {
                        PersistenceItems.Add(item);
                    }
                });
            });
        }

        public void CheckHostsFile()
        {
            var result = _diagnosticsService.AnalyzeHostsFile();
            IsHostsClean = result.IsClean;
            HostsStatusSummary = result.Summary;

            HostsEntries.Clear();
            foreach (var entry in result.Entries)
            {
                HostsEntries.Add(entry);
            }
        }

        public void RestoreHostsFile()
        {
            var confirm = MessageBox.Show(
                "¿Deseas restaurar el archivo 'hosts' a los valores estándar limpios de Microsoft?\n(Se creará una copia de respaldo automática).",
                "Restaurar Archivo Hosts",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                bool ok = _diagnosticsService.RestoreCleanHostsFile();
                if (ok)
                {
                    StatusMessage = "Archivo hosts restaurado con éxito.";
                    CheckHostsFile();
                }
                else
                {
                    StatusMessage = "Error al restaurar el archivo hosts (¿requiere permisos de Administrador?).";
                }
            }
        }

        private void ApplyDns(string provider)
        {
            StatusMessage = $"Configurando DNS seguro ({provider})...";
            Task.Run(() =>
            {
                bool ok = CyberSecurityOptimization.ApplySecureDns(provider);
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    StatusMessage = ok ? $"DNS seguro ({provider}) configurado correctamente." : "Error configurando DNS.";
                });
            });
        }

        private void ResetDns()
        {
            StatusMessage = "Restableciendo DNS a DHCP automático...";
            Task.Run(() =>
            {
                bool ok = CyberSecurityOptimization.ResetDnsToAutomatic();
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    StatusMessage = ok ? "DNS restablecido a automático (DHCP)." : "Error restableciendo DNS.";
                });
            });
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
