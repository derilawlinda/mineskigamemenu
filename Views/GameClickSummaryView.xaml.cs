using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameLauncher.Models;
using GameLauncher.ViewModels;
using LiteDB;
using Npgsql;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for GameClickSummaryView.xaml
    /// </summary>
    public partial class GameClickSummaryView : UserControl
    {
        public GameClickSummaryView()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        public ObservableCollection<GameClickList> GridSource
        { get
            {
                var asd = this.GetGameClicks();
                return asd;
            }
        }

        public ObservableCollection<GameClickList> GetGameClicks()
        {
            ObservableCollection<GameClickList> gcmlist = new ObservableCollection<GameClickList>();
            string connstring = String.Format("Server={0};Port={1};" +
                        "User Id={2};Password={3};Database={4};",
                        "35.247.132.1", "5432", "postgres",
                        "mineski1234", "mineski");

            using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
            {
                conn.Open();
                using (var db = new LiteDatabase("mineskigl.db"))
                {
                    var col = db.GetCollection("settings");
                    var setting = col.FindById(1);
                    string branch = setting["cabang"];
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM count_aggregate WHERE branch = '"+branch+"'";
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                GameClickList gcm = new GameClickList();
                                gcm.Name = dr["name"].ToString();
                                gcm.TodayCount = dr["today_count"] != DBNull.Value ? Convert.ToInt32(dr["last_week_count"]) : 0;
                                gcm.ThisWeekCount = dr["this_week_count"] != DBNull.Value ? Convert.ToInt32(dr["this_week_count"]) : 0;
                                gcm.LastWeekCount = dr["last_week_count"] != DBNull.Value ? Convert.ToInt32(dr["last_week_count"]) : 0;
                                gcm.ThisMonthCount = dr["this_month_count"] != DBNull.Value ? Convert.ToInt32(dr["this_month_count"]) : 0;
                                gcm.LastMonthCount = dr["last_month_count"] != DBNull.Value ? Convert.ToInt32(dr["last_month_count"]) : 0;
                                gcm.TotalCount = dr["total_count"] != DBNull.Value ? Convert.ToInt32(dr["total_count"]) : 0;
                                gcmlist.Add(gcm);
                            }
                        }
                    }

                }
                conn.Close();

            }
            return gcmlist;
        }
    }
}
