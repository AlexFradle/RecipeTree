using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace RecipeTree.UI
{
    class HoverImageButton : UIImageButton
    {
        internal string HoverText;

        public HoverImageButton(Texture2D texture, string hoverText) : base(texture)
        {
            HoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.hoverItemName = HoverText;
            }
        }
    }
}
