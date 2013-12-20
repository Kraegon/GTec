using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace GTec.User.Controller
{
    public class LanguageManager
    {
        private static JsonObject textCompilation;
        private static bool isInitialised = false;
        public Language CurrentLanguage = Language.Dutch;

        public LanguageManager()
        { 
            //Can't init asynchronously, init is checked in get method.
        }

        private async Task initAsync()
        {
            string rawText = String.Empty;
            //Three steps neccesary to subvert access shenanigans
            var installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var subfolder = await installFolder.GetFolderAsync("Assets\\strings\\");
            var file = await subfolder.GetFileAsync("LanguagePack.json");

            rawText = await FileIO.ReadTextAsync(file);                
            textCompilation = JsonObject.Parse(rawText)["stringRef"].GetObject();
            isInitialised = true;
        }
        public async Task<string> GetTextAsync(string tag)
        {
            if (!isInitialised)
                await initAsync();
            string retVal = String.Empty;
            switch (CurrentLanguage)
            {
                case Language.Dutch:
                    retVal = findByLanguage("nl", tag);
                    break;
                case Language.English:
                    retVal = findByLanguage("en", tag);
                    break;
            }
            return retVal;
        }
        private static string findByLanguage(string languageAbbreviation, string tag)
        {
            string retVal = String.Empty;
            JsonObject translations = textCompilation[tag].GetObject();
            try
            {
                retVal = translations[languageAbbreviation].GetString();
            }
            catch (KeyNotFoundException)
            {
                retVal = "error";
            }
            return retVal;
        }
    }
 
    public enum Language
    {
        English = 0,
        Dutch = 1
    }
}
