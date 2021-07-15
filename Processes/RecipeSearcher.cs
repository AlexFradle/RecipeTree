using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RecipeTree.Processes
{
    class RecipeSearcher
    {
        private static Random rng = new Random();
        public static List<(string, bool)> MakeTextTree(string itemToFind, Player player, int textSetting)
        {
            // Dictionary to contain all possible recipes for one item -> {recipeOBJ: [(item name, stack size), ...], ...}
            Dictionary<Recipe, List<Item>> recipeItems = GetRecipes(itemToFind);

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
                // recipe = {recipeID: [Item, ...]}
                foreach (var recipe in recipeItems)
                {
                    bool playerCanCraft = true;
                    foreach(var v in recipe.Value)
                    {
                        // Chekcs to see if the player has the required item in their inventory
                        if (!ItemChecker.CheckInventory(player, v.Name, v.stack))
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
                            (textSetting < 3 ? p.Name : "") + 
                            (textSetting != 2 ? $"[i/s{p.stack}:{p.netID}]" : "") + 
                            (textSetting < 3 ? ": " : "") + 
                            (textSetting < 3 ? p.stack.ToString() : "")
                        )
                    );
                    string recipeStr = String.Join(", ", strPairs);
                    recipeStrings.Add((recipeStr, playerCanCraft));
                }
            }
            
            return recipeStrings;
        }

        public static Dictionary<Recipe, List<Item>> GetRecipes(string itemToFind)
        {
            // Dictionary to contain all possible recipes for one item -> {recipeOBJ: [Item, ...], ...}
            Dictionary<Recipe, List<Item>> recipeItems = new Dictionary<Recipe, List<Item>>();

            // Loop through all recipes and check whether the item is the result of the recipe
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe r = Main.recipe[i];
                if (r.createItem.Name == itemToFind)
                {
                    List<Item> arr = (from item in r.requiredItem
                                      where !item.Name.Split(' ').Any(a => a == "Wall" || a == "Platform" || a == "Fence") && item.netID != 0
                                      select item).ToList();

                    if (arr.Count > 0)
                    {
                        recipeItems[r] = arr;
                    }
                }
            }
            return recipeItems;
        }

        public static Dictionary<Item, List<Item>> GetAllRecipes(Dictionary<Item, List<Item>> recipeDict, Item parent)
        {
            var r = GetRecipes(parent.Name);
            if (r.Count > 0)
            {
                var allRecipesList = new List<List<Item>>(r.Values);
                int randRecipePos = rng.Next(0, allRecipesList.Count);
                var randRecipe = allRecipesList[randRecipePos];

                // Sets the correct stack amount to the root iten
                if (recipeDict.Count == 0)
                {
                    var reversed = r.ToDictionary(x => x.Value, x => x.Key);
                    var correctItem = reversed[randRecipe].createItem;
                    parent.stack = correctItem.stack;
                }

                recipeDict[parent] = new List<Item>();

                foreach (Item i in randRecipe)
                {
                    recipeDict[parent].Add(i);
                }

                foreach (Item ingredient in recipeDict[parent])
                {
                    if (recipeDict.ContainsKey(ingredient))
                    {
                        recipeDict.Remove(ingredient);
                    }
                    else
                    {
                        GetAllRecipes(recipeDict, ingredient);
                    }
                }
            }

            // Write json file
            string json = "{";
            foreach (Item k in recipeDict.Keys)
            {
                json += $"\"{k.Name}\":[";
                foreach (Item v in recipeDict[k])
                {
                    json += $"\"{v.Name}\"";
                    if (v != recipeDict[k][recipeDict[k].Count - 1])
                    {
                        json += ", ";
                    }
                }
                json += "],";
            }
            json += "}";
            Debug.WriteLine(json);

            return recipeDict;
        }

    }
}
