using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Query
 * Author : Filipe
 * Date : 06/02/2014 17:59:49
 * Description :
 * This file regroup all queries of the emulator
 * as const strings with the correct format
 */

namespace Meteor.Source
{
    public class Query
    {
        // Login
        public const String SELECT_LOGIN = "SELECT * FROM accounts WHERE Username = '{0}'";
        
        // Character
        public const String SELECT_CHARACTERS = "SELECT characters.* FROM characters LEFT JOIN accounts ON characters.AccountId = accounts.Id WHERE accounts.Id = {0} AND characters.AccountId != -1;";
        public const String SELECT_DELETED_CHARACTERS = "SELECT characters.* FROM characters LEFT JOIN accounts ON characters.OldAccountId = accounts.Id WHERE accounts.Id = {0} AND characters.AccountId = -1;";
        public const String CREATE_CHARACTER = "INSERT INTO characters (AccountId, Name, Slot, Level, Job, Exp, Gold, Gender, MapId, PosX, PosY, PosZ, Strength, Stamina, Dexterity, Inteligence, Spirit, Hp, Mp, HeadMesh, HairColor, HairMesh, DeletedDate) VALUES ({0}, '{1}', {2}, 1, {3}, 0, 0, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, '{19}');";
        public const String DELETE_CHARACTER = "UPDATE characters SET AccountId = -1, OldAccountId = {0}, DeletedDate = '{2}' WHERE Id = {1};";
        public const String RESUME_CHARACTER = "UPDATE characters SET AccountId = {0}, OldAccountId = -1, DeletedDate = '{2}', Slot = {3} WHERE Id = {1};";
        public const String UPDATE_CHARACTER = "UPDATE characters SET Name='{0}', Slot={1}, Level={2}, Job={3}, Exp={4}, Gold={5}, Gender={6}, MapId={7}, PosX={8}, PosY={9}, PosZ={10}, Strength={11}, Stamina={12}, Dexterity={13}, Inteligence={14}, Spirit={15}, Hp={16}, Mp={17}, HairMesh={18}, HairColor={19}, HeadMesh={20} WHERE Id={21}";
        public const String GET_CHARACTER_ID = "SELECT Id FROM characters WHERE Name = '{0}' AND AccountId = {1} AND Slot = {2}";
        public const String GET_CHARACTER_BY_ID = "SELECT * FROM characters WHERE Id = {0}";
        public const String UPDATE_LASTIP = "UPDATE accounts Set Lastip={0} WHERE username='{1}';";

        // Game
        public const String GET_SHORTCUT = "SELECT * FROM shortcut WHERE dwCharacterID='{0}';";
        public const String GET_HOTKEY = "SELECT * FROM hotkey WHERE dwCharacterID='{0}';";
        public const String UPDATE_HOTKEY = "UPDATE hotkey SET szKeys = '{0}', szValues='{1}' WHERE dwCharacterID={2};";
    }
}
