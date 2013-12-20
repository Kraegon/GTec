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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.User.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dTimer = new DispatcherTimer();
        UserIcon icon = new UserIcon(); 
        Bing.Maps.Directions.RouteResponse traveresedRouteResponse;

        public MainPage()
        {

            this.InitializeComponent();

            Map.DirectionsRenderOptions.AutoUpdateMapView = false;

            Map.Children.Add(icon);
            
            dTimer.Interval = new TimeSpan(1000);
            dTimer.Tick += delegate
            {
                showTraversedRoute();
            };

            Controller.Control.GetInstance().ThreadsToNotify.Add(this);

            //Authentication
            AuthenticationFlyout login = new AuthenticationFlyout();
            login.ShowIndependent();

            //Map locations
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;

            standardRoute();

            while(GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Longitude == 777.777)
                Task.Delay(5);

            //Start position and zoomlevel.
            Map.Center = new Location(
                    GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Latitude,
                    GTec.User.Controller.Control.GetInstance().LocationProvider.CurrentLocation.Longitude);

            Map.ZoomLevel = 17.0;
            dTimer.Start();
        }

        public async void standardRoute()
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            waypoints.Add(new Waypoint(51.59380, 4.77963));
            waypoints.Add(new Waypoint(51.59307, 4.77969));
            waypoints.Add(new Waypoint(51.59250, 4.77969));
            waypoints.Add(new Waypoint(51.59250, 4.77968));
            waypoints.Add(new Waypoint(51.59256, 4.77889));
            waypoints.Add(new Waypoint(51.59265, 4.77844));
            waypoints.Add(new Waypoint(51.59258, 4.77806));
            waypoints.Add(new Waypoint(51.59059, 4.77707));
            waypoints.Add(new Waypoint(51.59061, 4.77624));
            waypoints.Add(new Waypoint(51.58992, 4.77634));
            waypoints.Add(new Waypoint(51.59033, 4.77623));
            waypoints.Add(new Waypoint(51.59043, 4.77518));
            waypoints.Add(new Waypoint(51.59000, 4.77429));
            waypoints.Add(new Waypoint(51.59010, 4.77336));
            waypoints.Add(new Waypoint(51.58982, 4.77321));
            waypoints.Add(new Waypoint(51.58932, 4.77444));
            waypoints.Add(new Waypoint(51.58872, 4.77501));
            waypoints.Add(new Waypoint(51.58878, 4.77549));
            waypoints.Add(new Waypoint(51.58864, 4.77501));
            waypoints.Add(new Waypoint(51.58822, 4.77525));
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
            waypoints.Add(new Waypoint(51.58752, 4.77994));
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
            

            Route route = new Route("HKtest1", "bleh.wav", waypoints);

            await Controller.DatabaseConnector.INSTANCE.SaveRouteAsync(route);

            showRoute();
        }

        public async void showRoute()
        {
            Route route =  await Controller.DatabaseConnector.INSTANCE.GetRouteAsync("HKtest1");

            List<Waypoint> waypoints = new List<Waypoint>();

            for (int i = 0; i < route.WayPoints.Count; i++ )
            {
                waypoints.Add(route.WayPoints.ElementAt(i));
            }

            Bing.Maps.Directions.DirectionsManager dm = Map.DirectionsManager;

            Bing.Maps.Directions.WaypointCollection wayPoints = new Bing.Maps.Directions.WaypointCollection();

            foreach(Waypoint wp in waypoints)
            {
                wayPoints.Add(new Bing.Maps.Directions.Waypoint(new Location(wp.Latitude, wp.Longitude)));
            }

            for(int i = 0; i < wayPoints.Count; ++i)
            {
                Bing.Maps.Directions.Waypoint wp = wayPoints[i];
                    if((i%10)==0 || i == 0 || i == wayPoints.Count - 1)
                    {
                        wp.IsViaPoint = false;
                    }
                    else
                    {
                        wp.IsViaPoint = true;
                    }
            }

            GTec.User.Controller.Control.GetInstance().PointOfInterestList = convertToWaypointList(wayPoints);
            dm.Waypoints = wayPoints;

//#if DEBUG
//            GTec.User.Controller.Control.GetInstance().PointOfInterestList[0].Visited = true;
//            GTec.User.Controller.Control.GetInstance().PointOfInterestList[1].Visited = true;
//            GTec.User.Controller.Control.GetInstance().PointOfInterestList[2].Visited = true;
//#endif

            dm.RequestOptions.RouteMode = Bing.Maps.Directions.RouteModeOption.Walking;
            dm.RenderOptions.WaypointPushpinOptions.PushpinTemplate = new ControlTemplate();
            dm.RenderOptions.ActiveRoutePolylineOptions.LineColor = Windows.UI.Colors.Blue;

            Bing.Maps.Directions.RouteResponse response = await dm.CalculateDirectionsAsync();

            dm.ShowRoutePath(response.Routes[0]);
            dm.ActiveRoute.RoutePath.LineWidth = 10;
        }

        public async void showTraversedRoute()
        {
            Bing.Maps.Directions.DirectionsManager dm = Map.DirectionsManager;

            if (GTec.User.Controller.Control.GetInstance().PointOfInterestList.Count <= 1)
                return;

            Bing.Maps.Directions.WaypointCollection col = new Bing.Maps.Directions.WaypointCollection();

            foreach (PointOfInterest poi in GTec.User.Controller.Control.GetInstance().PointOfInterestList)
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

            dm.RenderOptions.ActiveRoutePolylineOptions.LineColor = Windows.UI.Colors.Red;
                
            traveresedRouteResponse = await dm.CalculateDirectionsAsync();

            dm.ShowRoutePath(traveresedRouteResponse.Routes[0]);
        }

        void onSettingsCommand(IUICommand command)
        {
            SettingsCommand settingsCommand = (SettingsCommand)command;
            switch (settingsCommand.Id as string)
            {
                case "auth":
                    new AuthenticationFlyout().ShowIndependent();
                    break;
                default:
                    new AuthenticationFlyout().ShowIndependent();
                    break;
            }
        }

        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);
            SettingsCommand authenticateCommand = new SettingsCommand("auth", "Authenticatie", handler);
            eventArgs.Request.ApplicationCommands.Add(authenticateCommand);
        }

        public void OpenAuthenticationFlyout()
        {
            AuthenticationFlyout login = new AuthenticationFlyout();
            login.ShowIndependent();
        }

        private void fillMapWithPointsOfInterest(Route currentRoute) { 

            foreach (PointOfInterest pointOfInterest in currentRoute.WayPoints) { 
                Pushpin pushpin = new Pushpin();

                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Windows.UI.Colors.Blue;
                pushpin.Background = brush;

                pushpin.Text = pointOfInterest.Name; 
                MapLayer.SetPosition(pushpin, new Location(pointOfInterest.Latitude, pointOfInterest.Longitude));
                Map.Children.Add(pushpin); 
            }
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

        private Bing.Maps.Directions.WaypointCollection convertToBingWaypointCollection(List<PointOfInterest> wayPoints)
        {
            Bing.Maps.Directions.WaypointCollection wayPointsCollections = new Bing.Maps.Directions.WaypointCollection();
            foreach (PointOfInterest poi in wayPoints)
                wayPointsCollections.Add(new Bing.Maps.Directions.Waypoint(new Location(poi.Latitude, poi.Longitude)));
            return wayPointsCollections;
        }

        private List<PointOfInterest> convertToWaypointList(Bing.Maps.Directions.WaypointCollection wayPoints)
        {
            List<PointOfInterest> poiList = new List<PointOfInterest>();
            foreach(Bing.Maps.Directions.Waypoint waypoint in wayPoints)
                poiList.Add(new PointOfInterest(waypoint.Location.Latitude, waypoint.Location.Longitude, false, "", "", ""));
            return poiList;
        }
    }
}
