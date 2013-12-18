﻿using System;
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
       
        //Attributes
        public Account CurrentUser; //Neccessary? Do we do this here?
        SQLiteAsyncConnection Database;

        public DatabaseConnector()
        {
            ConnectionInit();
        }
        /// <summary>
        /// Initialise the Database connection and creates tables/default values if not existant.
        /// </summary>
        public async void ConnectionInit()
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

            //For debug purposes
            CurrentUser = GetAccountsAsync().Result[0];
        }

        public async Task<List<string>> GetRouteNamesAsync()
        {
            List<DatabaseRoute> result = await Database.QueryAsync<DatabaseRoute>("SELECT Name FROM DatabaseRoute");
            return result.Select(x => x.Name).ToList();
        }

        private async Task<List<Waypoint>> GetAssociatedWaypointsAsync(string routeName)
        {
            int routeID = Database.ExecuteScalarAsync<int>("SELECT \"RouteID\" FROM \"DatabaseRoute\" WHERE \"Name\" = ?", new object[]{ routeName }).Result;
            List<RouteBind> waypointsID = await Database.QueryAsync<RouteBind>("SELECT \"WaypointID\" FROM \"RouteBinds\" WHERE \"RouteID\" = ?", new object[] {routeID});
            List<Waypoint> retVal = new List<Waypoint>();
            foreach(RouteBind r in waypointsID)
            {
                List<DatabasePOI> temp = await Database.QueryAsync<DatabasePOI>("SELECT * FROM DatabasePOI WHERE WaypointID = ?", new object[]{ r.WaypointID });
                retVal.Add(DatabasePOI.ToWaypoint(temp[0]));
            }
            return retVal;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            return await Database.QueryAsync<Account>("SELECT * FROM Account"); //Why can't everything be like this. Jeez.
        }

        public Route GetRoute(String routeName) 
        {
            return DatabaseRoute.ToRoute(Database.QueryAsync<DatabaseRoute>("SELECT * FROM DatabaseRoute WHERE Name = ?", new object[] { routeName }).Result[0]);
        }

        public async Task<bool> SaveRouteAsync(Route route)
        {
            bool IsSuccesful = true;
            //If the name is used, disregard
            List<int> IsTaken = await Database.QueryAsync<int>("SELECT RouteID FROM DatabaseRoute WHERE Name = ?", new object[] { route.Name });
            if (IsTaken[0] == 0)
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

        public async Task DeleteRouteAsync(Route route)
        {
            await DeleteRouteAsync(route.Name);
        }

        public async Task DeleteRouteAsync(string routeName)
        {
            int routeID = await Database.ExecuteAsync("SELECT \"RouteID\" FROM \"DatabaseRoute\" WHERE \"Name\" = ?", new object[] { routeName });
            await Database.ExecuteAsync("DELETE FROM DatabaseRoute WHERE Name = ?", new object[] { routeName }); //WAYPOINTS PERSIST
            await Database.ExecuteAsync("DELETE FROM RouteBindings WHERE RouteID = ?", new object[] { routeID }); //Destroy bindings, even though the waypoints are still there.
        }

        private async Task<int> SaveWaypoint(Waypoint waypoint)
        {
            //If the exact coordinates are already used, disregard.
            Waypoint exists = await Database.ExecuteScalarAsync<Waypoint>("SELECT \"Latitude\", \"Longitude\" FROM \"DatabasePOI\" WHERE \"Latitude\" = ? AND \"Longitude\" = ?", new object[] { waypoint.Latitude, waypoint.Longitude});
            if(exists == null)
                return 0;
            DatabasePOI forDatabase;
            //Otherwises insert as Point Of Interest (Contains metadata)
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
        private int generateRouteID()
        {
            //Dirty. But atleast it's safe.
            int retVal;
            if (Database.ExecuteScalarAsync<int>("SELECT * FROM DatabaseRoute", new object[] { }) == null)
                retVal = 1;
            else
                retVal = Database.ExecuteScalarAsync<int>("SELECT RouteID FROM DatabaseRoute ORDER BY RouteID DESC").Result + 1;           
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
            public static Route ToRoute(DatabaseRoute toConvert)
            {
                return new Route(toConvert.Name, toConvert.SystemSoundPath, DatabaseConnector.INSTANCE.GetAssociatedWaypointsAsync(toConvert.Name).Result);
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
            public DatabasePOI(double latitude, double longitude, int waypointID, string name, string information, string imagePath)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.WaypointID = waypointID;
                this.Name = name;
                this.Information = information;
                this.ImagePath = imagePath;
            }
            public static DatabasePOI ToDatabasePOI(Waypoint toConvert)
            {
                return new DatabasePOI(toConvert.Latitude, toConvert.Longitude, DatabaseConnector.INSTANCE.generateWaypointID());              
            }
            public static DatabasePOI ToDatabasePOI(PointOfInterest toConvert)
            {
                return new DatabasePOI(toConvert.Latitude, toConvert.Longitude, DatabaseConnector.INSTANCE.generateWaypointID(),
                                        toConvert.Name, toConvert.Information, toConvert.ImagePath);
            }
            public static Waypoint ToWaypoint(DatabasePOI toConvert)
            {
                if (toConvert.Name != null)
                    return new PointOfInterest(toConvert.Latitude, toConvert.Longitude, false,
                                               toConvert.Name, toConvert.Information, toConvert.ImagePath);
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
