using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.DBModels
{
    //[Serializable]
    public class LaborFactorDB: INotifyPropertyChanged
    {
        
        public int ProjectId { get; set; }
        public string Name { get; set; }
        private double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                OnPropertyChanged("Value");
            }

        }
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                onSelectionChanged?.Invoke(value);

                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public int LaborId { get; set; }
        public bool IsDeleted { get; set; }
        private Action<bool> onSelectionChanged;
        internal void HookCheckBoxAction(Action<bool> onLaborSelectionChanged)
        {
            this.onSelectionChanged = onLaborSelectionChanged;
        }
    }
}
