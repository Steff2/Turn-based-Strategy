using UnityEngine;

public class BinaryTree
{
    private const bool right = true;
    private const bool left = false;
    public class TreeNode
    {
        public TreeNode parentNode;
        public TreeNode leftTreeNode;
        public TreeNode rightTreeNode;
        public PathNode pathNode;

        public TreeNode(PathNode pathNode, TreeNode leftNode, TreeNode rightNode)
        {
            this.pathNode = pathNode;
            leftTreeNode = leftNode;
            rightTreeNode = rightNode;
        }
    }

    private TreeNode root;
    public int nodeCount = 0;

    public BinaryTree()
    {
        root = null;
    }

    public TreeNode Search(TreeNode treeNode, PathNode pathNode)
    {
        if(treeNode == null)
        {
            return null;
        }
        else if(treeNode.pathNode == pathNode)
        {
            return treeNode;
        }
        
        if (treeNode.pathNode.fCost <= pathNode.fCost)
        {
            return Search(treeNode.rightTreeNode, pathNode);
        }
        else
        {
            return Search(treeNode.leftTreeNode, pathNode);
        }
    }
    public void AddNode(PathNode pathNode)
    {
        //debugCodePath += "AD; ";
        if (root == null)
        {
            root = new TreeNode(pathNode, null, null);
            nodeCount++;
        }
        else
        {
            // Has root
            try
            {
                Insert(root, pathNode, pathNode.fCost);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }
    }
    public void Insert(TreeNode treeNode, PathNode pathNode, int fCost)
    {
        if (treeNode.pathNode.fCost <= pathNode.fCost)
        {

            if (treeNode.rightTreeNode == null)
            {
                treeNode.rightTreeNode = new TreeNode(pathNode, null, null);
                treeNode.rightTreeNode.parentNode = treeNode;
                nodeCount++;
            }
            else
            {
                Insert(treeNode.rightTreeNode, pathNode, fCost);
            }

        }
        else if (treeNode.pathNode.fCost >= pathNode.fCost)
        {
            if (treeNode.leftTreeNode == null)
            {
                treeNode.leftTreeNode = new TreeNode(pathNode, null, null);
                treeNode.leftTreeNode.parentNode = treeNode;
                nodeCount++;
            }
            else
            {
                Insert(treeNode.leftTreeNode, pathNode, fCost);
            }
        }
    }
    public void RemoveNode(PathNode pathNode)
    {
        var nodeToBeRemoved = Search(root, pathNode);
        Remove(nodeToBeRemoved, nodeToBeRemoved.pathNode);
    }
    private void Remove(TreeNode treeNode, PathNode pathNode)
    {
        if (pathNode == null)
        {
            return;
        }

        var parentNode = treeNode.parentNode;
        // There are no leafs on either side
        if (treeNode.leftTreeNode == null && treeNode.rightTreeNode == null)
        {
            if (root == treeNode)
            {
                root = null;
                nodeCount--;
                return;
            }
            else
            {
                if (parentNode.leftTreeNode == treeNode)
                {
                    parentNode.leftTreeNode = null;
                    nodeCount--;
                    return;
                }
                else if (parentNode.rightTreeNode == treeNode)
                {
                    parentNode.rightTreeNode = null;
                    nodeCount--;
                    return;
                }
            }
        }
        // There are leafs on one side
        else if (treeNode.leftTreeNode == null || treeNode.rightTreeNode == null)
        {

            var newChild = treeNode.leftTreeNode ?? treeNode.rightTreeNode;

            if (root == treeNode)
            {
                root = newChild;
                root.parentNode = null;
                nodeCount--;
                return;
            }

            newChild.parentNode = parentNode;

            if (parentNode.leftTreeNode == treeNode)
            {
                parentNode.leftTreeNode = newChild;
                nodeCount--;
                return;
            }
            else if (parentNode.rightTreeNode == treeNode)
            {
                parentNode.rightTreeNode = newChild;
                nodeCount--;
                return;
            }
        }
        // There are leafs on both sides
        else
        {
            var rightChild = treeNode.rightTreeNode;
            var successor = GetLowestLeaf(rightChild, left);
            rightChild.parentNode = successor;
            parentNode.rightTreeNode = successor;
            nodeCount--;
            return;
        }
    }
    private TreeNode GetLowestLeaf(TreeNode node, bool direction)
    {
        if (node == null)
        { return null; }

        TreeNode treeDirection;
        // Which direction to traverse on the tree to look for leafs
        if (direction == right)
        {
            treeDirection = node.rightTreeNode;
        }
        else
        {
            treeDirection = node.leftTreeNode;
        }
        // Current node is leaf
        if (treeDirection == null)
        {
            return node;
        }
        // Recursively look for leaf node
        else
        {
            GetLowestLeaf(treeDirection, direction);
        }
        // Error
        return null;
    }
    public PathNode GetLowestValue()
    {
        if(root.leftTreeNode != null)
        {
            var tmp = root;
            TreeNode lowestValueNode = root;
            while (tmp != null)
            {
                lowestValueNode = tmp;
                tmp = tmp.leftTreeNode;
            }
            return lowestValueNode.pathNode;
        }
        else
        {
            return root.pathNode;
        }
    }
}
