using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Local
{
    class Configuration : ConfigurationSection
    {

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
