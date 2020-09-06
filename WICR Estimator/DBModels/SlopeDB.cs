using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    //[Serializable]
    public class SlopeDB
    {
        public int SlopeId { get; set; }
        public string SlopeName { get; set; }
        public double LaborRate { get; set; }
        public double PerMixCost { get; set; }
        public string SlopeType { get; set; }
        public int ProjectId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsChecked { get; set; }
    }
}
