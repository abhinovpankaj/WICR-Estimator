using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    //[Serializable]
    public class FreightDB 
    {
        
        public string FactorName { get; set; }
        public double FactorValue { get; set; }

        public bool IsDeleted { get; set; }

        public int FreightID { get; set; }
    }
}
