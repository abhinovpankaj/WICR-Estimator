using System;
using System.Collections.Generic;
using WICR_Estimator.DBModels;

namespace WICR_Estimator
{
    [Serializable]
    public class GSData
    {
        public IList<IList<object>> SlopeData { get; set; }
        public IList<IList<object>> MetalData { get; set; }
        public IList<IList<object>> MaterialData { get; set; }
        public IList<IList<object>> LaborRate { get; set; }
        public IList<IList<object>> LaborData { get; set; }
        public IList<IList<object>> FreightData { get; set; }
        public GSData()
        { }
    }
    
    public enum DataType
    {
        Slope,
        Metal,
        Material,
        Labor,
        Rate,
        Freight
    }

    [Serializable]
    public class DBData
    {
        public IList<SlopeDB> SlopeDBData { get; set; }
        public IList<MetalDB> MetalDBData { get; set; }
        public IList<MaterialDB> MaterialDBData { get; set; }

        public IList<LaborFactorDB> LaborDBData { get; set; }
        public IList<FreightDB> FreightDBData { get; set; }
        public DBData()
        { }
    }
}