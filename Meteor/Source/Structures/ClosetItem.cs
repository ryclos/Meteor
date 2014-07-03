using Meteor.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

/*
 * File : Closet
 * Author : Filipe
 * Date : 18/02/2014 11:49:47
 * Description :
 *
 */

namespace Meteor.Source
{
    public class ClosetItem
    {
        #region FIELDS

        public Int32 Id { get; set; }

        public Int32 CharacterId { get; set; }

        public Int32 Index { get; set; }

        public Int32 Level { get; set; }

        public Boolean Equiped { get; set; }

        public DateTime Date { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Create a new Closet item
        /// </summary>
        public ClosetItem()
        {
            this.Level = 1;
        }

        /// <summary>
        /// Create a new Closet item from database field
        /// </summary>
        /// <param name="set">Database result set (field)</param>
        public ClosetItem(ResultSet set)
        {
            this.Id = set.Read<Int32>("Id");
            this.CharacterId = set.Read<Int32>("CharacterId");
            this.Index = set.Read<Int32>("Slot");
            this.Level = set.Read<Int32>("Level");
            this.Equiped = set.Read<Boolean>("Equiped");
            this.Date = DateTime.Parse(set.Read<String>("Date"),CultureInfo.GetCultureInfo("en"));
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Insert the closet item in database
        /// </summary>
        public void Insert()
        {
            SQL.ExecuteQuery("INSERT INTO closet (Id, CharacterId, Slot, Level, Equiped, Date) VALUES ({0}, {1}, {2}, {3}, {4}, '{5}')",
                this.Id, this.CharacterId, this.Index, this.Level, this.Equiped, this.Date.ToString(CultureInfo.GetCultureInfo("en")));
        }

        /// <summary>
        /// Save the Closet item in database
        /// </summary>
        public void Save()
        {
            SQL.ExecuteQuery("UPDATE closet SET Id={0}, Slot={1}, Level={2}, Equiped={3}, Date='{4}' WHERE CharacterId = {5} AND Slot={1}",
                this.Id, this.Index, this.Level, this.Equiped, this.Date.ToString(CultureInfo.GetCultureInfo("en")), this.CharacterId);
        }

        #endregion
    }
}
