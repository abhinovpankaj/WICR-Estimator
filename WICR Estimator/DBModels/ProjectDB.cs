using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class ProjectDB:IDbData
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string MainGroupName { get; set; }
        public int Rank { get; set; }
        public int ProjectId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
