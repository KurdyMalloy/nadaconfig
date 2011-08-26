using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace NadaConfigService
{

    public sealed class ConfigSection
    {
        private static volatile ConfigSection instance;
        private static object syncRoot = new Object();

        private FileInfo FInfo;
        private DateTime LastConfigWriteTime;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> configDictionary;

        private ConfigSection() { }

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetDictionary()
        {
            if (configDictionary == null)
            {
                FInfo = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                XDocument mydoc = XDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                // The stringcomparer is to make the keys in the dictionary case insensitive.
                configDictionary = mydoc.Descendants("Environment").ToDictionary(e => e.Attribute("name").Value, e => e.Descendants("Section").ToDictionary(t => t.Attribute("name").Value, t => t.Descendants("Item").ToDictionary(i => i.Attribute("name").Value, i => i.Attribute("value").Value, StringComparer.InvariantCultureIgnoreCase), StringComparer.InvariantCultureIgnoreCase), StringComparer.InvariantCultureIgnoreCase);
                LastConfigWriteTime = FInfo.LastWriteTime;
            }
            else
            {
                FInfo.Refresh();
                if (FInfo.LastWriteTime != LastConfigWriteTime)
                {
                    XDocument mydoc = XDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    // The stringcomparer is to make the keys in the dictionary case insensitive.
                    configDictionary = mydoc.Descendants("Environment").ToDictionary(e => e.Attribute("name").Value, e => e.Descendants("Section").ToDictionary(t => t.Attribute("name").Value, t => t.Descendants("Item").ToDictionary(i => i.Attribute("name").Value, i => i.Attribute("value").Value, StringComparer.InvariantCultureIgnoreCase), StringComparer.InvariantCultureIgnoreCase), StringComparer.InvariantCultureIgnoreCase);
                    LastConfigWriteTime = FInfo.LastWriteTime;
                }
            }

            return configDictionary;
        }

        public List<string> GetEnvironments()
        {
            return new List<string>(GetDictionary().Keys);
        }

        public static ConfigSection Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ConfigSection();
                    }
                }

                return instance;
            }
        }
    }

    
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class NadaConfigService : INadaConfigService
    {
        //public string GetData(int value)
        //{
        //    return string.Format("You entered: {0}", value);
        //}

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}

        public Dictionary<string, Dictionary<string, string>> GetEnvironment(string env)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(@"Service GetEnvironment({0})", env)); 
            return ConfigSection.Instance.GetDictionary()[env];
        }

        public Dictionary<string, string> GetSection(string env, string section)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(@"Service GetSection({0}, {1})", env, section));
            try
            {
                return ConfigSection.Instance.GetDictionary()[env][section];
            }
            catch
            {
                return null;
            }

        }

        public string GetConfigItem(string env, string section, string item)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(@"Service GetConfigItem({0}, {1}, {2})", env, section, item)); 
            return ConfigSection.Instance.GetDictionary()[env][section][item];
        }

    }
}
