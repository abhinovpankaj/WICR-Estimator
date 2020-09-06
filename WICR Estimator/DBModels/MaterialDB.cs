using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    //[Serializable]
    public class MaterialDB
    {
        
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
        public bool IsChecked { get; set; }
    }
}
