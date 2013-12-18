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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.User.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Authentication
            AuthenticationFlyout login = new AuthenticationFlyout();
            login.ShowIndependent();

            //Map locations
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
            //fillMapWithPointsOfInterest(new Route("yay3", "yay.wav", new Waypoint[]{ new PointOfInterest(50,50,true,"yay","yay2","yay.png")}));

            //Start position and zoomlevel.
            Map.Center = new Location(51.58458, 4.77464);
            Map.ZoomLevel = 12.0;
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

    }
}
