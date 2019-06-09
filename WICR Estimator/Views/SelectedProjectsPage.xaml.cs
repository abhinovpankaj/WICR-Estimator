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
                if (tabControl.Header.ToString()=="Job Setup")
                {
                    ProjectViewModel vm = this.DataContext as ProjectViewModel;
                    if (vm != null)
                    {
                        vm.UpdateJobSettings(tabControl.DataContext);
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
                    }

                }
                if (tabControl.Header.ToString() == "Labor")
                {
                    Project prj = tabControl.DataContext as Project;
                    if (prj != null)
                    {
                        prj.MaterialViewModel.CalculateCost(null);
                    }

                }


            }
        }
    }
}
