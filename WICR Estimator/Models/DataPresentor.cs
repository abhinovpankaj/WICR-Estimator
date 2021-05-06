using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.Models
{
    public class DataPresentor: UndoRedoObservableObject
    {
        public string Key { get; set; }
        public string Formula { get; set; }
        public string Comment { get; set; }

        private double calculatedValue;

        public double CalculatedValue
        {
            get { return calculatedValue; }
            set
            {
                calculatedValue = value;
                RaisePropertyChanged("CalculatedValue");
            }
        }

    }
}
