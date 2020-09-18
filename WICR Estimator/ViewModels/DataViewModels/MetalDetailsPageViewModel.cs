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
    public class MetalVendor:BaseViewModel
    {
        public string VendorName { get; set; }
        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return VendorName;
        }
    }
    public class MetalItemType : BaseViewModel
    {
        public string MetalType { get; set; }
        public bool IsSelected { get; set; }
        public override string ToString()
        {
            return MetalType;
        }
    }
    public class MetalDetailsPageViewModel : BaseViewModel, IPageViewModel
    {
        public List<MetalVendor> Vendors { get; set; }
        public List<MetalItemType  > MetalTypes { get; set; }

        public MetalVendor _selectedVendors;
        public MetalVendor SelectedVendors
        {
            get
            {
                return _selectedVendors;
            }
            set
            {
                //if (_selectedVendors != value)
                //{
                    _selectedVendors = value;
                    OnPropertyChanged("SelectedVendors");
                    if (value!=null)
                    {
                        Vendors.First(x => x.VendorName == value.VendorName).IsSelected = !value.IsSelected;
                    }
                    
                //}
            }
        }
        public MetalItemType _selectedMetalType;
        public MetalItemType SelectedMetalType
        {
            get
            {
                return _selectedMetalType;
            }
            set
            {
                //if (_selectedMetalType != value)
                //{
                    _selectedMetalType = value;
                    OnPropertyChanged("SelectedMetalType");
                    if (value!=null)
                    {
                        MetalTypes.First(x => x.MetalType == value.MetalType).IsSelected = !value.IsSelected;
                    }
                    
                //}
            }
        }

        //private IEnumerable<MetalDB> MetalsFilterByProject;
        public string SearchText { get; set; } = "";
        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand(SearchMetal, CanSearch);
                }

                return _searchCommand;
            }

        }
        private DelegateCommand _updateMetalsCommand;
        public DelegateCommand UpdateMetalsCommand
        {
            get
            {
                if (_updateMetalsCommand == null)
                {
                    _updateMetalsCommand = new DelegateCommand(UpdateMetals, CanUpdate);
                }

                return _updateMetalsCommand;
            }

        }

        private bool CanUpdate(object obj)
        {
            return true;
        }

        private async void UpdateMetals(object obj)
        {
            var result = await HTTPHelper.PutMetalsAsync(FilteredSystemMetals.Where(x => x.IsChecked == true));
            if (result == null)
            {
                LastActionResponse = "Failed to save the data";
            }
            else
                LastActionResponse = "Changes Saved Successfully.";

        }

        private DelegateCommand _updateMetalCommand;
        public DelegateCommand UpdateMetalCommand
        {
            get
            {
                if (_updateMetalCommand == null)
                {
                    _updateMetalCommand = new DelegateCommand(UpdateMetal, CanUpdate);
                }

                return _updateMetalCommand;
            }

        }

        private async void UpdateMetal(object obj)
        {
            SelectedMetal = await HTTPHelper.PutMetalAsync(SelectedMetal.MetalId, SelectedMetal);
        }

        private void SearchMetal(object obj)
        {
            if (SearchText != null)
            {
                var filtered = Metals.Where(x => x.MetalName.ToUpper().Contains(SearchText.ToUpper()));

                //var selectedVen = Vendors.Where(x => x.IsSelected==true);

                //if (selectedVen!=null)
                //{
                //    List<MetalDB> tempFilter=new List<MetalDB>();
                //    foreach (var item in selectedVen)
                //    {
                //        var temp  = filtered.Where(x => x.Vendor == item.VendorName).ToList();
                //        if (temp!=null)
                //        {
                //            tempFilter = tempFilter.Add(temp);
                //        }

                //    }

                //}

                //var selectedtype = MetalTypes.Where(x => x.IsSelected==true);
                //if (selectedtype != null)
                //{
                //    foreach (var item in selectedtype)
                //    {
                //        filtered = filtered.Where(x => x.MetalType ==item.MetalType);
                //    }
                //}
                FilteredSystemMetals = filtered.ToObservableCollection();
            }
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
                SearchMetal(null);
            }

            return false;

        }

        private IEnumerable<MetalDB> _Metals = new ObservableCollection<MetalDB>();
        public IEnumerable<MetalDB> Metals
        {
            get
            {
                return _Metals;
            }
            set
            {
                if (_Metals != value)
                {
                    _Metals = value;
                    OnPropertyChanged("Metals");
                }
            }
        }
        private ObservableCollection<MetalDB> _filteredMetals = new ObservableCollection<MetalDB>();
        public ObservableCollection<MetalDB> FilteredSystemMetals
        {
            get
            {
                return _filteredMetals;
            }
            set
            {
                if (_filteredMetals != value)
                {
                    _filteredMetals = value;
                    OnPropertyChanged("FilteredSystemMetals");
                }
            }
        }
        private MetalDB _selectedMetal;
        public MetalDB SelectedMetal
        {
            get
            {
                return _selectedMetal;
            }
            set
            {
                if (_selectedMetal != value)
                {
                    _selectedMetal = value;
                    OnPropertyChanged("SelectedMetal");
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
        //private ProjectDB _selectedProject;
        //public ProjectDB SelectedProject
        //{
        //    get
        //    {
        //        return _selectedProject;
        //    }
        //    set
        //    {
        //        if (_selectedProject != value)
        //        {
        //            _selectedProject = value;
        //            GetMetalsById(SelectedProject.ProjectId);
        //            OnPropertyChanged("SelectedProject");
        //        }
        //    }
        //}

        public string Name => "Metal Details";


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
        public MetalDetailsPageViewModel()
        {
            Vendors = new List<MetalVendor>();
            Vendors.Add(new MetalVendor { VendorName="Chivon", IsSelected=true });
            Vendors.Add(new MetalVendor { VendorName = "Thunderbird", IsSelected = true });
            MetalTypes = new List<MetalItemType>();
            MetalTypes.Add(new MetalItemType { MetalType = "26 ga. Type 304 Stainless Steel", IsSelected = true });
            MetalTypes.Add(new MetalItemType { MetalType = "16oz Copper", IsSelected = true });
            MetalTypes.Add(new MetalItemType { MetalType = "24ga. Galvanized Primed Steel", IsSelected = true });
            getdata();
        }
        private async void getdata()
        {
            //await GetProjects();
            if (Metals.Count()==0)
            {
                await GetMetals();
            }
            
        }
        private async Task GetProjects()
        {
            Projects = await HTTPHelper.GetProjectsAsync();

        }
        private async Task GetMetals()
        {
            Metals = await HTTPHelper.GetMetalsAsync();
            if (Metals!=null)
            {
                FilteredSystemMetals = Metals.Where(x => x.MetalId < 20).ToObservableCollection();
            }
            
        }
        //private void GetMetalsById(int id)
        //{
        //    //Metals = HTTPHelper.getMetals().ToObservableCollection();
        //    //var filtered =await HTTPHelper.GetMetalsAsyncByID(id);
        //    MetalsFilterByProject = Metals.Where(x => x.ProjectId == id);
        //    FilteredSystemMetals = MetalsFilterByProject.ToObservableCollection();
        //}
    }
}
