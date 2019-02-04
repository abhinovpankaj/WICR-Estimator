using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
           
    public class Project: BaseViewModel
    {
        public Project()
        {
            if (updatedJobSetup == null)
            {
                updatedJobSetup = new DelegateCommand(UpdateJobSetUp, canUpdate);
            }
        }
        
        public string Name { get; set; }
        public double MetalCost
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalMetalPrice;
                }
                else
                    return 0;
            }
        }
        public double SlopeCost
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSlopingPrice;
                }
                else
                    return 0;
            }
        }
        public double  SystemNOther
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSystemPrice;
                }
                else
                    return 0;
            }
        }
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
        

        public string LaborPercentage
        {
            get
            {
                if (MaterialViewModel != null)
                {
                    return Math.Round(MaterialViewModel.AllTabsLaborTotal/ MaterialViewModel.TotalSale *100,2).ToString()+"%";
                }
                else
                    return "";
            }
        }
        public double TotalCost {
            get
            {
                if (MaterialViewModel != null)
                {
                    return MaterialViewModel.TotalSale;
                }
                else
                    return 0;
            }
        }
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
        public string GrpName { get; set; }
        public string MainGroup { get; set; }

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
        private SlopeBaseViewModel slopeViewModel;
        public SlopeBaseViewModel SlopeViewModel
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
        private MaterialBaseViewModel materialViewModel;
        public MaterialBaseViewModel MaterialViewModel
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
                    OnPropertyChanged("MaterialViewModel");
                }
            }
        }

        
        public static event EventHandler OnSelectedProjectChange;
        
        private ICommand updatedJobSetup;
        [XmlIgnore]
        public ICommand UpdatedJobSetup
        {
            set { updatedJobSetup = value; }
            get
            {                
                return updatedJobSetup;
            }
        }

        private bool canUpdate(object obj)
        {
            return true;
        }

        private void UpdateJobSetUp(object obj)
        {
            if(this.MetalViewModel!=null)
             this.MetalViewModel.CalculateCost(null);
        }

        //private void ProjectJobSetUp_OnJobSetupChange(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
