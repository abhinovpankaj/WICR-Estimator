using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;
using WICR_Estimator.ViewModels;
namespace WICR_Estimator.ViewModels
{
    public class LaborViewModel : BaseViewModel
    {

        #region Properties
        private ObservableCollection<Labor> Labors;
        private IList<IList<Object>> laborDetails;
        private IList<IList<Object>> materialDetails;
        private double totalSqft;
        private double deckPerimeter;
        private double riserCount;
        private int deckCount;
        private bool isApprovedforCement;
        private bool isPrevailingWage;
        private bool isSpecialMetal;
        private double stairWidth;
        private string weatherWearType;
        private bool isDiscounted;
        private ObservableCollection<SystemMaterial> materialData;
        private Totals materialTotals;
        private Totals metalTotals;
        private Totals slopeTotals;

        #endregion
        #region public properties
        public DelegateCommand CheckboxCommand { get; set; }

        public ObservableCollection<Labor> labor
        {
            get
            {
                return labor;
            }
            set
            {
                if (value != labor)
                {
                    labor = value;
                    OnPropertyChanged("Labor");
                }
            }
        }


        #endregion

        public LaborViewModel()
        {
            Labors = new ObservableCollection<Labor>();
            getLaborDetailsAsync();
            // Labors = CreateLabors();
            JobSetup.OnJobSetupChange += JobSetup_OnJobSetupChange;
                 
        }

        private void MaterialViewModel_OnMaterialsListChanged(object sender, EventArgs e)
        {
            materialData = sender as ObservableCollection<SystemMaterial>;
        }       

