using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WICR_Estimator.ViewModels;
using System.Collections.Generic;
using WICR_Estimator;
using System.Threading.Tasks;

namespace WICR_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            IList<IList<object>> LaborRate = await WICR_Estimator.GoogleUtility.SpreadSheetConnect.GetDataFromGoogleSheetsAsync("Weather Wear", DataType.Rate);
        }
    }
}
