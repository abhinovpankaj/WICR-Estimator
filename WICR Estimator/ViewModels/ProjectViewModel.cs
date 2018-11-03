﻿using System;
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
        //private static bool IsgoogleApiCalled;
        public System.Windows.Visibility IsAdminloggedIn { get; set; }
        //private bool loginWindowShown;
        private ObservableCollection<Project> enabledProjects;
        public ProjectViewModel(ObservableCollection<Project> enabledProjects)
            :this()
        {
            EnabledProjects = enabledProjects;
            
        }

        private void HomeViewModel_OnLoggedAsAdmin(object sender, EventArgs e)
        {
            IsAdminloggedIn = (bool)sender?System.Windows.Visibility.Visible: System.Windows.Visibility.Hidden;
            OnPropertyChanged("IsAdminloggedIn");
        }

        public ProjectViewModel()
        {
            HomeViewModel.OnLoggedAsAdmin += HomeViewModel_OnLoggedAsAdmin;
            //IsAdminloggedIn = System.Windows.Visibility.Collapsed;
            OnPropertyChanged("IsAdminloggedIn");
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
          
            if (EnabledProjects == null)
            {
                EnabledProjects = new ObservableCollection<Project>();
                EnabledProjects = HomeViewModel.MyselectedProjects;
            }
            if (EnabledProjects!=null)
            {
                foreach (Project prj in EnabledProjects)
                {
                    #region Google
                    var values = DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate, prj.Name);
                        if (values == null)
                        {
                            DataSerializer.DSInstance.googleData = new GSData();

                            //DataSerializer.DSInstance.googleData.LaborRate =  GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E2");

                            DataSerializer.DSInstance.googleData.LaborRate = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name,DataType.Rate);

                            //DataSerializer.DSInstance.googleData.MetalData =  GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "F3:M24");
                            DataSerializer.DSInstance.googleData.MetalData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Metal);

                            //DataSerializer.DSInstance.googleData.SlopeData =  GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "P25:Q30");
                            DataSerializer.DSInstance.googleData.SlopeData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Slope);

                            //DataSerializer.DSInstance.googleData.MaterialData =  GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "H33:N59");
                            DataSerializer.DSInstance.googleData.MaterialData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Material);

                            //DataSerializer.DSInstance.googleData.LaborData =   GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheets("Pricing", "E60:E76");
                            DataSerializer.DSInstance.googleData.LaborData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Labor);

                            DataSerializer.DSInstance.serializeGoogleData(DataSerializer.DSInstance.googleData, prj.Name);
                
                        }
                    
                    #endregion
                    if (prj.ProjectJobSetUp == null)
                    {
                        prj.ProjectJobSetUp = new JobSetup(prj.Name);
                        prj.ProjectJobSetUp.OnProjectNameChange += ProjectJobSetUp_OnProjectNameChange;
                                                                   
                    }
                    if (prj.MetalViewModel == null)
                    {
                        prj.MetalViewModel = ViewModelInstanceFactory.GetMetalViewModelInstance(prj.Name);
                    }
                    if (prj.SlopeViewModel == null)
                    {
                        prj.SlopeViewModel = new SlopeViewModel();
                    }
                    if (prj.MaterialViewModel == null)
                    {
                        prj.MaterialViewModel = new MaterialViewModel(prj.MetalViewModel.MetalTotals, prj.SlopeViewModel.SlopeTotals);
                    }
                }
            }          

        }

        private void ProjectJobSetUp_OnProjectNameChange(object sender, EventArgs e)
        {
            foreach (Project item in EnabledProjects)
            {
                item.Name = item.ProjectJobSetUp.ProjectName;
            }
            //JobSetup js = sender as JobSetup;
            //if (js != null)
            //{

            //}
        }

        private void ProjectJobSetUp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
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
