using Meteor.IO;
using Meteor.Network;
using Meteor.Source.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

/*
 * File : Character
 * Author : Filipe
 * Date : 06/02/2014 19:21:48
 * Description :
 *
 */

namespace Meteor.Source
{
    public enum ObjectType : int
    {
        OT_OBJ = 1,
        OT_STATIC = 2,
        OT_CTRL = 4,
        OT_COMMON = 8,
        OT_ITEM = 16,
        OT_SFX = 32,
        OT_SHIP = 64,
        OT_SPRITE = 128,
        OT_PLAYER = 256,
        OT_CLIENT = OT_OBJ | OT_CTRL | OT_SPRITE | OT_PLAYER,
        OT_MONSTER = 512,
        OT_MOB = OT_OBJ | OT_CTRL | OT_SPRITE | OT_MONSTER,
        OT_NPC = 1024,
        OT_NPC2 = OT_OBJ | OT_CTRL | OT_SPRITE | OT_MONSTER | OT_NPC,
        OT_GUARD = 2048,
        OT_PET = 4096,
        OT_REGION = 8192,
        OT_COMMON_SFX = 16384,
        OT_SHOP = 32768,
        OT_BODYGUARD = 65536,
        OT_TRAINBALL = 131072,
        MAX_OBJTYPE = 18
    }

    public class Character
    {
        #region FIELDS

        /* General */
        public Client Client { get; private set; }
        public LoginUser Account { get; set; }
        public Int32 Id { get; set; }
        public Int32 AccountId { get; set; }
        public Int32 Slot { get; set; }
        public Int32 Level { get; set; }
        public Int32 Job { get; set; }
        public Int64 Exp { get; set; }
        public Int32 Gold { get; set; }
        public Gender Gender { get; set; }
        public String Name { get; set; }

        /* Map */
        public UInt32 ObjectId { get; set; }
        public Int32 MapId { get; set; }
        public Position Position { get; set; }
        public Single Angle { get; set; }

        /* Attr */
        public Int32 HP { get; set; }
        public Int32 MP { get; set; }
        public Int32 Strength { get; set; }
        public Int32 Stamina { get; set; }
        public Int32 Dexterity { get; set; }
        public Int32 Inteligence { get; set; }
        public Int32 Spirit { get; set; }

        /* Attributes */
        public Attributes Attributes { get; set; }

        /* Apparence */
        public UInt32 ModelId { get; set; }
        public Int32 HairMesh { get; set; }
        public Int32 HairColor { get; set; }
        public Int32 HeadMesh { get; set; }

        public DateTime DeletedDate { get; set; }

        /* Inventory */
        public Inventory Inventory { get; set; }

        /* Friends */
        public List<UInt32> BlackList { get; set; }

        /* Visibilité */
        //public List<FObject> Spawns { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize an empty Character
        /// </summary>
        public Character() { }

        /// <summary>
        /// Initialize a new Character with the current client pointer
        /// </summary>
        /// <param name="c">Client pointer</param>
        public Character(Client c)
        {
            this.Initialize(c);
        }

