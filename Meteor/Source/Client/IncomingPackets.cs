using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : IncomingPackets
 * Author : Filipe
 * Date : 06/02/2014 17:35:17
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Handle incomming packets
        /// </summary>
        /// <param name="dp">Incomming data packet</param>
        public void HandlePacket(DataPacket dp)
        {
            UInt32 _packetSize = dp.Read<UInt32>();
            UInt16 _header = dp.Read<UInt16>();
            Header _code = (Header)_header;
            Log.Write(LogType.Debug, "Packet recieved: {0}", _code);
            switch (_code)
            {
                /* Login */
                case Header.CERTIFY: this.OnCertifyLogin(dp); break;
                case Header.PING: this.OnPing(dp); break;
                case Header.LEAVE: this.OnLeave(dp); break;

                /* Character */
                case Header.CREATEPLAYER: this.OnCreateCharacter(dp); break;
                case Header.DELETEPLAYER: this.OnDeleteCharacter(dp); break;
                case Header.GETBACK_PLAYER: this.OnResumeCharacter(dp); break;
                case Header.JOIN: this.OnJoinGame(dp); break;

                /* Game */
                case Header.COLLECT_CLIENT_LOG: this.OnCollectClientLog(dp); break;
                case Header.STATE_MSG: this.HandleStatePackets(dp); break;
                case Header.PLAYERCORR: this.OnPlayerCorrMovement(dp); break;

                /* Chat */
                case Header.NORMALCHAT: this.OnChat(dp); break;
                case Header.GMCMD: this.OnGmCommand(dp); break;

                /* Inventory */
                case Header.MOVEITEM: this.OnMoveItem(dp); break;

                /* Default */
                default: this.HandleUnknowPackets(_code); break;
            }
        }

        /// <summary>
        /// Handle state message packets
        /// </summary>
        /// <param name="dp">Incoming packet data</param>
        public void HandleStatePackets(DataPacket dp)
        {
            StateMessage _stateMessage = (StateMessage)dp.Read<Byte>();
            StateType _stateType = (StateType)dp.Read<UInt16>();
            Byte _group = dp.Read<Byte>();

            switch (_stateMessage)
            {
                case StateMessage.SYS_SET_STATE: this.OnSetPlayerState(_stateType, dp); break;
                case StateMessage.SYS_DEL_STATE: this.OnResetPlayerState(_stateType, dp); break;
                default: Log.Write(LogType.Debug, "Unknow StateMessage packet : {0}", _stateMessage); break;
            }
        }

        /// <summary>
        /// Handle unknow messages
        /// </summary>
        /// <param name="dp"></param>
        public void HandleUnknowPackets(Header code)
        {
            if (Enum.IsDefined(typeof(Header), code) == true)
            {
                Log.Write(LogType.Debug, "Unimplemented packet {0}", code);
            }
            else
            {
                Log.Write(LogType.Debug, "Unknow packet {0}", code);
            }
        }
    }
}
