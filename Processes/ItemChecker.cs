using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Diagnostics;
using ReLogic.Reflection;

namespace RecipeTree.Processes
{
    class ItemChecker
    {
        public static bool CheckInventory(Player player, string itemName, int curStackSize)
        {
            // if stackSize == 0, only check for presence
            for (int i = 0; i < 58; i++)
            {
                if (player.inventory[i].Name == itemName)
                {
                    if (player.inventory[i].stack >= curStackSize || curStackSize == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Item GetItemID(string itemStr)
        {
            // Currently only works for vanilla items

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Iterative Method

            // Iterate through every possible item ID and check if the item instance made using that ID has the same name as the requested name

            // Can use either Main.maxItemTypes or ItemLoader.ItemCount both = 3930
            // ItemLoader.ItemCount derives it from assigning a private var to ItemID.Count which = 3930
            // Also in ItemID soucre code there is a dictionary called Search, and contains all vanilla items as const with the name and id value

            //for(int type = 1; type < ItemLoader.ItemCount; type++)
            //{
            //    Item it = new Item();
            //    it.SetDefaults(type, false);
            //    if (it.Name == item)
            //    {
            //        return it.netID;
            //    }

            //}

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Getattr Method

            // The ItemID class has fields for every vanilla item using the field name as the item name (without spaces) and the value is the item ID
            // Method:
            // 1. Remove all spaces (whitespace) from the input string
            // 2. Check if the field with the requested string exists
            // 3. If it does exist, get the value of the field using the GetField and GetValue methods

            Item item = new Item();
            string itemName = String.Concat(itemStr.Where(x => !Char.IsWhiteSpace(x)));
            if (typeof(ItemID).GetField(itemName) != null)
            {
                var id = typeof(ItemID).GetField(itemName).GetValue(null);
                item.SetDefaults(Int32.Parse(id.ToString()), false);

                return item;
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------
            return null;
        }
    }
}
