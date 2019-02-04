using System;
using System.Collections.Generic;

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
}