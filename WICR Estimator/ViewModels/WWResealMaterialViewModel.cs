using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    public class WWResealMaterialViewModel : MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        public WWResealMaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup Js) : base(metalTotals, slopeTotals, Js)
        {
            materialNames = new Dictionary<string, string>();
            FillMaterialList();
            FetchMaterialValuesAsync(false);
        }

        private void FillMaterialList()
        {
            

            materialNames.Add("SLURRY COAT (RESISTITE) OVER TEXTURE","SQ FT");

            materialNames.Add("LIGHT CRACK REPAIR", "SQ FT");
            materialNames.Add("LARGE CRACK REPAIR", "LF");
            materialNames.Add("BUBBLE REPAIR (MEASURE SQ FT)", "SQ FT");
            materialNames.Add("RESISTITE LIQUID", "5 GAL PAIL");
            materialNames.Add("RESISTITE REGULAR GRAY", "55 LB BAG");
            materialNames.Add("RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)", "40 LB BAG");

            materialNames.Add("CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)", "40 LB BAG");
            materialNames.Add("RESISTITE UNIVERSAL PRIMER (ADD 50% WATER)", "5 GAL PAIL");
            materialNames.Add("VISTA PAINT ACRAPOXY SEALER", "5 GAL PAIL");
            materialNames.Add("DEXOTEX AJ-44", "5 GAL PAIL");
            materialNames.Add("WESTCOAT SC-10", "5 GAL PAIL");
            materialNames.Add("UPI PERMASHIELD", "5 GAL PAIL");
            materialNames.Add("PLI DEK GS88 WITH COLOR JAR 1 PER PAIL", "5 GAL PAIL");
            materialNames.Add("SLURRY COAT (RESISTITE)OVER TEXTURE", "5 GAL PAIL");
            materialNames.Add("OPTIONAL FOR WEATHER SEAL XL", "5 GAL PAIL");
        }

        public override ObservableCollection<SystemMaterial> GetSystemMaterial()
        {
            ObservableCollection<SystemMaterial> smCollection = new ObservableCollection<SystemMaterial>();
            int k = 0;
            foreach (string key in materialNames.Keys)
            {

                smCollection.Add(getSMObject(k, key, materialNames[key]));
                k++;
            }
            return smCollection;

        }

    }
}
