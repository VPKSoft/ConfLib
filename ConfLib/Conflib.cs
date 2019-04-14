#region License
/*
ConfLib

A library for storing application setting into a SQLite database.
Copyright (C) 2015 VPKSoft, Petteri Kautonen

Contact: vpksoft@vpksoft.net

This file is part of ConfLib.

ConfLib is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ConfLib is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with ConfLib.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace VPKSoft.ConfLib
{
    /// <summary>
    /// A simple library for saving application configuration settings.
    /// </summary>
    public class Conflib
    {
        /// <summary>
        /// The directory to save the configuration into.
        /// </summary>
        private string dataDir;

        /// <summary>
        /// The file name to save the configuration into.
        /// </summary>
        private string dbName = "config.sqlite";

        /// <summary>
        /// If the configuration entries should be automatically saved to the database.
        /// </summary>
        private bool autoCreate = false;

        /// <summary>
        /// The SQlite database connection to use.
        /// </summary>
        private SQLiteConnection conn = null;

        /// <summary>
        /// An internal cache where the configuration entries are held.
        /// <para/>This is for reading only. Writing is done immediately depending
        /// <para/>on the caching of the file system and SQLite database.
        /// </summary>
        private List<KeyValuePair<string, string>> configCache = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Just returns the default writable data directory for "non-roaming" applications.
        /// </summary>
        /// <returns>A writable data directory for "non-roaming" applications.</returns>
        static string GetAppSettingsFolder()
        {
            string appName = Application.ProductName;
            foreach (char chr in Path.GetInvalidFileNameChars())
            {
                appName = appName.Replace(chr, '_');
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + appName + @"\";
        }

        /// <summary>
        /// The constructor.
        /// <para/>A writable data directory variable is initialized.
        /// </summary>
        public Conflib()
        {
            dataDir = GetAppSettingsFolder();
        }

        /// <summary>
        /// Encrypts a string starting with "SECURE:" to a hex string.
        /// </summary>
        /// <param name="data">A string to encypt.</param>
        /// <returns>A string encrypeted as hex string if it started with "SECURE:"</returns>
        public static string ProtectData(string data)        
        {
            if (data == null)
            {
                return null;
            }
            if (data.StartsWith("SECURE:"))
            {
                if (data == "SECURE:")
                {
                    return string.Empty;
                }
                else
                {
                    data = data.Remove(0, "SECURE:".Length);
                }
            }
            else
            {
                return data;
            }
            byte[] bData = UTF8Encoding.UTF8.GetBytes(data.ToCharArray());            
            bData = ProtectedData.Protect(bData, null, DataProtectionScope.CurrentUser);
            string hex = BitConverter.ToString(bData);
            return "0x" + hex.Replace("-", "");
        }

        /// <summary>
        /// Decrypts a string starting with "0x" to a normal string.
        /// </summary>
        /// <param name="data">A data to decrypt.</param>
        /// <returns>A string decrypeted from a hex string.</returns>
        public static string UnprotectData(string data)
        {
            if (data == null)
            {
                return null;
            }
            if (data.StartsWith("0x"))
            {
                if (data == "0x")
                {
                    return string.Empty;
                }
                else
                {
                    data = data.Remove(0, "0x".Length);
                }
            }
            else
            {
                return data;
            }
            data.Replace("0x", string.Empty);
            byte[] bData = StringToByteArray(data);
            bData = ProtectedData.Unprotect(bData, null, DataProtectionScope.CurrentUser);
            return System.Text.UTF8Encoding.UTF8.GetString(bData);
        }

        /// <summary>
        /// Convers a hex string to a a byte array.
        /// </summary>
        /// <param name="hex">A hex string to convert.</param>
        /// <returns>A byte array which has been converted from a hex string.</returns>
        public static byte[] StringToByteArray(String hex)
        {
            int nChars = hex.Length;
            byte[] bytes = new byte[nChars / 2];
            for (int i = 0; i < nChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// Gets or sets a writable directory where the settings should be saved.
        /// <para/>The default is "[...]\AppData\Local\[Assembly product name]."
        /// </summary>
        public string DataDir
        {
            get
            {
                return dataDir;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentException("Empty string is not allowed.");
                }

                dataDir = value;
                foreach (char chr in Path.GetInvalidPathChars())
                {
                    dataDir = dataDir.Replace(chr, '_');
                }

                dataDir = dataDir.EndsWith(@"\") ? dataDir : dataDir + @"\";
            }
        }

        /// <summary>
        /// Gets or sets the SQLite database file name residing in DataDir.
        /// <para/>The default is config.sqlite.
        /// </summary>
        public string DBName
        {
            get
            {
                return dbName;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentException("Empty string is not allowed.");
                }

                dbName = value;
                foreach (char chr in Path.GetInvalidFileNameChars())
                {
                    dbName = dbName.Replace(chr, '_');
                }
            }
        }

        /// <summary>
        /// Gets the combination of DataDir and DBName as a full file name and path.
        /// </summary>
        public string ConfigFile
        {
            get
            {
                return dataDir + dbName;
            }
        }

        /// <summary>
        /// Indicates if the library should automatically create settings
        /// <para/>in to the database whether their exist or not.
        /// <para/>This appends to querying or creating a setting.
        /// </summary>
        public bool AutoCreateSettings
        {
            get
            {
                return autoCreate;
            }
            
            set
            {
                autoCreate = value;
            }
        }

        /// <summary>
        /// Opens the database in case it hasn't been opened yet.
        /// <para/>This connection is used by this library.
        /// <para/>An explicit call is not required, because the library opens
        /// <para/>the connection if it's not open yet.
        /// </summary>
        public void Open()
        {
            if (conn == null)
            {
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
                conn = new SQLiteConnection("Data Source=" + ConfigFile + ";Pooling=true;FailIfMissing=false");
                conn.Open();
            }
        }

        /// <summary>
        /// Loads all the settings from the database from to the cache.
        /// </summary>
        public void LoadCache()
        {
            Open();
            configCache.Clear();
            List<string> tables = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                                      // SQLite strings are case sensitive (except in like and COLLATE NOCASE ?.)...
                command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ";
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        tables.Add(dr[0].ToString());
                    }
                }

                foreach (string tbl in tables)
                {
                    command.CommandText = "SELECT KEY, VALUE FROM [" + tbl + "] ";
                    using (SQLiteDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            configCache.Add(new KeyValuePair<string, string>(tbl + "/" + dr[0].ToString(), dr.IsDBNull(1) ? null : UnprotectData(dr[1].ToString())));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates if a given database table <paramref name="name"/> is valid.
        /// <para/>Only alphanumeric characters and the underscore character [_] are valid.
        /// </summary>
        /// <param name="name">A database table name to check.</param>
        /// <returns>True if the valdiation was successfull, otherwise false.</returns>
        private static bool ValidTableName(string name)
        {
            char[] tmp = name.ToCharArray();
            foreach (char chr in tmp)
            {
                if (!char.IsLetterOrDigit(chr) && chr != '_')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Constructs a database table name and value name based on the given
        /// <para/><paramref name="confValue"/>, which indicates the configuration
        /// <para/>name in the code.
        /// </summary>
        /// <param name="confValue">A configuration value name to split into
        /// <para/>database table and value name.</param>
        /// <param name="table">A database table that split from the <paramref name="confValue"/>.
        /// <para/>If there is no splitting slash [/] character in the name of the
        /// <para/>config value a table name of [GENERAL] is used.</param>
        /// <param name="valueName">The value name where the database table name is separated from.</param>
        private void CreateNames(ref string confValue, ref string table, ref string valueName)
        {
            if (confValue == null || confValue == string.Empty)
            {
                throw new ArgumentException("Empty or null string is not allowed.");
            }

            confValue = confValue.TrimStart('/');
            confValue = confValue.TrimEnd('/');

            if (!confValue.Contains('/'))
            {
                confValue = "GENERAL/" + confValue;
            }

            string[] tmp = confValue.Split('/');

            if (tmp.Length < 2)
            {
                throw new ArgumentOutOfRangeException("Invalid config value.");
            }

            if (!ValidTableName(tmp[0]))
            {
                throw new ArgumentException(string.Format("Invalid setting prefix '{0}'.", tmp[0]));
            }

            string tmp2 = string.Empty;
            for (int i = 1; i < tmp.Length; i++)
            {
                tmp2 += tmp[i] + "/";
            }

            tmp2 = tmp2.TrimEnd('/');
            table = tmp[0];
            valueName = tmp2;
        }

        /// <summary>
        /// Used internally by this indexers.
        /// </summary>
        /// <param name="confValue">Creates a configuration entry base on the given <paramref name="confValue"/> parameter.</param>
        /// <param name="getter">Indicates if the method was called from get{} method from the this indexer.</param>
        /// <param name="defaultValue">A default value to be used if the config entry does not exist.</param>
        private void CreateConfig(ref string confValue, bool getter, string defaultValue = null)
        {
            string table = string.Empty, valueName = string.Empty;
            CreateNames(ref confValue, ref table, ref valueName);
            Open();

            if (getter)
            {
                foreach (KeyValuePair<string, string> pair in configCache)
                {
                    if (pair.Key == table + "/" + valueName)
                    {
                        return;
                    }
                }
                try
                {
                    using (SQLiteCommand command = new SQLiteCommand(conn))
                    {
                        command.CommandText = "SELECT VALUE FROM [" + table + "] " +
                                              "WHERE KEY = '" + valueName.Replace("'", "''") + "' ";
                        using (SQLiteDataReader dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr.IsDBNull(0))
                                {
                                    configCache.Add(new KeyValuePair<string, string>(table + "/" + valueName, null));
                                }
                                else
                                {
                                    configCache.Add(new KeyValuePair<string, string>(table + "/" + valueName, UnprotectData((dr[0].ToString()))));
                                }
                                return;
                            }
                        }
                    }
                } 
                catch
                {
                    // nothing this time..
                }
            }

            if (autoCreate)
            {
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS [" + table + "] ( " +
                                          "KEY TEXT NOT NULL, " +
                                          "VALUE TEXT NULL) ";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO [" + table + "] (KEY, VALUE) " +
                                          "SELECT '" + valueName.Replace("'", "''") + "', " + (defaultValue == null ? "NULL" : "'" + ProtectData(defaultValue).Replace("'", "''") + "'") + " " +
                                          "WHERE NOT EXISTS(SELECT 1 FROM [" + table + "] WHERE KEY = '" + valueName.Replace("'", "''") + "') ";
                    command.ExecuteNonQuery();

                    if (!getter)
                    {
                        command.CommandText = "UPDATE [" + table + "] " +
                                              "SET VALUE = " + (defaultValue == null ? "NULL" : "'" + ProtectData(defaultValue).Replace("'", "''") + "'") + " " +
                                              "WHERE KEY = '" + valueName.Replace("'", "''") + "' ";
                        command.ExecuteNonQuery();
                    }
                }
                configCache.Add(new KeyValuePair<string, string>(table + "/" + valueName, ProtectData(defaultValue)));
            }
        }

        /// <summary>
        /// Adds a config entry. 
        /// <para/>This method adds the entry and does not take
        /// <para/>into account the value of AutoCreateSettings.
        /// </summary>
        /// <param name="confValue">A config entry to add.</param>
        /// <param name="defaultValue">A default value for the config entry.</param>
        public void AddValue(string confValue, string defaultValue = null)
        {
            bool tmp = autoCreate;
            try
            {
                autoCreate = true;
                CreateConfig(ref confValue, false, defaultValue);
            }
            catch (Exception ex)
            {
                autoCreate = tmp;
                throw ex;
            }
            autoCreate = tmp;
        }

        /// <summary>
        /// Deletes a given config value from the database.
        /// </summary>
        /// <param name="confValue">A config value to delete.</param>
        public void DeleteValue(string confValue)
        {
            string table = string.Empty, valueName = string.Empty;
            CreateNames(ref confValue, ref table, ref valueName);

            for (int i = 0; i < configCache.Count; i++)
            {
                if (configCache[i].Key == table + "/" + valueName)
                {
                    configCache.RemoveAt(i);
                }
            }
            
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = "DELETE FROM [" + table + "] WHERE KEY = '" + valueName.Replace("'", "''") + "' ";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a group of values in the config database based on
        /// <para/><paramref name="confValueStart"/> parameter.
        /// </summary>
        /// <param name="confValueStart">A config group name to delete.</param>
        public void DeleteValues(string confValueStart = "GENERAL")
        {
            confValueStart = confValueStart.TrimStart('/').TrimEnd('/');
            if (confValueStart.Contains('/'))
            {
                confValueStart = confValueStart.Split('/')[0];
            }

            if (ValidTableName(confValueStart))
            {
                for (int i = configCache.Count - 1; i >= 0; i--)
                {
                    if (configCache[i].Key.StartsWith(confValueStart + "/"))
                    {
                        configCache.RemoveAt(i);
                    }
                }
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = "DROP TABLE IF EXISTS [" + confValueStart + "] ";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks if a certaing configuration value exists.
        /// </summary>
        /// <param name="confValue">A config value to check.</param>
        /// <returns>True if the config value exists, otherwise false.</returns>
        public bool ValueExists(string confValue)
        {
            string table = string.Empty, valueName = string.Empty;
            CreateNames(ref confValue, ref table, ref valueName);

            foreach (KeyValuePair<string, string> pair in configCache)
            {
                if (pair.Key == table + "/" + valueName)
                {
                    return true;
                }
            }

            Open();

            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = "SELECT 1 FROM [" + table + "] WHERE KEY = '" + valueName.Replace("'", "''") + "' ";
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    return dr.Read();
                }
            }
        }

        /// <summary>
        /// Gets a config value. If AutoCreateSettings is true,
        /// <para/>a config value is created with the given default value.
        /// </summary>
        /// <param name="confValue">A config value to get.</param>
        /// <param name="defaultValue">A default value to use if the config value doesn't already exist.</param>
        /// <returns>A config value.</returns>
        public string this [string confValue, string defaultValue]
        {
            get
            {
                CreateConfig(ref confValue, true, defaultValue);
                foreach (KeyValuePair<string, string> conf in configCache)
                {
                    if (conf.Key == confValue)
                    {
                        return UnprotectData(conf.Value);
                    }
                }
                if (!autoCreate)
                {
                    throw new Exception(string.Format("Config value '{0}' does not exist.", confValue));
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets a given config value. if AutoCreateSettings is true,
        /// a non-existent config value is created.
        /// </summary>
        /// <param name="confValue">A name of the config value to get or set.</param>
        /// <returns>A config value.</returns>
        public string this [string confValue]
        {
            get
            {
                CreateConfig(ref confValue, true);
                foreach (KeyValuePair<string, string> conf in configCache)
                {
                    if (conf.Key == confValue)
                    {
                        return UnprotectData(conf.Value);
                    }
                }
                if (!autoCreate)
                {
                    throw new Exception(string.Format("Config value '{0}' does not exist.", confValue));
                }
                return null;
            }
           

            set
            {
                CreateConfig(ref confValue, false, value);
                for (int i = 0; i < configCache.Count; i++)
                {
                    if (configCache[i].Key == confValue)
                    {
                        configCache.RemoveAt(i);
                        configCache.Add(new KeyValuePair<string, string>(confValue, value));
                        return;
                    }
                }
                if (!autoCreate)
                {
                    throw new Exception(string.Format("Config value '{0}' does not exist.{1}Use AddValue or enable AutoCreateSettings.", confValue, Environment.NewLine));
                }
            }
        }

        /// <summary>
        /// Closes and disposes the underlying SQLite database connection if
        /// <para/>one is open.
        /// </summary>
        public void Close()
        {
            if (conn != null)
            {
                // Simple disposal
                using (conn)
                {
                }
                conn = null;
            }
        }

        /// <summary>
        /// The class destructor.
        /// <para/>Closes and disposes the underlying SQLite database connection if
        /// <para/>one is open.
        /// </summary>
        ~Conflib()
        {
            Close();
        }
    }
}
