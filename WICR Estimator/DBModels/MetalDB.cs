using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    
    public class MetalDB: INotifyPropertyChanged
    {
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

        private Action<bool> onSelectionChanged;
       

        public int MetalId { get; set; }
        public string MetalName { get; set; }
        public int? Units { get; set; }
        public double MetalPrice { get; set; }
        public double ProductionRate { get; set; }
        public string MetalType { get; set; }

        public string Vendor { get; set; }
        public int ProjectId { get; set; }

        public bool IsDeleted { get; set; }

        internal void HookCheckBoxAction(Action<bool> onSelectionChanged)
        {
            this.onSelectionChanged = onSelectionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
