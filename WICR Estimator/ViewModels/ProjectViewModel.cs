using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{
    class ProjectViewModel : BaseViewModel, IPageViewModel
    {
        private static bool IsgoogleApiCalled;
        private ObservableCollection<Project> enabledProjects;
        public ProjectViewModel(ObservableCollection<Project> enabledProjects)
            :this()
        {
            EnabledProjects = enabledProjects;
        }


        public ProjectViewModel()
        {  
            initializeApp();                                      
        }
        #region Properties
        
        public ObservableCollection<Project> EnabledProjects
        {
            get
            {
                return enabledProjects;
            }
            set
            {
                if (enabledProjects != value)
                {
                    enabledProjects = value;
                    OnPropertyChanged("EnabledProjects");
                }
            }
        }
        #endregion

        #region Methods
        private async void initializeApp()
        {
            if (!IsgoogleApiCalled)
            {
                IsgoogleApiCalled = true;
                var values = DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate);
                if (values == null)
                {
                    DataSerializer.DSInstance.googleData = new GSData();
                    DataSerializer.DSInstance.googleData.LaborRate = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E2");

                    DataSerializer.DSInstance.googleData.MetalData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "F3:M24");

                    DataSerializer.DSInstance.googleData.SlopeData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "P25:Q30");
                    DataSerializer.DSInstance.googleData.MaterialData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "H33:N59");
                    DataSerializer.DSInstance.googleData.LaborData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E60:E80");
                    DataSerializer.DSInstance.serializeGoogleData(DataSerializer.DSInstance.googleData);
                    System.Threading.Thread.Sleep(500);
                }
            }

            if (EnabledProjects == null)
            {
                EnabledProjects = new ObservableCollection<Project>();
                EnabledProjects = HomeViewModel.MyselectedProjects;
            }
            if (EnabledProjects!=null)
            {
                foreach (Project prj in EnabledProjects)
                {
                    if (prj.ProjectJobSetUp == null)
                    {
                        prj.ProjectJobSetUp = new JobSetup();
                    }
                    if (prj.MetalViewModel == null)
                    {
                        prj.MetalViewModel = new MetalViewModel();
                    }
                    if (prj.SlopeViewModel == null)
                    {
                        prj.SlopeViewModel = new SlopeViewModel();
                    }
                    if (prj.MaterialViewModel == null)
                    {
                        prj.MaterialViewModel = new MaterialViewModel();
                    }
                    if (prj.LaborViewModel == null)
                    {
                        prj.LaborViewModel = new LaborViewModel();
                    }
                }
            }
            

        }
        
        #endregion


        public string Name
        {
            get
            {
                return "Project Page";
            }
        }
    }
}
