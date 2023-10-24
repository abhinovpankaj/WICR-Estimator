using System;
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
        //const string BASEURL = "http://wicrwebapi-dev.us-east-1.elasticbeanstalk.com/api/";
        const string BASEURL = "http://estimator.wicr.net/api/";
        //public static object ConfigurationManager { get; private set; }
        //ConfigurationManager.AppSettings["apiUrl"]
        //const string BASEURL = "http://localhost:61955/api/";

        static HttpClient GetApiClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BASEURL);
            return client;
        }

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
                    System.Threading.Thread.Sleep(1000);
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

        internal async static Task<IList<MaterialDB>> GetMaterialsAsyncByID(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("materials/project/" + id);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<MaterialDB>>();
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

        internal async static Task<IList<SlopeDB>> GetSlopesByProjectIDAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("slopes/project/" + id);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<SlopeDB>>();
                }
                else
                {
                    return null;
                }
            }
        }

        internal async static Task<IList<LaborFactorDB>> GetLaborFactorsAsyncByProjectID(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("labors/project/" + id);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<LaborFactorDB>>();
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

        internal async static Task<LaborFactorDB> GetLaborFactorByProjectName(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response =await client.GetAsync("labors/project/" + id);
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

        internal async static Task<ProjectDB> GetProjectByNameAsync(string projName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("projects/name/"+projName);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<ProjectDB>();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Metal

        private async static Task<string> PutMetalsAsync(IEnumerable<MetalDB> metals)
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

        internal async static Task<IList<MetalDB>> GetMetalsAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("metals");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<MetalDB>>();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Freight
        internal async static Task<string> PutFreightsAsync(IEnumerable<FreightDB> freights)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<IEnumerable<FreightDB>>("freights", freights);
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

        internal async static Task<FreightDB> PutFreightAsync(int freightId, FreightDB selectedFreight)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<FreightDB>("freights/" + freightId, selectedFreight);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<FreightDB>();
                }
                else
                {
                    return null;
                }
            }
        }

        internal async static Task<IList<FreightDB>> GetFreightsAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("freights");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<FreightDB>>();
                }
                else
                {
                    return null;
                }
            }
        }
        internal async static Task<IList<FreightDB>> GetFreights()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("freights");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<FreightDB>>();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        public  async static Task<DBData> FetchFromDbAndSave(string originalProjectname)
        {
            DBData dbData=new DBData();

            var project = await HTTPHelper.GetProjectByNameAsync(originalProjectname);
            //var task = HTTPHelper.GetMetalsAsync();
            //task.Wait();
            //DataSerializerService.DSInstance.dbData.MetalDBData = task.Result;
            
            dbData.MetalDBData = await HTTPHelper.GetMetalsAsync();

            dbData.LaborDBData = await HTTPHelper.GetLaborFactorsAsyncByProjectID(project.ProjectId==28?1:project.ProjectId);

            dbData.SlopeDBData = await HTTPHelper.GetSlopesByProjectIDAsync(project.ProjectId==28?1:project.ProjectId);


            dbData.MaterialDBData = await HTTPHelper.GetMaterialsAsyncByID(project.ProjectId);


            dbData.FreightDBData = await HTTPHelper.GetFreightsAsync();

            
            return dbData;
        }

        #region Projectdetails
        internal async static Task<ProjectDetailsDB> PostProjectDetails(ProjectDetailsDB prjDetails)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<ProjectDetailsDB>("projectdetails", prjDetails);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<ProjectDetailsDB>();
                }
                else
                {
                    return null;
                }
            }
        }

        

        internal async static Task<string> PutProjectDetails(int id ,ProjectDetailsDB prjDetails)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<ProjectDetailsDB>("projectdetails/" + id, prjDetails);
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


        #region estimate
        internal async static Task<EstimateDB> PostEstimate(EstimateDB est)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<EstimateDB>("estimates", est);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<EstimateDB>();
                }
                else
                {
                    return null;
                }
            }
        }

        internal async static Task<string> PutEstimate(int id, EstimateDB est)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<EstimateDB>("estimates/" + id, est);
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

        #region login
        public async static Task<LoginResponseDB> LoginUser(LoginModel user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<LoginModel>("authenticate/login" , user);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<LoginResponseDB>();
                }
                else
                {
                    return null;
                }
            }
        }

        public async static Task<SuccessResponse> AddAdminUser(UserModel user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<UserModel>("authenticate/register-admin", user);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<SuccessResponse>();
                }
                else
                {
                    return null;
                }
            }
        }
        
        public async static Task<SuccessResponse> AddUser(UserModel user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<UserModel>("authenticate/register", user);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<SuccessResponse>();
                }
                else
                {
                    return await response.Content.ReadAsAsync<SuccessResponse>();
                }
            }
        }
        public async static Task<IList<UserWithRoles>> GetAllUsers()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("authenticate/users");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IList<UserWithRoles>>();
                }
                else
                {
                    return null;
                }
            }
        }
        public async static Task<SuccessResponse> UpdateUser(UpdateUserModel updateUserModel, string userType)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<UpdateUserModel>("authenticate/" + userType,updateUserModel);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<SuccessResponse>();
                }
                else
                {
                    return null;
                }
            }
        }
        public async static Task<SuccessResponse> DeleteUser( string username)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.DeleteAsync("authenticate/" + username);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<SuccessResponse>();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion


        public static async Task<PriceVersion> PutPriceVersionAsync(int id, PriceVersion price)
        {

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsJsonAsync<PriceVersion>("version/" + id, price);
                if (response.IsSuccessStatusCode)
                {
                    System.Threading.Thread.Sleep(1000);
                    return await response.Content.ReadAsAsync<PriceVersion>();
                }
                else
                {
                    return null;
                }
            }
        }

        public static async Task<IEnumerable<PriceVersion>> GetPriceVersionsAsync()
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("version");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<PriceVersion>>();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
