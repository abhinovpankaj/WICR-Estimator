using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WICR_Estimator.Models;
using WICR_Estimator.ViewModels;
using WICR_Estimator.Views;

namespace WICR_Estimator.Views
{
    /// <summary>
    /// Interaction logic for SelectedProjectsPage.xaml
    /// </summary>
    public partial class SelectedProjectsPage : UserControl
    {
        public SelectedProjectsPage()
        {
            InitializeComponent();
            
            //this.DataContext = new ProjectViewModel();
        }

        private void JobSetupTab_Loaded(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count==0)
            {
                return;
            }
            var tabControl = e.RemovedItems[0] as TabItem;
            if (tabControl != null)
            {
                if (tabControl.Header.ToString() == "Job Setup")
                {
                    ProjectViewModel vm = this.DataContext as ProjectViewModel;
                    if (vm != null)
                    {

                        //vm.UpdateJobSettings(tabControl.DataContext);
                    }
                }
                if (tabControl.Header.ToString() == "Metal")
                {
                    Project prj=tabControl.DataContext as Project;
                    if (prj!=null)
                    {
                        prj.MetalViewModel.CalculateCost(null);
                    }
                    
                }
                if (tabControl.Header.ToString() == "Slope")
                {
                    Project prj = tabControl.DataContext as Project;
                    if (prj != null)
                    {
                        prj.SlopeViewModel.CalculateAll();
                    }
                }
                if (tabControl.Header.ToString() == "Material")
                {
                    Project prj = tabControl.DataContext as Project;
                    if (prj != null)
                    {
                        prj.MaterialViewModel.CalculateCost(null);
                        prj.ProjectJobSetUp.TotalSalesCostTemp = prj.MaterialViewModel.TotalSale;
                    }

                }
                if (tabControl.Header.ToString() == "Labor")
                {
                    Project prj = tabControl.DataContext as Project;
                    if (prj != null)
                    {
                        prj.MaterialViewModel.CalculateCost(null);
                        prj.ProjectJobSetUp.TotalSalesCostTemp = prj.MaterialViewModel.TotalSale;
                    }

                }


            }

            //Save Project Status
        }

        #region  dragdrop
        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var tabItem = e.Source as TabItem;

            if (tabItem == null)
                return;

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                pStart = e.GetPosition(tabItem);
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        Point pStart;
        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var tabItemTarget = e.Source as TabItem;
                var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;
                var srcProject = tabItemSource.DataContext as Project;
                var targetProject = tabItemTarget.DataContext as Project;
                if (!tabItemTarget.Equals(tabItemSource))
                {
                    var tabControl = GetParent((Visual)tabItemTarget);//tabItemTarget.Parent as TabControl;
                    var projectVM = tabControl.DataContext as ProjectViewModel;

                    int sourceIndex = projectVM.EnabledProjects.IndexOf(srcProject);
                    int targetIndex = projectVM.EnabledProjects.IndexOf(targetProject);

                    var temp = projectVM.EnabledProjects.ElementAt(sourceIndex);
                    projectVM.EnabledProjects.RemoveAt(sourceIndex);
                    projectVM.EnabledProjects.Insert(targetIndex, temp);

                    tabControl.SelectedIndex = targetIndex;

                }
            }
            catch (Exception)
            {

                
            }
            
        }

        private TabControl GetParent(Visual v)
        {
            while (v != null)
            {
                v = VisualTreeHelper.GetParent(v) as Visual;
                if (v is TabControl)
                    break;
            }
            return v as TabControl;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var tbControl = sender as TabControl;
            //ProjectViewModel vm = this.DataContext as ProjectViewModel;
            //if (vm != null)
            //{

            //    vm.ActiveTabIndex= tbControl.SelectedIndex;
            //}
        }
        #endregion
    }
}
