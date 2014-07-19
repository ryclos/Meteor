using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * Mover.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 18/07/2014 16:31:08
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.Source
{
    public class Mover : AObject
    {
        #region FIELDS

        public String Name { get; set; }

        public Attributes Attributes { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new Mover
        /// </summary>
        /// <param name="type">Object type</param>
        public Mover(ObjectType type)
            : base(type)
        {
            this.Attributes = new Attributes();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Serialize mover
        /// </summary>
        /// <param name="packet"></param>
        public override void Serialize(Packet packet)
        {
            base.Serialize(packet);
        }

        /// <summary>
        /// Update mover
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        #endregion
    }
}
