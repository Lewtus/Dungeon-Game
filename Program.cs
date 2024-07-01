using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Drawing;

namespace gam
{

    class Program
    {
        static void Main(string[] args)
        {
            //TODO:
            //Replace with relative path of install location
            string hallOfHeroesFileRoot = AppDomain.CurrentDomain.BaseDirectory;
            string hallOfHeroesFileName = @"HallofHeroes";
            string hallOfHeroesFileLocation = hallOfHeroesFileRoot + hallOfHeroesFileName + ".txt";
            bool playGame = true;
            HallofHeroes(hallOfHeroesFileLocation);
            while (playGame)
            {
                Random rnd = new Random();
                int seed = rnd.Next();
                playGame = Init(hallOfHeroesFileLocation, seed);
                if (playGame)
                    GameLoop(hallOfHeroesFileLocation, seed, rnd);
            }

        }

        static void HallofHeroes(string hallOfHeroesFileLocation)
        {
            //creates a file if one doesn't already exist to store heroes who win the game in

            bool fileExists = File.Exists(hallOfHeroesFileLocation);
            if (fileExists == false)
            {
                File.Create(hallOfHeroesFileLocation);
            }
        }

        static void AddCharToHallofHeroes(string hallOfHeroesFileLocation, Player playerChar, string roomsSurvived)
        {
            //adds any hero who escapes to the file created in HallofHeroes()
            string hOHStorage = Path.GetDirectoryName(hallOfHeroesFileLocation);
            string heroDataConcat = playerChar.playerFullName.ToString() + " - " + playerChar.playerClass.ToString() + " - " + roomsSurvived;
            string heroDataToWrite = heroDataConcat + Environment.NewLine;
            File.AppendAllText(Path.Combine(hOHStorage, "HallofHeroes.txt"), heroDataToWrite);
        }

        static void StarterText()
        {
            Console.WriteLine("  ");
            Console.WriteLine("  ");
            Console.WriteLine("  ");
            Console.WriteLine("Welcome to Text Dungeon Crawler");
            Console.WriteLine("Play");
            Console.WriteLine("Hall of Heroes");
            Console.WriteLine("Controls and Instructions");
            Console.WriteLine("Quit");
            Console.WriteLine("---");
        }


        static bool Init(string hallOfHeroesFileLocation, int seed)
        {
            StarterText();
            while (true)
            {
                //TODO
                //add function to interpret any input and return true or false rather than big if statements
                var input = Console.ReadLine().ToLower();

                if (input.ToLower() == "play" || input.ToLower() == "p")
                {
                    Console.WriteLine("seed:" + seed);//used for debugging issues occurring in random generation
                    return true;
                }

                if (input.ToLower() == "playseed" || input.ToLower() == "ps")
                {
                    while (true)
                    {
                        Console.WriteLine("input seed:");
                        var playerSeedInput = Console.ReadLine().ToLower();
                        int playerSeed = Convert.ToInt32(playerSeedInput);
                        Console.WriteLine("seed = " + playerSeed);
                        return true;
                    }
                }
                else if (input.ToLower() == "hallofheroes" || input.ToLower() == "h" || input.ToLower() == "hall of heroes")
                {
                    OutputHallofHeroes(hallOfHeroesFileLocation, seed);
                }
                else if (input.ToLower() == "controls" || input.ToLower() == "c" || input.ToLower() == "controls and instructions" || input.ToLower() == "controlsandinstructions")
                {
                    OutputControlsAndInstructions();
                    StarterText();
                }
                else if (input.ToLower() == "quit" || input.ToLower() == "q")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid action");
                }
            }
        }


        static void OutputHallofHeroes(string hallOfHeroesFileLocation, int seed)
        {
            Console.WriteLine("Below are the brave heroes who have escaped the dungeon!:");
            Console.WriteLine("Name - Class - Rooms Cleared");
            Console.WriteLine("-------------------------------");
            var heroesFile = File.OpenRead(hallOfHeroesFileLocation);
            var linesRead = File.ReadLines(hallOfHeroesFileLocation);
            foreach (var heroData in linesRead)
            {
                
                Console.WriteLine(heroData);
            }
            heroesFile.Close();
            StarterText();
        }

