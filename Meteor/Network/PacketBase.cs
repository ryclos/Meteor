using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 * File : PacketBase
 * Author : Filipe
 * Date : 06/02/2014 10:56:07
 * Description :
 *
 */

namespace Meteor.Network
{
    public class PacketBase
    {
        /// <summary>
        /// Encoding (Thanks to uXinEmu !)
        /// </summary>
        protected readonly Encoding Encoding = Encoding.GetEncoding("gb2312");

        /// <summary>
        /// Packet memory stream
        /// </summary>
        protected MemoryStream PacketStream;

        /// <summary>
        /// Gets the bytes in the current memory stream.
        /// </summary>
        public virtual Byte[] Buffer
        {
            get
            {
                return this.PacketStream.GetBuffer();
            }
            set { }
        }

        /// <summary>
        /// Gets the size of the current memory stream.
        /// </summary>
        public Int32 Size
        {
            get
            {
                return (Int32)this.PacketStream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the pointer of the current memory stream.
        /// </summary>
        public Int32 Pointer
        {
            get
            {
                return (Int32)this.PacketStream.Position;
            }
            set
            {
                this.PacketStream.Position = value;
            }
        }
    }
}