        //constructor to get values from Other VMs.
        public LaborViewModel(Totals matTotals,Totals metTotals,Totals slpTotals, ref ObservableCollection<SystemMaterial> sysMaterial)
            : this()
        {
            //use these values to calculate all the Slope,metala nd material Total.calling the same function
            
            materialTotals = matTotals;
            metalTotals = metTotals;
            slopeTotals = slpTotals;
            //materialData = new ObservableCollection<SystemMaterial>();
            materialData = sysMaterial;
        }
        private void JobSetup_OnJobSetupChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                stairWidth = js.StairWidth;
                totalSqft = js.TotalSqft;
                deckPerimeter = js.DeckPerimeter;
                riserCount = js.RiserCount;
                deckCount = js.DeckCount;
                isApprovedforCement = js.IsApprovedForSandCement;
                isPrevailingWage = js.IsPrevalingWage;
                isSpecialMetal = js.HasSpecialMaterial;
                weatherWearType = js.WeatherWearType;
                isDiscounted = js.HasDiscount;             
            }
            getLaborDetailsAsync();
        }

        #region private methods
        private void getLaborDetailsAsync()
        {
            if (laborDetails == null)
            {
                GSData gData=DataSerializer.DSInstance.deserializeGoogleData();
                laborDetails = gData.LaborData;
                materialDetails = gData.MaterialData;
            }
            //Labors = CreateLabors();
        }
        private double Calhours(string Material, double prodRH, double prodRV, double prodRS, double sqH, double sqV, double sqS)
        {
            double calh = 0;

            if (Material != "N")
            {
                if (prodRH == 0 && prodRV == 0 && prodRS == 0)
                {
                    calh = 0;
                }
                else if (prodRH == 0 && prodRV == 0)
                {
                    calh = Math.Round(sqS / prodRS, 2);
                }
                else if (prodRH == 0 && prodRV == 0)
                {
                    calh = Math.Round(sqS / prodRS, 2);
                }
                else if (prodRS == 0 && prodRV == 0)
                {
                    calh = Math.Round(sqH / prodRH, 2);
                }
                else if (prodRH == 0 && prodRS == 0)
                {
                    calh = Math.Round(sqV / prodRV, 2);
                }
                else if (prodRV == 0)
                {
                    calh = Math.Round(((sqH / prodRH) + (sqS / prodRS)), 2);
                }
                else if (prodRH == 0)
                {
                    calh = Math.Round(((sqV / prodRV) + (sqS / prodRS)), 2);
                }
                else if (prodRS == 0)
                {
                    calh = Math.Round(((sqV / prodRV) + (sqH / prodRH)), 2);
                }
                else if (prodRS != 0 && prodRV != 0 && prodRH != 0)
                {
                    calh = Math.Round(((sqV / prodRV) + (sqH / prodRH) + (sqS / prodRS)), 2);
                }
            }
            return calh;
        }
        private double getSqFtAreaH(string materialName)
        {
            switch (materialName.ToUpper())
            {
                case "LIGHT CRACK REPAIR":
                case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                case "CPC MEMBRANE":
                case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                case "NEOTEX STANDARD POWDER(BODY COAT)":
                case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                case "RESISTITE REGULAR WHITE":
                case "RESISTITE REGULAR GRAY":
                case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE REGULAR OR SMOOTH GRAY(KNOCK DOWN OR SMOOTH)":
                case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                case "VISTA PAINT ACRIPOXY":
                case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                    return 1000;
                case "LARGE CRACK REPAIR":
                case "BUBBLE REPAIR MAJOR SQFT":
                    return 0;// from System material
                case "Glasmat #4 (1200 Sq Ft) From Acme":
                case "NEOTEX-38 PASTE":
                case "RESISTITE LIQUID":
                case "Rp Fabric 10 Inch Wide X (300 Lf) From Acme":
                case "Stair Nosing From Dexotex":
                case "Extra Stair Nosing Lf":
                    return 0;
                default:
                    return 0;
            }
        }
        
        //everydetail here while initializing Labors collection would be coming from Materialdetails,
        //need to modify laborDetails array to materialDetails array.
        //private ObservableCollection<Labor> CreateLabors()
        //{
        //    ObservableCollection<Labor> labor = new ObservableCollection<Labor>();

        //    double sp; // Setup minimum charges from google sheet
        //    double prs; ///Production rate stairs from google sheet
        //    double spr;
        //    double sqh;
        //    double lfArea;
        //    string MatName;

        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    // lfArea = getlfArea("Light Crack Repair");
        //    MatName = "Light Crack Repair";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsChecked = true, ///get from Material
        //        IsMaterialChecked = getCheckboxCheckStatus("Light Crack Repair"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Light Crack Repair"),
        //        Name = "Light Crack Repair",

        //        Operation = "CAULK",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)


        //    });

        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);
        //    MatName = "Large Crack Repair";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsChecked = true, ///get from Material
        //        IsMaterialChecked = getCheckboxCheckStatus("Large Crack Repair"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Large Crack Repair"),
        //        Name = "Large Crack Repair",
        //        Operation = "GRIND,BURLAP,BODY,GROUT",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Bubble Repair(Measure Sq Ft)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {

        //        IsMaterialChecked = getCheckboxCheckStatus("Bubble Repair(Measure Sq Ft)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Bubble Repair(Measure Sq Ft)"),
        //        Name = "Bubble Repair(Measure Sq Ft)",
        //        Operation = "CUT OUT, FILL, GLASS, BODY & GROUT",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Resistite Regular Over Texture(#55 Bag)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {

        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Over Texture(#55 Bag)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Over Texture(#55 Bag)"),
        //        Name = "Resistite Regular Over Texture(#55 Bag)",
        //        Operation = "3/32 INCH THICK TROWEL DOWN",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "30# Divorcing Felt (200 Sq Ft) From Ford Wholesale";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {

        //        IsMaterialChecked = getCheckboxCheckStatus("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("30# Divorcing Felt (200 Sq Ft) From Ford Wholesale"),
        //        Name = "30# Divorcing Felt (200 Sq Ft) From Ford Wholesale",
        //        Operation = "SLIP SHEET",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Rp Fabric 10 Inch Wide X (300 Lf) From Acme");
        //    MatName = "Rp Fabric 10 Inch Wide X (300 Lf) From Acme";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {

        //        IsMaterialChecked = getCheckboxCheckStatus("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Rp Fabric 10 Inch Wide X (300 Lf) From Acme"),
        //        Name = "Rp Fabric 10 Inch Wide X (300 Lf) From Acme",
        //        Operation = "DETAIL STAIRS ONLY",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Glasmat #4 (1200 Sq Ft) From Acme");
        //    MatName = "Glasmat #4 (1200 Sq Ft) From Acme";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {

        //        IsMaterialChecked = getCheckboxCheckStatus("Glasmat #4 (1200 Sq Ft) From Acme"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Glasmat #4 (1200 Sq Ft) From Acme"),
        //        Name = "Glasmat #4 (1200 Sq Ft) From Acme",
        //        Operation = "INSTALL FIELD GLASS",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Cpc Membrane";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Cpc Membrane"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Cpc Membrane"),
        //        Name = "Cpc Membrane",
        //        Operation = "SATURATE GLASS & DETAIL PERIMETER",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Neotex-38 Paste";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //  IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Neotex-38 Paste"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Neotex-38 Paste"),
        //        Name = "Neotex-38 Paste",
        //        Operation = "ADD TO BODY COAT",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Neotex Standard Powder(Body Coat)");
        //    MatName = "Neotex Standard Powder(Body Coat)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat)"),
        //        Name = "Neotex Standard Powder(Body Coat)",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Neotex Standard Powder(Body Coat) 1");
        //    MatName = "Neotex Standard Powder(Body Coat) 1";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Neotex Standard Powder(Body Coat) 1"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Neotex Standard Powder(Body Coat) 1"),
        //        Name = "Neotex Standard Powder(Body Coat) 1",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Resistite Liquid");
        //    MatName = "Resistite Liquid";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        // IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Liquid"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Liquid"),
        //        Name = "Resistite Liquid",
        //        Operation = "ADD TO TAN FILLER",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Resistite Regular Gray");
        //    MatName = "Resistite Regular Gray";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        // IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Gray"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Gray"),
        //        Name = "Resistite Regular Gray",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Resistite Regular White";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //  IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular White"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular White"),
        //        Name = "Resistite Regular White",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)";
        //    lfArea = getlfArea("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //  IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Or Smooth Gray(Knock Down Or Smooth)"),
        //        Name = "Resistite Regular Or Smooth Gray(Knock Down Or Smooth)",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Resistite Regular Or Smooth White(Knock Down Or Smooth)";
        //    lfArea = getlfArea("Resistite Regular Or Smooth White(Knock Down Or Smooth)");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //  IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Regular Or Smooth White(Knock Down Or Smooth)"),
        //        Name = "Resistite Regular Or Smooth White(Knock Down Or Smooth)",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Aj-44A Dressing(Sealer)");
        //    MatName = "Aj-44A Dressing(Sealer)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //  IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Aj-44A Dressing(Sealer)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Aj-44A Dressing(Sealer)"),
        //        Name = "Aj-44A Dressing(Sealer)",
        //        Operation = "ROLL 2 COATS",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Vista Paint Acripoxy";
        //    lfArea = getlfArea("Vista Paint Acripoxy");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //   IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Vista Paint Acripoxy"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Vista Paint Acripoxy"),
        //        Name = "Vista Paint Acripoxy",
        //        Operation = "ROLL 2 COATS",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Lip Color");
        //    MatName = "Lip Color";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //  IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Lip Color"), //nOT FOUND
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Lip Color"),
        //        Name = "Lip Color",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });

        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Resistite Universal Primer(Add 50% Water)";
        //    lfArea = getlfArea("Resistite Universal Primer(Add 50% Water)");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Resistite Universal Primer(Add 50% Water)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Resistite Universal Primer(Add 50% Water)"),
        //        Name = "Resistite Universal Primer(Add 50% Water)",
        //        Operation = "PREPARE AND PRIME: SPRAY OR ROLL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth Gray)");
        //    MatName = "Custom Texture Skip Trowel(Resistite Smooth Gray)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        //    IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth Gray)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Custom Texture Skip Trowel(Resistite Smooth Gray)"),
        //        Name = "Custom Texture Skip Trowel(Resistite Smooth Gray)",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)
        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    lfArea = getlfArea("Custom Texture Skip Trowel(Resistite Smooth White)");
        //    MatName = "Custom Texture Skip Trowel(Resistite Smooth White)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        // IsCheckboxDependent = true,
        //        IsMaterialChecked = getCheckboxCheckStatus("Custom Texture Skip Trowel(Resistite Smooth White)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Custom Texture Skip Trowel(Resistite Smooth White)"),
        //        Name = "Custom Texture Skip Trowel(Resistite Smooth White)",
        //        Operation = "TROWEL",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Weather Seal XL two Coats";
        //    lfArea = getlfArea("Weather Seal XL two Coats"); //nOT FOUND
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Weather Seal XL two Coats"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Weather Seal XL two Coats"),
        //        Name = "Weather Seal XL two Coats",
        //        Operation = "CAULK",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Stair Nosing From Dexotex";
        //    lfArea = getlfArea("Stair Nosing From Dexotex");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Stair Nosing From Dexotex"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Stair Nosing From Dexotex"),
        //        Name = "Stair Nosing From Dexotex",
        //        Operation = "NAIL OR SCREW",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Extra Stair Nosing Lf";
        //    lfArea = getlfArea("Extra Stair Nosing Lf");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Extra Stair Nosing Lf"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Extra Stair Nosing Lf"),
        //        Name = "Extra Stair Nosing Lf",
        //        Operation = "NAIL OR SCREW",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Plywood 3/4 & Blocking(# Of 4X8 Sheets)";
        //    lfArea = getlfArea("Plywood 3/4 & Blocking(# Of 4X8 Sheets)");
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Plywood 3/4 & Blocking(# Of 4X8 Sheets)"),
        //        Name = "Plywood 3/4 & Blocking(# Of 4X8 Sheets)",
        //        Operation = "Remove and replace dry rot ",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });
        //    double.TryParse(laborDetails[0][0].ToString(), out sp);
        //    double.TryParse(laborDetails[0][3].ToString(), out prs);
        //    double.TryParse(laborDetails[0][0].ToString(), out spr);
        //    double.TryParse(laborDetails[0][3].ToString(), out sqh);

        //    MatName = "Stucco Material Remove And Replace (Lf)";
        //    sqh = getSqFtAreaH(MatName);
        //    labor.Add(new Labor
        //    {
        //        IsMaterialChecked = getCheckboxCheckStatus("Stucco Material Remove And Replace (Lf)"),
        //        IsMaterialEnabled = getCheckboxEnabledStatus("Stucco Material Remove And Replace (Lf)"),
        //        Name = "Stucco Material Remove And Replace (Lf)",
        //        Operation = "Remove and replace 12 inches of stucco ",
        //        SMSqftV = 0.0,
        //        VerticalProductionRate = 0,
        //        SMSqftH = sqh,
        //        HorizontalProductionRate = prs,
        //        StairsProductionRate = spr,
        //        StairSqft = sp,
        //        Hours = Calhours(MatName, prs, 0, spr, sqh, 0, sp)

        //    });


        //    return labor;
        //}
        
        private bool getCheckboxCheckStatus(string materialName)
        {
            if (weatherWearType == "Weather Wear")
            {
                switch (materialName.ToUpper())
                {
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X (300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX-38 PASTE":
                    case "NEOTEX STANDARD POWDER(BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                    case "RESISTITE LIQUID":
                    case "LIP COLOR":
                    case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                    //case "AJ-44A DRESSING(SEALER)":
                    //case "VISTA PAINT ACRIPOXY":
                    case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH WHITE)":
                    case "WEATHER SEAL XL TWO COATS":
                    case "STAIR NOSING FROM DEXOTEX":
                        return true;
                    default:
                        return false;
                }
            }
            else if (weatherWearType == "Weather Wear Rehab")
            {
                switch (materialName.ToUpper())
                {
                    case "LIGHT CRACK REPAIR":
                    case "RESISTITE REGULAR OVER TEXTURE (#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX-38 PASTE":
                    case "NEOTEX STANDARD POWDER (BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT)1":
                    case "RESISTITE LIQUID":
                    case "RESISTITE REGULAR GRAY":
                    case "RESISTITE REGULAR WHITE":
                    case "RESISTITE REGULAR OR SMOOTH WHITE(KNOCK DOWN OR SMOOTH)":
                    //case "RESISTITE REGULAR OR SMOOTH GRAY (KNOCK DOWN OR SMOOTH)":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING (SEALER)":
                    case "RESISTITE UNIVERSAL PRIMER(ADD 50% WATER)":
                    case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH WHITE)":
                    case "VISTA PAINT ACRIPOXY":
                    case "Stair Nosing From Dexotex":
                    //case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                    case "WEATHER SEAL XL TWO COATS":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        private bool getCheckboxEnabledStatus(string materialName)
        {
            if (weatherWearType == "Weather Wear")
            {
                switch (materialName.ToUpper())
                {
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING(SEALER)":
                    case "VISTA PAINT ACRIPOXY":
                    case "CUSTOM TEXTURE SKIP TROWEL(RESISTITE SMOOTH GRAY)":
                    case "WEATHER SEAL XL TWO COATS":
                    case "STAIR NOSING FROM DEXOTEX":
                        return true;
                    default:
                        return false;
                }
            }
            else if (weatherWearType == "Weather Wear Rehab")
            {
                switch (materialName.ToUpper())
                {
                    case "LIGHT CRACK REPAIR":
                    case "RESISTITE REGULAR OVER TEXTURE(#55 BAG)":
                    case "30# DIVORCING FELT (200 SQ FT) FROM FORD WHOLESALE":
                    case "RP FABRIC 10 INCH WIDE X(300 LF) FROM ACME":
                    case "GLASMAT #4 (1200 SQ FT) FROM ACME":
                    case "CPC MEMBRANE":
                    case "NEOTEX STANDARD POWDER(BODY COAT)":
                    case "NEOTEX STANDARD POWDER(BODY COAT) 1":
                    case "RESISTITE REGULAR GRAY":
                    case "LIP COLOR":
                    case "AJ-44A DRESSING(SEALER)":
                    case "VISTA PAINT ACRIPOXY":
                    case "CUSTOM TEXTURE SKIP TROWEL (RESISTITE SMOOTH GRAY)":
                    case "WEATHER SEAL XL TWO COATS":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        //Caculate Labor Totals
        private void calculateLaborTotals()
        {
            double preWage=0,laborDeduction=0;
            if (isPrevailingWage)
            {
                double.TryParse(laborDetails[0][0].ToString(), out preWage);
            }
            if (isDiscounted)
            {
                double.TryParse(laborDetails[0][1].ToString(), out laborDeduction);
            }
            IEnumerable<Labor> selectedLabors = Labors.Where(x => x.IsMaterialChecked == true).ToList();
            double totalHrs=Math.Round(selectedLabors.Select(x=>x.Hours).Sum(),2);

            double totalLaborUnitPrice = Math.Round(selectedLabors.Select(x => x.LaborUnitPrice).Sum() * (1 + preWage + laborDeduction));
            double totalLaborExtension = Math.Round(selectedLabors.Select(x => x.LaborExtension).Sum());
        }
        private double getTotals(double laborCost,double materialCost,double freightCost,double subcontractLabor)
        {
            double res = 0;
            double slopeTotal = laborCost;
            
            if (isPrevailingWage)
            {
                double.TryParse(laborDetails[4][0].ToString(), out res);
                slopeTotal = slopeTotal + laborCost * res;
            }
            else
            {
                double.TryParse(laborDetails[2][0].ToString(), out res);
                slopeTotal = slopeTotal + laborCost * res;
                double.TryParse(laborDetails[3][0].ToString(), out res);
                slopeTotal = slopeTotal + laborCost * res;
            }
            
            double.TryParse(laborDetails[5][0].ToString(), out res);
            slopeTotal = slopeTotal + laborCost * res;
            double.TryParse(laborDetails[6][0].ToString(), out res);
            double tax = res * (freightCost + materialCost)+materialCost+freightCost;//freight+material including tax

            slopeTotal = slopeTotal + tax;
            //subcontrctlabor
            double.TryParse(laborDetails[8][0].ToString(), out res);
            double subCLabor = subcontractLabor * res;
            //profitMargin
            double pmAdd;
            double.TryParse(laborDetails[8][0].ToString(), out pmAdd);
            double profitMarginAdd = (slopeTotal * pmAdd) *(1+ pmAdd) ;            
            //profit margin
            double pm;
            double.TryParse(laborDetails[10][0].ToString(), out pm);
            double specialMetalDeduction = 0;
            if (isSpecialMetal)
            {
                //Profit deduct for special metal
                double.TryParse(laborDetails[9][0].ToString(), out res);
                specialMetalDeduction = materialCost * res;
            }
            double TotalCost = (slopeTotal / pm) + profitMarginAdd+specialMetalDeduction+subCLabor;

            double.TryParse(laborDetails[11][0].ToString(), out res);
            double generalLiability = TotalCost * res / pm;
            double.TryParse(laborDetails[12][0].ToString(), out res);
            double directExpense = TotalCost * res / pm;
            double.TryParse(laborDetails[13][0].ToString(), out res);
            double contigency = TotalCost * res / pm;
            double ins, fuel, addup;
            double.TryParse(laborDetails[14][0].ToString(), out ins);
            double.TryParse(laborDetails[15][0].ToString(), out fuel);
            double.TryParse(laborDetails[16][0].ToString(), out addup);
            double restTotal = TotalCost * (ins + fuel + addup);
            //calculated Profit Margin,currently not being used.
            double ProfitMargin = TotalCost - slopeTotal;
            return Math.Round(TotalCost + generalLiability + directExpense + contigency + restTotal,2);
        }
                
        #endregion
    }
}
