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
        private string _formula;
        public string Formula
        {
            get
            {
                return _formula;
            }
            set
            {
                if (value!=_formula)
                {
                    _formula = value;
                    IsCalculated = true;
                }
            }
        }
        public string Comment { get; set; }
        public bool IsCalculated { get; set; }
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
