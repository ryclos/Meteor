using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Attributes
 * Author : Filipe
 * Date : 15/04/2014 12:00:07
 * Description :
 *
 */

namespace Meteor.Source
{
    public class Attributes
    {
        #region FIELDS

        /// <summary>
        /// Attributes collection
        /// </summary>
        private Dictionary<UInt32, Int32> AttrbitutesData { get; set; }

        /// <summary>
        /// Gets the number of attributes
        /// </summary>
        public Int32 Length
        {
            get
            {
                return this.AttrbitutesData.Count;
            }
        }

        /// <summary>
        /// Gets the keys of the attribute collection
        /// </summary>
        public List<UInt32> Keys
        {
            get
            {
                return this.AttrbitutesData.Keys.ToList();
            }
        }

        /// <summary>
        /// Gets the values of the attribute collection
        /// </summary>
        public List<Int32> Values
        {
            get
            {
                return this.AttrbitutesData.Values.ToList();
            }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize attributes
        /// </summary>
        public Attributes()
        {
            this.AttrbitutesData = new Dictionary<UInt32, Int32>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Gets or set an attribute
        /// </summary>
        /// <param name="_key">Attribute key</param>
        /// <returns></returns>
        public Int32 this[Define _key]
        {
            get
            {
                if (this.AttrbitutesData.ContainsKey((UInt32)_key) == true)
                {
                    return this.AttrbitutesData[(UInt32)_key];
                }
                return 0;
            }
            set
            {
                if (this.AttrbitutesData.ContainsKey((UInt32)_key) == false)
                {
                    this.AttrbitutesData.Add((UInt32)_key, value);
                }
                else
                {
                    this.AttrbitutesData[(UInt32)_key] = value;
                }
            }
        }

        #endregion
    }
}
