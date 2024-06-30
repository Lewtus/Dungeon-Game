using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace gam
{
    class Merchant
    {
        Random rnd = new Random();
        public List<Item> merchantItems = new List<Item>() { Item.Food, Item.Food };
        public List<Equipment> merchantEquipment = new List<Equipment>();
        public Dictionary<Item, int> ItempriceList = new Dictionary<Item, int>();
        public Dictionary<Equipment, int> EquipmentpriceList = new Dictionary<Equipment, int>();        
        public MerchantPersonality merchantPersonalityData = new MerchantPersonality();
        public Personality personality;
        
        public Merchant(Player playerChar, Map map)
        {
            int randomMerchantPersonality = rnd.Next(0, (int)Personality.Num);
            personality = (Personality)randomMerchantPersonality;
            GenerateMerchantItems(merchantPersonalityData , personality);
            GenerateMerchantEquipment();
            SetMerchantPrices(merchantPersonalityData, personality, playerChar, map);
        }
        

        public void GenerateMerchantItems(MerchantPersonality merchantPersonalityData, Personality merchantPersonality)
        {
            int merchantItemNumber = rnd.Next(1, 5);
            for (int i = 0; i <= merchantItemNumber; i++)
            {
                int randomItemIndex = rnd.Next(0, (int)Item.Num);
                while ((Item)randomItemIndex == Item.Gold)
                {
                    randomItemIndex = rnd.Next(0, (int)Item.Num);
                }
                merchantItems.Add((Item)randomItemIndex);
            }
            merchantItems.Add(merchantPersonalityData.GeneratePersonalityItems(merchantPersonality));
        }

        public void GenerateMerchantEquipment()
        {
            int merchantEquipmentNumber = rnd.Next(0, 3);
            for (int i = 0; i <= merchantEquipmentNumber; i++)
            {
                int randomEquipmentIndex = rnd.Next(0, (int)Equipment.Num);
                merchantEquipment.Add((Equipment)randomEquipmentIndex);
            }
        }

        public void SetMerchantPrices(MerchantPersonality merchantPersonalityData, Personality merchantPersonality, Player playerChar,Map map)
        {

            bool merchantConditionMet = CheckPersonalityCondition(merchantPersonality, playerChar);
            int merchantItemPriceVariance = 0;//for generic items
            int merchantFoodPriceVariance = 0;
            int merchantMapPriceVariance = 0;
            int merchantEquipmentPriceVariance = 0;
            if (merchantConditionMet)
            {
                int conditionTypeInt = ConvertConditionToInt(merchantPersonality);
                int merchantPriceVariance = merchantPersonalityData.GeneratePriceChangesOnPersonality(merchantPersonality);

                if (conditionTypeInt == 1)//food based cost change
                {
                    merchantFoodPriceVariance += merchantPriceVariance;
                }
                else if (conditionTypeInt == 2)//map based cost change
                {
                    merchantMapPriceVariance += merchantPriceVariance;
                }
                else if (conditionTypeInt == 3)//equipment based cost change
                {
                    merchantEquipmentPriceVariance += merchantPriceVariance;
                }
                else if (conditionTypeInt == 4)//generic cost change
                {
                    merchantItemPriceVariance += merchantPriceVariance;
                    merchantEquipmentPriceVariance += merchantPriceVariance;
                    merchantFoodPriceVariance += merchantPriceVariance;
                    merchantMapPriceVariance += merchantPriceVariance;
                }

                if (playerChar.playerClass == PlayerClass.Hustler)
                {
                    merchantItemPriceVariance = Math.Max(merchantItemPriceVariance -1, 0);
                    merchantEquipmentPriceVariance = Math.Max(merchantEquipmentPriceVariance -1, 0);
                    merchantFoodPriceVariance = Math.Max(merchantFoodPriceVariance -1, 0);
                    merchantMapPriceVariance = Math.Max(merchantMapPriceVariance -1, 0);
                }

                merchantItemPriceVariance = merchantPriceVariance - playerChar.merchantRep;
                merchantEquipmentPriceVariance = merchantPriceVariance - playerChar.merchantRep;
                merchantFoodPriceVariance = merchantPriceVariance - playerChar.merchantRep;
                merchantMapPriceVariance = merchantPriceVariance - playerChar.merchantRep;

            }
            
            ItempriceList.Add(Item.Food, rnd.Next(2,5)+ merchantFoodPriceVariance);
            ItempriceList.Add(Item.Key, rnd.Next(2, 6) + merchantItemPriceVariance);
            ItempriceList.Add(Item.Map, rnd.Next(5, 9) + merchantMapPriceVariance);
            EquipmentpriceList.Add(Equipment.Armour, rnd.Next(8, 13) + merchantEquipmentPriceVariance);
            EquipmentpriceList.Add(Equipment.Axe, rnd.Next(8,14) + merchantEquipmentPriceVariance);
            EquipmentpriceList.Add(Equipment.Potion, rnd.Next(6,11) + merchantEquipmentPriceVariance);
            EquipmentpriceList.Add(Equipment.Sword, rnd.Next(7,12) + merchantEquipmentPriceVariance);
            EquipmentpriceList.Add(Equipment.Shield, rnd.Next(7,11) + merchantEquipmentPriceVariance);

           
        }

        public int ConvertConditionToInt(Personality merchantPersonality)
        {
            int conditionTypeReturn = 0;
            if (merchantPersonality == Personality.LowFoodFriendly || merchantPersonality == Personality.LowFoodUnfriendly)
            {
                conditionTypeReturn = 1;
            }
            else if (merchantPersonality == Personality.NoMapFriendly || merchantPersonality == Personality.NoMapUnfriendly)
            {
                conditionTypeReturn = 2;
            }
            else if (merchantPersonality == Personality.LowEquipmentFriendly || merchantPersonality == Personality.LowEquipmentUnfriendly
                || merchantPersonality == Personality.HighEquipmentFriendly || merchantPersonality == Personality.HighEquipmentUnfriendly)
            {
                conditionTypeReturn = 3;
            }
            else if (merchantPersonality == Personality.JustANiceGuy || merchantPersonality == Personality.JustAnAsshole)
            {
                conditionTypeReturn = 4;
            }
            return conditionTypeReturn;
        }

        public bool CheckPersonalityCondition(Personality merchantPersonality, Player playerChar)
        {
            
            bool conditionMet = false;
            switch (merchantPersonality)
            {
                case Personality.LowFoodFriendly:
                case Personality.LowFoodUnfriendly:
                    if (playerChar.FindItemInInventory(Item.Food) <= 5)
                    {
                        conditionMet = true;
                    }
                    break;
                case Personality.LowEquipmentFriendly:
                case Personality.LowEquipmentUnfriendly:
                    if (playerChar.GetCountEquipment() <= 0)
                    {
                        conditionMet = true;
                    }
                    break;
                case Personality.HighEquipmentFriendly:
                case Personality.HighEquipmentUnfriendly:
                    if (playerChar.GetCountEquipment() >= 3)
                    {
                        conditionMet = true;
                    }
                    break;
                case Personality.NoMapFriendly:
                case Personality.NoMapUnfriendly:
                    if (playerChar.FindItemInInventory(Item.Map) >= 0)
                    {
                        conditionMet = true;
                    }
                    break;
                case Personality.JustANiceGuy:
                    conditionMet = true;
                    break;
                case Personality.JustAnAsshole:
                    conditionMet = true;
                    break;
                default:
                    break;
            }
            return conditionMet;
        }

      

        public void RunMerchantBehaviour(Player playerChar)
        {
            if (CheckPersonalityCondition(personality, playerChar))
            {
                Console.WriteLine(merchantPersonalityData.GenerateIntroTextOnPersonality(personality));

            }
            else
            {
                Console.WriteLine("\"Greetings, Adventurer, care to browse my wares?\"");
            }
            while (true)
            {
                Console.WriteLine("---");
                Console.WriteLine("Buy");
                Console.WriteLine("Sell");
                Console.WriteLine("Leave");
                Console.WriteLine("---");
                var input = Console.ReadLine();
                if (input.ToLower() == "buy" || input.ToLower() == "b")
                {
                    RunBuyBehaviour(playerChar);
                }
                else if (input.ToLower() == "sell" || input.ToLower() == "s")
                {
                    RunSellBehaviour(playerChar);
                }
                else if (input.ToLower() == "leave" || input.ToLower() == "l")
                {
                    return;
                }
            }
        }


        void RunBuyBehaviour(Player playerChar)
        { //display player's current gold

            var orderedInventoryItems = merchantItems.GroupBy(i => i);
            if (merchantItems.Count == 0)
            {
                }
            foreach (var type in orderedInventoryItems)
            {
                Console.WriteLine("{0} ({1}) Cost - {2}", type.Key, type.Count(), ItempriceList[type.Key]);
            }

            var orderedInventoryEquipment = merchantEquipment.GroupBy(i => i);
            if (merchantItems.Count == 0)
            {
            }
            foreach (var type in orderedInventoryEquipment)
            {
                Console.WriteLine("{0} ({1}) Cost - {2}", type.Key, type.Count(),EquipmentpriceList[type.Key]);
            }



            Console.WriteLine("None");
            Console.WriteLine("---");
            while (true)
            {
                int playerGoldCount = playerChar.FindItemInInventory(Item.Gold);
                Console.WriteLine("Current Player Gold: " + playerGoldCount);
                Console.WriteLine("Please type what you wish to purchase");
                //allow player to type the item they want
                var buyInput = Console.ReadLine();
                if (buyInput.ToLower() == "none")
                {
                    break;
                }
                else
                {
                    Item? wantedItem = null;
                    Equipment? wantedEquipment = null;
                    foreach (Item item in merchantItems)
                    {
                        if (buyInput.ToLower() == item.ToString().ToLower())
                        {
                            wantedItem = item;
                            break;
                        }
                    }
                    foreach (Equipment equipment in merchantEquipment)
                    {
                        if (buyInput.ToLower() == equipment.ToString().ToLower())
                        {
                            wantedEquipment = equipment;
                            break;
                        }
                    }
                    //check to see if player has enough gold
                    if (wantedItem != null)
                    {
                        if (ItempriceList[wantedItem.Value] <= playerGoldCount)
                        {
                            //if they do, - gold + item
                            Console.WriteLine("\"Thank you for your purchase!\"");
                            playerChar.AddToInventory(wantedItem.Value);
                            for (int i = 0; i < ItempriceList[wantedItem.Value]; i++)
                            {
                                playerChar.RemoveFromInventoryByItem(Item.Gold);
                                

                            }
                            merchantItems.Remove(wantedItem.Value);
                            Console.WriteLine("You Exchange " + ItempriceList[wantedItem.Value].ToString() + " Gold for a " + wantedItem);
                        }
                        else
                        {
                            //if they don't, tell them it's too expensive
                            Console.WriteLine("Hey, you don't have enough Gold!");
                            break;
                        }

                    }
                    else if (wantedEquipment != null)
                    {
                        if (EquipmentpriceList[wantedEquipment.Value] <= playerGoldCount)
                        {
                            //if they do, - gold + item
                            Console.WriteLine("\"Thank you for your purchase!\"");
                            playerChar.AddToEquipment((Equipment)wantedEquipment, playerChar);
                            for (int i = 0; i <
                                EquipmentpriceList[wantedEquipment.Value]; i++)
                            {
                                playerChar.RemoveFromInventoryByItem(Item.Gold);
                            }
                            merchantEquipment.Remove(wantedEquipment.Value);
                            Console.WriteLine("You Exchange " + EquipmentpriceList[wantedEquipment.Value].ToString() + " Gold for a " + wantedEquipment);
                        }
                        else
                        {
                            //if they don't, tell them it's too expensive
                            Console.WriteLine("Hey, you don't have enough Gold!");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\"That's not an item! What do you want?\"");
                        break;
                    }
                }
            }
        }

        void RunSellBehaviour(Player playerChar)
        {
            //display player's inventory, with a price per item
            Console.WriteLine("What do you want to Sell?");
            Console.WriteLine("---");
            playerChar.DisplayInventory();
            Console.WriteLine("none");
            Console.WriteLine("---");
            //allow them to type which item to sell
            while (true)
            {
                var sellInput = Console.ReadLine();
                //- item + gold
                if (sellInput.ToLower() == "none")
                {
                    break;
                }
                else
                {
                    switch (sellInput.ToLower())
                    {
                        case "food":
                            Console.WriteLine("\"I can take that off your hands!\"");
                            playerChar.RemoveFromInventoryByItem(Item.Food);
                            playerChar.AddToInventory(Item.Gold, ItempriceList[Item.Food]);
                            merchantItems.Add(Item.Food);
                            break;
                        case "key":
                            Console.WriteLine("\"I can take that off your hands!\"");
                            playerChar.RemoveFromInventoryByItem(Item.Key);
                            playerChar.AddToInventory(Item.Gold, ItempriceList[Item.Key]);
                            merchantItems.Add(Item.Key);
                            break;
                        case "map":
                            Console.WriteLine("\"I can take that off your hands!\"");
                            playerChar.RemoveFromInventoryByItem(Item.Map);
                            playerChar.AddToInventory(Item.Gold, ItempriceList[Item.Map]);
                            merchantItems.Add(Item.Map);
                            break;
                        case "gol.d":
                            Console.WriteLine("I can't buy that!");
                            break;
                        default:
                            Console.WriteLine("Hey! That's not an item");
                            break;
                    }
                }
            }
        }
    }
}
