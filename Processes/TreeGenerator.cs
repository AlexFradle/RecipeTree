using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RecipeTree.UI;

namespace RecipeTree.Processes
{
    class Node
    {
        public Node parent;
        public Item data;
        public List<Node> children;
        public int level;
        public float x;
        public float y;
        public int itemStack;

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
            this.itemStack = data.stack;
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
            sumStackAmounts(root, recipes);
            areaHeight = (getMaxLevel(root) - 1) * (nodeHeight * 2) + nodeHeight;
            List<BranchLine> verticalLines = makeLines(root, new List<BranchLine>());
            TreeDisplayArea.branchLines.AddRange(verticalLines);
            if (!TreeWindow.TreeArea.topDown)
            {
                applyRefelectionNode(root);
            }
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
            while (q.Count > 0)
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

        public static void sumStackAmounts(Node root, Dictionary<Item, List<Item>> recipeDict)
        {
            // FInd which recipe obj has the same children as the root
            // Then know the roots recipe item stack using recipe.createItem.stack
            // Then do calculation
            if (root.children.Count > 0)
            {
                var r = RecipeSearcher.GetRecipes(root.data.Name);
                var rootRecipe = (from kv in r 
                                  where kv.Value.SequenceEqual(new List<Item>(from i in root.children select i.data))
                                  select kv.Key).ToList()[0].createItem;
                foreach (Node child in root.children)
                {
                    child.itemStack = (int)(((float)child.data.stack / (float)rootRecipe.stack) * root.itemStack);
                    sumStackAmounts(child, recipeDict);
                }
            }
        }

        public static void applyRefelectionNode(Node root)
        {
            root.y = (-root.y) + areaHeight - nodeHeight;
            foreach (Node child in root.children)
            {
                applyRefelectionNode(child);
            }
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
}
