using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.DBModels
{
    public class EstimateDB
    {
        public EstimateDB()
        { }
        public EstimateDB(string jobName, string preparedBy, DateTime? jobCreationDate, ProjectsTotal totals)
        {
            JobName = jobName;
            PreparedBy = preparedBy;
            CreateTime = jobCreationDate;
            TotalLaborCost = totals.LaborCost;
            TotalSlopeCost = totals.SlopeCost;
            TotalMetalCost = totals.MetalCost;
            TotalSystemCost = totals.SystemCost;
            TotalMaterialCost = totals.MaterialCost;
            TotalLaborPercentage = totals.LaborPercentage;
        }
        public int EstimateID { get; set; }
        public string PreparedBy { get; set; }
        public string JobName { get; set; }
        public DateTime? CreateTime { get; set; }
        public double TotalLaborCost { get; set; }
        public double TotalSlopeCost { get; set; }
        public double TotalMetalCost { get; set; }
        public double TotalSystemCost { get; set; }
        public double TotalMaterialCost { get; set; }
        public string TotalLaborPercentage { get; set; }

    }
    
}
