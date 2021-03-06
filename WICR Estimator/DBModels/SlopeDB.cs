﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    //[Serializable]
    public class SlopeDB:INotifyPropertyChanged
    {


        public int SlopeId { get; set; }
        public string SlopeName { get; set; }
        
        
        private double _laborRate;
        public double LaborRate
        {
            get
            {
                return _laborRate;
            }
            set
            {
                if (_laborRate != value)
                {
                    _laborRate = value;
                    OnPropertyChanged("LaborRate");
                }
            }
        }
        
        private double _perMixCost;
        public double PerMixCost
        {
            get
            {
                return _perMixCost;
            }
            set
            {
                if (_perMixCost != value)
                {
                    _perMixCost = value;
                    OnPropertyChanged("PerMixCost");
                }
            }
        }

        public string SlopeType { get; set; }
        public int ProjectId { get; set; }
        public bool IsDeleted { get; set; }
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                onSelectionChanged?.Invoke(value);
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<bool> onSelectionChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void HookCheckBoxAction(Action<bool> onProjectSelectionChanged)
        {
            this.onSelectionChanged = onProjectSelectionChanged;
        }
    }
}
