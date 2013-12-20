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
        MapLayer layer;

        public MainPage()
        {
            this.InitializeComponent();

            Controller.Control.GetInstance().ThreadsToNotify.Add(this);
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
            layer = MainLayer;

            //Map locations
            Route TestRoute = new Route("Seattle - The Route", "yay.wav", new List<Waypoint> { new PointOfInterest(47.6035, -122.3294, true, "Seattle", "Seattle is in the state of Washington. Lorem ipsum dolor, lorem isum dolor, Lorem ipsum dolor sit amet, lorem ipsum dolor sit amet.", "ms-appx:///Assets/Seattle_Image.png") });
            AddPointsOfInterest(TestRoute);

            //Start position and zoomlevel.
            Map.Center = new Location(51.58458, 4.77464);
            Map.ZoomLevel = 4.0;
        }

        private void AddPointsOfInterest(Route currentRoute)
        {
            foreach (PointOfInterest pointOfInterest in currentRoute.WayPoints)
                AddPushpin(pointOfInterest);
        }

        private void AddPushpin(PointOfInterest poi)
        {
            Pushpin pp = new Pushpin()
            { 
                Tag = new InfoBoxData() { Title = poi.Name, Description = poi.Information, ImagePath = poi.ImagePath } 
            };
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Windows.UI.Colors.Blue;
            pp.Background = brush;

            MapLayer.SetPosition(pp, new Location(poi.Latitude, poi.Longitude));
            pp.Tapped += PinTapped;
            layer.Children.Add(pp);
        }

        private void PinTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Pushpin pp = sender as Pushpin;
            InfoBoxData ibd = (InfoBoxData)pp.Tag;
            InfoBox box = new InfoBox();
            box.AddData(ibd);
            layer.Children.Add(box);

            if (!String.IsNullOrEmpty(ibd.Title) || !String.IsNullOrEmpty(ibd.Description))          
                MapLayer.SetPosition(box, MapLayer.GetPosition(pp));
            else
                box.Visibility = Visibility.Collapsed;
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
            SettingsCommand authenticateCommand = new SettingsCommand("auth", "Authenticatie", handler);
            eventArgs.Request.ApplicationCommands.Clear();
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

        public struct InfoBoxData
        {
            public string Title;
            public string Description;
            public string ImagePath;
        }
    }
}
