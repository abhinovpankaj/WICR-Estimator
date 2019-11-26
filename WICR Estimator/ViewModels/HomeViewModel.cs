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
using System.Runtime.Remoting.Contexts;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

namespace WICR_Estimator.ViewModels
{
    [Serializable]
    class HomeViewModel:BaseViewModel,IPageViewModel
    {
        
        public static event EventHandler OnLoggedAsAdmin;
        public static event EventHandler OnProjectSelectionChange;
        public static string filePath;
        public static bool isEstimateLoaded;
        //private static NotifyIcon statusNotifier;

        public HomeViewModel()
        {
            FillProjects();
            Project.OnSelectedProjectChange += Project_OnSelectedProjectChange;
            HidePasswordSection = System.Windows.Visibility.Collapsed;
            ShowCalculationDetails = new DelegateCommand(CanShowCalculationDetails, canShow);
            SaveEstimate = new DelegateCommand(SaveProjectEstimate, canSaveEstimate);
            LoadEstimate = new DelegateCommand(LoadProjectEstimate, canLoadEstimate);
            ReplicateProject = new DelegateCommand(Replicate, canReplicate);
            ReplicateIndependentProject = new DelegateCommand(ReplicateIndependent, canReplicate);
            ClearProjects = new DelegateCommand(Clear, canClear);
            CreateSummary = new DelegateCommand(GenerateSummary, canCreateSummary);
            RefreshGoogleData = new DelegateCommand(DeleteGoogleData, canDelete);
            ProjectTotals = new ProjectsTotal();
            //statusNotifier = new NotifyIcon();

        }
        

        private void ReplicateIndependent(object obj)
        {
            Project prj = obj as Project;
            if (prj != null)
            {
                if (prj.IsSelectedProject)
                {
                    Project replicatedProject = new Project();

                    prj.CopyCount++;
                    replicatedProject.Name = prj.Name + "." + prj.CopyCount;
                    replicatedProject.OriginalProjectName = prj.OriginalProjectName;
                    replicatedProject.GrpName = "Independent";
                    replicatedProject.MainGroup = "Replicated Projects";
                    replicatedProject.IsSelectedProject = true;
                    replicatedProject.ProjectJobSetUp = new JobSetup(prj.OriginalProjectName);
                    replicatedProject.ProjectJobSetUp.IsProjectIndependent = true;
                    replicatedProject.MetalViewModel = new ZeroMetalViewModel(replicatedProject.ProjectJobSetUp);
                    SelectedProjects.Add(replicatedProject);
                    Projects.Add(replicatedProject);
                    Project_OnSelectedProjectChange(Projects[0], null);
                }
            }
        }

        #region Properties
        public static string LoadedFile=string.Empty;
        private bool applylatestPrice;
        public bool ApplyLatestPrice
        {
            get
            { return applylatestPrice; }
            set
            {
                if (value != applylatestPrice)
                {
                    applylatestPrice = value;
                    OnPropertyChanged("ApplyLatestPrice");
                    if (SelectedProjects != null)
                    {
                        foreach (Project item in SelectedProjects)
                        {
                            item.ApplyLatestPrices = applylatestPrice;
                            ApplyLatestGoogleData(item);
                        }
                    }
                    CanApplyLatestPrice = false;
                    OnPropertyChanged("CanApplyLatestPrice");
                }
            }
        }
        private string _filterString = string.Empty;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged("FilterString");

