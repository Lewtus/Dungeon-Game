using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace gam
{
    public enum ShrineType
    {
        Blood,
        Phoenix,
        Vitality,
        Sustenance,
        Suffering,
        Wayward,
        Wary,
        Undeath,
        Wealth,
        Commerce,
        Num
    }
    public enum ShrineBlessing
    {
        GrantVampire,
        GrantRessurection,
        GrantLifeSteal,
        GrantNoNeedForFood,
        DealMoreDamage,
        GainShield,
        GetGivenMap,        
        BecomeLichPositive,
        GiveGold,
        SetMerchantsPositive,
    }

    public enum ShrineCurse
    {
        LoseVampire,
        LoseRessurection,
        DrainHunger,
        RemoveLifeSteal,
        TakeMoreDamage,
        TakeAwayShield,
        LoseMapProgress,
        BecomeLichNegative,
        TakeGold,
        SetMerchantsNegative,
    }

    class Shrine
    {
        Random rnd = new Random();
        public ShrineType shrineType;
        public bool shrinePositive;
        public ShrineBlessing? shrineEncounterPositive;
        public ShrineCurse? shrineEncounterNegative;




        public Shrine(Player playerChar)
        {
            //shrinePositive = false; - ENABLE FOR DEBUG SHRINE TESTING
            //shrineType = (ShrineType)8;
            shrinePositive = ShrineBlessedOrCursed(playerChar);
            shrineType = GenerateShrineEncounterType();
            GenerateShrineEncounters(shrineType, shrinePositive);
            GenerateShrineLoot(playerChar);
        }


        public bool ShrineBlessedOrCursed(Player playerChar)
        {
            int ShrineRoll = rnd.Next(1, 5);
            if (playerChar.playerClass == PlayerClass.Vampire) //allows for modifications to shrines being cursed depending on class
            {
                ShrineRoll = ShrineRoll - 1;
            }
            if (ShrineRoll < 3)
            {
                //Blessed
                return true;
            }
            else
            {
                //cursed
                return false;
            }
        }

        public void GenerateShrineEncounters(ShrineType shrineType, bool shrinePositive)
        {
            if (shrinePositive)
            {
                shrineEncounterPositive = GenerateShrineEncountersPositive(shrineType);
            }
            else
            {
                shrineEncounterNegative = GenerateShrineEncountersNegative(shrineType);
            }

        }

        public ShrineBlessing GenerateShrineEncountersPositive(ShrineType shrineType)
        {
            return shrineType switch
            {
                ShrineType.Blood => ShrineBlessing.GrantVampire,
                ShrineType.Phoenix => ShrineBlessing.GrantRessurection,
                ShrineType.Vitality => ShrineBlessing.GrantLifeSteal,
                ShrineType.Sustenance => ShrineBlessing.GrantNoNeedForFood,
                ShrineType.Suffering => ShrineBlessing.DealMoreDamage,
                ShrineType.Wayward => ShrineBlessing.GetGivenMap,
                ShrineType.Wary => ShrineBlessing.GainShield,
                ShrineType.Undeath => ShrineBlessing.BecomeLichPositive,
                ShrineType.Wealth => ShrineBlessing.GiveGold,
                ShrineType.Commerce => ShrineBlessing.SetMerchantsPositive,
                _ => ShrineBlessing.GiveGold
            };
        }

        public ShrineCurse GenerateShrineEncountersNegative(ShrineType shrineType)
        {
            return shrineType switch
            {
                ShrineType.Blood => ShrineCurse.LoseVampire,
                ShrineType.Phoenix => ShrineCurse.LoseRessurection,
                ShrineType.Vitality => ShrineCurse.RemoveLifeSteal,
                ShrineType.Sustenance => ShrineCurse.DrainHunger,
                ShrineType.Suffering => ShrineCurse.TakeMoreDamage,
                ShrineType.Wayward => ShrineCurse.LoseMapProgress,
                ShrineType.Wary => ShrineCurse.TakeAwayShield,
                ShrineType.Undeath => ShrineCurse.BecomeLichNegative,
                ShrineType.Wealth => ShrineCurse.TakeGold,
                ShrineType.Commerce => ShrineCurse.SetMerchantsNegative,
                _ => ShrineCurse.TakeGold
            };
        }

        public ShrineType GenerateShrineEncounterType()
        {
            int newShrineInt = rnd.Next(0, (int)ShrineType.Num);
            return (ShrineType)newShrineInt;
        }

        public Item[] GenerateShrineLoot(Player playerChar)
        {
            List<Item> shrineLoot = new List<Item>();

            if (IsPlayerPunishedForLoot())
            {

                int takenLoot = rnd.Next(1, 4);
                Console.WriteLine("As you approach the loot, a booming voice echoes around you! \"YOU DARE PILLAGE MY SHRINE, FIEND?!\"");
                switch (takenLoot.ToString())
                {
                    case ("1"):
                        Console.WriteLine("you feel a force reach into your backpack and snatch " + takenLoot.ToString() + " piece of loot!");
                        break;
                    default:
                        Console.WriteLine("you feel a force reach into your backpack and snatch " + takenLoot.ToString() + " pieces of loot!");
                        break;
                }

                for (int i = 0; i <= takenLoot; i++)
                {
                    int randomItem = rnd.Next(0, (int)Item.Num);
                    playerChar.RemoveRandomFromInventory();

                }
            }
            else
            {
                int lootCount = rnd.Next(1, 4);


                for (int i = 0; i <= lootCount; i++)
                {
                    int randomItem = rnd.Next(0, (int)Item.Num);
                    playerChar.AddToInventory((Item)randomItem);

                }
            }
            return shrineLoot.ToArray();
        }

        public void RunShrineBehaviour(Player playerChar)
        {
            bool shrineUsed = false;
            switch (shrineType)
            {
                case ShrineType.Blood:
                    Console.WriteLine("It's a shrine of Blood");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Phoenix:
                    Console.WriteLine("It's a shrine of the Phoenix");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Vitality:
                    Console.WriteLine("It's a shrine of Vitality");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Sustenance:
                    Console.WriteLine("It's a shrine of Sustenance");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Suffering:
                    Console.WriteLine("It's a shrine of Suffering");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Wayward:
                    Console.WriteLine("It's a shrine of Wayward");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Wary:
                    Console.WriteLine("It's a shrine of the Wary");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Undeath:
                    Console.WriteLine("It's a shrine of Undeath");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Wealth:
                    Console.WriteLine("It's a shrine of Wealth");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Commerce:
                    Console.WriteLine("It's a shrine of Commerce");
                    shrineUsed = DoesPlayerUseShrine();
                    break;
                case ShrineType.Num:
                    Console.WriteLine("I will either give or steal gold");
                    break;
                default:
                    break;

            }

            if (shrineUsed)//either bless or curse the player
            {
                GrantPlayertShrineEffect(shrineType, shrinePositive, playerChar);
            }
        }


        public bool DoesPlayerUseShrine()
        {
            Console.WriteLine("Do you accept the Shrine's blessing? It may not be what you desire...");
            Console.WriteLine("Yes");
            Console.WriteLine("No");
            Console.WriteLine("---");
            bool usedShrine;
            while (true)
            {
                var input = Console.ReadLine().ToLower();

                if (input.ToLower() == "yes" || input.ToLower() == "y")
                {
                    usedShrine = true;
                    break;
                }
                else if (input.ToLower() == "no" || input.ToLower() == "n")
                {
                    usedShrine = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Do you accept the Shrine's blessing? It may not be what you desire...");
                    Console.WriteLine("Yes");
                    Console.WriteLine("No");
                    Console.WriteLine("---");
                    continue;
                }
            }
            return usedShrine;
        }


        public void GrantPlayertShrineEffect(ShrineType shrineType, bool shrinepositive, Player playerChar)
        {
             switch (shrineType)
            {
                case ShrineType.Blood:
                    if (shrinepositive)
                    {
                        if (!playerChar.playerIsVampire)
                        {
                            Console.WriteLine("You feel a tingling cold pass through your veins. You have become a vampire!");
                            playerChar.playerIsVampire = true;
                        }
                        else
                        {
                            Console.WriteLine("The shrine enhances your vampiric capabilities!");
                            playerChar.damage = playerChar.damage + 2;
                        }
                    }
                    else
                    {
                        if (playerChar.playerIsVampire)
                        {
                            Console.WriteLine("You feel the shrine drain your vampiric powers away. You are now back to being human!");
                            playerChar.playerIsVampire = false;
                        }
                        else
                        {
                            Console.WriteLine("The shrine drains your lifeforce!");
                            playerChar.health = playerChar.health - 5;
                        }
                    }
                    break;
                case ShrineType.Phoenix:
                    if (shrinepositive)
                    {
                        if (!playerChar.playerCanRessurect)
                        {
                            Console.WriteLine("The fire of the Phoenix wraps around you! You have been granted a second life");
                            playerChar.playerCanRessurect = true;
                        }
                        else
                        {
                            Console.WriteLine("You are already imbued by the Phoenix's restoring flames. Instead, you feel your fists begin to burn");
                            playerChar.damage = playerChar.damage + 2;
                        }
                    }
                    else
                    {
                        if (playerChar.playerCanRessurect)
                        {
                            Console.WriteLine("The wrath of the flaming bird scours away your magical protection! You have lost your ability to ressurect!");
                            playerChar.playerIsVampire = false;
                        }
                        else
                        {
                            Console.WriteLine("The Phoenix springs to life, engulfing you in flames and burning one of your items to ash");
                            playerChar.RemoveRandomFromInventory();
                        }
                    }
                    break;
                case ShrineType.Vitality:
                    if (shrinepositive)
                    {
                        if (!playerChar.playerHasLifeSteal)
                        {
                            Console.WriteLine("The shrine grants you the ability to leech health from enemies!");
                            playerChar.playerHasLifeSteal = true;
                        }
                        else
                        {
                            Console.WriteLine("The shrine produces a dark red liquid. When it touches you, you feel renewed. Health restored.");
                            playerChar.health = 15;
                        }
                    }
                    else
                    {
                        if (playerChar.playerHasLifeSteal)
                        {
                            Console.WriteLine("The shrine snaps at your hand as you touch it, causing you to bleed. You feel your power flow away with the gush of blood. You lose the ability to lifesteal ");
                            playerChar.playerHasLifeSteal = false;
                        }
                        else
                        {
                            Console.WriteLine("Pain erupts as the shrine clamps down on your hand, severing a finger. You lose combat effectiveness");
                            playerChar.damage --;
                        }
                    }
                    break; 
                case ShrineType.Sustenance:
                    if (shrinepositive)
                    {
                        if (!playerChar.playerNeedsFood)
                        {
                            Console.WriteLine("As you touch the shrine, you feel contentedly full. The sensation doesn't seem to fade.");
                            playerChar.playerNeedsFood = true;
                        }
                        else
                        {
                            Console.WriteLine("The aura of the shrine makes you feel unstoppable. Maximum health increased.");
                            playerChar.health = 20;
                        }
                    }
                    else
                    {
                        if (playerChar.playerHasLifeSteal)
                        {
                            Console.WriteLine("The shrine snaps at your hand as you touch it, causing you to bleed. You feel your power flow away with the gush of blood causing you lose the ability to lifesteal!");
                            playerChar.playerHasLifeSteal = false;
                        }
                        else
                        {
                            Console.WriteLine("Pain erupts as the shrine clamps down on your hand, severing a finger. You lose combat effectiveness");
                            playerChar.damage--;
                        }
                    }
                    break;
                case ShrineType.Suffering:
                    if (shrinepositive)
                    {
                        Console.WriteLine("The Shrine grants you power to bring suffering to those who cross you!");
                        playerChar.damage ++;
                    }
                    else
                    {
                        Console.WriteLine("The Shrine causes you great suffering. Who could have seen this coming?!");
                        playerChar.health -= 10;
                    }
                    break;
                case ShrineType.Wayward:
                    if (shrinepositive)
                    {
                        Console.WriteLine("A beacon to the lost, the shrine grants you knowledge of the dungeon");
                        playerChar.playerSightWithoutMap = true;
                    }
                    else
                    {
                        Console.WriteLine("You feel the darkness closing in, you have lost your map, and your sense of direction!");
                        playerChar.playerSightWithoutMap = false;
                        playerChar.RemoveFromInventoryByItem(Item.Map);

                    }
                    break;
                case ShrineType.Wary:
                    if (shrinepositive)
                    {
                        Console.WriteLine("The shrine wards you against the dark, and reveals your path. Defense Increased. You have gained a map");
                        playerChar.shield++;
                        playerChar.AddToInventory(Item.Map, 1);

                    }
                    else if (!shrinepositive & playerChar.shield == 0)
                    {
                        Console.WriteLine("You feel the shadows close in. Fear runs down your spine, causing you to rush away from the statue. In your haste, you drop an item");
                        playerChar.RemoveRandomFromInventory();

                    }
                    else
                    {
                        Console.WriteLine("You feel paranoid and cannot turn your gaze away from the shrine. It rots in your mind, dampening your focus and making it harder to defend yourself.");
                        playerChar.shield--;

                    }
                    break;
                case ShrineType.Undeath:
                    if (shrinepositive)
                    {
                        if (playerChar.playerIsLich == false)
                        {
                            Console.WriteLine("You feel your blood slow to a crawl, yet you still stand. You have gained the powers of unlife, and become a lich!");
                            playerChar.playerIsLich = true;
                        }
                        else
                        {
                            Console.WriteLine("Your pulse slows even further, and you feel cold rushing to the tips of your fingers. Your lich powers are enhanced");
                            playerChar.damage = playerChar.damage + 2;
                            playerChar.shield = playerChar.shield + 1;
                        }
                    }
                    else
                    {
                        if (playerChar.playerIsLich == true)
                        {
                            Console.WriteLine("A sickening warmth flows through your body, burning away your power as it travels. You lose your ghoulish powers, and are no longer a lich!");
                            playerChar.playerIsLich = false;
                        }
                        else
                        {
                            Console.WriteLine("A shambling Skeleton emerges from the shrine and advances towards you!");
                            Hazard newHazard = new Hazard();
                            Monster newMonster = Monster.Skeleton;
                            bool combatMonsterResult = newHazard.FightMonster(newMonster, playerChar);
                            if (combatMonsterResult)
                            {
                                int randomRewardAmount = rnd.Next(1, 6);
                                for (int i = 0; i < randomRewardAmount; i++)
                                    playerChar.AddToInventory(newHazard.Reward());
                            }
                            else
                            {
                                playerChar.playerAlive = false; 
                                break;
                            }
                        }
                    }
                    break;
                case ShrineType.Wealth:
                    if (shrinepositive)
                    {
                        Console.WriteLine("Your pack suddenly feels heavier. Looking inside, you see it's full of gold!");
                        playerChar.AddToInventory(Item.Gold, 25);
                    }
                    else
                    {
                        Console.WriteLine("Your pack feels lighter, as the shrine steals the coins from your pack!");
                        int loopCount = 0;
                        while (loopCount < 25) //man writes worst code in history, asked to leave function
                        {
                            playerChar.RemoveFromInventoryByItem(Item.Gold);
                            loopCount++;
                        }
                        
                    }
                    break;
                case ShrineType.Commerce:
                    if (shrinepositive)
                    {
                        Console.WriteLine("Kneeling before the shrine, you are gifted with a vision of how to converse with merchants. You are able use your newfound knowledge to get a discount from merchants");
                        playerChar.merchantRep++;

                    }
                    else
                    {
                        Console.WriteLine("The Shrine rumbles, chiding you for your wanten beggary. All merchants will be more hostile!");
                        playerChar.merchantRep--;

                    }
                    break;
                case ShrineType.Num:
                    Console.WriteLine("You have successfully broken my shrines. I can't believe it");
                    break;
                default:
                    break;

            }
        }

        public bool IsPlayerPunishedForLoot()
        {
            //will the player get punished for taking the loot
            bool playerPunishedChance = (rnd.Next(0, 3) == 1);
            return playerPunishedChance;

        }


    }   
}
