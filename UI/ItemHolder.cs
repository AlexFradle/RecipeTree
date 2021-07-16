using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using RecipeTree.Processes;
using RecipeTree.Commands;
using Color = Microsoft.Xna.Framework.Color;

namespace RecipeTree.UI
{
    class ItemHolder : UIPanel
    {
        private float x;
        private float y;
        private Item _currentItem;
        private bool isPartOfTree;
        private bool isRootItem;
        private Color recipeCompleteEndColour = new Color(77, 46, 0);
        private Color recipeCompleteStartColour = new Color(255, 153, 0);
        private int colourFrameCount = 0;
        public float itemStack = 1f;
        public Item CurrentItem => _currentItem;

        public void SetItem(Item newItem)
        {
            _currentItem = newItem;
        }

        public ItemHolder(float x, float y, bool isPartOfTree = false, bool isRootItem = false)
        {
            this.x = x;
            this.y = y;
            this.isPartOfTree = isPartOfTree;
            this.isRootItem = isRootItem;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (_currentItem != null)
            {
                // How the fuck does this section work???????????????????????????????????????????????????????????????
                Texture2D fullTexture = Main.itemTexture[_currentItem.type];
                Microsoft.Xna.Framework.Rectangle rectangle;

                if (Main.itemAnimations[_currentItem.type] != null)
                {
                    rectangle = Main.itemAnimations[_currentItem.type].GetFrame(fullTexture);
                }
                else
                {
                    rectangle = fullTexture.Frame(1, 1, 0, 0);
                }
                //???????????????????????????????????????????????????????????????????????????????????????????????????

                Vector2 drawPos = new Vector2(
                    TreeWindow.TreePanel.Left.Pixels + (isPartOfTree ? TreeWindow.TreeArea.Left.Pixels : 0) + x, 
                    TreeWindow.TreePanel.Top.Pixels + (isPartOfTree ? TreeWindow.TreeList.Top.Pixels : 0) + y
                );

                float sf = 1f;
                if (rectangle.Width > TreeGenerator.nodeWidth || rectangle.Height > TreeGenerator.nodeHeight)
                {
                    if (rectangle.Width > rectangle.Height)
                    {
                        sf = (float)TreeGenerator.nodeWidth / (float)rectangle.Width;
                    }
                    else
                    {
                        sf = (float)TreeGenerator.nodeHeight / (float)rectangle.Height;
                    }
                }

                float scaledWidth = rectangle.Width * sf;
                float scaledHeight = rectangle.Height * sf;

                drawPos.X += scaledWidth <= TreeGenerator.nodeWidth ? (TreeGenerator.nodeWidth / 2) - (scaledWidth / 2) : 0;
                drawPos.Y += scaledHeight <= TreeGenerator.nodeHeight ? (TreeGenerator.nodeHeight / 2) - (scaledHeight / 2) : 0;

                spriteBatch.Draw(fullTexture, drawPos, rectangle, _currentItem.GetAlpha(Color.White), 0f, Vector2.Zero, sf, SpriteEffects.None, 0f);

                if (IsMouseHovering)
                {
                    Main.hoverItemName = _currentItem.Name;
                }


                if (_currentItem != null && isPartOfTree)
                {
                    if (ItemChecker.CheckInventory(Main.LocalPlayer, _currentItem.Name, _currentItem.stack))
                    {
                        this.BackgroundColor = new Color(10, 92, 10);
                    }
                    else
                    {
                        this.BackgroundColor = new Color(92, 10, 10);
                    }
                }

                if (isRootItem)
                {
                    bool craftable = true;
                    foreach (Node n in TreeGenerator.treeRoot.children)
                    {
                        if (!ItemChecker.CheckInventory(Main.LocalPlayer, n.data.Name, n.data.stack))
                        {
                            craftable = false;
                            colourFrameCount = 0;
                            break;
                        }
                    }
                    if (craftable)
                    {
                        colourFrameCount = colourFrameCount > 60 ? 0 : colourFrameCount;
                        float t = (float)colourFrameCount / 60;
                        this.BackgroundColor = Color.Lerp(recipeCompleteStartColour, recipeCompleteEndColour, t);
                        colourFrameCount += 1;
                    }
                }

                if (itemStack > 1)
                {
                    CalculatedStyle innerDimensions = GetInnerDimensions();
                    float offsetX = innerDimensions.X;
                    float offsetY = innerDimensions.Y;
                    Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, itemStack.ToString(), offsetX, offsetY + 40f, Color.White, Color.Black, new Vector2(0.3f));
                }
            }
        }
    }
}
