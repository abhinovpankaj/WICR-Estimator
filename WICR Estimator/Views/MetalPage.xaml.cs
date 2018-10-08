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
using WICR_Estimator.ViewModels;
using WICR_Estimator.Views;

namespace WICR_Estimator.Views
{
    /// <summary>
    /// Interaction logic for MetalPage.xaml
    /// </summary>
    public partial class MetalPage : UserControl
    {
        public MetalViewModel metalVM ;
        public MetalPage()
        {
            InitializeComponent();
                       
        }
    }
}
