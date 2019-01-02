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
        //public System.Collections.IList SelectedItems
        //{
        //    get
        //    {
        //        return SelectedProjects;
        //    }
        //    set
        //    {

        //        foreach (Project item in Projects)
        //        {
        //            item.IsSelectedProject = false;
        //        }
        //        foreach (Project prj in value)
        //        {
                    
        //            prj.IsSelectedProject = true;
        //            if (!SelectedProjects.Contains(prj))
        //            {
        //                SelectedProjects.Add(prj);
        //            }
        //        }
        //        OnPropertyChanged("SelectedProjects");

        //        MyselectedProjects = SelectedProjects;

        //    }
        //}
        public static ObservableCollection<Project> MyselectedProjects;
        private ObservableCollection<Project> selectedProjects;

        public ObservableCollection<Project> SelectedProjects
        {
            get
            {
                var selected=  Projects.Where(p => p.IsSelectedProject == true).ToList();    
                return new ObservableCollection<Project>(selected); ;
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

        #endregion
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
            Projects.Add(new Project { Name = "Dexotex Weather Wear", Rank = 1,GrpName= "Deck Coatings" });
            Projects.Add(new Project { Name = "Dexotex Weather Wear Rehab", Rank = 2, GrpName = "Deck Coatings" });
            Projects.Add(new Project { Name = "Dexotex Barrier Gaurd", Rank = 3, GrpName = "Deck Coatings" });
            Projects.Add(new Project { Name = "Enduro Kote Metal", Rank = 4,GrpName= "EnduroKote" });
            //Projects.Add(new Project { Name = "Desert Crete", Rank = 5, GrpName = "Hill Brothers" });
            //Projects.Add(new Project { Name = "Dexcellent II", Rank = 6, GrpName = "Nevada Coatings" });
            //Projects.Add(new Project { Name = "Pli-Dek", Rank = 7, GrpName = "Pli-Dek" });
            Projects.Add(new Project { Name = "Pedestrian System", Rank = 8,GrpName= "UPI" });
            //Projects.Add(new Project { Name = "Vehicular System", Rank = 9, GrpName = "UPI" });
            //Projects.Add(new Project { Name = "Tufflex", Rank = 10, GrpName = "UPI" });
            Projects.Add(new Project { Name = "Westcoat Color Wash", Rank = 11, GrpName = "Westcoat" });
            //Projects.Add(new Project { Name = "ALX", Rank = 12, GrpName = "Westcoat" });
            //Projects.Add(new Project { Name = "MACoat", Rank = 13, GrpName = "Westcoat" });
            //Projects.Add(new Project { Name = "Reseal all systems", Rank = 14, GrpName = "Reseal" });
            Projects.Add(new Project { Name = "Desert Brand", Rank = 6 ,GrpName="Unknown"});
            Projects.Add(new Project { Name = "Weather Wear Reseal", Rank = 5, GrpName = "Unknown" });

            ProjectView = CollectionViewSource.GetDefaultView(Projects);
            ProjectView.GroupDescriptions.Add(new PropertyGroupDescription("GrpName"));
            ProjectView.SortDescriptions.Add(new SortDescription("Country", ListSortDirection.Ascending));          

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
