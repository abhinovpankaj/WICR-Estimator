using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using WICR_Estimator.DBModels;

namespace WICR_Estimator.Services
{
    public class HTTPHelper
    {
        const string BASEURL = "https://localhost:44367/api/";
        private static HttpClient client=new HttpClient();

        //public async Task<IEnumerable<T>> getResponseResult(string strAPI)
        //{          
        
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(BASEURL);
        //        //HTTP GET
        //        var responseTask = await client.GetAsync(strAPI);
        //        //responseTask.Wait();
        //        var result=responseTask.R
        //        if (result.IsSuccessStatusCode)
        //        {

        //            var readTask = await result.Content.ReadAsAsync<SysMaterial[]>();
                    
        //            _materials = readTask.Result.ToList().ToObservableCollection();

        //        }
        //}
        public static IEnumerable<MaterialDB> getMaterials()
        {
            List<MaterialDB> materials = new List<MaterialDB>();
            materials.Add(new MaterialDB { MaterialName = "tets", Coverage = 984, MaterialPrice = 23, ProdRateHorizontal = 80.4, LaborMinCharge = 0.3 });

            materials.Add(new MaterialDB { MaterialName = "abc", Coverage = 98.4, MaterialPrice = 33, ProdRateHorizontal = 8.4, LaborMinCharge = 0.3 });
            materials.Add(new MaterialDB { MaterialName = "abcd ", Coverage = .98, MaterialPrice = 13, ProdRateHorizontal = .58, LaborMinCharge = 0.3 });
            materials.Add(new MaterialDB { MaterialName = "this is a abc materal", Coverage = 18, MaterialPrice = 33, ProdRateHorizontal = 8, LaborMinCharge = 0.3 });

            return materials;

        }

    }
}
