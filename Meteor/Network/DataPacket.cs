using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 * File : DataPacket
 * Author : Filipe
 * Date : 06/02/2014 11:08:55
 * Description :
 *
 */

namespace Meteor.Network
{
    public class DataPacket : PacketBase
    {
        #region FIELDS

        private BinaryReader PacketReader { get; set; }

        public override Byte[] Buffer
        {
            get
            {
                return base.Buffer;
            }
        }

        private Dictionary<Type, Func<BinaryReader, Object>> ReadFunctions = new Dictionary<Type, Func<BinaryReader, Object>>()
        {
            { typeof(Boolean), s => s.ReadBoolean() },
            { typeof(Byte), s => s.ReadByte() },
            { typeof(Byte[]), s => s.ReadByte() },
            { typeof(Int16), s => s.ReadInt16() },
            { typeof(UInt16), s => s.ReadUInt16() },
            { typeof(Int32), s => s.ReadInt32() },
            { typeof(UInt32), s => s.ReadUInt32() },
            { typeof(Int64), s => s.ReadInt64() },
            { typeof(UInt64), s => s.ReadUInt64() },
            { typeof(Single), s => s.ReadSingle() },
            { typeof(String), s => new String(s.ReadChars(count: s.ReadUInt16())) },
        };

        #endregion

        #region CONSTRUCTORS

        public DataPacket(Byte[] buffer, Int32 offset, Int32 size)
        {
            this.PacketStream = new MemoryStream(buffer, offset, size);
            this.PacketReader = new BinaryReader(this.PacketStream, this.Encoding);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Read from stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Read<T>()
        {
            if (this.ReadFunctions.ContainsKey(typeof(T)) == true)
            {
                return (T)this.ReadFunctions[typeof(T)](this.PacketReader);
            }
            
            throw new NotImplementedException("This type is not implemented.");
        }

        /// <summary>
        /// Read bytes
        /// </summary>
        /// <param name="count">Bytes to read</param>
        /// <returns>Byte array</returns>
        public Byte[] ReadBytes(Int32 count)
        {
            return this.PacketReader.ReadBytes(count);
        }

        /// <summary>
        /// Free all resources
        /// </summary>
        public void Free()
        {
            this.PacketStream.Close();
            this.PacketStream.Dispose();
            this.PacketStream = null;

            this.PacketReader.Close();
            this.PacketReader.Dispose();
            this.PacketReader = null;
        }

        #endregion
    }
}
