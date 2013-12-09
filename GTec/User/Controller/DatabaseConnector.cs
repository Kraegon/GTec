using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTec.User.Model;

namespace GTec.User.Controller
{
    class DatabaseConnector
    {
        public Account CurrentUser;

        public string[] GetRouteNames()
        {
            return new string[] { "Historische kilometer" };
        }

        public Waypoint[] GetWaypoints(string routeName)
        {
            return new Waypoint[] { new Waypoint(51, 4, false)};
        }

        public Account GetAccounts(){
            //Probably gonna need revision
            return new Account("Admin", "Admin");
        }

        public Route GetRoute(String routeName) 
        {
            return new Route(routeName, "default/sys.wav", GetWaypoints(routeName));
        }
    }
}
