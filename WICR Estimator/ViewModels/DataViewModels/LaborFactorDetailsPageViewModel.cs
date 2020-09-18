using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.DBModels;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels.DataViewModels
{
    public class LaborFactorDetailsPageViewModel : BaseViewModel, IPageViewModel
    {
        private IEnumerable<LaborFactorDB> LaborFactorsFilterByProject;
        public string SearchText { get; set; } = "";
        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand(SearchLaborFactor, CanSearch);
                }

                return _searchCommand;
            }

        }
        private DelegateCommand _updateLaborFactorsCommand;
        public DelegateCommand UpdateLaborFactorsCommand
        {
            get
            {
                if (_updateLaborFactorsCommand == null)
                {
                    _updateLaborFactorsCommand = new DelegateCommand(UpdateLaborFactors, CanUpdate);
                }

                return _updateLaborFactorsCommand;
            }

        }

        private bool CanUpdate(object obj)
        {
            return true;
        }

        private async void UpdateLaborFactors(object obj)
        {
            var result = await HTTPHelper.PutLaborFactorsAsync(FilteredLaborFactors.Where(x => x.IsChecked == true));
            if (result == null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
                LastActionResponse = "Changes Saved Successfully.";

        }

        private DelegateCommand _updateLaborFactorCommand;
        public DelegateCommand UpdateLaborFactorCommand
        {
            get
            {
                if (_updateLaborFactorCommand == null)
                {
                    _updateLaborFactorCommand = new DelegateCommand(UpdateLaborFactor, CanUpdate);
                }

                return _updateLaborFactorCommand;
            }

        }

        private async void UpdateLaborFactor(object obj)
        {
            SelectedLaborFactor = await HTTPHelper.PutLaborFactorAsync(SelectedLaborFactor.LaborId, SelectedLaborFactor);
        }

        private void SearchLaborFactor(object obj)
        {
            if (LaborFactorsFilterByProject != null)
            {
                FilteredLaborFactors = LaborFactorsFilterByProject.Where(x => x.Name.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
            }
            else
                FilteredLaborFactors = LaborFactors.Where(x => x.Name.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
           
        }

        private bool CanSearch(object obj)
        {
            if (SearchText != null)
            {
                if (SearchText.Length > 0)
                {
                    return true;
                }
            }
            else
            {
                FilteredLaborFactors = LaborFactorsFilterByProject.ToObservableCollection();

            }

            return false;

        }

        private IEnumerable<LaborFactorDB> _LaborFactors = new ObservableCollection<LaborFactorDB>();
        public IEnumerable<LaborFactorDB> LaborFactors
        {
            get
            {
                return _LaborFactors;
            }
            set
            {
                if (_LaborFactors != value)
                {
                    _LaborFactors = value;
                    OnPropertyChanged("LaborFactors");
                }
            }
        }
        private ObservableCollection<LaborFactorDB> _filteredLaborFactors = new ObservableCollection<LaborFactorDB>();
        public ObservableCollection<LaborFactorDB> FilteredLaborFactors
        {
            get
            {
                return _filteredLaborFactors;
            }
            set
            {
                if (_filteredLaborFactors != value)
                {
                    _filteredLaborFactors = value;
                    OnPropertyChanged("FilteredLaborFactors");
                }
            }
        }
        private LaborFactorDB _selectedLaborFactor;
        public LaborFactorDB SelectedLaborFactor
        {
            get
            {
                return _selectedLaborFactor;
            }
            set
            {
                if (_selectedLaborFactor != value)
                {
                    _selectedLaborFactor = value;
                    OnPropertyChanged("SelectedLaborFactor");
                }
            }
        }
        private IEnumerable<ProjectDB> _projects;
        public IEnumerable<ProjectDB> Projects
        {
            get
            {
                return _projects;
            }
            set
            {
                if (_projects != value)
                {
                    _projects = value;
                    OnPropertyChanged("Projects");
                }
            }
        }
        private ProjectDB _selectedProject;
        public ProjectDB SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    GetLaborFactorsById(SelectedProject.ProjectId);
                    OnPropertyChanged("SelectedProject");
                }
            }
        }

        public string Name => "LaborFactor Details";


        private string _lastActionResponse;
        public string LastActionResponse
        {
            get
            {
                return _lastActionResponse;
            }
            set
            {
                if (_lastActionResponse != value)
                {
                    _lastActionResponse = value;
                    OnPropertyChanged("LastActionResponse");
                }
            }
        }
        public LaborFactorDetailsPageViewModel()
        {
            getdata();
        }
        private async void getdata()
        {
            if (Projects == null)
            {
                await GetProjects();
                await GetLaborFactors();
            }

        }
        private async Task GetProjects()
        {
            Projects = await HTTPHelper.GetProjectsAsync();
        }
        private async Task GetLaborFactors()
        {
            LaborFactors = await HTTPHelper.GetLaborFactorsAsync();
            if (LaborFactors!=null)
            {
                FilteredLaborFactors = LaborFactors.Where(x => x.ProjectId == 1).ToObservableCollection();
            }
            
        }
        private void GetLaborFactorsById(int id)
        {
            //LaborFactors = HTTPHelper.getLaborFactors().ToObservableCollection();
            //var filtered =await HTTPHelper.GetLaborFactorsAsyncByID(id);
            LaborFactorsFilterByProject = LaborFactors.Where(x => x.ProjectId == id);
            FilteredLaborFactors = LaborFactorsFilterByProject.ToObservableCollection();
        }
    }
}
