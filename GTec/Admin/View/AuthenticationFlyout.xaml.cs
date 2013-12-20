using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GTec.Admin.View
{
    public sealed partial class AuthenticationFlyout : SettingsFlyout
    {
        public AuthenticationFlyout()
        {
            this.InitializeComponent();
        }

        private async void OKLoginButton_Click(object sender, RoutedEventArgs e)
        {
            Tuple<string, string> authentication = new Tuple<string, string>(UsernameText.Text, PasswordText.Password);
            List<User.Model.Account> accounts = await User.Controller.DatabaseConnector.INSTANCE.GetAccountsAsync();
            foreach (User.Model.Account a in accounts)
            {
                if (a.Gebruikersnaam.Equals(authentication.Item1) && a.Password.Equals(authentication.Item2))
                {
                    User.Controller.Control.GetInstance().DatabaseConnnector.CurrentUser = a;
                    await new MessageDialog("Succesfully logged in as " + a.Gebruikersnaam + "!").ShowAsync();
                    this.Hide();
                    return;
                }
            }
            MessageTextBlock.Text = "Incorrect credentials!";
        }
    }
}
