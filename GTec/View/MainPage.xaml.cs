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
using Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            fillMapWithPointsOfInterest(new Route("yay3", "yay.wav", new Waypoint[]{ new PointOfInterest(50,50,true,"yay","yay2","yay.png")}));
        }

        private void CloseInfobox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Collapsed;
        }

        private void fillMapWithPointsOfInterest(Route currentRoute) { 
            //PushpinOptions pushpinOptions = new PushpinOptions(); 
            //pushpinOptions.icon = "PointOfInterest.png"; 
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

        public class Metadata
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        private void PinTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Pushpin p = sender as Pushpin;
            Metadata m = (Metadata)p.Tag;

            //Ensure there is content to be displayed before modifying the infobox control
            if (!String.IsNullOrEmpty(m.Title) || !String.IsNullOrEmpty(m.Description))
            {
                Grid.DataContext = m;

                Grid.Visibility = Visibility.Visible;

                MapLayer.SetPosition(Grid, MapLayer.GetPosition(p));
            }
            else
            {
                Grid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
