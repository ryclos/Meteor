using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * MapManager.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 18/07/2014 16:10:46
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.Source
{
    public class MapManager
    {
        #region FIELDS

        private Dictionary<Int32, Map> Maps { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new MapManager
        /// </summary>
        public MapManager()
        {
            this.Maps = new Dictionary<Int32, Map>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Add a new Map
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="name">Map name</param>
        public void Add(Int32 id, String name)
        {
            Map _map = new Map(id, name);
            this.Maps.Add(id, _map);
        }

        /// <summary>
        /// Delete a map with id
        /// </summary>
        /// <param name="id">Map id</param>
        public void Delete(Int32 id)
        {
            this.Maps[id].DeleteAll();
            this.Maps.Remove(id);
        }

        /// <summary>
        /// Update all maps
        /// </summary>
        public void Update()
        {
            foreach (Map map in this.Maps.Values)
            {
                map.Update();
            }
        }

        public Map this[Int32 id]
        {
            get
            {
                if (this.Maps.ContainsKey(id) == true)
                {
                    return this.Maps[id];
                }
                return null;
            }
            set
            {
                if (this.Maps.ContainsKey(id) == true)
                {
                    this.Maps[id] = value;
                }
                else
                {
                    this.Maps.Add(id, value);
                }
            }
        }

        #endregion
    }
}
