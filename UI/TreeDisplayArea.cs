using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System.Drawing;
using System.Drawing.Imaging;

namespace RecipeTree.UI
{
    class TreeDisplayArea : UIPanel
    {
        private List<ItemHolder> itemHolders = new List<ItemHolder>();
        public static List<BranchLine> branchLines;

        public void makeTree()
        {
            this.RemoveAllChildren();
            makeItemHolders(TreeGenerator.treeRoot);
            makeBranchImage();
        }

        private void makeBranchImage()
        {
            using (Bitmap b = new Bitmap(TreeGenerator.areaWidth, TreeGenerator.areaHeight))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    Pen p = new Pen(System.Drawing.Color.FromArgb(255, 0, 0));
                    foreach (BranchLine br in branchLines)
                    {
                        System.Drawing.Rectangle r = new System.Drawing.Rectangle(br.x, br.y, br.w, br.h);
                        g.DrawRectangle(p, r);
                    }
                }
                b.Save(@"C:\Users\Alw\Documents\my games\Terraria\ModLoader\Mod Sources\RecipeTree\UI\bars.png", ImageFormat.Png);
            }
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
                            Node endChild = cn.parent.children[cn.parent.children.Count - 1];
                            cn.parent.x = cn.parent.children[0].x + ((endChild.x - cn.parent.children[0].x) / 2);
                            cn.parent.y = cn.y - (nodeHeight * 2);

                            BranchLine hl = new BranchLine();
                            hl.x = (int)cn.parent.children[0].CenterX;
                            hl.y = (int)cn.parent.children[0].CenterY - nodeHeight;
                            hl.w = (int)endChild.CenterX;
                            hl.h = branchThickness;

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
    }
}
