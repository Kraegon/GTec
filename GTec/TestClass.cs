using GTec.User.Controller;
using GTec.User.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTec
{
    class TestClass
    {
        public async static void RunTest()
        {
            //Testing procedure
            LanguageManager m = new LanguageManager();
            string s = await m.GetTextAsync("helpText1");
            m.CurrentLanguage = Language.English;
            string s2 = await m.GetTextAsync("helpText2");
        }
    }
}
