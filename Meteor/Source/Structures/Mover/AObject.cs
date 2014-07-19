using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * FObject.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 18/07/2014 15:18:10
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.Source
{
    public abstract class AObject
    {
        #region FIELDS

        private static UInt32 ObjectIdGenerator = 0;
        public UInt32 ObjectId { get; set; }
        public UInt32 ModelId { get; set; }
        public Int32 MapId { get; set; }
        public Position Position { get; set; }
        public Single Angle { get; set; }
        public Boolean Spawned { get; set; }
        public List<AObject> SpawnedObjects { get; set; }

        public ObjectType Type { get; private set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new Object
        /// </summary>
        /// <param name="type">Object Type</param>
        public AObject(ObjectType type)
        {
            this.ObjectId = ++AObject.ObjectIdGenerator;
            this.Type = type;
            this.Spawned = false;
            this.SpawnedObjects = new List<AObject>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Serialize Object header
        /// </summary>
        /// <param name="packet"></param>
        public virtual void Serialize(Packet packet)
        {
            packet.Add<UInt32>((UInt32)this.Type); //Object Type
            packet.Add<UInt32>(this.ObjectId); // Object Id
        }

        public virtual void Update() { }

        #endregion
    }

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
}
