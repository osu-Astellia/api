using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AstelliaAPI
{
    public class ConfigScheme
    {
        public string Database;
        public string Username;
        public string Password;
        public string APIKey;
        public string AvatarPath;
        public string UpdaterPath;
        public string RecaptchaPrivate;
    }
    public class Config
    {
        public static ConfigScheme Get()
        {
            if (!File.Exists("config.json"))
            {
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
            }
            return JsonConvert.DeserializeObject<ConfigScheme>(File.ReadAllText("config.json"));
        }
    }
}