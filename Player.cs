using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace gam
{
    public enum Item
    { 
        Gold,
        Food,
        Key,
        Map,
        Num
    }
    public enum Players
    { 
        PlayerChar,
        MapBuilder
    }
    public enum Equipment
    {
        Sword,
        Shield,
        Axe,
        Armour,
        Potion,
        Num
    }
    public enum PlayerFirstName
    {
        Haxaw,
        Zathic,
        Jathil,
        Etha,
        Kirdyl,
        Mumnomnok,
        Umdela,
        Tacele,
        Larolnir,
        Jialodri,
        Wornil,
        Ilmyn,
        Runu,
        Mishe,
        Ollin,
        Elilne,
        Suldalny,
        Iledra,
        Yrmenner,
        Omrulgri,
        Lidru,
        Inarlik,
        Bythasu,
        Tograsab,
        Ollal,
        Zolgro,
        Wosho,
        Chathar,
        Phannu,
        Yre,
        Blanned,
        Num

    }
    public enum PlayerSecondName
    {
        Regalspear,
        Hammerflame,
        Mirthmaul,
        Havenstriker,
        Fullvalor,
        Slategem,
        Lionbasher,
        Marbleriver,
        Duskbrew,
        Fullmaul,
        Deadsnout,
        Barleymantle,
        Horsewoods,
        Beartrapper,
        Blueblower,
        Steelbash,
        Horsesnow,
        Ragedoom,
        Summerdown,
        Miststrength,
        Mossbrew,
        Blazebrew,
        Mourningreaper,
        Blueshaper,
        Deadbluff,
        Dusksnout,
        Leafbranch,
        Regalspire,
        Mastertalon,
        Brightsnarl,
        ToporFeed,
        TreeFriend,
        Num
    }

    class Player
    {
        Random rnd = new Random();
        List<Item> inventory = new List<Item>() {};
        List<Equipment> equipmentList = new List<Equipment>();
        public Dictionary<PlayerClass, string> classDescriptions = new Dictionary<PlayerClass, string>();
        public int startFood = 15;//starting food allowance
        public int startGold = 5;
        public int maxHunger = 10;//max hunger a player can have
        public int hunger = 10;
        public int health = 15;
        public int damage = 2;
        public int shield = 0;
        public int playerXLocation = 0;
        public int playerYLocation = 0;
        public PlayerClass playerClass = PlayerClass.Warrior;
        public string playerFullName = "";
        public bool playerAlive = true;
        public int merchantRep = 0;
        public bool escaped = false;

        //these are effects given by events/character choices
        public bool playerHasLifeSteal = false;
        public bool playerNeedsFood = true;
        public bool playerIsVampire = false;
        public bool playerIsLich = false;
        public bool playerCanRessurect = false;
        public bool playerSightWithoutMap = false;

        public PlayerClass PlayerChooseClass()
        {
            //give the player 3 choices of class
            GenerateClassStrings();
            PlayerClass choice1 =(PlayerClass)rnd.Next(0, (int)PlayerClass.Num);
            PlayerClass choice2 = (PlayerClass)rnd.Next(0, (int)PlayerClass.Num);
            PlayerClass choice3 = (PlayerClass)rnd.Next(0, (int)PlayerClass.Num);
            while (choice1 == choice2)
            {
                choice2 = (PlayerClass)rnd.Next(0, (int)PlayerClass.Num);

            }
            while (choice3 == choice2 || choice3 == choice1)
            {
                choice3 = (PlayerClass)rnd.Next(0, (int)PlayerClass.Num);
            }
            Console.WriteLine("Choose your Class!");
            while (true)
            {               
                Console.WriteLine(choice1 +" - "+ classDescriptions[choice1]);
                Console.WriteLine(choice2 +" - "+ classDescriptions[choice2]);
                Console.WriteLine(choice3 + " - " + classDescriptions[choice3]);
                Console.WriteLine("---");
                var input = Console.ReadLine().ToLower();
                if (input == choice1.ToString().ToLower())
                {
                    playerClass = choice1;
                    break;
                }
                else if (input == choice2.ToString().ToLower())
                {
                    playerClass = choice2;
                    break;
                }
                else if (input == choice3.ToString().ToLower())
                {
                    playerClass = choice3;
                    break;
                }
                else
                {
                    Console.WriteLine("Please Select a Class");
                }
            }
            ChangeCharacterStatsFromClass(playerClass);
            return playerClass;
        }

        public void GenerateClassStrings()
        {
            classDescriptions.Add(PlayerClass.Paladin, "A Noble Fighter with Aptitude for Shields"); //gets bonus from shield, more shield
            classDescriptions.Add(PlayerClass.Alchemist, "A scientist who can enchance potions in battle"); //gets makes potions increased
            classDescriptions.Add(PlayerClass.Warrior, "A Fierce, sword weilding Combatant"); //gets gets bonus from sword,more damage
            classDescriptions.Add(PlayerClass.Beserker, "An Uncontrollable Whirlwind, experienced with Axes"); //gets gets bonus from Axe, more damage
            classDescriptions.Add(PlayerClass.Knight, "A Honourbale Duelist, specialisng in Armour"); //gets gets bonus from Armour, more shield
            classDescriptions.Add(PlayerClass.Thief, "A crafty rogue who can Pick Locks with ease"); //gets can open chests without needing keys
            classDescriptions.Add(PlayerClass.Trapmaster, "A technical genius, able to Disarm Traps with ease"); // has higher detrap chance, gets treasure from detraps
            classDescriptions.Add(PlayerClass.Hustler, "An adventurer who always knows how to get a Good Deal"); //gets decreased merchant prices
            classDescriptions.Add(PlayerClass.Monk, "An immensely powerful Ascetic traveller"); //gets deals more damage and has shield by default, can't pick up equipment
            classDescriptions.Add(PlayerClass.Chef, "A Cook who knows how to get the most out of food"); //gets gets bonus health from food recovery
            classDescriptions.Add(PlayerClass.Shaman, "A spiritually attuned being, able to Return from the Brink"); //gets ressurects once on death
            classDescriptions.Add(PlayerClass.Assassin, "A murderous cutthroat, desicive at executing enemies"); //gets has a higher chance to crit and to be crit
            classDescriptions.Add(PlayerClass.Vampire, "An ancient being of power, who no longer needs mortal sustenance");//gets lifesteal, but cannot use shields/armour --this is a new mechanic
            classDescriptions.Add(PlayerClass.Brawler, "A warrior with natural defences, but cannot hit as hard");//starts with 1 shield but 1 less damage
            classDescriptions.Add(PlayerClass.Pirate, "A swashbuckling combatant with a hoard of booty");//start with random more gold
            classDescriptions.Add(PlayerClass.Survivalist, "A traveller adept at surviving harsh conditions"); //chance to not consume food on room move
            classDescriptions.Add(PlayerClass.Warlock, "A shadowy figure who enfeebles enemies");//Enfeebles enemies
        }

        public void PlayerGainsLifeFromLifeSteal(int damageDealt)
        {
            if (rnd.Next(0, 3) != 0)
            {
                Console.WriteLine("You drain " + damageDealt.ToString() + " life from your enemy!");
                health += damageDealt;
            }
        }

        public void ChangeCharacterStatsFromClass(PlayerClass playerClass)
        {
            switch (playerClass)
            {
                case PlayerClass.Monk:
                    damage += 3;
                    shield += 1;
                    break;
                case PlayerClass.Brawler:
                    damage -= 1;
                    shield += 1;
                    break;
                case PlayerClass.Pirate:
                    AddToInventory(Item.Gold, rnd.Next(2,6));
                    break;
                case PlayerClass.Vampire:
                    health += 8;
                    playerHasLifeSteal = true;
                    playerIsVampire = true;
                    playerNeedsFood = false;
                    break;
                case PlayerClass.Shaman:
                    playerCanRessurect = true;
                    break;
            }
        }
    
        public string GeneratePlayerName()
        {
            int firstNamenumber = rnd.Next(0, (int)PlayerFirstName.Num);
            int secondNameNumber = rnd.Next(0, (int)PlayerSecondName.Num);
            PlayerFirstName firstNameString = (PlayerFirstName)firstNamenumber;
            PlayerSecondName secondNameString = (PlayerSecondName)secondNameNumber;
            playerFullName = firstNameString.ToString() + " " + secondNameString.ToString();
            return playerFullName;
        }

        public void AddToInventory(Item item, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                inventory.Add(item);
            }
        }
       
        public void RemoveFromInventoryByItem(Item item)
        {
            inventory.Remove(item);
        }

        public void RemoveFromInventoryByPosition(int position)
        {
            inventory.Remove(inventory[position]);
        }

        public void RemoveRandomFromInventory()
        {
            if (inventory.Count > 0)
            inventory.Remove(inventory[rnd.Next(0, inventory.Count)]);
        }

        public void AddToEquipment(Equipment equipment, Player playerChar)
        {
            Console.WriteLine("You find a " + equipment.ToString() + "!");
            if (playerChar.playerClass == PlayerClass.Monk)
            {
                Console.WriteLine("Unfortuantely, your Monk training doesn't allow the use of equipment! You sadly replace the item");
                return;
            }
            else if (playerChar.playerClass == PlayerClass.Vampire && (equipment == Equipment.Armour || equipment == Equipment.Shield))
            {
                Console.WriteLine("You cannot use Armour or shields as a Vampire, you return the item");
                return;
            }

            equipmentList.Add(equipment);

            switch (equipment)
            {
                case Equipment.Sword:

                    if (playerChar.playerClass == PlayerClass.Warrior)
                    {
                        Console.WriteLine("As a Warrior, you specialise with Swords, Damage increased!");
                        damage += 2;
                    }
                    else
                    {
                        Console.WriteLine("Damage Increased");
                        damage += 1;
                    }
                    break;

                case Equipment.Shield:
                    if (playerChar.playerClass == PlayerClass.Paladin)
                    {
                        Console.WriteLine("As a Paladin, you specialise with shields, Defense increased!");
                        shield += 2;
                    }
                    else
                    {
                        Console.WriteLine("Defense Increased");
                        shield += 1;
                    }
                    break;

                case Equipment.Axe:
                    if (playerChar.playerClass == PlayerClass.Beserker)
                    {
                        Console.WriteLine("As a Beserker, you specialise with Axes, Damage increased!");
                        damage += 2;
                    }
                    else
                    {
                        Console.WriteLine("Damage Increased");
                        damage += 1;
                    }
                    break;

                case Equipment.Armour:
                    if (playerChar.playerClass == PlayerClass.Knight)
                    {
                        Console.WriteLine("As a Knight, you specialise with Armour, Defense increased!");
                        shield += 2;
                    }
                    else
                    {
                        Console.WriteLine("Defense Increased");
                        shield += 1;
                    }
                    break;

                case Equipment.Potion:
                    if (playerChar.playerClass == PlayerClass.Alchemist)
                    {
                        Console.WriteLine("Using your alchemical knowledge, you improve the potion to make you stronger!");
                        health *= 2;
                        damage += 1;
                        shield += 1;
                    }
                    else
                    {
                        Console.WriteLine("Health Increased");
                        health *= 2;
                    }
                    break;
                case Equipment.Num:
                    //this is a fallback for if you roll the num value, will just default to sword
                    Console.WriteLine("Damage Increased");
                    damage += 1;
                    break;
            }
        }

        public void AddRandomEquipment(Player playerChar)
        {            
                //truly random item requested
                Random rnd = new Random();
                int randomEquipment = rnd.Next(0, (int)Equipment.Num);
            Equipment equipment = (Equipment)randomEquipment;
            AddToEquipment(equipment, playerChar);
        }

        public int FindItemInInventory(Item item)
        {
            int itemcount = 0;
            foreach (var bagitem in inventory)
            {
                if (item == bagitem)
                {
                    itemcount += 1;
                }
            }
            return itemcount;
        }

        public void DisplayInventory()            
        {
            Console.WriteLine(playerFullName + " the " + playerClass.ToString()+"'s inventory");
            Console.WriteLine("---");
           var orderedInventory = inventory.GroupBy(i => i.ToString());
            if (inventory.Count == 0)
            {
                Console.WriteLine("You have nothing in your inventory");
            }
            foreach (var type in orderedInventory)
            {
                Console.WriteLine("{0} {1}", type.Key, type.Count());
            }
        }

        public int GetCountInventory()
        {
            int itemCount = 0;
            foreach (Item item in inventory)
            {
                itemCount += 1;
            }
            return itemCount;
        }

        public int GetCountEquipment()
        {
            int EquipmentCount = 0;
            foreach (Equipment equipment in equipmentList)
            {
                EquipmentCount += 1;
            }
            return EquipmentCount;
        }

        public void MovePlayer(Direction? direction)
        {
            if (direction == Direction.North)
            {
                playerYLocation += 1;
            }
            else if (direction == Direction.South)
            {
                playerYLocation -= 1;
            }
            else if (direction == Direction.East)
            {
                playerXLocation += 1;
            }            
            else if (direction == Direction.West)
            {
                playerXLocation -= 1;
            }
        }

        public Location Location() //location
        {
            return new Location(playerXLocation,playerYLocation);
        }     

    }
}
