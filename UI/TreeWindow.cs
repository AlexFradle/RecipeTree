﻿using System;
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

namespace RecipeTree.UI
{
    class TreeWindow : UIState
    {
        public static BaseUIPanel TreePanel;
        public static bool Visible;
        public static ItemHolder ItemPanel;
        public static TreeDisplayArea TreeArea;
        public static HoverImageButton CloseButton;

        public override void OnInitialize()
        {
            TreePanel = new BaseUIPanel();
            TreePanel.SetPadding(0);

            float TPLeft = 400;
            float TPTop = 100;
            float TPWidth = 500;
            float TPHeight = 500;

            TreePanel.Left.Set(TPLeft, 0f);
            TreePanel.Top.Set(TPTop, 0f);
            TreePanel.Width.Set(TPWidth, 0f);
            TreePanel.Height.Set(TPHeight, 0f);
            TreePanel.BackgroundColor = new Color(73, 94, 171);

            ItemPanel = new ItemHolder();
            ItemPanel.SetPadding(0);

            ItemPanel.Left.Set(10f, 0f);
            ItemPanel.Top.Set(10f, 0f);
            ItemPanel.Width.Set(50f, 0f);
            ItemPanel.Height.Set(50f, 0f);
            ItemPanel.BackgroundColor = new Color(46, 60, 107);
            TreePanel.Append(ItemPanel);


            // image is 22s22
            Texture2D closeButtonTexture = ModContent.GetTexture("Terraria/UI/ButtonDelete");
            CloseButton = new HoverImageButton(closeButtonTexture, Language.GetTextValue("LegacyInterface.52"));
            CloseButton.Left.Set(TPWidth - 22 - 10, 0f);
            CloseButton.Top.Set(10, 0f);
            CloseButton.Width.Set(22, 0f);
            CloseButton.Height.Set(22, 0f);
            CloseButton.OnClick += new MouseEvent(CloseButtonClicked);
            TreePanel.Append(CloseButton);

            TreeArea = new TreeDisplayArea();
            TreeArea.SetPadding(0);
            TreeArea.Left.Set(10f, 0f);
            TreeArea.Top.Set(70f, 0f);
            TreeArea.BackgroundColor = new Color(46, 60, 107);
            TreePanel.Append(TreeArea);

            Append(TreePanel);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}
