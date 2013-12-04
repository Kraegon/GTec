using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Account
    {
        public string gebruikersnaam;
        public string password;

        public Account(string gebruikersnaam, string password)
        {
            this.gebruikersnaam = gebruikersnaam;
            this.password = password;
        }
    }
}
