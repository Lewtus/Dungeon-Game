using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace gam
{
    public enum Personality
    {
        LowFoodFriendly,
        LowFoodUnfriendly,
        LowEquipmentFriendly,//this guy needs a map have equipment, check if he has one
        LowEquipmentUnfriendly,//this guy needs a map have equipment, check if he has one
        HighEquipmentFriendly,//this guy needs a map have equipment, check if he has one
        HighEquipmentUnfriendly,//this guy needs a map have equipment, check if he has one
        NoMapFriendly,//this guy needs a map to sell, check if he has one
        NoMapUnfriendly,//this guy needs a map to sell, check if he has one
        JustANiceGuy,
        JustAnAsshole,
        Num
    }


    class MerchantPersonality
    {

        Random rnd = new Random();
        //either match personality on item or on price reduction
        public Dictionary<Personality, string> personalityIntroductionText= new Dictionary<Personality, string>();
        


        
        public Item GeneratePersonalityItems(Personality merchantPersonality)
        {
            return merchantPersonality switch
            {
                Personality.LowFoodFriendly => Item.Food,
                Personality.LowFoodUnfriendly => Item.Food,
                Personality.NoMapFriendly => Item.Map,
                Personality.NoMapUnfriendly => Item.Map,
                _ => Item.Food
            };
        }

        public int GeneratePriceChangesOnPersonality(Personality merchantPersonality)
        {
            return merchantPersonality switch
            {
                Personality.LowFoodFriendly => -1,
                Personality.LowFoodUnfriendly => 1,
                Personality.LowEquipmentFriendly => -1,
                Personality.LowEquipmentUnfriendly => 2,
                Personality.HighEquipmentFriendly => -1,
                Personality.HighEquipmentUnfriendly => 2,
                Personality.NoMapFriendly => -2,
                Personality.NoMapUnfriendly => 2,
                Personality.JustANiceGuy => -1,
                Personality.JustAnAsshole => 1,
                _ => 0
            };
        }

        public string GenerateIntroTextOnPersonality(Personality merchantPersonality)
        {
            return merchantPersonality switch
            {
                Personality.LowFoodFriendly => "\"Hello Stranger, you look like you're low on food, I can sell you some for cheap!\"",
                Personality.LowFoodUnfriendly => "\"Hah, you look to be low on food there, friend. I can sell you some...for a price\"",
                Personality.LowEquipmentFriendly => "\"Travelling so light is dangerous, I can cut you a deal on equipment!\"",
                Personality.LowEquipmentUnfriendly => "\"Wow, must be hard with little equipment, adventurer. If only someone could sell you some!\"",
                Personality.HighEquipmentFriendly => "\"That's...a lot of gear..you definately get a discount!\"",
                Personality.HighEquipmentUnfriendly => "\"You think all that gear will intimidate me? Not likely!\"",
                Personality.NoMapFriendly => "\"You don't have a map? Maybe I can help you out!\"",
                Personality.NoMapUnfriendly => "\"What type of idiot comes into a dungeon with no map? Hopefully one with a lot of gold!\"",
                Personality.JustANiceGuy => "\"I'm feeling generous, discounts all round!\"",
                Personality.JustAnAsshole => "\"I charge double for adventurers, and you look the type!\"",
                _ => null
            };
        }
    }
}
