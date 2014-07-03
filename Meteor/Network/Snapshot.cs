using Meteor.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 * File : Snapshot
 * Author : Filipe
 * Date : 12/02/2014 19:25:09
 * Description :
 * Based on uXinEmu Snapshot class
 */

namespace Meteor.Network
{
    public class Snapshot : Packet
    {
        #region FIELDS

        private UInt16 SnapshotCount { get; set; }

        public override byte[] Buffer
        {
            get
            {
                Int64 _position = this.PacketWriter.BaseStream.Position;

                this.PacketWriter.BaseStream.Seek(6, SeekOrigin.Begin);
                this.PacketWriter.Write(this.SnapshotCount);
                this.PacketWriter.BaseStream.Seek(_position, SeekOrigin.Begin);
                return base.Buffer;
            }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Starts a basic snapshot
        /// </summary>
        public Snapshot()
            : base(Header.SNAPSHOT)
        {
            this.SnapshotCount = 0;
            this.Add<UInt16>(0); // Snapshot count
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Starts a new Snapshot
        /// </summary>
        /// <param name="type">Snapshot type</param>
        public void StartSnapshot(SnapshotType type)
        {
            if (type < (SnapshotType)128) // less than a 'Char' or 'Byte' (8bits)
            {
                this.Add<Byte>((Byte)type);
            }
            else
            {
                this.Add<Byte[]>(BitConverter.GetBytes((ushort)type).Reverse().ToArray());
            }
            this.SnapshotCount++;
        }

        #endregion
    }
}
