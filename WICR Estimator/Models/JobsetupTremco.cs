using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.Models
{
    public class JobsetupTremco:JobSetup
    {
        
        public JobsetupTremco(string ProjectName): base(ProjectName)
        {

        }

        #region Paraseal LG
        public System.Windows.Visibility IsParasealLGSectionVisible
        {
            get
            {

                if (ProjectName == "Paraseal LG")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
        private double rakerCornerBases;
        public double RakerCornerBases
        {
            get { return rakerCornerBases; }
            set
            {
                if (value != rakerCornerBases)
                {
                    rakerCornerBases = value;
                    OnPropertyChanged("RakerCornerBases");
                    base.OnJobSetupChanged(null);
                }
            }
        }
        private double cementBoardDetail;
        public double CementBoardDetail
        {
            get { return cementBoardDetail; }
            set
            {
                if (value != cementBoardDetail)
                {
                    cementBoardDetail = value;
                    base.OnJobSetupChanged(null);
                }
            }
        }
        private double rockPockets;
        public double RockPockets
        {
            get { return rockPockets; }
            set
            {
                if (value != rockPockets)
                {
                    rockPockets = value;
                    OnPropertyChanged("RockPockets");
                    base.OnJobSetupChanged(null);
                }
            }
        }
        private double parasealFoundation;
        public double ParasealFoundation
        {
            get { return parasealFoundation; }
            set
            {
                if (value != parasealFoundation)
                {
                    parasealFoundation = value;
                    base.OnJobSetupChanged(null);
                }
            }
        }
        private double rearMidLagging;
        public double RearMidLagging
        {
            get { return rearMidLagging; }
            set
            {
                if (value != rearMidLagging)
                {
                    rearMidLagging = value;
                    OnPropertyChanged("RearMidLagging");
                    base.OnJobSetupChanged(null);
                }
            }
        }
        #endregion
    }
}
