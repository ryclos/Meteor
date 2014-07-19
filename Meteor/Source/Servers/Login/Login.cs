using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Login
 * Author : Filipe
 * Date : 06/02/2014 17:36:38
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Certify client login
        /// </summary>
        /// <param name="dp">Incoming packet</param>
        public void OnCertifyLogin(DataPacket dp)
        {
            String _name = dp.Read<String>();
            String _password = dp.Read<String>();
          
            #region Account checking

            ResultSet _set = new ResultSet(Query.SELECT_LOGIN, _name);
            if (_set.Reading() == false)
            {
                Log.Write(LogType.Error, "Cannot find account '{0}'", _name);
                this.SendLoginError(Error.ERR_CERT_BAD_USERNAME);
                this.Disconnect();
                _set.Free();
                return;
            }

            String _dbPassword = _set.Read<String>("Password");
            if (_password.ToLower() != _dbPassword.ToLower())
            {
                Log.Write(LogType.Error, "Bad password for '{0}'", _name);
                this.SendLoginError(Error.ERR_CERT_BAD_PASSWORD);
                this.Disconnect();
                _set.Free();
                return;
            }

            Int32 _accountId = _set.Read<Int32>("Id");
            Int32 _accountAuthority = _set.Read<Int32>("Authority");

            if (_accountAuthority <= 0)
            {
                // TODO: send error account banned
                this.Disconnect();
                _set.Free();
                return;
            }
            _set.Free();
            #endregion
            if (this.CheckLoginClientConnected(_name) == true)
            {
                this.SendLoginError(Error.ERR_ACCOUNT_EXIST); // ??
                this.Disconnect();
                return;
            }
            //Verification de la version des fichiers
            String hdsn = dp.Read<String>();
            
            byte[] ipKey = dp.ReadBytes(21);
            string varchar = "";
            for (int i = 0; i < ipKey.Length; i++)
                if (ipKey[i] != 0)
                    varchar += ipKey[i].ToString();
            SQL.ExecuteQuery(string.Format(Query.UPDATE_LASTIP, varchar, _name));
            var version = dp.Read<UInt32>();
            var realVersion = dp.Read<UInt32>();
            var opFlag = dp.Read<Byte>();
            var otpPassword = dp.Read<String>();
            
            if (realVersion == Configuration.CurrentVersion)
            {
                this.User.AccountId = _accountId;
                this.User.Username = _name;
                this.User.Password = _password;
                this.User.Authority = _accountAuthority;
                this.User.Connected = true;

                this.SendLoginSuccess();
                this.LoadCharacterList();
                this.SendCharacterList(); 
            }
            else
            {
                Log.Write(LogType.Debug, "version incorrecte du client : {0} au lieu de {1}", realVersion, Configuration.CurrentVersion);
                this.SendVersionError(realVersion);
                Disconnect();
            }
        }

        /// <summary>
        /// Save and disconnect the client
        /// </summary>
        /// <param name="dp"></param>
        public void OnLeave(DataPacket dp)
        {
            if (this.Player != null)
            {
                this.Player.Save();
                if (this.Player.Spawned == true)
                {
                    Server.Maps[this.Player.MapId].Delete<Character>(this.Player);
                }
                this.Player = null;
            }
            this.Disconnect();
        }
        /// <summary>
        /// Reponse au ping du client
        /// </summary>
        /// <param name="dp"></param>
        public void OnPing(DataPacket dp)
        {
            Packet pak = new Packet((Int16)10);
            pak.Add<UInt32>(dp.Read<UInt32>());
            this.Send(pak);
        }

        /// <summary>
        /// Check if client is already connected
        /// </summary>
        /// <param name="name">Client name</param>
        /// <returns></returns>
        public Boolean CheckLoginClientConnected(String name)
        {
            foreach (Client _client in this.Server.Clients)
            {
                if (_client.User.Username == name && _client.User.Connected == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
