using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Models
{
    public class GameClickModel { }
    public class GameClickList : INotifyPropertyChanged
    {
        private string _name;
        private int _today_count;
        private int _this_week_count;
        private int _last_week_count;
        private int _this_month_count;
        private int _last_month_count;
        private int _total_count;

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public int TodayCount { get { return _today_count; } set { _today_count = value; OnPropertyChanged("TodayCount"); } }
        public int ThisWeekCount { get { return _this_week_count; } set { _this_week_count = value; OnPropertyChanged("ThisWeekCount"); } }
        public int LastWeekCount { get { return _last_week_count; } set { _last_week_count = value; OnPropertyChanged("LastWeekCount"); } }
        public int ThisMonthCount { get { return _this_month_count; } set { _this_month_count = value; OnPropertyChanged("ThisMonthCount"); } }
        public int LastMonthCount { get { return _last_month_count; } set { _last_month_count = value; OnPropertyChanged("LastMonthCount"); } }
        public int TotalCount { get { return _total_count; } set { _total_count = value; OnPropertyChanged("TotalCount"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
