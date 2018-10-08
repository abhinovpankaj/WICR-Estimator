using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using WICR_Estimator.Models;
using WICR_Estimator.Views;

namespace WICR_Estimator.ViewModels
{
    class HomeViewModel:BaseViewModel,IPageViewModel
    {
        
        public HomeViewModel()
        {
            FillProjects();
            Project.OnSelectedProjectChange += Project_OnSelectedProjectChange;            
        }

        private void Project_OnSelectedProjectChange(object sender, EventArgs e)
        {
            MyselectedProjects = SelectedProjects;
            OnPropertyChanged("SelectedProjects");
        }
        #region Properties\Commands

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
        void FillProjects()
        {
            Projects = new ObservableCollection<Project>();
            //SelectedProjects = new List<Project>();
            Projects.Add(new Project { Name = "Weather Wear ", Rank = 1 });
            Projects.Add(new Project { Name = "Weather Wear2", Rank = 2 });
            Projects.Add(new Project { Name = "Weather Wear 3", Rank = 3 });
            Projects.Add(new Project { Name = "Weather Wear4", Rank = 4 });
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
