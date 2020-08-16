using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;


namespace WICR_Estimator
{
    public class MaterialDetailsPageViewModel : BaseViewModel,IPageViewModel
    {
        private ObservableCollection<SysMaterial> _materials = new ObservableCollection<SysMaterial>();
        public ObservableCollection<SysMaterial> Materials
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

        private SysMaterial _selectedmaterial;
        public SysMaterial SelectedMaterial
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

            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {

            //        var readTask = result.Content.ReadAsAsync<SysMaterial[]>();
            //        readTask.Wait();

            //        _materials = readTask.Result.ToList().ToObservableCollection();

            //    }
            //}
        }
    }

}

