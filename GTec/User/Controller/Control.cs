using GTec.User.View;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace GTec.User.Controller
{
    public class Control : BaseClassForBindableProperties
    {
        private static Control Instance = new Control();
        private static User.Model.Account CurrentAccount = null;

        public static Control GetInstance()
        {
            return Instance;
        }

        public Control()
        { }

        /// <summary>
        /// Available for the UIThread to make properties update themselves on bind.
        /// </summary>
        public static List<UIElement> ThreadsToNotify = new List<UIElement>();

        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private Controller.LocationServiceProviderCaller locationProvider = new LocationServiceProviderCaller();
            
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

        public static void SetCurrentAccount(User.Model.Account a)
        { CurrentAccount = a; }

        public static User.Model.Account GetCurrentAccount()
        { return CurrentAccount; }

        public static bool IsAdminLoggedIn()
        {
            if (CurrentAccount != null)
                return true;
            else
                return false;
        }
    }
}
