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
        private UIImage itemImg = new UIImage(ModContent.GetTexture("Terraria/UI/ButtonDelete"));
        private Item _currentItem;
        public Item CurrentItem => _currentItem;

        public void SetImg(Item newItem)
        {
            _currentItem = newItem;
            itemImg.SetImage(Main.itemTexture[newItem.netID]);
        }

        public ItemHolder()
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
                Main.hoverItemName = _currentItem.Name;
            }
        }
    }
}
