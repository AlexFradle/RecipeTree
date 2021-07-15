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
using RecipeTree.Processes;

namespace RecipeTree.UI
{
    class TreeWindow : UIState
    {
        public static BaseUIPanel TreePanel;
        public static bool Visible;
        public static ItemHolder ItemPanel;
        public static TreeDisplayArea TreeArea;
        public static HoverImageButton CloseButton;
        public static HoverImageButton FlipButton;
        public static HoverImageButton FilterUpButton;
        public static HoverImageButton FilterDownButton;
        public static RecipeSearchBox SearchBox;
        public static int TreeDepth = 2;
        public static int MaxDepth = 8;

        public override void OnInitialize()
        {
            TreePanel = new BaseUIPanel();
            TreePanel.SetPadding(0);

            float TPLeft = 400;
            float TPTop = 100;
            float TPWidth = 366;
            float TPHeight = 100;

            TreePanel.Left.Set(TPLeft, 0f);
            TreePanel.Top.Set(TPTop, 0f);
            TreePanel.Width.Set(TPWidth, 0f);
            TreePanel.Height.Set(TPHeight, 0f);
            TreePanel.BackgroundColor = new Color(73, 94, 171);

            ItemPanel = new ItemHolder(10f, 10f);
            ItemPanel.SetPadding(0);

            ItemPanel.Left.Set(10f, 0f);
            ItemPanel.Top.Set(10f, 0f);
            ItemPanel.Width.Set(50f, 0f);
            ItemPanel.Height.Set(50f, 0f);
            ItemPanel.BackgroundColor = new Color(46, 60, 107);
            TreePanel.Append(ItemPanel);


            // image is 22s22
            // Texture2D closeButtonTexture = ModContent.GetTexture("Terraria/UI/ButtonDelete");
            Texture2D closeButtonTexture = ModContent.GetTexture("RecipeTree/UI/exit");
            CloseButton = new HoverImageButton(closeButtonTexture, Language.GetTextValue("LegacyInterface.52"));
            CloseButton.Left.Set(TPWidth - 22 - 10, 0f);
            CloseButton.Top.Set(10, 0f);
            CloseButton.Width.Set(22, 0f);
            CloseButton.Height.Set(22, 0f);
            CloseButton.OnClick += new MouseEvent(CloseButtonClicked);
            TreePanel.Append(CloseButton);

            // Texture2D flipButtonTexture = ModContent.GetTexture("Terraria/UI/ButtonSeed");
            Texture2D flipButtonTexture = ModContent.GetTexture("RecipeTree/UI/flip");
            FlipButton = new HoverImageButton(flipButtonTexture, "Flip Tree");
            FlipButton.Left.Set(TPWidth - 44 - 20, 0f);
            FlipButton.Top.Set(10, 0f);
            FlipButton.Width.Set(22, 0f);
            FlipButton.Height.Set(22, 0f);
            FlipButton.OnClick += new MouseEvent(FlipButtonClicked);
            TreePanel.Append(FlipButton);

            Texture2D upButtonTexture = ModContent.GetTexture("RecipeTree/UI/up_arrow");
            FilterUpButton = new HoverImageButton(upButtonTexture, "Increase Tree Depth: 2");
            FilterUpButton.Left.Set(TPWidth - 66 - 30, 0f);
            FilterUpButton.Top.Set(10, 0f);
            FilterUpButton.Width.Set(22, 0f);
            FilterUpButton.Height.Set(22, 0f);
            FilterUpButton.OnClick += new MouseEvent(UpButtonClicked);
            TreePanel.Append(FilterUpButton);

            Texture2D downButtonTexture = ModContent.GetTexture("RecipeTree/UI/down_arrow");
            FilterDownButton = new HoverImageButton(downButtonTexture, "Decrease Tree Depth: 2");
            FilterDownButton.Left.Set(TPWidth - 66 - 30, 0f);
            FilterDownButton.Top.Set(38, 0f);
            FilterDownButton.Width.Set(22, 0f);
            FilterDownButton.Height.Set(22, 0f);
            FilterDownButton.OnClick += new MouseEvent(DownButtonClicked);
            TreePanel.Append(FilterDownButton);

            SearchBox = new RecipeSearchBox("");
            SearchBox.Left.Set(70f, 0f);
            SearchBox.Top.Set(10f, 0f);
            SearchBox.Width.Set(200f, 0f);
            SearchBox.Height.Set(50f, 0f);
            TreePanel.Append(SearchBox);

            TreeArea = new TreeDisplayArea();
            TreeArea.SetPadding(0);
            TreeArea.Left.Set(10f, 0f);
            TreeArea.Top.Set(70f, 0f);
            TreeArea.BackgroundColor = new Color(46, 60, 107);
            TreePanel.Append(TreeArea);

            Append(TreePanel);
        }

        private void RefreshRecipeWindow()
        {
            // Remove non alaphanumeric characters
            char[] arr = TreeGenerator.treeRoot.data.Name.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray();
            string itemName = new string(arr);

            RecipeCommand.setRecipeWindow(itemName);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }

        private void FlipButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            TreeArea.topDown = !TreeArea.topDown;
            RefreshRecipeWindow();
        }

        private void UpButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            //if (TreeDepth + 1 )
            //{

            //}
            TreeDepth += 1;
            RefreshRecipeWindow();
        }

        private void DownButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (TreeDepth - 1 >= 2)
            {
                TreeDepth -= 1;
                RefreshRecipeWindow();
            }
        }
    }
}
