using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;
using WICR_Estimator.ViewModels;

namespace WICR_Estimator
{
    public static class ViewModelInstanceFactory
    {
        public static MetalBaseViewModel GetMetalViewModelInstance(string projectName,Models.JobSetup Js)
        {
            switch (projectName)
            {
                case "Dexotex Weather Wear":
                case "Dexotex Weather Wear Rehab":
                case "Westcoat Color Wash":
                case "Enduro Kote Metal":
                case "Desert Brand":
                case "Pedestrian System":
                    return new MetalViewModel(Js);
                case "Dexotex Barrier Gaurd":
                    return new DexoMetalViewModel(Js);
                
                default:
                    return null;
                    
            }
        }

        public static SlopeBaseViewModel GetSlopeViewModelInstance(string projectName,Models.JobSetup Js)
        {
            switch (projectName)
            {
                case "Dexotex Weather Wear":
                case "Dexotex Weather Wear Rehab":
                case "Westcoat Color Wash":
                case "Desert Brand":
                    return new SlopeViewModel(Js);
                case "Enduro Kote Metal":
                    return new EnduroKoteSlopeViewModel(Js);
                case "Dexotex Barrier Gaurd":
                    return new DexoSlopeViewModel(Js);
                case "Pedestrian System":
                    return new PedestrianSlopeViewModel(Js);
                default:
                    return null;

            }
        }

        public static MaterialBaseViewModel GetMaterialViewModelInstance(string projectName,Totals metalT,Totals slopeT, Models.JobSetup Js)
        {
            switch (projectName)
            {
                case "Dexotex Weather Wear":
                    return new MaterialViewModel(metalT,slopeT,Js);
                case "Dexotex Weather Wear Rehab":
                    return new WWRehabMaterialViewModel(metalT, slopeT, Js);
                case "Dexotex Barrier Gaurd":
                    return new DexoMaterialViewModel(metalT,slopeT,Js);
                case "Enduro Kote Metal":
                    return new EnduroKoteMaterialViewModel(metalT, slopeT, Js);
                case "Weather Wear Reseal":
                    return new WWResealMaterialViewModel(metalT, slopeT, Js);
                case "Westcoat Color Wash":
                    return new WestcoatColorMaterialViewModel(metalT, slopeT, Js);
                case "Desert Brand":
                    return new DesertbrandMaterialViewModel(metalT, slopeT, Js);
                case "Pedestrian System":
                    return new PedestrianMaterialViewModel(metalT, slopeT, Js);
                default:
                    return null;

            }
        }
    }
}
