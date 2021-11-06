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
            txtbox = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private bool prevBox;
        private TextBox txtbox;
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtbox = sender as TextBox;
            JobSetup js = txtbox.DataContext as JobSetup;
            preProjectName = js.ProjectName;
            var binding = BindingOperations.GetBinding(txtbox, TextBox.TextProperty).Path.Path;
            if (js.ZData!=null)
            {
                js.SelectedData = js.ZData.FirstOrDefault(x => x.Key == binding.ToString());
            }
            prevBox = true;
        }
        string preProjectName = string.Empty;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (txtbox == null)
            {
                return;
            }
            TextBox txtbox1 = sender as TextBox;
            JobSetup js = this.DataContext as JobSetup;

            if (preProjectName!=js.ProjectName)
            {
                return;
            }
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
                        
                        js[binding.ToString()] = Convert.ToDouble(calVal);
                    }
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
           
            
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            
            JobSetup js = txtbox.DataContext as JobSetup;
            if (js==null)
            {
                return;
            }
            var calVal = new DataTable().Compute(js.Formula ?? "0", null);
            if (calVal != null)
            {
                if(txtbox.Text!=calVal.ToString())
                {
                    js.Formula = txtbox.Text;
                }
            }

            ///txtbox = null;
        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (txtbox == null)
            {
                return;
            }
            TextBox txtbox1 = sender as TextBox;
            JobSetup js = this.DataContext as JobSetup;

            var binding = BindingOperations.GetBinding(txtbox, TextBox.TextProperty).Path.Path;
            try
            {
                var calVal = new DataTable().Compute(txtbox1.Text ?? "0", null);
                if (calVal != null)
                {
                    if (txtbox1.Text.Length == 0)
                    {
                        //txtbox.Text = "0";
                        js[binding.ToString()] = 0;
                    }
                    else
                    {
                        //txtbox.Text = calVal.ToString();
                        js[binding.ToString()] = Convert.ToDouble(calVal);
                    }
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
        }
    }
}
