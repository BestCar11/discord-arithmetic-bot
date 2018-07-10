using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiscordArithmeticBot
{
    internal class Utilities
    {
        #region Private Fields

        private static Dictionary<string, string> alerts;

        #endregion Private Fields

        #region Public Constructors

        static Utilities()
        {
            string json = File.ReadAllText("SystemLang/alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        #endregion Public Constructors

        #region Public Methods

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key))
            {
                return alerts[key];
            }
            return "";
        }

        #endregion Public Methods
    }
}