                ProjectView.Refresh();
            }
        }
        private bool isprocessing;
        public bool IsProcessing
        {
            get
            {
                return isprocessing;
            }
            set
            {
                if (value != isprocessing)
                {
                    isprocessing = value;
                    OnPropertyChanged("IsProcessing");
                }
            }
        }
        private int completedProjects;
        public int CompletedProjects
        {
            get
            {
                return completedProjects;
            }
            set
            {
                if (value != completedProjects)
                {
                    completedProjects = value;
                    OnPropertyChanged("CompletedProjects");
                }
            }
        }
        private string statusMessage;
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                if (value != statusMessage)
                {
                    statusMessage = value;
                    OnPropertyChanged("StatusMessage");
                }
            }
        }
        
        public bool CanApplyLatestPrice { get; set; }

        private string jobname;
        public string JobName
        {
            get { return jobname; }
            set
            {
                if (value!=jobname)
                {
                    jobname = value;
                    OnPropertyChanged("JobName");
                    BaseViewModel.IsDirty = true;
                }
            }
        }
        private string preparedBy;
        public string PreparedBy
        {
            get { return preparedBy; }
            set
            {
                if (value != preparedBy)
                {
                    preparedBy = value;
                    OnPropertyChanged("PreparedBy");
                    BaseViewModel.IsDirty = true;
                }
            }
        }
        private DateTime? jobCreationdate;
        public DateTime? JobCreationDate
        {
            get
            {
                return jobCreationdate;
            }
            set
            {
                if (value != jobCreationdate)
                {
                    jobCreationdate = value;
                    OnPropertyChanged("JobCreationDate");
                    BaseViewModel.IsDirty = true;
                }
            }
        }
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
                        HidePasswordSection = System.Windows.Visibility.Collapsed;
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
        
        private ProjectsTotal prjTotals;
        public ProjectsTotal ProjectTotals
        {
            get { return prjTotals; }
            set
            {
                if (prjTotals != value)
                {
                    prjTotals = value;
                    OnPropertyChanged("ProjectTotals");
                }
            }
        }
        #endregion

        #region Commands
        private bool canCreateSummary(object obj)
        {
            if (SelectedProjects.Count > 0)
            {
                return true;
            }
            else
                return false;
        }
        
        private void DeleteGoogleData(object obj)
        {
            IsProcessing = true;
            StatusMessage = "Deleting the previous Google data";

            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR"))
            {
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR", true);
            }
            DownloadGoogleData();
            
        }
        private bool canDelete(object obj)
        {
            if (IsProcessing)
            {
                return false;
            }
            else
                return true;
        }

        private void GenerateSummary(object obj)
        {
            WriteToSummary();
        }
        private bool canClear(object obj)
        {
            if (SelectedProjects.Count > 0)
            {
                return true;
            }
            else
                return false;
        }
       
       private void Clear(object obj)
        {
            string message = "All the Project selection and values will be cleared. \nDo you want to proceed?";
            string caption = "Refresh WICR Tool";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.
            
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {

                Process.Start(Application.ExecutablePath);
                
                
                Thread.Sleep(1000);
                Environment.Exit(-1);
            }
            

        }
        
        private bool canReplicate(object obj)
        {
            bool isEnabled = false;
            Project prj = obj as Project;
            if (prj != null)
            {
                if (prj.MaterialViewModel != null ||prj.ProjectJobSetUp!=null)
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
                    
                    prj.CopyCount++;
                    string prjName = prj.WorkArea == null ? prj.Name + "." + prj.CopyCount : prj.WorkArea;
                    replicatedProject.Name = prjName; 
                    replicatedProject.OriginalProjectName = prj.OriginalProjectName;
                    replicatedProject.GrpName = "Copied";
                    replicatedProject.MainGroup = "Replicated Projects";
                    replicatedProject.IsSelectedProject = true;
                    
                    SelectedProjects.Add(replicatedProject);
                    Projects.Add(replicatedProject);
                    Project_OnSelectedProjectChange(Projects[0], null);
                }
            }
        }

        public void OpenEstimateFile(string filePath)
        {
            Project savedProject = null;
            DataContractSerializer deserializer = new DataContractSerializer(typeof(ObservableCollection<Project>));

            FileStream fs = new FileStream(filePath, FileMode.Open);
            XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            try
            {
                ObservableCollection<Project> est = (ObservableCollection<Project>)deserializer.ReadObject(reader);

                //SelectedProjects = (ObservableCollection<Project>)deserializer.ReadObject(reader);//est.ToObservableCollection();

                foreach (Project item in est)
                {
                    bool adminLabor = item.MaterialViewModel.ZAddLaborMinCharge;
                    savedProject = Projects.Where(x => x.Name == item.OriginalProjectName).FirstOrDefault();
                    Projects.Remove(savedProject);
                    Projects.Add(item);
                    //code to rename the Material Name for paraseal LG, to make sure old estimates work.
                    if (item.OriginalProjectName == "Paraseal LG")
                    {
                        SystemMaterial sm = item.MaterialViewModel.SystemMaterials.FirstOrDefault(x => x.Name == "SUPER STOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT\"");
                        if (sm!=null)
                        {
                            sm.Name = "SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT";
                        }
                        
                    }
                    if (item.CreationDetails != null)
                    {
                        //fill the Creaters Details
                        string[] creationArray = item.CreationDetails.Split(new string[] { ":;" }, StringSplitOptions.None);
                        JobName = creationArray[0];
                        PreparedBy = creationArray[1];
                        DateTime res = DateTime.Today;
                        if (creationArray[2].Length > 0)
                        {
                            DateTime.TryParse(creationArray[2], out res);
                            JobCreationDate = res;
                        }

                        OnPropertyChanged("JobName");
                        OnPropertyChanged("JobCreationDate");
                        OnPropertyChanged("PreparedBy");
                    }

                    item.ProjectJobSetUp.OnProjectNameChange += ProjectJobSetUp_OnProjectNameChange;
                    SelectedProjects.Add(item);
                    if (item.ProjectJobSetUp != null)
                    {
                        item.ProjectJobSetUp.JobSetupChange += item.MaterialViewModel.JobSetup_OnJobSetupChange;
                        item.ProjectJobSetUp.GetOriginalName();
                        item.ProjectJobSetUp.UpdateJobSetup();
                    }
                    if (item.MetalViewModel != null)
                    {
                        item.MetalViewModel.MetalTotals.OnTotalsChange += item.MaterialViewModel.MetalTotals_OnTotalsChange;
                        item.ProjectJobSetUp.JobSetupChange += item.MetalViewModel.JobSetup_OnJobSetupChange;
                    }
                    if (item.SlopeViewModel != null)
                    {
                        item.SlopeViewModel.SlopeTotals.OnTotalsChange += item.MaterialViewModel.MetalTotals_OnTotalsChange;
                        item.ProjectJobSetUp.JobSetupChange += item.SlopeViewModel.JobSetup_OnJobSetupChange;
                    }
                    item.MaterialViewModel.CheckboxCommand = new DelegateCommand(item.MaterialViewModel.ApplyCheckUnchecks, item.MaterialViewModel.canApply);
                    SystemMaterial.OnQTyChanged += (s, e) => { item.MaterialViewModel.setExceptionValues(s); };

                    //keep other material and other labor materials in sync
                    var ot = item.MaterialViewModel.OtherLaborMaterials;
                    item.MaterialViewModel.OtherLaborMaterials = item.MaterialViewModel.OtherMaterials;
                    int k = 0;
                    foreach (OtherItem olm in item.MaterialViewModel.OtherLaborMaterials)
                    {
                        //olm.Name = ot[k].Name;
                        olm.LQuantity = ot[k].LQuantity;
                        olm.LMaterialPrice = ot[k].LMaterialPrice;
                        k++;
                    }
                    //ends

                    //item.MaterialViewModel.CalculateCost(null);
                    item.MaterialViewModel.ZAddLaborMinCharge = adminLabor;
                }
                Project_OnSelectedProjectChange(Projects[0], null);
                reader.Close();
                ClearProjects.RaiseCanExecuteChanged();
                CreateSummary.RaiseCanExecuteChanged();
                CanApplyLatestPrice = true;
                OnPropertyChanged("CanApplyLatestPrice");
                ApplyLatestPrice = false;
                OnPropertyChanged("SelectedProjects");
                OnPropertyChanged("Projects");

            }
            catch
            {
                MessageBox.Show("Your estimate seems to be created in Older version of WICR Estimator. \n\nPlease re-create the estimates, Or Contact the manufacturer.");
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

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            
            DataContractSerializer deserializer = new DataContractSerializer(typeof(ObservableCollection<Project>));
                
            FileStream fs = new FileStream(filePath, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            try
            {
                ObservableCollection<Project> est = (ObservableCollection<Project>)deserializer.ReadObject(reader);

                //SelectedProjects = (ObservableCollection<Project>)deserializer.ReadObject(reader);//est.ToObservableCollection();

                foreach (Project item in est)
                {
                    bool adminLabor = item.MaterialViewModel.ZAddLaborMinCharge;
                    savedProject = Projects.Where(x => x.Name == item.OriginalProjectName).FirstOrDefault();
                    Projects.Remove(savedProject);
                    Projects.Add(item);
                    //code to rename the Material Name for paraseal LG, to make sure old estimates work.
                    if (item.OriginalProjectName=="Paraseal LG")
                    {
                        SystemMaterial sm = item.MaterialViewModel.SystemMaterials.FirstOrDefault(x => x.Name == "SUPER STOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT\"");
                        if (sm!=null)
                        {
                            sm.Name = "SUPERSTOP (FOUNDATIONS AND WALLS) 1/2\" X 1\"X 20 FT";
                        }
                        
                    }
                    if (item.CreationDetails != null)
                    {
                        //fill the Creaters Details
                        string[] creationArray = item.CreationDetails.Split(new string[] { ":;" }, StringSplitOptions.None);
                        JobName = creationArray[0];
                        PreparedBy = creationArray[1];
                        DateTime res = DateTime.Today;
                        if (creationArray[2].Length > 0)
                        {
                            DateTime.TryParse(creationArray[2], out res);
                            JobCreationDate = res;
                        }

                        OnPropertyChanged("JobName");
                        OnPropertyChanged("JobCreationDate");
                        OnPropertyChanged("PreparedBy");
                    }

                    item.ProjectJobSetUp.OnProjectNameChange += ProjectJobSetUp_OnProjectNameChange;
                    SelectedProjects.Add(item);
                    if (item.ProjectJobSetUp != null)
                    {
                        item.ProjectJobSetUp.JobSetupChange += item.MaterialViewModel.JobSetup_OnJobSetupChange;
                        item.ProjectJobSetUp.EnableMoreMarkupCommand = new DelegateCommand(item.ProjectJobSetUp.CanAddMoreMarkup, item.ProjectJobSetUp.canAdd);
                        item.ProjectJobSetUp.GetOriginalName();
                        item.ProjectJobSetUp.UpdateJobSetup();
                    }
                    if (item.MetalViewModel != null)
                    {
                        item.MetalViewModel.MetalTotals.OnTotalsChange += item.MaterialViewModel.MetalTotals_OnTotalsChange;
                        item.ProjectJobSetUp.JobSetupChange += item.MetalViewModel.JobSetup_OnJobSetupChange;
                    }
                    if (item.SlopeViewModel != null)
                    {
                        item.SlopeViewModel.SlopeTotals.OnTotalsChange += item.MaterialViewModel.MetalTotals_OnTotalsChange;
                        item.ProjectJobSetUp.JobSetupChange += item.SlopeViewModel.JobSetup_OnJobSetupChange;
                    }
                    item.MaterialViewModel.CheckboxCommand = new DelegateCommand(item.MaterialViewModel.ApplyCheckUnchecks, item.MaterialViewModel.canApply);

                    SystemMaterial.OnQTyChanged += (s, e) => { item.MaterialViewModel.setExceptionValues(s); };
                    
                    //keep other material and other labor materials in sync
                    var ot= item.MaterialViewModel.OtherLaborMaterials;
                    item.MaterialViewModel.OtherLaborMaterials= item.MaterialViewModel.OtherMaterials;
                    int k = 0;
                    foreach (OtherItem olm in item.MaterialViewModel.OtherLaborMaterials)
                    {
                        //olm.Name = ot[k].Name;
                        olm.LQuantity = ot[k].LQuantity;
                        olm.LMaterialPrice = ot[k].LMaterialPrice;
                        k++;
                    }
                    //ends

                    //item.MaterialViewModel.CalculateCost(null);
                    item.MaterialViewModel.ZAddLaborMinCharge = adminLabor;
                }
                Project_OnSelectedProjectChange(Projects[0], null);
                reader.Close();
                ClearProjects.RaiseCanExecuteChanged();
                CreateSummary.RaiseCanExecuteChanged();
                CanApplyLatestPrice = true;
                OnPropertyChanged("CanApplyLatestPrice");
                ApplyLatestPrice = false;
                OnPropertyChanged("Projects");
                OnPropertyChanged("SelectedProjects");


            }
            catch(Exception ex)
            {
                MessageBox.Show("Your estimate seems to be created in Older version of WICR Estimator. \n\nPlease re-create the estimates, Or Contact the manufacturer."
                    +ex.Message);
            }  
            finally
            {
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
                        foreach (Project item in SelectedProjects)
                        {
                            item.CreationDetails = JobName + ":;" + PreparedBy + ":;" + JobCreationDate.ToString();
                        }
                        serializer.WriteObject(writer,SelectedProjects );
                        
                        writer.Flush();
                        MessageBox.Show("Project Estimate Saved Succesfully","Success");
                    }
                }

            }

        }
        public DelegateCommand ClearProjects { get; set; }
        public DelegateCommand CreateSummary { get; set; }
        public DelegateCommand RefreshGoogleData { get; set; }
        public DelegateCommand ReplicateProject { get; set; }
        public DelegateCommand ReplicateIndependentProject { get; set; }
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


        #region Private Methods
        private void Project_OnSelectedProjectChange(object sender, EventArgs e)
        {
            if (OnProjectSelectionChange != null)
            {
                OnProjectSelectionChange(SelectedProjects, EventArgs.Empty);
            }

            MyselectedProjects = SelectedProjects;
            OnPropertyChanged("SelectedProjects");
            if (SelectedProjects != null)
            {
                UpdateProjectTotals();
            }
        }

        private void ApplyLatestGoogleData(Project item)
        {
            double laborRate;
            var rate = DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate, item.OriginalProjectName);
            var freightData = DataSerializer.DSInstance.deserializeGoogleData(DataType.Freight, item.OriginalProjectName);
            var laborData = DataSerializer.DSInstance.deserializeGoogleData(DataType.Labor, item.OriginalProjectName);

            if (rate != null)
            {
                double.TryParse(rate[0][0].ToString(), out laborRate);
                item.ProjectJobSetUp.LaborRate = laborRate;
            }
            else
            {
                MessageBox.Show("Latest Price data for this Project is not Available,Please refresh Data or generate new estimate for this project.");
                return;
            }
            
            if (item.MetalViewModel != null)
            {
                item.MetalViewModel.laborRate = laborRate;
                item.MetalViewModel.metalDetails = DataSerializer.DSInstance.deserializeGoogleData(DataType.Metal, item.OriginalProjectName);
                item.MetalViewModel.freightDetails =freightData ;
                item.MetalViewModel.pWage = laborData;
                item.MetalViewModel.OnJobSetupChange(null);
            }
            if (item.SlopeViewModel != null)
            {
                item.SlopeViewModel.pWage =laborData ;
                item.SlopeViewModel.freightData = freightData;
                item.SlopeViewModel.perMixRates = DataSerializer.DSInstance.deserializeGoogleData(DataType.Slope, item.OriginalProjectName);
                item.SlopeViewModel.JobSetup_OnJobSetupChange(item.ProjectJobSetUp, null);
                //item.SlopeViewModel.CalculateAll();
            }
            if (item.MaterialViewModel!=null)
            {
                item.MaterialViewModel.freightData = freightData;
                item.MaterialViewModel.laborDetails= laborData;
                item.MaterialViewModel.laborRate = laborRate;
                item.MaterialViewModel.materialDetails= DataSerializer.DSInstance.deserializeGoogleData(DataType.Material, item.OriginalProjectName);
                item.MaterialViewModel.JobSetup_OnJobSetupChange(item.ProjectJobSetUp, null);
                //item.MaterialViewModel.CalculateCost(null);
            }
            item.UpdateMainTable();
        }
        private void ProjectJobSetUp_OnProjectNameChange(object sender, EventArgs e)
        {
            JobSetup js = sender as JobSetup;
            if (js != null)
            {
                if (js.WorkArea != "")
                {
                    var prj = SelectedProjects.Where(x => x.WorkArea == js.WorkArea && x.OriginalProjectName != js.ProjectName);
                    if (prj.Count() != 0)
                    {
                        MessageBox.Show("Same Workarea name has already been applied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        js.WorkArea = "";
                        return;
                    }
                }

            }
            foreach (Project item in SelectedProjects)
            {
                
                if (item.ProjectJobSetUp.WorkArea != null)
                {
                    if (item.ProjectJobSetUp.WorkArea != "")
                    {
                        item.Name = item.ProjectJobSetUp.WorkArea;
                    }
                    else
                        item.Name = item.ProjectJobSetUp.ProjectName;
                }


            }

        }
        //private void SetBalloonTip()
        //{
        //    statusNotifier.Icon = SystemIcons.Information;
        //    statusNotifier.BalloonTipTitle = "WICR";
        //    //statusNotifier.BalloonTipText = "Balloon Tip Text.";
        //    statusNotifier.BalloonTipIcon = ToolTipIcon.Info;
        //    statusNotifier.Visible = true;
        //}
        private async void DownloadGoogleData()
        {
            //SetBalloonTip();
            IList<IList<object>> LaborRate = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Rate);
            
            IList<IList<object>> MetalData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Metal);
            IList<IList<object>> FreightData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Freight);

            foreach (var prj in Projects)
            {
                
                var values = DataSerializer.DSInstance.deserializeGoogleData(DataType.Rate, prj.Name);
                if (values == null)
                {
                    StatusMessage = "Please Wait! Refreshing Google Data for " + prj.Name;
                                        
                    //statusNotifier.BalloonTipText = StatusMessage;
                    //statusNotifier.ShowBalloonTip(2000);
                    CompletedProjects++;
                    DataSerializer.DSInstance.googleData = new GSData();
                    
                    DataSerializer.DSInstance.googleData.LaborRate = LaborRate;

                    //Thread.Sleep(1000);
                    DataSerializer.DSInstance.googleData.MetalData = MetalData;

                    //Thread.Sleep(1000);
                    DataSerializer.DSInstance.googleData.SlopeData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Slope);
                    Thread.Sleep(1000);

                    DataSerializer.DSInstance.googleData.MaterialData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Material);

                    //Thread.Sleep(1000);
                    DataSerializer.DSInstance.googleData.LaborData = await GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync(prj.Name, DataType.Labor);
                    //Thread.Sleep(1000);
                    DataSerializer.DSInstance.googleData.FreightData = FreightData;

                    DataSerializer.DSInstance.serializeGoogleData(DataSerializer.DSInstance.googleData, prj.Name);
                    Thread.Sleep(3000);
                }
            }
            IsProcessing = false;
            CompletedProjects = 0;

        }
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

                HidePasswordSection = System.Windows.Visibility.Collapsed;
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
            Projects.Add(new Project { Name = "Weather Wear", OriginalProjectName= "Weather Wear",GrpName= "Dexotex" ,MainGroup="Deck Coatings"});
            Projects.Add(new Project { Name = "Weather Wear Rehab", OriginalProjectName = "Weather Wear Rehab", GrpName = "Dexotex", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Barrier Guard", OriginalProjectName = "Barrier Guard", GrpName = "Dexotex", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Endurokote", OriginalProjectName = "Endurokote",GrpName= "Endurokote", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Desert Crete", OriginalProjectName = "Desert Crete", GrpName = "Hill Brothers", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Paraseal", OriginalProjectName = "Paraseal", Rank = 6, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Paraseal LG", OriginalProjectName = "Paraseal LG", Rank = 17, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "860 Carlisle", OriginalProjectName = "860 Carlisle", Rank = 18, GrpName = "Carlisle", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "201", OriginalProjectName = "201", Rank = 18, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "250 GC", OriginalProjectName = "250 GC", Rank = 19, GrpName = "Tremco", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Pli-Dek", OriginalProjectName = "Pli-Dek", Rank = 7, GrpName = "Pli -Dek", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Pedestrian System", OriginalProjectName = "Pedestrian System", Rank = 8,GrpName= "UPI", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Parking Garage", OriginalProjectName = "Parking Garage", Rank = 9, GrpName = "UPI", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Tufflex", OriginalProjectName = "Tufflex", Rank = 10, GrpName = "UPI", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Color Wash Reseal", OriginalProjectName = "Color Wash Reseal", Rank = 11, GrpName = "Westcoat", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "ALX", OriginalProjectName = "ALX", Rank = 12, GrpName = "Westcoat", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "MACoat", OriginalProjectName = "MACoat", Rank = 13, GrpName = "Westcoat", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Reseal all systems", OriginalProjectName = "Reseal all systems", Rank = 14, GrpName = "Reseal", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Resistite", OriginalProjectName = "Resistite", Rank = 15, GrpName = "Dexotex", MainGroup = "Concrete On Grade" });
            Projects.Add(new Project { Name = "Multicoat", OriginalProjectName = "Multicoat", Rank = 16, GrpName = "Multicoat", MainGroup = "Concrete On Grade" });
            Projects.Add(new Project { Name = "Dexcellent II", OriginalProjectName = "Dexcellent II", Rank = 16, GrpName = "Nevada Coatings", MainGroup = "Deck Coatings" });
            Projects.Add(new Project { Name = "Westcoat BT", OriginalProjectName = "Westcoat BT", Rank = 19, GrpName = "Westcoat", MainGroup = "Below Tile" });
            Projects.Add(new Project { Name = "UPI BT", OriginalProjectName = "UPI BT", Rank = 20, GrpName = "UPI", MainGroup = "Below Tile" });
            Projects.Add(new Project { Name = "Dual Flex", OriginalProjectName = "Dual Flex", Rank = 21, GrpName = "Dexotex", MainGroup = "Below Tile" });
            Projects.Add(new Project { Name = "Westcoat Epoxy", OriginalProjectName = "Westcoat Epoxy", Rank = 22, GrpName = "Westcoat", MainGroup = "Epoxy Coatings" });
            Projects.Add(new Project { Name = "Polyurethane Injection Block", OriginalProjectName = "Polyurethane Injection Block", Rank = 23, GrpName = "DeNeef", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Xypex", OriginalProjectName ="Xypex", Rank = 24, GrpName = "Negative side coating", MainGroup = "Below Grade" });
            Projects.Add(new Project { Name = "Blank", OriginalProjectName = "Blank", Rank = 25, GrpName = "Independent", MainGroup = "Blank Template" });
            ProjectView = CollectionViewSource.GetDefaultView(Projects);
            
            ProjectView.GroupDescriptions.Add(new PropertyGroupDescription("MainGroup"));
            ProjectView.SortDescriptions.Add(new SortDescription("MainGroup", ListSortDirection.Ascending));
            ProjectView.GroupDescriptions.Add(new PropertyGroupDescription("GrpName"));
            ProjectView.SortDescriptions.Add(new SortDescription("GrpName", ListSortDirection.Ascending));
            ProjectView.Filter = FilterProject;
        }

        
       
        private bool FilterProject(object item)
        {
            Project prj = item as Project;
            prj.IsGroupExpanded = true;
            return prj.Name.Contains(_filterString);
            
        }

        
        private string getStartRange(string section)
        {
            if (doc == null)
            {
                doc = new XmlDocument();
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                doc.Load(dir+"SummaryRange.xml");
            }
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Sections/" + section);
            return node.InnerText;
        }

        #region SummaryCreationRegion
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
                if (currentProjectName == "Dual Flex")
                {
                    DualFlexSlopeViewModel PVM = SVM as DualFlexSlopeViewModel;

                    if (PVM != null)
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
                        
                        dataRange.Offset[k+1, 3].Value = PVM.UrethaneSumTotal;
                        dataRange.Offset[k+1, 4].Value = PVM.UrethaneSumTotalMixes;
                       
                    }
                    k = 0;
                }
                else
                {
                    PedestrianSlopeViewModel PVM = SVM as PedestrianSlopeViewModel;
                    if (PVM != null)
                    {
                        if (PVM.UrethaneOverrideManually)
                        {
                            startRange = getStartRange("OverriddenUrethaneSlope");
                            dataRange = exlWs.Range[startRange];
                            k = 0;
                            dataRange.Value = "Yes";
                            dataRange.Offset[k, 1].Value = PVM.UrethaneTotalMixesMan;
                            dataRange.Offset[k, 3].Value = PVM.UrethaneAverageMixesPrice;
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
        private string currentProjectName;
        private int WriteJobSetup(JobSetup Js)
        {
            int k = 2;
            currentProjectName = Js.ProjectName;
            string jobSetupRange = getStartRange("JobSetup");
            if (Js.SpecialProductName=="" || Js.SpecialProductName==null)
            {
                exlWs.Range[jobSetupRange].Offset[1, 0].Value = Js.ProjectName + "("+ Js.WorkArea+")";
            }
            else
                exlWs.Range[jobSetupRange].Offset[1, 0].Value = Js.SpecialProductName + "(" + Js.WorkArea + ")";

            exlWs.Range["G99"].Value = Js.NotesToBill;

            Excel.Range dataRange = exlWs.Range[jobSetupRange].Offset[k, 0];
            k = 0;
            dataRange.Offset[k, 0].Value = "JOB NAME";
            dataRange.Offset[k, 1].Value =JobName;
            k++;
            dataRange.Offset[k, 0].Value = "BID BY";
            dataRange.Offset[k, 1].Value = PreparedBy;
            k++;
            dataRange.Offset[k, 0].Value = "DATE";
            dataRange.Offset[k, 1].Value = Js.SelectedDate; 
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

            //check if dual Flex is visible
            if (Js.DualFlexVisible ==System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "1  1/4 inch MORTAR BED WITH 2X2 METAL";
                dataRange.Offset[k, 1].Value = Js.HasQuarterMortarBed;
                k++;
                dataRange.Offset[k, 0].Value = "3/4 INCH MORTAR BED WITH METAL LATHE";
                dataRange.Offset[k, 1].Value = Js.HasQuarterLessMortarBed;
                k++;
                dataRange.Offset[k, 0].Value = "ADD FOR ELASTATEX 2-MEMBRANE SYSTEM";
                dataRange.Offset[k, 1].Value = Js.HasElastex;
                k++;
                dataRange.Offset[k, 0].Value = "EASY ACCESS WITH  NO LADDER?";
                dataRange.Offset[k, 1].Value = Js.HasEasyAccess;
                k++;
            }
            //check if section 250 is visible
            if (Js.Is201SectionVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "TERM BAR (LF)";
                dataRange.Offset[k, 1].Value = Js.TermBarLF;
                k++;
                dataRange.Offset[k, 0].Value = "REBAR PREP @ WALLS (LF)";
                dataRange.Offset[k, 1].Value = Js.RebarPrepWallsLF;
                k++;
                dataRange.Offset[k, 0].Value = "SUPERSTOP (LF)";
                dataRange.Offset[k, 1].Value = Js.SuperStopLF;
                k++;
                dataRange.Offset[k, 0].Value = "PENETRATIONS";
                dataRange.Offset[k, 1].Value = Js.Penetrations;
                k++;
            }
            //check if section 201 is visible
            if (Js.Is201SectionVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "TERM BAR (LF)";
                dataRange.Offset[k, 1].Value = Js.TermBarLF;
                k++;
                dataRange.Offset[k, 0].Value = "REBAR PREP @ WALLS (LF)";
                dataRange.Offset[k, 1].Value = Js.RebarPrepWallsLF;
                k++;
                dataRange.Offset[k, 0].Value = "SUPERSTOP (LF)";
                dataRange.Offset[k, 1].Value = Js.SuperStopLF;
                k++;
                dataRange.Offset[k, 0].Value = "PENETRATIONS";
                dataRange.Offset[k, 1].Value = Js.Penetrations;
                k++;
            }
            //check if section 860 is visible
            if (Js.Is860SectionVisible==System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "SUPER STOP AT FOOTING (Y/N)";
                dataRange.Offset[k, 1].Value = Js.SuperStopAtFooting;
                k++;
                dataRange.Offset[k, 0].Value = "ADDITIONAL TERM BAR (LF)";
                dataRange.Offset[k, 1].Value = Js.TermBarLF;
                k++;
                dataRange.Offset[k, 0].Value = "INSIDE AND OUTSIDE CORNER DETAILS (LF)";
                dataRange.Offset[k, 1].Value = Js.InsideOutsideCornerDetails;
                k++;
                dataRange.Offset[k, 0].Value = "PENETRATIONS (EA)";
                dataRange.Offset[k, 1].Value = Js.Penetrations;
                k++;
            }
            //check if section Paraseal is visible
            if (Js.IsParasealSectionVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "SUPER STOP AT FOOTING?";
                dataRange.Offset[k, 1].Value = Js.SuperStopAtFooting;
                k++;
                dataRange.Offset[k, 0].Value = "ADDITIONAL TERM BAR (LF)";
                dataRange.Offset[k, 1].Value = Js.AdditionalTermBarLF;
                k++;
                
                dataRange.Offset[k, 0].Value = "INSIDE AND OUTSIDE CORNER DETAILS (LF)";
                dataRange.Offset[k, 1].Value = Js.InsideOutsideCornerDetails;
                k++;
                
            }
            //check if section ParasealLG is visible
            if (Js.IsParasealLGSectionVisible == System.Windows.Visibility.Visible)
            {
                dataRange.Offset[k, 0].Value = "SUPER STOP NEEDED AT FOOTING? (Y/N)";
                dataRange.Offset[k, 1].Value = Js.SuperStopAtFooting;
                k++;
                dataRange.Offset[k, 0].Value = "ADDITIONAL TERM BAR (LF)";
                dataRange.Offset[k, 1].Value = Js.AdditionalTermBarLF;
                k++;
                dataRange.Offset[k, 0].Value = "RAKERS/CORNER BRACES EA";
                dataRange.Offset[k, 1].Value = Js.RakerCornerBases;
                k++;

                dataRange.Offset[k, 0].Value = "CEMENT BOARD DETAIL EA";
                dataRange.Offset[k, 1].Value = Js.CementBoardDetail;
                k++;
               
                               
                dataRange.Offset[k, 0].Value = "INSIDE AND OUTSIDE CORNER DETAILS MATERIAL ONLY (LF)";
                dataRange.Offset[k, 1].Value = Js.InsideOutsideCornerDetails;
                k++;
                dataRange.Offset[k, 0].Value = "ROCK POCKETS EA.";
                dataRange.Offset[k, 1].Value = Js.RockPockets;
                k++;

                dataRange.Offset[k, 0].Value = "LABOR FOR 3 FT STRIP OF PARASEAL AT FOUNDATION OR OTHER AREAS (LF)";
                dataRange.Offset[k, 1].Value = Js.ParasealFoundation;
                k++;
                dataRange.Offset[k, 0].Value = "REAR/MID LAGGING LF of beam";
                dataRange.Offset[k, 1].Value = Js.RearMidLagging;
                k++;

            }
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

        private void writeCalculationDetails(MaterialBaseViewModel MVM)
        {
            int k = 2;
            string startRange = getStartRange("CostCalculationDetails");

            Excel.Range dataRange = exlWs.Range[startRange].Offset[k, 0];

            k = 0;
            CostBreakup costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Workers Comp TL > $24.00").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost;

            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Workers Comp All < $23.99").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost;

            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Workers Comp Prevailing Wage").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost;

            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Payroll Expense (SS, ET, Uemp, Dis, Medicare)").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost;


            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Tax").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost;

            k++;
            dataRange.Offset[k, 1].Value = MVM.SubContractMarkup * MVM.TotalSubContractLaborCostBrkp; 

            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "General Liability").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost + costBreakUp.SubContractLaborCost;

            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Direct Expense (Gas, Small Tools, Etc,)").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost+costBreakUp.SubContractLaborCost;

            k++;
            costBreakUp = MVM.LCostBreakUp.Where(x => x.Name == "Contingency").FirstOrDefault();
            dataRange.Offset[k, 1].Value = costBreakUp.MetalCost + costBreakUp.SlopeCost + costBreakUp.SystemCost+ costBreakUp.SubContractLaborCost;

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
            if (MVM.SlopeTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.SlopeTotals.LaborExtTotal;
                
            }
            k++;
            if (MVM.MetalTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.MetalTotals.LaborExtTotal;
            }
            
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalLaborExtension;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsLaborTotal;
            k = 0;
            l++;
            if (MVM.SlopeTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.SlopeTotals.MaterialExtTotal;
            }
            
            k++;
            if (MVM.MetalTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.MetalTotals.MaterialExtTotal;
            }
            
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalMaterialCostbrkp;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsMaterialTotal;
            k = 0;
            l++;
            if (MVM.SlopeTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.SlopeTotals.MaterialFreightTotal;
            }
            
            k++;
            if (MVM.MetalTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.MetalTotals.MaterialFreightTotal;
            }
            
            k++;
            dataRange.Offset[k, l].Value = MVM.TotalFreightCostBrkp;
            k++;
            dataRange.Offset[k, l].Value = 0;
            k++;
            dataRange.Offset[k, l].Value = MVM.AllTabsFreightTotal;
            k = 0;
            l++;
            if (MVM.SlopeTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.SlopeTotals.SubContractLabor;
            }
           
            k++;
            if (MVM.MetalTotals!=null)
            {
                dataRange.Offset[k, l].Value = MVM.MetalTotals.SubContractLabor;
            }
            
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
            bool metalDeleted=false, slopeDeleted=false,bothSlopeDeleted=false;
            try
            {              
                //delete empty Labor Other Costs
                int rowN=item.lastUsedRows["LaborOtherCosts"];
                exlWs.Range["A" +rowN , "E123"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //delete empty SystemMaterial Other materials
                rowN = item.lastUsedRows["OtherSystemMaterials"];
                exlWs.Range["A" + rowN, "E101"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //delete empty Slopes
                if (item.SlopeViewModel!=null)
                {
                    if (item.lastUsedRows.ContainsKey("UrethaneSlope"))
                    {
                        rowN = item.lastUsedRows["UrethaneSlope"];
                        if (rowN != 0)
                        {
                            exlWs.Range["A" + rowN, "E67"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                            slopeDeleted = true;
                        }
                    }
                    if (currentProjectName=="Dual Flex")
                    {
                        exlWs.Range["A63", "E63"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                        
                    }
                }
                else
                {
                    rowN = 46;
                    exlWs.Range["A" + rowN, "E67"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                    bothSlopeDeleted = true;
                }
                
                //delete empty System Materials
                rowN = item.lastUsedRows["SystemMaterials"];
                exlWs.Range["G" + rowN, "L75"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);


                //Delete Metals         
                if (item.MetalViewModel == null)
                {
                    rowN = 26;
                    exlWs.Range["G1", "L40"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    exlWs.Range["A" + rowN, "E44"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    metalDeleted = true;
                }
                else
                {
                    rowN = item.lastUsedRows["MiscMetals"];
                    exlWs.Range["A" + rowN, "E44"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);                    
                }
                
                //delete empty jobsetup
                rowN = item.lastUsedRows["JobSetup"];
                exlWs.Range["A" + rowN, "E24"].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

                //check if Sub Contract Labor is Starting before Row 40 only if Metal and Slope has been deleted
                if (slopeDeleted  || metalDeleted)
                {
                    Excel.Range subRange = exlWs.Range["A1", "E100"];
                    
                    Excel.Range foundRange = subRange.Find("Material SUB CONTRACT LABOR", Type.Missing,
                        Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                        Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                        Type.Missing, Type.Missing);

                    if (foundRange != null)
                    {
                        int foundRow = foundRange.Row;

                        if (foundRow < 40)
                        {
                            do
                            {
                                exlWs.Range["A" + foundRow, "E" + foundRow].Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                                foundRow++;
                            } while (foundRow < 41);

                        }
                    }
                }
                else if (bothSlopeDeleted)
                {
                    Excel.Range subRange = exlWs.Range["A1", "E100"];

                    Excel.Range foundRange = subRange.Find("Material OTHER COSTS", Type.Missing,
                        Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                        Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                        Type.Missing, Type.Missing);

                    if (foundRange != null)
                    {
                        int foundRow = foundRange.Row;

                        if (foundRow < 40)
                        {
                            do
                            {
                                exlWs.Range["A" + foundRow, "E" + foundRow].Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                                foundRow++;
                            } while (foundRow < 41);

                        }
                        else
                        {
                            foundRange = subRange.Find("Labor Hours", Type.Missing,
                        Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                        Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                        Type.Missing, Type.Missing);
                            if (foundRange != null)
                            {
                                foundRow = foundRange.Row;

                                if (foundRow < 40)
                                {
                                    do
                                    {
                                        exlWs.Range["A" + foundRow, "E" + foundRow].Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                                        foundRow++;
                                    } while (foundRow < 41);

                                }
                            }
                        }
                    }
                }
                else
                {
                    Excel.Range subRange = exlWs.Range["A1", "E100"];

                    Excel.Range foundRange = subRange.Find("Urethane Slope", Type.Missing,
                        Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                        Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                        Type.Missing, Type.Missing);
                    if (foundRange != null)
                    {
                        int foundRow = foundRange.Row;

                        if (foundRow <= 40)
                        {
                            do
                            {
                                exlWs.Range["A" + foundRow, "E" + foundRow].Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                                foundRow++;
                            } while (foundRow < 41);

                        }
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
                    exlApp.Visible = false;
                    exlApp.DisplayAlerts = false;
                }
                summaryWb = exlApp.Workbooks.Add();
                ws = (Excel.Worksheet)exlWb.Worksheets["Summary"];

                foreach (Project item in SelectedProjects)
                {
                    item.lastUsedRows = new Dictionary<string, int>();
                    ws.Copy(summaryWb.Worksheets["Sheet1"]);
                    
                    exlWs = (Excel.Worksheet)summaryWb.Worksheets["Summary"];
                    if (item.Name.Length>22)
                    {
                        string suffix = item.Name.Contains(".") ? new string(item.Name.Reverse().ToArray()).Split('.')[0]:"";
                        if (suffix=="")
                        {
                            exlWs.Name = item.Name.Substring(0, 21) ;
                        }
                        else
                            exlWs.Name = item.Name.Substring(0, 17) + "." + suffix;

                    }
                    else
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
                    writeCalculationDetails(item.MaterialViewModel);
                    clearEmptyRows(item);

                }
                //Ask user to save the File 
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Execl files (*.xlsx)|*.xlsx";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.CreatePrompt = false;
                if (JobCreationDate != null)
                {
                    saveFileDialog.FileName = JobName + " " + string.Format(JobCreationDate.Value.ToShortDateString(), "mm-dd-yyyy");
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

        #region Public Methods
        public void UpdateProjectTotals()
        {
            double tabsLaborTotal = 0;
            ProjectTotals.Name = "Totals";
            ProjectTotals.LaborCost = Math.Round(SelectedProjects.Sum(x => x.LaborCost), 2);
            ProjectTotals.SlopeCost = Math.Round(SelectedProjects.Sum(x => x.SlopeCost), 2);
            ProjectTotals.MetalCost = Math.Round(SelectedProjects.Sum(x => x.MetalCost), 2);
            ProjectTotals.SystemCost = Math.Round(SelectedProjects.Sum(x => x.SystemNOther), 2);
            ProjectTotals.MaterialCost = Math.Round(SelectedProjects.Sum(x => x.MaterialCost), 2);
            ProjectTotals.TotalCost = Math.Round(SelectedProjects.Sum(x => x.TotalCost), 2);

            foreach (Project item in SelectedProjects)
            {
                if (item.MaterialViewModel != null)
                {
                    tabsLaborTotal = tabsLaborTotal + item.MaterialViewModel.AllTabsLaborTotal;
                }


            }
            if (ProjectTotals.TotalCost != 0)
            {
                ProjectTotals.LaborPercentage = Math.Round(tabsLaborTotal / ProjectTotals.TotalCost * 100, 2).ToString() + "%";
            }
            else
                ProjectTotals.LaborPercentage = "0%";
        }
        #endregion

        public string Name
        {
            get
            {
                return "Home";
            }
        }

        public string ProductVersion
        {
            
            get
            {
                return "Version 1.0";
            }
        }       
    }
}
