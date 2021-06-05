using System;
using System.Collections.Generic;
using System.Data;
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

namespace WICR_Estimator.Views
{
    /// <summary>
    /// Interaction logic for JobSetupPage.xaml
    /// </summary>
    public partial class JobSetupPage : UserControl
    {
        public JobSetupPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private TextBox txtbox;
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
             txtbox = sender as TextBox;
            JobSetup js = txtbox.DataContext as JobSetup;

            var binding = BindingOperations.GetBinding(txtbox, TextBox.TextProperty).Path.Path;
            if (js.ZData!=null)
            {
                js.SelectedData = js.ZData.FirstOrDefault(x => x.Key == binding.ToString());
            }
          

           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtbox1 = sender as TextBox;
            JobSetup js = this.DataContext as JobSetup;
            var binding = BindingOperations.GetBinding(txtbox, TextBox.TextProperty).Path.Path;
            try
            {                
                var calVal = new DataTable().Compute(txtbox1.Text ?? "0", null);
                if (calVal != null)
                {
                    if (txtbox1.Text.Length==0)
                    {
                        //txtbox.Text = "0";
                        js[binding.ToString()] = 0;
                    }
                    else
                    {                       
                        //txtbox.Text = calVal.ToString();
                        js[binding.ToString()] = calVal;
                    }
                    
                    
                    

                }
            }
            catch (Exception)
            {

                //throw;
            }
           
            //js.SelectedData.CalculatedValue.ToString();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //JobSetup js = txtbox.DataContext as JobSetup;

            
            //    js.SelectedData = null;
            
        }
    }
}
