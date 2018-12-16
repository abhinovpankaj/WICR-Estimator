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
            switch (projectName)
            {
                case "Dexotex Weather Wear":
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
                    return new SlopeViewModel(Js);
                case "Dexotex Barrier Gaurd":
                    return new DexoSlopeViewModel(Js);
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
                case "Dexotex Barrier Gaurd":
                    return new DexoMaterialViewModel(metalT,slopeT,Js);
                default:
                    return null;

            }
        }
    }
}
