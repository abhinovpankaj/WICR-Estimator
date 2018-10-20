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
using System.Windows.Shapes;

namespace WICR_Estimator
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    /// 
    public partial class LoginWindow : Window
    {
        public static event EventHandler onOKClick;
        public System.Windows.Visibility isAdmin { set; get; }
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (username.Text=="admin" && password.Text=="admin")
            {
                isAdmin = System.Windows.Visibility.Visible;
            }
            else
                isAdmin = System.Windows.Visibility.Hidden;
            
            if (onOKClick!=null)
            {
                onOKClick(this.isAdmin, EventArgs.Empty);
            }
        }
    }
}
