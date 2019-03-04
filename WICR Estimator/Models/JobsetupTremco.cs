using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.Models
{
    public class JobsetupTremco:JobSetup
    {
        public double AdditionalTermBarLF { get; set; }
        public bool SuperStopAtFooting { get; set; }
        public double InsideOutsideCornerDetails { get; set; }

        public JobsetupTremco(string ProjectName): base(ProjectName)
        {

        }
    }
}
