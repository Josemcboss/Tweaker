using System.ComponentModel;

namespace Tweaker.Models
{
    /// <summary>
    /// Modelo que representa un elemento del Debloat Wizard
    /// </summary>
    public class DebloatItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        /// <summary>
        /// Identificador �nico del tweak
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Nombre visible del tweak
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Descripci�n detallada
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Comando PowerShell a ejecutar
        /// </summary>
        public string? PowerShellCommand { get; set; }

        /// <summary>
        /// Si el usuario lo seleccion� para eliminar
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// Nivel de riesgo del tweak (importado de TweakModel)
        /// </summary>
        public RiskLevel Risk { get; set; }

        /// <summary>
        /// Categor�a (Bloatware, Privacy, System, etc.)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// RAM estimada a liberar (en MB)
        /// </summary>
        public int EstimatedRamFreedMB { get; set; }

        /// <summary>
        /// Espacio en disco a liberar (en MB)
        /// </summary>
        public int EstimatedDiskFreedMB { get; set; }

        /// <summary>
        /// Si es recomendado para la mayor�a de usuarios
        /// </summary>
        public bool IsRecommended { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

