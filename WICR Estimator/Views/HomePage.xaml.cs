using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WICR_Estimator.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        //HomeViewModel HomeVM;
        public HomePage()
        {
            InitializeComponent();
            //HomeVM = new HomeViewModel();
            //this.DataContext = HomeVM;
        }

        private void refreshGData_Click(object sender, RoutedEventArgs e)
        {
            if(System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR"))
            {
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR",true);
            }
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(FilterTextBox.Text.Length==0)
            //{
            //    CollapseAll();
            //}
        }

        private void CollapseAll()
        {
            // get each listBoxItem by index from the listBox
            for (int i = 0; i < ProjectList.Items.Count; i++)
            {
                var ad = ProjectList.ItemContainerGenerator.ContainerFromIndex(i);
                Expander item = (Expander)ProjectList.ItemContainerGenerator.ContainerFromIndex(i);

                // find its parents expander
                var exp = FindParent<Expander>(item);

                if (exp != null)
                {
                    //if its not null expand it and see if it has a parent expander
                    exp.IsExpanded = true;
                    exp = FindParent<Expander>(exp);
                    if (exp != null)
                        exp.IsExpanded = false;
                }

            }
        }

        private T FindParent<T>(DependencyObject child)
                where T : DependencyObject

        {

            T parent = VisualTreeHelper.GetParent(child) as T;

            if (parent != null)

                return parent;

            else

                return FindParent<T>(parent);

        }
        //private void ListBox_Selected(object sender, RoutedEventArgs e)
        //{            
        //    HomeVM.SelectedProjects.Clear();
        //    foreach (Project item in ProjectList.SelectedItems)
        //    {
        //        HomeVM.SelectedProjects.Add(item);
        //    }
        //    HomeVM.SelectedProjects = HomeVM.SelectedProjects.OrderBy(project => project.Rank).ToList();
        //}
    }
}
