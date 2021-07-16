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
    class TreeContainerList : UIList
    {
        private Vector2 offset;
        public bool dragging;

        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.RightMouseDown(evt);
            DragStart(evt);
        }

        public override void RightMouseUp(UIMouseEvent evt)
        {
            base.RightMouseUp(evt);
            DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            offset = new Vector2(evt.MousePosition.X - TreeWindow.TreeArea.Left.Pixels, evt.MousePosition.Y - TreeWindow.TreeArea.Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 end = evt.MousePosition;
            dragging = false;

            TreeWindow.TreeArea.Left.Set(MathHelper.Clamp(end.X - offset.X, -TreeGenerator.areaWidth + TreeWindow.TreePanel.Width.Pixels - 10f, 10f), 0f);
            TreeWindow.TreeArea.Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                TreeWindow.TreeArea.Left.Set(Main.mouseX - offset.X, 0f);
                TreeWindow.TreeArea.Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }
        }

    }
}
