using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PointOfInterest : WayPoint
    {
        public string Name;
        public string Information;
        public string ImagePath;

        public PointOfInterest(long Latitude, long Longitude, bool Visited, string Name, string Information, string ImagePath)
            : base(Latitude, Longitude, Visited)
        {
            this.Name = Name;
            this.Information = Information;
            this.ImagePath = ImagePath;
        }
    }
}
