﻿using System;
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
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }
            
            switch (projectName)
            {
                case "Weather Wear":
                case "Weather Wear Rehab":
                case "Color Wash Reseal":
                case "Endurokote":
                case "Desert Crete":
                case "Pedestrian System":
                case "Parking Garage":
                case "Barrier Guard":
                case "Resistite":
                case "MACoat":
                case "ALX":
                case "Multicoat":
                case "Pli-Dek":
                case "Tufflex":
                //case "Paraseal":
                case "Paraseal LG":
                case "Paraseal GM":
                case "201":
                case "250 GC":
                case "Dexcellent II":
                //case "860":
                case "Westcoat BT":
                case "UPI BT":
                case "Dual Flex":
                case "Westcoat Epoxy":
                case "Polyurethane Injection Block":
                case "Xypex":
                case "Reseal all systems":
                    return new MetalViewModel(Js);
                case "Blank":
                    return new ZeroMetalViewModel(Js);
                
                default:
                    return null;
                    
            }
        }

        public static SlopeBaseViewModel GetSlopeViewModelInstance(string projectName,Models.JobSetup Js)
        {
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }

            switch (projectName)
            {
                case "Weather Wear":
                case "Weather Wear Rehab":
                case "Color Wash Reseal":
                case "Desert Crete":
                case "Resistite":
                case "MACoat":
                case "ALX":
                case "Multicoat":
                case "Pli-Dek":
                case "Dexcellent II":
                case "Westcoat BT":
                case "Reseal all systems":
                case "Blank":
                    return new SlopeViewModel(Js);
                case "Endurokote":
                    return new EnduroKoteSlopeViewModel(Js);
                case "Barrier Guard":
                    return new DexoSlopeViewModel(Js);
                case "Pedestrian System":
                case "Parking Garage":
                case "Tufflex":
                case "201":
                case "250 GC":
                case "860 Carlisle":
                case "UPI BT":
                
                    return new PedestrianSlopeViewModel(Js);
                case "Dual Flex":
                    return new DualFlexSlopeViewModel(Js);
                default:
                    return null;

            }
        }
        public static MaterialBaseViewModel GetMaterialViewModelInstance(string projectName,Totals metalT,Totals slopeT, Models.JobSetup Js)
        {
            if (projectName.Contains('.'))
            {
                projectName = projectName.Split('.')[0];
            }

            switch (projectName)
            {
                case "Weather Wear":
                    return new MaterialViewModel(metalT,slopeT,Js);
                case "Weather Wear Rehab":
                    return new WWRehabMaterialViewModel(metalT, slopeT, Js);
                case "Barrier Guard":
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
                case "ALX":
                    return new ALXMaterialViewModel(metalT, slopeT, Js);
                case "Multicoat":
                    return new MulticoatMaterialViewModel(metalT, slopeT, Js);
                case "Pli-Dek":
                    return new PlideckMaterialViewModel(metalT, slopeT, Js);
                case "Tufflex":
                    return new TufflexMaterialViewModel(metalT, slopeT, Js);
                case "Paraseal":
                    return new ParasealMaterialViewModel(metalT, slopeT, Js);
                case "Paraseal LG":
                    return new ParasealLGMaterialViewModel(metalT, slopeT, Js);
                case "Paraseal GM":
                    return new ParasealGMMaterialViewModel(metalT, slopeT, Js);
                case "201":
                    return new _201MaterialViewModel(metalT, slopeT, Js);
                case "250 GC":
                    return new _250MaterialViewModel(metalT, slopeT, Js);
                case "Dexcellent II":
                    return new DexellentIIMaterialViewModel(metalT, slopeT, Js);
                case "860 Carlisle":
                    return new Carlisle860MaterialViewModel(metalT, slopeT, Js);
                case "Westcoat BT":
                    return new WestcoatdualMaterialViewModel(metalT, slopeT, Js);
                case "Dual Flex":
                    return new DualFlexMaterialViewModel(metalT, slopeT, Js);
                case "UPI BT":
                    return new UPIBelowTileMaterialViewModel(metalT, slopeT, Js);
                case "Westcoat Epoxy":
                    return new DexoColorFlakeMaterialViewModel(metalT, null, Js);
                case "Polyurethane Injection Block":
                    return new DeneefMaterialViewModel(metalT, null, Js);
                case "Xypex":
                    return new XypexMaterialViewModel(metalT, null, Js);
                case "Blank":
                    return new IndependentMaterialViewModel(metalT, slopeT, Js);
                default:
                    return null;
            }
        }
    }
}
