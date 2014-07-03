using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

/*
 * File : Character
 * Author : Filipe
 * Date : 06/02/2014 19:16:44
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Load the character list of the account
        /// </summary>
        public void LoadCharacterList()
        {
            ResultSet _set = new ResultSet(Query.SELECT_CHARACTERS, this.User.AccountId);
            this.Characters.Clear();
            while (_set.Reading())
            {
                this.Characters.Add(new Character(_set, this));
            }
            _set.Free();
            
            _set = new ResultSet(Query.SELECT_DELETED_CHARACTERS, this.User.AccountId); // TODO: 30+ days ignore (number days in config.ini file ?)
            this.DeletedCharacters.Clear();
            while (_set.Reading() == true)
            {
                this.DeletedCharacters.Add(new Character(_set, this));
            }
            _set.Free();

            foreach (Character _char in this.Characters)
            {
                _char.Inventory.LoadCloset();
            }
        }

        /// <summary>
        /// Creates a character
        /// </summary>
        /// <param name="dp">Incoming packet</param>
        public void OnCreateCharacter(DataPacket dp)
        {
            String _name = dp.Read<String>();
            String _hdHash = dp.Read<String>();
            Int32 _slot = dp.Read<Int32>();
            Byte _job = dp.Read<Byte>();
            Byte _gender = dp.Read<Byte>();
            Byte _hairMesh = dp.Read<Byte>();
            UInt32 _hairColor = dp.Read<UInt32>();
            Byte _headMesh = dp.Read<Byte>();
            Int32 _city = dp.Read<Int32>();
            Int32 _zodiacSign = dp.Read<Int32>();
            Byte _country = dp.Read<Byte>();
            String _snCard = dp.Read<String>();
            Int32 _cardType = dp.Read<Int32>();
            String _hdSerialNumber = dp.Read<String>();
            String _binAccount = dp.Read<String>();
            Int32[] _clothList = new Int32[5];
            for (Int32 i = 0; i < 5; ++i)
            {
                _clothList[i] = dp.Read<Int32>();
            }

            if (Helper.CharacterNameExists(_name) == true)
            {
                this.SendCharacterCreationResult(Error.ERR_PLAYER_EXIST);
                return;
            }
            else
            {
                // create character
                Character _character = new Character(this);

                _character.Account = this.User;
                _character.Name = _name;
                _character.Slot = _slot;
                _character.Job = _job;
                _character.Attributes[Define.LV] = 1;
                _character.Attributes[Define.GOLD] = 0;
                _character.Attributes[Define.EP] = 0;
                _character.Gender = (Gender)_gender;
                _character.HairMesh = _hairMesh;
                _character.HairColor = (Int32)_hairColor;
                _character.HeadMesh = _headMesh;

                _character.Attributes[Define.STR] = 15;
                _character.Attributes[Define.STA] = 15;
                _character.Attributes[Define.DEX] = 15;
                _character.Attributes[Define.INT] = 15;
                _character.Attributes[Define.SPI] = 15;

                // TODO: Find a way to calculate the HP and MP max with level and Stamina
                _character.Attributes[Define.HP] = 100;
                _character.Attributes[Define.MP] = 100;

                for (Int32 i = 0; i < _clothList.Length; i++)
                {
                    _character.Inventory.Closet.Add(new ClosetItem()
                    {
                        Id = _clothList[i],
                        Index = i + 1,
                        Equiped = true,
                        Level = 1,
                        Date = DateTime.UtcNow
                    });
                }


                _character.MapId = Configuration.Get<Int32>("StartMapId");
                _character.Position = new Position(Configuration.Get<Int32>("StartPosX"), 
                    Configuration.Get<Int32>("StartPosY"), Configuration.Get<Int32>("StartPosZ"));
                _character.InsertIntoDatabase();

                this.SendCharacterCreationResult(Error.ERR_SUCCESS);
                this.LoadCharacterList();
                this.SendCharacterData(_character);
            }
        }

        /// <summary>
        /// Delete a character
        /// </summary>
        /// <param name="dp">Incoming packet</param>
        public void OnDeleteCharacter(DataPacket dp)
        {
            Int32 _id = dp.Read<Int32>(); // Character id
            String passwordbefore = dp.Read<String>();

            if (passwordbefore != "111111")
            {
                // Send delete error
                Packet _packet = new Packet(Header.DELETEPLAYERRESULT);                
                _packet.Add<Int32>((Int32)Error.ERR_PASSWORD);
                _packet.Add<Int32>((Int32)_id);
                _packet.Add<Int32>((Int32)0);
                this.Send(_packet);
                return;
            }
            foreach (Character _character in this.Characters)
            {
                if (_character.Id == _id)
                {
                    SQL.ExecuteQuery(Query.DELETE_CHARACTER, this.User.AccountId, _id, new DateTime().GetDateTimeFromTimeStamp(0).ToString());                    
                    this.SendCharacterDelete(_character.Id);
                    this.LoadCharacterList();
                  foreach (Character _char in this.Characters)
                    {
                        this.SendCharacterData(_char);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Resume a character
        /// </summary>
        /// <param name="dp">Incoming packet</param>
        public void OnResumeCharacter(DataPacket dp)
        {
            Int32 _cmd = dp.Read<Int32>();
            Int32 _playerId = dp.Read<Int32>();

            if (_cmd == 0)
            {
                this.SendCharacterResumeList();
            }
            else
            {
                Character _restoreCharacter;
                foreach (Character _character in this.DeletedCharacters)
                {
                    if (_character.Id == _playerId)
                    {
                        _restoreCharacter = _character;
                        break;
                    }
                }
                if (this.Characters.Count >= 3)
                {
                    // TODO: send error
                    return;
                }

                // Get free slot
                Int32 _slot = 0;
                Byte[] _chars = new Byte[3];
                foreach (Character _character in this.Characters)
                {
                    _chars[_character.Slot] = 1;
                }
                if (_chars[0] == 0)
                {
                    _slot = 0;
                }
                else if (_chars[1] == 0)
                {
                    _slot = 1;
                }
                else if (_chars[2] == 0)
                {
                    _slot = 2;
                }
                SQL.ExecuteQuery(Query.RESUME_CHARACTER, this.User.AccountId, _playerId, new DateTime().GetDateTimeFromTimeStamp(0).ToString(), _slot);
                this.LoadCharacterList();
                foreach (Character _char in this.Characters)
                {
                    this.SendCharacterData(_char);
                }
                this.SendCharacterResume();
            }
        }

        /// <summary>
        /// Join game
        /// </summary>
        /// <param name="dp"></param>
        public void OnJoinGame(DataPacket dp)
        {
            Int32 _playerId = dp.Read<Int32>();
            Byte _patch = dp.Read<Byte>(); // ar:WriteByte(CWndMgr.m_patch_ver)
            String _hdInfo = dp.Read<String>(); // ar:WriteString(hd_info)
            ResultSet _set = new ResultSet(Query.GET_CHARACTER_BY_ID, _playerId);

            if (_set.Reading() == true)
            {
                this.Player = new Character(_set, this);
                _set.Free();
                this.Player.Inventory.LoadInventory();
                this.Player.Inventory.LoadCloset();

                this.Player.ObjectId = 15;

                SendJoinRight();
                SendSpawnSelf();
                
                // Add to world (for visibility)
            }
            else
            {
                this.Disconnect();
                _set.Free();
            }
            
        }

        public void SendJoinRight()
        {
            Packet _packet = new Packet(Header.JOIN_RIGHT);
            _packet.Add<Int32>(this.Player.Id);
            _packet.Add<Int32>(this.Player.MapId);
            _packet.Add<Position>(this.Player.Position);
            _packet.Add<Boolean>(false); //Is festival
            _packet.Add<Int32>(0); //Festival 'yday'
            this.Send(_packet);
        }

        public void SendSpawnToOthers(Client c)
        {
            Snapshot _snapShotNearPlayers = new Snapshot();
            Player.Serialize(_snapShotNearPlayers);            
            c.Send(_snapShotNearPlayers);
        }

        public void SendSpawnSelf()
        {
            Snapshot _snapshot = new Snapshot();
            _snapshot.StartSnapshot(SnapshotType.SET_STATE_VER);
            _snapshot.Add<UInt32>(1);
            _snapshot.StartSnapshot(SnapshotType.UPDATE_SERVER_TIME);
            _snapshot.Add<Int32>(DateTime.UtcNow.GetUnixTimestamp());
            this.Player.Serialize(_snapshot, true);
            this.Send(_snapshot);

            this.SendPlayerData();
        }
    }
}
