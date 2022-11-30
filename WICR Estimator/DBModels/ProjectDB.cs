using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{

    
    public class ProjectDB : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string MainGroupName { get; set; }
        public int Rank { get; set; }
        public int ProjectId { get; set; }
        public bool IsDeleted { get; set; }

        private bool _isChecked;
        public bool IsSelected
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                onSelectionChanged?.Invoke(value);
                OnPropertyChanged("IsSelected");
            }
        }
        private Action<bool> onSelectionChanged;

        internal void HookCheckBoxAction(Action<bool> onProjectSelectionChanged)
        {
            this.onSelectionChanged = onProjectSelectionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
