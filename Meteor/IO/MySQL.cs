using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using Meteor.Source;

/*
 * File : MySQL
 * Author : Filipe
 * Date : 06/02/2014 16:42:22
 * Description :
 *
 */

namespace Meteor.IO
{
    public class SQL
    {
        #region FIELDS

        internal static MySqlConnection connection { get; private set; }

        public static Boolean State
        {
            get
            {
                if (connection == null)
                {
                    return false;
                }
                return connection.State == System.Data.ConnectionState.Open ? true : false;
            }
        }

        private static Boolean Initialized = false;

        #endregion

        #region CONSTRUCTORS

        public SQL() { }

        #endregion

        #region METHODS

        /// <summary>
        /// Initialize Sql connection
        /// </summary>
        /// <param name="server">MySQL host</param>
        /// <param name="user">MySQL username</param>
        /// <param name="password">MySQL password</param>
        /// <param name="database">MySQL database</param>
        /// <returns></returns>
        public static Boolean InitDatabase()
        {
            try
            {
                if (Initialized == false)
                {
                    String _server = Configuration.Get<String>("SqlHost");
                    String _user = Configuration.Get<String>("SqlUser");
                    String _pass = Configuration.Get<String>("SqlPass");
                    String _database = Configuration.Get<String>("SqlBase");
                    Log.Write(LogType.Info, "Connecting to database...");
                    String _connectionString = String.Format("server={0};user={1};password={2};database={3};port=3306",
                        _server, _user, _pass, _database);
                    connection = new MySqlConnection(_connectionString);
                    connection.Open();
                    Initialized = true;
                }
            }
            catch (Exception e)
            {
                Log.Write(LogType.Error, "Cannot initialise SQL database.\n{0}", e.Message);
                return false;
            }
            Log.Write(LogType.Done, "Connected to database!\n");
            return true;
        }

        /// <summary>
        /// Execute a Sql query
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void ExecuteQuery(String format, params Object[] args)
        {
            String _query = String.Format(format, args);

            try
            {
                MySqlCommand cmd = new MySqlCommand(_query, connection);
                cmd.ExecuteNonQuery();
                cmd = null;
            }
            catch (Exception e)
            {
                Log.Write(LogType.Error, "MySQL error when exectute a query : " + e.Message);
            }
        }

        /// <summary>
        /// Restarts the sql connection
        /// </summary>
        public static void RestartConnection()
        {
            if (State == true)
            {
                connection.Close();
                connection.Dispose();
                connection.Open();
            }
        }

        #endregion
    }

    public class ResultSet
    {
        #region FIELDS

        private MySqlDataReader SqlReader = null;
        private String Query;
        private Boolean Active = true;

        #endregion

        #region CONSTRUCTORS

        public ResultSet(String query, params object[] args)
        {
            try
            {
                this.Query = String.Format(query, args);
                this.SqlReader = new MySqlCommand(this.Query, SQL.connection).ExecuteReader();
            }
            catch (Exception e)
            {
                Log.Write(LogType.Error, "Reader execution failed: {0}", e.Message);
                SQL.RestartConnection();
                this.Active = false;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Read sql data
        /// </summary>
        /// <returns></returns>
        public Boolean Reading()
        {
            if (this.SqlReader == null || this.SqlReader.HasRows == false)
            {
                return false;
            }
            if (this.Active == true)
            {
                return this.SqlReader.Read();
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public T Read<T>(String column)
        {
            if (this.Active == false)
            {
                return default(T);
            }
            try
            {
                if (typeof(T) == typeof(DateTime))
                {
                    String _date = (String)this.SqlReader[column];
                    if (_date == "")
                    {
                        return default(T);
                    }
                    DateTime date1 = DateTime.Parse(_date, System.Globalization.CultureInfo.GetCultureInfo("en"));
                    return (T)((Object)date1);
                }
                return typeof(T) == typeof(Boolean) ? (T)(Object)(Read<Int32>(column) != 0) : (T)this.SqlReader[column];
            }
            catch
            {
                Log.Write(LogType.Warning, "Query: failed to convert T {0}, returning default value {1} (column: {2} - expected {3})", typeof(T).Name, default(T), column, this.SqlReader[column].GetType().Name);
                return default(T);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public T Read<T>(Int32 column)
        {
            if (this.Active == false)
            {
                return default(T);
            }
            try
            {
                return typeof(T) == typeof(Boolean) ? (T)(Object)(Read<Int32>(column) != 0) : (T)this.SqlReader[column];
            }
            catch
            {
                Log.Write(LogType.Warning, "Query: failed to convert T {0}, returning default value {1} (column ID: {2} - expected {3})", typeof(T).Name, default(T), column, this.SqlReader[column].GetType().Name);
                return default(T);
            }
        }

        /// <summary>
        /// Free all resources
        /// </summary>
        public void Free()
        {
            this.Active = false;
            if (this.SqlReader != null)
            {
                this.SqlReader.Dispose();
                this.SqlReader = null;
            }
        }

        #endregion
    }
}
