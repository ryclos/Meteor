using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : CharacterPackets
 * Author : Filipe
 * Date : 06/02/2014 19:16:53
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Send the character list of the current client
        /// </summary>
        public void SendCharacterList()
        {
            Packet _packet = new Packet(Header.PLAYERLIST);
            _packet.Add<Int32>(DateTime.UtcNow.GetUnixTimestamp());
            _packet.Add<Byte>(0); //AcceptedAgreement
            _packet.Add<Byte>((Byte)this.Characters.Count); // count 
            _packet.Add<UInt32>(0); //m_cbg_sell_player
            _packet.Add<Int32>(0);//cbg_ordersn
            _packet.Add<UInt32>(0);//m_cbg_sign_player
            _packet.Add<Int32>(0);//m_cbg_sign_time
            foreach (Character _character in this.Characters)
            {
                _packet.Add<Int32>(_character.Slot);
                _packet.Add<String>(_character.Name);
                _packet.Add<Int32>(_character.Id);
                _packet.Add<Int32>(_character.MapId);
                _packet.Add<Byte>((Byte)_character.Gender);
                _packet.Add<Position>(_character.Position);
                _packet.Add<Int32>(_character.Attributes[Define.LV]);
                _packet.Add<Int32>(_character.Job);

                /* Attr */
                _packet.Add<Int32>(_character.Attributes[Define.STR]);
                _packet.Add<Int32>(_character.Attributes[Define.STA]);
                _packet.Add<Int32>(_character.Attributes[Define.DEX]);
                _packet.Add<Int32>(_character.Attributes[Define.INT]);
                _packet.Add<Int32>(_character.Attributes[Define.SPI]);

                /* Apparence */
                _packet.Add<Int32>(_character.HairMesh); // hair mesh
                _packet.Add<UInt32>((UInt32)_character.HairColor); // hair color
                _packet.Add<Int32>(_character.HeadMesh); // head mesh

                _packet.Add<Int32>(0); // blocked or not
                _packet.Add<Int32>(0); // block time
               
                _packet.Add<Int32>(0); // item count
                //to do item :
                /*for i = 0, NEUZ.MAX_HUMAN_PARTS - 1 do
              player.equip_info[i] = equip_info_t.new()
            end
            local parts, item_id, flag, attr = nil, nil, nil, nil
            local count = LAr:ReadInt(ar)
            for i = 1, count do
              parts = LAr:ReadByte(ar)
              item_id = LAr:ReadDword(ar)
              flag = LAr:ReadDword(ar)
              attr = LAr:ReadInt(ar)
              if parts >= 0 and parts < NEUZ.MAX_HUMAN_PARTS then
                player.equip_info[parts].m_itemid = item_id
                player.equip_info[parts].m_word = flag
                player.equip_info[parts].m_option = attr
              end
            end*/
                foreach (ClosetItem _closet in _character.Inventory.GetEquipedCloset())
                {
                    _packet.Add<Int32>(_closet.Id);
                }
            }

            _packet.Add<Int32>(this.DeletedCharacters.Count);
            _packet.Add<Int32>(0); //m_city_id
            _packet.Add<Int32>(0); //m_province_id
            _packet.Add<SByte>(0); //m_realname
            string message = "";
            
            this.Send(_packet);

            foreach (Character _char in this.Characters)
            {
                this.SendCharacterData(_char);
            }
        }

        /// <summary>
        /// Send character data
        /// </summary>
        /// <param name="character"></param>
        public void SendCharacterData(Character character)
        {
            Packet _packet = new Packet(Header.PLAYERSLOT);
            _packet.Add<Int32>(character.Slot);
            _packet.Add<String>(character.Name);
            _packet.Add<Int32>(character.Id);
            _packet.Add<Int32>(character.MapId);
            _packet.Add<Byte>((Byte)character.Gender);
            _packet.Add<Position>(character.Position);
            _packet.Add<Int32>(character.Attributes[Define.LV]);
            _packet.Add<Int32>(character.Job);

            /* Attr */
            _packet.Add<Int32>(character.Attributes[Define.STR]);
            _packet.Add<Int32>(character.Attributes[Define.STA]);
            _packet.Add<Int32>(character.Attributes[Define.DEX]);
            _packet.Add<Int32>(character.Attributes[Define.INT]);
            _packet.Add<Int32>(character.Attributes[Define.SPI]);

            /* Apparence */
            _packet.Add<Int32>(character.HairMesh); // hair mesh
            _packet.Add<UInt32>((UInt32)character.HairColor); // hair color
            _packet.Add<Int32>(character.HeadMesh); // head mesh

            _packet.Add<Int32>(0); // blocked or not
            _packet.Add<Int32>(0); // block time

            _packet.Add<Int32>(0); // item count

            foreach (ClosetItem _closet in character.Inventory.GetEquipedCloset())
            {
                _packet.Add<Int32>(_closet.Id);
            }
            _packet.Add<Int32>(this.DeletedCharacters.Count);
            this.Send(_packet);
        }

        /// <summary>
        /// Send the character creation result
        /// </summary>
        /// <param name="result">Result of the character creation</param>
        public void SendCharacterCreationResult(Error result)
        {
            Packet _packet = new Packet(Header.CREATEPLAYERRESULT);
            _packet.Add<Int32>((Int32)result);
            this.Send(_packet);
        }

        /// <summary>
        /// Send the character delete result
        /// </summary>
        /// <param name="id"></param>
        public void SendCharacterDelete(Int32 id)
        {
            Packet _packet = new Packet(Header.DELETEPLAYERRESULT);
            _packet.Add<Int32>((Int32)Error.ERR_SUCCESS);
            _packet.Add<Int32>(id);
            _packet.Add<Int32>(2);
            this.Send(_packet);
        }

        /// <summary>
        /// Send the character resume list
        /// </summary>
        public void SendCharacterResumeList()
        {
            Packet _packet = new Packet(Header.RESUME_LIST);
            _packet.Add<Int32>(this.DeletedCharacters.Count);
            foreach (Character _character in this.DeletedCharacters)
            {
                _packet.Add<String>(_character.Name);
                _packet.Add<Int32>(_character.Id);
                _packet.Add<Int32>((Int32)_character.Gender);
                _packet.Add<Int32>(_character.Job);
                _packet.Add<Int32>(_character.Attributes[Define.LV]);
                _packet.Add<Int32>(0); // Is back (not back yet)
                _packet.Add<Int32>(_character.DeletedDate.GetUnixTimestamp()); // Time
            }
            this.Send(_packet);
        }

        /// <summary>
        /// Send character resume success
        /// </summary>
        public void SendCharacterResume()
        {
            Packet _packet = new Packet(Header.GETBACK_RESULT);
            _packet.Add<Int32>(1);
            _packet.Add<Int32>(1);
            this.Send(_packet);
        }
    }
}
