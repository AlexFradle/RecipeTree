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

namespace RecipeTree.UI
{
    class TreeWindow : UIState
    {
        public BaseUIPanel TreePanel;
        public static bool Visible;

        public override void OnInitialize()
        {
            TreePanel = new BaseUIPanel();
            TreePanel.SetPadding(0);

            float TPLeft = 400;
            float TPTop = 100;
            float TPWidth = 170;
            float TPHeight = 70;

            TreePanel.Left.Set(TPLeft, 0f);
            TreePanel.Top.Set(TPTop, 0f);
            TreePanel.Width.Set(TPWidth, 0f);
            TreePanel.Height.Set(TPHeight, 0f);
            TreePanel.BackgroundColor = new Color(73, 94, 171);

            ItemHolder ItemPanel = new ItemHolder();
            ItemPanel.SetPadding(0);

            ItemPanel.Left.Set(10f, 0f);
            ItemPanel.Top.Set(10f, 0f);
            ItemPanel.Width.Set(40f, 0f);
            ItemPanel.Height.Set(40f, 0f);
            ItemPanel.BackgroundColor = new Color(10, 29, 94);
            TreePanel.Append(ItemPanel);


            // image is 22s22
            Texture2D closeButtonTexture = ModContent.GetTexture("Terraria/UI/ButtonDelete");
            HoverImageButton closeButton = new HoverImageButton(closeButtonTexture, Language.GetTextValue("LegacyInterface.52"));
            closeButton.Left.Set(TPWidth - 22 - 10, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            TreePanel.Append(closeButton);

            Append(TreePanel);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
    }
}
