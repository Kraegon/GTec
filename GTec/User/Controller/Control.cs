using GTec.User.Model;
using GTec.User.View;
using System.Collections.Generic;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml;

namespace GTec.User.Controller
{
    public class Control : BaseClassForBindableProperties
    {
        private static Control Instance;

        public static Control GetInstance()
        {
            if (Instance == null)
                Instance = new Control();
            return Instance;
        }

        public Control()
        { }

        /// <summary>
        /// Available for the UIThread to make properties update themselves on bind.
        /// </summary>
        public List<UIElement> ThreadsToNotify = new List<UIElement>();

        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private Controller.LocationServiceProviderCaller locationProvider;
        private DatabaseConnector databaseConnector = DatabaseConnector.INSTANCE;
        private List<PointOfInterest> pointOfInterestList = new List<PointOfInterest>();
        private LanguageManager languageManager = new LanguageManager();
        private Route currentRoute;

        /// <summary>
        /// The properties which you can bind to.
        /// </summary>
        public Controller.LocationServiceProviderCaller LocationProvider
        {
            get { if (locationProvider == null) locationProvider = new LocationServiceProviderCaller(ThreadsToNotify); return locationProvider; }
            set
            {
                if (locationProvider == value) return;
                locationProvider = value;
                OnPropertyChanged("LocationProvider");
            }
        }
        public DatabaseConnector DatabaseConnnector
        {
            get { return databaseConnector; }
            set
            {
                if (databaseConnector == value) return;
                databaseConnector = value;
                OnPropertyChanged("DatabaseConnnector");
            }
        }
        public Route CurrentRoute
        {
            get { return currentRoute; }
            set
            {
                if (currentRoute == value) return;

                if(currentRoute != null)
                    if(currentRoute.Name != value.Name)
                        GeofenceMonitor.Current.Geofences.Clear();

                currentRoute = value;
                OnPropertyChanged("CurrentRoute");


            }
        }

        public LanguageManager LanguageManager
        {
            get { return languageManager; }
            set
            {
                if (languageManager == value) return;
                languageManager = value;
                OnPropertyChanged("LanguageManager");
            }
        }
    }
}
