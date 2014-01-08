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
            //if (User.Controller.Control.GetInstance().DatabaseConnnector.CurrentUser != null)
            //{
                //PasswordText.IsEnabled = false;
                //UsernameText.IsEnabled = false;
                //OKLoginButton.IsEnabled = false;
                //MessageTextBlock.Text = "Already logged in as " + User.Controller.Control.GetInstance().DatabaseConnnector.CurrentUser.Gebruikersnaam + "!";
            //}
            loadStrings();
        }

        private async void loadStrings()
        {
            UserNameTextTextBlock.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("UserNameInlogScreen");
            PassWordTextTextBlock.Text = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("PasswordInlogScreen");
            AuthenticationFlyoutText.Title = await GTec.User.Controller.Control.GetInstance().LanguageManager.GetTextAsync("AuthenticationTitle");
        }

        private async void OKLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var currentLanguage = User.Controller.Control.GetInstance().LanguageManager.CurrentLanguage;
            Tuple<string, string> authentication = new Tuple<string, string>(UsernameText.Text, PasswordText.Password);
            List<User.Model.Account> accounts = await User.Controller.DatabaseConnector.INSTANCE.GetAccountsAsync();
            foreach (User.Model.Account a in accounts)
            {
                if ((authentication.Item1 == a.Gebruikersnaam) && (authentication.Item2 == a.Password))
                {
                    if (currentLanguage == User.Controller.Language.English)
                        await new MessageDialog("Succesfully logged in as " + a.Gebruikersnaam + "!").ShowAsync();
                    else if (currentLanguage == User.Controller.Language.Dutch)
                        await new MessageDialog("Succesvol ingelogd als " + a.Gebruikersnaam + "!").ShowAsync();
                    this.Hide();
                    (GTec.User.Controller.Control.GetInstance().ThreadsToNotify[0] as User.View.MainPage).Frame.Navigate(typeof(AdminPage));
                    return;
                }
            }
            if (currentLanguage == User.Controller.Language.English)
                MessageTextBlock.Text = "Incorrect credentials!";
            else if (currentLanguage == User.Controller.Language.Dutch)
                MessageTextBlock.Text = "Verkeerd Wachtwoord of Gebruikersnaam!";
        }
    }
}
