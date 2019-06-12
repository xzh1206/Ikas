﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ClassLib
{
    public static class FileFolderUrl
    {
        #region File

        public const string UserConfiguration = @"\user.ini";
        public const string SystemConfiguration = @"\config.ini";

        public const string UserConfigurationGeneralSection = "Ikas";
        public const string UserConfigurationCookie = "Cookie";
        public const string SystemConfigurationGeneralSection = "Ikas";
        public const string SystemConfigurationLanguage = "Language";
        public const string SystemConfigurationNetworkSection = "Network";
        public const string SystemConfigurationUseProxy = "UseProxy";
        public const string SystemConfigurationUseProxyHost = "UseProxyHost";
        public const string SystemConfigurationUseProxyPort = "UseProxyPort";

        #endregion

        #region Folder

        public static string ApplicationData
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetEntryAssembly().GetName().Name;
            }
        }

        /// <summary>
        /// Get the folder of a file.
        /// </summary>
        /// <param name="file">Full path to the file</param>
        /// <returns></returns>
        public static string GetFolder(string file)
        {
            string newFile = file.Replace('/', '\\');
            return newFile.Substring(0, newFile.Length - newFile.LastIndexOf(@"\") + 1);
        }

        #endregion

        #region URL

        public const string SplatNet = @"https://app.splatoon2.nintendo.net";
        public const string SplatNetScheduleApi = @"/api/schedules";
        public const string SplatNetBattleApi = @"/api/results/{}";

        #endregion
    }
}
