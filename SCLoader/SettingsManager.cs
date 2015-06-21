using SCLoader.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCLoader
{
    class SettingsManager
    {

        // Settings load order:
        // 1. Try to find a configuration file named "[ExecutingAssembly].custom.config" and use this
        // 2. Look for an entry in the "AppSettings" section which is possibly inserted by a third party (i.e. AppHarbor)
        // 3. Use the default ApplicationSettings of the default App.config file (preferred way)

        public SettingsManager()
        {

            var customConfigFileName = Assembly.GetExecutingAssembly().Location + ".custom.config";
            if (File.Exists(customConfigFileName))
            {
                // Use the custom configuration file
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", customConfigFileName);
            }

        }

        public dynamic GetSetting(string name)
        {

            // Get the preferred setting from ApplicationSettings
            var applicationSetting = GetApplictionSetting(name);

            // Check if there is a AppSettings value which overwrites the ApplicatioNSettings value
            var appSetting = GetAppSetting(name);
            if (appSetting != null)
            {
                // Convert to the known type of the ApplicationSetting if this is not unknown
                return Convert.ChangeType(appSetting, applicationSetting.GetType());
            }

            return applicationSetting;

        }

        private string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        private dynamic GetApplictionSetting(string name)
        {

            try
            {
                return Settings.Default[name];
            }
            catch (Exception)
            {
                // Setting not found
                return null;
            }

        }

    }
}
