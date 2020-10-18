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
        public double Coverage { get; set; }
        public double MaterialPrice { get; set; }
        public double Weight { get; set; }

        public double ProdRateHorizontal { get; set; }

        public double ProdRateVertical { get; set; }
        public double ProdRateStair { get; set; }
        public double LaborMinCharge { get; set; }

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
