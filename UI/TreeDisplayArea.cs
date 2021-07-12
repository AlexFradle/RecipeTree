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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using ReLogic.Graphics;
using ReLogic.Localization.IME;
using ReLogic.OS;
using ReLogic.Utilities;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Terraria.Achievements;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.Skies;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Map;
using Terraria.Net;
using Terraria.ObjectData;
using Terraria.Social;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace RecipeTree.UI
{
    class TreeDisplayArea : UIPanel
    {
        private List<ItemHolder> itemHolders = new List<ItemHolder>();
        public static List<BranchLine> branchLines;
        private UIImage branchImage;

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
                // b.Save(@"C:\Users\Alw\Documents\my games\Terraria\ModLoader\Mod Sources\RecipeTree\UI\bars.png", ImageFormat.Png);
                using (MemoryStream s = new MemoryStream())
                {
                    b.Save(s, ImageFormat.Png);
                    s.Seek(0, SeekOrigin.Begin);
                    texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, s);
                }
            }
            branchImage.SetImage(texture);
        }

        private void makeItemHolders(Node root)
        {
            ItemHolder itemHolder = new ItemHolder();
            itemHolder.SetPadding(0);
            itemHolder.Left.Set(root.x, 0f);
            itemHolder.Top.Set(root.y, 0f);
            itemHolder.Width.Set(TreeGenerator.nodeWidth, 0f);
            itemHolder.Height.Set(TreeGenerator.nodeHeight, 0f);
            itemHolder.BackgroundColor = new Microsoft.Xna.Framework.Color(10, 29, 94);
            itemHolder.SetImg(root.data);
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
                if (rect.Contains(Main.mouseX, Main.mouseY))
                {
                    Main.hoverItemName = ih.CurrentItem.Name;
                }
            }
        }
    }

    class Node
    {
        public Node parent;
        public Item data;
        public List<Node> children;
        public int level;
        public float x;
        public float y;

        public float CenterX => this.x + (TreeGenerator.nodeWidth / 2);
        public float CenterY => this.y + (TreeGenerator.nodeHeight / 2);

        public Node(Node parent, Item data)
        {
            this.parent = parent;
            this.data = data;
            this.children = new List<Node>();
            this.level = 0;
            this.x = 0;
            this.y = 0;
        }
    }

    class TreeGenerator
    {
        public const int nodeWidth = 50;
        public const int nodeHeight = 50;
        public const int branchThickness = 10;
        public const int margin = 10;
        public static Node treeRoot;
        public static int areaWidth;
        public static int areaHeight;

        public TreeGenerator(Item rootItem, Dictionary<Item, List<Item>> recipes)
        {
            Node root = generateTree(new Node(null, rootItem), recipes);
            root.level = 1;
            setLevels(root, 2);
            treeRoot = setCoords(root);
            areaHeight = (getMaxLevel(root) - 1) * (nodeHeight * 2) + nodeHeight;
            List<BranchLine> verticalLines = makeLines(root, new List<BranchLine>());
            TreeDisplayArea.branchLines.AddRange(verticalLines);
        }

        private Node generateTree(Node parent, Dictionary<Item, List<Item>> treeDict)
        {
            if (treeDict.ContainsKey(parent.data))
            {
                foreach (Item child in treeDict[parent.data])
                {
                    Node childNode = new Node(parent, child);
                    parent.children.Add(generateTree(childNode, treeDict));
                }
            }
            return parent;
        }

        private void setLevels(Node root, int level)
        {
            foreach (Node child in root.children)
            {
                child.level = level;
                setLevels(child, level + 1);
            }
        }

        private List<Node> getEndNodes(Node root, List<Node> endNodes)
        {
            foreach (Node child in root.children)
            {
                if (child.children.Count == 0)
                {
                    endNodes.Add(child);
                }
                getEndNodes(child, endNodes);
            }
            return endNodes;
        }

        private int getMaxLevel(Node root)
        {
            // BFS
            List<Node> seen = new List<Node>();
            seen.Add(root);
            List<Node> q = new List<Node>();
            q.Add(root);
            int max_lvl = 0;
            while(q.Count > 0)
            {
                Node v = q[0];
                q.RemoveAt(0);
                if (v.level > max_lvl)
                {
                    max_lvl = v.level;
                }
                foreach (Node e in v.children)
                {
                    if (!seen.Contains(e))
                    {
                        seen.Add(e);
                        q.Add(e);
                    }
                }
            }
            return max_lvl;
        }

        private List<BranchLine> makeLines(Node root, List<BranchLine> lines)
        {
            if (root.children.Count > 0)
            {
                lines.Add(new BranchLine(
                    (int)(root.CenterX - (branchThickness / 2)), 
                    (int)(root.y + nodeHeight), 
                    branchThickness, 
                    (int)(nodeHeight / 2)
                ));
            }
            foreach (Node child in root.children)
            {
                lines.Add(new BranchLine(
                    (int)(child.CenterX - (branchThickness / 2)),
                    (int)(child.CenterY - nodeHeight),
                    branchThickness,
                    (int)(nodeHeight / 2)
                ));
                makeLines(child, lines);
            }
            return lines;
        }

        private Node setCoords(Node root)
        {
            // List of nodes that have no children
            List<Node> endNodes = getEndNodes(root, new List<Node>());

            // Calculate node spaceing
            int numOfNodes = endNodes.Count;
            areaWidth = ((numOfNodes - 1) * margin) + (numOfNodes * nodeWidth);  // Uses min_area_width from the python prototype because the area should be as small as possible
            int areaRemainder = areaWidth - (nodeWidth * numOfNodes);
            float interNodeArea = numOfNodes == 1 ? 0 : areaRemainder / (numOfNodes - 1);

            // Position end nodes
            for (int pos = 0; pos < endNodes.Count; pos++)
            {
                endNodes[pos].x = (nodeWidth * pos) + (interNodeArea * pos);
                endNodes[pos].y = (endNodes[pos].level - 1) * (nodeHeight * 2);
            }

            List<Node> curNodes = new List<Node>(endNodes);

            List<BranchLine> horizontalLines = new List<BranchLine>();

            while (curNodes.Count > 0)
            {
                List<Node> rmList = new List<Node>();
                List<Node> apList = new List<Node>();

                foreach (Node cn in curNodes)
                {
                    if (cn.parent != null)
                    {
                        HashSet<Node> nodeSet = new HashSet<Node>(cn.parent.children);
                        if (nodeSet.IsSubsetOf(curNodes) && (!rmList.Contains(cn)))
                        {
                            Node firstChild = cn.parent.children[0];
                            Node endChild = cn.parent.children[cn.parent.children.Count - 1];
                            cn.parent.x = firstChild.x + ((endChild.x - firstChild.x) / 2);
                            cn.parent.y = cn.y - (nodeHeight * 2);

                            BranchLine hl = new BranchLine(
                                (int)(firstChild.CenterX - (branchThickness / 2)),
                                (int)(firstChild.CenterY - nodeHeight),
                                (int)((endChild.CenterX - firstChild.CenterX) + (branchThickness / 2)),
                                branchThickness
                            );

                            horizontalLines.Add(hl);

                            foreach (Node n in cn.parent.children)
                            {
                                rmList.Add(n);
                            }

                            apList.Add(cn.parent);
                        }
                    }
                    else
                    {
                        cn.x = cn.children[0].x + ((cn.children[cn.children.Count - 1].x - cn.children[0].x)) / 2;
                        rmList.Add(cn);
                    }
                }
                foreach (Node rn in rmList)
                {
                    curNodes.Remove(rn);
                }
                foreach (Node an in apList)
                {
                    curNodes.Add(an);
                }
            }

            TreeDisplayArea.branchLines = horizontalLines;
            return root;
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
