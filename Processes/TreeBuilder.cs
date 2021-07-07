using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RecipeTree.Processes
{
    class TreeBuilder
    {
        public static List<(string, bool)> MakeTree(string[] itemWords, Player player, int textSetting)
        {
            // Join the string array to create a space seperated string
            string itemToFind = String.Join(" ", itemWords);

            // Dictionary to contain all possible recipes for one item -> {recipeOBJ: [(item name, stack size), ...], ...}
            Dictionary<Recipe, List<(int, string, int)>> recipeItems = GetRecipes(itemToFind);

            // String and bool tuple to store whether the player can craft the item currently
            List<(string, bool)> recipeStrings = new List<(string, bool)>();

            // if no recipes have been found, then show player error message
            if (recipeItems.Count == 0)
            {
                // f-string equivalent
                recipeStrings.Add(($"Cannot find any recipes for {itemToFind}", false));
            }
            else
            { 
                // recipe = {recipeID: [(itemID, itemName, stackSize), ...]}
                foreach (var recipe in recipeItems)
                {
                    bool playerCanCraft = true;
                    foreach(var v in recipe.Value)
                    {
                        // Chekcs to see if the player has the required item in their inventory
                        if (!InventoryChecker.CheckItem(player, v.Item2, v.Item3))
                        {
                            // if the player doesn't have one of any of the required items, they cannot craft and therefore the loop breaks
                            playerCanCraft = false;
                            break;
                        }
                    }
                    // Python map/list comprehension equivalent
                    // Concatenate the item name and amount in each tuple of the recipe, uses textSetting to determine what the string displays
                    var strPairs = recipe.Value.Select(
                        p => (
                            (textSetting < 3 ? p.Item2 : "") + 
                            (textSetting != 2 ? $"[i/s{p.Item3}:{p.Item1}]" : "") + 
                            (textSetting < 3 ? ": " : "") + 
                            (textSetting < 3 ? p.Item3.ToString() : "")
                        )
                    );
                    string recipeStr = String.Join(", ", strPairs);
                    recipeStrings.Add((recipeStr, playerCanCraft));
                }
            }
            
            return recipeStrings;
        }

        private static Dictionary<Recipe, List<(int, string, int)>> GetRecipes(string itemToFind)
        {
            // Dictionary to contain all possible recipes for one item -> {recipeOBJ: [(item name, stack size), ...], ...}
            Dictionary<Recipe, List<(int, string, int)>> recipeItems = new Dictionary<Recipe, List<(int, string, int)>>();

            // Loop through all recipes and check whether the item is the result of the recipe
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe r = Main.recipe[i];
                if (r.createItem.Name == itemToFind)
                {
                    List<(int, string, int)> tempList = new List<(int, string, int)>();

                    // loop through all the items in the recipe
                    foreach (Item it in r.requiredItem)
                    {
                        // Filter out any items with an id == 0, don't know why they are there
                        if (it.netID != 0)
                        {
                            // Append the item's id, name and stack size to the list
                            (int, string, int) recipeComponent = (it.netID, it.Name, it.stack);
                            tempList.Add(recipeComponent);
                        }
                    }

                    // Set the key-value pair of the recipe and the required items in the recipeItems dictionary
                    recipeItems[r] = tempList;
                }

            }

            return recipeItems;
        }

    }
}
