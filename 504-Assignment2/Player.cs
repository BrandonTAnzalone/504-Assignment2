//***************************************************************************
//
//  Troy DeClerck       - Z1877438
//  Brandon Anzalone    - Z1884778
//  CSCI 473/504        Assignment 2
//
//  We certify that this is our own work and where appropriate an extension
//  of the starter code provided for the assignment
//***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    // Each unique race
    public enum Race { Orc, Troll, Tauren, Forsaken };

    // Each unique class
    public enum Class { Warrior, Mage, Druid, Priest, Warlock, Rogue, Paladin, Hunter, Shaman };


    public class Player : IComparable
    {
        // Private attributes
        private uint id;
        private string name;
        private Race playerRace;
        private uint level;
        private uint exp;
        private uint guildID;
        private Class playerClass;
        private static uint MAX_LEVEL = 60;



        // Read Only Access
        public uint Id
        {
            get { return id; }
            private set { id = value; }
        }

        // Read Only Access
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        // Read Only Access
        // Must be between 0 and 3
        public Race PlayerRace
        {
            get { return playerRace; }
            private set
            {
                if (value >= (Race)0 || value <= (Race)3)
                    playerRace = value;
                else
                {
                    Console.WriteLine(value + " is not a valid race id");
                }


            }
        }

        // Read Only Access
        // Must be between 0 and 8
        public Class PlayerClass
        {
            get { return playerClass; }
            private set
            {
                if (value >= (Class)0 || value <= (Class)8)
                    playerClass = value;
                else
                {
                    Console.WriteLine(value + " is not a valid class id");
                }
            }
        }

        // Read/Write Access
        public uint Level
        {
            get { return level; }
            set
            {
                if (value >= 0 || value <= MAX_LEVEL)
                    level = value;
                else
                {
                    Console.WriteLine(value + " is not a valid level");
                }
            }
        }

        // Read Access, Write Access Increases Current Level
        public uint Exp
        {
            get { return exp; }
            set
            {
                exp = exp + value;
            }
        }

        // Read/Write Access
        public uint GuildID
        {
            get { return guildID; }
            set { guildID = value; }
        }


        // Default Constructor
        public Player()
        {
            Id = 0;
            Name = "";
            PlayerRace = 0;
            Level = 1;
            GuildID = 0;
        }

        // Constructor
        public Player(uint newID, string newName, Race newPlayerRace, Class newPlayerClass, uint newLevel, uint newExp, uint newGuildID)
        {
            Id = newID;
            Name = newName;
            PlayerRace = newPlayerRace;
            Level = newLevel;
            GuildID = newGuildID;
            PlayerClass = newPlayerClass;
            Exp = newExp;

        }


        // ToString Method
        public override string ToString()
        {
            // Displays the player name, race, level, and guild
            string raceString = "" + (Race)this.PlayerRace;
            string levelString = "" + this.Level;
            // If the player is in a guild it will say which guild, if not then nothing
            string guildString = "";
            if (Form1.GuildDictionary.ContainsKey(this.guildID))
            {
                guildString = "Guild: " + Form1.GuildDictionary[this.guildID];
            }

            string output = String.Format("Name: {0}Race: {1} Level: {2}{3}", this.Name.PadRight(20), raceString.PadRight(10), levelString.PadRight(10), guildString);

            return output;
        } 
        

        // IComparable Method
        public int CompareTo(object alpha)
        {
            //Check for null values
            if (alpha == null) return 1;

            //typecast
            Player rightOp = alpha as Player;

            //Players are compared by name
            if (rightOp != null)
                return Name.CompareTo(rightOp.Name);
            else
                throw new ArgumentException("Item CompareTo argument is not an item");
        }

    }
}
