using GalaSoft.MvvmLight;
using GameLauncher.Models;
using LiteDB;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GameLauncher.ViewModels
{
    public class GameClickSummaryViewModel : ViewModelBase
    {
        private ObservableCollection<GameClickList> _gridSource;
        public ObservableCollection<GameClickList> GridSource;

            
       

        

    }
}
