using UnityEngine;

public class BinaryTree
{

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
        
        if (root.pathNode.fCost < pathNode.fCost)
        {
            return Search(root.rightTreeNode, pathNode);
        }
        else
        {
            return Search(root.leftTreeNode, pathNode);
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

        if(treeNode.leftTreeNode == null && treeNode.rightTreeNode == null)
        {
            if (root.pathNode == pathNode)
            {
                root = null;
            }
        }
        else
        {
            var parentRoot = treeNode.parentNode;

            if (treeNode.leftTreeNode == null || treeNode.rightTreeNode == null)
            {
                var newChild = treeNode.leftTreeNode ?? treeNode.rightTreeNode;
                newChild.parentNode = parentRoot;

                if(newChild.leftTreeNode == treeNode)
                {
                    parentRoot.leftTreeNode = newChild;
                }
                else if (newChild.leftTreeNode == treeNode)
                {
                    parentRoot.rightTreeNode = newChild;
                }

                if(treeNode == root)
                {
                    root = newChild;
                    root.parentNode = null;
                }
            }
            else
            {
                var newChild = GetRightMostLeaf(treeNode);
                parentRoot.leftTreeNode = newChild;
                newChild.leftTreeNode = treeNode.leftTreeNode;
            }
        }
        nodeCount--;
    }
    private TreeNode GetRightMostLeaf(TreeNode node)
    {
        var testNode = node;
        var targetNode = node;
        while (testNode != null) 
        {
            targetNode = testNode;
            testNode = testNode.rightTreeNode;
        }
        return targetNode;
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
