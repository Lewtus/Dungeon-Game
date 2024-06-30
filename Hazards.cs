using System;
using System.Collections.Generic;
using System.Text;

namespace gam
{

    public enum HazardType
    {
        Monster,
        Boss,
        Trap
    }

    public enum Monster
    {
        Goblin,
        Ogre,
        Orc,
        Gargoyle,
        Skeleton,
        Golem,
        Imp,
        Num
    }

    public enum Boss
    {
        Minotaur,
        Guardian,
        Num
    }


    public enum Trap
    {
        Tripwire,
        Bolt,
        Ambush,
        Portal
    }

    public struct MonsterStats
    {
        public int health;
        public int damage;
    }

    public struct BossStats
    {
        public int health;
        public int damage;
        public int shield;
    }
        
    class Hazard
    {
        Random rnd = new Random();
        public Monster SummonMonster()
        {
            int RndMonsterValue = rnd.Next(0, ((int)Monster.Num)-1);
            var currentMonster = (Monster)RndMonsterValue;       
            Console.WriteLine("You encounter " + currentMonster + "s");
            return currentMonster;
        }

        public Boss SummonBoss()
        {     
           int RndBossValue = rnd.Next(0, ((int)Boss.Num) - 1);
           var currentBoss = (Boss)RndBossValue;
           Console.WriteLine("You encounter a Boss! A " + currentBoss + " approaches!");
           return currentBoss;
         
        }

        public Trap SummonTrap()
        {  
            var curentEnemy = rnd.Next(0, 4);
            var currentTrap = curentEnemy switch
            {
                0 => Trap.Bolt,
                1 => Trap.Tripwire,
                2 => Trap.Ambush,
                3 => Trap.Portal,
                _ => Trap.Portal,
            };
            return currentTrap;

        }


