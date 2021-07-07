using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RecipeTree.Processes
{
    class InventoryChecker
    {
        public static bool CheckItem(Player player, string itemName, int curStackSize)
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
    }
}
