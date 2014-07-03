using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Extention
 * Author : Filipe
 * Date : 06/02/2014 20:35:04
 * Description :
 *
 */

namespace Meteor.Source
{
    public static class Extention
    {
        public static Int32 GetUnixTimestamp(this DateTime dateTime)
        {
            return (Int32)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime()).TotalSeconds;
        }

        public static DateTime GetDateTimeFromTimeStamp(this DateTime dateTime, Int32 timeStamp)
        {
            dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
