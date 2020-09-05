using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.DBModels;
using WICR_Estimator.Services;
using WICR_Estimator.ViewModels;


namespace WICR_Estimator
{
    public class MaterialDetailsPageViewModel : BaseViewModel,IPageViewModel
    {
        private IEnumerable<MaterialDB> MaterialsFilterByProject;
        public string SearchText { get; set; } = "";
        private DelegateCommand _searchCommand ;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand(SearchMaterial, CanSearch);
                }

                return _searchCommand;
            }
            
        }
        private DelegateCommand _updateMaterialsCommand;
        public DelegateCommand UpdateMaterialsCommand
        {
            get
            {
                if (_updateMaterialsCommand == null)
                {
                    _updateMaterialsCommand = new DelegateCommand(UpdateMaterials, CanUpdate);
                }

                return _updateMaterialsCommand;
            }

        }

        private bool CanUpdate(object obj)
        {
            if (FilteredSystemMaterials.Count>0)
            {
                return true;
            }
            else
                return false;
        }

        private async void UpdateMaterials(object obj)
        {
            LastActionResponse = "";
            string result=await HTTPHelper.PutMaterialsAsync(FilteredSystemMaterials.Where(x=>x.IsChecked==true));
            LastActionResponse = result;
        }

        private DelegateCommand _updateMaterialCommand;
        public DelegateCommand UpdateMaterialCommand
        {
            get
            {
                if (_updateMaterialCommand == null)
                {
                    _updateMaterialCommand = new DelegateCommand(UpdateMaterial, CanUpdate);
                }

                return _updateMaterialCommand;
            }

        }

        private async void UpdateMaterial(object obj)
        {
            //LastActionResponse =await HTTPHelper.PutMaterialAsync(SelectedMaterial.MaterialId, SelectedMaterial); ;
            var result = await HTTPHelper.PutMaterialAsync(SelectedMaterial.MaterialId,SelectedMaterial);
            if (result==null)
            {
                LastActionResponse = "Failed to Save data.";
            }
            else
            {
                LastActionResponse = "Changes Saved Successfully.";
                SelectedMaterial = result;
            }
                     
        }

        private void SearchMaterial(object obj)
        {
            if (MaterialsFilterByProject!=null)
            {
                FilteredSystemMaterials = MaterialsFilterByProject.Where(x => x.MaterialName.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
            }
            else
                FilteredSystemMaterials = Materials.Where(x => x.MaterialName.ToUpper().Contains(SearchText.ToUpper())).ToObservableCollection();
        }

        private bool CanSearch(object obj)
        {
            if (SearchText!=null)
            {
                if (SearchText.Length > 0)
                {
                    return true;
                }
            }
            else
            {
                FilteredSystemMaterials = MaterialsFilterByProject.ToObservableCollection();
                
            }

            return false;

        }

        private IEnumerable<MaterialDB> _materials = new ObservableCollection<MaterialDB>();
        public IEnumerable<MaterialDB> Materials
        {
            get
            {
                return _materials;
            }
            set
            {
                if (_materials != value)
                {
                    _materials = value;
                    OnPropertyChanged("Materials");
                }
            }
        }
        private ObservableCollection<MaterialDB> _filteredmaterials = new ObservableCollection<MaterialDB>();
        public ObservableCollection<MaterialDB> FilteredSystemMaterials
        {
            get
            {
                return _filteredmaterials;
            }
            set
            {
                if (_filteredmaterials != value)
                {
                    _filteredmaterials = value;
                    OnPropertyChanged("FilteredSystemMaterials");
                }
            }
        }
        private MaterialDB _selectedmaterial;
        public MaterialDB SelectedMaterial
        {
            get
            {
                return _selectedmaterial;
            }
            set
            {
                if (_selectedmaterial != value)
                {
                    _selectedmaterial = value;
                    OnPropertyChanged("SelectedMaterial");
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
                    GetMaterialsById(SelectedProject.ProjectId);
                    OnPropertyChanged("SelectedProject");
                }
            }
        }
        
        public string Name => "Material Details";

        
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
        public MaterialDetailsPageViewModel()
        {
            getdata();
        }
        private async void getdata()
        {
            if (Projects==null)
            {
                await GetProjects();
                await GetMaterials();
            }
            
        }
        private async Task GetProjects()
        {
            Projects= await HTTPHelper.GetProjectsAsync();
            
        }
        private async Task GetMaterials()
        {
            Materials =await HTTPHelper.GetMaterialsAsync();
            FilteredSystemMaterials = Materials.Where(x=>x.ProjectId==1).ToObservableCollection();
        }
        private void GetMaterialsById(int id)
        {
            //Materials = HTTPHelper.getMaterials().ToObservableCollection();
            //var filtered =await HTTPHelper.GetMaterialsAsyncByID(id);
            LastActionResponse = "";
            MaterialsFilterByProject = Materials.Where(x=>x.ProjectId==id);
            FilteredSystemMaterials = MaterialsFilterByProject.ToObservableCollection();
        }
    }

}