        static void OutputControlsAndInstructions()
        {
            //this will display how to play the game
            Console.WriteLine("Instructions:");
            Console.WriteLine("The goal of this game is to navigate the randomly generated map and find the exit.");
            Console.WriteLine("Commands can be entered by either typing the full word shown, or by using the first character.");
            Console.WriteLine(" ");
            Console.WriteLine("The player has a few useful commands that can be used at any time:");
            Console.WriteLine("\"stats\" can be used to see the current characters stat details.");
            Console.WriteLine("\"map\" or \"m\" can be used to display the parts of the dungeon the player has navigated.");
            Console.WriteLine("\"inventory\" or \"i\" can be used to display the character's current items.");
            return;
        }


        private static void EncounterLogic(Player playerChar, Room newRoom, Random rnd, Hazard newHazard, Map map)
        {
            switch (newRoom.encounter)
            {
                case Encounter.Gold:
                    playerChar.AddToInventory(Item.Gold);
                    Console.WriteLine("you have found some " + newRoom.encounter);
                    newRoom.encounter = null;
                    break;
                case Encounter.Food:
                    playerChar.AddToInventory(Item.Food);
                    Console.WriteLine("you have found some " + newRoom.encounter);
                    newRoom.encounter = null;
                    break;
                case Encounter.Key:
                    Console.WriteLine("you have found a " + newRoom.encounter);
                    playerChar.AddToInventory(Item.Key);
                    newRoom.encounter = null;
                    break;
                case Encounter.Equipment:
                    Console.WriteLine("you have found some " + newRoom.encounter);
                    playerChar.AddRandomEquipment(playerChar);
                    newRoom.encounter = null;
                    break;
                case Encounter.Map:
                    playerChar.AddToInventory(Item.Map);
                    map.PlayerFindsMap();
                    Console.WriteLine("you have found a " + newRoom.encounter);
                    if (playerChar.FindItemInInventory(Item.Map) == 1)
                    {
                        Console.WriteLine("The map reveals the way out!");

                    }
                    else
                    {
                        Console.WriteLine("You already have a map, you take it anyway. Perhaps you can sell it!");
                    }
                    newRoom.encounter = null;
                    break;
                case Encounter.Chest:
                    Console.WriteLine("you have found a " + newRoom.encounter);
                    int keyNumber = playerChar.FindItemInInventory(Item.Key);
                    if (playerChar.playerClass == PlayerClass.Thief)
                    {
                        Console.WriteLine("You skillfully pick the lock on the chest, and grab the loot!");
                        int randomRewardAmount = rnd.Next(1, 5);
                        for (int i = 0; i < randomRewardAmount; i++)
                            playerChar.AddToInventory(newHazard.Reward());
                        newRoom.encounter = null;
                    }
                    else if (keyNumber > 0)
                    {
                        playerChar.RemoveFromInventoryByItem(Item.Key);
                        Console.WriteLine("You use a key to open the chest");
                        int randomRewardAmount = rnd.Next(1, 5);
                        for (int i = 0; i < randomRewardAmount; i++)
                            playerChar.AddToInventory(newHazard.Reward());
                        newRoom.encounter = null;
                    }
                    else
                    {
                        Console.WriteLine("You have no keys, so cannot open the chest!");
                    }
                    break;
                case Encounter.Monster:
                    Console.WriteLine("A " + newRoom.encounter + " in in the room, prepare for battle!");
                    Monster newMonster = newHazard.SummonMonster();
                    bool combatMonsterResult = newHazard.FightMonster(newMonster, playerChar);//return true is victory, return false is defeat
                    if (combatMonsterResult)
                    {
                        int randomRewardAmount = rnd.Next(1, 3);
                        for (int i = 0; i < randomRewardAmount; i++)
                            playerChar.AddToInventory(newHazard.Reward());
                        newRoom.encounter = null;
                    }
                    break;
                case Encounter.Boss:
                    Console.WriteLine("You have found a huge " + newRoom.encounter + " guarding a door!");
                    Boss newBoss = newHazard.SummonBoss();
                    bool combatBossResult = newHazard.FightBoss(newBoss, playerChar);
                    if (combatBossResult == false) 
                    {
                        playerChar.playerAlive = false;
                    
                    }
                    if (playerChar.health > 0)
                    {
                        int randomRewardAmount = rnd.Next(1, 6);
                        for (int i = 0; i < randomRewardAmount; i++)
                            playerChar.AddToInventory(newHazard.Reward());
                        newRoom.encounter = null;
                    }
                    break;
                case Encounter.Trap:
                    //Console.WriteLine("you have been " + newRoom.encounter + "ed");
                    Console.WriteLine("you have been trapped!");
                    if (newRoom.trapType == null)
                    {
                        Trap newTrap = newHazard.SummonTrap();
                        newRoom.trapType = newTrap;
                    }
                    newHazard.RunTrapBehaviour(newRoom.trapType, playerChar, newRoom, map);
                    break;
                case Encounter.Merchant:
                    Console.WriteLine("you have found a " + newRoom.encounter);
                    if (map.guildOfMerchants.TryGetValue(playerChar.Location(), out Merchant value))
                    {
                        value.RunMerchantBehaviour(playerChar);
                    }
                    else
                    {
                        Merchant newMerchant = new Merchant(playerChar, map);
                        map.AddMerchantToGuild(playerChar.Location(), newMerchant);
                        newMerchant.RunMerchantBehaviour(playerChar);
                    }
                    break;
                case Encounter.Shrine:
                    {
                        Console.WriteLine("you have found a " + newRoom.encounter);
                        Shrine newShrine = new Shrine((playerChar));
                        newShrine.RunShrineBehaviour(playerChar);
                        break;
                    }
                default:
                    break;
            }
        }

