using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteor.Source.Structures
{
    /// <summary>
    /// Utilisé pour la gestion des touches de raccourcis
    /// </summary>
    public struct HotKey
    {
        public Int32 Key1;
        public Int32 Value;
    }
    /// <summary>
    /// Structure utilisé pour les dialogue des monstres
    /// </summary>
    public struct MonsterSpeak
    {

        public int Id;

        public int IdleInterval;

        public String[] szIdle;

        public String[] szFight;

        public int FightInterval;

        public String[] Escape;

        public int EscapeInterval;

        public String[] szHome;

        public int HomeInterval;

        public String Rage;

        public int Probability;

        public String[] Dead;

    }

    /// <summary>
    /// Paramètres pour les NPC qui ont une apparence homme/Femme standart
    /// </summary>
    public struct Figure
    {
        public Byte hair_mesh;
        public Int32 mover_idx;
        public UInt32 hair_color;
        public Byte head_mesh;
    }

    public struct NPCData
    {
        public string Name;
        public String Dialog;
        public List<Int32[]> VendorSlot;
        public Int32[] Equipement;
        public Figure figure;        
    }

    public enum STATE_T  { STATE_NONE = 0, STATE_SIT = 1, STATE_MELEE_ATK = 2, STATE_SKILLING = 3, STATE_CASTING = 4, STATE_FSKILLING = 5, STATE_FCASTING = 6, STATE_STAND_G = 7, STATE_STAND_F = 8, STATE_STUN = 9, STATE_FALL = 10, STATE_JUMP = 11, STATE_MOVE_G = 12, STATE_MOVE_F = 13, STATE_MOVE_TO = 14, STATE_TRANSFORM = 15, STATE_AIRBOAT = 16, STATE_SNEAK = 17, STATE_FREEZE = 18, STATE_CHAOS = 19, STATE_PUSH = 20, STATE_MOTION = 21, STATE_DAMAGE = 22, STATE_FLY = 23, STATE_GROUND = 24, STATE_COMBAT = 25, STATE_DEAD = 26, STATE_COLLECT = 27, STATE_TRADE = 28, STATE_LINK = 29, STATE_NAVIGATE = 30, STATE_QUIZ = 31, STATE_MAX = 32, STATE_GUARD = 100 }
}
