using GTec.User.View;

namespace GTec.User.Model
{
    public class PointOfInterest : Waypoint
    {
        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private string name;
        private string information;
        private string imagePath;        
        
        /// <summary>
        /// The properties which you can bind to.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Information
        {
            get { return information; }
            set
            {
                if (information == value) return;
                information = value;
                OnPropertyChanged("Information");
            }
        }
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if (imagePath == value) return;
                imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        public PointOfInterest(long Latitude, long Longitude, bool Visited, string Name, string Information, string ImagePath)
            : base(Latitude, Longitude, Visited)
        {
            this.name = Name;
            this.information = Information;
            this.imagePath = ImagePath;
        }
    }
}