        /// <summary>
        /// Initialize a new Character with a sql result set
        /// </summary>
        /// <param name="set">SQL Result set</param>
        /// <param name="c">Client pointer</param>
        public Character(ResultSet set, Client c)
        {
            this.Initialize(c);
            this.Initialize(set);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Intialize
        /// </summary>
        /// <param name="c"></param>
        public void Initialize(Client c)
        {
            this.Client = c;
            this.Attributes = new Attributes();
            this.Inventory = new Inventory(this);
            //this.BlackList = new List<UInt32>();
            //this.Spawns = new List<FObject>();
            //this.Taskbarre = new TaskBar();
            //this.Taskbarre.DBLoadTaskBar(this.Id);
            //this.HotKey = new HotKey[47];
            //LoadHotKey();
            //this.m_data[Define.MOVE_SPEED] = 6000;
        }

        /// <summary>
        /// Intialize the character with a sql result set
        /// </summary>
        /// <param name="set"></param>
        private void Initialize(ResultSet set)
        {
            Gender gender = set.Read<Int32>("Gender") == 0 ? Gender.Male : Gender.Female;
            UInt32 _modelId = (UInt32)(gender == Source.Gender.Male ? 11 : 12);
            this.Account = this.Client.User;
            this.ModelId = _modelId;
            this.Id = set.Read<Int32>("Id");
            this.Slot = set.Read<Int32>("Slot");
            this.Attributes[Define.LV] = set.Read<Int32>("Level");
            this.Job = set.Read<Int32>("Job");
            this.Attributes[Define.EP] = set.Read<Int32>("Exp");
            this.Name = set.Read<String>("Name");
            this.MapId = set.Read<Int32>("MapId");
            this.Position = new Position(set.Read<Single>("PosX"), set.Read<Single>("PosY"), set.Read<Single>("PosZ"));
            this.Gender = gender;
            this.DeletedDate = set.Read<DateTime>("DeletedDate");
            
            /* Attr */
            this.Attributes[Define.HP] = set.Read<Int32>("Hp");
            this.Attributes[Define.MP] = set.Read<Int32>("Mp");
            this.Attributes[Define.STR] = set.Read<Int32>("Strength");
            this.Attributes[Define.STA] = set.Read<Int32>("Stamina");
            this.Attributes[Define.DEX] = set.Read<Int32>("Dexterity");
            this.Attributes[Define.INT] = set.Read<Int32>("Inteligence");
            this.Attributes[Define.SPI] = set.Read<Int32>("Spirit");
            this.Attributes[Define.GOLD] = set.Read<Int32>("Gold");
            this.HairColor = set.Read<Int32>("HairColor");
            this.HairMesh = set.Read<Int32>("HairMesh");
            this.HeadMesh = set.Read<Int32>("HeadMesh");

            this.Attributes[Define.MAXHP] = GetMaxHp();
            this.Attributes[Define.MAXMP] = GetMaxMp();
        }
        
        public Int32 GetMaxHp()
        {
            return 397; // TODO: find formula
        }

        public Int32 GetMaxMp()
        {
            return 254;
        }

        /// <summary>
        /// Insert the character in the database and set his Id
        /// </summary>
        public void InsertIntoDatabase()
        {
            SQL.ExecuteQuery(Query.CREATE_CHARACTER,
                this.Account.AccountId, this.Name, this.Slot, this.Job, 
                this.Gender == Source.Gender.Male ? 0 : 1,
                this.MapId, this.Position.X, this.Position.Y, this.Position.Z,
                this.Attributes[Define.STR], this.Attributes[Define.STA], this.Attributes[Define.DEX], this.Attributes[Define.INT], this.Attributes[Define.SPI],
                this.Attributes[Define.HP], this.Attributes[Define.MP], this.HeadMesh, this.HairColor, this.HairMesh, DateTime.Now.ToString(CultureInfo.GetCultureInfo("en")));

            ResultSet _rs = new ResultSet(Query.GET_CHARACTER_ID, this.Name, this.Account.AccountId, this.Slot);
            if (_rs.Reading() == true)
            {
                this.Id =_rs.Read<Int32>("Id");
            }
            _rs.Free();

            this.Inventory.Insert(); // Insert inventory in database at character creation
        }

        /// <summary>
        /// Save the character in the database
        /// </summary>
        public void Save()
        {
            SQL.ExecuteQuery(Query.UPDATE_CHARACTER,
                this.Name, this.Slot, this.Attributes[Define.LV], this.Job, this.Attributes[Define.EP], this.Attributes[Define.GOLD], this.Gender == Source.Gender.Male ? 0 : 1,
                this.MapId, (Int32)this.Position.X, (Int32)this.Position.Y, (Int32)this.Position.Z,
                this.Attributes[Define.STR], this.Attributes[Define.STA], this.Attributes[Define.DEX], this.Attributes[Define.INT], this.Attributes[Define.SPI],
                this.Attributes[Define.HP], this.Attributes[Define.MP], this.HeadMesh, this.HairColor, this.HairMesh, this.Id);

            this.Inventory.Save();
        }

        /// <summary>
        /// Serialize the character into a Snapshot packet (for join world)
        /// </summary>
        /// <param name="packet">Snapshot packet</param>
        /// <param name="me">Me or not</param>
        public void Serialize(Snapshot packet, Boolean me = false)
        {
            packet.StartSnapshot(SnapshotType.ADDOBJ);

            Dictionary<String, Int32> _checkValues = new Dictionary<String, Int32>
			                  {
				                  {"base", 0x77777777},
				                  {"extend", 0x11111111},
				                  {"inventory", 0x22222222},
				                  {"taskbar", 0x33333333},
				                  {"quest", 0x44444444},
				                  {"messenger", 0x55555555},
				                  {"skill", 0x66666666},
				                  {"tbag", 0x7777777a},
				                  {"credit", 0x7777777b},
				                  {"faction", 0x7777777c},
				                  {"lover", 0x77777790},
				                  {"closet", 0x77777791},
				                  {"vessel", 0x77777792},
				                  {"hotkey", 0x77777793},
                                  {"spirittatoo", 0x77777794},
                                  {"advent", 0x77777795},
                                  {"domesticate", 0x77777796}
			                  };

            packet.Add<UInt32>((UInt32)ObjectType.OT_CLIENT); //Object Type  OT_PLAYER = 256 + sprite 128 + CTRL = 4 + OBJ = 1 = 389 ?
            packet.Add<UInt32>(this.ObjectId); // Object Id
            packet.Add<Int32>(this.Id); // Character Id   self.m_player_id,
            packet.Add<Byte>((Byte)this.Gender); //self.m_sex,
            packet.Add<Int32>(this.Job); //self.m_job,
            packet.Add<Int32>((Int32)this.Account.Authority); //self.m_auth, 
            packet.Add<Int32>(Configuration.Get<Int32>("ServerId")); //self.m_server_id,
            packet.Add<String>(""); // uXinEmu (VIP Bar name ???) self.m_vipbar_name

            packet.Add<UInt32>(this.ModelId); //self.m_index model ?
            packet.Add<UInt32>(0xFFFFFFFF); //link_id
            packet.Add<String>(this.Name); //m_name
            packet.Add<Int16>(9); //m_constellation
            packet.Add<Int16>(1); //m_city
            packet.Add<Byte>((Byte)this.HairMesh); //m_hair_mesh
            packet.Add<UInt32>((UInt32)this.HairColor); //m_hair_color
            packet.Add<Byte>((Byte)this.HeadMesh); //m_head_mesh
            packet.Add<UInt32>(0x7FFFFFFF); //m_option
            packet.Add<UInt32>(0); //m_team_id
            packet.Add<String>(""); // Vendor string
            packet.Add<Int32>(0); // PK Kills m_slaughter
            packet.Add<Int32>(0); // PK Deaths  .m_kill_num
            packet.Add<Int32>(0x64); // PK Fame  m_fame
            packet.Add<Byte>(0); // PK Mode m_pk_mode
            packet.Add<Byte>(0); // PK Name
            packet.Add<Int32>(0); // PK Grey time
            packet.Add<Int32>(0x5322AFDF); // PK Protection cooldown  m_prot_cool_down_start
            packet.Add<Int32>(0); // (sf ??)  issf

            packet.Add<Position>(this.Position);
            packet.Add<Single>(0); // Angle
            packet.Add<Single>(0); //anglex
            packet.Add<UInt16>(100); // player scale

            packet.Add<UInt16>(9);
            packet.Add<Byte[]>(new Byte[] { 0x76, 0x31, 0x24, 0x32, 0x34, 0x2C, 0x24, 0x24, 0x24 });

            /* Stream data */
            Dictionary<Define, Int32> _streamData = new Dictionary<Define, Int32>
			                 {
				                 {Define.HP, 397}, //HP
				                 {Define.MP, 254}, //MP
				                 {Define.GP, 2},
				                 {Define.LV, this.Attributes[Define.LV]},
				                 {Define.FLV, 1 },
				                 {Define.VIT, 120},
				                 {Define.FHP, 450},
				                 {Define.MOVE_SPEED, 6000}, //character level
				                 
				                 {Define.STR, this.Attributes[Define.STR] }, //Strength
				                 {Define.STA, this.Attributes[Define.STA] }, //Stamina
				                 {Define.INT, this.Attributes[Define.INT] }, //Intelligence
                                 {Define.FMP, 290 }, //Fly Intelligence
				                 {Define.DEX, this.Attributes[Define.DEX] }, //Dexterity
				                 {Define.SPI, this.Attributes[Define.SPI] }, //Spirit				                 
			                 };
            packet.Add<Int32>(_streamData.Count);
            foreach (var pair in _streamData)
            {
                packet.Add<Int16>((short)pair.Key);
                packet.WriteX(pair.Value);
            }

            /* Buffs */
            packet.Add<Byte>(0); //00 00 00 00 (Stream Buff Size => ?) stream_buff count

            /* Equipement */
            packet.Add<UInt32>(0); // Equipement count

            packet.Add<Byte>(0);

            packet.Add<UInt32>(0); //00 00 00 00 (Player Title Count)
            packet.Add<UInt32>(7); //07 00 00 00 (Player Title Flag => ?)
            packet.Add<Byte>(0); //00 (Player Title Count => ?)

            /* Kingdom */
            packet.Add<Byte>(0); //00 self.m_k_job
            packet.Add<Byte>(0); //self.m_k_sub_job
            packet.Add<UInt32>(0); //00 00 00 00 (Family Id)
            packet.Add<String>(""); //00 00 (Family Name)
            packet.Add<Byte>(0); //00 (Family Job)
            packet.Add<Byte>(0); //00 (Title Id => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Family Popularity)
            packet.Add<UInt32>(0); //00 00 00 00 (Family Rank)
            packet.Add<Byte>(0); //00 (Family Icon Id)
            packet.Add<UInt32>(0); //self.m_alliance_id
            packet.Add<Byte>(0); //00 self.m_alliance_job
            packet.Add<String>(""); //00 00 m_alliance_name     

            /* Master */
            packet.Add<String>(""); //00 00 (Master Name)
            packet.Add<String>(""); //00 00 (Master Faction Name => ?)
            packet.Add<String>(""); //00 00 (Faction Name => ?)
            packet.Add<Int32>(0); //00 00 00 00 (m_close_points_total_with_master)


            packet.Add<UInt32>(0); //0C 00 00 00 (Safety Immunity => ?)
            packet.Add<Byte>(0); //00 (Lover Count)

            /* Weding */
            packet.Add<Boolean>(true); //01 (Marriage System Enabled)
            packet.Add<Byte>(0); //00 (Marriage System Kind => ?)

            /* Closet */
            packet.Add<Int32>(this.Inventory.GetEquipedCloset().Count);
            foreach (ClosetItem _closet in this.Inventory.GetEquipedCloset())
            {
                packet.Add<Int32>(_closet.Id); //index
            }

            /* Vessel */
            packet.Add<UInt32>(0); //?? ?? ?? ?? (Vessel Refine Level => ?)
            packet.Add<UInt32>(0); //?? ?? ?? ?? (Vessel Equip Index => ?)
            packet.Add<Boolean>(false); //?? (Vessel Equipped => ?)

            packet.Add<Byte>((byte)(me ? 1 : 0)); //flag
            if (me == false)
            {
                return;
            }

            
            packet.Add<Int32>(_checkValues["base"]); //77 77 77 77 (Data Check => Constant)

            packet.Add<Byte>(0); // m_need_activate_game
            packet.Add<Byte>(1); // m_game_flag
            packet.Add<UInt32>(1394954851); // m_last_logout_time
            packet.Add<UInt32>(0); // 00 00 00 00 (Pet ID)

            /* Title */
            packet.Add<UInt32>(0); // 00 00 00 00 (Title Count)

            packet.Add<Int32>(0); //02 00 00 00 (title_list_count Count =>)
            //packet.Add<Int32>(24000); //C0 5D 00 00 (Renown Title => ?)
            //packet.Add<Int32>(26000); //90 65 00 00 (Renown Title => ?)

            packet.Add<Byte>(0); //00 (Renown Title Host Count => ?)

            /* Inventory */
            this.Inventory.Serialize(packet);
            packet.Add<Int32>(_checkValues["inventory"]);

            /* Quest Inventory: */
            packet.Add<Byte>((byte)Define.MAX_INVENTORY); //m_item_max
            packet.Add<Byte>((byte)Define.MAX_INVENTORY); //m_index_num
            packet.Add<Byte[]>(new byte[]
			                    {
				                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D,
				                    0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B,
				                    0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29
			                    }); //Slots?
            packet.Add<Byte>(0);
            packet.Add<Int32>(_checkValues["tbag"]);

            /* Taskbar */
            packet.Add<Int32>(0);
            packet.Add<Int32>(0);
            packet.Add<Int32>(0); //CWndTaskSubVer
            packet.Add<Int32>(4);
            packet.Add<Byte[]>(new Byte[] { 1, 2, 3, 4 });
            packet.Add<Int32>(0);
            packet.Add<Int32>(_checkValues["taskbar"]);

            /* Skills */
            packet.Add<UInt32>(0); //03 00 00 00 (Skill Count)
            packet.Add<UInt32>(0); //self.m_domesticate_skill_time = LAr:ReadDword(ar)
            packet.Add<Int32>(_checkValues["skill"]);

            /* Quests */
            packet.Add<Byte>(1); //01 (Quest Change Flag => Always 1?)

            /* Quests in progress */
            packet.Add<UInt32>(0); //01 00 00 00 (Quest Count)

            /* Quests finished */
            packet.Add<UInt32>(0); //05 00 00 00 (Completed Quest Count)

            /* Repeat quests */
            packet.Add<UInt16>(0); //00 00 ('repeat_quest' => ?)

            packet.Add<Int32>(_checkValues["quest"]);

            /* Friends */
            packet.Add<Int32>(0); //00 00 00 00 ('my_state' => ?)  self.m_my_state = LAr:ReadInt(ar)
            packet.Add<Int32>(0); //00 00 00 00 ('favor_value' => ?) self.m_favor_value = LAr:ReadInt(ar)
            packet.Add<Int32>(0); //00 00 00 00 ('flower_count' => ?) local flower_count = LAr:ReadInt(ar)

            packet.Add<UInt32>(0); //00 00 00 00 (Friend Count)

            packet.Add<Int32>(0); //00 00 00 00 (Blacklist Count) local black_count = LAr:ReadInt(ar)

            packet.Add<Int32>(0); //00 00 00 00 (Murderer Count) local murderer_count = LAr:ReadInt(ar)

            packet.Add<Int32>(_checkValues["messenger"]);

            /* Faction */
            packet.Add<Boolean>(false); //00 (Faction Master Flag)  local is_master = LAr:ReadByte(ar)
            packet.Add<Int32>(0); //00 00 00 00 (Faction Protege Count => ?)
            packet.Add<Int32>(0); //00 00 00 00 (Faction Honor Points => ?)
            packet.Add<Int32>(0); //00 00 00 00 (Faction Honor Points Total => ?)
            packet.Add<Int32>(0); //00 00 00 00 (Faction Level Points => ?)
            packet.Add<Boolean>(false); //00 (Faction Is Faction => ?)
            packet.Add<String>(""); //00 00 (Faction Name)
            packet.Add<Int32>(_checkValues["faction"]);

            /* Memorised position */
            packet.Add<Int32>(0); //00 00 00 00 (Position Count => ?)

            /* PVP */
            packet.Add<Int32>(0); //00 00 00 00 (Duels Won)
            packet.Add<Int32>(0); //00 00 00 00 (Duels Tied)
            packet.Add<Int32>(0); //00 00 00 00 (Duels Lost)
            packet.Add<Int32>(0); //00 00 00 00 (PKs Won)
            packet.Add<Int32>(0); //00 00 00 00 (Total PKs)
            packet.Add<Int32>(135); //87 00 00 00 ('adv' Stamina => ?)
            packet.Add<Byte>(1); //01 (Auto Assign => ?)

            /* Credit card */
            packet.Add<Byte>(0); //00 (Credit Card Type => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Limit => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Current Limit => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Recharge => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card 'Pay Recharge' => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Preferential => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card One Day Consume => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Total Consume => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Last Consume Date => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card One Day Trade => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Last Trade Date => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Recharged Money => ?)
            packet.Add<Int32>(-1); //FF FF FF FF (Credit Card Trade Points => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Credit Card Recharge Reward => ?)
            packet.Add<Int32>(_checkValues["credit"]);

            /* VIP */

            packet.Add<Byte>(0); //00 (VIP Level Game => ?)
            packet.Add<Byte>(0); //00 (VIP Level GM => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (VIP Expiry Date => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Last Wage Time Game => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Last Wage Time GM => ?)

            /* Equip attr bonus */

            packet.Add<Byte>(0x0F); //00 (Equipment Attribute Stone STR Bonus => ?)
            packet.Add<Byte>(0x0F); //00 (Equipment Attribute Stone DEX Bonus => ?)
            packet.Add<Byte>(0x2D); //2D (Equipment Attribute Stone STA Bonus => ?)
            packet.Add<Byte>(0x0F); //0A (Equipment Attribute Stone SPI Bonus => ?)
            packet.Add<Byte>(0x0A); //2D (Equipment Attribute Stone INT Bonus => ?)

            /* Lovers */
            packet.Add<UInt32>(0); //00 00 00 00 (Lovers Count)
            packet.Add<Int32>(_checkValues["lover"]);

            /* Closet */
            packet.Add<Int32>(8);//character.Closet.Capacity); //08 00 00 00 (Closet Capacity)
            packet.Add<Int32>(1);//character.Closet.Level); //01 00 00 00 (Closet Level)
            packet.Add<Int32>(this.Inventory.GetEquipedCloset().Count); //00 00 00 00 (Fate Number => ?)
            foreach (ClosetItem _closet in this.Inventory.GetEquipedCloset())
            {
                packet.Add<Int32>(_closet.Index);
                packet.Add<Int32>(_closet.Id);
                packet.Add<Int32>(_closet.Level);
                packet.Add<Int32>(_closet.Equiped ? 1 : 0);
                packet.Add<Int32>(_closet.Date.GetUnixTimestamp());
                for (Int32 i = 0; i < 15; i++)
                {
                    packet.Add<Int32>(0);
                }
            }

            packet.Add<Int32>(_checkValues["closet"]);

            /* Vessel */
            packet.Add<UInt32>(1); //01 00 00 00 (Vessel Level => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Vessel Equip Slot => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Vessel Spirit => ?)
            packet.Add<UInt32>(0); //00 00 00 00 (Vessel Battle => ?
            packet.Add<UInt32>(0); //00 00 00 00 (Vessel Slots => ?)
            packet.Add<Int32>(_checkValues["vessel"]);

            /* Hotkey */
            for (Int32 i = 0; i < 47; ++i)
            {
                packet.Add<Int32>(0);
                packet.Add<Int32>(0);
            }
            packet.Add<Int32>(_checkValues["hotkey"]);

            /* Spirittatoo */
            packet.Add<String>("1,10,2,0,1,10,3,0,1,10,2,0,1,10,1,0,1,10,4,0,1,10,4,0,");
            packet.Add<Int32>(_checkValues["spirittatoo"]);

            /* Serialize advent */
            packet.Add<Int32>(1);
            packet.Add<Int32>(0);
            packet.Add<Int32>(1);
            packet.Add<Int32>(0);
            packet.Add<Int32>(1);
            packet.Add<String>("1=0;2=0;3=0;4=0;5=0;6=0;7=0;8=0;");
            packet.Add<Int32>(_checkValues["advent"]);

            /* Pet domesticate */
            packet.Add<Int32>(0);
            packet.Add<Int32>(0);
            packet.Add<Int32>(_checkValues["domesticate"]);

            /* End */
            packet.Add<Int32>(0);  //m_cbg_sign_time
            packet.Add<Int32>(0);  //m_cbg_sign_player
            packet.Add<Int32>(0); //m_cbg_sell_player
            packet.Add<UInt32>(7621499);
        }

        #endregion

    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
    }

}
