using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Route
    {
        public string Name;
        public string SystemSoundPath;
        public WayPoint[] wayPoints;

        public Route(string Name, string SystemSoundPath, WayPoint[] wayPoints)
        {
            this.Name = Name;
            this.SystemSoundPath = SystemSoundPath;
            this.wayPoints = wayPoints;
        }
    }
}
