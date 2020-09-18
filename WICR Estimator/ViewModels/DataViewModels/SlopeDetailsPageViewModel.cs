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

        private bool CanUpdate(object obj)
        {
            return true;
        }

        private async void UpdateSlopes(object obj)
        {
            var result = await HTTPHelper.PutSlopesAsync(FilteredSystemSlopes.Where(x => x.IsChecked == true));
            if (result==null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
                LastActionResponse = "Changes Saved Successfully.";

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
            SelectedSlope = await HTTPHelper.PutSlopeAsync(SelectedSlope.SlopeId, SelectedSlope);
        }

        private void SearchSlope(object obj)
        {
            if (SlopesFilterByProject != null)
            {
                FilteredSystemSlopes = SlopesFilterByProject.Where(x => x.SlopeName.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
            }
            else
                FilteredSystemSlopes = Slopes.Where(x => x.SlopeName.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
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

            return false;

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
        }
        private async Task GetSlopes()
        {
            Slopes = await HTTPHelper.GetSlopesAsync();
            if (Slopes!=null)
            {
                FilteredSystemSlopes = Slopes.Where(x => x.ProjectId == 1).ToObservableCollection();
            }
            
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
