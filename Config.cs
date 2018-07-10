using Newtonsoft.Json;
using System.IO;

namespace DiscordArithmeticBot
{
    public struct BotConfig
    {
        #region Public Fields

        public string cmdPrefix;
        public string token;

        #endregion Public Fields
    }

    internal class Config
    {
        #region Public Fields

        public static BotConfig bot;

        #endregion Public Fields

        #region Private Fields

        private const string configFile = "config.json";
        private const string configFolder = "Resources";

        #endregion Private Fields

        #region Public Constructors

        static Config()
        {
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);
            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        #endregion Public Constructors
    }
}