using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WICR_Estimator.DBModels;
using Excel = Microsoft.Office.Interop.Excel;

namespace WICR_Estimator.Services
{
    public class ExcelService
    {
        static Microsoft.Office.Interop.Excel.Application  exlApp;
        static Microsoft.Office.Interop.Excel.Workbook  exlWb;
        

        public static async Task UpdatePrices()
        {
              
            try
            {
                foreach (Excel.Worksheet sht in exlWb.Sheets)
                {
                    switch (sht.Name)
                    {
                        case "slopes":
                            List<SlopeDB> updatedSlopes = new List<SlopeDB>();
                            for (int i = 2; i <= 273; i++)
                            {
                                if (sht.Cells[i,8].Value=="Yes")
                                {
                                    SlopeDB slp = new SlopeDB();
                                    slp.SlopeId = (int)sht.Cells[i,1].Value;
                                    slp.ProjectId = (int)sht.Cells[i, 6].Value;
                                    slp.PerMixCost = (double)sht.Cells[i, 4].Value;
                                    slp.SlopeName = sht.Cells[i, 2].Value;
                                    slp.SlopeType = sht.Cells[i, 5].Value;
                                    slp.LaborRate = (double)sht.Cells[i, 3].Value;
                                    updatedSlopes.Add(slp);
                                    sht.Cells[i, 8].Value = "";
                                }
                            }
                             var result=await HTTPHelper.PutSlopesAsync(updatedSlopes);
                            
                                break;
                        case "metals":
                            //List<MetalDB> updatedMetals = new List<MetalDB>();
                            for (int i = 2; i <= 247; i++)
                            {
                                if (sht.Cells[i, 9].Value == "Yes")
                                {
                                    MetalDB metal = new MetalDB();
                                    metal.MetalId = (int)sht.Cells[i, 1].Value;
                                    metal.ProjectId = (int)sht.Cells[i, 8].Value;
                                    metal.MetalName = sht.Cells[i, 2].Value;
                                    metal.MetalPrice = (double)sht.Cells[i, 4].Value;
                                    metal.MetalType = sht.Cells[i, 6].Value;
                                    metal.ProductionRate = (double)sht.Cells[i, 5].Value;
                                    metal.Vendor = sht.Cells[i, 7].Value;
                                    var cellValue = sht.Cells[i, 3].Value;
                                    if (sht.Cells[i, 3].Value != null)
                                    {
                                        metal.Units = (int)sht.Cells[i, 3].Value;
                                    }

                                    await HTTPHelper.PutMetalAsync(metal.MetalId,metal);
                                    sht.Cells[i, 9].Value = "";
                                }
                            }
                            
                            break;
                        case "materials":
                            List<MaterialDB> materials = new List<MaterialDB>();
                            for (int i = 2; i < 492; i++)
                            {
                                if (sht.Cells[i, 12].Value == "Yes")
                                {
                                    MaterialDB mat = new MaterialDB();
                                    mat.MaterialId = (int)sht.Cells[i, 2].Value;
                                    mat.ProjectId = (int)sht.Cells[i, 1].Value;
                                    mat.MaterialPrice = (double)sht.Cells[i, 5].Value;
                                    mat.Coverage = (double)sht.Cells[i, 4].Value;
                                    mat.Weight =(double) sht.Cells[i, 6].Value;
                                    mat.ProdRateHorizontal = (double)sht.Cells[i, 7].Value;
                                    mat.ProdRateStair = (double)sht.Cells[i, 9].Value;
                                    mat.ProdRateVertical = (double)sht.Cells[i, 8].Value;
                                    mat.LaborMinCharge= (double)sht.Cells[i, 10].Value;
                                    mat.MaterialName = sht.Cells[i, 3].Value;
                                    materials.Add(mat);
                                    sht.Cells[i, 12].Value = "";
                                }
                            }
                            var mats = await HTTPHelper.PutMaterialsAsync(materials);

                            break;
                        case "labors":
                            List<LaborFactorDB> labors = new List<LaborFactorDB>();
                            for (int i = 2; i <= 595; i++)
                            {
                                if (sht.Cells[i, 6].Value == "Yes")
                                {
                                    LaborFactorDB slp = new LaborFactorDB();
                                    slp.LaborId = (int)sht.Cells[i, 4].Value;
                                    slp.ProjectId = (int)sht.Cells[i, 3].Value;
                                    slp.Name = sht.Cells[i, 1].Value;
                                    slp.Value = (double)sht.Cells[i, 2].Value;

                                    labors.Add(slp);
                                    sht.Cells[i, 6].Value = "";
                                }
                            }
                            var lbr = await HTTPHelper.PutLaborFactorsAsync(labors);

                            break;
                        case "freights":
                            List<FreightDB> freights = new List<FreightDB>();
                            for (int i = 2; i <= 9; i++)
                            {
                                if (sht.Cells[i, 4].Value == "Yes")
                                {
                                    FreightDB slp = new FreightDB();
                                    slp.FreightID = (int)sht.Cells[i, 3].Value;
                                    
                                    slp.FactorName = sht.Cells[i, 1].Value;
                                    slp.FactorValue = (double)sht.Cells[i, 2].Value;

                                    freights.Add(slp);
                                    sht.Cells[i, 4].Value = "";
                                }
                            }
                            var frg = await HTTPHelper.PutFreightsAsync(freights);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                exlWb.Save();
                exlWb.Close(true);
                exlWb = null;
                exlApp.EnableEvents = true;
                
                exlApp.DisplayAlerts = true;
                exlApp.Quit();
                exlApp = null;
            }
        }
        static string filePath;
        public static void BrowseFile()
        {
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Browse Estimate File",
                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "xlsm",
                Filter = "Estimator files (*.xlsm)|*.xlsm",
                FilterIndex = 2,
                RestoreDirectory = true,

                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
        }
        public  static async Task ReadPriceExcel()
        {

            if (filePath!="")
            {
                exlApp = new Microsoft.Office.Interop.Excel.Application();
                exlApp.EnableEvents = false;
                exlApp.Visible = false;
                exlApp.DisplayAlerts = false;
                exlWb = exlApp.Workbooks.Open(filePath);


               await UpdatePrices();
            }
            

        }
    }
}
