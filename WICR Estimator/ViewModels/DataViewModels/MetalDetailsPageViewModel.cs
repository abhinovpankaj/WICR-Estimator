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
        
        //public List<MetalVendor> Vendors { get; set; }
        //public List<MetalItemType  > MetalTypes { get; set; }

        public string VendorName { get; set; }
        public string MetalType { get; set; }
        public string SelectedFactor { get; set; }
        public double UpdateFactor { get; set; }

        //public MetalVendor _selectedVendors;
        //public MetalVendor SelectedVendors
        //{
        //    get
        //    {
        //        return _selectedVendors;
        //    }
        //    set
        //    {
        //        //if (_selectedVendors != value)
        //        //{
        //            _selectedVendors = value;
        //            OnPropertyChanged("SelectedVendors");
        //            if (value!=null)
        //            {
        //                Vendors.First(x => x.VendorName == value.VendorName).IsSelected = !value.IsSelected;
        //            }
                    
        //        //}
        //    }
        //}
        //public MetalItemType _selectedMetalType;
        //public MetalItemType SelectedMetalType
        //{
        //    get
        //    {
        //        return _selectedMetalType;
        //    }
        //    set
        //    {
        //        //if (_selectedMetalType != value)
        //        //{
        //            _selectedMetalType = value;
        //            OnPropertyChanged("SelectedMetalType");
        //            if (value!=null)
        //            {
        //                MetalTypes.First(x => x.MetalType == value.MetalType).IsSelected = !value.IsSelected;
        //            }
                    
        //        //}
        //    }
        //}

        //private IEnumerable<MetalDB> MetalsFilterByProject;

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
                    foreach (var frt in this.FilteredSystemMetals)
                        frt.IsChecked = value;
                    OnPropertyChanged("AllSelected");
                }

            }
        }
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
            OnTaskStarted("Updating Metal factors.");
            LastActionResponse = "";
            var filteredMetals = FilteredSystemMetals.Where(x => x.IsChecked == true);
            foreach (var item in filteredMetals)
            {
                switch (SelectedFactor.Split(':')[1].Trim())
                {
                    case "Metal Price":
                        item.MetalPrice = item.MetalPrice * (1 + UpdateFactor);
                        break;
                    case "Production Rate":
                        item.ProductionRate = item.ProductionRate * (1 + UpdateFactor);
                        break;
                    
                }
                await HTTPHelper.PutMetalAsync(item.MetalId, item);
            }

            
            //var result = await HTTPHelper.PutMetalsAsync(filteredMetals);
            //if (result == null)
            //{
            //    LastActionResponse = "Failed to save the data";
            //}
            //else
            //    LastActionResponse = "Changes Saved Successfully."+ filteredMetals.Count() +" metals updated.";
            OnTaskCompleted("Changes Saved Successfully." + filteredMetals.Count() + " metals updated.");
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
            OnTaskStarted("Updating Selected Metal factors.");
            SelectedMetal = await HTTPHelper.PutMetalAsync(SelectedMetal.MetalId, SelectedMetal);
            if (SelectedMetal!=null)
            {
                LastActionResponse= "Failed to save the data";
            }
            else
                LastActionResponse = "Changes saved successfully.";
            OnTaskCompleted(LastActionResponse);
        }

        private void SearchMetal(object obj)
        {
            IEnumerable<MetalDB> query = Metals;
            if (SearchText != null)
            {
                query = Metals.Where(x => x.MetalName.ToUpper().Contains(SearchText.ToUpper()));               
            }

            if (VendorName != null)
            {
                query = query.Where(x => x.Vendor == VendorName);
            }

            
            if (MetalType != null)
            {

                query = query.Where(x => x.MetalType == MetalType);

            }
            FilteredSystemMetals = query.ToObservableCollection();
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
            if (VendorName!=null)
            {
                return true;
            }
            else if (MetalType!=null)
            {
                return true;

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
            //Vendors = new List<MetalVendor>();
            //Vendors.Add(new MetalVendor { VendorName="Chivon", IsSelected=true });
            //Vendors.Add(new MetalVendor { VendorName = "Thunderbird", IsSelected = true });
            //MetalTypes = new List<MetalItemType>();
            //MetalTypes.Add(new MetalItemType { MetalType = "26 ga. Type 304 Stainless Steel", IsSelected = true });
            //MetalTypes.Add(new MetalItemType { MetalType = "16oz Copper", IsSelected = true });
            //MetalTypes.Add(new MetalItemType { MetalType = "24ga. Galvanized Primed Steel", IsSelected = true });
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

        private void OnSelectionChanged(bool value)
        {
            if (allSelected && !value)
            { // all are selected, and one gets turned off
                allSelected = false;
                OnPropertyChanged("AllSelected");
            }
            else if (!allSelected && this.FilteredSystemMetals.All(c => c.IsChecked))
            { // last one off one gets turned on, resulting in all being selected
                allSelected = true;
                OnPropertyChanged("AllSelected");
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
            
            foreach (var item in FilteredSystemMetals)
            {
                item.HookCheckBoxAction(OnSelectionChanged);
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
