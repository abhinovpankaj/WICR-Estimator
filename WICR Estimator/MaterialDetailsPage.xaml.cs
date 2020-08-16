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

namespace WICR_Estimator
{
    /// <summary>
    /// Interaction logic for MaterialDetailsPage.xaml
    /// </summary>
    public partial class MaterialDetailsPage : UserControl
    {
        public MaterialDetailsPage()
        {
            InitializeComponent();
            MaterialDetailsPageViewModel vm= new MaterialDetailsPageViewModel();
            this.DataContext = vm;
            materials.ItemsSource = vm.Materials;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(materials.ItemsSource);
            
            view.Filter = UserFilter;
        }
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as SysMaterial).MaterialName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(materials.ItemsSource).Refresh();
        }

    }
}
