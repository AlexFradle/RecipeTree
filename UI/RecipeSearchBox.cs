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
using Terraria.Localization;
using RecipeTree.Commands;
using RecipeTree.Processes;

namespace RecipeTree.UI
{
    class RecipeSearchBox : UITextBox
    {
        public static bool focused;
        private static string currentString;
        private static Color originalColour;
        public RecipeSearchBox(string text) : base(text)
        {
            originalColour = this.BackgroundColor;
            this.SetTextMaxLength(40);
        }

        public override void Click(UIMouseEvent evt)
        {
            // https://github.com/JavidPack/RecipeBrowser/blob/master/UIElements/NewUITextBox.cs
            Focus();
            base.Click(evt);
        }

        public void Focus()
        {
            if (!focused)
            {
                Main.clrInput();
                focused = true;
                TreeWindow.DropDownList.visible = true;
                Main.blockInput = true;
                this.BackgroundColor = new Color(46, 60, 107);
            }
        }

        public void Unfocus()
        {
            if (focused)
            {
                focused = false;
                TreeWindow.DropDownList.visible = false;
                Main.blockInput = false;
                this.BackgroundColor = originalColour;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!ContainsPoint(new Vector2((float)Main.mouseX, (float)Main.mouseY)) && (Main.mouseLeft || Main.mouseRight))
            {
                Unfocus();
            }
            base.Update(gameTime);
        }

        private static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(currentString);
                if (newString != currentString && currentString != null)
                {
                    TreeWindow.DropDownList.Clear();
                    List<string> suggestedWords = ItemChecker.GetClosestMatches(currentString);
                    foreach (string word in suggestedWords)
                    {
                        UITextPanel<string> button = new UITextPanel<string>(word);
                        button.OnClick += (a, b) => {
                            currentString = word;
                            RecipeCommand.setRecipeWindow(word);
                        };
                        TreeWindow.DropDownList.Add(button);
                    }
                }
                currentString = newString;
                this.SetText(currentString, 1f, false);
                
                if (JustPressed(Keys.Enter))
                {
                    Main.drawingPlayerChat = false;
                    Unfocus();
                    RecipeCommand.setRecipeWindow(currentString);
                }
            }
        }
    }
}
