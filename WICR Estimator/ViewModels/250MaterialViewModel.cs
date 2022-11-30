using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    [DataContract]
    class _250MaterialViewModel:MaterialBaseViewModel
    {
        private Dictionary<string, string> materialNames;
        private double totalSqftVertical;
        private double termBar;
        private double rebarPrepWalls;
        private double superStop;
        private double penetrations;
        private double totalPlywoodSqft;
        private bool hasNewPlywood;
        //private double termBar;
        public _250MaterialViewModel(Totals metalTotals, Totals slopeTotals, JobSetup js) : base(metalTotals, slopeTotals, js)
        {
            materialNames = new Dictionary<string, string>();

            FillMaterialList();

            FetchMaterialValuesAsync(false);
        }

        private void FillMaterialList(int addMore=0)
        {
            materialNames.Add("191 QD PRIMER AND PREPARATION FOR RE-SURFACE", "1 GALLON");
            materialNames.Add("TREMPRIME MULTI SURFACE (CONCRETE & OTHER)", "3 GAL KIT");
            materialNames.Add("#191 QD INTERLAMINATE PRIMER", "1 GALLON");
            materialNames.Add("Vulkem Tremproof 250 GC L", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R", "5 GAL PAIL");
            
            materialNames.Add("Vulkem Tremproof 201 L", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 R", "5 GAL PAIL");
            materialNames.Add("Tremco Dymonic 100 (Cant at footing and prep rebar)", "20OZ SAUSAGE");
            materialNames.Add("GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS", "1200 SF ROLL");
            materialNames.Add("GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO", "1200 SF ROLL");
            materialNames.Add("PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)", "150 SF ROLL");
            materialNames.Add("TREMCO DYMONIC 100 OR VULKEM 116 (PERIMETER JOINTS)", "20OZ SAUSAGE");
            materialNames.Add("PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)", "150 SF ROLL");
            materialNames.Add("TREMCO DYMONIC 100 OR VULKEM 116 (PLYWOOD JOINTS)", "20OZ SAUSAGE");
            materialNames.Add("PROTECTION MAT (HORIZONTAL ONLY)", "667 SF ROLL");
            materialNames.Add("PB-4 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("TREMDRAIN 1000 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)", "5 GAL PAIL");
            materialNames.Add("TREMDRAIN 1000 (HORIZONTAL ONLY)", "200 SF ROLL");
            materialNames.Add("TERM BAR, VULKEM 116, PINS AND LOADS", "LF");
            materialNames.Add("SUPERSTOP(LF)", "LF");
            materialNames.Add("PENETRATIONS", "EACH");
            materialNames.Add("UNIVERSAL OUTLET", "EACH");
            materialNames.Add("TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)", "LINEAR FEET");
            materialNames.Add("Vulkem Tremproof 250 GC L 30 MILS", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R 30 MILS", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC L 30 MILS(Additional)", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R 30 MILS(Additional)", "5 GAL PAIL");
            if (addMore==1)
            {
                materialNames.Add("Vulkem Tremproof 201 L 30 MILS", "5 GAL PAIL");
                materialNames.Add("Vulkem Tremproof 201 R 30 MILS", "5 GAL PAIL");
                materialNames.Add("Vulkem Tremproof 201 L 30 MILS(Additional)", "5 GAL PAIL");
                materialNames.Add("Vulkem Tremproof 201 R 30 MILS(Additional)", "5 GAL PAIL");
            }
            
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
            materialNames.Add("PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST", "LF");
           
        }
        private void FillMaterialListEx()
        {
            materialNames.Add("191 QD PRIMER AND PREPARATION FOR RE-SURFACE", "1 GALLON");
            materialNames.Add("TREMPRIME MULTI SURFACE (CONCRETE & OTHER)", "3 GAL KIT");
            materialNames.Add("#191 QD INTERLAMINATE PRIMER", "1 GALLON");
            materialNames.Add("Vulkem Tremproof 250 GC L", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R", "5 GAL PAIL");

            materialNames.Add("Vulkem Tremproof 201 L", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 201 R", "5 GAL PAIL");
            materialNames.Add("Tremco Dymonic 100 (Cant at footing and prep rebar)", "20OZ SAUSAGE");
            materialNames.Add("GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS", "1200 SF ROLL");
            materialNames.Add("GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO", "1200 SF ROLL");
            materialNames.Add("PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)", "150 SF ROLL");
            materialNames.Add("TREMCO DYMONIC 100 OR VULKEM 116 (PERIMETER JOINTS)", "20OZ SAUSAGE");
            materialNames.Add("PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)", "150 SF ROLL");
            materialNames.Add("TREMCO DYMONIC 100 OR VULKEM 116 (PLYWOOD JOINTS)", "20OZ SAUSAGE");
            materialNames.Add("PROTECTION MAT (HORIZONTAL ONLY)", "667 SF ROLL");
            materialNames.Add("PB-4 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("TREMDRAIN 1000 (VERTICAL ONLY)", "200 SF ROLL");
            materialNames.Add("CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)", "5 GAL PAIL");
            materialNames.Add("TREMDRAIN 1000 (HORIZONTAL ONLY)", "200 SF ROLL");
            materialNames.Add("TERM BAR, VULKEM 116, PINS AND LOADS", "LF");
            materialNames.Add("SUPERSTOP(LF)", "LF");
            materialNames.Add("PENETRATIONS", "EACH");
            materialNames.Add("UNIVERSAL OUTLET", "EACH");
            materialNames.Add("TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)", "LINEAR FEET");
            materialNames.Add("Vulkem Tremproof 250 GC L 30 MILS", "5 GAL PAIL");
            materialNames.Add("Vulkem Tremproof 250 GC R 30 MILS", "5 GAL PAIL");

            //materialNames.Add("Vulkem Tremproof 250 GC L 30 MILS(Additional)", "5 GAL PAIL");
            //materialNames.Add("Vulkem Tremproof 250 GC R 30 MILS(Additional)", "5 GAL PAIL");

            //materialNames.Add("Vulkem Tremproof 201 L 30 MILS", "5 GAL PAIL");
            //materialNames.Add("Vulkem Tremproof 201 R 30 MILS", "5 GAL PAIL");
            //materialNames.Add("Vulkem Tremproof 201 L 30 MILS(Additional)", "5 GAL PAIL");
            //materialNames.Add("Vulkem Tremproof 201 R 30 MILS(Additional)", "5 GAL PAIL");
            materialNames.Add("Plywood 3/4 & blocking (# of 4x8 sheets)", "4x8 sheets");
            materialNames.Add("Stucco Material Remove and replace (LF)", "LF");
            materialNames.Add("PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST", "LF");

        }
        public override void FetchMaterialValuesAsync(bool hasSetupChanged)
        {
            Dictionary<string, double> qtyList = new Dictionary<string, double>();

            foreach (SystemMaterial item in SystemMaterials)
            {
                if (item.Name == "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)"
                    ||item.Name== "UNIVERSAL OUTLET"
                    ||item.Name== "Plywood 3/4 & blocking (# of 4x8 sheets)"
                    || item.Name == "Stucco Material Remove and replace (LF)"
                    || item.Name == "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST")
                {
                    qtyList.Add(item.Name, item.Qty);
                }

            }
            if (materialNames == null)
            {
                materialNames = new Dictionary<string, string>();
                if (SystemMaterials.Count>27)
                {
                    FillMaterialList(1);
                }
                else
                    FillMaterialListEx();

            }
            var sysMat = GetSystemMaterial(materialNames);
            sysMat = sysMat.Where(x => x != null).ToObservableCollection();
            //remove GC 250 System Material

            List<SystemMaterial> mat250 = sysMat.Where(x => x.Name.Contains("201")).ToList();
            foreach (SystemMaterial item in mat250)
            {
                sysMat.Remove(item);
            }

            #region  Update Special Material Pricing and QTY on JobSetup change
            if (hasSetupChanged)
            {
                for (int i = 0; i < SystemMaterials.Count; i++)
                {

                    double sp = SystemMaterials[i].SpecialMaterialPricing;
                    bool iscbChecked = SystemMaterials[i].IsMaterialChecked;
                    bool iscbEnabled = SystemMaterials[i].IsMaterialEnabled;

                    //SystemMaterials[i] = sysMat[i];

                    //SystemMaterials[i].SpecialMaterialPricing = sp;
                    UpdateMe(sysMat[i]);

                    SystemMaterials[i].UpdateSpecialPricing(sp);

                    if (iscbEnabled)
                    {
                        if (SystemMaterials[i].Name == "Vulkem Tremproof 201 L 30 MILS" || SystemMaterials[i].Name == "Vulkem Tremproof 201 R 30 MILS"
                            || SystemMaterials[i].Name == "Vulkem Tremproof 201 L 30 MILS(Additional)" || SystemMaterials[i].Name == "Vulkem Tremproof 201 R 30 MILS(Additional)"
                            || SystemMaterials[i].Name == "Vulkem Tremproof 250 GC L 30 MILS" || SystemMaterials[i].Name == "Vulkem Tremproof 250 GC R 30 MILS"
                            || SystemMaterials[i].Name == "Vulkem Tremproof 250 GC L 30 MILS(Additional)" || SystemMaterials[i].Name == "Vulkem Tremproof 250 GC R 30 MILS(Additional)")
                        {
                            //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                            SystemMaterials[i].UpdateCheckStatus(iscbChecked);

                        }
                        else
                        {
                            SystemMaterials[i].UpdateCheckStatus(iscbEnabled, iscbChecked);
                            //SystemMaterials[i].IsMaterialEnabled = iscbEnabled;
                            //SystemMaterials[i].IsMaterialChecked = iscbChecked;
                        }
                        
                    }

                    if (SystemMaterials[i].Name == "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)"
                        || SystemMaterials[i].Name == "UNIVERSAL OUTLET"
                        || SystemMaterials[i].Name == "Plywood 3/4 & blocking (# of 4x8 sheets)"
                        || SystemMaterials[i].Name == "Stucco Material Remove and replace (LF)"||
                        SystemMaterials[i].Name == "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST")
                    {
                        if (qtyList.ContainsKey(SystemMaterials[i].Name))
                        {
                            SystemMaterials[i].UpdateQuantity(qtyList[SystemMaterials[i].Name]);
                        }
                    }

                }

            }
            #endregion

            else
                SystemMaterials = sysMat;
            
            setExceptionValues(null);
            setCheckBoxes();

            if (OtherMaterials.Count == 0)
            {
                OtherMaterials = GetOtherMaterials();
                OtherLaborMaterials = OtherMaterials;
            }


            if (SubContractLaborItems.Count == 0)
            {
                SubContractLaborItems = GetLaborItems();
            }
            calculateRLqty();
            //CalculateLaborMinCharge(hasSetupChanged);
            //CalculateAllMaterial();
            CalculateCost(null);
            
        }

        public override void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup Js = sender as JobSetup;
            if (Js != null)
            {
                termBar = Js.TermBarLF;
                rebarPrepWalls = Js.RebarPrepWallsLF;
                superStop = Js.SuperStopLF;
                penetrations = Js.Penetrations;
                totalSqftVertical = Js.TotalSqftVertical;
                totalPlywoodSqft = Js.TotalSqftPlywood;
                hasNewPlywood = (bool)Js.IsNewPlywood;
            }
            base.JobSetup_OnJobSetupChange(sender, e);
        }
        public override bool getCheckboxCheckStatus(string materialName)
        {
            
            switch (materialName)
            {
                case "191 QD PRIMER AND PREPARATION FOR RE-SURFACE":
                case "Vulkem Tremproof 250 GC L":
                case "Vulkem Tremproof 250 GC R":
                case "Tremco Dymonic 100 (Cant at footing and prep rebar)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO":
                case "CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                    return true;
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS":
                    return totalSqftVertical> 0 ? true : false;
                case "PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)":
                case "TREMCO DYMONIC 100 OR VULKEM 116 (PERIMETER JOINTS)":
                    return riserCount+totalSqft+totalPlywoodSqft > 0 ? true : false;
                case "PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)":
                case "TREMCO DYMONIC 100 OR VULKEM 116 (PLYWOOD JOINTS)":
                    return hasNewPlywood;
                default:
                    return false;
            }
        }

        public override bool getCheckboxEnabledStatus(string materialName)
        {
            switch (materialName)
            {
                case "191 QD PRIMER AND PREPARATION FOR RE-SURFACE":
                case "TREMPRIME MULTI SURFACE (CONCRETE & OTHER)":
                case "#191 QD INTERLAMINATE PRIMER":
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO":
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS":
                case "PROTECTION MAT (HORIZONTAL ONLY)":
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                    return true;
                case "Vulkem Tremproof 250 GC L 30 MILS":
                //case "Vulkem Tremproof 201 L 30 MILS":
                case "Vulkem Tremproof 250 GC L 30 MILS(Additional)":
                //case "Vulkem Tremproof 201 L 30 MILS(Additional)":
                    //return totalSqft + totalPlywoodSqft > 0 ? true : false;
                case "Vulkem Tremproof 250 GC R 30 MILS":
                //case "Vulkem Tremproof 201 R 30 MILS":
                case "Vulkem Tremproof 250 GC R 30 MILS(Additional)":
                    //case "Vulkem Tremproof 201 R 30 MILS(Additional)":
                    //return totalSqftVertical + riserCount > 0 ? true : false;
                    return true;
                default:
                    return false;
            }

        }
        public override void CalculateCostPerSqFT()
        {
            CostPerSquareFeet = (totalSqft +totalPlywoodSqft+totalSqftVertical+ riserCount) == 0 ? 0 : Math.Round(TotalMaterialCost / (totalSqft + totalPlywoodSqft + totalSqftVertical + riserCount), 2);
        }
        public override double getlfArea(string materialName)
        {
            switch (materialName)
            {
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                case "PROTECTION MAT (HORIZONTAL ONLY)":
                    return (totalSqft + totalPlywoodSqft) + riserCount * stairWidth;
                case "Vulkem Tremproof 250 GC L":
                case "Vulkem Tremproof 201 L":
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO":
                case "Vulkem Tremproof 250 GC L 30 MILS":
                case "Vulkem Tremproof 201 L 30 MILS":
                case "Vulkem Tremproof 250 GC L 30 MILS(Additional)":
                case "Vulkem Tremproof 201 L 30 MILS(Additional)":
                    return totalPlywoodSqft + totalSqft;

                case "Vulkem Tremproof 250 GC R":
                case "Vulkem Tremproof 201 R":
                case "Vulkem Tremproof 201 R 30 MILS":
                case "Vulkem Tremproof 250 GC R 30 MILS":
                case "Vulkem Tremproof 201 R 30 MILS(Additional)":
                case "Vulkem Tremproof 250 GC R 30 MILS(Additional)":
                    return totalSqftVertical + riserCount * stairWidth * 2;

                case "Tremco Dymonic 100 (Cant at footing and prep rebar)":
                    return totalSqftVertical / 10 + rebarPrepWalls;
                //case "":
                //    return totalSqftVertical / 10 + penetrations;

                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS":
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                case "CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)":
                    return totalSqftVertical;

                case "PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)":
                case "TREMCO DYMONIC 100 OR VULKEM 116 (PLYWOOD JOINTS)":
                    return totalPlywoodSqft / 32 * 12 + riserCount * stairWidth * 2;

                case "PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)":
                case "TREMCO DYMONIC 100 OR VULKEM 116 (PERIMETER JOINTS)":
                    return deckPerimeter + riserCount * 2 * 2;
                case "TERM BAR, VULKEM 116, PINS AND LOADS":
                    
                case "UNIVERSAL OUTLET":
                case "SUPERSTOP(LF)":
                case "PENETRATIONS":
                case "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)":
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return 0;
                
                default:
                    return (totalSqftVertical + totalSqft + totalPlywoodSqft)+riserCount*stairWidth*2;
            }
        }

        public override double getQuantity(string materialName, double coverage, double lfArea)
        {
            switch (materialName)
            {
                case "TERM BAR, VULKEM 116, PINS AND LOADS":
                    return Ceiling(termBar, 10);
                case "UNIVERSAL OUTLET":
                case "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST":
                    return 0;
                case "SUPERSTOP(LF)":
                    return superStop;
                case "PENETRATIONS":
                    return penetrations;
                default:
                    return coverage == 0 ? 0 : lfArea / coverage;
            }
        }
    

        public override double getSqFtAreaH(string materialName)
        {
            
            switch (materialName)
            {
                case "PARATERM BAR LF":
                    return deckPerimeter;
                case "191 QD PRIMER AND PREPARATION FOR RE-SURFACE":
                case "TREMPRIME MULTI SURFACE (CONCRETE & OTHER)":
                case "#191 QD INTERLAMINATE PRIMER":
                case "Vulkem Tremproof 250 GC L":
                case "Vulkem Tremproof 201 L":
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO":
                case "PROTECTION MAT (HORIZONTAL ONLY)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                case "Vulkem Tremproof 201 L 30 MILS":
                case "Vulkem Tremproof 250 GC L 30 MILS":
                case "Vulkem Tremproof 201 L 30 MILS(Additional)":
                case "Vulkem Tremproof 250 GC L 30 MILS(Additional)":
                    return totalPlywoodSqft+totalSqft;
                case "PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)":
                    return deckPerimeter;
                case "PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)":
                    return totalPlywoodSqft / 32 * 12;


                default:
                    return 0;
            }

        }
        public override double getSqftAreaVertical(string materialName)
        {
            switch (materialName)
            {
                case "191 QD PRIMER AND PREPARATION FOR RE-SURFACE":
                case "TREMPRIME MULTI SURFACE (CONCRETE & OTHER)":
                case "#191 QD INTERLAMINATE PRIMER":
                case "Vulkem Tremproof 250 GC R":
                case "Vulkem Tremproof 201 R":
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) WALLS":
                case "PB-4 (VERTICAL ONLY)":
                case "TREMDRAIN 1000 (VERTICAL ONLY)":
                case "CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)":
                case "Vulkem Tremproof 201 R 30 MILS":
                case "Vulkem Tremproof 250 GC R 30 MILS":
                case "Vulkem Tremproof 201 R 30 MILS(Additional)":
                case "Vulkem Tremproof 250 GC R 30 MILS(Additional)":
                    return totalSqftVertical;
                
                case "Tremco Dymonic 100 (Cant at footing and prep rebar)":
                    return rebarPrepWalls;
                //case "Tremco Dymonic 100 (Cant at footing and prep rebar)":
                //    return penetrations;
                case "GLASSMAT #II (FROM MERKOTE / LOWRYS) FLOORS YES/NO":
                    return deckPerimeter;
                case "TERM BAR, VULKEM 116, PINS AND LOADS":
                    return Ceiling(termBar, 10);
                case "UNIVERSAL OUTLET":
                    return 0;
                case "SUPERSTOP(LF)":
                    return superStop;
                case "PENETRATIONS":
                    return penetrations;
                default:
                    return 0;
            }
        }
        public override double getSqFtStairs(string materialName)
        {
            switch (materialName)
            {
                case "191 QD PRIMER AND PREPARATION FOR RE-SURFACE":
                case "TREMPRIME MULTI SURFACE (CONCRETE & OTHER)":
                case "#191 QD INTERLAMINATE PRIMER":
                case "Vulkem Tremproof 250 GC L":
                case "Vulkem Tremproof 250 GC R":
                case "PW POLYESTER FABRIC FROM UPI 4\"(PLYWOOD SEAMS)":
                case "TREMDRAIN 1000 (HORIZONTAL ONLY)":
                case "Vulkem Tremproof 201 R 30 MILS":
                case "Vulkem Tremproof 250 GC R 30 MILS":
                case "Vulkem Tremproof 201 R 30 MILS(Additional)":
                case "Vulkem Tremproof 250 GC R 30 MILS(Additional)":
                case "Vulkem Tremproof 201 L":
                case "Vulkem Tremproof 201 R":
                    return riserCount*stairWidth*2;
                case "PW POLYESTER FABRIC FROM UPI 4\"(PERIMETER)":
                    return riserCount * 2 * 2;
                case "PROTECTION MAT (HORIZONTAL ONLY)":
                    return riserCount * stairWidth;
                
                default:
                    return 0;
            }
            
        }

        public override void calculateLaborHrs()
        {
            calLaborHrs(10, totalSqft);

        }
        //calculate for Desert Crete
        public override void calculateRLqty()
        {
            SystemMaterial sysMat;
            sysMat = SystemMaterials.Where(x => x.Name == "TERM BAR, VULKEM 116, PINS AND LOADS").First();
            if (sysMat!=null)
            {
                sysMat.Qty= Ceiling(termBar, 10);
            }
            sysMat = SystemMaterials.Where(x => x.Name == "SUPERSTOP(LF)").First();
            if (sysMat != null)
            {
                sysMat.Qty = superStop;
            }
            sysMat = SystemMaterials.Where(x => x.Name == "PENETRATIONS").First();
            if (sysMat != null)
            {
                sysMat.Qty = penetrations;
            }
            
        }

        public override bool canApply(object obj)
        {
            return true;
        }
        public override void setExceptionValues(object s)
        {
            if (SystemMaterials.Count != 0)
            {

                SystemMaterial item = SystemMaterials.Where(x => x.Name == "UNIVERSAL OUTLET").FirstOrDefault();
                if (item != null)
                {

                    item.VerticalSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate,item.VerticalSqft,item.VerticalProductionRate);

                    item.LaborExtension = (item.Hours != 0) ? (item.SetupMinCharge + item.Hours) * laborRate : 0;
                    item.LaborUnitPrice = item.LaborExtension / (riserCount + totalSqft);

                }

                item = SystemMaterials.Where(x => x.Name == "TOTAL DRAIN MINUS BOTTOM TD 1000(IN LIEU OF ROCK & PIPE)").FirstOrDefault();
                if (item != null)
                {
                    item.VerticalSqft = item.Qty;
                    //item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.VerticalSqft, item.VerticalProductionRate);

                    item.LaborExtension = item.Hours==0?0:item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "Plywood 3/4 & blocking (# of 4x8 sheets)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty*32;
                    //item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.Hours==0?0:item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "Stucco Material Remove and replace (LF)").FirstOrDefault();
                if (item != null)
                {
                    item.SMSqftH = item.Qty;
                    //item.SMSqft = item.Qty;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate);

                    item.LaborExtension = item.Hours==0?0:item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
                item = SystemMaterials.Where(x => x.Name == "PRIME AND ONE COAT OF VULKEM 801 ALUMINUM ROOF COATING @ WALL WITH SAND BROADCAST").FirstOrDefault();
                if (item != null)
                {
                    item.VerticalSqft = item.Qty;
                    item.SMSqft = item.Qty*0.5;
                    item.Hours = CalculateHrs(item.SMSqftH, item.HorizontalProductionRate, item.StairSqft, item.StairsProductionRate, item.VerticalSqft, item.VerticalProductionRate);

                    item.LaborExtension = item.Hours == 0 ? 0 : item.SetupMinCharge > item.Hours ? item.SetupMinCharge * laborRate : item.Hours * laborRate;
                    item.LaborUnitPrice = item.LaborExtension / item.Qty;
                }
            }   
            //CalculateLaborMinCharge(false);
        }
        
        public override bool IncludedInLaborMin(string matName)
        {
            switch (matName)
            {
                case "Plywood 3/4 & blocking (# of 4x8 sheets)":
                case "Stucco Material Remove and replace (LF)":
                    return false;
                default:
                    return true;
            }            
        }

        public override void UpdateSumOfSqft()
        {
            double sumVal = totalSqft + totalPlywoodSqft + totalSqftVertical;
            TotalLaborUnitPrice = sumVal == 0 ? 0 : TotalLaborWithoutDrive / sumVal;
            RaisePropertyChanged("TotalLaborUnitPrice");
        }
        public override void ApplyCheckUnchecks(object obj)
        {
            lastCheckedMat = obj.ToString();
            SystemMaterial sysmat = null;
            bool ischecked = false, ischecked1 = false;
            if (obj.ToString() == "TREMDRAIN 1000 (VERTICAL ONLY)" || obj.ToString() == "TREMDRAIN 1000 (HORIZONTAL ONLY)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
                ischecked = sysmat.IsMaterialChecked;
                sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (HORIZONTAL ONLY)").FirstOrDefault();
                ischecked1 = sysmat.IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)").FirstOrDefault().IsMaterialChecked = ischecked || ischecked1;
            }

            if (obj.ToString() == "Vulkem Tremproof 250 GC L 30 MILS")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "Vulkem Tremproof 250 GC L 30 MILS").FirstOrDefault();
                ischecked = sysmat.IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Vulkem Tremproof 250 GC R 30 MILS").FirstOrDefault().IsMaterialChecked = ischecked;
            }
            if (obj.ToString() == "Vulkem Tremproof 250 GC L 30 MILS(Additional)")
            {
                sysmat = SystemMaterials.Where(x => x.Name == "Vulkem Tremproof 250 GC L 30 MILS(Additional)").FirstOrDefault();
                ischecked = sysmat.IsMaterialChecked;
                SystemMaterials.Where(x => x.Name == "Vulkem Tremproof 250 GC R 30 MILS(Additional)").FirstOrDefault().IsMaterialChecked = ischecked;
            }

            calculateRLqty();
            //CalculateLaborMinCharge(false);
        }

        
        public override double CalculateLabrExtn(double calhrs, double setupMin, string matName = "")
        {
            if (calhrs==0)
            {
                return 0;
            }
            else
                return setupMin > calhrs ? setupMin * laborRate : calhrs * laborRate;
        }
        public override void setCheckBoxes()
        {
            SystemMaterial sysmat = SystemMaterials.Where(x => x.Name == "TREMDRAIN 1000 (VERTICAL ONLY)").FirstOrDefault();
            SystemMaterials.Where(x => x.Name == "CALIFORNIA SEALER FROM LOWRYS (GLUING DRAIN MAT)").FirstOrDefault().IsMaterialChecked = sysmat.IsMaterialChecked;

            SystemMaterial mat = SystemMaterials.Where(x => x.Name == "TERM BAR, VULKEM 116, PINS AND LOADS").FirstOrDefault();
            if (mat != null)
            {
                mat.IsMaterialChecked = mat.Qty > 0 ? true : false;
            }
            mat = SystemMaterials.Where(x => x.Name == "SUPERSTOP(LF)").FirstOrDefault();
            if (mat != null)
            {
                mat.IsMaterialChecked = mat.Qty > 0 ? true : false;
            }
            mat = SystemMaterials.Where(x => x.Name == "PENETRATIONS").FirstOrDefault();
            if (mat != null)
            {
                mat.IsMaterialChecked = mat.Qty > 0 ? true : false;
            }
        }

        private double Ceiling(double value, double significance)
        {
            if ((value % significance) != 0)
            {
                return ((int)(value / significance) * significance) + significance;
            }

            return Convert.ToDouble(value);
        }

        public override void CalculateTotalSqFt()
        {
            if ((totalSqft + totalPlywoodSqft + totalSqftVertical) == 0)
            {
                CostperSqftSlope = 0;
                CostperSqftMetal = 0;
                CostperSqftMaterial = 0;
                CostperSqftSubContract = 0;
            }
            else
            {
                CostperSqftSlope = TotalSlopingPrice / (totalSqft + totalPlywoodSqft + totalSqftVertical);
                CostperSqftMetal = TotalMetalPrice / (totalSqft + totalPlywoodSqft + totalSqftVertical);
                CostperSqftMaterial = TotalSystemPrice / (totalSqft + totalPlywoodSqft + totalSqftVertical);
                CostperSqftSubContract = TotalSubcontractLabor / (totalSqft + totalPlywoodSqft + totalSqftVertical);
            }
            TotalCostperSqft = CostperSqftSlope + CostperSqftMetal + CostperSqftMaterial + CostperSqftSubContract;
            RaisePropertyChanged("CostperSqftSlope");
            RaisePropertyChanged("CostperSqftMetal");
            RaisePropertyChanged("CostperSqftMaterial");
            RaisePropertyChanged("CostperSqftSubContract");
            RaisePropertyChanged("TotalCostperSqft");
        }
        public override double getLaborUnitPrice(double laborExtension, double riserCount, double totalSqft, double sqftVert = 0, double sqftHor = 0,
            double sqftStairs = 0, string materialName = "")
        {
            //return base.getLaborUnitPrice(laborExtension, riserCount, totalSqft);

            return (totalSqft + totalPlywoodSqft + totalSqftVertical+riserCount) == 0 ? 0 : laborExtension / (totalSqft + totalPlywoodSqft + totalSqftVertical + riserCount);


        }
    }
}
