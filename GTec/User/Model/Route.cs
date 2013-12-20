using GTec.User.View;
using System.Collections.Generic;

namespace GTec.User.Model
{
    public class Route : BaseClassForBindableProperties
    {
        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private string name;
        private string systemSoundPath;
        private List<Waypoint> wayPoints;

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
        public string SystemSoundPath
        {
            get { return systemSoundPath; }
            set
            {
                if (systemSoundPath == value) return;
                systemSoundPath = value;
                OnPropertyChanged("SystemSoundPath");
            }
        }
        public List<Waypoint> WayPoints
        {
            get { return wayPoints; }
            set
            {
                if (wayPoints == value) return;
                wayPoints = value;
                OnPropertyChanged("WayPoints");
            }
        }

        public Route(string Name, string SystemSoundPath, List<Waypoint> waypoints)
        {
            this.name = Name;
            this.systemSoundPath = SystemSoundPath;
            this.wayPoints = waypoints;
        }

        public Route() { wayPoints = new List<Waypoint>(); }
    }
}
