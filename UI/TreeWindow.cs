using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace RecipeTree.UI
{
    class TreeWindow : UIState
    {
        public BaseUIPanel TreePanel;
        public static bool Visible;

        public override void OnInitialize()
        {
            TreePanel = new BaseUIPanel();
            TreePanel.SetPadding(0);

            TreePanel.Left.Set(400f, 0f);
            TreePanel.Top.Set(100f, 0f);
            TreePanel.Width.Set(170f, 0f);
            TreePanel.Height.Set(70f, 0f);
            TreePanel.BackgroundColor = new Color(73, 94, 171);

            Append(TreePanel);
        }
    }
}
