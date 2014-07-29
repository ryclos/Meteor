using FFEncryptionLibrary;
using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

/*
 * File : Client
 * Author : Filipe
 * Date : 06/02/2014 13:52:43
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        #region FIELDS

        /* General */
        public Socket Socket { get; set; }
        public Server Server { get; set; }

        public ReadWriteMutex _lock = new ReadWriteMutex();
        public KeyPair KeyPair { get; set; }

        public String IP
        {
            get
            {
                return ((IPEndPoint)this.Socket.RemoteEndPoint).Address.ToString();
            }
        }

        /* Login informations */
        public LoginUser User { get; set; }

        /* Characters informations */
        public List<Character> Characters { get; set; }
        public List<Character> DeletedCharacters { get; set; }

        /* Game informations */
        public Character Player { get; set; }

        /// <summary>
        /// Near client list
        /// </summary>
        public IEnumerable<Character> NearClients
        {
            get
            {
                return Server.Maps[this.Player.MapId].Players.Where(c => c != this.Player && c != null && c.Position.DistanceTo(this.Player.Position) <= 100);
                //return this.Server.Clients.Where(c => c != this && c.Player != null && c.Player.Position.DistanceTo(this.Player.Position) <= 100);
            }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new client
        /// </summary>
        /// <param name="s">Socket</param>
        /// <param name="server">Server instance</param>
        public Client(Socket s, Server server)
        {
            this.Socket = s;
            this.Server = server;
            this.KeyPair = new KeyPair(new Random().Next());
            this.User = new LoginUser();
            this.Characters = new List<Character>();
            this.DeletedCharacters = new List<Character>();
            this.SendSessionKey();
            Log.Write(LogType.Info, "New client connected! (SessionKey: {0})", this.KeyPair.Seed);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Send session key to client
        /// </summary>
        public void SendSessionKey()
        {
            BinaryWriter _writer = new BinaryWriter(new MemoryStream(8));
            _writer.Write((uint)8);
            _writer.Write(KeyPair.Seed);

            Byte[] _keyPacket = ((MemoryStream)_writer.BaseStream).GetBuffer();
            this.Socket.Send(_keyPacket, 0, _keyPacket.Length, SocketFlags.None);
        }

        /// <summary>
        /// Disconnect client
        /// </summary>
        public void Disconnect()
        {
            Log.Write(LogType.Info, "{0} disconnected", this.User.Username);
            this.Server.DeleteClient(this);
        }

        /// <summary>
        /// Send a packet to client
        /// </summary>
        /// <param name="pak">Packet to send</param>
        public void Send(Packet pak)
        {
            _lock.AcquireWriteLock();
            Byte[] _buffer = pak.Buffer;
            
            this.KeyPair.Encrypt(ref _buffer, 0, pak.Size);
            
            if (this.Socket != null && this.Socket.Connected)
                this.Socket.Send(_buffer, 0, pak.Size, SocketFlags.None);
            else
                Disconnect();
            _lock.ReleaseWriteLock();
        }

        /// <summary>
        /// Send packet to all client connected
        /// </summary>
        /// <param name="pak">Packet to send</param>
        public void SendToAll(Packet pak)
        {
            foreach (Client _client in this.Server.Clients)
            {
                if (_client != null)
                {
                    _client.Send(pak);
                }
            }
        }

        /// <summary>
        /// Send packet to near clients
        /// </summary>
        /// <param name="pak">Packet to send</param>
        public void SendToVisiblePlayers(Packet pak)
        {
            this.Send(pak);
            foreach (Character _character in this.NearClients)
            {
                _character.Client.Send(pak);
            }
        }

        /// <summary>
        /// Close and dispose the socket
        /// </summary>
        public void CloseSocket()
        {
            this.Socket.Close();
            this.Socket.Dispose();
        }

        #endregion
    }
}
