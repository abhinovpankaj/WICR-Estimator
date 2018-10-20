using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.Models
{
    public class CostBreakup
    {
        public string Name { get; set; }
        public double CalFactor { get; set; }
        public double SlopeCost { get; set; }
        public double MetalCost { get; set; }
        public double SystemCost { get; set; }
        public System.Windows.Visibility HideCalFactor { get; set; }
    }
}
