using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator
{
    public static class ViewModelInstanceFactory
    {
        public static MetalBaseViewModel GetMetalViewModelInstance(string projectName)
        {
            switch (projectName)
            {
                case "Weather Wear":
                    return new MetalViewModel();
                case "Dexotex Resistite":
                    return new DexoMetalViewModel();
                default:
                    return null;
                    
            }
        }

        public static SlopeBaseViewModel GetSlopeViewModelInstance(string projectName)
        {
            switch (projectName)
            {
                case "Weather Wear":
                    return new SlopeViewModel();
                case "Dexotex Resistite":
                    return new DexoSlopeViewModel();
                default:
                    return null;

            }
        }
    }
}
