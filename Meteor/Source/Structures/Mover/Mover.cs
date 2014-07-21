using Meteor.IO;
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
    public abstract partial class Mover : AObject
    {
        #region FIELDS

        public String Name { get; set; }

        public Attributes Attributes { get; set; }

        public abstract Int32 Level { get; set; }

        public Single Speed { get; set; }

        /* Timers */
        protected Int64 LastMoveTime { get; set; }

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
            this.Speed = 0.1f;
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
            this.UpdateMoves();
        }

        private void UpdateMoves()
        {
            Single _distX = this.Destination.X - this.Position.X;
            Single _distZ = this.Destination.Z - this.Position.Z;
            Single _distAll = (Single)Math.Sqrt(_distX * _distX + _distZ * _distZ);
            Double _time1 = Time.GetTickFromStart() - this.LastMoveTime;
            this.LastMoveTime = Time.GetTickFromStart();
            Double _time2 = _distAll / (this.Attributes[Define.MOVE_SPEED] * 0.0006 * this.Speed);

            if (_time2 <= _time1 || _distAll == 0 || this.Position.IsInCircle(this.Destination, 0.1f))
            {
                this.Destination.Copy(ref this.Position);
                //if (this.Following == true)
                //{
                //    this.Following = false;
                //    if (this.Target != null)
                //    {
                //        this.Target.SendFollowArrival(this);
                //        if (this.Target is Drop)
                //        {
                //            (this.Target as Drop).PickUp(this);
                //        }
                //    }
                //}
            }
            else
            {
                Single _moveX = _distX * ((Single)_time1 / (Single)_time2);
                Single _moveZ = _distZ * ((Single)_time1 / (Single)_time2);
                this.Position.X += _moveX;
                this.Position.Z += _moveZ;
            }
        }

        #endregion
    }
}
