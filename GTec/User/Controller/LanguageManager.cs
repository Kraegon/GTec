using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTec.User.Controller
{
    class LanguageManager
    {
        public Language CurrentLanguage = Language.Dutch;
        public string Get(string tag)
        {
            switch (CurrentLanguage)
            {
                case Language.Dutch:
                    break;
                case Language.English:
                    break;
            }
            //JSon stuff
            return tag;
        }
    }

    public enum Language
    {
        Dutch = 1,
        English = 2
    }
}
