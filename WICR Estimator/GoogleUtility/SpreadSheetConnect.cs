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

namespace WICR_Estimator.GoogleUtility
{
    class SpreadSheetConnect
    {
        private static readonly object lockobj=new object();
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "WICR Estimator";

        public async static Task<IList<IList<Object>>> GetDataFromGoogleSheetsAsync(string sheetName,string dataRange )
        {
            UserCredential credential;
            //lock (lockobj)
            //{
                using (var stream =
                new FileStream(@"GoogleUtility\credentials.json", FileMode.Open, FileAccess.Read,FileShare.ReadWrite))
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
            String range = sheetName + "!" + dataRange;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            
            ValueRange response = await request.ExecuteAsync();
            IList<IList<Object>> values = response.Values;
            
            return values;
            
        }

        public static IList<IList<Object>> GetDataFromGoogleSheets(string sheetName, string dataRange)
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
            String range = sheetName + "!" + dataRange;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            
            IList<IList<Object>> values = response.Values;

            return values;

        }
    }
}
