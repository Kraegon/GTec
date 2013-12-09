using GTec.View;

namespace Model
{
    public class Waypoint : BaseClassForBindableProperties
    {
        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private double latitude;
        private double longitude;
        private bool visited;

        /// <summary>
        /// The properties which you can bind to.
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
            set
            {
                if (latitude == value) return;
                latitude = value;
                OnPropertyChanged("Latitude");
            }
        }
        public double Longitude
        {
            get { return longitude; }
            set
            {
                if (longitude == value) return;
                longitude = value;
                OnPropertyChanged("Longitude");
            }
        }
        public bool Visited
        {
            get { return visited; }
            set
            {
                if (visited == value) return;
                visited = value;
                OnPropertyChanged("Visited");
            }
        }

        public Waypoint(double Latitude, double longitude, bool Visited)
        {
            this.latitude = Latitude;
            this.longitude = longitude;
            this.visited = Visited;
        }

        public override string ToString()
        {
            return "Latitude = " + latitude + ", " + "Longitude = " + longitude;
        }
    }
}
