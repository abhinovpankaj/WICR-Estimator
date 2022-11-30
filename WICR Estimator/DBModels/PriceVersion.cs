using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class PriceVersion
    {
        public double Version { get; set; }

        public DateTime LastUpdatedOn { get; set; }
        public bool ApplyPrice { get; set; }

        public int PriceVersionId { get; set; }
    }
}
