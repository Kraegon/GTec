using GTec.User.Model;
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
        List<Waypoint> WayPoints = new List<Waypoint>();
        List<Waypoint> EditedRouteWayPoints = new List<Waypoint>();

        public AdminPage()
        {
            this.InitializeComponent();
            officeListBox.ItemsSource = WayPoints;
            officeListBox.ItemTemplate = Resources["PointOfInterestDisplay"] as DataTemplate;
            officeListBox2.ItemsSource = EditedRouteWayPoints;
            officeListBox2.ItemTemplate = Resources["EditedPointOfInterestDisplay"] as DataTemplate;
            initWayPoints(); 
            initRoutes();
            loadStrings();
        }

        private async void loadStrings()
        {
            NewRouteHeader.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("back");
            goBackward.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("back");
            AddWayPointButton.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("toevoegen");
            NewWaypointNameLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("name");
            NewWaypointLongitudeLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("longitude");
            NewWaypointLatitudeLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("latitude");
            NewWaypointPadnaamAfbeeldingLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("imagepath");
            PickExistingWaypointPickingLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("PickExistingWaypoint");
            PickExistingWaypointImageLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("image");
            PickExistingWaypointNameLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("name");
            PickExistingWaypointLongitudeLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("longitude");
            PickExistingWaypointLatitudeLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("latitude");
            PickExistingWaypointImagePathLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("imagepath");
            AddExistingWaypointButton.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("toevoegen");
            PlaySoundExistingWaypointButton.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("playSound");
            RouteNameLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("name");
            ActiveLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("active");
            SaveRouteButton.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("save");
            RemoveWaypointButton.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("removeRoute");
            NewWaypointTextLabel.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("description");

            EditHeaderTextBlock.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("edit");
            NewWaypointNameLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("name");
            NewWaypointLongitudeLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("longitude");
            NewWaypointLatitudeLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("latitude");
            NewWaypointPadnaamAfbeeldingLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("imagepath");
            NewWaypointTextLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("description");
            AddWayPointButton2.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("editWaypoint");
            PickExistingWaypointPickingLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("PickExistingWaypoint");
            PickExistingWaypointImageLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("image");
            PickExistingWaypointNameLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("name");
            PickExistingWaypointLongitudeLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("longitude");
            PickExistingWaypointLatitudeLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("latitude");
            PickExistingWaypointImagePathLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("imagepath");
            AddExistingWaypointButton2.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("edit");
            RouteNameLabel2.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("name");
            SaveRouteButton2.Content = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("save");
        }

        public async void initWayPoints()
        {
            List<Waypoint> temp = await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.GetAllWaypoints();
            ExistingWayPointsBox.ItemsSource = temp;
            ExistingWayPointsBox2.ItemsSource = temp;
        }

        public async void initRoutes()
        {
            List<string> temp =  await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.GetRouteNamesAsync();
            CurrentRoute.ItemsSource = temp;
            CurrentRoute2.ItemsSource = temp;
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
            if (Latitude.Text == "" || Longitude.Text == "")
                return;
            if (Name.Text == "")
            {
                try
                {
                    WayPoints.Add(new Waypoint(Double.Parse(Latitude.Text), Double.Parse(Longitude.Text)));
                }
                catch { return; }
            }    //Add as waypoint
            else
            {
                try
                {
                    WayPoints.Add(new PointOfInterest(Double.Parse(Latitude.Text), Double.Parse(Longitude.Text), false, Name.Text, Text.Text, ImagePath.Text, ""));
                }
                catch { return; }
            }    //Add as PoI
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
            foreach (Waypoint waypoint in WayPoints)
            {
                if (((string)((sender as Button).Tag)) == waypoint.StringRep)
                {
                    WayPoints.Remove(waypoint);
                    officeListBox.ItemsSource = null;
                    officeListBox.ItemsSource = WayPoints;
                    break;
                }
            }
        }

        private async void SaveRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if (WayPoints.Count == 0 || RouteName.Text == "" || WayPoints.Count < 2)
            {
                if (CurrentRoute.SelectedItem != null)
                {
                    var currentLanguage = User.Controller.Control.GetInstance().LanguageManager.CurrentLanguage;
                    MessageDialog msgDialog = new MessageDialog("bleh");
                    if (currentLanguage == User.Controller.Language.English)
                    {
                        msgDialog.Content = "Please wait a second, while we take a few seconds to set things up for you.";
                        msgDialog.Title = "Please be patient!";
                    }
                    else if (currentLanguage == User.Controller.Language.Dutch)
                    {
                        msgDialog.Content = "Graag een moment geduld, terwijl alles in gereedheid wordt gebracht";
                        msgDialog.Title = "Een moment geduld aub!";
                    }
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
            foreach (Waypoint waypoint in WayPoints)
            {
                if(waypoint is PointOfInterest)
                    waypoints.Add(waypoint as PointOfInterest);
                else
                    waypoints.Add(waypoint as Waypoint);               
            }
            Route route = new Route(RouteName.Text, "", waypoints);

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
            Waypoint waypoint = ExistingWayPointsBox.SelectedItem as Waypoint;
            if (waypoint is PointOfInterest)
            {
                PointOfInterest poi = waypoint as PointOfInterest;
                Windows.UI.Xaml.Media.Imaging.BitmapImage icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                try
                {
                    icon.UriSource = new Uri(poi.ImagePath);
                    ImageExistingWayPoint.Source = icon;
                }
                catch { }
                LatitudeExistingWayPoint.Text = poi.Latitude.ToString();
                LongitudeExistingWayPoint.Text = poi.Longitude.ToString();
                NameExistingWayPoint.Text = poi.Name;
                Text.Text = poi.Information;
                ImagePathExistingWayPoint.Text = poi.ImagePath.ToString();

            }
            else
            {
                ImageExistingWayPoint.Source = null;
                LatitudeExistingWayPoint.Text = waypoint.Latitude.ToString();
                LongitudeExistingWayPoint.Text = waypoint.Longitude.ToString();
                NameExistingWayPoint.Text = "";
                ImagePathExistingWayPoint.Text = "";
            }
        }
        private async void CurrentRoute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Route currentSelection = await User.Controller.Control.GetInstance().DatabaseConnnector.GetRouteAsync(CurrentRoute.SelectedItem as string);
            WayPoints = currentSelection.WayPoints;
            RouteName.Text = currentSelection.Name;
            officeListBox.ItemsSource = null;
            officeListBox.ItemsSource = WayPoints;
        }
        /// <summary>
        /// NEW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewWayPointToEditedRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Latitude2.Text == "" || Longitude2.Text == "")
                return;
            if (Name2.Text == "")
            {
                try
                {
                    EditedRouteWayPoints.Add(new Waypoint(Double.Parse(Latitude2.Text), Double.Parse(Longitude2.Text)));
                }
                catch { return; }
            }    //Add as waypoint
            else
            {
                try
                {
                    EditedRouteWayPoints.Add(new PointOfInterest(Double.Parse(Latitude2.Text), Double.Parse(Longitude2.Text), false, Name2.Text, Text2.Text, ImagePath2.Text, ""));
                }
                catch { return; }
            }    //Add as PoI


            officeListBox2.ItemsSource = null;
            officeListBox2.ItemsSource = EditedRouteWayPoints;
        }

        private void AddExistingWayPointToEditedRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ExistingWayPointsBox2.SelectedItem == null)
                return;

            EditedRouteWayPoints.Add(ExistingWayPointsBox2.SelectedItem is PointOfInterest ? ExistingWayPointsBox2.SelectedItem as PointOfInterest : new PointOfInterest((ExistingWayPointsBox2.SelectedItem as Waypoint).Latitude, (ExistingWayPointsBox2.SelectedItem as Waypoint).Longitude, false, "", "", "", ""));
            officeListBox2.ItemsSource = null;
            officeListBox2.ItemsSource = EditedRouteWayPoints;
        }

        private void DeleteItemFromEditedRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Waypoint waypoint in EditedRouteWayPoints)
            {
                if (((string)((sender as Button).Tag)) == waypoint.StringRep)
                {
                    EditedRouteWayPoints.Remove(waypoint);
                    officeListBox2.ItemsSource = null;
                    officeListBox2.ItemsSource = EditedRouteWayPoints;
                    break;
                }
            }
        }

        private async void SaveEditedRoute_Button_Click(object sender, RoutedEventArgs e)
        {
            if (EditedRouteWayPoints.Count == 0 || RouteName2.Text == "" || EditedRouteWayPoints.Count < 2)
                return;
            if (CurrentRoute.SelectedItem != null)
                return;
            var currentLanguage = User.Controller.Control.GetInstance().LanguageManager.CurrentLanguage;
            MessageDialog msgDialog = new MessageDialog("bleh");
            if (currentLanguage == User.Controller.Language.English)
            {
                msgDialog.Content = "Please wait a second, while we take a few seconds to set things up for you.";
                msgDialog.Title = "Please be patient!";
            }
            else if (currentLanguage == User.Controller.Language.Dutch)
            {
                msgDialog.Content = "Graag een moment geduld, terwijl alles in gereedheid wordt gebracht";
                msgDialog.Title = "Een moment geduld aub!";
            }
            IAsyncOperation<IUICommand> asyncCommand = msgDialog.ShowAsync();  //No need to wait for this.

            Route oldRoute = await GTec.User.Controller.DatabaseConnector.INSTANCE.GetRouteAsync(CurrentRoute2.SelectedItem as string);      

            List<Waypoint> waypoints = new List<Waypoint>();
            foreach (PointOfInterest poi in EditedRouteWayPoints)
            {
                if(poi.Name == String.Empty)
                    waypoints.Add(poi as Waypoint);
                else
                    waypoints.Add(poi);
            }
            Route newRoute = new Route(RouteName2.Text, "", waypoints);

            await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.EditRouteAsync(oldRoute.Name, newRoute);
            asyncCommand.Cancel();
        }

        private void EditedRouteExistingWayPointsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Waypoint waypoint = ExistingWayPointsBox2.SelectedItem as Waypoint;
            if (waypoint is PointOfInterest)
            {
                PointOfInterest poi = waypoint as PointOfInterest;
                Windows.UI.Xaml.Media.Imaging.BitmapImage icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                try
                {
                    icon.UriSource = new Uri(poi.ImagePath);
                    ImageExistingWayPoint2.Source = icon;
                }
                catch { }
                LatitudeExistingWayPoint2.Text = poi.Latitude.ToString();
                LongitudeExistingWayPoint2.Text = poi.Longitude.ToString();
                NameExistingWayPoint2.Text = poi.Name;
                Text2.Text = poi.Information;
                ImagePathExistingWayPoint2.Text = poi.ImagePath.ToString();

            }
            else
            {
                ImageExistingWayPoint2.Source = null;
                LatitudeExistingWayPoint2.Text = waypoint.Latitude.ToString();
                LongitudeExistingWayPoint2.Text = waypoint.Longitude.ToString();
                NameExistingWayPoint2.Text = "";
                ImagePathExistingWayPoint2.Text = "";
            }
        }

        private async void CurrentRoute2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Route toEdit = await User.Controller.Control.GetInstance().DatabaseConnnector.GetRouteAsync(CurrentRoute2.SelectedItem as string);
            EditedRouteWayPoints = toEdit.WayPoints;
            RouteName2.Text = toEdit.Name;
            officeListBox2.ItemsSource = null;
            officeListBox2.ItemsSource = EditedRouteWayPoints;
        }

        private async void RemoveWaypointButton_Click(object sender, RoutedEventArgs e)
        {
            await GTec.User.Controller.Control.GetInstance().DatabaseConnnector.DeleteVisitedRoute();

            foreach (Waypoint wp in GTec.User.Controller.Control.GetInstance().CurrentRoute.WayPoints)
                wp.Visited = false;

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
