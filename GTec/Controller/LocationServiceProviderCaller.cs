using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.System.Threading;

namespace Controller
{
    public class LocationServiceProviderCaller
    {
        public class Location
        {
            public double Latitude;
            public double Longitude;

            public Location(double Latitude, double Longitude)
            {
                this.Latitude = Latitude;
                this.Longitude = Longitude;
            }

            public override string ToString()
            {
                return "Latitude = " + Latitude + ", " + "Longitude = " + Longitude;
            }
        }

        public static Location currentLocation = new Location(666.666, 777.777);
        private Geolocator geoLocation = new Geolocator();
        private TimeSpan delay = TimeSpan.FromSeconds(1);
        private bool isRequestingLocation = true;

        public LocationServiceProviderCaller()
        {
            //geoLocation.DesiredAccuracyInMeters = 1;
            geoLocation.DesiredAccuracy = PositionAccuracy.High;
            getLocation();

            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                async (workItem) =>
                {
                    while (true)
                    {
                        if(isRequestingLocation)
                            getLocation();

                        await Task.Delay(delay);

                        System.Diagnostics.Debug.WriteLine(currentLocation);
                    }
                });
        }

        public async void getLocation()
        {
            try
            {
                Geoposition position = await geoLocation.GetGeopositionAsync();

                lock (currentLocation)
                {
                    currentLocation.Latitude = position.Coordinate.Point.Position.Latitude;
                    currentLocation.Longitude = position.Coordinate.Point.Position.Longitude;
                }
            }
            catch
            {
                isRequestingLocation = false;
            }
        }
    }
}
