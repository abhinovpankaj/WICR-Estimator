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
                case "Weather Wear":
                case "Weather Wear Rehab":
                case "Color Wash Reseal":
                case "Endurokote":
                case "Desert Crete":
                case "Pedestrian System":
                case "Parking Garage":
                case "Barrier Gaurd":
                case "Resistite":
                case "MACoat":
                    return new MetalViewModel(Js);
                //case "Dexotex Barrier Gaurd":
                //    return new DexoMetalViewModel(Js);
                
                default:
                    return null;
                    
            }
        }

        public static SlopeBaseViewModel GetSlopeViewModelInstance(string projectName,Models.JobSetup Js)
        {
            switch (projectName)
            {
                case "Weather Wear":
                case "Weather Wear Rehab":
                case "Color Wash Reseal":
                case "Desert Crete":
                case "Resistite":
                case "MACoat":
                    return new SlopeViewModel(Js);
                case "Endurokote":
                    return new EnduroKoteSlopeViewModel(Js);
                case "Barrier Gaurd":
                    return new DexoSlopeViewModel(Js);
                case "Pedestrian System":
                case "Parking Garage":
                    return new PedestrianSlopeViewModel(Js);
                default:
                    return null;

            }
        }
        public static MaterialBaseViewModel GetMaterialViewModelInstance(string projectName,Totals metalT,Totals slopeT, Models.JobSetup Js)
        {
            switch (projectName)
            {
                case "Weather Wear":
                    return new MaterialViewModel(metalT,slopeT,Js);
                case "Weather Wear Rehab":
                    return new WWRehabMaterialViewModel(metalT, slopeT, Js);
                case "Barrier Gaurd":
                    return new DexoMaterialViewModel(metalT,slopeT,Js);
                case "Endurokote":
                    return new EnduroKoteMaterialViewModel(metalT, slopeT, Js);
                case "Reseal all systems":
                    return new WWResealMaterialViewModel(metalT, slopeT, Js);
                case "Color Wash Reseal":
                    return new WestcoatColorMaterialViewModel(metalT, slopeT, Js);
                case "Desert Crete":
                    return new DesertbrandMaterialViewModel(metalT, slopeT, Js);
                case "Pedestrian System":
                    return new PedestrianMaterialViewModel(metalT, slopeT, Js);
                case "Parking Garage":
                    return new ParkingMaterialViewModel(metalT, slopeT, Js);
                case "Resistite":
                    return new ResistiteConcreteMaterialViewModel(metalT, slopeT, Js);
                case "MACoat":
                    return new MACoatMaterialViewModel(metalT, slopeT, Js);
                default:
                    return null;
            }
        }
    }
}
