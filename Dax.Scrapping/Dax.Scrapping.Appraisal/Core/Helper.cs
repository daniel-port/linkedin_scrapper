using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Dax.Scrapping.Appraisal.Core
{
    public static class Helper
    {
        static AppSettingsReader reader = new AppSettingsReader();
        static string _serverHub = null;


        public static string ServerHub { get { return _serverHub; } set {  _serverHub = value; } }

        /// <summary>
        /// read a config value as string 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettingAsString(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        /// <summary>
        /// read a config value as int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetAppSettingAsInt(string key)
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get(key));

        }


        /// <summary>
        /// Get the  signalr server url
        /// </summary>
        /// <returns></returns>
        public static string GetServerUrl()
        {
            var server = _serverHub?? GetAppSettingAsString("ServerUrl").Trim();

            if (!server.EndsWith("/"))
                server = server + "/";


            return server;
        }

        public static void Modify(string key, string value)
        {
            string appSettingsTag = "appSettings";
            //ar appSettings = (AppSettingsSection)ConfigurationManager.GetSection("appSettings");
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }


        public static string GetEnumDescription<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetFileContent(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            return  File.ReadAllText(path);
        }

        public static void WriteCSV<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join("| ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join("| ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }
    }
}
