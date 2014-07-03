using Meteor.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

/*
 * File : Configuration
 * Author : Filipe
 * Date : 06/02/2014 10:42:59
 * Description :
 *
 */

namespace Meteor.Source
{
    public static class Configuration
    {
        #region FIELDS

        private static readonly Byte MainVersion = Get<Byte>("MainVersion");
        private static readonly Byte MajorVersion = Get<Byte>("MajorVersion");
        private static readonly UInt16 MinorVersion = Get<UInt16>("MinorVersion");

        /// <summary>
        /// Client current version
        /// </summary>
        public static UInt32 CurrentVersion
        {
            get { return (UInt32)(MainVersion << 24 | MajorVersion << 16 | MinorVersion); }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Get configuration value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="name">Key</param>
        /// <returns></returns>
        public static T Get<T>(String name)
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[name], typeof(T));
        }

        #endregion
    }
}
