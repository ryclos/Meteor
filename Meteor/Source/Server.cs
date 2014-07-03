using Meteor.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * File : Server
 * Author : Filipe
 * Date : 05/02/2014 22:55:42
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Server
    {
        #region FIELDS

        public Boolean Initialized { get; set; }

        private Boolean Running { get; set; }

        private Socket ListenSocket { get; set; }

        public static DataLoader Loader;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new server
        /// </summary>
        public Server()
        {
            this.Initialized = false;
            this.Running = false;
            this.ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Clients = new List<Client>();
            Loader = new DataLoader();
        }

        /// <summary>
        /// Server destructor
        /// </summary>
        ~Server() { }

        #endregion

        #region METHODS

        /// <summary>
        /// Run the server
        /// </summary>
        public void Run()
        {
            if (this.Initialized == false)
            {
                Log.Write(LogType.Error, "Cannot run the server. Initialization failed.");
                Console.ReadKey();
                return;
            }
            this.AcceptClients();
        }

        /// <summary>
        /// Initialize the server
        /// </summary>
        public void Initialize()
        {
            try
            {
                // Set console title
                Console.Title = "Meteor";

                // Initialize sql connection
                if (SQL.InitDatabase() == false)
                {
                    throw new Exception("Cannot connect to MySQL Database.");
                }

                // Load data
                if (Loader.LoadAll() == false)
                {
                    throw new Exception("Loading resources failed.");
                }

                // Initialize socket
                Log.Write(LogType.Info, "Initialize socket listener...");
                this.ListenSocket.Bind(new IPEndPoint(IPAddress.Any, Configuration.Get<Int32>("Port")));
                this.ListenSocket.Listen(100);
                Log.Write(LogType.Done, "Server listening on port {0}\n", Configuration.Get<Int32>("Port"));
            }
            catch (Exception e)
            {
                Log.Write(LogType.Error, "Cannot Initialize server. {0}", e.Message);
                this.Initialized = false;
                return;
            }
            this.Initialized = true;
        }

        /// <summary>
        /// Save the server
        /// </summary>
        private void Save()
        {
            foreach (Client _client in this.Clients)
            {
                if (_client.Player != null)
                {
                    _client.Player.Save();
                    _client.Player = null;
                }
            }
        }

        /// <summary>
        /// Accept clients
        /// </summary>
        private void AcceptClients()
        {
            this.Running = true;
            while (this.Running == true)
            {
                this.UpdateAcceptor();
                this.UpdateClients();
            }
            this.Save();
        }

        #endregion
    }
}
