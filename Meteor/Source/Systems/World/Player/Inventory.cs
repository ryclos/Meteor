using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Inventory
 * Author : Filipe
 * Date : 18/02/2014 11:03:52
 * Description :
 *
 */

namespace Meteor.Source
{
    public class Inventory
    {
        #region FIELDS

        public static readonly Byte[] InventorySlots = new byte[]
		                                               {
			                                               0x34,0x02,0x03,0x01,0x01,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,0x10,0x11,0x12,0x13,
                                                           0x14,0x15,0x16,0x17,0x18,0x19,0x1A,0x1B,0x1C,0x1D,0x1E,0x1F,0x20,0x21,0x22,0x23,0x24,0x25,0x26,0x27,
                                                           0x28,0x29,0x2A,0x2B,0x42,0x2D,0x2E,0x2C,0x30,0x31,0x32,0x33,0x43,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,
                                                           0x3C,0x3D,0x3E,0x3F,0x40,0x41,0x00,0x2F,0x44,0x45,0x46,0x47,0x48
		                                               };

        public Character Owner { get; private set; }

        public Byte MaxItem
        {
            get
            {
                return ((Byte)Define.MAX_INVENTORY) + ((Byte)Define.MAX_HUMAN_PARTS);
            }
        }

        public List<Item> Items { get; set; }

        public List<ClosetItem> Closet { get; set; }

        private Boolean[] Slots { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initiliaze the Inventory with his owner
        /// </summary>
        /// <param name="owner">Inventory owener character</param>
        public Inventory(Character owner)
        {
            this.Owner = owner;
            this.Items = new List<Item>();
            this.Closet = new List<ClosetItem>();
            this.Slots = new Boolean[this.MaxItem];
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Load the inventory
        /// </summary>
        public void LoadInventory()
        {
            if (this.Items == null)
            {
                this.Items = new List<Item>();
            }
            ResultSet _set = new ResultSet("SELECT * FROM items WHERE CharacterId = {0}", this.Owner.Id);
            while (_set.Reading() == true)
            {
                Item _item = new Item(_set);
                _item.CharacterId = this.Owner.Id;
                this.Items.Add(_item);
                this.Slots[_item.Slot] = true;
            }
            _set.Free();
        }

        /// <summary>
        /// Load closet items
        /// </summary>
        public void LoadCloset()
        {
            if (this.Closet == null)
            {
                this.Closet = new List<ClosetItem>();
            }
            ResultSet _set = new ResultSet("SELECT * FROM closet WHERE CharacterId = {0}", this.Owner.Id);
            while (_set.Reading() == true)
            {
                this.Closet.Add(new ClosetItem(_set));
            }
            _set.Free();
        }

        /// <summary>
        /// Gets the equiped closet item
        /// </summary>
        /// <returns></returns>
        public List<ClosetItem> GetEquipedCloset()
        {
            return this.Closet.Where(c => c.Equiped == true).ToList();
        }

        /// <summary>
        /// Get free inventory slot
        /// </summary>
        /// <returns></returns>
        public Byte GetFreeSlot()
        {
            for (Byte i = 0; i < (Byte)Define.MAX_INVENTORY; i++)
            {
                if (this.Slots[i] == false)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a list containing the current equiped items
        /// </summary>
        /// <returns></returns>
        public List<Item> GetEquipedItems()
        {
            List<Item> _items = new List<Item>();
            foreach (Item _item in this.Items)
            {
                if (_item.Slot > (Int32)Define.MAX_INVENTORY)
                {
                    _items.Add(_item);
                }
            }
            return _items;
        }

        /// <summary>
        /// Create Item
        /// </summary>
        /// <param name="item">Item</param>
        public void CreateItem(Item item)
        {
            if (this.Items.Count >= (Int32)Define.MAX_INVENTORY)
            {
                Log.Write(LogType.Warning, "{0}'s inventory is full. Cannot create item", this.Owner.Name);
                return;
            }
            item.Slot = this.GetFreeSlot();
            item.CharacterId = this.Owner.Id;
            item.Insert();
            this.Slots[item.Slot] = true;
            this.Items.Add(item);
            // Send packet
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

        /// <summary>
        /// Delete an item with his position in the inventory
        /// </summary>
        /// <param name="position"></param>
        public void DeleteItem(Int32 position)
        {
        }

        /// <summary>
        /// Move an item
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public void MoveItem(Int32 source, Int32 dest)
        {
        }

        /// <summary>
        /// Insert the inventory in database
        /// </summary>
        public void Insert()
        {
            foreach (Item _item in this.Items)
            {
                _item.Insert();
            }
            foreach (ClosetItem _closet in this.Closet)
            {
                _closet.CharacterId = this.Owner.Id;
                _closet.Insert();
            }
        }

        /// <summary>
        /// Save the inventory
        /// </summary>
        public void Save()
        {
            foreach (Item _item in this.Items)
            {
                _item.Save();
            }
            foreach (ClosetItem _closet in this.Closet)
            {
                _closet.Save();
            }
        }

        /// <summary>
        /// Serialize the inventory in packet
        /// </summary>
        /// <param name="packet">Outgoing packet</param>
        public void Serialize(Packet packet)
        {
            packet.Add<Byte>(this.MaxItem);
            packet.Add<Byte>((Byte)Define.MAX_INVENTORY);
            packet.Add<Byte[]>(InventorySlots);
            // TO DO : 
            /*if self.m_index[i] >= 0 then
              local elem = self.m_item[self.m_index[i]]
              elem.m_objindex = i
              elem.m_bag_id = self.m_bag_id
            end*/
            packet.Add<Byte>((Byte)this.Items.Count);
            foreach (Item _item in this.Items)
            {
                _item.Serialize(packet);
                packet.Add<Byte>((byte)Array.IndexOf(InventorySlots, _item.Slot));
            }
        }

        #endregion
    }
}
