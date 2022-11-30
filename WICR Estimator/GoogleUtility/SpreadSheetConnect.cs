using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace WICR_Estimator.GoogleUtility
{

    public static class SpreadSheetConnect
    {
        //private static readonly object lockobj=new object();
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly};
        static string ApplicationName = "WICR Estimator";
        public static  GoogleCredential credential;
        //public static UserCredential credential;
        public static SheetsService service ;
        private static string spreadsheetId = "1pQG-Z9vaaWhCjCUiG1XqmEj1Pjavoz86RCfG6-ews2k";
        
            public async static Task<IList<IList<Object>>> GetDataFromGoogleSheetsAsync(string projectName, DataType datatype)
        //public static IList<IList<Object>> GetDataFromGoogleSheetsAsync(string projectName, DataType datatype)
        {
            if (credential==null)
            {
                //using (var stream =
                //    new FileStream(@"GoogleUtility\credentials.json", FileMode.Open, FileAccess.Read))
                //{
                //    string credPath = "token.json";
                //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                //        GoogleClientSecrets.Load(stream).Secrets,
                //        new[] { SheetsService.Scope.SpreadsheetsReadonly },
                //        "user",
                //        CancellationToken.None,
                //        new FileDataStore(credPath, true)).Result;

                //}
                string jsonPath= System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); 
                using (var stream = new FileStream(jsonPath + @"\GoogleUtility\client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(Scopes);
                }
            }

            // Create Google Sheets API service.
            if (service==null)
            {
                service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,                   
                });
            }            
            // Define request parameters.
            
            String range =  "Pricing!" + GetRangeFromXML(projectName, datatype);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            //ValueRange response = request.ExecuteAsync().Result;
            ValueRange response = await request.ExecuteAsync();

            return response.Values;
            
        }
        private static XmlDocument doc;
        
        private static string GetRangeFromXML(string projectName,DataType datatype )
        {
            string prjName="";
            if (doc==null)
            {
                doc = new XmlDocument();
                string jsonPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                doc.Load(jsonPath+@"\GoogleUtility\ProjectGoogleRangeInfo.xml");
            }
            switch (projectName)
            {
                case "IsSheetUpdated":
                    prjName = "IsSheetUpdated";
                    break;
                case "Weather Wear":
                    prjName = "WeatherWear";
                    break;
                case "Weather Wear Rehab":
                    prjName = "WeatherWearRehab";
                    break;
                case "Barrier Guard":
                    prjName = "DexotexBarrier";
                    break;
                case "Endurokote":
                    prjName = "EnduroKote";
                    break;
                case "Reseal all systems":
                    prjName = "WeatherWearReseal";
                    break;
                case "Color Wash Reseal":
                    prjName = "WestcoatColor";
                    break;
                case "Desert Crete":
                    prjName = "DesertBrand";
                    break;
                case "Pedestrian System":
                    prjName = "Pedestrian";
                    break;
                case "Parking Garage":
                    prjName = "Parking";
                    break;
                case "Resistite":
                    prjName = "DRConcrete";
                    break;
                case "MACoat":
                    prjName = "MACoat";
                    break;
                case "ALX":
                    prjName = "ALX";
                    break;
                case "Multicoat":
                    prjName = "Multicoat";
                    break;
                case "Pli-Dek":
                    prjName = "Plidek";
                    break;
                case "Paraseal":
                    prjName = "Paraseal";
                    break;
                case "Tufflex":
                    prjName = "Tufflex";
                    break;
                case "Paraseal LG":
                    prjName = "ParasealLG";
                    break;
                case "201":
                case "250 GC":
                    prjName = "Tremco201";
                    break;
                case "Dexcellent II":
                    prjName = "Dexcellent";
                    break;
                case "860 Carlisle":
                    prjName = "Carlisle860";
                    break;
                case "Westcoat BT":
                    prjName = "WestcoatDualMembrane";
                    break;
                case "UPI BT":
                    prjName = "UPIBelowTile";
                    break;
                case "Dual Flex":
                    prjName = "DualFlex";
                    break;
                case "Westcoat Epoxy":
                    prjName = "ColorFlake";
                    break;
                case "Polyurethane Injection Block":
                    prjName = "DeNeef";
                    break;
                case "Xypex":
                    prjName = "Xypex";
                    break;
                case "Blank":
                    prjName = "Blank";
                    break;
                default:
                    break;
            }
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Projects/"+prjName+ "/"+datatype+"Range");
            return node.InnerText;
        }

        public static IList<IList<Object>> GetDataFromGoogleSheets(string projectName, DataType datatype)
        
        {
            try
            {
                if (credential == null)
                {
                    string jsonPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    
                    using (var stream = new FileStream(jsonPath + @"\GoogleUtility\client_secret.json", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        credential = GoogleCredential.FromStream(stream)
                            .CreateScoped(Scopes);
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
 
            // Create Google Sheets API service.
            if (service == null)
            {
                service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            // Define request parameters.

            String range = "Pricing!" + GetRangeFromXML(projectName, datatype);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            //ValueRange response = request.ExecuteAsync().Result;
            ValueRange response = request.Execute();
            return response.Values;

        }

    }
}
