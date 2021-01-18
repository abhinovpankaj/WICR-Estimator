using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using WICR_Estimator.DBModels;
using WICR_Estimator.Models;
using WICR_Estimator.Services;
using WICR_Estimator.ViewModels;
using WICR_Estimator.ViewModels.DataViewModels;
using WICR_Estimator.ViewModels.PaletteSpecifics;
using WICR_Estimator.Views;

namespace WICR_Estimator
{
    class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        private ICommand _changePageCommand;
        private DelegateCommand _restartAppCommand;
        private DelegateCommand _saveEstimateCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private DelegateCommand _minimizeCommand;
        
        private DelegateCommand _navigationCommand;
        private DelegateCommand _logoutCommand;
        #endregion
        public bool IsUserAdmin { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public bool LoginEnabled { get; set; }
        public string Username { get; set; }
        public MainWindowViewModel()
        {
            // Add available pages
           
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new ProjectViewModel(HomeViewModel.MyselectedProjects));
            PageViewModels.Add(new MaterialDetailsPageViewModel());
            PageViewModels.Add(new LoginPageViewModel());
            PageViewModels.Add(new SlopeDetailsPageViewModel());
            PageViewModels.Add(new MetalDetailsPageViewModel());
            PageViewModels.Add(new LaborFactorDetailsPageViewModel());
            // Set starting page
            CurrentPageViewModel = PageViewModels[3];
            CurWindowState = WindowState.Maximized;
            LoginPageViewModel.OnLoggedIn += LoginPage_OnLoggedIn;

            IsUserLoggedIn = false;
            //WindowStyle = WindowStyle.SingleBorderWindow;
        }
        public IDialogCoordinator dialogCoordinator;
        private MetroDialogSettings dialogSettings;
        ProgressDialogController controller;
        public MainWindowViewModel(IDialogCoordinator instance)
        {

            IsUserLoggedIn = false;
            dialogCoordinator = instance;
            dialogSettings = new MetroDialogSettings();
            //dialogSettings.ColorScheme = MetroDialogColorScheme.Inverted;
            dialogSettings.AnimateHide = true;
            dialogSettings.AnimateShow = true;
            dialogSettings.CustomResourceDictionary =
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                    };

