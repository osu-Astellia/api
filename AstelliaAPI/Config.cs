using System.IO;
using Newtonsoft.Json;

namespace AstelliaAPI
{
    public class ConfigScheme
    {
        public string APIKey;
        public string AvatarPath;
        public string Database;
        public string Password;
        public string RecaptchaPrivate;
        public string UpdaterPath;
        public string Username;
    }

    public static class Config
    {
        public static ConfigScheme Get()
        {
            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new ConfigScheme
                {
                    Database = "ripple",
                    Username = "root",
                    Password = "",
                    AvatarPath = "",
                    UpdaterPath = "",
                    APIKey = "",
                    RecaptchaPrivate = ""
                }));
            return JsonConvert.DeserializeObject<ConfigScheme>(File.ReadAllText("config.json"));
        }
    }
}