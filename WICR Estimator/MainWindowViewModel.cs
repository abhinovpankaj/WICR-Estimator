﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using WICR_Estimator.Models;
using WICR_Estimator.ViewModels;
using WICR_Estimator.ViewModels.DataViewModels;
using WICR_Estimator.Views;

namespace WICR_Estimator
{
    class MainWindowViewModel : BaseViewModel, ICloseWindow
    {
        #region Fields

        private ICommand _changePageCommand;
        private DelegateCommand _saveEstimateCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private DelegateCommand _minimizeCommand;
        private DelegateCommand _closeWindowCommand;
        private DelegateCommand _navigationCommand;
        #endregion

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
            CurrentPageViewModel = PageViewModels[0];
            CurWindowState = WindowState.Maximized;
            //WindowStyle = WindowStyle.SingleBorderWindow;
        }

        #region Properties / Commands
        //private static WindowStyle windowStyle;
        public static WindowStyle WindowStyle
        { get; set;
            //get
            //{
            //    return windowStyle;
            //}

            //set
            //{
            //    if (value!=windowStyle)
            //    {
            //        WindowStyle = value;
            //        if (PropertyChanged != null)
            //        {
            //            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //        }
            //}
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

        private bool canSave(object obj)
        {
            if (ViewModels.BaseViewModel.IsDirty)
            {
                return true;
            }
            else
                return false;
        }
        private void SaveEstimate(object obj)
        {
            SaveEstimate(ViewModels.HomeViewModel.MyselectedProjects);
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

        public void SaveEstimate(ObservableCollection<Project> SelectedProjects)
        {
            string JobCreationDate = string.Empty, JobName = string.Empty, PreparedBy = string.Empty;

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
            try
            {
                MainWindowViewModel vm = this;
                HomeViewModel hm = null;
                if (vm != null)
                {
                    hm = vm.PageViewModels.FirstOrDefault(x => x.Name == "Home") as HomeViewModel;
                    if (hm != null)
                    {
                        JobName = hm.JobName;
                        PreparedBy = hm.PreparedBy;
                        JobCreationDate = hm.JobCreationDate.ToString();
                    }
                }
                var serializer = new DataContractSerializer(typeof(ObservableCollection<Project>));

                using (var sw = new StringWriter())
                {
                    using (var writer = new XmlTextWriter(HomeViewModel.filePath, null))
                    {
                        writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                        foreach (Project item in SelectedProjects)
                        {
                            item.MaterialViewModel.CalculateCost(null);
                            item.UpdateMainTable();
                            if (hm != null)
                                hm.UpdateProjectTotals();

                            item.CreationDetails = JobName + ":;" + PreparedBy + ":;" + JobCreationDate.ToString();
                            item.ProductVersion = "3.0";
                        }
                        serializer.WriteObject(writer, SelectedProjects);

                        writer.Flush();
                        //MessageBox.Show("Project Estimate Saved Succesfully", "Success");
                        SetBalloonTip("Project Estimate Saved Succesfully");

                    }
                }
                ViewModels.BaseViewModel.IsDirty = false;
            }
            catch (Exception)
            {

                SetBalloonTip("Failed to Save the Project Estimate.");
            }

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
                default:
                    break;
            }
            
            
        }

        public DelegateCommand CloseCommand
        {
            get
            {
                if (_closeWindowCommand == null)
                {
                    _closeWindowCommand = new DelegateCommand(CloseWindow, canClose);
                }

                return _closeWindowCommand;
            }
        }

        private bool canClose(object obj)
        {
            return true;
        }

        private void CloseWindow(object obj)
        {
            Close?.Invoke();
        }
        public Action Close {get;set;}
        #endregion

    }

    internal interface ICloseWindow
    {
        Action Close { get; set; }
    }
}

