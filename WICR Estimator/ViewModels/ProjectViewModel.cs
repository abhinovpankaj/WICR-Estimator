﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WICR_Estimator.Models;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels
{

    class ProjectViewModel : BaseViewModel, IPageViewModel
    {
        //Retain the active tab
        public int ActiveTabIndex { get; set; }
        private ObservableCollection<Project> enabledProjects;
        public ProjectViewModel(ObservableCollection<Project> enabledProjects)
            :this()
        {
            EnabledProjects = enabledProjects;
            
        }
  
        private void HomeViewModel_OnLoggedAsAdmin(object sender, EventArgs e)
        {
            //IsAdminloggedIn = (bool)sender ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; 
            IsAdminloggedIn = (bool)sender;
        }

        public ProjectViewModel()
        {
            EnabledProjects = new ObservableCollection<Project>();
            EnabledProjects = HomeViewModel.MyselectedProjects;
            
            HomeViewModel.OnLoggedAsAdmin += HomeViewModel_OnLoggedAsAdmin;
            HomeViewModel.OnProjectSelectionChange += HomeViewModel_OnProjectSelectionChange;
            
            
        }
        List<Project> newlyAddedProjects;
        private void EnabledProjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            newlyAddedProjects = e.NewItems as List<Project>;

        }

        public void UpdateJobSettings(object obj)
        {
            Project prj = obj as Project;
            if (prj!=null)
            {
                prj.ProjectJobSetUp.UpdateJobSetup();
            }
            //foreach (Project item in EnabledProjects)
            //{
            //    item.ProjectJobSetUp.UpdateJobSetup();
            //}
        }

        private void HomeViewModel_OnProjectSelectionChange(object sender, EventArgs e)
        {


            EnabledProjects = sender as ObservableCollection<Project>;

            //if (EnabledProjects != null)
            //{
            //    EnabledProjects.CollectionChanged += EnabledProjects_CollectionChanged;
            //}
            initializeApp();
        }


        #region Properties
        private static bool isAdminloggedIn;

        public static bool IsAdminloggedIn
        {
            get { return isAdminloggedIn; }

            set
            {
                if (value != isAdminloggedIn)
                {
                    isAdminloggedIn = value;
                    //OnPropertyChanged("IsAdminloggedIn");
                }
            }
        }
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
        public string Name
        {
            get
            {
                return "Projects";
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
                    
                    prj.ActiveTabIndex = 0;
                    if (prj.ProjectJobSetUp == null)
                    {
                        
                        prj.ProjectJobSetUp = new JobSetup(prj.OriginalProjectName);

                        prj.ProjectJobSetUp.OnProjectNameChange += ProjectJobSetUp_OnProjectNameChange;
                        prj.ProjectJobSetUp.UpdateJobSetup();
                    }
                    string originalProjectname=prj.OriginalProjectName;
                    //if (prj.Name.Contains('.'))
                    //{
                    //    originalProjectname = prj.Name.Split('.')[0];
                    //}
                    //else
                    //    originalProjectname = prj.Name;
             #region Google
                    var values = DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate, originalProjectname);
                    if (values == null)
                    {
                        DataSerializer.DSInstance.googleData = new GSData();
                        //IList<IList<object>> LaborRate=await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Rate);
                        DataSerializer.DSInstance.googleData.LaborRate = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Rate);

                        DataSerializer.DSInstance.googleData.MetalData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Metal);

                        Thread.Sleep(1000);
                        DataSerializer.DSInstance.googleData.SlopeData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(originalProjectname, DataType.Slope);

                        
                        DataSerializer.DSInstance.googleData.MaterialData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(originalProjectname, DataType.Material);

                        Thread.Sleep(1000);
                        DataSerializer.DSInstance.googleData.LaborData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(originalProjectname, DataType.Labor);
                        DataSerializer.DSInstance.googleData.FreightData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Freight);

                        Thread.Sleep(2000);
                        DataSerializer.DSInstance.serializeGoogleData(DataSerializer.DSInstance.googleData, originalProjectname);

                    }

                    #endregion

                    #region DBConnectAndSaveDataLocally

                    //var dbValues = DataSerializerService.DSInstance.deserializeDbData( originalProjectname);
                    //if (dbValues == null)
                    //{
                        DataSerializerService.DSInstance.dbData = new DBData();

                        var project = await HTTPHelper.GetProjectByNameAsync(originalProjectname);

                        DataSerializerService.DSInstance.dbData.LaborDBData = await HTTPHelper.GetLaborFactorsAsyncByProjectID(project.ProjectId);


                        DataSerializerService.DSInstance.dbData.MetalDBData = await HTTPHelper.GetMetalsAsync();


                        DataSerializerService.DSInstance.dbData.SlopeDBData = await HTTPHelper.GetSlopesByProjectIDAsync(project.ProjectId);


                        DataSerializerService.DSInstance.dbData.MaterialDBData = await HTTPHelper.GetMaterialsAsyncByID(project.ProjectId);

                        
                        DataSerializerService.DSInstance.dbData.FreightDBData = await HTTPHelper.GetFreightsAsync();                      

                        DataSerializerService.DSInstance.serializeDbData(DataSerializerService.DSInstance.dbData, originalProjectname);
                    //}

                    #endregion

                    double laborRate = 0;
                    var rate = DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate, originalProjectname);
                    if (rate!=null)
                    {
                        double.TryParse(rate[0][0].ToString(), out laborRate);
                        prj.ProjectJobSetUp.LaborRate = laborRate;

                    }
                    

                    if (originalProjectname == "Paraseal")
                    {
                        if (prj.MaterialViewModel == null)
                        {

                            prj.MaterialViewModel = ViewModelInstanceFactory.GetMaterialViewModelInstance(originalProjectname, null,
                                null, prj.ProjectJobSetUp);
                        }
                        
                    }
                    else if(originalProjectname == "860 Carlisle")
                    {
                        if (prj.SlopeViewModel == null)
                        {
                            prj.SlopeViewModel = ViewModelInstanceFactory.GetSlopeViewModelInstance(originalProjectname, prj.ProjectJobSetUp);
                        }
                        if (prj.MaterialViewModel == null)
                        {
                            prj.MaterialViewModel = ViewModelInstanceFactory.GetMaterialViewModelInstance(originalProjectname, null,
                                  prj.SlopeViewModel.SlopeTotals, prj.ProjectJobSetUp);
                        }
                    }
                    else if (originalProjectname=="Paraseal LG"|| originalProjectname == "Westcoat Epoxy"||originalProjectname== "Polyurethane Injection Block" || originalProjectname == "Block Wall")
                    {
                        if (prj.MetalViewModel == null)
                        {
                            prj.MetalViewModel = ViewModelInstanceFactory.GetMetalViewModelInstance(originalProjectname, prj.ProjectJobSetUp);
                        }
                        if (prj.MaterialViewModel == null)
                        {
                            prj.MaterialViewModel = ViewModelInstanceFactory.GetMaterialViewModelInstance(originalProjectname, prj.MetalViewModel.MetalTotals,
                                null, prj.ProjectJobSetUp);
                        }
                    }
                    else
                    {
                        if (prj.MetalViewModel == null)
                        {
                            prj.MetalViewModel = ViewModelInstanceFactory.GetMetalViewModelInstance(originalProjectname, prj.ProjectJobSetUp);
                        }
                        
                        if (prj.SlopeViewModel == null)
                        {
                            prj.SlopeViewModel = ViewModelInstanceFactory.GetSlopeViewModelInstance(originalProjectname, prj.ProjectJobSetUp);
                        }
                        if (prj.MaterialViewModel == null)
                        {
                            if (prj.SlopeViewModel!=null)
                            {
                                prj.MaterialViewModel = ViewModelInstanceFactory.GetMaterialViewModelInstance(originalProjectname, prj.MetalViewModel.MetalTotals,
                                prj.SlopeViewModel.SlopeTotals, prj.ProjectJobSetUp);
                            }
                            else
                                prj.MaterialViewModel = ViewModelInstanceFactory.GetMaterialViewModelInstance(originalProjectname, prj.MetalViewModel.MetalTotals,
                                null, prj.ProjectJobSetUp);


                        }
                    }
                    prj.ProjectJobSetUp.TotalSalesCostTemp = prj.MaterialViewModel.TotalSale;
                    
                }
            }          

        }

        private void ProjectJobSetUp_OnProjectNameChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                if (js.WorkArea!="")
                {
                    var prj = EnabledProjects.Where(x => x.WorkArea == js.WorkArea && x.OriginalProjectName != js.ProjectName);
                    if (prj.Count() != 0)
                    {
                        MessageBox.Show("Same Workarea name has already been applied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        js.WorkArea = "";
                        return;
                    }
                }
                
            }
            foreach (Project item in EnabledProjects)
            {
                
                if (item.ProjectJobSetUp.WorkArea != null)
                {
                    if (item.ProjectJobSetUp.WorkArea != "")
                    {
                        item.Name = item.ProjectJobSetUp.WorkArea;
                    }
                    else
                        item.Name = item.ProjectJobSetUp.ProjectName;
                }

            }

        }

        private void ProjectJobSetUp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        #endregion

       
    }
}
