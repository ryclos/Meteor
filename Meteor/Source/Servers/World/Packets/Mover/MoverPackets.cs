using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * MoverPackets.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 20/07/2014 11:26:49
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.Source
{
    public partial class Mover
    {
        /// <summary>
        /// Sends mover destination to all visible clients
        /// </summary>
        public void SendMoverDestination()
        {
            Snapshot _snap = new Snapshot();
            _snap.StartSnapshot((SnapshotType)23);
            _snap.Add<Int32>((Int32)this.ObjectId);
            _snap.Add<Byte[]>(new byte[] { 0x00, 0x0E, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }); //TODO: Figure out how this works.
            _snap.Add<Int32>((Int32)this.Destination.X * 1000);
            _snap.Add<Int32>((Int32)this.Destination.Y * 1000);
            _snap.Add<Int32>((Int32)this.Destination.Z * 1000);
            this.SendToVisiblePlayers(_snap);
        }
    }
}
