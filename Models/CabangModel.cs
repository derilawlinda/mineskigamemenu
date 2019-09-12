using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Models
{
    public class CabangModel { }

    public class CabangList : INotifyPropertyChanged
    {
        private string _name;
       
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

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
