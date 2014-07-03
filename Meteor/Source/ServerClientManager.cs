using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

/*
 * File : ServerClientManager
 * Author : Filipe
 * Date : 06/02/2014 13:55:23
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Server
    {
        #region FIELDS

        public List<Client> Clients { get; set; }

        private List<Client> ReadyClients { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Delete client from list
        /// </summary>
        /// <param name="c">Client to delete</param>
        internal void DeleteClient(Client c)
        {
            this.Clients.Remove(c);
            c.Socket.Close();
            c.Socket.Dispose();
            c = null;
        }

        /// <summary>
        /// Update the server client acceptor
        /// </summary>
        private void UpdateAcceptor()
        {
            if (this.ListenSocket.Poll(10, SelectMode.SelectRead) == true)
            {
                this.Clients.Add(new Client(this.ListenSocket.Accept(), this));
            }
        }

        /// <summary>
        /// Update the clients connected to the server
        /// </summary>
        private void UpdateClients()
        {
            this.ReadyClients = new List<Client>();
            foreach (Client _client in this.Clients)
            {
                if (_client.Socket.Poll(1, SelectMode.SelectRead) == true)
                {
                    this.ReadyClients.Add(_client);
                }
            }
            while (this.ReadyClients.Count > 0)
            {
                Byte[] _buffer = null;
                Int32 _packetSize = 0;

                try
                {
                    _packetSize = this.ReadyClients[0].Socket.Receive((_buffer = new Byte[this.ReadyClients[0].Socket.Available]));
                    if (_packetSize < 1)
                    {
                        this.DeleteClient(this.ReadyClients[0]);
                        this.ReadyClients.RemoveAt(0);
                        continue;
                    }                    

                    this.ReadyClients[0].KeyPair.Decrypt(ref _buffer, 0, _packetSize);
                   
                }
                catch (Exception)
                {
                    if (this.ReadyClients[0].Socket.Connected == false)
                    {
                        Log.Write(LogType.Info, "Client disconnected");
                        this.DeleteClient(this.ReadyClients[0]);
                        this.ReadyClients.RemoveAt(0);
                    }
                    continue;
                }
                List<DataPacket> _packets = new List<DataPacket>();
                Int32 _used = 0;
                while (_used < _packetSize)
                {
                    Int32 _size = BitConverter.ToUInt16(_buffer, _used);
                    _packets.Add(new DataPacket(_buffer, _used, _size));
                    _used += _size;
                }
                foreach (DataPacket _packet in _packets)
                {
                    this.ReadyClients[0].HandlePacket(_packet);
                    _packet.Free();
                }
                _packets.Clear();
                _packets = null;
                _buffer = null;
                this.ReadyClients.RemoveAt(0);
            }
        }
        /// <summary>
        /// Fonction qui va déconnecter tous les clients
        /// </summary>
        internal void RemoveAllClient()
        {
            for (int i=0;i< this.Clients.Count-1;i--)
                DeleteClient(Clients[i]);
        }
        #endregion
    }
}
