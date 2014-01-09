using GTec.User.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System.Threading;
using System.Threading.Tasks;
using Bing.Maps;
using GTec.User.Model;
using GTec.User.View;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using GTec.Admin.View;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.User.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MapLayer layer = new MapLayer();
        UserIcon icon = new UserIcon();
        Bing.Maps.Directions.RouteResponse traveresedRouteResponse;

        public MainPage()
        {
                this.InitializeComponent();
              //GTec.User.Controller.Control.GetInstance();
              Map.DirectionsRenderOptions.AutoUpdateMapView = false;
              GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;
              GeofenceMonitor.Current.Geofences.Clear();

              Map.Children.Add(icon);
              Map.Children.Add(layer);

              Controller.Control.GetInstance().ThreadsToNotify.Add(this);
  
              //Authentication
              //AuthenticationFlyout login = new AuthenticationFlyout();
              //login.ShowIndependent();
  
              //Map locations
              SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
             //fillMapWithPointsOfInterest(new Route("yay3", "yay.wav", new Waypoint[]{ new PointOfInterest(50,50,true,"yay","yay2","yay.png")}));

              

              standardRoute();



              //Current Language Combobox
              SetComboboxLanguage();
              //Controller.Control.GetInstance().MainPage = this;
              initLocationServ();
          }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            GeofenceMonitor.Current.Geofences.Clear();
            GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;
        }

          private async void initLocationServ()
          {
              GTec.User.Controller.Control.GetInstance().MainPage = this;

              while (GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Longitude == 777.777)
              {
                  await Task.Delay(10);
                  //GTec.User.Controller.Control.GetInstance().LocationProvider = new LocationServiceProviderCaller(GTec.User.Controller.Control.GetInstance().ThreadsToNotify);
              }

              //Start position and zoomlevel.
              Map.Center = new Location(51.58458, 4.77464);
              Map.ZoomLevel = 12.0;
              Map.Center = new Location(
                      GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Latitude,
                      GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Longitude);

              Map.ZoomLevel = 17.0;

              ChangeUserIconPosition();
          }

        public async void ChangeUserIconPosition()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
              {
                  MapLayer.SetPosition(icon, new Location(
                      GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Latitude,
                      GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Longitude));
              });
        }

          public async void OnGeofenceStateChanged(GeofenceMonitor sender, object e)
          {
              var reports = sender.ReadReports();

              await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  foreach (GeofenceStateChangeReport report in reports)
                  {
                      GeofenceState state = report.NewState;

                      Geofence geofence = report.Geofence;

                      if (state == GeofenceState.Entered)
                      {
                          try
                          {
                              Waypoint waypoint = GTec.User.Controller.Control.GetInstance().CurrentRoute.WayPoints[int.Parse(geofence.Id)];
                              waypoint.Visited = true;
                              try
                              {
                                  foreach (UIElement element in layer.Children)
                                  {
                                      if (waypoint == null)
                                          continue;

                                      if (waypoint as PointOfInterest == null)
                                          continue;

                                      if((waypoint as PointOfInterest).Name == null)
                                        continue;

                                      if (element == null)
                                          continue;

                                      if (element as Pushpin == null)
                                          continue;

                                      if ((element as Pushpin).Tag == null)
                                          continue;

                                      if(((InfoBoxData)(element as Pushpin).Tag).Title == null)
                                          continue;

                                      if ((((InfoBoxData)(element as Pushpin).Tag)).Title == (waypoint as PointOfInterest).Name)
                                      {
                                          PinTapped(element, new TappedRoutedEventArgs());
                                          break;
                                      }
                                  }
                              }
                              catch
                              {
                                  //YAY
                              }
                          }
                          catch
                          {
                              System.Diagnostics.Debug.WriteLine("Mate, your OnGeofenceStateChanged? It sucks, look at this?! THIS IS CALLED AN EERRRROOORRRR!!!!");
                          }
                          showTraversedRoute();
                          saveTraversedRoute();
                      }
                  }
              });
          }

          private async void saveTraversedRoute()
          {
                Route traversedRoute = new Route(Controller.Control.GetInstance().CurrentRoute.Name,
                                                 Controller.Control.GetInstance().CurrentRoute.SystemSoundPath,
                                                 new List<Waypoint>());
                foreach(Waypoint wp in Controller.Control.GetInstance().CurrentRoute.WayPoints)
                {
                    if (wp.Visited)
                        traversedRoute.WayPoints.Add(wp);
                }
                await Controller.Control.GetInstance().DatabaseConnnector.SaveVisitedRouteAsync(traversedRoute);
          }

        private void AddPointsOfInterest(Route currentRoute)
        {
            GeofenceMonitor.Current.Geofences.Clear();
            for (int i = 0; i < currentRoute.WayPoints.Count; ++i)
            {
                AddPushpin(currentRoute.WayPoints[i], i);
            }
        }

        private void AddPushpin(Waypoint poi, int ID)
        {
            Pushpin pp = null;
            if (poi is PointOfInterest)
            {
                PointOfInterest poii = poi as PointOfInterest;
                pp = new Pushpin()
                {
                    Tag = new InfoBoxData() { Title = poii.Name, Description = poii.Information, ImagePath = poii.ImagePath }
                };
                pp.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                pp = new Pushpin();
                pp.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
            }

            MapLayer.SetPosition(pp, new Location(poi.Latitude, poi.Longitude));
            pp.Tapped += PinTapped;
            layer.Children.Add(pp);

            Geocircle circle = new Geocircle(new BasicGeoposition() { Latitude = poi.Latitude, Longitude = poi.Longitude, Altitude=0 }, 15);
            Geofence fence = new Geofence(""+ID, circle, MonitoredGeofenceStates.Entered, false, new TimeSpan(10));
            
            GeofenceMonitor.Current.Geofences.Add(fence);
        }

        private void PinTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            foreach (UIElement element in layer.Children)
            {
                try
                {
                    if (!(element is InfoBox))
                        continue;

                    if ((element as InfoBox).Tag == "Popup")
                        (element as InfoBox).Visibility = Visibility.Collapsed;
                }
                catch
                { }
            }

            Pushpin pp = sender as Pushpin;

            try
            {
                InfoBoxData ibd = (InfoBoxData)pp.Tag;
                InfoBox box = new InfoBox();
                box.AddData(ibd);
                layer.Children.Add(box);
                box.Tag = "Popup";

                if (!String.IsNullOrEmpty(ibd.Title) || !String.IsNullOrEmpty(ibd.Description))
                    MapLayer.SetPosition(box, MapLayer.GetPosition(pp));
                else
                    box.Visibility = Visibility.Collapsed;
                SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
            }
            catch { }
        }

        public async void standardRoute()
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            GTec.User.Model.Route route = await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.GetCurrentRoute();

            #region hard-coded waypoints
            List<string> routeNameList = await GTec.User.Controller.DatabaseConnector.INSTANCE.GetRouteNamesAsync();

            bool found = false;
            foreach (string str in routeNameList)
                if (str == "HKtest1")
                    found = true;

            if (!found)
            {
                waypoints.Add(new PointOfInterest(51.59380, 4.77963, false, "VVV", "VVV", "ms-appx:///Assets/BadgeLogo.scale-100.png", " "));
                waypoints.Add(new Waypoint(51.59307, 4.77969));
                waypoints.Add(new Waypoint(51.59250, 4.77969));
                waypoints.Add(new PointOfInterest(51.59250, 4.77968, false, "NassauMonument", "NassauMonument", "ms-appx:///Assets/images/NassauMonument.jpg", " "));
                waypoints.Add(new PointOfInterest(51.59256, 4.77889, false, "Valkenberg", "Valkenberg", " ", " "));
                waypoints.Add(new Waypoint(51.59265, 4.77844));
                waypoints.Add(new Waypoint(51.59258, 4.77806));
                waypoints.Add(new Waypoint(51.59059, 4.77707));
                waypoints.Add(new PointOfInterest(51.59061, 4.77624, false, "Kasteel", "Kasteel", "ms-appx:///Assets/images/kasteel.jpg", " "));
                waypoints.Add(new Waypoint(51.58992, 4.77634));
                waypoints.Add(new Waypoint(51.59033, 4.77623));
                waypoints.Add(new Waypoint(51.59043, 4.77518));
                waypoints.Add(new Waypoint(51.59000, 4.77429));
                waypoints.Add(new PointOfInterest(51.59010, 4.77336, false, "Vishal", "Vishal", "ms-appx:///Assets/images/Vishal.jpg", " "));
                waypoints.Add(new Waypoint(51.58982, 4.77321));
                waypoints.Add(new Waypoint(51.58932, 4.77444));
                waypoints.Add(new PointOfInterest(51.58872, 4.77501, false, "Stadhuis", "Stadhuis", "ms-appx:///Assets/images/stadhuis.jpg", " "));
                waypoints.Add(new PointOfInterest(51.58878, 4.77549, false, "AntoniusKerk", "AntoniusKerk", "ms-appx:///Assets/images/stAntoniuskerk.jpg", " "));
                waypoints.Add(new Waypoint(51.58864, 4.77501));
                waypoints.Add(new PointOfInterest(51.58822, 4.77525, false, "Kloosterkazerne", "Kloosterkazerne", "ms-appx:///Assets/images/kloosterKazerne.jpg", " "));
                waypoints.Add(new Waypoint(51.58716, 4.77582));
                waypoints.Add(new Waypoint(51.58747, 4.77662));
                waypoints.Add(new Waypoint(51.58771, 4.77652));
                waypoints.Add(new Waypoint(51.58797, 4.77638));
                waypoints.Add(new Waypoint(51.58885, 4.77616));
                waypoints.Add(new Waypoint(51.58883, 4.77617));
                waypoints.Add(new Waypoint(51.58889, 4.77659));
                waypoints.Add(new Waypoint(51.58883, 4.77618));
                waypoints.Add(new Waypoint(51.58747, 4.77663));
                waypoints.Add(new Waypoint(51.58761, 4.77712));
                waypoints.Add(new Waypoint(51.58828, 4.77858));
                waypoints.Add(new Waypoint(51.58773, 4.77948));
                waypoints.Add(new PointOfInterest(51.58752, 4.77994, false, "Bibliotheek", "Bibliotheek", "ms-appx:///Assets/images/bibliotheek.jpg", " "));
                waypoints.Add(new Waypoint(51.58794, 4.78105));
                waypoints.Add(new Waypoint(51.58794, 4.78218));
                waypoints.Add(new Waypoint(51.58794, 4.78106));
                waypoints.Add(new Waypoint(51.58862, 4.78079));
                waypoints.Add(new Waypoint(51.58955, 4.78038));
                waypoints.Add(new Waypoint(51.58966, 4.78076));
                waypoints.Add(new Waypoint(51.58939, 4.77982));
                waypoints.Add(new Waypoint(51.58905, 4.77981));
                waypoints.Add(new Waypoint(51.58846, 4.77830));
                waypoints.Add(new Waypoint(51.58905, 4.77801));
                waypoints.Add(new Waypoint(51.58918, 4.77841));
                waypoints.Add(new Waypoint(51.58905, 4.77802));
                waypoints.Add(new Waypoint(51.58960, 4.77770));
                waypoints.Add(new Waypoint(51.58965, 4.77830));
                waypoints.Add(new Waypoint(51.58997, 4.77810));
                waypoints.Add(new Waypoint(51.58965, 4.77831));
                waypoints.Add(new Waypoint(51.58950, 4.77649));

                Route route1 = new Route("HKtest1", "bleh.wav", waypoints);

                await Controller.DatabaseConnector.INSTANCE.SaveRouteAsync(route1);
            }
            #endregion

            //if (route == null)
            //{
            //    MessageDialog makeAdminWork = new MessageDialog("Admin, work.", "Make admin work.");
            //    await makeAdminWork.ShowAsync();
            //    return;
            //}

            showRoute();
        }

        public async void showRoute()
        {
            //Route route = await Controller.DatabaseConnector.INSTANCE.GetRouteAsync("HKtest1");
            if (Controller.Control.GetInstance().CurrentRoute == null || Controller.Control.GetInstance().CurrentRoute.Name == null)
            {
                Controller.Control.GetInstance().CurrentRoute = await Controller.DatabaseConnector.INSTANCE.GetCurrentRoute();
                Route r = await Controller.DatabaseConnector.INSTANCE.GetVisitedRoute();
                

                if (r != null && Controller.Control.GetInstance().CurrentRoute != null)
                {
                    foreach (Waypoint wp in r.WayPoints)
                    {
                        foreach (Waypoint rwp in Controller.Control.GetInstance().CurrentRoute.WayPoints)
                        {
                            if (wp.Equals(rwp))
                            {
                                rwp.Visited = true;
                                break;
                            }
                        }
                    }
                }
                else
                    System.Diagnostics.Debug.WriteLine("TraversedRoute in DB is null");
            }
            Route route = Controller.Control.GetInstance().CurrentRoute;

            if (route == null)
                return;

            List<Waypoint> waypoints = new List<Waypoint>();

            for (int i = 0; i < route.WayPoints.Count; i++)
            {
                waypoints.Add(route.WayPoints.ElementAt(i));
            }

            Bing.Maps.Directions.DirectionsManager dm = Map.DirectionsManager;

            Bing.Maps.Directions.WaypointCollection wayPoints = new Bing.Maps.Directions.WaypointCollection();

            foreach (Waypoint wp in waypoints)
            {
                wayPoints.Add(new Bing.Maps.Directions.Waypoint(new Location(wp.Latitude, wp.Longitude)));
            }

            for (int i = 0; i < wayPoints.Count; ++i)
            {
                Bing.Maps.Directions.Waypoint wp = wayPoints[i];
                if ((i % 10) == 0 || i == 0 || i == wayPoints.Count - 1)
                {
                    wp.IsViaPoint = false;
                }
                else
                {
                    wp.IsViaPoint = true;
                }
            }

            //GTec.User.Controller.Control.GetInstance().CurrentRoute.WayPoints = convertToWaypointList(wayPoints);
            dm.Waypoints = wayPoints;
            dm.RequestOptions.RouteMode = Bing.Maps.Directions.RouteModeOption.Walking;
            dm.RenderOptions.WaypointPushpinOptions.PushpinTemplate = new ControlTemplate();
            dm.RenderOptions.ActiveRoutePolylineOptions.LineColor = Windows.UI.Colors.Orange;

            Bing.Maps.Directions.RouteResponse response = await dm.CalculateDirectionsAsync();
            if (response.Routes.Count > 0)
            {
                dm.ShowRoutePath(response.Routes[0]);
                dm.ActiveRoute.RoutePath.LineWidth = 10;

                AddPointsOfInterest(GTec.User.Controller.Control.GetInstance().CurrentRoute);
            }

            showTraversedRoute();
        }

        public async void showTraversedRoute()
        {
            Bing.Maps.Directions.DirectionsManager dm = Map.DirectionsManager;

            if (GTec.User.Controller.Control.GetInstance().CurrentRoute == null || GTec.User.Controller.Control.GetInstance().CurrentRoute.WayPoints.Count <= 1)
                return;

            Bing.Maps.Directions.WaypointCollection col = new Bing.Maps.Directions.WaypointCollection();

            foreach (Waypoint poi in GTec.User.Controller.Control.GetInstance().CurrentRoute.WayPoints)
            {

                if (poi.Visited)
                {
                    Bing.Maps.Directions.Waypoint wp = new Bing.Maps.Directions.Waypoint(new Location(poi.Latitude, poi.Longitude));
                    wp.IsViaPoint = true;
                    col.Add(wp);
                }
            }

            if (col.Count <= 1)
                return;

            col[0].IsViaPoint = false;
            col[col.Count - 1].IsViaPoint = false;

            dm.Waypoints = col;

            for (int i = 0; i < col.Count - 1; ++i)
            {
                Bing.Maps.Directions.Waypoint wp = col[i];
                if ((i % 10) == 0 || i == 0 || i == col.Count - 1)
                {
                    wp.IsViaPoint = false;
                }
                else
                {
                    wp.IsViaPoint = true;
                }
            }

            dm.RenderOptions.ActiveRoutePolylineOptions.LineColor = Windows.UI.Colors.Gray;

            traveresedRouteResponse = await dm.CalculateDirectionsAsync();

            dm.ShowRoutePath(traveresedRouteResponse.Routes[0]);
        }

        void onSettingsCommand(IUICommand command)
        {
            SettingsCommand settingsCommand = (SettingsCommand)command;
            switch (settingsCommand.Id as string)
            {
                case "auth":
                    OpenAuthenticationFlyout();
                    break;
                default:
                    OpenAuthenticationFlyout();
                    break;
            }
        }

        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);
            SettingsCommand authenticateCommand = new SettingsCommand("auth", "Admin", handler);
            eventArgs.Request.ApplicationCommands.Clear();
            eventArgs.Request.ApplicationCommands.Add(authenticateCommand);
        }

        public void OpenAuthenticationFlyout()
        {
            AuthenticationFlyout login = new AuthenticationFlyout();
            login.ShowIndependent();
        }

        //Zoom in/out buttons
        private void max_Click(object sender, RoutedEventArgs e)
        {
            Map.ZoomLevel += 0.2f;
        }

        private void min_Click(object sender, RoutedEventArgs e)
        {
            Map.ZoomLevel -= 0.2f;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HelpPage));
        }

        public struct InfoBoxData
        {
            public string Title;
            public string Description;
            public string ImagePath;
        }
        private Bing.Maps.Directions.WaypointCollection convertToBingWaypointCollection(List<Waypoint> wayPoints)
        {
            Bing.Maps.Directions.WaypointCollection wayPointsCollections = new Bing.Maps.Directions.WaypointCollection();
            foreach (Waypoint poi in wayPoints)
                wayPointsCollections.Add(new Bing.Maps.Directions.Waypoint(new Location(poi.Latitude, poi.Longitude)));
            return wayPointsCollections;
        }

        private List<Waypoint> convertToWaypointList(Bing.Maps.Directions.WaypointCollection wayPoints)
        {
            List<Waypoint> poiList = new List<Waypoint>();
            foreach (Bing.Maps.Directions.Waypoint waypoint in wayPoints)
                poiList.Add(new Waypoint(waypoint.Location.Latitude, waypoint.Location.Longitude, false));
            return poiList;
        }

        private void SetComboboxLanguage()
        {
            var currentLanguage = User.Controller.Control.GetInstance().LanguageManager.CurrentLanguage;
            if (currentLanguage == User.Controller.Language.Dutch)
                languageBox.SelectedIndex = 0;
            else if(currentLanguage == User.Controller.Language.English)
                languageBox.SelectedIndex = 1;
        }

        private void languageBox_Select(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null && dutchFlag != null)
            {
                System.Diagnostics.Debug.WriteLine("ToString van het plaatje: " + languageBox.SelectedIndex);
                if ((sender as ComboBox).SelectedIndex == 0)//TODO: This does not work, fix later. FIXME
                {
                    Controller.Control.GetInstance().LanguageManager.CurrentLanguage = Controller.Language.Dutch;
                    languageBox.SelectedItem = dutchFlag;
                }
                else if ((sender as ComboBox).SelectedIndex == 1)
                {
                    Controller.Control.GetInstance().LanguageManager.CurrentLanguage = Controller.Language.English;
                    languageBox.SelectedItem = englishFlag;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // De huidige content van de applicatie opslaan alvorens het naar de andere pagina gaat....
            saveContent(e.Content);
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Hier een link naar een manier om de toestand van de applicatie terug te halen zoals het toen was....
            base.OnNavigatedTo(e);
        }

        private void saveContent(Object o)
        {
            // Een manier verzinnen om de huidige content van de applicatie hier op te slaan...
        }
    }
}
