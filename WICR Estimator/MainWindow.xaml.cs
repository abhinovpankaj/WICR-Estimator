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
            //DateTime d1 = new DateTime(2019, 7, 15);
            //if (DateTime.UtcNow>d1)
            //{
            //    Environment.Exit(-1);
            //}
            
            this.DataContext = new MainWindowViewModel();
        }
    }
}
