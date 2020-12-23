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
        

        private int? _units;
        public int? Units
        {
            get
            {
                return _units;
            }
            set
            {
                if (_units != value)
                {
                    _units = value;
                    OnPropertyChanged("Units");
                }
            }
        }
        private double _metalPrice;
        public double MetalPrice
        {
            get
            {
                return _metalPrice;
            }
            set
            {
                if (_metalPrice != value)
                {
                    _metalPrice = value;
                    OnPropertyChanged("MetalPrice");
                }
            }
        }

        private double _productionRate;
        public double ProductionRate
        {
            get
            {
                return _productionRate;
            }
            set
            {
                if (_productionRate != value)
                {
                    _productionRate = value;
                    OnPropertyChanged("ProductionRate");
                }
            }
        }
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
