using GameLauncher.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using MaterialDesignExtensions.Model;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Windows.Input;
using GenericCodes.CRUD.WPF.Core.MVVM;
using LiteDB;
using GenericCodes.CRUD.WPF.ViewModel.CRUDBases;

namespace GameLauncher.ViewModels
{
    public class SettingsViewModel  
    {
        public static ObservableCollection<GenreList> GenreListOC { get; set; }

        public SettingsViewModel()

        {
            m_autocompleteSource = new CabangAutoCompleteSource();

            m_selectedItem = null;
        }


        public class CabangAutoCompleteSource : IAutocompleteSource
        {
            private List<CabangList> m_cabangItems;
            string baseURL = "http://35.247.132.1:8080/Dashboard/GetAllCabang";

            public CabangAutoCompleteSource()
            {
                m_cabangItems = new List<CabangList>();
                using (WebClient client = new WebClient())
                {
                    //Now get your response from the client from get request to baseurl.
                    //Use the await keyword since the get request is asynchronous, and want it run before next asychronous operation.
                    string strJSON = client.DownloadString(baseURL);
                    var dataObj = JsonValue.Parse(strJSON);
                    if (dataObj != null)
                    {
                        //Parse your data into a object.
                        foreach (JsonObject cabang in dataObj)
                        {
                            CabangList newcabang = new CabangList();
                            newcabang.Name = cabang["name"];
                            m_cabangItems.Add(newcabang);

                        }
                        //Then create a new instance of PokeItem, and string interpolate your name property to your JSON object.
                        //Which will convert it to a string, since each property value is a instance of JToken.
                        //Log your pokeItem's name to the Console.
                    }
                    else
                    {
                        //If data is null log it into console.
                        return;
                    }



                }
            }

            public IEnumerable Search(string searchTerm)
            {
                searchTerm = searchTerm ?? string.Empty;
                searchTerm = searchTerm.ToLower();

                return m_cabangItems.Where(item => item.Name.ToLower().Contains(searchTerm));
            }
        }
        public void LoadGenres()
        {
            GenreListOC = MainWindow.GenreListMW;          
        }

        public List<CabangList> GetItems()
        {
            string baseURL = "http://35.247.132.1:8080/Dashboard/GetAllCabang";
            List<CabangList> Items = new List<CabangList>();
            //Have your api call in try/catch block.
            try
            {
                //Now we will have our using directives which would have a HttpClient 
                using (WebClient client = new WebClient())
                {
                    //Now get your response from the client from get request to baseurl.
                    //Use the await keyword since the get request is asynchronous, and want it run before next asychronous operation.
                    string strJSON = client.DownloadString(baseURL);
                    var dataObj = JsonValue.Parse(strJSON);
                    if (dataObj != null)
                    {
                        //Parse your data into a object.
                        foreach (JsonObject cabang in dataObj)
                        {
                            CabangList newcabang = new CabangList();
                            newcabang.Name = cabang["name"];
                            Items.Add(newcabang);

                        }
                        return Items;
                        //Then create a new instance of PokeItem, and string interpolate your name property to your JSON object.
                        //Which will convert it to a string, since each property value is a instance of JToken.
                        //Log your pokeItem's name to the Console.
                    }
                    else
                    {
                        //If data is null log it into console.
                        return Items;
                    }

                   

                }
                //Catch any exceptions and log it into the console.
            }
            catch (Exception exception)
            {
                return Items;
            }
        }

        private IAutocompleteSource m_autocompleteSource;


        public IAutocompleteSource AutocompleteSource
        {
            get
            {
                return m_autocompleteSource;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private CabangList m_selectedItem;
        bool firstGet = true;
        public CabangList SelectedItem
        {
            get
            {
                
                using (var db = new LiteDatabase("mineskigl.db"))
                {
                    var col = db.GetCollection("settings");                    
                                
                    if (col.Count() > 0 && firstGet == true)
                    {
                        var setting = col.FindById(1);
                        var cabang = setting["cabang"];
                        var tempSelected = new CabangList();
                        tempSelected.Name = cabang;
                        m_selectedItem = tempSelected;
                        firstGet = false;
                    }

                    return m_selectedItem;
                }

            }

            set
            {
                Console.WriteLine(value);
                this.m_selectedItem = value;
                OnPropertyChanged("SelectedItem");

            }
        }

        private RelayCommand _saveBranchCommand;

        public RelayCommand SaveBranchCommand
        {
            get
            {
                return _saveBranchCommand
                    ?? (_saveBranchCommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            
                            using (var db = new LiteDatabase("mineskigl.db"))
                            {
                                // Get collection instance
                                var col = db.GetCollection("settings");

                                if(col.Count() == 0)
                                {
                                    var setting = new BsonDocument();
                                    setting.Add("_id", 1);
                                    setting.Add("cabang", SelectedItem.Name);
                                    col.Insert(setting);
                                }
                                else
                                {
                                    var setting = col.FindById(1);
                                    setting["cabang"] = SelectedItem.Name;
                                    col.Update(setting);


                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message.ToString());
                        }
                    }));
            }

        }



    }
}