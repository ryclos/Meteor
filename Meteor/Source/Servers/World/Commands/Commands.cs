using Meteor.IO;
using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteor.Source
{
    public partial class Client
    {
        enum Authority
        {
            GENERAL = 1, RESPECTOR = 2, HIGHLEVEL = 3, GAMEMASTER = 4, OPERATOR = 5, SECURITY = 6, ADMINISTRATOR = 7
        }

        enum ChatFlags
        {
            None = 0,
            ShortWords = 1
        }

        /// <summary>
        /// On chat
        /// </summary>
        /// <param name="dp"></param>
        private void OnChat(DataPacket dp)
        {
            Byte _chatFlag = dp.Read<Byte>();
            String _text = dp.Read<String>();
            String _infoText = dp.Read<String>();
            String[] _chatCommand = _text.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (_chatCommand.Length == 0)
            {
                return;
            }
            if (_chatCommand[0].Trim().StartsWith("\0.") == true)
            {
                if (this.Player.Account.Authority >= (Int32)Authority.GAMEMASTER)
                {
                    switch (_chatCommand[0])
                    {
                        case ".createitem": this.OnCreateItem(_chatCommand); break;
                        default: Log.Write(LogType.Error, "Unknow command {0}", _chatCommand[0]); break;
                    }
                }
            }
            else
            {
                Snapshot _snap = new Snapshot();
                _snap.StartSnapshot(SnapshotType.NORMALCHAT);
                _snap.Add<UInt32>(this.Player.ObjectId);
                _snap.Add<Int32>(this.Player.Id);
                _snap.Add<Byte>((Byte)ChatFlags.None);
                _snap.Add<String>(_text);
                _snap.Add<String>("");
                _snap.Add<Byte>(0); // issf (???)
                this.SendToVisiblePlayers(_snap);

                //snapshot.SetType(SnapshotType.GET_CLIENT_INFO);

                //snapshot.WriteUInt32(1337); //Source Player Id
                //snapshot.WriteString(splitString[0]);
                //snapshot.WriteString(splitString.Length > 1 ? splitString.Skip(1).Aggregate("", (current, s) => current + s) : "");

                //Snapshot _snap = new Snapshot();
                //_snap.StartSnapshot(SnapshotType.NORMALCHAT);
                //_snap.Add<Byte>((Byte)ChatFlags.None);
                //_snap.Add<String>(_text);
                //_snap.Add<String>("");
                //this.SendToVisiblePlayers(_snap);
            }
        }

        /// <summary>
        /// GM commands
        /// </summary>
        /// <param name="dp"></param>
        private void OnGmCommand(DataPacket dp)
        {
            String command = dp.Read<String>();
            Log.Write(LogType.Debug, command);
            string[] c = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (c.Length == 0)
            {
                return;
            }
            if (this.Player.Account.Authority >= (int)Authority.GAMEMASTER)
            {
                switch (c[0].ToLower())
                {
                    case "createitem": this.OnCreateItem(c); break;
                    default: Log.Write(LogType.Error, "Unknow GM Command '{0}'", command); break;
                }
            }
        }

        #region COMMANDS

        private void OnCreateItem(String[] c)
        {
            if (c[1] == "?" || c.Length < 2)
            {
                Log.Write(LogType.Debug, "CreateItem command usage : .createitem item_id [stack = 1]");
                return;
            }
            Int32 _itemId = 0;
            UInt32 _stack = 1;
            if (Int32.TryParse(c[1], out _itemId) == false)
            {
                Log.Write(LogType.Debug, "CreateItem => Item id not a number");
                return;
            }
            if (c[2] != null && UInt32.TryParse(c[2], out _stack) == false)
            {
                Log.Write(LogType.Debug, "CreateItem => Stack size not a number");
                return;
            }
            Item _newItem = new Item(_itemId, this.Player.Id);
            _newItem.Quantity = _stack;
            this.Player.Inventory.CreateItem(_newItem);
        }

        #endregion
    }
}
