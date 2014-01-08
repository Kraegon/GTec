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
    public class DatabaseConnector
    {
        //Singleton
        private static DatabaseConnector instance;

        public static DatabaseConnector INSTANCE
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseConnector();
                }
                return instance;
            }
        }
       
        SQLiteAsyncConnection Database;

        private DatabaseConnector()
        {
            ConnectionInit();
        }
        /// <summary>
        /// Initialise the Database connection and creates tables/default values if not existant.
        /// </summary>
        private async void ConnectionInit()
        {
            //Connection init
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            Database = new SQLite.SQLiteAsyncConnection(dbPath);
            //Create tables if not exists
            await Database.CreateTablesAsync(new Account().GetType(), new DatabaseRoute().GetType(), new DatabasePOI().GetType());
            await Database.ExecuteAsync("create table if not exists \"RouteBinds\"(\"RouteID\" integer,\"WaypointID\" integer);", new object[] { });
            //Set default Admin Admin password
            var result = await Database.ExecuteScalarAsync<String>("Select Gebruikersnaam From Account WHERE Gebruikersnaam = ? AND Password = ?", new object[] { "Admin", "Admin" });
            if(result == null)
                await Database.InsertAsync(new Account("Admin", "Admin"));
        }

        public async Task<List<string>> GetRouteNamesAsync()
        {
            List<DatabaseRoute> result = await Database.QueryAsync<DatabaseRoute>("SELECT Name FROM DatabaseRoute WHERE RouteID <> 999");
            return result.Select(x => x.Name).ToList();
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            return await Database.QueryAsync<Account>("SELECT * FROM Account"); //Why can't everything be like this. Jeez.
        }

        public async Task<Route> GetRouteAsync(String routeName)
        {
            List<DatabaseRoute> results = await Database.QueryAsync<DatabaseRoute>("SELECT * FROM DatabaseRoute WHERE Name = ?", new object[] { routeName });
            if (results.Count == 0)
                return null;
            else
                return DatabaseRoute.ToRoute(results[0]);
        }

        public async Task<bool> SaveRouteAsync(Route route)
        {
            bool IsSuccesful = true;
            //If the name is used, disregard
            List<int> IsTaken = await Database.QueryAsync<int>("SELECT RouteID FROM DatabaseRoute WHERE Name = ?", new object[] { route.Name });
            if (IsTaken.Count != 0)
                return false; //IsSuccesful = false;
            //Otherwise insert it.
            DatabaseRoute forDatabase = DatabaseRoute.ToDatabaseRoute(route);
            await Database.InsertAsync(forDatabase);
            foreach (Waypoint w in route.WayPoints)
            {
                int existingID = await Database.ExecuteScalarAsync<int>("SELECT WaypointID FROM DatabasePOI WHERE Latitude = ? AND Longitude = ?", new object[] {w.Latitude, w.Longitude});
                if (existingID == 0)
                {
                    int waypointID = await SaveWaypoint(w);
                    await Database.ExecuteAsync("INSERT INTO RouteBinds VALUES(?, ?)", new object[] { forDatabase.RouteID, waypointID });
                }
                else
                {
                    await Database.ExecuteAsync("INSERT INTO RouteBinds VALUES(?, ?)", new object[] { forDatabase.RouteID, existingID });
                }
            }
            return IsSuccesful;
        }

        public async Task<bool> SaveCurrentRouteAsync(Route route)
        {
            bool IsSuccesful = true;
            DatabaseRoute forDatabase = DatabaseRoute.ToDatabaseRoute(route);
            await DeleteCurrentRoute();
            forDatabase.RouteID = 999; //Set to reserved ID
            await Database.InsertAsync(forDatabase);
            foreach (Waypoint w in route.WayPoints)
            {
                int existingID = await Database.ExecuteScalarAsync<int>("SELECT WaypointID FROM DatabasePOI WHERE Latitude = ? AND Longitude = ?", new object[] { w.Latitude, w.Longitude });
                await Database.ExecuteAsync("INSERT INTO RouteBinds VALUES(?, ?)", new object[] { forDatabase.RouteID, existingID });
            }
            return IsSuccesful;
        }

        public async Task<bool> SaveVisitedRouteAsync(Route route)
        {
            bool IsSuccesful = true;
            DatabaseRoute forDatabase = DatabaseRoute.ToDatabaseRoute(route);
            await DeleteVisitedRoute();
            forDatabase.RouteID = 1000; //Set to reserved ID
            await Database.InsertAsync(forDatabase);
            foreach (Waypoint w in route.WayPoints)
            {
                int existingID = await Database.ExecuteScalarAsync<int>("SELECT WaypointID FROM DatabasePOI WHERE Latitude = ? AND Longitude = ?", new object[] { w.Latitude, w.Longitude });
                await Database.ExecuteAsync("INSERT INTO RouteBinds VALUES(?, ?)", new object[] { forDatabase.RouteID, existingID });
            }
            return IsSuccesful;
        }

        public async Task<Route> GetCurrentRoute()
        {
            List<DatabaseRoute> results = await Database.QueryAsync<DatabaseRoute>("SELECT * FROM DatabaseRoute WHERE RouteID = 999");
            if (results.Count == 0)
                return null;
            else
                return DatabaseRoute.ToRoute(results[0]);
        }
        public async Task<Route> GetVisitedRoute()
        {
            List<DatabaseRoute> results = await Database.QueryAsync<DatabaseRoute>("SELECT * FROM DatabaseRoute WHERE RouteID = 1000");
            if (results.Count == 0)
                return null;
            else
                return DatabaseRoute.ToRoute(results[0]);
        }
        public async Task DeleteCurrentRoute()
        {
            await Database.ExecuteAsync("DELETE FROM DatabaseRoute WHERE RouteID = 999");
            await Database.ExecuteAsync("DELETE FROM RouteBinds WHERE RouteID = 999");
        }
        public async Task DeleteVisitedRoute()
        {
            await Database.ExecuteAsync("DELETE FROM DatabaseRoute WHERE RouteID = 1000");
            await Database.ExecuteAsync("DELETE FROM RouteBinds WHERE RouteID = 1000");
        }
        public async Task DeleteRouteAsync(Route route)
        {
            await DeleteRouteAsync(route.Name);
        }

        public async Task DeleteRouteAsync(string routeName)
        {
            List<DatabaseRoute> toDelete = Database.QueryAsync<DatabaseRoute>("SELECT \"RouteID\" FROM \"DatabaseRoute\" WHERE \"Name\" = ?", new object[] { routeName }).Result;
            await Database.ExecuteAsync("DELETE FROM DatabaseRoute WHERE Name = ? AND RouteID <> 999 AND RouteID <> 1000", new object[] { routeName }); //WAYPOINTS PERSIST            
            foreach(DatabaseRoute r in toDelete)
            {
                await Database.ExecuteAsync("DELETE FROM RouteBinds WHERE RouteID = ?", new object[] { r.RouteID }); //Destroy bindings, even though the waypoints are still there.
            }
        }
        public async Task DeleteWaypoint(Waypoint waypoint)
        {
            await DeleteWaypoint(waypoint.Latitude, waypoint.Longitude);
        }

        public async Task DeleteWaypoint(double latitude, double longitude)
        {
            List<DatabasePOI> toDelete = Database.QueryAsync<DatabasePOI>("SELECT \"Latitude\", \"Longitude\", \"WaypointID\" FROM \"DatabasePOI\" WHERE \"Latitude\" = ? AND \"Longitude\" = ?", new object[] { latitude, longitude }).Result;
            foreach (DatabasePOI w in toDelete)
            {
                await Database.ExecuteAsync("DELETE FROM DatabasePOI WHERE WaypointID = ?", new object[] { w.WaypointID });
            }
        }
        public async Task<List<Waypoint>> GetAllWaypoints()
        {
            List<DatabasePOI> raw = await Database.QueryAsync<DatabasePOI>("SELECT * FROM DatabasePOI");
            List<Waypoint> retVal = new List<Waypoint>();
            foreach (DatabasePOI w in raw)
            {
                if (w.Name != null)
                    retVal.Add(new PointOfInterest(w.Latitude, w.Longitude, false,
                                               w.Name, w.Information, w.ImagePath, w.SoundPath));
                else
                    retVal.Add(new Waypoint(w.Latitude, w.Longitude));
            }
            return retVal;
        }
        public async Task<int> SaveWaypoint(Waypoint waypoint)
        {
            //If the exact coordinates are already used, disregard.
            List<DatabasePOI> exists = await Database.QueryAsync<DatabasePOI>("SELECT \"Latitude\", \"Longitude\" FROM \"DatabasePOI\" WHERE \"Latitude\" = ? AND \"Longitude\" = ?", new object[] { waypoint.Latitude, waypoint.Longitude});
            if(exists.Count != 0)
                return exists[0].WaypointID;
            DatabasePOI forDatabase;
            //Otherwises insert as Point Of Interest (Contains at least a name)
            if(waypoint is PointOfInterest){
               forDatabase = DatabasePOI.ToDatabasePOI(waypoint as PointOfInterest);
            }
            //Or waypoint (Without metadata)
            else
            {
                forDatabase = DatabasePOI.ToDatabasePOI(waypoint);
            }
            await Database.InsertAsync(forDatabase);
            return forDatabase.WaypointID;
        }

        private async Task<List<Waypoint>> getAssociatedWaypointsAsync(string routeName)
        {
            int routeID = Database.ExecuteScalarAsync<int>("SELECT \"RouteID\" FROM \"DatabaseRoute\" WHERE \"Name\" = ? AND RouteID <> 999 AND RouteID <> 1000", new object[] { routeName }).Result;
            List<RouteBind> waypointsID = Database.QueryAsync<RouteBind>("SELECT \"WaypointID\" FROM \"RouteBinds\" WHERE \"RouteID\" = ?", new object[] { routeID }).Result;
            List<Waypoint> retVal = new List<Waypoint>();
            foreach (RouteBind r in waypointsID)
            {
                List<DatabasePOI> temp = Database.QueryAsync<DatabasePOI>("SELECT * FROM DatabasePOI WHERE WaypointID = ?", new object[] { r.WaypointID }).Result;
                retVal.Add(DatabasePOI.ToWaypoint(temp[0]));
            }
            return retVal;
        }

        public async Task EditRouteAsync(string oldRouteName, Route newRoute)
        {
            //Remember the Database ID of the route
            int id = Database.QueryAsync<DatabaseRoute>("SELECT * FROM \"DatabaseRoute\" WHERE \"Name\" = ? AND RouteID <> 999 AND RouteID <> 1000", new object[] { oldRouteName }).Result[0].RouteID;
            //And remove it
            await DeleteRouteAsync(oldRouteName);
            //Now create & insert the new route 
            DatabaseRoute forDatabase = DatabaseRoute.ToDatabaseRouteIDless(newRoute, id);
            await Database.InsertAsync(forDatabase);
            //Then bind the waypoints
            foreach (Waypoint newWaypoint in newRoute.WayPoints)
            {
                int existingID = await Database.ExecuteScalarAsync<int>("SELECT WaypointID FROM DatabasePOI WHERE Latitude = ? AND Longitude = ?", new object[] { newWaypoint.Latitude, newWaypoint.Longitude });
                if (existingID == 0)
                {
                    //In the case it's a new waypoint, save and bind to this route 
                    int waypointID = await SaveWaypoint(newWaypoint);
                    await Database.ExecuteAsync("INSERT INTO RouteBinds VALUES(?, ?)", new object[] { forDatabase.RouteID, waypointID });
                }
                else
                {
                    //But if it exists, edit waypoint if neccesary, check if the bind persisted and then insert
                    await EditWaypointAsync(DatabasePOI.ToWaypoint(
                        Database.QueryAsync<DatabasePOI>("SELECT * FROM DatabasePOI WHERE Latitude = ? AND Longitude = ?", new object[] { newWaypoint.Latitude, newWaypoint.Longitude }).Result[0])
                        , newWaypoint);
                    List<RouteBind> binds = await Database.QueryAsync<RouteBind>("SELECT WaypointID FROM RouteBinds WHERE RouteID = ? AND WaypointID = ?", new object[] { id, existingID });
                    if(binds.Count == 0)
                        await Database.ExecuteAsync("INSERT INTO RouteBinds VALUES(?, ?)", new object[] { forDatabase.RouteID, existingID });
                }
            }
            //In the case this is the active route, replace.
            Route currentRoute = await GetCurrentRoute();
            
            if (currentRoute != null && currentRoute.Name == oldRouteName)
                await SaveCurrentRouteAsync(newRoute);
        }

        public async Task EditWaypointAsync(Waypoint oldWaypoint, Waypoint newWaypoint)
        {
            //Remember the ID
            int id = Database.QueryAsync<DatabasePOI>("SELECT WaypointID FROM \"DatabasePOI\" WHERE \"Latitude\" = ? AND \"Longitude\" = ?", new object[] { oldWaypoint.Latitude, oldWaypoint.Longitude }).Result[0].WaypointID;
           
            await DeleteWaypoint(oldWaypoint);
            //If the exact coordinates are still in use, disregard. This shouldn't happen, but you'll never be certain
            List<DatabasePOI> exists = await Database.QueryAsync<DatabasePOI>("SELECT \"Latitude\", \"Longitude\" FROM \"DatabasePOI\" WHERE \"Latitude\" = ? AND \"Longitude\" = ?", new object[] { newWaypoint.Latitude, newWaypoint.Longitude });
            if (exists.Count != 0)
                return;
            DatabasePOI forDatabase;
            //Otherwises insert as Point Of Interest (Contains a name)
            if (newWaypoint is PointOfInterest)
            {
                forDatabase = DatabasePOI.ToDatabasePOIIDless(newWaypoint as PointOfInterest, id);
            }
            //Or waypoint (Without metadata)
            else
            {
                forDatabase = DatabasePOI.ToDatabasePOIIDless(newWaypoint, id);
            }
            await Database.InsertAsync(forDatabase);
        }

        private int generateRouteID()
        {
            //Dirty. But atleast it's safe.
            int retVal;
            if (Database.ExecuteScalarAsync<int>("SELECT * FROM DatabaseRoute", new object[] { }) == null)
                retVal = 1;
            else
                retVal = Database.ExecuteScalarAsync<int>("SELECT RouteID FROM DatabaseRoute WHERE (RouteID <> 999) AND (RouteID <> 1000) ORDER BY RouteID DESC").Result + 1;
            if ((retVal == 999) || (retVal == 1000))
                retVal = 1001;
            return retVal;
        }

        private int generateWaypointID()
        {
            //Still dirty.
            int retVal;
            if (Database.ExecuteScalarAsync<int>("SELECT * FROM DatabasePOI", new object[] { }) == null)
                retVal = 1;
            else
                retVal = Database.ExecuteScalarAsync<int>("SELECT WaypointID FROM DatabasePOI ORDER BY WaypointID DESC").Result + 1;
            return retVal; 
        }
        /// <summary>
        /// Database ready routes. Added an ID and removed waypoint array. Array is constructed using RouteBinds table.
        /// </summary>
        private class DatabaseRoute
        {
            public string Name { get; set; }
            public string SystemSoundPath { get; set; }
            public int RouteID { get; set; }

            public DatabaseRoute()
            {
                RouteID = -1;
            }
            public DatabaseRoute(string name, string systemSoundPath, int routeID)
            {
                this.Name = name;
                this.SystemSoundPath = systemSoundPath;
                this.RouteID = routeID;
            }
            public static DatabaseRoute ToDatabaseRoute(Route toConvert)
            {
                return new DatabaseRoute(toConvert.Name, toConvert.SystemSoundPath, DatabaseConnector.INSTANCE.generateRouteID());
            }
            public static DatabaseRoute ToDatabaseRouteIDless(Route toConvert, int id)
            {
                return new DatabaseRoute(toConvert.Name, toConvert.SystemSoundPath, id);
            }
            public static Route ToRoute(DatabaseRoute toConvert)
            {
                return new Route(toConvert.Name, toConvert.SystemSoundPath, DatabaseConnector.INSTANCE.getAssociatedWaypointsAsync(toConvert.Name).Result);
            }
        }
        /// <summary>
        /// Database ready waypoints & points of interest. Added ID.
        /// </summary>
        private class DatabasePOI
        {
            public double Latitude { get; set; }
            public double Longitude{get; set;}
            public int WaypointID{get; set;}
            public string Name{get; set;}
            public string Information{get; set;}
            public string ImagePath{get; set;}
            public string SoundPath { get; set; }

            public DatabasePOI()
            {
                WaypointID = -1;
            }
            public DatabasePOI(double latitude, double longitude, int waypointID)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.WaypointID = waypointID;
            }
            public DatabasePOI(double latitude, double longitude, int waypointID, string name, string information, string imagePath, string soundPath)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.WaypointID = waypointID;
                this.Name = name;
                this.Information = information;
                this.ImagePath = imagePath;
                this.SoundPath = soundPath;
            }
            public static DatabasePOI ToDatabasePOI(Waypoint toConvert)
            {
                return new DatabasePOI(toConvert.Latitude, toConvert.Longitude, DatabaseConnector.INSTANCE.generateWaypointID());              
            }
            public static DatabasePOI ToDatabasePOI(PointOfInterest toConvert)
            {
                return new DatabasePOI(toConvert.Latitude, toConvert.Longitude, DatabaseConnector.INSTANCE.generateWaypointID(),
                                        toConvert.Name, toConvert.Information, toConvert.ImagePath, toConvert.SoundPath);
            }
            public static DatabasePOI ToDatabasePOIIDless(Waypoint toConvert, int id)
            {
                return new DatabasePOI(toConvert.Latitude, toConvert.Longitude, id);
            }
            public static DatabasePOI ToDatabasePOIIDless(PointOfInterest toConvert, int id)
            {
                return new DatabasePOI(toConvert.Latitude, toConvert.Longitude, id,
                                        toConvert.Name, toConvert.Information, toConvert.ImagePath, toConvert.SoundPath);
            }
            public static Waypoint ToWaypoint(DatabasePOI toConvert)
            {
                if (toConvert.Name != null)
                    return new PointOfInterest(toConvert.Latitude, toConvert.Longitude, false,
                                               toConvert.Name, toConvert.Information, toConvert.ImagePath,toConvert.SoundPath);
                else
                    return new Waypoint(toConvert.Latitude, toConvert.Longitude);
            }
        }

        private class RouteBind
        {
            public int RouteID { get; set; }
            public int WaypointID { get; set; }
        }
    }
}
