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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Specialized;

namespace Assignment2
{
    public partial class Form1 : Form
    {
        // File Paths
        public static string GuildFile = @"..\..\guilds.txt";
        public static string PlayerFile = @"..\..\players.txt";

        // Search string declarations
        private string playerSearch;
        private string guildSearch;

        // Dictionary declarations
        public static Dictionary<uint, Player> PlayerDictionary = new Dictionary<uint, Player>();
        public static Dictionary<uint, string> GuildDictionary = new Dictionary<uint, string>();

        // Add player/guild declarations
        private UInt32 maxPlayerId = 0;
        private UInt32 maxguildId = 1;

        public Form1()
        {
            InitializeComponent();
        }

        /**
         * Reads the Player file and stores them into a dictionary.
         * 
         * Converts each input separated by tabs and adds them to 
         * a previously defined player dictionary.
         * 
         * @param input - The name of the input file to be read.
         ****************************************************************************/
        private void ReadPlayers(string input)
        {
            // String to read lines into
            string inputLine;

            // Try block to catch filenotfound
            try
            {
                using (StreamReader inFile = new StreamReader(input))
                {
                    inputLine = inFile.ReadLine();

                    while (inputLine != null)
                    {
                        // Separate with tab
                        string[] inPlayers = inputLine.Split('\t');
                        // If the correct number of attributes are on the line, create a player from line
                        if (inPlayers.Length == 7)
                        {
                            AddPlayer(Convert.ToUInt32(inPlayers[0]), inPlayers[1], (Race)Convert.ToUInt32(inPlayers[2]), (Class)Convert.ToUInt32(inPlayers[3]),
                                        Convert.ToUInt32(inPlayers[4]), Convert.ToUInt32(inPlayers[5]), Convert.ToUInt32(inPlayers[6]));

                        }

                        // Read next line
                        inputLine = inFile.ReadLine();
                    }
                }
                updateList();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(input + "file does not exist");
                Environment.Exit(1);
            }
        }

        // Refactored AddPlayer function
        private void AddPlayer(uint newID, string newName, Race newPlayerRace, Class newPlayerClass, uint newLevel, uint newExp, uint newGuildID)
        {
            //Create the player from the array, convert string to uints and other attributes when necessary
            Player inputPlayer = new Player( newID,  newName,  newPlayerRace,  newPlayerClass,  newLevel,  newExp,  newGuildID);

            // Add to dictionary of players
            PlayerDictionary.Add(inputPlayer.Id, inputPlayer);
        }

        /**
         * Reads the Guild file and stores them into a dictionary.
         * 
         * Converts each input separated by tabs and adds them to 
         * a previously defined guild dictionary.
         * 
         * @param input - The name of the input file to be read.
         ****************************************************************************/
        private void ReadGuilds(string input)
        {
            // String to read lines into
            string inputLine;

            // Try block to catch filenotfound
            try
            {
                using (StreamReader inFile = new StreamReader(input))
                {
                    inputLine = inFile.ReadLine();

                    while (inputLine != null)
                    {
                        // Separate with tab
                        string[] inGuilds = inputLine.Split('\t');
                        // If the correct number of attributes are on the line, create a guild from line
                        if (inGuilds.Length == 2)
                            AddGuild(Convert.ToUInt32(inGuilds[0]), inGuilds[1]);

                        // Read next line
                        inputLine = inFile.ReadLine();
                    }
                }
                updateGuildList();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(input + "file does not exist");
                Environment.Exit(1);
            }
        }

        // Refactored Guild Add
        private void AddGuild(uint guildId, string guildName)
        {

            // Add to dictionary of items
            GuildDictionary.Add(guildId, guildName);
            string[] guildSplit = guildName.Split('-');
            listBox2.Items.Add(String.Format("{0,-25}{1,-12}", guildSplit[0], String.Format("[{0}]", guildSplit[1])));
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            ReadGuilds(GuildFile);
            ReadPlayers(PlayerFile);
            PopDropDowns();
        }

