using Meteor.Source.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 * File : Positions
 * Author : Filipe
 * Date : 08/02/2014 16:18:05
 * Description :
 *
 */

namespace Meteor.Source
{
    public class Position
    {
        #region FIELDS

        public Single X { get; set; }
        public Single Y { get; set; }
        public Single Z { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new position object
        /// </summary>
        public Position() 
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        /// <summary>
        /// Initialize a new position with values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
        public Position(Single x, Single y, Single z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Returns the distance from this position and an other position
        /// </summary>
        /// <param name="posTo">Point dont on veux estimer la distance depuis la position actuelle</param>
        /// <returns></returns>
        public Double DistanceTo(Position posTo)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(this.X - posTo.X, 2) + Math.Pow(this.Y - posTo.Y, 2) + Math.Pow(this.Z - posTo.Z, 2)));
        }

        public bool IsInRange(Position posTo, int range)
        {
            return DistanceTo(posTo) <= range;
        }

        public bool IsZero()
        {
            return X == 0 && Y == 0 && Z == 0;
        }

        /// <summary>
        /// Writes the Position data into the BinaryWriter passed as parameter
        /// </summary>
        /// <param name="writer">Packet writer</param>
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.X);
            writer.Write(this.Y);
            writer.Write(this.Z);
        }

        public String ToString()
        {
            return string.Format("{0} {1} {2}", this.X, this.Y, this.Z);
        }

        #endregion

        #region OPERATORS

        public static Position operator +(Position posA, Position posB)
        {
            return new Position(posA.X + posB.X,
                posA.Y + posB.Y,
                posA.Z + posB.Z);
        }

        public static Position operator -(Position posA, Position posB)
        {
            return new Position(posA.X - posB.X,
                posA.Y - posB.Y,
                posA.Z - posB.Z);
        }

        public static Position operator *(Position posA, float B)
        {
            return new Position(posA.X * B,
                posA.Y * B,
                posA.Z * B);
        }

        public static Position Clone(Position pos)
        {
            return new Position(pos.X, pos.Y, pos.Z);
        }

        //public void RandomPointInCircle(float fRadius)
        //{
        //    float fAngle = (float)(Utility.RandomF(0F, 360F) * Math.PI / 180F);
        //    float fPower = Utility.RandomF(0F, fRadius);
        //    X += (float)Math.Sin(fAngle) * fPower;
        //    Z -= (float)Math.Cos(fAngle) * fPower;
        //}
        
        #endregion
    }
}
