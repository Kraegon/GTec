using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.User.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : Page
    {
        public string userScreenString
        {
            get
            {
                return userScreenString;
            }
            set
            {
                //setUserScreenString();
            }
        }

        public HelpPage()
        {
            this.InitializeComponent();
            setUserScreenString();
        }

        private void goBackward_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        public async void setUserScreenString()
        {
            userScreenString = await Controller.Control.GetInstance().LanguageManager.GetTextAsync("UserScreenHelpButton");
        }
    }
}
