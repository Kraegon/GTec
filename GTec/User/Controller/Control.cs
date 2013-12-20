using GTec.User.View;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace GTec.User.Controller
{
    public class Control : BaseClassForBindableProperties
    {
        private static Control Instance = new Control();

        public static Control GetInstance()
        {
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
        private Controller.LocationServiceProviderCaller locationProvider = new LocationServiceProviderCaller();
        private DatabaseConnector databaseConnector = new DatabaseConnector();

        /// <summary>
        /// The properties which you can bind to.
        /// </summary>
        public Controller.LocationServiceProviderCaller LocationProvider
        {
            get { return locationProvider; }
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
    }
}
