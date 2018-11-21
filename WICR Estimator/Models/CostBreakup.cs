using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    public class CostBreakup: BaseViewModel
    {
        public string Name { get; set; }
        private double calFactor;
        public double CalFactor
        { get { return calFactor; }
            set
            {
                if (value!=calFactor)
                {
                    calFactor = value;
                    OnPropertyChanged("CalFactor");
                }
            }
        }
        public double SlopeCost { get; set; }
        public double MetalCost { get; set; }
        public double SystemCost { get; set; }
        public System.Windows.Visibility HideCalFactor { get; set; }
    }
}
