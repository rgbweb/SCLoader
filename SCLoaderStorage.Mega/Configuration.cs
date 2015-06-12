using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Mega
{
    class Configuration : ConfigurationSection
    {

        [ConfigurationProperty("Email", DefaultValue = "", IsRequired = true)]
        public string Email
        {
            get { return (string)this["Email"]; }
            set { this["Email"] = value; }
        }

        [ConfigurationProperty("Password", DefaultValue = "", IsRequired = true)]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("TrackListFilePath", DefaultValue = "/", IsRequired = false)]
        public string TrackListFilePath
        {
            get { return (string)this["TrackListFilePath"]; }
            set { this["TrackListFilePath"] = value; }
        }

        [ConfigurationProperty("InstanceLockFilePath", DefaultValue = "/", IsRequired = false)]
        public string InstanceLockFilePath
        {
            get { return (string)this["InstanceLockFilePath"]; }
            set { this["InstanceLockFilePath"] = value; }
        }

        [ConfigurationProperty("Mp3TargetPath", DefaultValue = "/Downloads/", IsRequired = false)]
        public string Mp3TargetPath
        {
            get { return (string)this["Mp3TargetPath"]; }
            set { this["Mp3TargetPath"] = value; }
        }

    }
}
