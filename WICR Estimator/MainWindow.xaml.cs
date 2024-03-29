﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using WICR_Estimator.Models;
using WICR_Estimator.ViewModels;
using WICR_Estimator.Views;

namespace WICR_Estimator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = new MainWindowViewModel(DialogCoordinator.Instance);

            //Apply Pallete
            var accentColor = Properties.Settings.Default.Accent1;
            var primaryColor = Properties.Settings.Default.Primary1;
            if (primaryColor.Length==0)
            {
                return;
            }
            System.Windows.Media.Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(accentColor);
            ITheme theme = _paletteHelper.GetTheme();
            theme.SetSecondaryColor(color);
            

            color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(primaryColor);
            theme.SetPrimaryColor(color);
            _paletteHelper.SetTheme(theme);



        }
        private bool isClosingConfirmed;
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModels.HomeViewModel.MyselectedProjects!=null && ViewModels.BaseViewModel.IsDirty == true)
            {
                if (this.isClosingConfirmed)
                {
                    // window will close, if e.Cancel is passed in as "false"
                    return;
                }
                e.Cancel = true;
                MainWindowViewModel vm = this.DataContext as MainWindowViewModel;
                var res = await vm.ShowActionMessage("Do you want to Save the Estimate.", "WICR");

                switch (res)
                {
                    case MessageDialogResult.Affirmative:
                        var hm1 = vm.PageViewModels.FirstOrDefault(x => x.Name == "Home") as HomeViewModel;
                        if (hm1 != null)
                        {
                            if (hm1.PreparedBy == null || hm1.PreparedBy == string.Empty || hm1.JobName == null || hm1.JobName == string.Empty)
                            {
                                vm.OnTaskCompleted("Please fill JobName & Prepared by and then save the estimate.");
                                return;
                            }
                        }

                        
                        await vm.SaveEstimates(ViewModels.HomeViewModel.MyselectedProjects);

                        isClosingConfirmed = true;
                        this.Close();
                        break;
                    case MessageDialogResult.Negative:
                        isClosingConfirmed = true;
                        this.Close();
                        break;
                    default:
                        break;
                }
            }
           
            //MessageBoxResult res = MessageBox.Show("Do you want to Save the Estimate.", "Save State", MessageBoxButton.YesNoCancel);   
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (DataContext is ICloseWindow )
            {
                var vm = DataContext as ICloseWindow;
                vm.Close += () =>
                  {
                      this.Close();
                  };
            }
        }
        
    }
}
