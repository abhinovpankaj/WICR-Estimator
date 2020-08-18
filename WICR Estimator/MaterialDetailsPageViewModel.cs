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
        public string SearchText { get; set; }
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

        private void SearchMaterial(object obj)
        {
            FilteredSystemMaterials = Materials.Where(x=>x.MaterialName.Contains(SearchText)).ToObservableCollection();
        }

        private bool CanSearch(object obj)
        {
            return true;
        }

        private ObservableCollection<MaterialDB> _materials = new ObservableCollection<MaterialDB>();
        public ObservableCollection<MaterialDB> Materials
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

        public string Name => "Material Details";

        

        public MaterialDetailsPageViewModel()
        {
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://localhost:44367/api/");
            //    //HTTP GET
            //    var responseTask =  client.GetAsync("materials");
            //    responseTask.Wait();

            //    var result = responseTask.Result;z            //    if (result.IsSuccessStatusCode)
            //    {

            //        var readTask = result.Content.ReadAsAsync<SysMaterial[]>();
            //        readTask.Wait();

            //        _materials = readTask.Result.ToList().ToObservableCollection();

            //    }
            //}
            Materials = HTTPHelper.getMaterials().ToObservableCollection();
        }
    }

}

