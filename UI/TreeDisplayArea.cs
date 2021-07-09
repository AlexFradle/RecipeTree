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
    class TreeDisplayArea : UIPanel
    {
        private List<ItemHolder> itemHolders;
        private List<BranchLine> branchLines;
    }

    class BranchLine : UIPanel
    {
        public override void OnInitialize()
        {
            this.BackgroundColor = new Color(10, 29, 94);
        }
    }
}
