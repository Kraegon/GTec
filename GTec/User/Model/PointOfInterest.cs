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
        private string soundPath;
        
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
        public string SoundPath
        {
            get { return soundPath; }
            set
            {
                if (soundPath == value) return;
                soundPath = value;
                OnPropertyChanged("SoundPath");
            }
        }

        public PointOfInterest(double Latitude, double Longitude, bool Visited, string Name, string Information, string ImagePath, string SoundPath)
            : base(Latitude, Longitude, Visited)
        {
            this.name = Name;
            this.information = Information;
            this.imagePath = ImagePath;
            this.soundPath = SoundPath;
        }
    }
}
