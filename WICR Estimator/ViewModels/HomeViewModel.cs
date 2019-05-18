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
using Excel = Microsoft.Office.Interop.Excel;

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
            ClearProjects = new DelegateCommand(Clear, canClear);
            CreateSummary = new DelegateCommand(GenerateSummary, canCreateSummary);
            
        }

        

        private void Project_OnSelectedProjectChange(object sender, EventArgs e)
        {
            MyselectedProjects = SelectedProjects;
            OnPropertyChanged("SelectedProjects");
        }
        #region Properties\Commands
        public string JobName { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? JobDate { get; set; }
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
        private bool canCreateSummary(object obj)
        {
            if (SelectedProjects.Count > 0)
            {
                return true;
            }
            else
                return false;
        }

        private void GenerateSummary(object obj)
        {
            WriteToSummary();
        }
        private bool canClear(object obj)
        {
            return true;
        }

        private void Clear(object obj)
        {
            
            //foreach (Project item in Projects)
            //{
            //    if (item.IsSelectedProject)
            //    {
            //        item.IsSelectedProject = false;
            //        item.ProjectJobSetUp = null;
            //        item.MetalViewModel = null;
            //        item.SlopeViewModel = null;
            //        item.MaterialViewModel = null;
                    
            //    }
            //}
            //MyselectedProjects = null;
            //SelectedProjects = null;
            //OnPropertyChanged("SelectedProjects");
            //OnPropertyChanged("Projects");
            FillProjects();
            Project.OnSelectedProjectChange += Project_OnSelectedProjectChange;
            Project_OnSelectedProjectChange(null, null);
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
                    //replicatedProject = ReplicateObject.DeepClone(prj);

                    prj.CopyCount++;
                    replicatedProject.Name = prj.Name + "." + prj.CopyCount;
                    replicatedProject.GrpName = "Copied";
                    replicatedProject.MainGroup = "Replicated Projects";
                    replicatedProject.IsSelectedProject = true;
                    //replicatedProject.ProjectJobSetUp.OnProjectNameChange += ProjectJobSetUp_OnProjectNameChange;
                    SelectedProjects.Add(replicatedProject);
                    Projects.Add(replicatedProject);
                    //replicatedProject.MetalViewModel.MetalTotals.OnTotalsChange += replicatedProject.MaterialViewModel.MetalTotals_OnTotalsChange;
                    //replicatedProject.SlopeViewModel.SlopeTotals.OnTotalsChange += replicatedProject.MaterialViewModel.MetalTotals_OnTotalsChange;
                    //replicatedProject.ProjectJobSetUp.JobSetupChange += replicatedProject.MaterialViewModel.JobSetup_OnJobSetupChange;
                    //replicatedProject.ProjectJobSetUp.JobSetupChange += replicatedProject.MetalViewModel.JobSetup_OnJobSetupChange;

                    //replicatedProject.ProjectJobSetUp.JobSetupChange += replicatedProject.SlopeViewModel.JobSetup_OnJobSetupChange;
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
                    //replicatedProject.ProjectJobSetUp.OnProjectNameChange += ProjectJobSetUp_OnProjectNameChange;
                    SelectedProjects.Add(item);

                    item.MetalViewModel.MetalTotals.OnTotalsChange += item.MaterialViewModel.MetalTotals_OnTotalsChange;
                    item.SlopeViewModel.SlopeTotals.OnTotalsChange += item.MaterialViewModel.MetalTotals_OnTotalsChange;
                    item.ProjectJobSetUp.JobSetupChange += item.MaterialViewModel.JobSetup_OnJobSetupChange;
                    item.ProjectJobSetUp.JobSetupChange += item.MetalViewModel.JobSetup_OnJobSetupChange;
                    item.MaterialViewModel.CheckboxCommand = new DelegateCommand(item.MaterialViewModel.ApplyCheckUnchecks, item.MaterialViewModel.canApply);
                    item.ProjectJobSetUp.JobSetupChange += item.SlopeViewModel.JobSetup_OnJobSetupChange;
                    
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
        public DelegateCommand ClearProjects { get; set; }
        public DelegateCommand CreateSummary { get; set; }
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
        private string getStartRange(string section)
        {
            if (doc == null)
            {
                doc = new XmlDocument();
                doc.Load("SummaryRange.xml");
            }
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Sections/" + section);
            return node.InnerText;
        }

        #region SummaryRegion
        private static XmlDocument doc;
        Microsoft.Office.Interop.Excel.Application exlApp;
        Microsoft.Office.Interop.Excel.Workbook exlWb;
        Microsoft.Office.Interop.Excel.Worksheet exlWs;
        

        
        
        private int writeMetals(MetalBaseViewModel MVM)
        {
            int k = 2;
            string startRange = getStartRange("Metals");
            Excel.Range dataRange = exlWs.Range[startRange].Offset[k, 0];
            k = 0;
            foreach (Metal item in MVM.Metals)
            {
                dataRange.Offset[k,0].Value = item.Name;
                dataRange.Offset[k, 1].Value = item.Size;
                if (item.Name.Equals("STAIR METAL"))
                {
                    dataRange.Offset[k, 2].Value = item.IsStairMetalChecked ? item.Units : 0;
                }
                else
                    dataRange.Offset[k, 2].Value = item.Units;
                if (item.SpecialMetalPricing != 0)
                {
                    dataRange.Offset[k, 3].Value =  item.SpecialMetalPricing;
                    dataRange.Offset[k,3].Interior.Color= Excel.XlRgbColor.rgbYellow;
                }
                else
                    dataRange.Offset[k, 3].Value =  item.MaterialPrice;

                dataRange.Offset[k, 4].Value = item.MaterialExtension;
                k = k + 1;
            }
            //k = 0;
            //startRange = getStartRange("AddonMetals");
            //dataRange = exlWs.Range[startRange].Offset[2, 0];

            foreach (AddOnMetal item in MVM.AddOnMetals)
            {
                dataRange.Offset[k, 0].Value = item.Name;
                dataRange.Offset[k, 1].Value = item.Size;
                dataRange.Offset[k, 2].Value = item.IsMetalChecked ? item.Units : 0;
                if (item.SpecialMetalPricing != 0)
                {
                    dataRange.Offset[k, 3].Value = item.SpecialMetalPricing;
                    dataRange.Offset[k, 3].Interior.Color = Excel.XlRgbColor.rgbYellow;
                }
                else
                    dataRange.Offset[k, 3].Value = item.MaterialPrice;

                dataRange.Offset[k, 4].Value = item.MaterialExtension;
                k = k + 1;
            }
            k = 0;
            startRange = getStartRange("MiscMetals");
            dataRange = exlWs.Range[startRange].Offset[2, 0];
            foreach (MiscMetal item in MVM.MiscMetals)
            {
                dataRange.Offset[k, 0].Value = item.Name + " " + item.Size;
                
                dataRange.Offset[k, 1].Value = item.Units;
                dataRange.Offset[k, 2].Value = item.UnitPrice;
                dataRange.Offset[k, 3].Value = item.MaterialPrice;
                k = k + 1;
            }

            k = k + dataRange.Row;
            return k;
            
        }
        private int writeSlope(SlopeBaseViewModel SVM)
        {
            int k = 2;
            string startRange;
            Excel.Range dataRange;
            if (SVM.OverrideManually)
            {
                startRange = getStartRange("OverriddenSlope");
                dataRange = exlWs.Range[startRange];
                k = 0;
                dataRange.Value = "Yes";
               dataRange.Offset[k, 1].Value = SVM.TotalMixesMan;
                //var nu = dataRange.Offset[k, 3].Column;
               dataRange.Offset[k, 3].Value = SVM.AverageMixesPrice;
            }
            else
            {
                startRange= getStartRange("Slope");
                dataRange = exlWs.Range[startRange].Offset[k, 0];
                k = 0;
                foreach (Slope item in SVM.Slopes)
                {
                    dataRange.Offset[k,0].Value = item.Thickness;
                    dataRange.Offset[k, 1].Value = item.Sqft;
                    dataRange.Offset[k, 2].Value = item.DeckCount;
                    dataRange.Offset[k, 3].Value = item.Total;
                    dataRange.Offset[k, 4].Value = item.TotalMixes;
                    k = k + 1;
                }
                dataRange.Offset[k, 3].Value = SVM.SumTotal;
                dataRange.Offset[k, 4].Value = SVM.SumTotalMixes;
            }

            k = 2;
            if (SVM.IsUrethaneVisible == System.Windows.Visibility.Visible)
            {

                
                PedestrianSlopeViewModel PVM = SVM as PedestrianSlopeViewModel;
                if (PVM !=null)
                {
                    if (PVM.UrethaneOverrideManually)
                    {
                        startRange = getStartRange("OverriddenUrethaneSlope");
                        dataRange = exlWs.Range[startRange];
                        k = 0;
                        dataRange.Value = "Yes";
                        dataRange.Offset[k , 1].Value = PVM.TotalMixesMan;
                        dataRange.Offset[k, 2].Value = PVM.UrethaneSumTotalMixes;
                    }
                    else
                    {
                        startRange = getStartRange("UrethaneSlope");
                        dataRange = exlWs.Range[startRange].Offset[k, 0];
                        k = 0;
                        foreach (Slope item in PVM.UrethaneSlopes)
                        {
                            dataRange.Offset[k, 0].Value = item.Thickness;
                            dataRange.Offset[k, 1].Value = item.Sqft;
                            dataRange.Offset[k, 2].Value = item.DeckCount;
                            dataRange.Offset[k, 3].Value = item.Total;
                            dataRange.Offset[k, 4].Value = item.TotalMixes;
                            k = k + 1;
                        }
                        dataRange.Offset[k, 3].Value = PVM.UrethaneSumTotal;
                        dataRange.Offset[k, 4].Value = PVM.UrethaneSumTotalMixes;
                    }
                    
                }
                k = 0;
                
            }
            else
            {
                k = 57;
            }
            return k;
        }
        private string WriteMaterials(MaterialBaseViewModel MVM)
        {
            string rowString;
            int k = 2;
            string startRange = getStartRange("OtherSystemMaterials");
            Excel.Range dataRange= exlWs.Range[startRange].Offset[k, 0];
            k = 0;
            foreach (OtherItem item in MVM.OtherMaterials)
            {
                dataRange.Offset[k, 0].Value = item.Name;
                dataRange.Offset[k, 1].Value = item.Quantity;
                dataRange.Offset[k, 2].Value = item.MaterialPrice;
                dataRange.Offset[k, 3].Value = item.Extension;
                
                k = k + 1;
            }
            k = k + dataRange.Row;
            rowString = k.ToString();
            exlWs.Range[getStartRange("MaterialOtherCostTotal")].Offset[0, 1].Value = MVM.TotalOCExtension;
            
            startRange = getStartRange("SystemMaterials");
            dataRange = exlWs.Range[startRange].Offset[2, 0];
            k = 0;
            foreach (SystemMaterial item in MVM.SystemMaterials)
            {
                dataRange.Offset[k, 0].Value = item.Name;
                dataRange.Offset[k, 1].Value = item.SMUnits;
                dataRange.Offset[k, 2].Value = item.IsMaterialChecked ? item.Qty:0;
                if (item.SpecialMaterialPricing!=0)
                {
                    dataRange.Offset[k, 3].Value = item.SpecialMaterialPricing;
                    dataRange.Offset[k, 3].Interior.Color = Excel.XlRgbColor.rgbYellow;
                }
                else
                    dataRange.Offset[k, 3].Value = item.MaterialPrice;

                dataRange.Offset[k, 4].Value = item.MaterialExtension;
                k = k + 1;
            }
            k = k + dataRange.Row;
            rowString = rowString + ":" + k.ToString();
            
            //dataRange.Offset[k, 3].Value = MVM.TotalMaterialCost;

            startRange = getStartRange("SubSystemMaterials");
            
            dataRange = exlWs.Range[startRange].Offset[2, 0];
            k = 0;
            foreach (LaborContract item in MVM.SubContractLaborItems)
            {
                dataRange.Offset[k, 0].Value = item.Name;
                dataRange.Offset[k, 1].Value = item.UnitConlbrcst;
                dataRange.Offset[k, 2].Value = item.UnitPriceConlbrcst;
                dataRange.Offset[k, 3].Value = item.MaterialExtensionConlbrcst;

                k = k + 1;
            }
            dataRange.Offset[k, 3].Value = MVM.TotalSubContractLaborCostBrkp;
            return rowString;
        }
        private int WriteJobSetup(JobSetup Js)
        {
            int k = 2;
            string jobSetupRange = getStartRange("JobSetup");
            if (Js.SpecialProductName!="")
            {
                exlWs.Range[jobSetupRange].Offset[1, 0].Value = Js.ProjectName + "("+ Js.WorkArea+")";
            }
            else
                exlWs.Range[jobSetupRange].Offset[1, 0].Value = Js.SpecialProductName + "(" + Js.WorkArea + ")";

            Excel.Range dataRange = exlWs.Range[jobSetupRange].Offset[k, 0];
            k = 0;
            dataRange.Offset[k, 0].Value = "JOB NAME";
            dataRange.Offset[k, 1].Value =JobName;
            k++;
            dataRange.Offset[k, 0].Value = "BID BY";
            dataRange.Offset[k, 1].Value = PreparedBy;
            k++;
            dataRange.Offset[k, 0].Value = "DATE";
            dataRange.Offset[k, 1].Value = JobDate;
            dataRange.Offset[k, 1].NumberFormat = "mm-dd-yyyy";
            k++;
            dataRange.Offset[k, 0].Value = "NOTE HERE IF A DIFFERENT PRODUCT IS BEING USED";
            dataRange.Offset[k, 1].Value = Js.SpecialProductName;
            k++;
            if (Js.IsPrevalingWage)
            {
                Excel.Range prevailingRange = dataRange.Offset[k, 0];
                prevailingRange.Value = "PREVAILING WAGE (Y or N)";
                prevailingRange.Offset[0, 1].Value = "YES";
                prevailingRange.Offset[0,2].Value = Js.ActualPrevailingWage;
                k++;
            }
            else
            {
                Excel.Range prevailingRange = dataRange.Offset[k, 0];
                prevailingRange.Value = "PREVAILING WAGE (Y or N)";
                prevailingRange.Offset[0, 1].Value = "No";
                prevailingRange.Offset[0, 2].Value = 0;
                k++;

            }
            if (Js.IsNewPlywoodVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "NEW PLYWOOD (Y or N)";
                dataRange.Offset[k, 1].Value = Js.IsNewPlywood.ToString();
                k++;
            }
            if (Js.IsJobByArchitectVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "Is This Job Specified By Architect?";
                dataRange.Offset[k, 1].Value = Js.IsJobSpecifiedByArchitect;
                k++;
            }
            if (Js.IsSandCementVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = Js.FirstCheckBoxLabel;
                dataRange.Offset[k, 1].Value = Js.IsApprovedForSandCement;
                k++;
            }
            
                dataRange.Offset[k, 0].Value = "Metal Flashing Over Concrete?";
                dataRange.Offset[k, 1].Value = Js.IsFlashingRequired;
                k++;
            
            if (Js.IsResealVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "Reseal?";
                dataRange.Offset[k, 1].Value = Js.IsReseal;
                k++;
            }
            if (Js.IsPlywoodSqftVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "Total Sq Ft Plywood";
                dataRange.Offset[k, 1].Value = Js.TotalSqftPlywood;
                k++;
            }
            if (Js.IsSqftConcreteVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = Js.SqftLabel;
                dataRange.Offset[k, 1].Value = Js.TotalSqft;
                k++;
            }
            if (Js.IsSqftVerticleVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = Js.VerticalSqftLabel;
                dataRange.Offset[k, 1].Value = Js.TotalSqftVertical;
                k++;
            }
            if (Js.IslinearCopingFootageVisible== System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = Js.LinearFootageText;
                dataRange.Offset[k, 1].Value = Js.LinearCopingFootage;
                k++;
            }

            dataRange.Offset[k, 0].Value = Js.StairRiserText;
            dataRange.Offset[k, 1].Value = Js.RiserCount;
            k++;
            dataRange.Offset[k, 0].Value = Js.DeckLabel;
            dataRange.Offset[k, 1].Value = Js.DeckPerimeter;
            k++;

            dataRange.Offset[k, 0].Value = Js.DeckCountText;
            dataRange.Offset[k, 1].Value = Js.DeckCount;
            k++;
            dataRange.Offset[k, 0].Value = "Stair Width";
            dataRange.Offset[k, 1].Value = Js.StairWidth;
            k++;

            
            dataRange.Offset[k, 0].Value = "Future Projects";
            dataRange.Offset[k, 1].Value = Js.ProjectDelayFactor;
            k++;

            dataRange.Offset[k, 0].Value = "Vendor Name";
            dataRange.Offset[k, 1].Value = Js.VendorName;
            k++;
            dataRange.Offset[k, 0].Value = "Material Type";
            dataRange.Offset[k, 1].Value = Js.MaterialName;
            k++;
            k =dataRange.Row+k;
            return k;
            
        }
        private int WriteLabor(MaterialBaseViewModel MVM)
        {
            int rowN;
            int k = 2;
            string startRange = getStartRange("LaborOtherCosts");
            
            Excel.Range dataRange = exlWs.Range[startRange].Offset[k, 0];
            k = 0;
            foreach (OtherItem item in MVM.OtherLaborMaterials)
            {
                dataRange.Offset[k,0].Value = item.Name;
                dataRange.Offset[k, 1].Value = item.LQuantity;
                dataRange.Offset[k, 2].Value = item.LMaterialPrice;
                dataRange.Offset[k, 3].Value = item.LExtension;

                k = k + 1;
            }
            k = k + dataRange.Row;
            rowN = k;
            exlWs.Range[getStartRange("LaborOtherCostTotal")].Offset[0, 1].Value  = MVM.TotalOCLaborExtension;

            //write labor hrs
            k = 1;
            startRange = getStartRange("LaborHours");
            dataRange = exlWs.Range[startRange].Offset[1, 0];
            k = 0;
            //dataRange.Offset[k,1].Value = MVM.AllTabsLaborTotal;
            //k++;
            //dataRange.Offset[k, 1].Value = MVM.AllTabsMaterialTotal;
            //k++;
            //dataRange.Offset[k, 1].Value = MVM.AllTabsFreightTotal;
            //k++;
            //dataRange.Offset[k, 1].Value = MVM.AllTabsSubContractTotal;
            //k++;
            dataRange.Offset[k, 1].Value = MVM.TotalHrsLabor;
            k++;
            dataRange.Offset[k, 1].Value = MVM.TotalHrsSystemLabor;
            k++;
            dataRange.Offset[k, 1].Value = MVM.TotalHrsMetalLabor;
            k++;
            dataRange.Offset[k, 1].Value = MVM.TotalHrsSlopeLabor;
            k++;
            dataRange.Offset[k, 1].Value = MVM.TotalHrsDriveLabor;

            
            int l = 1;
            startRange = getStartRange("CostSummary");
            dataRange = exlWs.Range[startRange].Offset[2, 0];
            k = 0;
            dataRange.Offset[k, l].Value = MVM.TotalSlopingPrice;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalMetalPrice;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalSystemPrice;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalSubcontractLabor;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalSale;
            k=0;
            l++;

            dataRange.Offset[k, l].Value = MVM.SlopeTotals.LaborExtTotal;
            k++;
            dataRange.Offset[k, l].Value = MVM.MetalTotals.LaborExtTotal;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalLaborExtension;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsLaborTotal;
            k = 0;
            l++;

            dataRange.Offset[k, l].Value = MVM.SlopeTotals.MaterialExtTotal;
            k++;
            dataRange.Offset[k, l].Value = MVM.MetalTotals.MaterialExtTotal;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalMaterialCostbrkp;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsMaterialTotal;
            k = 0;
            l++;

            dataRange.Offset[k, l].Value = MVM.SlopeTotals.MaterialFreightTotal;
            k++;
            dataRange.Offset[k, l].Value = MVM.MetalTotals.MaterialFreightTotal;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalFreightCostBrkp;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsFreightTotal;
            k = 0;
            l++;

            dataRange.Offset[k, l].Value = MVM.SlopeTotals.SubContractLabor;
            k++;
            dataRange.Offset[k, l].Value = MVM.MetalTotals.SubContractLabor;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalSubContractLaborCostBrkp;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsSubContractTotal;
            k++;
            dataRange.Offset[k, 1].Value = MVM.DriveLaborValue;

            return rowN;
        }

        private void clearEmptyRows(Project item)
        {
            try
            {              
                //delete empty Labor Other Costs
                int rowN=item.lastUsedRows["LaborOtherCosts"];
                exlWs.Range["A" +rowN , "E123"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //delete empty SystemMaterial Other materials
                rowN = item.lastUsedRows["OtherSystemMaterials"];
                exlWs.Range["A" + rowN, "E101"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //delete empty Slopes
                rowN = item.lastUsedRows["UrethaneSlope"];
                if (rowN!=0)
                {
                    exlWs.Range["A" + rowN, "E67"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                }
                
                //delete empty System Materials
                rowN = item.lastUsedRows["SystemMaterials"];
                exlWs.Range["G" + rowN, "L75"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //delete empty Misc metals
                rowN = item.lastUsedRows["MiscMetals"];
                exlWs.Range["A" + rowN, "E44"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                //delete empty jobsetup
                rowN = item.lastUsedRows["JobSetup"];
                exlWs.Range["A" + rowN, "E24"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //check if Sub Contract Labor is Starting before Row 40
                Excel.Range subRange = exlWs.Range["A1", "E100"];
                // You should specify all these parameters every time you call this method,
                // since they can be overridden in the user interface. 
                Excel.Range foundRange = subRange.Find("Material SUB CONTRACT LABOR", Type.Missing,
                    Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                    Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                    Type.Missing, Type.Missing);
                
                if (foundRange!=null)
                {
                    int foundRow = foundRange.Row;
                    
                    if (foundRow < 40)
                    {
                        do
                        {
                            exlWs.Range["A" + foundRow, "E" + foundRow].Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                            foundRow++;
                        } while (foundRow<41);
                        
                    }
                }
               
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show("Failed to clear Empty Rows" + "\n" + ex.Message, "Failure");
            }
        }
        private void WriteToSummary()
        {
            WaitWindow ww = new WaitWindow();
            ww.Show();
            Excel.Workbook summaryWb;
            Excel.Worksheet ws;
            try
            {
                if (exlApp == null)
                {
                    exlApp = new Microsoft.Office.Interop.Excel.Application();
                    var dir = AppDomain.CurrentDomain.BaseDirectory;
                    exlWb = exlApp.Workbooks.Open(dir +"SummaryTemplate.xlsx");
                    exlApp.Visible = true;
                    exlApp.DisplayAlerts = false;
                }
                summaryWb = exlApp.Workbooks.Add();
                ws = (Excel.Worksheet)exlWb.Worksheets["Summary"];

                foreach (Project item in SelectedProjects)
                {
                    item.lastUsedRows = new Dictionary<string, int>();
                    ws.Copy(summaryWb.Worksheets["Sheet1"]);
                    exlWs = (Excel.Worksheet)summaryWb.Worksheets["Summary"];
                    exlWs.Name = item.Name + " Summary";
                    item.lastUsedRows.Add("JobSetup",  WriteJobSetup(item.ProjectJobSetUp));

                    if (item.MetalViewModel != null)
                    {
                        item.lastUsedRows.Add("MiscMetals", writeMetals(item.MetalViewModel));                        
                    }
                    if (item.SlopeViewModel != null)
                    {                        
                        int row=writeSlope(item.SlopeViewModel);
                        if (row!=0)
                        {
                            item.lastUsedRows.Add("UrethaneSlope", row);
                        }
                    }
                    if (item.MaterialViewModel != null)
                    {
                        string rowStrng=WriteMaterials(item.MaterialViewModel);
                        if (rowStrng.Contains(":"))
                        {
                            item.lastUsedRows.Add("OtherSystemMaterials", Int32.Parse(rowStrng.Split(':')[0]));
                            item.lastUsedRows.Add("SystemMaterials", Int32.Parse(rowStrng.Split(':')[1]));
                        }

                        item.lastUsedRows.Add("LaborOtherCosts", WriteLabor(item.MaterialViewModel)); ;
                    }
                    clearEmptyRows(item);

                }
                //Ask user to save the File 
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Execl files (*.xlsx)|*.xlsx";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.CreatePrompt = true;
                if (JobDate!=null)
                {
                    saveFileDialog.FileName = JobName + " " + string.Format(JobDate.Value.ToShortDateString(), "mm-dd-yyyy");
                }
                
                saveFileDialog.Title = "Save WICR Estimator Summary";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Save. The selected path can be got with saveFileDialog.FileName.ToString()
                    summaryWb.SaveAs(saveFileDialog.FileName.ToString());
                }
                else
                {
                    System.Windows.MessageBox.Show("Summary Sheet won't be saved now", "Save Cancelled",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message +"\n\n"+ "Please SaveAs different name or close the file before Saving the Summary File.", "Summary File Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //Close Excel file
                if (exlApp != null)
                {
                    ws = null;
                    exlWb.Close(false);
                    exlWb = null;
                    exlApp.Quit();
                    exlApp = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    
                }
                ww.Close();
            }
            
        }
        #endregion

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
