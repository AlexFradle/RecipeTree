using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using RecipeTree.UI;

namespace RecipeTree
{
	public class RecipeTree : Mod
	{
		private UserInterface TreeUserInterface;
		internal TreeWindow TreeWindow;
        public static ModHotKey ToggleRecipeTreeHotKey;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TreeWindow = new TreeWindow();
                TreeWindow.Activate();
                TreeUserInterface = new UserInterface();
                TreeUserInterface.SetState(TreeWindow);

                ToggleRecipeTreeHotKey = RegisterHotKey("Toggle Recipe Tree Window", "P");
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (ToggleRecipeTreeHotKey.JustPressed)
            {
                TreeWindow.Visible = !TreeWindow.Visible;
            }

            if (TreeWindow.Visible)
            {
                TreeUserInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // No fucking clue what this does
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "RecipeTree: Recipe Tree",
                    delegate {
                        if (TreeWindow.Visible)
                        {
                            TreeUserInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}