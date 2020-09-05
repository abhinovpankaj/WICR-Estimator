using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class LaborFactorDB:IDbData
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public bool IsChecked { get; set; }
        public int LaborId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
