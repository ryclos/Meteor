using Meteor.IO;
using Meteor.Network;
using Meteor.Source.Data;
using Meteor.Source.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Player
 * Author : Filipe
 * Date : 13/02/2014 12:58:21
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Collect client log
        /// </summary>
        /// <param name="dp">Incoming packet</param>
        public void OnCollectClientLog(DataPacket dp)
        {
            Int32 _playerId = dp.Read<Int32>();
            Byte _systemId = dp.Read<Byte>();
            String _logString = dp.Read<String>();

            if (this.Player != null && this.Player.Id == _playerId)
            {
                Log.Write(LogType.Debug, "[{0}]: {1}", this.Player.Name, _logString);
            }
            else
            {
                this.Disconnect();
            }
        }

        /// <summary>
        /// Paquet envoyé par le client pour mettre à jour la position du personnage
        /// </summary>
        /// <param name="dp"></param>
        public void OnPlayerCorrMovement(DataPacket dp)
        {
            this.Player.Position.X = dp.Read<Single>();
            this.Player.Position.Y = dp.Read<Single>();
            this.Player.Position.Z = dp.Read<Single>();
            this.Player.Angle = dp.Read<Single>();
            int angleX = dp.Read<Int32>(); //pure supposition
            short motion = dp.Read<Int16>(); //sure
            byte nloop = dp.Read<Byte>(); //sure  ANILOOP_1PLAY
            int nMotionEx = dp.Read<Int32>(); //pure supposition
            int dwMotionOption = dp.Read<Int32>(); //pure supposition
            uint dwTicks = dp.Read<UInt32>(); //pure supposition
        }

        /// <summary>
        /// Player move
        /// </summary>
        /// <param name="dp">Incoming packet data</param>
        public void OnPlayerMove(DataPacket dp)
        {
            Int32 _angle = dp.Read<Int32>(); // ???
            Int32 _x = dp.Read<Int32>();
            Int32 _y = dp.Read<Int32>();
            Int32 _z = dp.Read<Int32>();

            //this.Player.Destination.X = _x / 1000f;
            //this.Player.Destination.Y = _y / 1000f;
            //this.Player.Destination.Z = _z / 1000f;
            //this.Player.SendStateMessage(this.Player.ObjectId, false, (short)StateType.STATE_MOVE_TO, 0xFFFFFFFF, _x, _y, _z, 0xFF);        
        }
       
        /// <summary>
        /// Set the player state
        /// </summary>
        /// <param name="type">State</param>
        /// <param name="dp">Incoming packet data</param>
        public void OnSetPlayerState(StateType type, DataPacket dp)
        {
            switch (type)
            {
                case StateType.STATE_MOVE_TO: this.OnPlayerMove(dp); break;
                default: Log.Write(LogType.Debug, "Unknow StateType {0} for {1}", type, this.Player.Name); break;
            }
        }

        /// <summary>
        /// Reset player state
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dp"></param>
        public void OnResetPlayerState(StateType type, DataPacket dp)
        {
            switch (type)
            {
                case StateType.STATE_MOTION: break; //To do manage
            }
        }

        /// <summary>
        /// Moving item
        /// </summary>
        /// <param name="dp">Data incoming packet</param>
        public void OnMoveItem(DataPacket dp)
        {
            Byte _bag = dp.Read<Byte>();
            Byte _source = dp.Read<Byte>();
            Byte _dest = dp.Read<Byte>();

            Log.Write(LogType.Debug, "MoveItem => {0} to {1}", _source, _dest);
        }

    }
}
