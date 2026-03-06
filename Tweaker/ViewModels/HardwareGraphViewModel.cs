using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tweaker.ViewModels
{
    public class HardwareGraphViewModel : INotifyPropertyChanged
    {
        private const int MaxPoints = 60;

        public HardwareGraphViewModel()
        {
            // Initialize values with zeros
            var values = Enumerable.Repeat(0.0, MaxPoints).ToList();
            _values = new ObservableCollection<double>(values);
        }

        private readonly ObservableCollection<double> _values;

        // Añade un nuevo valor a la serie
        public void AddValue(double value)
        {
            if (_values.Count >= MaxPoints)
            {
                _values.RemoveAt(0);
            }
            _values.Add(value);

            OnPropertyChanged(null);
        }

        public IList<double> GetValuesSnapshot()
        {
            lock (_values)
            {
                return _values.ToList();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
