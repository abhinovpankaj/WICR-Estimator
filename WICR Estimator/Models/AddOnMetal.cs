using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.Models
{
    public class AddOnMetal:Metal
    {
        private bool isMetalChecked;
        public bool IsMetalChecked
        {
            get { return isMetalChecked; }
            set
            {
                if (value!=isMetalChecked)
                {
                    isMetalChecked = value;
                    OnPropertyChanged("IsMetalChecked");
                }
            }
        }
        public AddOnMetal(string name,string size, double productionRate, double laborRate, double units, double materialPrice, bool isStairMetal, double specialPricing = 0)
            :base(name,size,  productionRate,  laborRate,  units,  materialPrice,  isStairMetal,  specialPricing = 0)
        {
        }
    }
}
