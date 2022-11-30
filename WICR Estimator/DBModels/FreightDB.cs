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
    public class FreightDB : INotifyPropertyChanged
    {
        private Action<bool> onSelectionChanged;
        public string FactorName { get; set; }
        public double FactorValue { get; set; }

        public bool IsDeleted { get; set; }

        public int FreightID { get; set; }

        private bool _isChecked;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
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

        public void HookCheckBoxAction(Action <bool> onSelectionChange)
        {
            this.onSelectionChanged = onSelectionChange;
        }
    }
}
