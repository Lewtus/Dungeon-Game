using System;
using System.Collections.Generic;
using System.Text;

namespace gam
{

    public enum Encounter
    {
        Monster,
        Trap,
        Gold,
        Boss,
        Food,
        Key,
        Equipment,
        Chest,
        Merchant,
        Shrine,
        Map
    }   

    static class Helper
    {
        
        public static Encounter? GenerateEncounter()
        {
            

            Random rnd = new Random();
            bool hasMonster = rnd.Next(0, 5) == 1;
            bool hasGold = rnd.Next(0, 5) == 1;
            bool hasTrap = rnd.Next(0, 7) == 1;
            bool hasBoss = rnd.Next(0, 21) == 1;
            bool hasFood = rnd.Next(0, 3) == 1;
            bool hasKey = rnd.Next(0, 9) == 1;
            bool hasEquipment = rnd.Next(0, 9) == 1;
            bool hasChest = rnd.Next(0, 9) == 1;
            bool hasMerchant = rnd.Next(0,11) == 1;
            bool hasShrine = rnd.Next(0, 15) == 1;
            bool hasMap = rnd.Next(0, 16) == 1;
         

            if (hasFood)
            {
                return Encounter.Food;
            }
            else if (hasMonster)
            {
                return Encounter.Monster;
            }
            else if (hasGold)
            {
                return Encounter.Gold;
            }
            else if (hasEquipment)
            {
                return Encounter.Equipment;
            }
            else if (hasMerchant)
            {
                return Encounter.Merchant;
            }            
            else if (hasTrap)
            {
                return Encounter.Trap;
            }           
            else if (hasChest)
            {
                return Encounter.Chest;
            }
            else if (hasMap)
            {
                return Encounter.Map;
            }
            else if (hasShrine)
            {
                return Encounter.Shrine;
            }
            else if (hasKey)
            {
                return Encounter.Key;
            }
            else if (hasBoss)
            {
                return Encounter.Boss;
            }
            else
            {
                return null;
            }
        }       
    }
}
