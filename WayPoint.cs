using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WayPoint
    {
        public long Latitude;
        public long Longitude;
        public bool Visited;

        public WayPoint(long Latitude, long Longitude, bool Visited)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Visited = Visited;
        }
    }
}
