using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    public class Project: BaseViewModel
    {
        public string Name { get; set; }
        public double MetalCost { get; set; }
        public double SlopeCost { get; set; }
        public double  SystemNOther { get; set; }
        public double SubContractCost { get; set; }
        public double LaborCost
        {
            get
            {
                if (MetalViewModel != null)
                {
                    return MetalViewModel.TotalLaborCost;
                }
                else
                    return 0;
                
            }
        }
        public double MaterialCost
        {
            get
            {
                if (MetalViewModel != null)
                {
                    return MetalViewModel.TotalMaterialCost;
                }
                else
                    return 0;

            }
        }
        public double LaborPercentage { get; set; }
        public double TotalCost { get; set; }
        private bool isSelectedProject;
        public bool IsSelectedProject
        {
            get
            {
                return isSelectedProject;
            }
            set
            {
                if (isSelectedProject != value)
                {
                    isSelectedProject = value;
                    OnPropertyChanged("IsSelectedProject");
                    if (OnSelectedProjectChange!=null)
                    {
                        OnSelectedProjectChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        public int Rank { get; set; }
        public JobSetup ProjectJobSetUp { get; set; }
        private MetalBaseViewModel metalViewModel;
        public MetalBaseViewModel MetalViewModel
        {
            get
            {
                return metalViewModel;
            }
            set
            {
                if (metalViewModel != value)
                {
                    metalViewModel = value;
                    OnPropertyChanged("MetalViewModel");

                }
            }
        }
        private SlopeViewModel slopeViewModel;
        public SlopeViewModel SlopeViewModel
        {
            get
            {
                return slopeViewModel;
            }
            set
            {
                if (slopeViewModel != value)
                {
                    slopeViewModel = value;
                    OnPropertyChanged("SlopeViewModel");

                }
            }
        }
        private MaterialViewModel materialViewModel;
        public MaterialViewModel MaterialViewModel
        {
            get
            {
                return materialViewModel;
            }
            set
            {
                if (materialViewModel != value)
                {
                    materialViewModel = value;
                    
                }
            }
        }
        //private LaborViewModel laborViewModel;
        //public LaborViewModel LaborViewModel
        //{
        //    get
        //    {
        //        return laborViewModel;
        //    }
        //    set
        //    {
        //        if (laborViewModel != value)
        //        {
        //            laborViewModel = value;
        //            OnPropertyChanged("LaborViewModel");

        //        }
        //    }
        //}
        public static event EventHandler OnSelectedProjectChange;
    }
}