        /**
         * Applies the necessary search from user input in the player or guild text boxes
         * 
         * Checks both text fields for user input and calls the appropriate updatelist
         * function to adjust the Player/Guild list respectively
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void applySearch(object sender, System.EventArgs e)
        {
            outputBox.Clear();

            // Player Search
            if (searchPlayerField.Text != null && searchPlayerField.Text != "")
            {
                string inputPlayer = searchPlayerField.Text;

                // Regex is to check if the string contains only letters
                if ((Regex.IsMatch(inputPlayer, @"^[a-zA-Z]+$")))
                {
                    outputBox.Text += "Filter Successfully Applied!\n";

                    //update string & update the textBox
                    playerSearch = inputPlayer;
                    updateList();

                }
            }
            else // Nothing is entered in
            {
                playerSearch = null;
                updateList();
                updateGuildList();
            }

            if (searchGuildField.Text != null && searchGuildField.Text != "")
            {
                string inputGuild = searchGuildField.Text;

                // Regex is to check if the string contains only letters
                if ((Regex.IsMatch(inputGuild, @"^[a-zA-Z]+$")))
                {
                    outputBox.Text += "Filter Successfully Applied!\n";

                    //update string & update the textBox
                    guildSearch = inputGuild;
                    updateGuildList();

                }
            }
            else // Nothing is entered in
            {
                guildSearch = null;
                updateList();
                updateGuildList();
            }

        }

        /**
         * Creates a sorted list of players and prints the result
         * 
         * Clears the player list box and updates the list depending on the user's 
         * search option.
         ****************************************************************************/
        private void updateList()
        {
            // Clear current list
            listBox1.Items.Clear();

            // Declare a sorted list for players
            SortedSet<Player> playerSet = new SortedSet<Player>();

            // Counter for the number of matches in the player dictionary
            int count = 0;

            // Fill the player list
            foreach (Player p in PlayerDictionary.Values)
            {
                playerSet.Add(p);
            }

            if (playerSearch == null) // If the apply search is empty, print the entire player list
            {

                foreach (Player p in playerSet)
                {
                    listBox1.Items.Add(String.Format("{0,-14} {1,-10} {2,-3}", p.Name, p.PlayerClass, p.Level));
                }
            }
            else // Print any matching players to the apply search option
            {
                foreach (Player p in playerSet)
                {
                    if (p.Name.StartsWith(playerSearch))
                    {
                        listBox1.Items.Add(String.Format("{0,-14} {1,-10} {2,-3}", p.Name, p.PlayerClass, p.Level));
                        count++;
                    }
                }

                // Print default list if no matches
                if (count == 0)
                {
                    foreach (Player p in playerSet)
                    {
                        listBox1.Items.Add(String.Format("{0,-14} {1,-10} {2,-3}", p.Name, p.PlayerClass, p.Level));
                    }
                }
            }
        }

        /**
         * Creates a sorted list of guilds and prints the result
         * 
         * Clears the guild list box and updates the list depending on the user's 
         * search option.
         ****************************************************************************/
        private void updateGuildList()
        {
            // Clear current list
            listBox2.Items.Clear();

            // Declare a sorted list for the guilds
            SortedSet<string> guildSet = new SortedSet<string>();

            // Counter for the number of matches in the guild dictionary
            int count = 0;

            // Fill the Guild list
            foreach (string s in GuildDictionary.Values)
            {
                guildSet.Add(s);
            }

            if (guildSearch == null) // If the apply search is empty, print the entire guild list
            {

                foreach (string s in guildSet)
                {
                    
                    string[] guildSplit = s.Split('-');
                    listBox2.Items.Add(String.Format("{0,-25}{1,-12}", guildSplit[0], String.Format("[{0}]", guildSplit[1])));
                }
            }
            else // Print any matching players to the apply search option
            {
                foreach (string s in guildSet)
                {
                    string[] guildSplit = s.Split('-');
                    if (guildSearch == guildSplit[1]) 
                    {
                        listBox2.Items.Add(String.Format("{0,-25}{1,-12}", guildSplit[0], String.Format("[{0}]", guildSplit[1])));
                        count++;
                    }
                }

                // Print default list if no matches
                if (count == 0)
                {
                    foreach (string s in guildSet)
                    {
                        outputBox.Clear();
                        outputBox.Text += "No Results!";
                        string[] guildSplit = s.Split('-');
                        listBox2.Items.Add(String.Format("{0,-25}{1,-12}", guildSplit[0], String.Format("[{0}]", guildSplit[1])));
                    }
                }
            }
        }

