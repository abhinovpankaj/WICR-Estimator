using Microsoft.Win32;
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
using WICR_Estimator.Views;

namespace WICR_Estimator
{
    class MainWindowViewModel:BaseViewModel
    {
        #region Fields

        private ICommand _changePageCommand;
        private DelegateCommand _saveEstimateCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        public MainWindowViewModel()
        {
            // Add available pages
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new ProjectViewModel(HomeViewModel.MyselectedProjects));

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

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
                HomeViewModel hm=null;
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
                            if(hm!=null)
                                hm.UpdateProjectTotals();

                            item.CreationDetails = JobName + ":;" + PreparedBy + ":;" + JobCreationDate.ToString();
                            item.ProductVersion = "2.3";
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
             NotifyIcon statusNotifier=new NotifyIcon();

            statusNotifier.Icon = SystemIcons.Information;
            statusNotifier.BalloonTipTitle = "WICR";
            statusNotifier.BalloonTipText = tip;
            statusNotifier.BalloonTipIcon = ToolTipIcon.Info;
            statusNotifier.Visible = true;
            
            statusNotifier.ShowBalloonTip(1500);

        }
    }
    
}

