using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.GameInput;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using RecipeTree.Processes;
using RecipeTree.Commands;

namespace RecipeTree.UI
{
    class SearchDropDown : UIList
    {
        public bool visible;
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                base.Draw(spriteBatch);
        }
    }
}
