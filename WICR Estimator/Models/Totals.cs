using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator.Models
{
    public class Totals:BaseViewModel
    {
        //public event EventHandler OnTotalsChange;
        private double matExtTotal;
        public double MaterialExtTotal
        {
            get { return matExtTotal; }
            set
            {
                if (value!=matExtTotal)
                {
                    matExtTotal = value;
                    OnPropertyChanged("MaterialExtTotal");
                    //if (OnTotalsChange!=null)
                    //{
                    //    OnTotalsChange(this, EventArgs.Empty);
                    //}
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
                    OnPropertyChanged("MaterialFreightTotal");
                    //if (OnTotalsChange != null)
                    //{
                    //    OnTotalsChange(this, EventArgs.Empty);
                    //}
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
                    OnPropertyChanged("SubContractLabor");
                    //if (OnTotalsChange != null)
                    //{
                    //    OnTotalsChange(this, EventArgs.Empty);
                    //}
                }
            }
        }
       
    }
}
