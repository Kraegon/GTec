using GTec.View;

namespace Model
{
    public class Account : BaseClassForBindableProperties
    {
        /// <summary>
        /// The backing for the properties.
        /// </summary>
        private string gebruikersnaam;
        private string password;

        /// <summary>
        /// The properties which you can bind to.
        /// </summary>
        public string Gebruikersnaam
        {
            get { return gebruikersnaam; }
            set
            {
                if (gebruikersnaam == value) return;
                gebruikersnaam = value;
                OnPropertyChanged("Gebruikersnaam");
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                if (password == value) return;
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public Account(string gebruikersnaam, string password)
        {
            this.gebruikersnaam = gebruikersnaam;
            this.password = password;
        }
    }
}
