using GTec.View;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace GTec.Controller
{
    public class Control : BaseClassForBindableProperties
    {
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
    }
}