            // Add available pages

            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new ProjectViewModel(HomeViewModel.MyselectedProjects));
            PageViewModels.Add(new MaterialDetailsPageViewModel());
            PageViewModels.Add(new LoginPageViewModel());
            PageViewModels.Add(new SlopeDetailsPageViewModel());
            PageViewModels.Add(new MetalDetailsPageViewModel());
            PageViewModels.Add(new LaborFactorDetailsPageViewModel());
            PageViewModels.Add(new PaletteSelectorViewModel());
            // Set starting page
            var filePath = Path.GetTempPath() + "wicrlogin.json";
            if (File.Exists(filePath))
            {
                
                string json = File.ReadAllText(filePath);
                UserDB myObj = JsonConvert.DeserializeObject<UserDB>(json);
                IsUserLoggedIn = true;
                LoginEnabled = false;
                OnPropertyChanged("LoginEnabled");
                OnPropertyChanged("IsUserLoggedIn");
                
                IsUserAdmin = myObj.IsAdmin;
                Username = myObj.Username;
                OnPropertyChanged("Username");
                OnPropertyChanged("IsUserAdmin");
                CurrentPageViewModel = PageViewModels[0];
            }
            else
                CurrentPageViewModel = PageViewModels[3];
            CurWindowState = WindowState.Maximized;

            LoginPageViewModel.OnLoggedIn += LoginPage_OnLoggedIn;
            LoginPageViewModel.ProgressStarted += LoginPageViewModel_ProgressStarted;
            BaseViewModel.TaskStarted+= PageViewModel_TaskStarted;
            BaseViewModel.TaskCompleted += PageViewModel_TaskCompleted;
            BaseViewModel.UpdateTask += PageViewModel_UpdateTaskStatus;

        }




        #region EVENTS

       
        private async void PageViewModel_TaskStarted(object sender, EventArgs e)
        {
            
            controller = await dialogCoordinator.ShowProgressAsync(this, "Wait",
             sender.ToString(), false, dialogSettings);
            controller.SetIndeterminate();
        }

        private void PageViewModel_UpdateTaskStatus(object sender, EventArgs e)
        {

           controller.SetMessage(sender.ToString());
        }
        private bool loginFailed;
        private async void PageViewModel_TaskCompleted(object sender, EventArgs e)
        {
            //if (controller.IsOpen)
            //{
            //    await controller.CloseAsync();

            //} 
            if (sender.ToString().Length>0)
            {
                ShowMessage(sender.ToString());
                if (sender.ToString().Contains("Failed to Login"))
                {
                    loginFailed = true;
                }
            }
            
            if (controller?.IsOpen==true)
            {
                await controller.CloseAsync();

            }
            else
                tmr=SetInterval(isControllerOpen, 2000);
        }

        private async void isControllerOpen()
        {
            if (controller?.IsOpen == true)
            {
                tmr.Stop();
                await controller.CloseAsync();
                
            }
        }
        #region Timer to close processing Window
        private System.Timers.Timer tmr;
        public  System.Timers.Timer SetInterval(Action Act, int Interval)
        {
            System.Timers.Timer tmr = new System.Timers.Timer();
            tmr.Elapsed += (sender, args) => Act();
            tmr.AutoReset = true;
            tmr.Interval = Interval;
            tmr.Start();

            return tmr;
        }
        #endregion

        private async void LoginPageViewModel_ProgressStarted(object sender, EventArgs e)
        {
            controller = await dialogCoordinator.ShowProgressAsync(this, "Wait",
              sender.ToString(), false, dialogSettings);
            controller.SetIndeterminate();
            
        }

        private void LoginPage_OnLoggedIn(object sender, EventArgs e)
        {
            if (!loginFailed)
            {
                var user = (UserDB)sender;


                IsUserLoggedIn = true;
                LoginEnabled = false;
                OnPropertyChanged("LoginEnabled");
                OnPropertyChanged("IsUserLoggedIn");

                IsUserAdmin = user.IsAdmin;
                Username = user.Username;
                OnPropertyChanged("Username");
                OnPropertyChanged("IsUserAdmin");
                CurrentPageViewModel = PageViewModels[0];
            }
            
            CloseAsync();
        }
        #endregion
        #region dialogs
        public async Task<MessageDialogResult> ShowActionMessage(string msg,string header)
        {
            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary =
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                    },
                NegativeButtonText = "No",
                AffirmativeButtonText="Yes",
                AnimateHide = true,
                AnimateShow = true,
                FirstAuxiliaryButtonText = "Cancel"
                //ColorScheme= MetroDialogColorScheme.Inverted   
            };

           return await dialogCoordinator.ShowMessageAsync(this, header, msg,MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, metroDialogSettings);
           
        }

        public async Task<MessageDialogResult> ShowActionMessage(string msg, string header,MetroDialogSettings dialogsettings)
        {
            

            return await dialogCoordinator.ShowMessageAsync(this, header, msg, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, dialogsettings);

        }
        public async void ShowMessage(string message)
        {
            
            await dialogCoordinator.ShowMessageAsync(this, "WICR", message,MessageDialogStyle.Affirmative,dialogSettings);
        }

        public async void ShowProgress(string msg)
        {
            // Show...
            controller = await dialogCoordinator.ShowProgressAsync(this, "Wait",
                msg);

            controller.SetIndeterminate();

            // Do your work... 
            //var result = await Task.Run(...);

            // Close...
           
        }
        public async void CloseAsync()
        {
            if (controller==null)
            {
                Thread.Sleep(500);
            }
            await controller?.CloseAsync();
        }
        #endregion

            #region Properties / Commands
            //private static WindowStyle windowStyle;
        public static WindowStyle WindowStyle
        { get; set;
         
        }

        public DelegateCommand RestartAppCommand
        {
            get
            {
                if (_restartAppCommand == null)
                {
                    _restartAppCommand = new DelegateCommand(RestartApp, canRestart);
                }

                return _restartAppCommand;
            }
        }
        public DelegateCommand SaveEstimateCommand
        {
            get
            {
                if (_saveEstimateCommand == null)
                {
                    _saveEstimateCommand = new DelegateCommand(SaveEstimate, canSave);
                }

                return _saveEstimateCommand;
            }
        }
        public DelegateCommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new DelegateCommand(Logout, canlogOut);
                }

                return _logoutCommand;
            }
        }


        private async void RestartApp(object obj)
        {

            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary =
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                    },
                NegativeButtonText = "No",
                AffirmativeButtonText = "Yes",
                AnimateHide = true,
                AnimateShow = true
                
                //ColorScheme= MetroDialogColorScheme.Inverted   
            };
            // Displays the MessageBox.
            var res = await ShowActionMessage("All the Project selection and values will be cleared. \nDo you want to proceed?", "WICR", metroDialogSettings);

            switch (res)
            {
                case MessageDialogResult.Affirmative:
                    Process.Start(System.Windows.Forms.Application.ExecutablePath);
                    Thread.Sleep(1000);
                    Environment.Exit(-1);
                    break;
               
                default:
                    break;
            }
            

        }
        private bool canRestart(object obj)
        {
            return IsUserLoggedIn;
        }
        private bool canlogOut(object obj)
        {
            return IsUserLoggedIn;
        }

        private async void Logout(object obj)
        {
            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary =
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                    },
                NegativeButtonText = "No",
                AffirmativeButtonText = "Yes",
                AnimateHide = true,
                AnimateShow = true

                //ColorScheme= MetroDialogColorScheme.Inverted   
            };
            var res= await ShowActionMessage("Do you want to logout from App?", "WICR - Logged-in As " + Username,metroDialogSettings);
            if (res==MessageDialogResult.Affirmative)
            {
                IsUserLoggedIn = false;
                OnPropertyChanged("IsUserLoggedIn");
                ChangeViewModel(PageViewModels[3]);
                //delete login file
                File.Delete(Path.GetTempPath() + "wicrlogin.json");
            }
            
        }

        private bool canSave(object obj)
        {
            if (ViewModels.BaseViewModel.IsDirty)
            {
                return true;
            }
            else
                return false;
        }
        private async void SaveEstimate(object obj)
        {
           await SaveEstimates(ViewModels.HomeViewModel.MyselectedProjects);
        }

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new DelegateCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        canChange);
                }

                return _changePageCommand;
            }
        }

        private bool canChange(object obj)
        {
            return true;
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
            if (CurrentPageViewModel is HomeViewModel)
            {
                HomeViewModel hvm = CurrentPageViewModel as HomeViewModel;
                hvm.UpdateProjectTotals();
            }
        }

        public async Task SaveEstimates(ObservableCollection<Project> SelectedProjects)
        {
            DateTime? JobCreationDate = DateTime.Now;
            string JobName = string.Empty, PreparedBy = string.Empty;
            ProjectsTotal ProjectTotals=null;

            if (HomeViewModel.filePath == null)
            {

                Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog1.Title = "Save Project Estimate";
                saveFileDialog1.CheckFileExists = false;
                saveFileDialog1.CheckPathExists = false;
                //saveFileDialog1.DefaultExt = "txt";
                saveFileDialog1.Filter = "Project files (*.est)|*.est|All files (*.*)|*.*";
                //saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.ShowDialog();


                if (saveFileDialog1.FileName != "")
                {
                    HomeViewModel.filePath = saveFileDialog1.FileName;
                }
                else
                    return;
            }
            controller = await dialogCoordinator.ShowProgressAsync(this, "WICR",
               "Wait! Saving Estimate...",false,dialogSettings);
            try
            {
               

                controller.SetIndeterminate();

                MainWindowViewModel vm = this;
                HomeViewModel hm = null;
                if (vm != null)
                {
                    hm = vm.PageViewModels.FirstOrDefault(x => x.Name == "Home") as HomeViewModel;
                    if (hm != null)
                    {
                        JobName = hm.JobName;
                        PreparedBy = hm.PreparedBy;
                        JobCreationDate = hm.JobCreationDate;
                        ProjectTotals = hm.ProjectTotals;
                    }
                }
                var serializer = new DataContractSerializer(typeof(ObservableCollection<Project>));
                //Save to DB  part
                controller.SetMessage("Saving estimate to Database...");
                int myEstimateID;
                if (SelectedProjects[0].EstimateID != 0)
                {
                    myEstimateID = SelectedProjects[0].EstimateID;
                }
                else
                {
                    
                    EstimateDB myEstimate = new EstimateDB(JobName, PreparedBy, JobCreationDate, ProjectTotals);
                    //Add Estimate to DB, get EstimateID
                    var estimate = await HTTPHelper.PostEstimate(myEstimate);
                    myEstimateID = estimate.EstimateID;
                }
                //save db ends 
                controller.SetMessage("Estimate Saved to Database Successfully.");
                
                using (var sw = new StringWriter())
                {
                    controller.SetMessage("Saving Estimate Locally...");
                    using (var writer = new XmlTextWriter(HomeViewModel.filePath, null))
                    {
                        writer.Formatting = System.Xml.Formatting.Indented; // indent the Xml so it's human readable
                        foreach (Project item in SelectedProjects)
                        {
                            item.MaterialViewModel.CalculateCost(null);
                            item.UpdateMainTable();
                            if (hm != null)
                                hm.UpdateProjectTotals();

                            item.CreationDetails = JobName + ":;" + PreparedBy + ":;" + JobCreationDate.ToString();
                            item.ProductVersion = "4.0";
                            //Update DB
                            if (item.EstimateID != 0)
                            {
                                //Get the estimate from DB
                                controller.SetMessage("Updating exiting estimate to DB...");
                                ProjectDetailsDB prjDB = new ProjectDetailsDB();
                                prjDB.EstimateID = item.EstimateID;
                                prjDB.LaborCost = item.LaborCost;
                                prjDB.LaborPercentage = item.LaborPercentage;
                                prjDB.MaterialCost = item.MaterialCost;
                                prjDB.SlopeCost = item.SlopeCost;
                                prjDB.MetalCost = item.MetalCost;
                                prjDB.SystemCost = item.SystemNOther;
                                prjDB.CostPerSqFoot = item.CostPerSqFoot;
                                prjDB.TotalCost = item.TotalCost;
                                prjDB.ProjectDetailID = item.ProjectID;
                                prjDB.HasContingencyDisc = item.ProjectJobSetUp.VHasContingencyDisc;
                                prjDB.HasPrevailingWage = item.ProjectJobSetUp.IsPrevalingWage;
                                prjDB.ProjectProfitMargin = item.MaterialViewModel.ProjectProfitMargin;
                                await HTTPHelper.PutProjectDetails(item.ProjectID, prjDB);
                            }
                            else
                            {
                                ProjectDetailsDB prjDB = new ProjectDetailsDB();
                                prjDB.EstimateID = myEstimateID;
                                prjDB.LaborCost = item.LaborCost;
                                prjDB.LaborPercentage = item.LaborPercentage;
                                prjDB.MaterialCost = item.MaterialCost;
                                prjDB.SlopeCost = item.SlopeCost;
                                prjDB.MetalCost = item.MetalCost;
                                prjDB.SystemCost = item.SystemNOther;
                                prjDB.CostPerSqFoot = item.CostPerSqFoot;
                                prjDB.TotalCost = item.TotalCost;
                                prjDB.HasContingencyDisc = item.ProjectJobSetUp.VHasContingencyDisc;
                                prjDB.HasPrevailingWage = item.ProjectJobSetUp.IsPrevalingWage;
                                prjDB.ProjectProfitMargin = item.MaterialViewModel.ProjectProfitMargin;
                                ProjectDetailsDB prj = await HTTPHelper.PostProjectDetails(prjDB);
                                if (prj != null)
                                {
                                    item.ProjectID = prj.ProjectDetailID;
                                }
                                else
                                {
                                    //System.Windows.MessageBox.Show("Failed to Post the project to DB", "Failure");
                                    controller.SetMessage("Failed to Post the project to DB.");
                                    //ShowMessage("Failed to Post the project to DB.");
                                }

                            }
                            item.EstimateID = myEstimateID;
                        }
                        
                        if (hm != null)
                            hm.UpdateProjectTotals();

                        var updatedEstimate = new EstimateDB(JobName, PreparedBy, JobCreationDate, ProjectTotals);
                        updatedEstimate.EstimateID = myEstimateID;
                        await HTTPHelper.PutEstimate(myEstimateID, updatedEstimate);
                        //Ends

                        serializer.WriteObject(writer, SelectedProjects);
                        
                        writer.Flush();
                        
                        controller.SetMessage("Project Estimate Saved locally.");
                        
                    }                    
                }
                ViewModels.BaseViewModel.IsDirty = false;
                
            }
            catch (Exception ex )
            {
                // SetBalloonTip("Failed to Save the Project Estimate.");
                //ShowMessage("Failed to Save the Project Estimate.");
                controller.SetMessage("Failed to Save the Project Estimate."+ ex.Message);
            }
           
            await controller.CloseAsync();

        }
        #endregion
        private void SetBalloonTip(string tip)
        {
            NotifyIcon statusNotifier = new NotifyIcon();

            statusNotifier.Icon = SystemIcons.Information;
            statusNotifier.BalloonTipTitle = "WICR";
            statusNotifier.BalloonTipText = tip;
            statusNotifier.BalloonTipIcon = ToolTipIcon.Info;
            statusNotifier.Visible = true;

            statusNotifier.ShowBalloonTip(1500);

        }

        #region WindowState
        private WindowState _curWindowState;
        public WindowState CurWindowState
        {
            get
            {
                return _curWindowState;
            }
            set
            {
                _curWindowState = value;
                base.OnPropertyChanged("CurWindowState");
            }
        }
        public DelegateCommand MinimizeCommand
        {
            get
            {
                if (_minimizeCommand == null)
                {
                    _minimizeCommand = new DelegateCommand(MinimizeWindow, canMinimize);
                }

                return _minimizeCommand;
            }
        }

        private void MinimizeWindow(object obj)
        {
            CurWindowState = WindowState.Minimized;
        }

        private bool canMinimize(object obj)
        {
            return true;
        }

        public DelegateCommand NavigationCommand
        {
            get
            {
                if (_navigationCommand == null)
                {
                    _navigationCommand = new DelegateCommand(NavigatePage, canNavigate);
                }

                return _navigationCommand;
            }
        }

        private bool canNavigate(object obj)
        {
            return true;
        }

        private void NavigatePage(object obj)
        {
            System.Windows.Controls.ListViewItem listItem = obj as System.Windows.Controls.ListViewItem;
            if (listItem == null)
                return;
            switch (listItem.Name)
            {
                
                case "Estimates":
                    ChangeViewModel(PageViewModels[0]);
                    break;
                case "ProjectList":
                    ChangeViewModel(PageViewModels[1]);
                    break;
                case "Login":
                    ChangeViewModel(PageViewModels[3]);
                    break;
                case "Materials":
                    ChangeViewModel(PageViewModels[2]);
                    break;
                case "Slopes":
                    ChangeViewModel(PageViewModels[4]);
                    break;
                case "Metals":
                    ChangeViewModel(PageViewModels[5]);
                    break;
                case "Laborfactors":
                    ChangeViewModel(PageViewModels[6]);
                    break;
                case "ChangePalette":
                    ChangeViewModel(PageViewModels[7]);
                    break;
                default:
                    break;
            }
            
            
        }
        public ICommand WindowClosingCommand { get; private set; }

        public bool cancelClose;
        public async void OnWindowClosing()
        {
            string result="";
            if (ViewModels.BaseViewModel.IsDirty)
            {
                try {
                    //result =await InputDialog("Do you want to Save the Estimate.", "WICR");
                    var metroDialogSettings = new MetroDialogSettings
                    {
                        CustomResourceDictionary =
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                    },
                        //NegativeButtonText = "No",
                        ////AnimateHide = true,
                        ////AnimateShow = true,
                        ////FirstAuxiliaryButtonText = "Cancel"


                    };

                    result= await dialogCoordinator.ShowInputAsync(this, "Do you want to Save the Estimate.", "WICR",
                        metroDialogSettings);
                    
                }

                
                catch(Exception ex)
                {

                }

                switch (result)

                {
                    case "Yes":
                        SaveEstimateCommand.Execute(null);
                       
                        break;
                    case "Cancel":
                        cancelClose = true;
                        break;
                    case "No":
                        ViewModels.BaseViewModel.IsDirty = false;
                        break;
                    default:
                        break;
                }
            }
            
        }
        
        #endregion

    }

    internal interface ICloseWindow
    {
        Action Close { get; set; }
    }
}

