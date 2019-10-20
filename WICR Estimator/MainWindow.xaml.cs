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
using WICR_Estimator.Views;

namespace WICR_Estimator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //DateTime d1 = new DateTime(2019, 12, 15);
            //if (DateTime.UtcNow>d1)
            //{
            //    Environment.Exit(-1);
            //}
            
            this.DataContext = new MainWindowViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModels.BaseViewModel.IsDirty)
            {
                MessageBoxResult res= MessageBox.Show("Do you want to Save the State of WICR Estimator.","Save State",MessageBoxButton.YesNoCancel);
                switch (res)
                {                   
                    case MessageBoxResult.Yes:
                        //Save the state.
                        //ViewModels.HomeViewModel.MyselectedProjects
                        break;
                case MessageBoxResult.Cancel:
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                    default:
                        break;
                }
                ViewModels.BaseViewModel.IsDirty = false;
            }
            else
            {

            }
        }
    }
}
