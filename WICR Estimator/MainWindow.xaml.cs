using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
using System.Xml;
using WICR_Estimator.Models;
using WICR_Estimator.ViewModels;
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
                MessageBoxResult res = MessageBox.Show("Do you want to Save the Estimate.", "Save State", MessageBoxButton.YesNoCancel);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        //Save the state.
                        //SaveEstimate(ViewModels.HomeViewModel.MyselectedProjects);
                        MainWindowViewModel vm = this.DataContext as MainWindowViewModel;
                        if (vm!=null)
                        {
                            vm.SaveEstimateCommand.Execute(null);
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.No:
                        ViewModels.BaseViewModel.IsDirty = false;
                        break;
                    default:
                        break;
                }                
            }
            else
            {

            }
        }

        public void SaveEstimate(ObservableCollection<Project> SelectedProjects)
        {
            string JobCreationDate=string.Empty, JobName = string.Empty, PreparedBy = string.Empty;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog1.Title = "Save Project Estimate";
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = false;
            //saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Project files (*.est)|*.est|All files (*.*)|*.*";
            //saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                MainWindowViewModel vm = this.DataContext as MainWindowViewModel;
                if (vm!=null)
                {
                    HomeViewModel hm=vm.PageViewModels.FirstOrDefault(x => x.Name == "Home Page") as HomeViewModel;
                    if (hm!=null)
                    {
                        JobName = hm.JobName;
                        PreparedBy = hm.PreparedBy;
                        JobCreationDate = hm.JobCreationDate.ToString();
                    }
                }
                var serializer = new DataContractSerializer(typeof(ObservableCollection<Project>));

                using (var sw = new StringWriter())
                {
                    using (var writer = new XmlTextWriter(saveFileDialog1.FileName, null))
                    {
                        writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                        foreach (Project item in SelectedProjects)
                        {
                            item.CreationDetails = JobName + ":;" + PreparedBy + ":;" + JobCreationDate.ToString();
                        }
                        serializer.WriteObject(writer, SelectedProjects);

                        writer.Flush();
                        MessageBox.Show("Project Estimate Saved Succesfully", "Success");
                    }
                }
                ViewModels.BaseViewModel.IsDirty = false;
            }
        }
    }
}
