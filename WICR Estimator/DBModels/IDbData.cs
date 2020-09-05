using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public interface IDbData
    {
        bool IsDeleted { get; set; }
    }
}
