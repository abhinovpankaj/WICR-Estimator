using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class IndependentMaterialViewModel:MaterialBaseViewModel
    {
        public override SystemMaterial getSMObject(int seq, string matName, string unit)
        {
            double cov;
            double mp;
            double w;
            double lfArea;
            double setUpMin = 0; // Setup minimum charges from google sheet, col 6
            double pRateStairs = 0; ///Production rate stairs from google sheet, col 5
            double hprRate = 0;///Horizontal Production rate  from google sheet, col 4
            double vprRate = 0;///Vertical Production rate  from google sheet, col 1
            double sqh = 0;
            double sqv = 0;
            double labrExt = 0;
            double calcHrs = 0;
            double sqStairs = 0;
            double qty = 0;
            string operation = "";
            if (isPrevailingWage)
            {
                double.TryParse(freightData[5][0].ToString(), out prPerc);
            }
            else
                prPerc = 0;

            double.TryParse(materialDetails[seq][1].ToString(), out vprRate);
            double.TryParse(materialDetails[seq][2].ToString(), out cov);
            double.TryParse(materialDetails[seq][0].ToString(), out mp);
            double.TryParse(materialDetails[seq][3].ToString(), out w);
            lfArea = getlfArea(matName);
            double.TryParse(materialDetails[seq][6].ToString(), out setUpMin);
            double.TryParse(materialDetails[seq][5].ToString(), out pRateStairs);
            double.TryParse(materialDetails[seq][4].ToString(), out hprRate);
            pRateStairs = pRateStairs * (1 + prPerc);
            hprRate = hprRate * (1 + prPerc);
            vprRate = vprRate * (1 + prPerc);
            sqv = getSqftAreaVertical(matName);
            sqh = getSqFtAreaH(matName);
            sqStairs = getSqFtStairs(matName);
            calcHrs = CalculateHrs(sqh, hprRate, sqStairs, pRateStairs, sqv, vprRate);

            labrExt = CalculateLabrExtn(calcHrs, setUpMin, matName);
            qty = getQuantity(matName, cov, lfArea);
            if (lfArea == -1)
            {
                lfArea = qty;
            }
            if (sqh == -1)
            {
                sqh = qty;
            }
            if (sqStairs == -1)
            {
                sqStairs = qty;
            }
            operation = GetOperation(matName);
            return (new SystemMaterial
            {
                Name = matName,
                SMUnits = unit,
                SMSqft = lfArea,
                Coverage = cov,
                MaterialPrice = mp,
                Weight = w,
                Qty = qty,
                SMSqftH = sqh,
                Operation = operation,
                HorizontalProductionRate = hprRate,
                StairsProductionRate = pRateStairs,
                StairSqft = sqStairs,
                SetupMinCharge = setUpMin,
                Hours = calcHrs,
                LaborExtension = labrExt,
                VerticalProductionRate = vprRate,
                LaborUnitPrice = getLaborUnitPrice(labrExt, riserCount, totalSqft, sqv, sqh, sqStairs, matName),//labrExt / (riserCount + totalSqft),
                FreightExtension = w * qty,
                MaterialExtension = mp * qty,
                IsMaterialChecked = false,//getCheckboxCheckStatus(matName),
                IsMaterialEnabled = true,//getCheckboxEnabledStatus(matName),
                IncludeInLaborMinCharge = IncludedInLaborMin(matName)
            });
        }
    }
}
