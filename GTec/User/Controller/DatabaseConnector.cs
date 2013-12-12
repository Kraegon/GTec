using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTec.User.Model;
using SQLite;
using System.IO;

namespace GTec.User.Controller
{
    class DatabaseConnector
    {
        //Singleton
        public static DatabaseConnector INSTANCE;
        SQLiteAsyncConnection conn;
        //Attributes
        public Account CurrentUser;

        public static DatabaseConnector GetInstance()
        {
            if(INSTANCE == null)
            {
                INSTANCE = new DatabaseConnector();
            }
            return INSTANCE;
        }

        public bool ConnectionInit()
        {
            //SQLite business 
            return true;
        }

        public string[] GetRouteNames()
        {
            return new string[] { "Historische kilometer" };
        }

        public Waypoint[] GetWaypoints(string routeName)
        {
            return new Waypoint[] { new Waypoint(51, 4, false)};
        }

        public Account[] GetAccounts(){
            return new Account[]{new Account("Admin", "Admin")};
        }

        public Route GetRoute(String routeName) 
        {
            return new Route(routeName, "default/sys.wav", GetWaypoints(routeName));
        }

        private void ConnData()
        {
            //Als de database niet bestaat maakt hij die automatisch aan.
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            conn = new SQLite.SQLiteAsyncConnection(dbPath);

        }
    }
}
