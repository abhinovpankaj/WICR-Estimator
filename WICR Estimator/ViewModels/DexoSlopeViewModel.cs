﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WICR_Estimator.Models;

namespace WICR_Estimator.ViewModels
{

    public class DexoSlopeViewModel:SlopeBaseViewModel
    {
        public DexoSlopeViewModel(JobSetup  Js):base(Js.dbData)
        {
            //GetSlopeDetailsFromGoogle(Js.ProjectName);
            GetSlopeDetailsDB(Js.ProjectName);
            //Slopes = CreateSlopes();
            Slopes = CreateSlopesDB("Cement");
            CalculateAll();
            Js.JobSetupChange += JobSetup_OnJobSetupChange;
        }
        public DexoSlopeViewModel()
        { }
    }
}
