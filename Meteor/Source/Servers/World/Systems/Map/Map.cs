using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*--------------------------------------------------------
 * Map.cs - file description
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 18/07/2014 16:11:23
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace Meteor.Source
{
    public class Map
    {
        #region FIELDS

        public Int32 Id { get; set; }
        public String Name { get; set; }

        public List<Character> Players { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new Map
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Map(Int32 id, String name)
        {
            this.Id = id;
            this.Name = name;
            this.Players = new List<Character>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Add a T object to the map
        /// </summary>
        /// <typeparam name="T">Object type (Client, npc, monster, drop)</typeparam>
        /// <param name="value">Object value</param>
        public void Add<T>(T value)
        {
            if (typeof(T) == typeof(Character))
            {
                this.Players.Add(value as Character);
            }
        }

        /// <summary>
        /// Delete a T object from the map
        /// </summary>
        /// <typeparam name="T">Object type (Client, npc, monster, drop)</typeparam>
        /// <param name="value">Object value</param>
        public void Delete<T>(T value)
        {
            if (typeof(T) == typeof(Character))
            {
                this.Players.Remove(value as Character);
                foreach (Character character in this.Players)
                {
                    if (character.GetHashCode() != (value as Character).GetHashCode())
                    {
                        if (character.SpawnedObjects.Contains(value as Character) == true)
                        {
                            character.SpawnedObjects.Remove(value as Character);
                            character.Client.DespawnObject(value as Character);
                            character.Spawned = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete the map
        /// </summary>
        public void DeleteAll()
        {
            this.Players.Clear();
        }

        /// <summary>
        /// Update the map
        /// </summary>
        public void Update()
        {
            this.UpdatePlayerVisibility();
        }

        private void UpdatePlayerVisibility()
        {
            foreach (Character c in this.Players)
            {
                if (c.Spawned == false)
                {
                    continue;
                }
                c.Update();

                /* Other player visibility */
                foreach (Character otherCharacter in this.Players)
                {
                    if (otherCharacter.Spawned == false || c.GetHashCode() == otherCharacter.GetHashCode())
                    {
                        continue;
                    }
                    if (c.Position.IsInCircle(otherCharacter.Position, 100) == true)
                    {
                        if (c.SpawnedObjects.Contains(otherCharacter) == false)
                        {
                            c.Client.SpawnObject(otherCharacter);
                        }
                    }
                    else
                    {
                        c.Client.DespawnObject(otherCharacter);
                    }
                }
            }
        }

        #endregion
    }
}
