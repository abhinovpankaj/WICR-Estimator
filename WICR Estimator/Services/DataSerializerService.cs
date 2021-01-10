using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.DBModels;

namespace WICR_Estimator.Services
{
    public class DataSerializerService
    {
        private readonly object serializeLock = new object();
        private static DataSerializerService dsInstance = null;
        private static object sync = new object();
        public DBData dbData;

        DataSerializerService()
        {
            dbData = new DBData();            
        }

        public static DataSerializerService DSInstance
        {
            get
            {
                lock (sync)
                {
                    if (dsInstance == null)
                    {
                        dsInstance = new DataSerializerService();
                    }
                    return dsInstance;
                }
            }
        }

        //public void serializeDbData(List<IDbData> gData, DataType DataType, string ProjectName)
        //{
        //    switch (DataType)
        //    {
        //        case DataType.Labor:
        //            dbData.LaborDBData = gData;
        //            break;
        //        case DataType.Material:
        //            dbData.MaterialDBData = gData;
        //            break;
        //        case DataType.Metal:
        //            dbData.MetalDBData = gData;
        //            break;
        //        case DataType.Slope:
        //            dbData.SlopeDBData = gData;
        //            break;
               
        //        case DataType.Freight:
        //            dbData.FreightDBData = gData;
        //            break;
        //        default:
        //            break;
        //    }
        //    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\"))
        //        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\");

        //    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_GoogleData.dat";
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

        //    formatter.Serialize(stream, dbData);
        //    stream.Close();
        //}

        public void serializeDbData(DBData gData, string ProjectName)
        {
            lock (serializeLock)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_DbData.dat";
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\");


                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;

                using (StreamWriter sw = new StreamWriter(path))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, gData);
                }
            }
        }
        public DBData deserializeDbData(string ProjectName)
        {
            lock (serializeLock)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_DbData.dat";
                IFormatter formatter = new BinaryFormatter();
                if (!File.Exists(path))
                {
                    return null;
                }
                try
                {
                    DBData dbData;
                    using (StreamReader file = File.OpenText(path))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        dbData = (DBData)serializer.Deserialize(file, typeof(DBData));
                    }
                    return dbData;
                }
                catch (FileNotFoundException ex)
                {
                    return null;
                }
            }
        }
        //public IEnumerable<IDbData> deserializeDbData(DataType dataType, string ProjectName)
        //{
        //    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WICR\\" + ProjectName + "_DbData.dat";
        //    if (!File.Exists(path))
        //    {
        //        return null;
        //    }
        //    IFormatter formatter = new BinaryFormatter();
        //    try
        //    {
        //        using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        //        {

        //            DBData objnew = (DBData)formatter.Deserialize(stream);
        //            switch (dataType)
        //            {
        //                case DataType.Labor:
        //                    return objnew.LaborDBData;

        //                case DataType.Material:
        //                    return objnew.MaterialDBData;

        //                case DataType.Metal:
        //                    return objnew.MetalDBData;

        //                case DataType.Slope:
        //                    return objnew.SlopeDBData;

        //                case DataType.Freight:
        //                    return objnew.FreightDBData;
        //                default:
        //                    return null;

        //            }
        //        }

        //    }
        //    catch (FileNotFoundException ex)
        //    {
        //        return null;
        //    }


        //}

    }
}
