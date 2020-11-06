using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class ProjectDetailsDB
    {
        public string WorkArea { get; set; }
        public int EstimateID { get; set; }
        public int ProjectDetailID { get; set; }
        public double MetalCost { get; set; }
        public double SlopeCost { get; set; }
        public double SystemCost { get; set; }
        public double MaterialCost { get; set; }
        public double LaborCost { get; set; }

        public double TotalCost { get; set; }
        public double CostPerSqFoot { get; set; }
        public string LaborPercentage { get; set; }

        public bool HasPrevailingWage { get; set; }

        public bool HasContingencyDisc { get; set; }
        public double ProjectProfitMargin { get; set; }
    }
}
