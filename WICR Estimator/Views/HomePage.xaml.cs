using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
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
            
            projectsDatagrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(projectsDatagrid_PreviewMouseLeftButtonDown);
            projectsDatagrid.Drop += new DragEventHandler(projectsDatagrid_Drop);

        }

        HomeViewModel vm;

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (HomeViewModel.isEstimateLoaded)
            {
                return;
            }
            ViewModels.HomeViewModel hm = this.DataContext as HomeViewModel;
            if (hm!=null)
            {
                if (HomeViewModel.LoadedFile != string.Empty)
                {
                    hm.OpenEstimateFile(HomeViewModel.LoadedFile);
                    HomeViewModel.isEstimateLoaded = true;
                    HomeViewModel.filePath = HomeViewModel.LoadedFile;
                }
            }
            
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

        #region native
        public void HideCloseButton()
        {
            var hwnd = new WindowInteropHelper(this.Parent as Window).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion

        #region dragandDrop
        public delegate Point GetPosition(IInputElement element);
        int rowIndex = -1;

        void projectsDatagrid_Drop(object sender, DragEventArgs e)
        {
            if (vm==null)
            {
                vm = this.DataContext as HomeViewModel;
            }
            if (rowIndex < 0)
                return;
            int index = this.GetCurrentRowIndex(e.GetPosition);
            if (index < 0)
                return;
            if (index == rowIndex)
                return;
            if (index == projectsDatagrid.Items.Count - 1)
            {
                MessageBox.Show("This row-index cannot be drop");
                return;
            }
            ObservableCollection<Project> projects = projectsDatagrid.ItemsSource as ObservableCollection<Project>;
            Project changedProduct = projects[rowIndex];
            projects.RemoveAt(rowIndex);
            projects.Insert(index, changedProduct);
            //vm.AddProjectSequence();
            //vm.fireEvent(changedProduct);
            AddProjectSequence(projects,index);
        }
        public void AddProjectSequence(ObservableCollection<Project> selectedProjects,int index)
        {
            int k = 1;
            foreach (var item in selectedProjects)
            {
                item.Sequence = k;
                k++;
                item.RefreshProjectName();                
                //item.OriginalProjectName = item.Sequence +"."+ item.OriginalProjectName;
            }

            
            if (selectedProjects.Count > 1)
            {
                var result = from item in selectedProjects
                             orderby item.Sequence ascending
                             select item;

                selectedProjects = (ObservableCollection<Project>)result.ToObservableCollection();
            }
            //vm.SelectedProjects=selectedProjects; 
            HomeViewModel.MyselectedProjects = selectedProjects;
            projectsDatagrid.InvalidateVisual();
            vm.fireEvent(selectedProjects[index]);
        }
        void projectsDatagrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rowIndex = GetCurrentRowIndex(e.GetPosition);
            if (rowIndex<0)
            {
                return;
            }
            projectsDatagrid.SelectedIndex = rowIndex;
            Project selectedProject = projectsDatagrid.Items[rowIndex] as Project;
            if (selectedProject==null)
            {
                return;
            }
            DragDropEffects dragDropEffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(projectsDatagrid,selectedProject,dragDropEffects)!= DragDropEffects.None)
            {
                projectsDatagrid.SelectedItem = selectedProject;
            }

        }

        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }

        private DataGridRow GetRowItem(int index)
        {
            if (projectsDatagrid.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return projectsDatagrid.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        private int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < projectsDatagrid.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
        #endregion
    }
}