        static void GameLoop(string hallOfHeroesFileLocation, int seed, Random rnd)
        {
            var playerChar = new Player();
            PlayerClass playerClass = playerChar.PlayerChooseClass();

            string playerFullName = playerChar.GeneratePlayerName();
            string playerFullNameAndTitle = playerFullName + " the " + playerClass.ToString();
            for (int i = 1; i <= playerChar.startFood; i++)
            {
                playerChar.AddToInventory(Item.Food);
            }
            for (int i = 1; i <= playerChar.startGold; i++)
            {
                playerChar.AddToInventory(Item.Gold);
            }
            bool alive = true;
            bool escaped = playerChar.escaped;
            Map map = Map.GenerateDungeon();
            //Random rnd = new Random();
            Console.WriteLine("Welcome, adventurer, your name is " + playerFullNameAndTitle + "!");
            Console.WriteLine("As you enter the dungeon, the door seals behind you! You need to find a way out!");
            Console.WriteLine("");
           while (alive == true && escaped == false && playerChar.health > 0)//this is how many rooms you want the player to navigate
            {
                Room currentRoom = map.GetRoom(playerChar.Location());
                var doors = currentRoom.AvailableDoors();
                Hazard newHazard = new Hazard();
                if (!currentRoom.encounter.HasValue)
                {
                    Console.WriteLine("You find nothing");
                }
                else
                {
                    EncounterLogic(playerChar, currentRoom, rnd, newHazard, map);
                }
                if (playerChar.health <= 0)
                {
                    if (playerChar.playerCanRessurect)
                    {
                        playerChar.playerCanRessurect = false;
                        playerChar.health = 5;
                        map.TeleportPlayerToRandomLocation(playerChar);
                        Console.WriteLine("As you die, you feel yourself being whisked away. You wake up hours later in an unfamiliar location!");
                    }
                    else
                    {
                        break;
                    }
                }

                if (playerChar.playerAlive == false)
                {
                    break;
                }
                else if (playerChar.escaped == true)
                {
                    escaped = true;
                    break;
                }
                Console.WriteLine("---");
                foreach (var door in doors)
                {
                    Console.WriteLine(door);
                }
                Console.WriteLine("---");
                if (!currentRoom.IsEscapable())
                {
                    Console.WriteLine("The door seals behind you, you're trapped!");
                    alive = false;
                    break;

                }
                while (true)
                {
                    var input = Console.ReadLine().ToLower();

                    if (input.ToLower() == "inventory" || input.ToLower() == "inv")
                    {
                        playerChar.DisplayInventory();
                        continue;
                    }
                    else if (input.ToLower() == "stats" || input.ToLower() == "stat")
                    {
                        Console.WriteLine("health : " + playerChar.health.ToString());
                        Console.WriteLine("hunger : " + playerChar.hunger.ToString());
                        Console.WriteLine("damage : " + playerChar.damage.ToString());
                        Console.WriteLine("shield : " + playerChar.shield.ToString());
                        continue;
                    }
                    else if (input.ToLower() == "controls" || input.ToLower() == "c" || input.ToLower() == "controls and instructions" || input.ToLower() == "controlsandinstructions")
                    {
                        OutputControlsAndInstructions();
                    }
                    else if (input.ToLower() == "eat" || input.ToLower() == "ea")
                    {
                        if (playerChar.playerIsVampire)
                        {
                            Console.WriteLine("That won't help a Vampire!");
                        }
                        else if (playerChar.FindItemInInventory(Item.Food) > 0)
                        {
                            playerChar.RemoveFromInventoryByItem(Item.Food);
                            int healthGain = rnd.Next(1, 4);
                            if (playerChar.playerClass == PlayerClass.Chef)
                            {
                                Console.WriteLine("A chef knows how to cook up a good meal!");
                                healthGain += 1;
                            }
                            Console.WriteLine("You eat a food to gain " + healthGain.ToString() + " health");
                            playerChar.health += healthGain;
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("You have no food!");
                            continue;
                        }

                    }
                    else if (input.ToLower() == "map" || input.ToLower() == "m")
                    {
                        if (playerChar.FindItemInInventory(Item.Map) != 0 | playerChar.playerSightWithoutMap == true)
                        {
                            map.PlayerFindsMap();
                        }
                        map.PrintMap(playerChar.Location(), playerChar);
                    }

                    //DEBUG COMMANDS
                    else if (input.ToLower() == "portaltest") //remember to remove me
                    {
                        Trap newTrap = Trap.Portal;
                        Room newRoomTest = new Room();
                        newHazard.RunTrapBehaviour(newTrap, playerChar, newRoomTest, map);
                        break;
                    }
                    else if (input.ToLower() == "equipme") //remember to remove me
                    {
                        playerChar.AddRandomEquipment(playerChar);
                        break;
                    }
                    else if ((input.ToLower() == "fullgear"))
                    {
                        int x = 0;
                        while (x < 50)
                        {
                            playerChar.AddRandomEquipment(playerChar);
                            x++;
                        }
                        break;
                    }

                    else if (input.ToLower() == "debuglocation")
                    {
                        Console.WriteLine(playerChar.playerXLocation.ToString());
                        Console.WriteLine(playerChar.playerYLocation.ToString());
                    }
                    else if (input.ToLower() == "feedme")
                    {
                        playerChar.AddToInventory(Item.Food, 10);
                    }
                    else if (input.ToLower() == "mapme")
                    {
                        playerChar.AddToInventory(Item.Map, 1);
                        map.PlayerFindsMap();
                    }
                    else if (input.ToLower() == "merchantspoof")
                    {
                        Merchant newMerchant = new Merchant(playerChar, map);
                        newMerchant.RunMerchantBehaviour(playerChar);
                    }
                    else if (input.ToLower() == "teleporttest")
                    {
                        map.TeleportPlayerToRandomLocation(playerChar);
                    }
                    else if (input.ToLower() == "shrinetest")
                    {
                        Shrine newShrine = new Shrine(playerChar);
                        newShrine.RunShrineBehaviour(playerChar);

                    }
                    //END OF DEBUG COMMANDS

                    switch (input){
                        case ("n"):
                            input = "north";
                            break;
                        case ("e"):
                            input = "east";
                            break;
                        case ("s"):
                            input = "south";
                            break;
                        case ("w"):
                            input = "west";
                            break;
                        default:
                            break;
                    }

                    var direction = input.ToDirection();
                    if (!direction.HasValue)
                    {
                        Console.WriteLine("Enter a Direction");
                    }
                    else if (!doors.Contains(direction.Value))
                    {
                        Console.WriteLine("You can't go that way!");
                    }
                    else
                    {
                        if (playerChar.hunger > playerChar.maxHunger)
                        {
                            playerChar.hunger = playerChar.maxHunger;
                        }
                        playerChar.MovePlayer(direction);
                        if (currentRoom.exitRoom)
                        {
                            Boss newBoss = newHazard.SummonBoss();
                            bool combatBossResult = newHazard.FightBoss(newBoss, playerChar);
                            if (combatBossResult)
                            {
                                escaped = true;
                            }
                            else
                            {
                                escaped = false;
                            }
                            break;
                        }
                        else if (!playerChar.playerNeedsFood)
                        {
                            if (playerChar.playerIsVampire)
                            {
                                if (rnd.Next(0, 4) == 0)
                                {
                                    Console.WriteLine("You are slowly drained of life force! You lose a health");
                                    playerChar.health -= 1;
                                }
                            }
                        }
                        else if (playerChar.hunger > playerChar.maxHunger && playerChar.FindItemInInventory(Item.Food) != 0)
                        {
                            if (playerChar.playerClass == PlayerClass.Survivalist)
                            {
                                if (rnd.Next(0, 3) != 0)
                                {
                                    Console.WriteLine("As a Survivalist, your constitution keeps you full, no food consumed");
                                }
                            }
                            else
                            {
                                playerChar.RemoveFromInventoryByItem(Item.Food);
                            }

                        }
                        else if (playerChar.hunger <= playerChar.maxHunger && playerChar.FindItemInInventory(Item.Food) != 0)
                        {
                            if (playerChar.playerClass == PlayerClass.Survivalist)
                            {
                                if (rnd.Next(0, 3) != 0)
                                {
                                    Console.WriteLine("As a Survivalist, your constitution keeps you full, no food consumed");
                                }
                            }
                            else
                            {
                                playerChar.RemoveFromInventoryByItem(Item.Food);
                                playerChar.hunger = 10;
                            }
                        }
                        else if (playerChar.hunger != 0 && playerChar.FindItemInInventory(Item.Food) == 0)
                        {
                            if (playerChar.playerClass == PlayerClass.Survivalist)
                            {
                                if (rnd.Next(0, 3) != 0)
                                {
                                    Console.WriteLine("As a Survivalist, your constitution keeps you full, no food consumed");
                                    Console.WriteLine("You have run out of food! You can travel " + playerChar.hunger.ToString() + " rooms!");
                                }
                            }
                            else
                            {
                                playerChar.hunger -= 1;
                                Console.WriteLine("You have run out of food! You can travel " + playerChar.hunger.ToString() + " rooms!");
                            }
                        }
                        else if (playerChar.hunger <= 0 && playerChar.FindItemInInventory(Item.Food) == 0)
                        {
                            if (playerChar.playerCanRessurect)
                            {
                                playerChar.playerCanRessurect = false;
                                playerChar.AddToInventory(Item.Food, 5);
                                playerChar.hunger = 10;
                                map.TeleportPlayerToRandomLocation(playerChar);
                                Console.WriteLine("As you die, you feel yourself being whisked away. You wake up hours later in an unfamiliar location!");
                            }
                            else
                            {
                                Console.WriteLine("With no food, you starve to death!");
                                alive = false;
                                break;
                            }
                        }


                        var locationIdentifier = playerChar.Location();
                        currentRoom = map.GetRoomOrAdd(locationIdentifier);
                        currentRoom.playerVisited = true;

                        Console.WriteLine("You travel " + direction);
                        Console.WriteLine("---");
                        break;
                    }
                }

            }
            if (escaped)
            {
                Console.WriteLine("You have found the exit!");
                string roomsSurvived = map.GetRoomCount().ToString();
                Console.WriteLine(playerFullNameAndTitle + " Survived " + roomsSurvived + " rooms, and escaped the dungeon!");
                AddCharToHallofHeroes(hallOfHeroesFileLocation, playerChar, roomsSurvived);
            }
            else
            {
                Console.WriteLine("You Lose!");
                Console.WriteLine(playerFullNameAndTitle + " Survived " + map.GetRoomCount().ToString() + " rooms before dying!");
            }
        }
    }
}