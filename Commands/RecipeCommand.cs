using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using RecipeTree.Processes;
using RecipeTree.UI;

namespace RecipeTree.Commands
{
    public class RecipeCommand : ModCommand
    {
        // 1 = both icons and text, 2 = just text, 3 = just icons
        public int setting = 1;
        public override CommandType Type => CommandType.Chat;

        public override string Command => "recipe";

        public override string Usage => "/recipe <output type> <item name|text output format>" + "\nOutputs all items needed to make the specified item";

        public override string Description => "Create recipe tree from specified item";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // caller = the player the ran the command
            // input = the comamnd itself, e.g "/recipe"
            // args = an array of all strings entered after the input seperated by spaces

            // Equivalent to item_name = args[1:];
            string itemName = String.Join(" ", args.Skip(1));

            if (args[0] == "text")
            {
                // This isn't a great solution because no item name can start with the number 1, 2 or 3
                if (args[1] != "1" && args[1] != "2" && args[1] != "3")
                {
                    List<(string, bool)> tree = TreeBuilder.MakeTextTree(itemName, caller.Player, setting);
                    caller.Reply("Recipes:", Color.White);
                    foreach (var pair in tree)
                    {
                        caller.Reply("|> " + pair.Item1, pair.Item2 ? Color.Green : Color.Red);
                    }
                }
                else
                {
                    setting = Int32.Parse(args[1]);
                    caller.Reply($"Recipe text changed to setting {setting}", Color.Blue);
                }
            }
            else if (args[0] == "window")
            {
                setRecipeWindow(itemName);

            }
            else if(args[0] == "flip")
            {
                TreeWindow.TreeArea.topDown = !TreeWindow.TreeArea.topDown;
            }
            else
            {
                var rd = new Dictionary<Item, List<Item>>();
                Item item = ItemChecker.GetItemID(itemName);
                TreeBuilder.GetAllRecipes(rd, item);
            }
        }

        public static void setRecipeWindow(string itemName)
        {
            TreeWindow.ItemPanel.SetImg(ItemChecker.GetItemID(itemName));
            Item item = ItemChecker.GetItemID(itemName);
            var recipeDict = TreeBuilder.GetAllRecipes(new Dictionary<Item, List<Item>>(), item);
            var tree = new TreeGenerator(item, recipeDict);
            TreeWindow.TreePanel.Width.Set(TreeGenerator.areaWidth + 20f, 0f);
            TreeWindow.TreePanel.Height.Set(TreeGenerator.areaHeight + 80f, 0f);
            TreeWindow.TreeArea.Width.Set(TreeGenerator.areaWidth, 0f);
            TreeWindow.TreeArea.Height.Set(TreeGenerator.areaHeight, 0f);
            TreeWindow.CloseButton.Left.Set(TreeGenerator.areaWidth - 10, 0f);
            TreeWindow.FlipButton.Left.Set(TreeGenerator.areaWidth - 22 - 20, 0f);
            TreeWindow.TreeArea.makeTree();
            TreeWindow.Visible = true;
        }
    }
}
