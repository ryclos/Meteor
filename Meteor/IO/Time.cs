using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * Time.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 21/07/2014 15:56:18
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.IO
{
    public static class Time
    {
        #region FIELDS

        private static readonly Int64 Start = Environment.TickCount;
        private static readonly DateTime Utc;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize Time()
        /// </summary>
        static Time()
        {
            Utc = new DateTime(1970, 1, 1);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Get the time in seconds by date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Int64 TimeInSeconds(DateTime date)
        {
            if (date < Utc)
            {
                date = Utc;
            }
            return (Int64)(date - Utc).TotalSeconds;
        }

        /// <summary>
        /// Get the time in seconds now
        /// </summary>
        /// <returns></returns>
        public static long TimeInSeconds()
        {
            return TimeInSeconds(DateTime.UtcNow);
        }

        /// <summary>
        /// Get the number of ticks from program starts
        /// </summary>
        /// <returns></returns>
        public static Int64 GetTickFromStart()
        {
            return Environment.TickCount - Start;
        }

        /// <summary>
        /// Get current environement tick
        /// </summary>
        /// <returns></returns>
        public static Int64 GetCurrentTick()
        {
            return Environment.TickCount;
        }

        #endregion
    }
}
