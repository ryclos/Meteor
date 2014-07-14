using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * InventoryPackets.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 14/07/2014 11:35:08
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.Source.World
{
    public partial class Inventory
    {
        /// <summary>
        /// Creates an Item
        /// </summary>
        /// <param name="item"></param>
        private void SendCreateItem(Item item)
        {
            Snapshot _snapshot = new Snapshot();
            _snapshot.StartSnapshot(SnapshotType.CREATEITEM);
            _snapshot.Add<Int32>((Int32)this.Owner.ObjectId);
            _snapshot.Add<Byte>(0);
            item.Serialize(_snapshot);
            _snapshot.Add<Byte>(1);
            _snapshot.Add<Byte>(item.Slot);
            _snapshot.Add<Int16>((Int16)item.Quantity);
            this.Owner.Client.Send(_snapshot);
        }
    }
}
