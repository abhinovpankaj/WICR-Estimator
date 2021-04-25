using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    
    public class Totals:NotifiableObject
    {
        public Totals()
        { }
        public event EventHandler OnTotalsChange;
        public string TabName { get; set; }
        private double laborextTotal;
        public double  LaborExtTotal
        { get { return laborextTotal; }
            set
            {
                if (value!=laborextTotal)
                {
                    laborextTotal = value;
                    RaisePropertyChanged("LaborExtTotal");
                    if (OnTotalsChange != null)
                    {
                        OnTotalsChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        private double matExtTotal;
        public double MaterialExtTotal
        {
            get { return matExtTotal; }
            set
            {
                if (value!=matExtTotal)
                {
                    matExtTotal = value;
                    RaisePropertyChanged("MaterialExtTotal");
                    if (OnTotalsChange != null)
                    {
                        OnTotalsChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        private double frTotal;
        public double MaterialFreightTotal
        {
            get { return frTotal; }
            set
            {
                if (value != frTotal)
                {
                    frTotal = value;
                    RaisePropertyChanged("MaterialFreightTotal");
                    if (OnTotalsChange != null)
                    {
                        OnTotalsChange(this, EventArgs.Empty);
                    }
                }
            }
        }
        private double scLabor;
        public double SubContractLabor
        {
            get { return scLabor; }
            set
            {
                if (value != scLabor)
                {
                    scLabor = value;
                    RaisePropertyChanged("SubContractLabor");
                    if (OnTotalsChange != null)
                    {
                        OnTotalsChange(this, EventArgs.Empty);
                    }
                }
            }
        }
       
    }
}
