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
        private static Item currentItem;

        public static void SetImg(Item newItem)
        {
            currentItem = newItem;
            itemImg.SetImage(Main.itemTexture[newItem.netID]);
        }

        public override void OnInitialize()
        {
            itemImg = new UIImage(ModContent.GetTexture("Terraria/UI/ButtonFavoriteActive"));
            itemImg.VAlign = 0.5f;
            itemImg.HAlign = 0.5f;
            Append(itemImg);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (IsMouseHovering)
            {
                Main.hoverItemName = currentItem.Name;
            }
        }
    }
}
