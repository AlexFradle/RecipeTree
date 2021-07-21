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
    class TreeDisplayArea : UIPanel
    {
        private List<ItemHolder> itemHolders = new List<ItemHolder>();
        public static List<BranchLine> branchLines;
        private UIImage branchImage;
        public bool topDown = true;

        public void makeTree()
        {
            this.RemoveAllChildren();
            itemHolders = new List<ItemHolder>();
            branchImage = new UIImage(ModContent.GetTexture("RecipeTree/UI/bars"));
            branchImage.Left.Set(0f, 0f);
            branchImage.Top.Set(0f, 0f);
            this.Append(branchImage);
            makeBranchImage();
            makeItemHolders(TreeGenerator.treeRoot);
        }

        private void makeBranchImage()
        {
            Texture2D texture = null;
            using (Bitmap b = new Bitmap(TreeGenerator.areaWidth, TreeGenerator.areaHeight))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    SolidBrush p = new SolidBrush(System.Drawing.Color.FromArgb(10, 29, 94));
                    foreach (BranchLine br in branchLines)
                    {
                        System.Drawing.Rectangle r = new System.Drawing.Rectangle(br.x, br.y, br.w, br.h);
                        g.FillRectangle(p, r);
                    }
                }
                if (!topDown)
                {
                    b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                // b.Save(@"C:\Users\Alw\Documents\my games\Terraria\ModLoader\Mod Sources\RecipeTree\UI\bars.png", ImageFormat.Png);
                using (MemoryStream s = new MemoryStream())
                {
                    b.Save(s, ImageFormat.Png);
                    s.Seek(0, SeekOrigin.Begin);
                    texture = Texture2D.FromStream(Main.instance.GraphicsDevice, s);
                }
            }
            branchImage.SetImage(texture);
        }

        private void makeItemHolders(Node root)
        {
            ItemHolder itemHolder = new ItemHolder(root.x, root.y, true, root.parent != null ? false : true);
            itemHolder.SetPadding(0);
            itemHolder.Left.Set(root.x, 0f);
            itemHolder.Top.Set(root.y, 0f);
            itemHolder.Width.Set(TreeGenerator.nodeWidth, 0f);
            itemHolder.Height.Set(TreeGenerator.nodeHeight, 0f);
            itemHolder.BackgroundColor = new Microsoft.Xna.Framework.Color(10, 29, 94);
            // Clone node item and assign itemStack to its stack field
            Item itemHolderItem = root.data.Clone();
            itemHolderItem.stack = (int)Math.Ceiling(root.itemStack);
            itemHolder.itemStack = root.itemStack;
            itemHolder.SetItem(itemHolderItem);
            itemHolders.Add(itemHolder);
            this.Append(itemHolder);

            foreach (Node child in root.children)
            {
                makeItemHolders(child);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (ItemHolder ih in itemHolders)
            {
                var rect = ih.GetDimensions().ToRectangle();
                if (rect.Contains(Main.mouseX, Main.mouseY) && Main.mouseX > TreeWindow.TreePanel.Left.Pixels && Main.mouseX < TreeWindow.TreePanel.Left.Pixels + TreeWindow.TreePanel.Width.Pixels)
                {
                    Main.hoverItemName = ih.CurrentItem.Name;
                    Main.HoverItem = ih.CurrentItem.Clone();
                    //Main.hoverItemName = Main.HoverItem.Name;
                    if (Main.mouseLeft)
                    {
                        RecipeCommand.setRecipeWindow(ih.CurrentItem.Name);
                        Main.mouseLeft = false;
                    }
                }
            }
        }
    }

    class BranchLine
    {
        public int x;
        public int y;
        public int w;
        public int h;

        public BranchLine(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
}
