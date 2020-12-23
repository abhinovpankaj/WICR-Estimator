using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    //[Serializable]
    public class MaterialDB:INotifyPropertyChanged
    {
        private bool _isChecked;

        public int ProjectId { get; set; }

        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        private double _coverage;
        public double Coverage 
        {
            get
            {
                return _coverage;
            }
            set
            {
                if(_coverage!=value)
                {
                    _coverage = value;
                    OnPropertyChanged("Coverage");
                }
            }
        }
        private double _materialPrice;
        public double MaterialPrice
        {
            get
            {
                return _materialPrice;
            }
            set
            {
                if (_materialPrice != value)
                {
                    _materialPrice = value;
                    OnPropertyChanged("MaterialPrice");
                }
            }
        }

        private double _weight;
        public double Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged("Weight");
                }
            }
        }

        private double _prodRateHorizontal;
        public double ProdRateHorizontal
        {
            get
            {
                return _prodRateHorizontal;
            }
            set
            {
                if (_prodRateHorizontal != value)
                {
                    _prodRateHorizontal = value;
                    OnPropertyChanged("ProdRateHorizontal");
                }
            }
        }
        private double _prodRateVertical;
        public double ProdRateVertical
        {
            get
            {
                return _prodRateVertical;
            }
            set
            {
                if (_prodRateVertical != value)
                {
                    _prodRateVertical = value;
                    OnPropertyChanged("ProdRateVertical");
                }
            }
        }
        
        private double _prodRateStair;
        public double ProdRateStair
        {
            get
            {
                return _prodRateStair;
            }
            set
            {
                if (_prodRateStair != value)
                {
                    _prodRateStair = value;
                    OnPropertyChanged("ProdRateStair");
                }
            }
        }

        private double _laborMinCharge;
        public double LaborMinCharge
        {
            get
            {
                return _laborMinCharge;
            }
            set
            {
                if (_laborMinCharge != value)
                {
                    _laborMinCharge = value;
                    OnPropertyChanged("LaborMinCharge");
                }
            }
        }
       

        public bool IsDeleted { get; set; }
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
        internal void HookCheckBoxAction(Action<bool> onLaborSelectionChanged)
        {
            this.onSelectionChanged = onLaborSelectionChanged;
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
