using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : PlayerPackets
 * Author : Filipe
 * Date : 15/02/2014 15:26:14
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Packet envoyé au client pour indiquer qu'une objet n'est plus visible
        /// </summary>
        /// <param name="objid">ID Unique de l'objet</param>
        public void SendRemoveObject(UInt32 objid)
        {
            Snapshot _snap = new Snapshot();
            _snap.StartSnapshot(SnapshotType.REMOVEOBJ);
            _snap.Add<UInt32>(objid);
            this.Send(_snap);
        }

        public void SendSetValue(uint OBJID, ushort Attrib, Int32 val)
        {
            Snapshot _snap = new Snapshot();
            _snap.StartSnapshot(SnapshotType.SET_VAL);
            _snap.Add<UInt32>(OBJID);
            _snap.Add<UInt16>(Attrib);
            _snap.Add<Int32>(val);
            Send(_snap);
        }

        public void SendPlayerData()
        {
            Snapshot _snap = new Snapshot();
            List<UInt32> _key = this.Player.Attributes.Keys;
            List<Int32> _value = this.Player.Attributes.Values;
            for (Int32 i = 0; i < this.Player.Attributes.Length; ++i)
            {
                if (_value[i] != 0)
                {
                    _snap.StartSnapshot(SnapshotType.SET_VAL);
                    _snap.Add<UInt32>(this.Player.ObjectId);
                    _snap.Add<UInt16>((UInt16)_key[i]);
                    _snap.Add<Int32>(_value[i]);
                }
            }
            Send(_snap); 
        }
    }
}
