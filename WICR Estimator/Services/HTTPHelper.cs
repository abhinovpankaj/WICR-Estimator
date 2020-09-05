﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.DBModels;

namespace WICR_Estimator.Services
{
    public class HTTPHelper
    {
        //const string BASEURL = "http://wicrwebapi-test.us-east-1.elasticbeanstalk.com/api/";
        const string BASEURL = "http://localhost:5000/api/";
        #region Material
        public static async Task<IEnumerable<MaterialDB>> GetMaterialsAsync()
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("materials");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<MaterialDB>>();                    
                }
                else
                {
                    return null;
                }
            }
        }
        public static async Task<MaterialDB> PutMaterialAsync(int id, MaterialDB material)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<MaterialDB>("materials/"+id,material);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MaterialDB>();
                }
                else
                {
                    return null;
                }
            }
        }

       

        public static async Task<string> PutMaterialsAsync(IEnumerable<MaterialDB> materials)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<IEnumerable<MaterialDB>>("materials", materials);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<string>();
                }
                else
                {
                    return "Post Failed,Failed to Save Data.";
                }
            }
        }

        
        public static IEnumerable<MaterialDB> getMaterials()
        {
            List<MaterialDB> materials = new List<MaterialDB>();
            materials.Add(new MaterialDB { MaterialName = "tets", Coverage = 984, MaterialPrice = 23, ProdRateHorizontal = 80.4, LaborMinCharge = 0.3 });

            materials.Add(new MaterialDB { MaterialName = "abc", Coverage = 98.4, MaterialPrice = 33, ProdRateHorizontal = 8.4, LaborMinCharge = 0.3 });
            materials.Add(new MaterialDB { MaterialName = "abcd ", Coverage = .98, MaterialPrice = 13, ProdRateHorizontal = .58, LaborMinCharge = 0.3 });
            materials.Add(new MaterialDB { MaterialName = "this is a abc materal", Coverage = 18, MaterialPrice = 33, ProdRateHorizontal = 8, LaborMinCharge = 0.3 });

            return materials;

        }

        internal async static Task<IEnumerable<MaterialDB>> GetMaterialsAsyncByID(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("materials/project/" + id);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<MaterialDB>>();
                }
                else
                {
                    return null;
                }
            }
        }
        internal static MaterialDB GetMaterialByName(string matName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("materials/name/" + matName).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<MaterialDB>().Result;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Slope

        internal async static Task<string> PutSlopesAsync(IEnumerable<SlopeDB> slopes)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<IEnumerable<SlopeDB>>("slopes", slopes);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<string>();
                }
                else
                {
                    return "Post Failed,Failed to Save Data.";
                }
            }
        }

        internal async static Task<SlopeDB> PutSlopeAsync(int slopeId, SlopeDB selectedSlope)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<SlopeDB>("slopes/" + slopeId, selectedSlope);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<SlopeDB>();
                }
                else
                {
                    return null;
                }
            }
        }

       
        internal async static Task<IEnumerable<SlopeDB>> GetSlopesAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("slopes");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<SlopeDB>>();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region LaborFactor
        internal async static Task<IEnumerable<LaborFactorDB>> GetLaborFactorsAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("labors");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<LaborFactorDB>>();
                }
                else
                {
                    return null;
                }
            }
        }

        internal async static Task<LaborFactorDB> PutLaborFactorAsync(int id, LaborFactorDB laborfactor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<LaborFactorDB>("labors/" + id, laborfactor);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<LaborFactorDB>();
                }
                else
                {
                    return null;
                }
            }
        }

        internal async static Task<string> PutLaborFactorsAsync(IEnumerable<LaborFactorDB> laborfactors)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<IEnumerable<LaborFactorDB>>("labors", laborfactors);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<string>();
                }
                else
                {
                    return "Post Failed,Failed to Save Data.";
                }
            }
        }
        #endregion
        #region Project
        internal async static Task<IEnumerable<ProjectDB>> GetProjectsAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("projects");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<ProjectDB>>();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Metal

        internal async static Task<string> PutMetalsAsync(IEnumerable<MetalDB> metals)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<IEnumerable<MetalDB>>("metals", metals);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<string>();
                }
                else
                {
                    return "Post Failed,Failed to Save Data.";
                }
            }
        }

        internal async static Task<MetalDB> PutMetalAsync(int metalId, MetalDB selectedMetal)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<MetalDB>("metals/" + metalId, selectedMetal);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<MetalDB>();
                }
                else
                {
                    return null;
                }
            }
        }

        internal async static Task<IEnumerable<MetalDB>> GetMetalsAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("metals");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<MetalDB>>();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion


    }
}
