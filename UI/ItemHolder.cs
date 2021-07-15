using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using RecipeTree.Processes;
using RecipeTree.Commands;

namespace RecipeTree.UI
{
    class ItemHolder : UIPanel
    {
        private UIImage itemImg = new UIImage(ModContent.GetTexture("Terraria/UI/ButtonSeed"));
        private Item _currentItem;
        private bool isPartOfTree;
        private bool isRootItem;
        private Color recipeCompleteEndColour = new Color(77, 46, 0);
        private Color recipeCompleteStartColour = new Color(255, 153, 0);
        private int colourFrameCount = 0;
        public float itemStack = 1f;
        public Item CurrentItem => _currentItem;

        public void SetImg(Item newItem)
        {
            _currentItem = newItem;
            itemImg.SetImage(Main.itemTexture[newItem.netID]);
        }

        public ItemHolder(bool isPartOfTree = false, bool isRootItem = false)
        {
            this.isPartOfTree = isPartOfTree;
            this.isRootItem = isRootItem;
            itemImg.VAlign = 0.5f;
            itemImg.HAlign = 0.5f;
            Append(itemImg);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (_currentItem != null)
            {
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
