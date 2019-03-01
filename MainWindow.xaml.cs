﻿using GameLauncher.ViewModels;
using GameLauncher.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GameLauncher.Properties;
using MaterialDesignThemes.Wpf;
using System.Windows.Data;
using System.Net;
using System.Diagnostics;
using System.Windows.Threading;

namespace GameLauncher
{
    public partial class MainWindow : Window
    {
        public bool isDownloadOpen = false;
        public bool isExeSearchOpen = false;
        public static AddGames DialogAddGames = new AddGames();
        public static EditGames DialogEditGames = new EditGames();
        public static ExeSelection DialogExeSelection = new ExeSelection();
        private BannerViewModel bannerViewModel;
        private ListViewModel listViewModel;
        private PosterViewModel posterViewModel;
        private SettingsViewModel settingsViewModel;
        private ExesViewModel exesViewModel;
        public Views.PosterView pv = new Views.PosterView();
        public Views.BannerView bv = new Views.BannerView();
        public Views.ListView lv = new Views.ListView();
        public CollectionViewSource cvs;
        public bool isDialogOpen;
        public string dialogOpen;
        public string DLGameTitle;
        public string DLImgType;

        public MainWindow()
        {
            Trace.Listeners.Clear();
            InitTraceListen();
            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.75);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.75);
            LoadAllGames lag = new LoadAllGames();
            LoadSearch ls = new LoadSearch();
            MakeDirectories();
            MakeDefaultGenres();
            lag.LoadGenres();
            InitializeComponent();
            bannerViewModel = new BannerViewModel();
            posterViewModel = new PosterViewModel();
            listViewModel = new ListViewModel();
            exesViewModel = new ExesViewModel();
            posterViewModel.LoadGames();
            posterViewModel.LoadGenres();
            DataContext = posterViewModel;
            isDownloadOpen = false;
            LoadSettings();
            Trace.WriteLine(DateTime.Now + ": New Session started");

        }
        public void MakeDirectories()
        {
            if (!Directory.Exists("./Resources/")) { Directory.CreateDirectory("./Resources/"); }
            if (!Directory.Exists("./Resources/img/")) { Directory.CreateDirectory("./Resources/img/"); }
            if (!Directory.Exists("./Resources/shortcuts/")) { Directory.CreateDirectory("./Resources/shortcuts/"); }
        }

        public void MakeDefaultGenres()
        {
            if (!File.Exists("./Resources/GenreList.txt"))
            {
                TextWriter tsw = new StreamWriter(@"./Resources/GenreList.txt", true);
                Guid gameGuid = Guid.NewGuid();
                tsw.WriteLine("Action|" + Guid.NewGuid());
                tsw.WriteLine("Adventure|" + Guid.NewGuid());
                tsw.WriteLine("Casual|" + Guid.NewGuid());
                tsw.WriteLine("Emulator|" + Guid.NewGuid());
                tsw.WriteLine("Horror|" + Guid.NewGuid());
                tsw.WriteLine("Indie|" + Guid.NewGuid());
                tsw.WriteLine("MMO|" + Guid.NewGuid());
                tsw.WriteLine("Open World|" + Guid.NewGuid());
                tsw.WriteLine("Platform|" + Guid.NewGuid());
                tsw.WriteLine("Racing|" + Guid.NewGuid());
                tsw.WriteLine("Retro|" + Guid.NewGuid());
                tsw.WriteLine("RPG|" + Guid.NewGuid());
                tsw.WriteLine("Simulation|" + Guid.NewGuid());
                tsw.WriteLine("Sport|" + Guid.NewGuid());
                tsw.WriteLine("Strategy|" + Guid.NewGuid());
                tsw.WriteLine("VR|" + Guid.NewGuid());
                tsw.Close();
            }
        }

        public void InitTraceListen()
        {
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(appdir + "\\log")) { Directory.CreateDirectory(appdir + "\\log"); }
            if (File.Exists(appdir + "\\log\\event.log")) {
                File.Delete(appdir + "\\log\\event.log");
                File.Create(appdir + "\\log\\event.log"); }
            else { File.Create(appdir + "\\log\\event.log"); }
            TextWriterTraceListener twtl = new TextWriterTraceListener("C:\\Windows\\Temp\\Breeze\\log.log");
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;
            ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;
        }


        private void OpenAddGameWindow_OnClick(object sender, RoutedEventArgs e)
        {
            OpenAddGameDialog();
            RefreshGames();
        }

        public void OpenAddGameDialog()
        {
            DialogFrame.Visibility = Visibility.Visible;
            DialogFrame.Content = DialogAddGames;
            dialogOpen = "add";
            isDialogOpen = true;
            DialogAddGames.AddGameDialog.IsOpen = true;
        }
        public void UpdateObsCol(string title, string exe)
        {
            exesViewModel.UpdateObsCol(title, exe);
        }
        public bool CheckBinding(string title)
        {
            bool result = exesViewModel.CheckBinding(title);
            if (result == true) { return true; }
            else { return false; }
        }
        public void OpenExeSearchDialog()
        {
            DataContext = exesViewModel;
            exesViewModel.SearchExe();
            DialogFrame.Visibility = Visibility.Visible;
            DialogFrame.Content = DialogExeSelection;
            isDialogOpen = true;
            dialogOpen = "exeSelection";
            DialogExeSelection.ExeSelectionDialog.IsOpen = true;
            isExeSearchOpen = true;
        }

        public void CloseExeSearchDialog()
        {
            DataContext = settingsViewModel;
            DialogFrame.Visibility = Visibility.Hidden;
            isDialogOpen = false;
            DialogExeSelection.ExeSelectionDialog.IsOpen = false;
            isExeSearchOpen = false;
        }

        public void OpenEditGameDialog(string guid)
        {
            DialogFrame.Visibility = Visibility.Visible;
            DialogFrame.Content = DialogEditGames;
            dialogOpen = "edit";
            isDialogOpen = true;
            DialogEditGames.currentGuid(guid);
            DialogEditGames.EditGameDialog.IsOpen = true;
        }
        public void OpenImageDL(string gametitle, string searchstring, string imagetype)
        {
            ImageDownload DialogImageDL = new ImageDownload(gametitle, searchstring, imagetype);
            DLGameTitle = gametitle;
            DLImgType = imagetype;
            if (DialogFrame.Content.ToString() == "GameLauncher.EditGames" || DialogFrame.Content.ToString() == "GameLauncher.AddGames") {
            DialogFrame.Visibility = Visibility.Visible;
            DialogFrame.Content = DialogImageDL;
            DialogAddGames.AddGameDialog.IsOpen = false;
            DialogEditGames.EditGameDialog.IsOpen = false;
            DialogImageDL.DownloadDialog.IsOpen = true;
            isDownloadOpen = true;
            }
            else if (DialogFrame.Content.ToString() == "GameLauncher.Views.ImageDownload")
            {
                if (dialogOpen == "edit")
                {
                    DialogFrame.Content = DialogEditGames;
                    DialogEditGames.EditGameDialog.IsOpen = true;
                    DialogImageDL.DownloadDialog.IsOpen = false;
                    isDownloadOpen = false;
                }
                else if (dialogOpen == "add")
                {
                    DialogFrame.Content = DialogAddGames;
                    DialogAddGames.AddGameDialog.IsOpen = true;
                    DialogImageDL.DownloadDialog.IsOpen = false;
                    isDownloadOpen = false;
                }
                else { Trace.WriteLine(DateTime.Now + ": -System unsure which dialog currently open"); }
            }
        }

        public void DownloadImage(string url)
        {
            if (!File.Exists(@"Resources/img/" + DLGameTitle + "-" + DLImgType + ".png")){
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        client.UseDefaultCredentials = true;
                        client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        client.DownloadFile(new Uri(url), @"Resources\img\" + DLGameTitle + "-" + DLImgType + ".png");
                        SetPath(DLGameTitle, DLImgType, dialogOpen);
                    }catch(Exception e) { Trace.WriteLine(DateTime.Now + ": DownloadImage:" + e); MessageBox.Show("Sorry! That's failed, Try again or try another image"); }
                } }
            else if (File.Exists(@"Resources/img/" + DLGameTitle + "-" + DLImgType + ".png")){
                File.Delete(@"Resources/img/" + DLGameTitle + "-" + DLImgType + ".png");
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        client.UseDefaultCredentials = true;
                        client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        client.DownloadFile(new Uri(url), @"Resources\img\" + DLGameTitle + "-" + DLImgType + ".png");
                        SetPath(DLGameTitle, DLImgType, dialogOpen);
                    }
                    catch (Exception e) { Trace.WriteLine(DateTime.Now + ": DownloadImage2: " + e); }
                }
            }
        }

        public void SetPath(string title, string imagetype, string dialogType)
        {
            string imgpath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\img\\" + DLGameTitle + "-" + DLImgType + ".png";
            if (imagetype == "icon")
            {
                if (dialogType == "edit")
                {
                    DialogEditGames.EditIcon.Text = imgpath;
                    OpenImageDL("","","");
                }
                else if (dialogType == "add")
                {
                    DialogAddGames.NewGameIcon.Text = imgpath;
                    OpenImageDL("","","");
                }
            }
            else if (imagetype == "poster")
            {
                if (dialogType == "edit")
                {
                    DialogEditGames.EditPoster.Text = imgpath;
                    OpenImageDL("","","");
                }
                else if (dialogType == "add")
                {
                    DialogAddGames.NewGamePoster.Text = imgpath;
                    OpenImageDL("","","");
                }
            }
            else if (imagetype == "banner")
            {
                if (dialogType == "edit")
                {
                    DialogEditGames.EditBanner.Text = imgpath;
                    OpenImageDL("","","");
                }
                else if (dialogType == "add")
                {
                    DialogAddGames.NewGameBanner.Text = imgpath;
                    OpenImageDL("","","");
                }
            }
        }

        //Apply the selected genre filter
        private void ApplyGenreFilter_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext == settingsViewModel)
                DataContext = posterViewModel;
            string genreToFilter = ((Button)sender).Tag.ToString();
            pv.GenreToFilter(genreToFilter);
            pv.RefreshList2(cvs);
            bv.GenreToFilter(genreToFilter);
            bv.RefreshList2(cvs);
            lv.GenreToFilter(genreToFilter);
            lv.RefreshList2(cvs);
            MenuToggleButton.IsChecked = false; //hide hamburger
        }

        //Poster button
        private void PosterButton_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(DateTime.Now + ": Poster View Active");
            PosterViewActive();
        }

        public void PosterViewActive()
        {
            posterViewModel = new PosterViewModel();
            posterViewModel.LoadGames();
            posterViewModel.LoadGenres();
            DataContext = posterViewModel;
            Properties.Settings.Default.viewtype = "Poster";
            Properties.Settings.Default.Save();
        }
        
        //Banner button
        private void BannerButton_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(DateTime.Now + ": Banner View Active");
            BannerViewActive();
        }

        public void BannerViewActive()
        {
            bannerViewModel = new BannerViewModel();
            bannerViewModel.LoadGames();
            bannerViewModel.LoadGenres();
            DataContext = bannerViewModel;
            Properties.Settings.Default.viewtype = "Banner";
            Properties.Settings.Default.Save();
        }

        //List button
        private void ListButton_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(DateTime.Now + ": List View Active");
            ListViewActive();
        }

        public void ListViewActive()
        {
            listViewModel = new ListViewModel();
            listViewModel.LoadGames();
            listViewModel.LoadGenres();
            DataContext = listViewModel;
            Properties.Settings.Default.viewtype = "List";
            Properties.Settings.Default.Save();
        }

        //Settings button
        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(DateTime.Now + ": Settings View Active");
            SettingsViewActive();
        }

        private void SettingsViewActive()
        {
            settingsViewModel = new SettingsViewModel();
            settingsViewModel.LoadGenres();
            DataContext = settingsViewModel;
        }

        //Refresh button
        private void RefreshGames_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshGames();
        }

        public void RefreshGames()
        {
            if (DataContext == listViewModel) { ListViewActive(); }
            else if (DataContext == posterViewModel) { PosterViewActive(); }
            else if (DataContext == bannerViewModel) { BannerViewActive(); }
            else if (DataContext == settingsViewModel) { SettingsViewActive(); }
        }

        private void MWSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resized();
        }
        public void Resized()
        {
            if (isDownloadOpen == true)
            {
                ImageDownload.ChangeWindowSize(this.ActualWidth * 0.8, this.ActualHeight * 0.8);
            }
            if (isExeSearchOpen == true)
            {
                ExeSelection.ChangeWindowSize(this.ActualWidth * 0.9, this.ActualHeight * 0.9);
            }
        }
        //Load settings
        public void LoadSettings()
        {
            //Theme Light or Dark
            if (Settings.Default.theme.ToString() == "Dark")
            {
                ThemeAssist.SetTheme(Application.Current.MainWindow, BaseTheme.Dark);
            }
            else if (Settings.Default.theme.ToString() == "Light")
            {
                ThemeAssist.SetTheme(Application.Current.MainWindow, BaseTheme.Light);
            }
            if (Settings.Default.primary.ToString() != "")
            {
                new PaletteHelper().ReplacePrimaryColor(Settings.Default.primary.ToString());
            }
            if (Settings.Default.accent.ToString() != "")
            {
                new PaletteHelper().ReplaceAccentColor(Settings.Default.accent.ToString());
            }
            if (Settings.Default.viewtype.ToString() == "Poster")
            {
                PosterViewActive();
            }
            if (Settings.Default.viewtype.ToString() == "Banner")
            {
                BannerViewActive();
            }
            if (Settings.Default.viewtype.ToString() == "List")
            {
                ListViewActive();
            }
        }
    }
}