        public void RunTrapBehaviour(Trap? trap,Player playerChar, Room newRoom, Map map)
        {
            bool isDisarmable = false;
            switch (trap)
            {
                case Trap.Bolt:
                    isDisarmable = true;
                    //chance to damage the player
                    int chanceToHit = rnd.Next(0, 3);//doubles up as damage amount
                    if (chanceToHit >= 1)
                    {
                        playerChar.health -= chanceToHit;
                        Console.WriteLine("A Bolt flies into you! You take " + chanceToHit.ToString() + " damage!");
                    }
                    else
                    {
                        Console.WriteLine("A Bolt flies at you, but you dodge just in time!");
                    }
                    break;
                case Trap.Tripwire:
                    isDisarmable = true;
                    int chancetoTrip = rnd.Next(0, 3);//doubles up as item number dropped
                    if (chancetoTrip >= 1)
                    {
                        Console.WriteLine("As you're walking, you get snagged on a tripwire! You fall, dropping " +chancetoTrip.ToString() +" items!");

                        for (int i = 1; i <= chancetoTrip; i++)
                        {
                            playerChar.RemoveRandomFromInventory();
                        }
                    }
                    else
                    {
                        Console.WriteLine("As you walk, you notice a tripwire ahead of you!");
                    }
                    break;
                case Trap.Ambush:
                    //spawn an encounter
                    Monster newMonster = SummonMonster();
                    MonsterStats monsterStats = GetMonsterStats(newMonster);
                    
                    int chancetoAmbush = rnd.Next(0, 5);//doubles up as item number dropped
                    if (chancetoAmbush >= 2)
                    {
                        Console.WriteLine("Monsters jump out at you, catching you off guard! They strike you before you are ready!");
                        playerChar.health -= monsterStats.damage;

                    }
                    else if (chancetoAmbush == 1)
                    {
                        Console.WriteLine("Monsters jump out at you, Prepare for a fight!");

                    }
                    else if(chancetoAmbush == 0)
                    {
                        Console.WriteLine("Monsters jump out at you, but you are more than ready! You get a free strike");
                        monsterStats.health = -playerChar.damage;
                    }

                    bool combatMonsterResult = FightMonster(newMonster, playerChar);//return true is victory, return false is defeat
                    if (combatMonsterResult)
                    {
                        int randomRewardAmount = rnd.Next(1, 3);
                        for (int i = 0; i < randomRewardAmount; i++)
                        {
                            playerChar.AddToInventory(Reward());
                        }
                        newRoom.encounter = null;
                        break;
                    }
                    break;

                case Trap.Portal:
                    //teleport player to random location
                    //is this too harsh, should I make it a punishment for greed?
                    Console.WriteLine("You find an ancient looking portal, do you approach it?");
                    Console.WriteLine("Approach");
                    Console.WriteLine("Leave");
                    Console.WriteLine("---");
                    //if you approach, roll to see what happens
                    var portalInput = Console.ReadLine().ToLower();
                    bool portalClose = false;
                    while (true)
                    {
                        if (portalInput == "approach" || portalInput == "a")
                        {
                            int portalResult = rnd.Next(0, 9);
                            switch (portalResult)
                            {
                                case 0:
                                    Console.WriteLine("As you walk towards the portal, you are wrenched from your feet and pulled into it! Who knows where you are now!");
                                    map.TeleportPlayerToRandomLocation(playerChar);
                                    portalClose = true;
                                    break;
                                case 1:
                                    //player finds a boss
                                    Console.WriteLine("As you approach the portal, a Boss comes through, sealing the portal behind it! Prepare for a fight!");
                                    Boss newPortalBoss = SummonBoss();
                                    bool portalBossResult = FightBoss(newPortalBoss, playerChar);//return true is victory, return false is defeat
                                    if (portalBossResult)
                                    {
                                        int randomRewardAmount = rnd.Next(3, 7);
                                        for (int i = 0; i < randomRewardAmount; i++)
                                        {
                                            playerChar.AddToInventory(Reward());
                                        }
                                        break;
                                    }
                                    portalClose = true;
                                    break;
                                case 2:
                                case 3:
                                case 4:
                                    //player finds a monster
                                    Console.WriteLine("As you approach the portal, a Monster comes through! Prepare for a fight!");
                                    Monster newPortalMonster = SummonMonster();
                                    bool portalMonsterResult = FightMonster(newPortalMonster, playerChar);//return true is victory, return false is defeat
                                    if (portalMonsterResult)
                                    {
                                        int randomRewardAmount = rnd.Next(2, 4);
                                        for (int i = 0; i < randomRewardAmount; i++)
                                        {
                                            playerChar.AddToInventory(Reward());
                                        }
                                        newRoom.encounter = null;
                                        break;
                                    }
                                    break;
                                case 5:
                                case 6:
                                case 7:
                                    //player gets an item
                                    int randomItem = rnd.Next(0, (int)Item.Num);
                                    Item portalItem = (Item)randomItem;
                                    Console.WriteLine("As you approach the portal, an item falls out. You gain a " + portalItem);
                                    playerChar.AddToInventory(portalItem);

                                    break;
                                case 8:
                                    //player gets "gold room" - x gold and 2 equipment
                                    Console.WriteLine("You peer through the portal and see a room full of Gold and Equipment! You grab what you can before the portal closes!");
                                    playerChar.AddRandomEquipment(playerChar);
                                    playerChar.AddRandomEquipment(playerChar);
                                    int portalGoldValue = rnd.Next(3, 11);
                                    Console.WriteLine("You grab " + portalGoldValue.ToString() + "pieces of gold!");
                                    playerChar.AddToInventory(Item.Gold, portalGoldValue);

                                    portalClose = true;
                                    break;
                            }

                            if (portalClose)
                            {
                                newRoom.encounter = null;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid action");
                            }

                        }
                        else if (portalInput == "leave" || portalInput == "l")
                        {
                            //if you don't, leave
                            Console.WriteLine("You leave the portal alone.");
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
            break;
            }

            if (isDisarmable)
            {
                //does the player want to try to disarm
                Console.WriteLine("You can attempt to disarm this trap, but it could be dangerous");
                while (true)
                {
                    Console.WriteLine("Disarm");
                    Console.WriteLine("Leave");
                    Console.WriteLine("---");
                    int disarmChance = rnd.Next(0, 6);
                    var input = Console.ReadLine().ToLower();
                    bool wantToLeave = false;
                    if (playerChar.playerClass == PlayerClass.Trapmaster)
                    {
                        disarmChance += 1;
                    }

                    switch (input)
                    {
                        case "disarm":
                        case "d":
                            if (disarmChance > 2)
                            {
                                Console.WriteLine("You are able to disarm the trap! The room is now safe");
                                if (playerChar.playerClass == PlayerClass.Trapmaster)
                                {
                                    int trapmasterGoldReward = rnd.Next(1, 4);
                                    Console.WriteLine("You find a concealed compartment in the traps inner workings. Inside there is "+trapmasterGoldReward.ToString()+" gold!");
                                    playerChar.AddToInventory(Item.Gold, trapmasterGoldReward);
                                }
                                newRoom.encounter = null;
                                wantToLeave = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("In your attempt to disarm the trap, you end up cutting yourself on the mechanism! You lose a health");
                                playerChar.health -= 1;
                                break;
                            }
                        case "leave":
                        case "l":
                            Console.WriteLine("You leave the trap as is.");
                            wantToLeave = true;
                            break;
                        default:
                            break;
                    }
                    if (wantToLeave)
                    {
                        break;
                    }
                }


            }


        }



        public Item Reward()
        {
            int randomReward = rnd.Next(0, (int)Item.Num);
            while ((Item) randomReward == Item.Map)
            {
                randomReward = rnd.Next(0, (int)Item.Num);
            }
            Console.WriteLine("You find a " + (Item)randomReward);
            return (Item)randomReward;
        }

        public MonsterStats GetMonsterStats(Monster monster)
        {
            return monster switch
            {
                Monster.Goblin => new MonsterStats() { health = 5, damage = 1 },
                Monster.Orc => new MonsterStats() { health = 7, damage = 2 },
                Monster.Ogre => new MonsterStats() { health = 8, damage = 1 },
                Monster.Skeleton => new MonsterStats() { health = 5, damage = 3 },
                Monster.Gargoyle => new MonsterStats() { health = 3, damage = 4 },
                Monster.Golem => new MonsterStats() { health = 10, damage = 1 },
                Monster.Imp => new MonsterStats() { health = 2, damage = 5},
                _ => new MonsterStats()
            };
        }

        public BossStats GetBossStats(Boss boss)
        {
            BossStats bossStats = new BossStats();
            switch (boss)
            {
                case (Boss.Guardian):
                    bossStats.health = 20;
                    bossStats.damage = 2;
                    bossStats.shield = 2;
                    break;
                case (Boss.Minotaur):
                    bossStats.health = 15;
                    bossStats.damage = 5;
                    bossStats.shield = 1;
                    break;
            }
            return bossStats;
        }

        public bool FightMonster(Monster monster, Player playerChar)
        {

            MonsterStats monsterStats = GetMonsterStats(monster);
            while (monsterStats.health > 0 && playerChar.health > 0)
            {
                Console.WriteLine("Player Health: " + playerChar.health.ToString());
                Console.WriteLine(monster + " Health: " + monsterStats.health.ToString());
                Console.WriteLine("What do you do?");
                Console.WriteLine("Run Away?");
                Console.WriteLine("Fight?");
                Console.WriteLine("Eat?");
                Console.WriteLine("---");
                var input = Console.ReadLine().ToLower();
                if (input == "fight" || input == "f")
                {
                    int MonsterCombatRoll = rnd.Next(1, 11);
                    int PlayerCombatRoll = rnd.Next(1, 11);
                    if (playerChar.playerClass == PlayerClass.Assassin)
                    {
                        PlayerCombatRoll += 1;
                    }
                    else if (playerChar.playerClass == PlayerClass.Warlock)
                    {
                        MonsterCombatRoll -= 1;
                    }
                   
                    if (MonsterCombatRoll == PlayerCombatRoll)
                    {
                        Console.WriteLine("You and the " + monster + " clash, but neither deal damage");
                    }
                    else if (MonsterCombatRoll > PlayerCombatRoll)//monster hits player
                    {
                        switch (MonsterCombatRoll)
                        {
                            case 1:
                            case 2:
                                Console.WriteLine("The " + monster + " strikes you, but you take no damage!");
                                break;
                            case 9:
                            case 10:
                                Console.WriteLine("The " + monster + " strikes you hard! Critical Hit!");
                                playerChar.health -= (monsterStats.damage * 2 - playerChar.shield);
                                break;
                            default:
                                Console.WriteLine("The " + monster + " strikes you!");
                                playerChar.health -= Math.Max(monsterStats.damage - playerChar.shield, 0);
                                break;
                        }
                    }
                    else if (MonsterCombatRoll < PlayerCombatRoll)//player hits monster
                    {
                        switch (PlayerCombatRoll)
                        {
                            case 0:
                            case 1:
                                Console.WriteLine("You strike the " + monster + ", but it takes no damage!");
                                break;
                            case 9:
                            case 10:
                                Console.WriteLine("You strike the " + monster + " hard! Critical Hit!");
                                monsterStats.health -= (playerChar.damage * 2);
                                break;
                            default:
                                Console.WriteLine("You strike the " + monster);
                                monsterStats.health -= (playerChar.damage);
                                break;
                        }
                        if (playerChar.playerHasLifeSteal)
                        {
                            if (PlayerCombatRoll == 0 || PlayerCombatRoll == 1)
                            {
                                playerChar.PlayerGainsLifeFromLifeSteal(1);
                            }
                            else if (PlayerCombatRoll == 9 || PlayerCombatRoll == 10)
                            {
                                playerChar.PlayerGainsLifeFromLifeSteal(playerChar.damage * 2);
                            }
                            else
                            {
                                playerChar.PlayerGainsLifeFromLifeSteal(playerChar.damage);
                            }

                        }
                    }
                }
                else if (input.ToLower() == "eat" || input.ToLower() == "heal" || input.ToLower() == "e" || input.ToLower() == "h")
                {
                    if (playerChar.FindItemInInventory(Item.Food) > 0)
                    {
                        playerChar.RemoveFromInventoryByItem(Item.Food);
                        int healthgain = rnd.Next(1, 4);
                        Console.WriteLine("You eat a food to gain " + healthgain.ToString() + " health");
                        playerChar.health += healthgain;
                        int MonsterCombatRoll = rnd.Next(1, 11);
                        int shieldPlayerChance = rnd.Next(1, 5);
                        switch (MonsterCombatRoll)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                Console.WriteLine("The " + monster + " strikes you, but you take no damage!");
                                break;
                            case 9:
                            case 10:
                                Console.WriteLine("The " + monster + " strikes you hard! Critical Hit!");
                                if (shieldPlayerChance < 3)
                                {
                                    Console.WriteLine("You Block " + playerChar.shield + " with your shields!");
                                    playerChar.health -= Math.Max(monsterStats.damage * 2 - playerChar.shield,0);
                                }
                                else
                                {
                                    playerChar.health -= (monsterStats.damage * 2);
                                }
                                break;
                            default:
                                Console.WriteLine("The " + monster + " strikes you!");
                                if (shieldPlayerChance < 3)
                                {
                                    Console.WriteLine("You Block " + playerChar.shield + " with your shields!");
                                    playerChar.health -= Math.Max(monsterStats.damage - playerChar.shield,0);
                                }
                                else 
                                {
                                    playerChar.health -= (monsterStats.damage);
                                }
                                break;
                        }
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("You have no food!");
                        continue;
                    }

                }
                else if (input == "Run Away" || input == "run" || input == "r")
                {
                    int leaveStrike = rnd.Next(0, 5);
                    if (leaveStrike < 2)
                    {
                        Console.WriteLine("The " + monster + " strikes you as you run away!");
                        playerChar.health -= monsterStats.damage;
                    }
                    else
                    {
                        Console.WriteLine("You Run Away!");
                    }
                    return false;
                }
            }

            if (playerChar.health <= 0)
            {
                Console.WriteLine("You have been slain by the " + monster + "!");
                return false;
             }
            else if (monsterStats.health <= 0)
            {
                Console.WriteLine("You have slain the " + monster + "!");
                return true;
            }
            else
            { 
                return false;
            }
        }


        public bool FightBoss(Boss boss, Player player)
        {
            BossStats bossStats = GetBossStats(boss);
            while (bossStats.health > 0 && player.health > 0)
            {
                Console.WriteLine("Player Health: " + player.health.ToString());
                Console.WriteLine(boss + " Health: " + bossStats.health.ToString());
                Console.WriteLine("What do you do?");
                Console.WriteLine("Run Away?");
                Console.WriteLine("Fight?");
                Console.WriteLine("Eat?");
                Console.WriteLine("---");
                var input = Console.ReadLine().ToLower();
                if (input == "fight" || input == "f")
                {
                    int BossCombatRoll = rnd.Next(1, 14);
                    int PlayerCombatRoll = rnd.Next(1, 11);
                    if (BossCombatRoll == PlayerCombatRoll)
                    {
                        Console.WriteLine("You and the " + boss + " clash, but neither deal damage");
                    }
                    else if (BossCombatRoll > PlayerCombatRoll)//boss hits player
                    {
                        int shieldPlayerChance = rnd.Next(1, 5);
                        switch (BossCombatRoll)
                        {
                            case 1:
                            case 2:
                            case 3:
                                Console.WriteLine("The " + boss + " strikes you, but you take no damage!");
                                break;
                            case 11:
                            case 12:
                            case 13:
                                Console.WriteLine("The " + boss + " strikes you hard! Critical Hit!");
                                if (shieldPlayerChance < 3)
                                {
                                    Console.WriteLine("You Block " + player.shield + " with your shields!");
                                    player.health -= Math.Max(bossStats.damage * 2 - player.shield, 0);
                                }
                                else
                                {
                                    player.health -= (bossStats.damage * 2);
                                }
                                break;
                                default:
                                Console.WriteLine("The " + boss + " strikes you!");
                                if (shieldPlayerChance < 3)
                                {
                                    Console.WriteLine("You Block " + player.shield + " with your shields!");
                                    player.health -= Math.Max(bossStats.damage - player.shield, 0);
                                }
                                else
                                {
                                    player.health -= (bossStats.damage);
                                }
                                break;
                        }
                    }
                    else if (BossCombatRoll < PlayerCombatRoll)//player hits boss
                    {
                        int shieldChance = rnd.Next(1, 5);
                        switch (PlayerCombatRoll)
                        {
                            case 0:
                            case 1:
                                Console.WriteLine("You strike the " + boss + ", but it takes no damage!");
                                break;
                            case 9:
                            case 10:
                                Console.WriteLine("You strike the " + boss + " hard! Critical Hit!");
                                
                            if (shieldChance < 3)
                            {
                                Console.WriteLine("The " + boss + " has a shield, " + bossStats.shield + " negated!");
                                bossStats.health -= Math.Max(player.damage *2 - bossStats.shield,0);
                                break;
                            }
                            else
                            {
                                bossStats.health -= Math.Max(player.damage - bossStats.shield,0);
                                break;
                            }
                        default:
                            Console.WriteLine("You strike the " + boss);
                            if (shieldChance < 3)
                            {
                                Console.WriteLine("The " + boss + " has a shield, " + bossStats.shield + " negated!");
                                bossStats.health -= Math.Max(player.damage - bossStats.shield,0);
                                break;
                            }
                            else
                            {
                                bossStats.health -= Math.Max(player.damage - bossStats.shield,0);
                                break;
                            }
                        }
                    }
                }
                else if (input.ToLower() == "eat" || input.ToLower() == "heal" || input.ToLower() == "e" || input.ToLower() == "h")
                {
                    if (player.FindItemInInventory(Item.Food) > 0)
                    {
                        player.RemoveFromInventoryByItem(Item.Food);
                        int healthgain = rnd.Next(1, 4);
                        Console.WriteLine("You eat a food to gain " + healthgain.ToString() + " health");
                        player.health += healthgain;
                        int BossCombatRoll = rnd.Next(1, 14);
                        switch (BossCombatRoll)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                Console.WriteLine("The " + boss + " strikes you, but you take no damage!");
                                break;
                            case 11:
                            case 12:
                            case 13:
                                Console.WriteLine("The " + boss + " strikes you hard! Critical Hit!");
                                player.health -= (bossStats.damage * 2 - player.shield);
                                break;
                            default:
                                Console.WriteLine("The " + boss + " strikes you!");
                                player.health -= (bossStats.damage - player.shield);
                                break;

                        }
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("You have no food!");

                        continue;
                    }
                }
                else if (input == "Run Away" || input == "run" || input == "r")
                {
                    int leaveStrike = rnd.Next(0, 5);
                    if (leaveStrike < 2)
                    {
                        Console.WriteLine("The " + boss + " strikes you as you run away!");
                        player.health -= bossStats.damage;
                    }
                    else
                    {
                        Console.WriteLine("You Run Away!");
                    }
                    return false;
                }
            }

            if (player.health <= 0)
            {
                Console.WriteLine("You have been slain by the " + boss + "!");
                return false;
            }
            else
            {
                Console.WriteLine("You have slain the " + boss + "!");
                player.escaped = true;
                return true;
            }
        }
    }
}
