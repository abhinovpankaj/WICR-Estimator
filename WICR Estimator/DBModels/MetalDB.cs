using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class MetalDB:IDbData
    {
        public bool IsChecked { get; set; }

        public int MetalId { get; set; }
        public string MetalName { get; set; }
        public int? Units { get; set; }
        public double MetalPrice { get; set; }
        public double ProductionRate { get; set; }
        public string MetalType { get; set; }

        public string Vendor { get; set; }
        public int ProjectId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
