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
    public class SlopeDetailsPageViewModel : BaseViewModel, IPageViewModel
    {
        

        private IEnumerable<SlopeDB> SlopesFilterByProject;
        public string SearchText { get; set; } = "";
        public int SelectedProjectCount { get; set; }
        public string SelectedFactor { get; set; }
        public double UpdateFactor { get; set; }
        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand(SearchSlope, CanSearch);
                }

                return _searchCommand;
            }

        }
        
        private DelegateCommand _updateSlopesCommand;
        public DelegateCommand UpdateSlopesCommand
        {
            get
            {
                if (_updateSlopesCommand == null)
                {
                    _updateSlopesCommand = new DelegateCommand(UpdateSlopes, CanUpdate);
                }

                return _updateSlopesCommand;
            }

        }
        
        private bool allSelected;
        public bool AllSelected
        {
            get
            {
                return this.allSelected;
            }
            set
            {
                if (value != allSelected)
                {
                    allSelected = value;
                    foreach (var frt in this.FilteredSystemSlopes)
                        frt.IsChecked = value;
                    OnPropertyChanged("AllSelected");
                }

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

        private async void UpdateSlopes(object obj)
        {
            LastActionResponse = "";
            OnTaskStarted("Updating Slope details: ");
            var filteredSlopes = FilteredSystemSlopes.Where(x => x.IsChecked == true);
            foreach (var item in filteredSlopes)
            {
                switch (SelectedFactor.Split(':')[1].Trim())
                {
                    case "Labor Rate":
                        item.LaborRate = item.LaborRate * (1 + UpdateFactor);
                        break;
                    case "Cost per Mix":
                        item.PerMixCost = item.PerMixCost * (1 + UpdateFactor);
                        break;

                }
            }

            var result = await HTTPHelper.PutSlopesAsync(filteredSlopes);
            if (result==null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
                LastActionResponse = "Changes Saved Successfully." + filteredSlopes.Count() + " Slopes updated.";
            
            OnTaskCompleted(LastActionResponse);
        }

        private DelegateCommand _updateSlopeCommand;
        public DelegateCommand UpdateSlopeCommand
        {
            get
            {
                if (_updateSlopeCommand == null)
                {
                    _updateSlopeCommand = new DelegateCommand(UpdateSlope, CanUpdate);
                }

                return _updateSlopeCommand;
            }

        }

        private async void UpdateSlope(object obj)
        {
            OnTaskStarted("Updating Selected Slope.");
            SelectedSlope = await HTTPHelper.PutSlopeAsync(SelectedSlope.SlopeId, SelectedSlope);
            if (SelectedSlope == null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
            {
                SlopeDB slp = FilteredSystemSlopes.FirstOrDefault(x => x.SlopeId == SelectedSlope.SlopeId);
                slp.LaborRate = SelectedSlope.LaborRate;
                slp.PerMixCost = SelectedSlope.PerMixCost;
                
                LastActionResponse = "Changes saved successfully.";
            }
                
            OnTaskCompleted(LastActionResponse);
        }
        bool lastSearchProjectChanged;
        private async void SearchSlope(object obj)
        {
            List<SlopeDB> selectedSlopes = new List<SlopeDB>();
            if (lastSearchProjectChanged)
            {
                lastSearchProjectChanged = false;
                OnTaskStarted("Applying Filter, Please wait...");


                foreach (var item in Projects)
                {

                    if (item.IsSelected)
                    {
                        var filtered = await HTTPHelper.GetSlopesByProjectIDAsync(item.ProjectId);
                        if (filtered != null)
                        {
                            selectedSlopes.AddRange(filtered);
                            foreach (var mat in filtered)
                            {
                                mat.HookCheckBoxAction(OnSelectionChanged);
                            }
                        }

                    }

                }
                SlopesFilterByProject = selectedSlopes;
                OnTaskCompleted("");
            }
            if (SlopesFilterByProject != null)
            {
                if (SearchText != null)
                {
                    FilteredSystemSlopes = SlopesFilterByProject.Where(x => x.SlopeName.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
                }
                else
                    FilteredSystemSlopes = SlopesFilterByProject.ToObservableCollection();

            }
      
        }

        private void UpdateSelectedProjectMaterials()
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
                SlopesFilterByProject = Slopes.Where(x => x.ProjectId == 1);

            }
            else
                SlopesFilterByProject = Slopes.Join(selectedIDs, x => x.ProjectId, id => id, (x, id) => x);
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
                FilteredSystemSlopes = SlopesFilterByProject.ToObservableCollection();

            }

            return true;

        }

        private IEnumerable<SlopeDB> _Slopes = new ObservableCollection<SlopeDB>();
        public IEnumerable<SlopeDB> Slopes
        {
            get
            {
                return _Slopes;
            }
            set
            {
                if (_Slopes != value)
                {
                    _Slopes = value;
                    OnPropertyChanged("Slopes");
                }
            }
        }
        private ObservableCollection<SlopeDB> _filteredSlopes = new ObservableCollection<SlopeDB>();
        public ObservableCollection<SlopeDB> FilteredSystemSlopes
        {
            get
            {
                return _filteredSlopes;
            }
            set
            {
                if (_filteredSlopes != value)
                {
                    _filteredSlopes = value;
                    OnPropertyChanged("FilteredSystemSlopes");
                }
            }
        }
        private SlopeDB _selectedSlope;
        public SlopeDB SelectedSlope
        {
            get
            {
                return _selectedSlope;
            }
            set
            {
                if (_selectedSlope != value)
                {
                    _selectedSlope = value;
                    OnPropertyChanged("SelectedSlope");
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
                    GetSlopesById(SelectedProject.ProjectId);
                    OnPropertyChanged("SelectedProject");
                }
            }
        }

        public string Name => "Slope Details";


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
        public SlopeDetailsPageViewModel()
        {
            getdata();
        }
        private async void getdata()
        {
            if (Projects==null)
            {
                await GetProjects();
                await GetSlopes();
            }
           
        }
        private async Task GetProjects()
        {
            Projects = await HTTPHelper.GetProjectsAsync();
            foreach (var item in Projects)
            {
                item.HookCheckBoxAction(OnProjectSelectionChanged);
            }
        }
        private async Task GetSlopes()
        {
            Slopes = await HTTPHelper.GetSlopesAsync();
            if (Slopes!=null)
            {
                FilteredSystemSlopes = Slopes.Where(x => x.ProjectId == 1).ToObservableCollection();
            }

            foreach (var item in FilteredSystemSlopes)
            {
                item.HookCheckBoxAction(OnSelectionChanged);
            }
        }

        private void OnSelectionChanged(bool value)
        {
            if (allSelected && !value)
            { 
                allSelected = false;
                OnPropertyChanged("AllSelected");
            }
            else if (!allSelected && this.FilteredSystemSlopes.All(c => c.IsChecked))
            { 
                allSelected = true;
                OnPropertyChanged("AllSelected");
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
            lastSearchProjectChanged= true;
        }

        private void GetSlopesById(int id)
        {
            //Slopes = HTTPHelper.getSlopes().ToObservableCollection();
            //var filtered =await HTTPHelper.GetSlopesAsyncByID(id);
            SlopesFilterByProject = Slopes.Where(x => x.ProjectId == id);
            FilteredSystemSlopes = SlopesFilterByProject.ToObservableCollection();
        }
    }
}
