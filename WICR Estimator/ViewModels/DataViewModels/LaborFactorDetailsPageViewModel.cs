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
        #region Laborfactors
        private IEnumerable<LaborFactorDB> LaborFactorsFilterByProject;
        private bool allLaborSelected;
        public int SelectedProjectCount { get; set; }
        public double UpdateFactor { get; set; }
        public bool AllLaborSelected
        {
            get
            {
                return this.allLaborSelected;
            }
            set
            {
                if (value != allLaborSelected)
                {
                    allLaborSelected = value;
                    foreach (var frt in this.FilteredLaborFactors)
                        frt.IsChecked = value;
                    OnPropertyChanged("AllLaborSelected");
                }

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
        private bool _checkAllProjects;
        public bool CheckAllProjects
        {
            get { return _checkAllProjects; }
            set
            {
                _checkAllProjects = value;
                foreach (var item in Projects)
                {
                    item.IsSelected = value;
                }
                OnPropertyChanged("CheckAllProjects");
                OnPropertyChanged("Projects");
            }
        }

        private bool CanUpdate(object obj)
        {
            return true;
        }

        private async void UpdateLaborFactors(object obj)
        {
            LastActionResponse = "";
            var filteredlabors = FilteredLaborFactors.Where(x => x.IsChecked == true);
            foreach (var item in filteredlabors)
            {
                item.Value = item.Value * (1 + UpdateFactor);
            }
            var result = await HTTPHelper.PutLaborFactorsAsync(filteredlabors);
            if (result == null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
                LastActionResponse = "Changes Saved Successfully. " + filteredlabors.Count() + " factors updated successfully.";

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
            var result  = await HTTPHelper.PutLaborFactorAsync(SelectedLaborFactor.LaborId, SelectedLaborFactor);
            if (result == null)
            {
                LastActionResponse = "Failed to Save data.";
            }
            else
            {
                LastActionResponse = "Changes Saved Successfully.";
                SelectedLaborFactor = result;
            }
        }

        private void SearchLaborFactor(object obj)
        {
            UpdateSelectedProjectLaborFactors();
            if (LaborFactorsFilterByProject != null)
            {
                FilteredLaborFactors = LaborFactorsFilterByProject.Where(x => x.Name.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
            }
            else
                FilteredLaborFactors = LaborFactors.Where(x => x.Name.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();

        }

        private void UpdateSelectedProjectLaborFactors()
        {
            List<int> selectedIDs = new List<int>();
            foreach (var item in Projects)
            {

                if (item.IsSelected)
                {
                    selectedIDs.Add(item.ProjectId);
                }

            }
            if (selectedIDs.Count == 0)
            {
                LaborFactorsFilterByProject = LaborFactors.Where(x => x.ProjectId == 1);

            }
            else
                LaborFactorsFilterByProject = LaborFactors.Join(selectedIDs, x => x.ProjectId, id => id, (x, id) => x);
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

            return true;

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

        private void OnProjectSelectionChanged(bool value)
        {
            if (_checkAllProjects && !value)
            { // all are selected, and one gets turned off
                _checkAllProjects = false;
                OnPropertyChanged("CheckAllProjects");
            }
            else if (!_checkAllProjects && this.Projects.All(c => c.IsSelected))
            { // last one off one gets turned on, resulting in all being selected
                _checkAllProjects = true;
                OnPropertyChanged("CheckAllProjects");
            }
            SelectedProjectCount = Projects.Where(c => c.IsSelected).Count();
            OnPropertyChanged("SelectedProjectCount");
        }

        private async Task GetLaborFactors()
        {
            LaborFactors = await HTTPHelper.GetLaborFactorsAsync();
            if (LaborFactors != null)
            {
                FilteredLaborFactors = LaborFactors.Where(x => x.ProjectId == 1).ToObservableCollection();
            }
            foreach (var item in FilteredLaborFactors)
            {
                item.HookCheckBoxAction(OnLaborSelectionChanged);
            }
        }
        private void GetLaborFactorsById(int id)
        {
            //LaborFactors = HTTPHelper.getLaborFactors().ToObservableCollection();
            //var filtered =await HTTPHelper.GetLaborFactorsAsyncByID(id);
            LaborFactorsFilterByProject = LaborFactors.Where(x => x.ProjectId == id);
            FilteredLaborFactors = LaborFactorsFilterByProject.ToObservableCollection();
        }

        private void OnLaborSelectionChanged(bool value)
        {
            if (allLaborSelected && !value)
            { // all are selected, and one gets turned off
                allLaborSelected = false;
                OnPropertyChanged("AllLaborSelected");
            }
            else if (!allFreightSelected && this.FilteredLaborFactors.All(c => c.IsChecked))
            { // last one off one gets turned on, resulting in all being selected
                allLaborSelected = true;
                OnPropertyChanged("AllLaborSelected");
            }
        }
        #endregion

        #region freight

        public ObservableCollection<FreightDB> FreightFactors { get; set; }

        private bool allFreightSelected;

        public bool AllFreightSelected
        {
            get
            {
                return this.allFreightSelected;
            }
            set
            {
                if (value!= allFreightSelected)
                {
                    allFreightSelected = value;
                    foreach (var frt in this.FreightFactors)
                        frt.IsChecked = value;
                    OnPropertyChanged("AllFreightSelected");
                }
                
            }
        }

        private DelegateCommand _updateFreightFactorCommand;
        public DelegateCommand UpdateFreightFactorCommand
        {
            get
            {
                if (_updateFreightFactorCommand == null)
                {
                    _updateFreightFactorCommand = new DelegateCommand(UpdateFreightFactor, CanUpdate);
                }

                return _updateFreightFactorCommand;
            }

        }

        private async void UpdateFreightFactor(object obj)
        {
            SelectedFreightFactor = await HTTPHelper.PutFreightAsync(SelectedFreightFactor.FreightID, SelectedFreightFactor);
        }
        private DelegateCommand _updateFreightFactorsCommand;
        public DelegateCommand UpdateFreightFactorsCommand
        {
            get
            {
                if (_updateFreightFactorsCommand == null)
                {
                    _updateFreightFactorsCommand = new DelegateCommand(UpdateFreightFactors, CanUpdate);
                }

                return _updateFreightFactorsCommand;
            }

        }

        private async void UpdateFreightFactors(object obj)
        {
            var result = await HTTPHelper.PutFreightsAsync(FreightFactors.Where(x => x.IsChecked == true));
            if (result == null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
                LastActionResponse = "Changes Saved Successfully.";

        }
        private async Task GetFreightFactors()
        {

            var freightFactors = await HTTPHelper.GetFreightsAsync();
            FreightFactors = freightFactors.ToObservableCollection();
            foreach (var item in FreightFactors)
            {
                item.HookCheckBoxAction(OnFreightSelectionChanged);
            }
            OnPropertyChanged("FreightFactors");
        }

        private FreightDB _selectedFreightFactor;
        public FreightDB SelectedFreightFactor
        {
            get
            {
                return _selectedFreightFactor;
            }
            set
            {
                if (_selectedFreightFactor != value)
                {
                    _selectedFreightFactor = value;
                    OnPropertyChanged("SelectedFreightFactor");
                }
            }
        }

        private void OnFreightSelectionChanged(bool value)
        {
            if (allFreightSelected && !value)
            { // all are selected, and one gets turned off
                allFreightSelected = false;
                OnPropertyChanged("AllFreightSelected");
            }
            else if (!allFreightSelected && this.FreightFactors.All(c => c.IsChecked))
            { // last one off one gets turned on, resulting in all being selected
                allFreightSelected = true;
                OnPropertyChanged("AllFreightSelected");
            }
        }
        #endregion

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
                await GetFreightFactors();
            }
            foreach (var item in Projects)
            {
                item.HookCheckBoxAction(OnProjectSelectionChanged);
            }

        }
        private async Task GetProjects()
        {
            Projects = await HTTPHelper.GetProjectsAsync();
        }
       
    }
}