        /**
         * Prints the amount of players in a specific guild
         * 
         * Manipulates the selected guild string to find a matching guild. Then loops
         * through the player dictionary to find any players in the guild
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void printGuildRoster(object sender, System.EventArgs e)
        {
            string selectedGuild = null;
            outputBox.Clear();

            int count = 0;

            // set string only if a guild was selected
            if (listBox2.SelectedIndex != -1)
            {
                // Format string to match guild in file
                selectedGuild = listBox2.SelectedItem.ToString();
                char[] x = { '[', ']' };
                string searchGuild = selectedGuild.Replace(" ", "");
                searchGuild = searchGuild.Replace("]", "");
                searchGuild = searchGuild.Replace("[", "-");

                outputBox.Text += "Guild Listing for " + selectedGuild;
                outputBox.Text += "\n-------------------------------------------------------";

                // Loop to find all matching players in a guild and print the result
                foreach (KeyValuePair<uint, string> pair in GuildDictionary)
                {
                    string matchGuild = pair.Value.Replace(" ", "");

                    if (matchGuild.Equals(searchGuild))
                    {
                        foreach (KeyValuePair<uint, Player> playerPair in PlayerDictionary)
                        {
                            if (playerPair.Value.GuildID.Equals(pair.Key))
                            {
                                outputBox.Text += "\n" + playerPair.Value;
                                count++;
                            }
                        }
                    }
                }

                // Check if guild is empty
                if(count == 0)
                {
                    outputBox.Text += "\nNo Guild Members";
                }

            }
            else
            {
                outputBox.Text += "No Guild selected";
                return;
            }

        }

        /**
         * Disband the chosen guild from the guild list
         * 
         * Manipulates the selected guild string to find a matching guild. Then loops
         * through the player dictionary to find any players in the guild and set their
         * GuildID to 0.
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void disbandGuild(object sender, System.EventArgs e)
        {
            string selectedGuild = null;
            uint id = 0;
            outputBox.Clear();

            if (listBox2.SelectedIndex != -1)
            {
                selectedGuild = listBox2.SelectedItem.ToString();
                outputBox.Text += "Guild successfully disbanded";
                // Remove selected Guild
                listBox2.Items.Remove(listBox2.SelectedItem);

                // Format string to match guild in file
                string disbandedGuild = selectedGuild.Replace(" ", "");
                disbandedGuild = disbandedGuild.Replace("]", "");
                disbandedGuild = disbandedGuild.Replace("[", "-");

                // Loop to find all matching players in a guild and set their new ID to 0
                foreach (KeyValuePair<uint, string> pair in GuildDictionary)
                {
                    // Adjust guild name to match the current string
                    string matchGuild = pair.Value.Replace(" ", "");

                    if (matchGuild.Equals(disbandedGuild))
                    {
                        foreach (KeyValuePair<uint, Player> playerPair in PlayerDictionary)
                        {
                            if (playerPair.Value.GuildID.Equals(pair.Key))
                            {
                                playerPair.Value.GuildID = 0;
                                
                            }
                        }
                        id = pair.Key;
                    }
                }

                GuildDictionary.Remove(id);
                

            }
        }

        /**
         * A chosen player joins a chosen guild
         * 
         * Manipulates the selected guild string to find a matching guild. Then loops
         * through the player dictionary to find a matching player. Update the player's
         * GuildID to match the selected Guild ID
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void joinGuild(object sender, System.EventArgs e)
        {
            string selectedGuild = null;
            string selectedPlayer = null;

            outputBox.Clear();

            if (listBox1.SelectedIndex == -1)
            {
                outputBox.Text += "A player was not selected!";
            }
            else if (listBox2.SelectedIndex == -1)
            {
                outputBox.Text += "A guild was not selected!";
            }
            else
            {
                selectedGuild = listBox2.SelectedItem.ToString();
                selectedPlayer = listBox1.SelectedItem.ToString();

                // Format string to match guild in file
                string chosenGuild = selectedGuild.Replace(" ", "");
                chosenGuild = chosenGuild.Replace("]", "");
                chosenGuild = chosenGuild.Replace("[", "-");

                // Format string to match player in file
                var chosenPlayer = selectedPlayer.Split(' ')[0];

                // Loop through the player dictionary
                foreach (KeyValuePair<uint, Player> pair in PlayerDictionary)
                {
                    // Check for a player match
                    if (pair.Value.Name == chosenPlayer)
                    {
                        // Begin looping through the guild dictionary
                        foreach (KeyValuePair<uint, string> guildPair in GuildDictionary)
                        {
                            // Adjust guild name to match the current string
                            string matchGuild = guildPair.Value.Replace(" ", "");

                            if (matchGuild.Equals(chosenGuild))
                            {
                                // Put player in guild if not already in
                                if(pair.Value.GuildID != guildPair.Key)
                                {
                                    pair.Value.GuildID = guildPair.Key;
                                    outputBox.Text += chosenPlayer + " has joined " + chosenGuild + " successfully!";
                                }
                                else 
                                    outputBox.Text += chosenPlayer + " is already in " + chosenGuild + "!";
                            }

                        }
                    }

                }
            }
        }

        /**
         * A chosen player leaves a chosen guild
         * 
         * Manipulates the selected guild string to find a matching guild. Then loops
         * through the player dictionary to find a matching player. Update the player's
         * GuildID to equal 0
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void leaveGuild(object sender, System.EventArgs e)
        {
            string selectedGuild = null;
            string selectedPlayer = null;

            outputBox.Clear();

            if (listBox1.SelectedIndex == -1)
            {
                outputBox.Text += "A player was not selected!";
            }
            else if (listBox2.SelectedIndex == -1)
            {
                outputBox.Text += "A guild was not selected!";
            }
            else
            {
                selectedGuild = listBox2.SelectedItem.ToString();
                selectedPlayer = listBox1.SelectedItem.ToString();

                // Format string to match guild in file
                string chosenGuild = selectedGuild.Replace(" ", "");
                chosenGuild = chosenGuild.Replace("]", "");
                chosenGuild = chosenGuild.Replace("[", "-");

                // Format string to match player in file
                var chosenPlayer = selectedPlayer.Split(' ')[0];

                // Loop through the player dictionary
                foreach (KeyValuePair<uint, Player> pair in PlayerDictionary)
                {
                    // Check for a player match
                    if (pair.Value.Name == chosenPlayer)
                    {
                        // Begin looping through the guild dictionary
                        foreach (KeyValuePair<uint, string> guildPair in GuildDictionary)
                        {
                            // Adjust guild name to match the current string
                            string matchGuild = guildPair.Value.Replace(" ", "");

                            if (matchGuild.Equals(chosenGuild))
                            {
                                // Leave guild only if already in a guild
                                if (pair.Value.GuildID == guildPair.Key)
                                {
                                    pair.Value.GuildID = 0;
                                    outputBox.Text += chosenPlayer + " has left " + chosenGuild + " successfully!";
                                }
                                else
                                    outputBox.Text += chosenPlayer + " is not in " + chosenGuild + "!";
                            }

                        }
                    }

                }
            }
        }

         // Collection of drop down menus for the add player/guild forms
        private void PopDropDowns()
        {
            //Race DropDown Box
            RaceBox.DisplayMember = "Text";
            RaceBox.ValueMember = "ID";
            RaceBox.DataSource = new Item[]
            {
            new Item{ ID = 0, Text = "Orc" },
            new Item{ ID = 1, Text = "Troll" },
            new Item{ ID = 2, Text = "Tauren"},
            new Item{ ID = 3, Text = "Forsaken"}
            };

            RaceBox.SelectedIndex = -1;

            //Class DropDown Box
            ClassBox.DisplayMember = "Text";
            ClassBox.ValueMember = "ID";
            ClassBox.DataSource = new Item[]
            {
            new Item{ ID = 0, Text = "Warrior" },
            new Item{ ID = 1, Text = "Mage" },
            new Item{ ID = 2, Text = "Druid"},
            new Item{ ID = 3, Text = "Priest"},
            new Item{ ID = 4, Text = "Warlock" },
            new Item{ ID = 5, Text = "Rogue" },
            new Item{ ID = 6, Text = "Paladin" },
            new Item{ ID = 7, Text = "Hunter" },
            new Item{ ID = 8, Text = "Shaman" }
            };

            ClassBox.SelectedIndex = -1;

            //Guild DropDown Box
            ServerBox.DisplayMember = "Text";
            ServerBox.ValueMember = "ID";
            ServerBox.DataSource = new Item[]
            {
            new Item{ ID = 0, Text = "Beta4Azeroth" },
            new Item{ ID = 1, Text = "TKWasASetback" },
            new Item{ ID = 2, Text = "ZappyBoi"}
            };

            ServerBox.SelectedIndex = -1;

            //Type DropDown Box
            TypeBox.DisplayMember = "Text";
            TypeBox.ValueMember = "ID";
            TypeBox.DataSource = new Item[]
            {
            new Item{ ID = 0, Text = "Casual" },
            new Item{ ID = 1, Text = "Questing" },
            new Item{ ID = 2, Text = "Mythic+"},
            new Item{ ID = 3, Text = "Raiding"},
            new Item{ ID = 4, Text = "PVP"}
            };

            TypeBox.SelectedIndex = -1;
            RoleBox.SelectedIndex = -1;
        }

        /**
         * Method used to provide the appropriate roles for each class
         * 
         * Sends the string through a switch/case that fills the drop box with 
         * the correct information.
         * 
         * @param className The user's chosen class name for a new player
         ****************************************************************************/
        private void PopRoleBox(string className)
        {
            //Class DropDown box
            switch (className)
            {
                case "Warrior":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    new Item{ ID = 1, Text = "Tank"}
                    };
                    break;
                case "Mage":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    };
                    break;
                case "Druid":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    new Item{ ID = 1, Text = "Tank"},
                    new Item{ ID = 2, Text = "Healer"}
                    };
                    break;
                case "Priest":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    new Item{ ID = 2, Text = "Healer"}
                    };
                    break;
                case "Warlock":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    };
                    break;
                case "Rogue":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    };
                    break;
                case "Paladin":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    new Item{ ID = 1, Text = "Tank"},
                    new Item{ ID = 2, Text = "Healer"}
                    };
                    break;
                case "Hunter":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    };
                    break;
                case "Shaman":
                    RoleBox.DisplayMember = "Text";
                    RoleBox.ValueMember = "ID";
                    RoleBox.DataSource = new Item[]
                    {
                    new Item{ ID = 0, Text = "DPS"},
                    new Item{ ID = 2, Text = "Healer"}
                    };
                    break;
            };

        }

        /**
         * Method to confirm a player being added
         * 
         * Calls the AddPlayer method and updates the list with a new player being
         * added.
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void AddPlayerButton_Click(object sender, EventArgs e)
        {
            // Simple check to see if all forms are filled out properly
            if ((new Regex(@"^[A-Za-z0-9_-]*$")).IsMatch(PlayerNameBox.Text) && PlayerNameBox.Text != null && PlayerNameBox.Text != "" && RaceBox.SelectedIndex != -1 && ClassBox.SelectedIndex != -1)
            {
                outputBox.Clear();
                AddPlayer(maxPlayerId++, PlayerNameBox.Text, (Race)RaceBox.SelectedValue, (Class)ClassBox.SelectedValue, 0, 0, 0);
                updateList();
                outputBox.Text = string.Format("{0} Was Succesfully Created!", PlayerNameBox.Text);
            }
            else
            {
                outputBox.Clear();
                outputBox.Text = "Please fill out every box in the form!";
            }
           
        }

        /**
         * Method that grabs the className string from the user input
         * 
         * Grabs the className string from the user and sends it to the PopRoleBox
         * method for populating each drop down menu.
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void ClassBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ClassBox.SelectedIndex != -1)
            {
                string className = ((Item)ClassBox.SelectedItem).Text;
                PopRoleBox(className);
            }
        }

        /**
         * Method to confirm a guild being added
         * 
         * Calls the AddGuild method and updates the list with a new guild being
         * added.
         * 
         * @param sender The reference to the object that contains the event data 
         * 
         * @param e The parameter which contains the event data
         ****************************************************************************/
        private void AddGuildButton_Click(object sender, EventArgs e)
        {
            // Simple check to see if all forms are filled out properly
            if ((new Regex(@"^[A-Za-z0-9_-]*$")).IsMatch(GuildNameBox.Text) && GuildNameBox.Text != null && GuildNameBox.Text != "" && ServerBox.SelectedIndex != -1 && TypeBox.SelectedIndex != -1)
            { 
                outputBox.Clear();
                AddGuild(maxguildId++, GuildNameBox.Text.Trim() + "-" + ((Item)ServerBox.SelectedItem).Text.Trim());

                outputBox.Text = string.Format("{0}, Guild was Succesfully Created!", GuildNameBox.Text);
            }
            else
            {
                outputBox.Clear();
                outputBox.Text = "Please fill out every box in the form!";
            }
        }
    }
}

