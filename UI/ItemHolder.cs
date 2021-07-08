using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace RecipeTree.UI
{
    class ItemHolder : UIPanel
    {
        private static UIImage itemImg;

        public static void SetImg(int newItemID)
        {
            itemImg.SetImage(Main.itemTexture[newItemID]);
        }

        public override void OnInitialize()
        {
            itemImg = new UIImage(ModContent.GetTexture("Terraria/UI/ButtonFavoriteActive"));
            itemImg.VAlign = 0.5f;
            itemImg.HAlign = 0.5f;
            Append(itemImg);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
