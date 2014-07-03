using Meteor.IO;
using Meteor.Source;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

/*
 * File : Packet
 * Author : Filipe
 * Date : 06/02/2014 10:51:54
 * Description :
 *
 */

namespace Meteor.Network
{
    public class Packet : PacketBase
    {
        #region FIELDS

        protected BinaryWriter PacketWriter { get; set; }

        public override Byte[] Buffer
        {
            get
            {
                Int32 _size = Size;

                this.PacketWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                this.PacketWriter.Write(_size);
                this.PacketWriter.BaseStream.Seek(_size, SeekOrigin.Begin);
                return ((MemoryStream)this.PacketWriter.BaseStream).GetBuffer();
            }
        }

        private Dictionary<Type, Action<BinaryWriter, Object>> WriteFunctions = new Dictionary<Type, Action<BinaryWriter, Object>>()
        {
            { typeof(Boolean), (s, b) => s.Write(Boolean.Parse(b.ToString())) },
            { typeof(Byte), (s, b) => s.Write(Byte.Parse(b.ToString())) },
            { typeof(SByte), (s, b) => s.Write(SByte.Parse(b.ToString())) },
            { typeof(Int16), (s, b) => s.Write(Int16.Parse(b.ToString())) },
            { typeof(UInt16), (s, b) => s.Write(UInt16.Parse(b.ToString())) },
            { typeof(Int32), (s, b) => s.Write(Int32.Parse(b.ToString())) },
            { typeof(UInt32), (s, b) => s.Write(UInt32.Parse(b.ToString())) },
            { typeof(Int64), (s, b) => s.Write(Int64.Parse(b.ToString())) },
            { typeof(UInt64), (s, b) => s.Write(UInt64.Parse(b.ToString())) },
            { typeof(Single), (s, b) => s.Write(Single.Parse(b.ToString())) },
            { typeof(String), 
                (s, b) => 
                    {
                        s.Write((UInt16)b.ToString().Length);
                        if (b.ToString().Length > 0)
                            s.Write(Encoding.ASCII.GetBytes(b.ToString()));
                    } 
            },
            { typeof(Byte[]), (s, b) => s.Write((Byte[])b) },
            { typeof(Position), (s, b) => { ((Position)b).Serialize(s); } },
        };

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new packet
        /// </summary>
        /// <param name="header">Packet header</param>
        public Packet(Int16 header)
        {
            this.PacketStream = new MemoryStream();
            this.PacketWriter = new BinaryWriter(this.PacketStream, this.Encoding);
            this.Add<Int32>(0);
            this.Add<Int16>(header); // header
        }

        /// <summary>
        /// Initialize a new packet
        /// </summary>
        /// <param name="header">Packet header</param>
        public Packet(Header header)
            : this((Int16)header)
        {
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Add value to the packet
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="value">Value to add</param>
        public void Add<T>(T value)
        {
            if (this.WriteFunctions.ContainsKey(typeof(T)) == true)
            {
                this.WriteFunctions[typeof(T)](this.PacketWriter, value);
                return;
            }
            throw new NotImplementedException("This type is not implemented.");
        }

        public void WriteX(int value)
        {
            if (value == 0)
            {
                this.Add<UInt16>(1);
                //WriteUInt16(1);

                return;
            }

            BitArray bits;

            var temp = new BitArray(new[] { value });

            temp = BitWriter.TrimZero(temp, false);
            if (temp.Count <= 4)
            {
                bits = BitWriter.TrimZero(BitWriter.AddFront(temp, 2), true);


            }
            else if (temp.Count <= 13)
            {
                bits = BitWriter.TrimZero(BitWriter.AddFront(temp, 2), true);

                bits[0] = true;
            }
            else
            {
                bits = BitWriter.TrimZero(BitWriter.AddFront(temp, 8), true);

                bits[1] = true;
            }

            var bytes = BitWriter.GetBytes(BitWriter.TrimZero(bits, true));

            if (bytes.Length == 4)
                bytes = BitWriter.GetBytes(BitWriter.AddBack(BitWriter.TrimZero(bits, true), 8));
            this.Add<Byte[]>(bytes);
            //WriteBytes(bytes);
        }

        /// <summary>
        /// Free all the resources
        /// </summary>
        public void Free()
        {
            this.PacketStream.Close();
            this.PacketStream.Dispose();
            this.PacketStream = null;

            this.PacketWriter.Close();
            this.PacketWriter.Dispose();
            this.PacketWriter = null;
        }

        #endregion
    }
}
