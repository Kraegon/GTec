using GTec.User.View;
using GTec.User.Model;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace GTec.User.Controller
{
    /// <summary>
    /// Updates the CurrentLocation and for convenience also a string version of this, CurrentLocationString. Both can be bound to.
    /// </summary>
    public class LocationServiceProviderCaller : BaseClassForBindableProperties
    {
        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private Waypoint currentLocation = new Waypoint(666.666, 777.777, true);
        private string currentLocationString = "Error";
        private bool isRequestingLocation = true;

        /// <summary>
        /// The properties which you can bind to.
        /// </summary>
        public Waypoint CurrentLocation
        {
            get { return currentLocation; }
            set
            {
                if (currentLocation == value) return;
                currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }
        public string CurrentLocationString
        {
            get { return currentLocationString; }
            set
            {
                if (currentLocationString == value) return;
                currentLocationString = value;
                OnPropertyChanged("CurrentLocationString");
            }
        }
        public bool IsRequestingLocation
        {
            get { return isRequestingLocation; }
            set
            {
                if (isRequestingLocation == value) return;
                isRequestingLocation = value;
                OnPropertyChanged("IsRequestingLocation");
            }
        }

        /// <summary>
        /// The required fields for this class.
        /// </summary>
        private Geolocator geoLocation = new Geolocator();
        private TimeSpan delay = TimeSpan.FromSeconds(1);

        public LocationServiceProviderCaller(System.Collections.Generic.List<UIElement> ThreadsToNotif)
        {
            geoLocation.DesiredAccuracy = PositionAccuracy.High;
            getConsent();
            //getLocation();
            //Task.Delay(1000);
            new TaskFactory().StartNew(updateLocationLoop);
        }

        private async void getConsent()
        {
            await ((UIElement)Control.GetInstance().ThreadsToNotify[0]).Dispatcher.RunAsync(CoreDispatcherPriority.High, () => getLocation());
        }

        private async void updateLocationLoop()
        {
            while (true)
            {
                if (isRequestingLocation)
                    getLocation();

                await Task.Delay(delay);
            }
        }
        private async void getLocation()
        {
            try
            {
                Geoposition position = await geoLocation.GetGeopositionAsync();

                currentLocation.Latitude = position.Coordinate.Point.Position.Latitude;
                currentLocation.Longitude = position.Coordinate.Point.Position.Longitude;
                CurrentLocationString = currentLocation.ToString();
            }
            catch
            {
                IsRequestingLocation = false;
            }
        }
    }
}
