using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Diagnostics;
using ReLogic.Reflection;

namespace RecipeTree.Processes
{
    class ItemChecker
    {
        public static Dictionary<string, Item> allItems = makeItemDict();

        public static Dictionary<string, Item> makeItemDict()
        {
            Dictionary<string, Item> itemDict = new Dictionary<string, Item>();
            for (int type = 1; type < ItemLoader.ItemCount; type++)
            {
                Item it = new Item();
                if (type >= ItemID.Count)
                {
                    ModItem mItem = ItemLoader.GetItem(type);
                    it.SetDefaults(mItem.item.Clone().type);
                    
                }
                else
                {
                    it.SetDefaults(type);
                }
                itemDict[it.Name] = it;
            }

            return itemDict;
        }

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
            if (allItems.ContainsKey(itemStr))
            {
                return allItems[itemStr];
            }
            return null;
        }

        public static List<string> GetClosestMatches(string itemName)
        {
            int LevenshteinDistance(string s, string t)
            {
                int m = s.Length + 1;
                int n = t.Length + 1;

                int[,] d = new int[m, n];

                for (int i = 1; i < m; i++) d[i, 0] = i;

                for (int j = 1; j < n; j++) d[0, j] = j;

                for (int j = 1; j < n; j++)
                {
                    for (int i = 1; i < m; i++)
                    {
                        int subCost;

                        if (s[i - 1] == t[j - 1])
                            subCost = 0;
                        else
                            subCost = 1;

                        List<int> possibleValues = new List<int>() { 
                            d[i - 1, j] + 1,  
                            d[i, j - 1] + 1,
                            d[i - 1, j - 1] + subCost
                        };
                        d[i, j] = possibleValues.Min();
                    }
                }

                return d[m - 1, n - 1];
            }

            List<string> allItemNames = allItems.Keys.ToList();
            Dictionary<int, List<string>> values = new Dictionary<int, List<string>>();

            int maxCost = 8;
            for (int i = 0; i <= maxCost; i++)
            {
                values[i] = new List<string>();
            }

            foreach (string compItem in allItemNames)
            {
                if (compItem != "")
                {
                    int cost = LevenshteinDistance(itemName, compItem);
                    if (cost < maxCost)
                    {
                        values[cost].Add(compItem);
                    }
                }
            }

            int chosenNum = 5;
            List<string> chosenItems = new List<string>();
            for (int i = 0; i <= maxCost; i++)
            {
                if (chosenItems.Count == chosenNum)
                {
                    break;
                }
                int rem = chosenNum - chosenItems.Count;
                rem = rem > values[i].Count ? values[i].Count : rem;
                //chosenItems.AddRange(values[i].GetRange(0, rem));
                foreach (string w in values[i].GetRange(0, rem))
                {
                    if (itemName != "")
                    {
                        if (w[0] == char.ToUpper(itemName[0]) || w[0] == char.ToLower(itemName[0]))
                        {
                            chosenItems.Add(w);
                        }
                    }
                }
            }

            return chosenItems;
        }
    }
}
