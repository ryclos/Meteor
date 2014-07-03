using LuaInterface;
using Meteor.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : DataLoader
 * Author : Filipe
 * Date : 15/04/2014 14:53:51
 * Description :
 *
 */

namespace Meteor.Source
{
    public class DataLoader
    {
        #region FIELDS

        public Dictionary<Int32, ItemData> ItemsData { get; private set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new DataLoader
        /// </summary>
        public DataLoader() 
        {
            this.ItemsData = new Dictionary<Int32, ItemData>();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Load all data
        /// </summary>
        /// <returns>true on success; fail on failure</returns>
        public Boolean LoadAll()
        {
            try
            {
                DateTime _time = DateTime.Now;
                this.LoadItems();
                Log.Write(LogType.Info, "Resources loaded in {0}s\n", Math.Round((DateTime.Now - _time).TotalSeconds, 0));
            }
            catch (Exception e)
            {
                Log.Write(LogType.Error, "Cannot load Data. {0}", e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Load items
        /// </summary>
        private void LoadItems()
        {
            Log.Write(LogType.Info, "Loading items...");
            Log.Write(LogType.Loading, "Loading item files...");
            String _addonsPath = Configuration.Get<String>("AddonsPath");
            Lua _lua = new Lua();
            _lua.DoFile(_addonsPath + "/DefineText.lua");
            _lua.DoFile(_addonsPath + "/DefineBuff.lua");
            _lua.DoFile(_addonsPath + "/jobs.lua");
            _lua.DoFile(_addonsPath + "/propItem.lua");
            LuaTable _items = _lua.GetTable("propItem");

            if (_items != null)
            {
                foreach (DictionaryEntry _entry in _items)
                {
                    Int32 _itemId = (Int32)((Double)_entry.Key);
                    if (this.ItemsData.ContainsKey(_itemId) == false)
                    {
                        try
                        {
                            ItemData _itemData = new ItemData((LuaTable)_entry.Value);
                            if (_itemData != null)
                            {
                                this.ItemsData.Add(_itemId, _itemData);
                            }
                        }
                        catch { }
                    }
                    Log.Write(LogType.Loading, "{0} items loaded...", this.ItemsData.Count);
                }
            }
            _lua.Close();
            Log.Write(LogType.Done, "{0} items loaded!\t\n", this.ItemsData.Count);
        }

        #endregion
    }
}
