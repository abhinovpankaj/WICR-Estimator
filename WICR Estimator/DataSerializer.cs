using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator
{
    public class DataSerializer
    {
        private readonly object serializeLock = new object();
        private static DataSerializer dsInstance = null;
        private static object sync = new object();
        public  GSData googleData;
        DataSerializer()
        {
            googleData = new GSData();
            //googleData = deserializeGoogleData();
        }

        public static DataSerializer DSInstance
        {
            get
            {
                lock(sync)
                {
                    if (dsInstance == null)
                    {
                        dsInstance = new DataSerializer();
                    }
                    return dsInstance;
                }
            }
        }

        public void serializeGoogleData(IList<IList<object>> gData,DataType DataType,string ProjectName)
        {
            switch (DataType)
            {
                case DataType.Labor:
                    googleData.LaborData = gData;
                    break;
                case DataType.Material:
                    googleData.MaterialData = gData;
                    break;
                case DataType.Metal:
                    googleData.MetalData = gData;
                    break;
                case DataType.Slope:
                    googleData.SlopeData = gData;
                    break;
                case DataType.Rate:
                    googleData.LaborRate = gData;
                    break;
                case DataType.Freight:
                    googleData.FreightData = gData;
                    break;
                default:
                    break;
            }
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\");

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName+"_GoogleData.dat";
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, googleData);
            stream.Close();
        }

        public void serializeGoogleData(GSData gData,string ProjectName)
        {
            lock (serializeLock)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_GoogleData.dat";
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\");
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);


                formatter.Serialize(stream, gData);
                stream.Close();
            }
            
        }
        public GSData deserializeGoogleData(string ProjectName)
        {
            lock (serializeLock)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_GoogleData.dat";
                IFormatter formatter = new BinaryFormatter();
                try
                {
                    Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    GSData objnew = (GSData)formatter.Deserialize(stream);
                    return objnew;
                }
                catch (FileNotFoundException ex)
                {
                    return null;
                }
            }
        }
        public IList<IList<object>> deserializeGoogleData(DataType dataType,string ProjectName)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_GoogleData.dat";
            if (!File.Exists(path))
            {
                return null;
            }
            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    
                    GSData objnew = (GSData)formatter.Deserialize(stream);
                    switch (dataType)
                    {
                        case DataType.Labor:
                            return objnew.LaborData;

                        case DataType.Material:
                            return objnew.MaterialData;

                        case DataType.Metal:
                            return objnew.MetalData;

                        case DataType.Slope:
                            return objnew.SlopeData;

                        case DataType.Rate:
                            return objnew.LaborRate;
                        case DataType.Freight:
                            return objnew.FreightData;
                        default:
                            return null;

                    } 
                }
                
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
            
           
        }

    }
}
