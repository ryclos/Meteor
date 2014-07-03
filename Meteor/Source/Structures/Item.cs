using LuaInterface;
using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteor.Source
{
    public class Item
    {
        #region FIELDS

        public Int32 Id { get; private set; }

        public Int32 CreatorId { get; private set; }

        public Int32 CharacterId { get; set; }

        public Byte Slot { get; set; }

        public UInt32 Quantity { get; set; }

        public Boolean Equiped { get; set; }

        public Boolean Closet { get; set; }

        public Byte[] Attributes { get; set; }

        public Boolean Locked
        {
            get
            {
                if (this.CreatorId != -1)
                {
                    return true;
                }
                return false;
            }
        }

        internal ItemData ItemData
        {
            get
            {
                return Server.Loader.ItemsData[this.Id]; 
            }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Create a new Item
        /// </summary>
        public Item()
        {
            this.CreatorId = -1;
            Attributes = new Byte[17];
            Attributes[0] = 0x40;
        }

        /// <summary>
        /// Create a new Item with his id
        /// </summary>
        /// <param name="itemId">Item Id</param>
        public Item(Int32 itemId)
        {
            this.CreatorId = -1;
            this.Id = itemId;
            Attributes = new Byte[17];
            Attributes[0] = 0x40;
        }

        /// <summary>
        /// Create a new Item with his id and creator id
        /// </summary>
        /// <param name="itemId">Item Id</param>
        /// <param name="creatorId">Creator Id</param>
        public Item(Int32 itemId, Int32 creatorId)
        {
            this.CreatorId = creatorId;
            this.Id = itemId;
            Attributes = new Byte[17];
            Attributes[0] = 0x40;
        }

        /// <summary>
        /// Create a new Item with a result set from the database
        /// </summary>
        /// <param name="set"></param>
        public Item(ResultSet set)
        {
            this.Id = set.Read<Int32>("ItemId");
            this.CreatorId = set.Read<Int32>("CreatorId");
            this.Slot = (Byte)set.Read<Int32>("Slot");
            this.Quantity = (UInt32)set.Read<Int32>("Quantity");
            this.Equiped = set.Read<Boolean>("Equiped");
            Attributes = new Byte[17];
            Attributes[0] = 0x40;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Insert Item in database
        /// </summary>
        public void Insert()
        {
            SQL.ExecuteQuery("INSERT INTO items (ItemId, CharacterId, CreatorId, Slot, Quantity, Equiped) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                this.Id, this.CharacterId, this.CreatorId, this.Slot, this.Quantity, this.Equiped);
        }

        /// <summary>
        /// Save Item in database
        /// </summary>
        public void Save()
        {
            SQL.ExecuteQuery("UPDATE items SET ItemId={0}, CreatorId={1}, Slot={2}, Quantity={3}, Equiped={4}", 
                this.Id, this.CreatorId, this.Slot, this.Quantity, this.Equiped);
        }

        /// <summary>
        /// Delete Item from database
        /// </summary>
        public void Delete()
        {
            SQL.ExecuteQuery("DELETE FROM items WHERE ItemId={0} and CharacterId={1}", this.Id, this.CharacterId);
        }

        /// <summary>
        /// Serialize the current Item in the packet
        /// </summary>
        /// <param name="packet"></param>
        public void Serialize(Packet packet)
        {
            packet.Add<Byte>(this.Slot); //m_objid
            packet.Add<Int32>(this.Id); //m_itemid
            packet.Add<UInt32>(0x00000000); //EC D9 14 91  (m_serial_number)
            packet.WriteX((Int32)this.Quantity); //m_item_num
            packet.WriteX(0); //CF 46 00 00  (Hitpoints => Current Durability?)
            packet.WriteX(0); //18 47 00 00  (Maximum Hitpoints => Maximum Durability?)
            packet.Add<UInt32>(0x00000000); //00 00 00 00  (Word => ?)
            packet.Add<Byte>(0x00); //00  (Ability Option => ?)
            packet.Add<Byte>(0x00); //00  (Item Resistance => ?)
            packet.Add<Byte>(0x00); //00  (Resistance Ability Option => ?)
            packet.WriteX(0x00000000); //00 00 00 00  (Keep Time)
            packet.Add<Byte>(0x00); //00  (Item Lock)
            packet.Add<Int32>(0x00000000); //00 00 00 00  (Bind End Time)
            packet.Add<Byte>(0x00); //00  (Stability => ?)
            packet.Add<Byte>(0x00); //00  (Quality => ?)
            packet.Add<Byte>(0x00); //00  (Ability Rate => ?)
            packet.WriteX(0); //00 00 00 00  (Use Time => ?)
            packet.WriteX(0); //00 00 00 00  (Buy 'tm' => Buy Time?)
            packet.WriteX(0); //00 00 00 00  (Price => Sell/Buy?)
            packet.WriteX(0); //00 00 00 00  (Pay 银币 => ?)
            packet.WriteX(0); //00 00 00 00  (Free 银币 => ?)
            packet.Add<UInt32>(Configuration.Get<UInt32>("ServerId"));
            packet.Add<Byte>(0); //Attributes  LAr:ReadVarArrayInt16(ar, self.m_attr)
        }

        #endregion
    }

    public struct DestParam
    {
        public String Type; //ex : ADDDATA
        public Int16 Attribute; //ex : STR
        public Int32 Value; //ex : 5
    }

    public class ItemData
    {
        #region FIELDS
        
        public UInt32 ID {get;private set;}
        public String Name { get; private set; }
        public Int16 PackMax { get; private set; }
        public Int16 ItemKind1 { get; private set; }
        public Int16 ItemKind2 { get; private set; }
        public Int16 ItemKind3 { get; private set; } //max 256 mais min -1 on peux pas utiliser un Byte ou  un SByte
        public SByte Series { get; private set; }   //max 9 mais min -1 on peux utiliser un SByte
        public SByte ItemJob { get; private set; }
        public SByte ItemSex { get; private set; }
        public SByte Permanence { get; private set; }
        public Byte Charged { get; private set; }  //0 ou -1 un Byte prend moins de place qu'un booleen = optimisation
        public SByte Lock { get; private set; }
        public Byte ShopAble { get; private set; }
        public Byte Dropable { get; private set; }
        public Byte Broadcast { get; private set; } //apparemment prend une valeur null ou un chiffre
        public Byte Delete { get; private set; }
        public Byte ShopIn { get; private set; }
        public Byte TradingpostIn { get; private set; }
        public Byte MailIn { get; private set; }
        public Byte Trade { get; private set; }
        public Byte Sell { get; private set; }
        public Byte ForceBank { get; private set; }
        public Byte BankIn { get; private set; }
        public Byte Info { get; private set; }
        public Byte Repair { get; private set; }
        public Byte ToreDown { get; private set; }
        public Byte Refinement { get; private set; }
        public Byte Enchase { get; private set; }
        public Byte Produceable { get; private set; }
        public Byte Lease { get; private set; }
        public Byte BFUsable { get; private set; }
        public Byte TimeWay { get; private set; }
        public Int32 UseableDate { get; private set; }
        public Int32 TradePoint { get; private set; }
        public Int32 RmbTrade { get; private set; }
        public Int32 Cost { get; private set; }
        public Int32 SellCost { get; private set; }
        public Byte TaxQuota { get; private set; }
        public Int16 Endurance { get; private set; }
        public SByte ItemLV { get; private set; }
        public Int16 LimitLevel1 { get; private set; }
        public Byte LimitLevel2 { get; private set; }
        public Byte ItemRare { get; private set; }
        public Byte Quality { get; private set; }
        public Int32 SkillReadyType { get; private set; }
        public Int32 SkillReady { get; private set; }
        public Int32 SkillTime { get; private set; }
        public Int32 CircleTime { get; private set; }
        public Int16 UseMotion { get; private set; } //pas vu plus de 12000
        public Int32 SfxObj { get; private set; }
        public Int32 SfxObj2 { get; private set; }
        public Int32 SfxObj3 { get; private set; }
        public Int32 AbilityMin { get; private set; }
        public Int32 AbilityMax { get; private set; }
        public DestParam[] DestParam1 { get; private set; }
        public DestParam[] DestParam2 { get; private set; }
        public Int32 QuestID { get; private set; }
        
        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new ItemData with a LuaTable
        /// </summary>
        /// <param name="table"></param>
        public ItemData(LuaTable table)
        {
            this.Initialize(table);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Initialize the item data
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>
        public ItemData Initialize(LuaTable lt)
        {
            this.ID = Convert.ToUInt32((Double)(lt["ID"]));
            this.Name = (String)lt["Name"];
            this.PackMax = Convert.ToInt16((Double)(lt["PackMax"]));
            this.ItemKind1 = Convert.ToInt16((Double)(lt["ItemKind1"]));
            this.ItemKind2 = Convert.ToInt16((Double)(lt["ItemKind2"]));
            if (lt["ItemKind3"] != null)
                this.ItemKind3 = Convert.ToInt16((Double)(lt["ItemKind3"]));
            else
                this.ItemKind3 = -1;
            if (lt["Series"] != null)
                this.Series = Convert.ToSByte((Double)(lt["Series"]));
            if (lt["ItemJob"] != null)
                this.ItemJob = Convert.ToSByte((Double)(lt["ItemJob"]));
            else
                this.ItemJob = -1;
            if (lt["ItemSex"] != null)
                this.ItemSex = Convert.ToSByte((Double)(lt["ItemSex"]));
            else
                this.ItemSex = -1;
            if (lt["Permanence"] != null)
                this.Permanence = Convert.ToSByte((Double)(lt["Permanence"]));
            if (lt["Charged"] != null)
                this.Charged = Convert.ToByte((Double)(lt["Charged"]));
            if (lt["Lock"] != null)
                this.Lock = Convert.ToSByte((Double)(lt["Lock"]));
            else
                this.Lock = -1;
            if (lt["ShopAble"] != null)
                this.ShopAble = Convert.ToByte((Double)(lt["ShopAble"]));
            if (lt["Dropable"] != null)
                this.Dropable = Convert.ToByte((Double)(lt["Dropable"]));
            if (lt["Broadcast"] != null)
                this.Broadcast = Convert.ToByte((Double)(lt["Broadcast"]));
            if (lt["Delete"] != null)
                this.Delete = Convert.ToByte((Double)(lt["Delete"]));
            if (lt["ShopIn"] != null)
                this.ShopIn = Convert.ToByte((Double)(lt["ShopIn"]));
            if (lt["TradingpostIn"] != null)
                this.TradingpostIn = Convert.ToByte((Double)(lt["TradingpostIn"]));
            if (lt["MailIn"] != null)
                this.MailIn = Convert.ToByte((Double)(lt["MailIn"]));
            if (lt["Trade"] != null)
                this.Trade = Convert.ToByte((Double)(lt["Trade"]));
            if (lt["Sell"] != null)
                this.Sell = Convert.ToByte((Double)(lt["Sell"]));
            if (lt["ForceBank"] != null)
                this.ForceBank = Convert.ToByte((Double)(lt["ForceBank"]));
            if (lt["BankIn"] != null)
                this.BankIn = Convert.ToByte((Double)(lt["BankIn"]));
            if (lt["Info"] != null)
                this.Info = Convert.ToByte((Double)(lt["Info"]));
            if (lt["Repair"] != null)
                this.Repair = Convert.ToByte((Double)(lt["Repair"]));
            if (lt["Produceable"] != null)
                this.Produceable = Convert.ToByte((Double)(lt["Produceable"]));
            if (lt["BFUsable"] != null)
                this.BFUsable = Convert.ToByte((Double)(lt["BFUsable"]));
            if (lt["TimeWay"] != null)
                this.TimeWay = Convert.ToByte((Double)(lt["TimeWay"]));
            if (lt["UseableDate"] != null)
                this.UseableDate = Convert.ToInt32((Double)(lt["UseableDate"]));
            if (lt["TradePoint"] != null)
                this.TradePoint = Convert.ToInt32((Double)(lt["TradePoint"]));
            if (lt["RmbTrade"] != null)
                this.RmbTrade = Convert.ToInt32((Double)(lt["RmbTrade"]));
            if (lt["Cost"] != null)
                this.Cost = Convert.ToInt32((Double)(lt["Cost"]));
            if (lt["SellCost"] != null)
                this.SellCost = Convert.ToInt32((Double)(lt["SellCost"]));
            if (lt["TaxQuota"] != null)
                this.TaxQuota = Convert.ToByte((Double)(lt["TaxQuota"]));
            if (lt["ItemLV"] != null)
                this.ItemLV = Convert.ToSByte((Double)(lt["ItemLV"]));
            this.LimitLevel1 = Convert.ToInt16((Double)(lt["LimitLevel1"]));
            if (lt["LimitLevel2"] != null)
                this.LimitLevel2 = Convert.ToByte((Double)(lt["LimitLevel2"]));
            if (lt["ItemRare"] != null)
                this.ItemRare = Convert.ToByte((Double)(lt["ItemRare"]));
            if (lt["Quality"] != null)
                this.Quality = Convert.ToByte((Double)(lt["Quality"]));
            if (lt["SkillReadyType"] != null)
                this.SkillReadyType = Convert.ToInt32((Double)(lt["SkillReadyType"]));
            else
                this.SkillReadyType = -1;
            if (lt["SkillReady"] != null)
                this.SkillReady = Convert.ToInt32((Double)(lt["SkillReady"]));
            else
                this.SkillReady = -1;
            if (lt["SkillTime"] != null)
                this.SkillTime = Convert.ToInt32((Double)(lt["SkillTime"]));
            else
                this.SkillTime = -1;
            if (lt["CircleTime"] != null)
                this.CircleTime = Convert.ToInt32((Double)(lt["CircleTime"]));
            else
                this.CircleTime = -1;
            if (lt["UseMotion"] != null)
                this.UseMotion = Convert.ToInt16((Double)(lt["UseMotion"]));
            else
                this.UseMotion = -1;
            if (lt["SfxObj"] != null)
                this.SfxObj = Convert.ToInt32((Double)(lt["SfxObj"]));
            else
                this.SfxObj = -1;
            if (lt["SfxObj2"] != null)
                this.SfxObj2 = Convert.ToInt32((Double)(lt["SfxObj2"]));
            else
                this.SfxObj2 = -1;
            if (lt["SfxObj3"] != null)
                this.SfxObj3 = Convert.ToInt32((Double)(lt["SfxObj3"]));
            else
                this.SfxObj3 = -1;
            if (lt["AbilityMin"] != null)
                this.AbilityMin = Convert.ToInt32((Double)(lt["AbilityMin"]));
            else
                this.AbilityMin = -1;

            if (lt["AbilityMax"] != null)
                this.AbilityMax = Convert.ToInt16((Double)(lt["AbilityMax"]));
            else
                this.AbilityMax = -1;
            if (lt["DestParam1"] != null)
            {
                LuaTable _dest = (LuaTable)lt["DestParam1"];
                Int32 _count = 0;
                foreach (DictionaryEntry pair in _dest)
                    ++_count;
                this.DestParam1 = new DestParam[_count];
                if (_count > 1)
                {
                    _count = 0;
                    foreach (DictionaryEntry de in _dest)
                    {
                        LuaTable ltable = (LuaTable)de.Value;
                        DestParam param = new DestParam();
                        param.Type = (String)ltable[1.0];
                        param.Attribute = Convert.ToInt16((Double)ltable[2.0]);
                        param.Value = Convert.ToInt32((Double)ltable[3.0]);
                        this.DestParam1[_count] = param;
                        ++_count;
                    }
                }
                else
                {
                    LuaTable _table = (LuaTable)_dest.Values;
                    DestParam _param = new DestParam();
                    _param.Type = (String)_table[1.0];
                    _param.Attribute = Convert.ToInt16((Double)_table[2.0]);
                    _param.Value = Convert.ToInt32((Double)_table[3.0]);
                    this.DestParam1[0] = _param;
                }
            }

            if (lt["DestParam2"] != null)
            {
                LuaTable _table = (LuaTable)(lt["DestParam2"]);
                Int32 _count = 0;
                foreach (DictionaryEntry pair in _table)
                    ++_count;
                this.DestParam2 = new DestParam[_count];
                if (_count > 1)
                {
                    _count = 0;
                    foreach (DictionaryEntry de in _table)
                    {
                        LuaTable _table2 = (LuaTable)de.Value;
                        DestParam _param = new DestParam();
                        _param.Type = (String)_table2[1.0];
                        _param.Attribute = Convert.ToInt16((Double)_table2[2.0]);
                        _param.Value = Convert.ToInt32((Double)_table2[3.0]);
                        this.DestParam1[_count] = _param;
                        ++_count;
                    }
                }
                else
                {
                    LuaTable _table2 = (LuaTable)_table.Values;
                    DestParam _param = new DestParam();
                    _param.Type = (String)_table2[1.0];
                    _param.Attribute = Convert.ToInt16((Double)_table2[2.0]);
                    _param.Value = Convert.ToInt32((Double)_table2[3.0]);
                    this.DestParam2[0] = _param;
                }
            }
            return this;
        }

        #endregion
    }
    
}
