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

namespace WICR_Estimator.Views.DataViews
{
    /// <summary>
    /// Interaction logic for MaterialDetailsPage.xaml
    /// </summary>
    public partial class MaterialDetailsPage : UserControl
    {
        public MaterialDetailsPage()
        {
            InitializeComponent();
            //MaterialDetailsPageViewModel vm= new MaterialDetailsPageViewModel();
            //this.DataContext = vm;
            //materials.ItemsSource = vm.Materials;
            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(materials.ItemsSource);
            
            //view.Filter = UserFilter;
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Visual visual = e.OriginalSource as Visual;

            if (!visual.IsDescendantOf(expander))
            {

                expander.IsExpanded = false;
            }
                
        }
    }
}
