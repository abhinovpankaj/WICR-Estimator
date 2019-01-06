using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WICR_Estimator.GoogleUtility
{
    class SpreadSheetConnect
    {
        private static readonly object lockobj=new object();
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "WICR Estimator";
        private static UserCredential credential;
        private static SheetsService service ;
        private static string spreadsheetId = "1pQG-Z9vaaWhCjCUiG1XqmEj1Pjavoz86RCfG6-ews2k";

        public async static Task<IList<IList<Object>>> GetDataFromGoogleSheetsAsync(string projectName, DataType datatype)
        {
            if (credential==null)
            {
                using (var stream =
                new FileStream(@"GoogleUtility\credentials.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    //Console.WriteLine("Credential file saved to: " + credPath);
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

            
            ValueRange response = await request.ExecuteAsync();
            IList<IList<Object>> values = response.Values;
            
            return values;
            
        }
        private static XmlDocument doc;
        
        private static string GetRangeFromXML(string projectName,DataType datatype )
        {
            string prjName="";
            if (doc==null)
            {
                doc = new XmlDocument();
                doc.Load(@"GoogleUtility\ProjectGoogleRangeInfo.xml");
            }
            switch (projectName)
            {
                case "Dexotex Weather Wear":
                    prjName = "WeatherWear";
                    break;
                case "Dexotex Weather Wear Rehab":
                    prjName = "WeatherWearRehab";
                    break;
                case "Dexotex Barrier Gaurd":
                    prjName = "DexotexBarrier";
                    break;
                case "Enduro Kote Metal":
                    prjName = "EnduroKote";
                    break;
                case "Weather Wear Reseal":
                    prjName = "WeatherWearReseal";
                    break;
                case "Westcoat Color Wash":
                    prjName = "WestcoatColor";
                    break;
                case "Desert Brand":
                    prjName = "DesertBrand";
                    break;
                case "Pedestrian System":
                    prjName = "Pedestrian";
                    break;
                case "Parking Garage":
                    prjName = "Parking";
                    break;
                default:
                    break;
            }
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Projects/"+prjName+ "/"+datatype+"Range");
            return node.InnerText;
        }
        public static IList<IList<Object>> GetDataFromGoogleSheets(string projectName, DataType datatype)
        {
            UserCredential credential;
            //lock (lockobj)
            //{
            using (var stream =
            new FileStream(@"GoogleUtility\credentials.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }
            //}


            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1pQG-Z9vaaWhCjCUiG1XqmEj1Pjavoz86RCfG6-ews2k";
            String range =  "Pricing!" + GetRangeFromXML(projectName,datatype);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            
            IList<IList<Object>> values = response.Values;

            return values;

        }
    }
}
