using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Input;
using System.Windows.Navigation;
using WICR_Estimator.Models;
using WICR_Estimator.Views;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace WICR_Estimator.ViewModels
{
    
    class HomeViewModel:BaseViewModel,IPageViewModel
    {
        public static event EventHandler OnLoggedAsAdmin;
        public HomeViewModel()
        {
            FillProjects();
            Project.OnSelectedProjectChange += Project_OnSelectedProjectChange;
            HidePasswordSection = System.Windows.Visibility.Hidden;
            ShowCalculationDetails = new DelegateCommand(CanShowCalculationDetails, canShow);
            SaveEstimate = new DelegateCommand(SaveProjectEstimate, canSaveEstimate);
            LoadEstimate = new DelegateCommand(LoadProjectEstimate, canLoadEstimate);
            ReplicateProject = new DelegateCommand(Replicate, canReplicate);
        }

        

        private void Project_OnSelectedProjectChange(object sender, EventArgs e)
        {
            MyselectedProjects = SelectedProjects;
            OnPropertyChanged("SelectedProjects");
        }
        #region Properties\Commands

        public string LoginMessage { get; set; }
        private bool showLogin;
        public bool ShowLogin
        {
            get { return showLogin; }
            set
            {
                if (value!=showLogin)
                {
                    showLogin = value;                    
                    if (!value)
                    {                    
                        LoginMessage = "";
                        HidePasswordSection = System.Windows.Visibility.Hidden;
                        if (OnLoggedAsAdmin != null)
                        {
                            OnLoggedAsAdmin(false, EventArgs.Empty);
                        }
                    }
                    else
                        HidePasswordSection = System.Windows.Visibility.Visible;

                    OnPropertyChanged("HidePasswordSection");
                    OnPropertyChanged("LoginMessage");
                    OnPropertyChanged("ShowLogin");
                }
            }
        }

        private bool canReplicate(object obj)
        {
            bool isEnabled = false;
            Project prj = obj as Project;
            if (prj != null)
            {
                if (prj.MaterialViewModel != null || prj.MetalViewModel != null || prj.SlopeViewModel != null)
                {
                    isEnabled= true;
                }
            }
            return isEnabled;
        }
        private void Replicate(object obj)
        {
            Project prj = obj as Project;
            if (prj!=null)
            {
                if (prj.IsSelectedProject)
                {
                    Project replicatedProject = new Project();
                    replicatedProject = ReplicateObject.DeepClone(prj);

                    prj.CopyCount++;
                    replicatedProject.Name = prj.Name + "." + prj.CopyCount;
                    replicatedProject.GrpName = "Copied";
                    replicatedProject.MainGroup = "Replicated Projects";
                    //replicatedProject.ProjectJobSetUp.OnProjectNameChange += replicatedProject.ProjectJobSetUp_OnProjectNameChange;
                    //SelectedProjects.Add(replicatedProject);
                    Projects.Add(replicatedProject);
                    replicatedProject.MetalViewModel.MetalTotals.OnTotalsChange += replicatedProject.MaterialViewModel.MetalTotals_OnTotalsChange;
                    replicatedProject.SlopeViewModel.SlopeTotals.OnTotalsChange += replicatedProject.MaterialViewModel.MetalTotals_OnTotalsChange;
                    replicatedProject.ProjectJobSetUp.JobSetupChange += replicatedProject.MaterialViewModel.JobSetup_OnJobSetupChange;
                    replicatedProject.ProjectJobSetUp.JobSetupChange += replicatedProject.MetalViewModel.JobSetup_OnJobSetupChange;

                    replicatedProject.ProjectJobSetUp.JobSetupChange += replicatedProject.SlopeViewModel.JobSetup_OnJobSetupChange;
                    Project_OnSelectedProjectChange(Projects[0], null);
                }
            }
        }

        private void LoadProjectEstimate(object obj)
        {
            Project savedProject = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Browse Estimate File",
                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "est",
                Filter = "Estimator files (*.est)|*.est",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            string filePath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(ObservableCollection<Project>));
                
                FileStream fs = new FileStream(filePath, FileMode.Open);
                XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                ObservableCollection<Project> est = (ObservableCollection<Project>)deserializer.ReadObject(reader );
                //SelectedProjects = (ObservableCollection<Project>)deserializer.ReadObject(reader);//est.ToObservableCollection();

                foreach (Project item in est)
                {
                    savedProject = Projects.Where(x => x.Name == item.Name).FirstOrDefault();
                    Projects.Remove(savedProject);
                    Projects.Add(item);
                }
                Project_OnSelectedProjectChange(Projects[0], null);
                reader.Close();
            }
            
        }

        private bool canLoadEstimate(object obj)
        {
            return true;
        }
        private bool canSaveEstimate(object obj)
        {
            if (SelectedProjects.Count > 0)
            {
                return true;
            }
            else
                return false;
        }

        private void SaveProjectEstimate(object obj)
        {
            ////serialize Enabled Project Data and Save

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
                //// Insert code to set properties and fields of the object.  
                //XmlSerializer mySerializer = new XmlSerializer(typeof(ObservableCollection<Project>));
                //// To write to a file, create a StreamWriter object.  
                //StreamWriter myWriter = new StreamWriter(saveFileDialog1.FileName);
                //mySerializer.Serialize(myWriter, SelectedProjects);
                //myWriter.Close();
                var serializer = new DataContractSerializer(typeof(ObservableCollection<Project>));
                
                using (var sw = new StringWriter())
                {
                    using (var writer = new XmlTextWriter(saveFileDialog1.FileName,null))
                    {
                        writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                        serializer.WriteObject(writer,SelectedProjects );
                        writer.Flush();
                        MessageBox.Show("Project Estimate Saved Succesfully","Success");
                    }
                }

            }

        }
        public DelegateCommand ReplicateProject { get; set; }
        private DelegateCommand showCalculationDetails;
        public DelegateCommand ShowCalculationDetails
        {
            get { return showCalculationDetails; }
            set
            {
                if (value != showCalculationDetails)
                {
                    showCalculationDetails = value;
                    OnPropertyChanged("ShowCalculationDetails");
                }
            }
        }
        private DelegateCommand loadEstimate;
        public DelegateCommand LoadEstimate
        {
            get { return loadEstimate; }
            set
            {
                if (value != loadEstimate)
                {
                    loadEstimate = value;
                    OnPropertyChanged("LoadEstimate");
                }
            }
        }
        private DelegateCommand saveEstimate;
        public DelegateCommand SaveEstimate
        {
            get { return saveEstimate; }
            set
            {
                if (value != saveEstimate)
                {
                    saveEstimate = value;
                    OnPropertyChanged("SaveEstimate");
                }
            }
        }
        #endregion
        public System.Windows.Visibility HidePasswordSection { get; set; }
        public ICollectionView ProjectView { get; set; }
        private ObservableCollection<Project> projects;
        public ObservableCollection<Project> Projects
        {
            get
            {
                return projects;
            }
            set
            {
                if (projects != value)
                {
                    projects = value;
                    OnPropertyChanged("Projects");
                }
            }
        }
        
        public static ObservableCollection<Project> MyselectedProjects;
        private ObservableCollection<Project> selectedProjects;

        public ObservableCollection<Project> SelectedProjects
        {
            get
            {
                var selected = Projects.Where(p => p.IsSelectedProject == true).ToList();
                return new ObservableCollection<Project>(selected);                
            }
            set
            {
                if (selectedProjects != value)
                {
                    selectedProjects = value;
                    OnPropertyChanged("SelectedProjects");
                    
                    MyselectedProjects = selectedProjects;
                }
            }
        }

        
        #region Methods
        private bool canShow(object obj)
        {
            return true;
        }
        private void CanShowCalculationDetails(object obj)
        {
            var passwordBox = obj as PasswordBox;
            var password = passwordBox.Password;
            if (password == "737373")
            {
                passwordBox.Password = "";
                LoginMessage = "Calculation Details Are Visible Now.";

                HidePasswordSection = System.Windows.Visibility.Hidden;
                OnPropertyChanged("HidePasswordSection");
                if (OnLoggedAsAdmin!=null)
                {
                    OnLoggedAsAdmin(true, EventArgs.Empty);
                }
                
            }
            else
            {
                passwordBox.Password = "";
                LoginMessage = "Incorrect Password.";
                if (OnLoggedAsAdmin != null)
                {
                    OnLoggedAsAdmin(false, EventArgs.Empty);
                }
            }
            OnPropertyChanged("LoginMessage");
        }

        void FillProjects()
        {
            Projects = new ObservableCollection<Project>();
            //SelectedProjects = new List<Project>();
            Projects.Add(new Project { Name = "Weather Wear", Rank = 1,GrpName= "Dexotex" ,MainGroup="Deck Coatings"});
            Projects.Add(new Project { Name = "Weather Wear Rehab", Rank = 2, GrpName = "Dexotex", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Barrier Gaurd", Rank = 3, GrpName = "Dexotex", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Endurokote", Rank = 4,GrpName= "Endurokote", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Desert Crete", Rank = 5, GrpName = "Hill Brothers", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Paraseal", Rank = 6, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Paraseal LG", Rank = 17, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "860", Rank = 18, GrpName = "Carlisle", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "201", Rank = 18, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "250", Rank = 19, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Pli-Dek", Rank = 7, GrpName = "Pli-Dek", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Pedestrian System", Rank = 8,GrpName= "UPI", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Parking Garage", Rank = 9, GrpName = "UPI", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Tufflex", Rank = 10, GrpName = "UPI", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Color Wash Reseal", Rank = 11, GrpName = "Westcoat", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "ALX", Rank = 12, GrpName = "Westcoat", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "MACoat", Rank = 13, GrpName = "Westcoat", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Reseal all systems", Rank = 14, GrpName = "Reseal", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Resistite", Rank = 15, GrpName = "Dexotex", MainGroup = "Concrete On Grade" });
            Projects.Add(new Project { Name = "Multicoat", Rank = 16, GrpName = "Multicoat", MainGroup = "Concrete On Grade" });
            Projects.Add(new Project { Name = "Dexcellent II", Rank = 16, GrpName = "Nevada Coatings", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Dual Membrane", Rank = 19, GrpName = "Westcoat", MainGroup = "Below Tile" });
            Projects.Add(new Project { Name = "UPI Below Tile", Rank = 20, GrpName = "UPI", MainGroup = "Below Tile" });
            Projects.Add(new Project { Name = "Dual Flex", Rank = 21, GrpName = "Dexotex", MainGroup = "Below Tile" });
            Projects.Add(new Project { Name = "Color Flake", Rank = 22, GrpName = "Westcoat", MainGroup = "Epoxy Coatings" });
            Projects.Add(new Project { Name = "Polyurethane Injection Block", Rank = 23, GrpName = "DeNeef", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Block Wall", Rank = 24, GrpName = "Xypex", MainGroup = "Below Grade" });
            ProjectView = CollectionViewSource.GetDefaultView(Projects);
            
            ProjectView.GroupDescriptions.Add(new PropertyGroupDescription("MainGroup"));
            ProjectView.SortDescriptions.Add(new SortDescription("MainGroup", ListSortDirection.Ascending));
            ProjectView.GroupDescriptions.Add(new PropertyGroupDescription("GrpName"));
            ProjectView.SortDescriptions.Add(new SortDescription("GrpName", ListSortDirection.Ascending));          

        }

        #endregion
        public string Name
        {
            get
            {
                return "Home Page";
            }
        }
       

    }
}
