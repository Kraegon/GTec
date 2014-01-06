﻿using GTec.User.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.Admin.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        List<PointOfInterest> WayPoints = new List<PointOfInterest>();

        public AdminPage()
        {
            this.InitializeComponent();
            officeListBox.ItemsSource = WayPoints;
            officeListBox.ItemTemplate = Resources["PointOfInterestDisplay"] as DataTemplate;
            initWayPoints(); 
            initRoutes();
        }

        public async void initWayPoints()
        {
            ExistingWayPointsBox.ItemsSource = await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.GetAllWaypoints();
        }

        public async void initRoutes()
        {
            CurrentRoute.ItemsSource = await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.GetRouteNamesAsync();
        }

        private void goBackward_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void AddNewWayPointToRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Latitude.Text == "" || Longitude.Text == "" || Name.Text == "" || ImagePath.Text == "" || SoundPath.Text == "")
                return;

            try
            {
                WayPoints.Add(new PointOfInterest(Double.Parse(Latitude.Text), Double.Parse(Longitude.Text), false, Name.Text, "", ImagePath.Text, SoundPath.Text));
            }
            catch { return; }

            officeListBox.ItemsSource = null;
            officeListBox.ItemsSource = WayPoints;
        }

        private void AddExistingWayPointToRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ExistingWayPointsBox.SelectedItem == null)
                return;

            WayPoints.Add(ExistingWayPointsBox.SelectedItem is PointOfInterest ? ExistingWayPointsBox.SelectedItem as PointOfInterest : new PointOfInterest((ExistingWayPointsBox.SelectedItem as Waypoint).Latitude, (ExistingWayPointsBox.SelectedItem as Waypoint).Longitude, false, "", "", "", ""));
            officeListBox.ItemsSource = null;
            officeListBox.ItemsSource = WayPoints;
        }

        private void DeleteItem_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(PointOfInterest poi in WayPoints)
            {
                if (((string)((sender as Button).Tag)) == poi.Name)
                {
                    WayPoints.Remove(poi);
                    officeListBox.ItemsSource = null;
                    officeListBox.ItemsSource = WayPoints;
                    break;
                }
            }
        }

        private async void SaveRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if (WayPoints.Count == 0 || RouteName.Text == "" || RouteSoundPath.Text == "" || WayPoints.Count < 2)
            {
                if (CurrentRoute.SelectedItem != null)
                {
                    MessageDialog msgDialog = new MessageDialog("Please wait a second, while we take a few seconds to set things up for you.", "Please be patient!");
                    IAsyncOperation<IUICommand> asyncCommand = msgDialog.ShowAsync();  //No need to wait for this.

                    Route r = await GTec.User.Controller.DatabaseConnector.INSTANCE.GetRouteAsync(CurrentRoute.SelectedItem as string);
                    await GTec.User.Controller.DatabaseConnector.INSTANCE.SaveCurrentRouteAsync(r);
                    GTec.User.Controller.Control.GetInstance().CurrentRoute = r;

                    asyncCommand.Cancel();
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                    }
                }
                return;
            }

            List<Waypoint> waypoints = new List<Waypoint>();
            foreach (PointOfInterest poi in WayPoints)
            {
                waypoints.Add(poi as Waypoint);
            }
            Route route = new Route(RouteName.Text, RouteSoundPath.Text, waypoints);

            await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.SaveRouteAsync(route);

            if ((bool)SetAsCurrentRouteCheckBox.IsChecked)
            {
                GTec.User.Controller.Control.GetInstance().CurrentRoute = route;
                await GTec.User.Controller.DatabaseConnector.INSTANCE.SaveCurrentRouteAsync(route);
            }
            else
            {
                if (CurrentRoute.SelectedItem != null)
                {
                    await GTec.User.Controller.DatabaseConnector.INSTANCE.SaveCurrentRouteAsync(route);
                    GTec.User.Controller.Control.GetInstance().CurrentRoute = await GTec.User.Controller.DatabaseConnector.INSTANCE.GetRouteAsync(CurrentRoute.SelectedItem as string);
                }
            }
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void ExistingWayPointsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PointOfInterest poi = ExistingWayPointsBox.SelectedItem as PointOfInterest;
            Windows.UI.Xaml.Media.Imaging.BitmapImage icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
            try
            {
                icon.UriSource = new Uri(poi.ImagePath);
                ImageExistingWayPoint.Source = icon;
            }
            catch { }

            NameExistingWayPoint.Text = poi.Name;
            LatitudeExistingWayPoint.Text = poi.Latitude.ToString();
            LongitudeExistingWayPoint.Text = poi.Longitude.ToString();
            SoundPathExistingWayPoint.Text = poi.SoundPath.ToString();
            ImagePathExistingWayPoint.Text = poi.ImagePath.ToString();
        }
    }
}
