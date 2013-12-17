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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GTec.View
{
    public sealed partial class AuthenticationFlyout : SettingsFlyout
    {
        public AuthenticationFlyout()
        {
            this.InitializeComponent();
        }

        private void OKLoginButton_Click(object sender, RoutedEventArgs e)
        {
            Tuple<string, string> authentication = new Tuple<string, string>(UsernameText.Text, PasswordText.Text);
            //Check for accountzz and give user access to database
        }
    }
}
