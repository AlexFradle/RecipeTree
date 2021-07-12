using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.Localization;
using RecipeTree.Commands;

namespace RecipeTree.UI
{
    class RecipeSearchBox : UITextBox
    {
        public RecipeSearchBox(string text) : base(text)
        {
            
        }

        public override void Click(UIMouseEvent evt)
        {
            // https://github.com/JavidPack/RecipeBrowser/blob/master/UIElements/NewUITextBox.cs
            base.Click(evt);
        }
    }
}
