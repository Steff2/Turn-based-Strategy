
using System;

public class BinaryTree : IDisposable
{
    public TreeNode Search(TreeNode root, int value)
    {
        if (root == null || root.value == value)
        {
            return root;
        }
        
        if (root.value < value)
        {
            return Search(root.right, value);
        }

        return Search(root.left, value);
    }

    public TreeNode Insert(TreeNode root, int value)
    {
        if (root == null)
        {
            root = new TreeNode(value);
            return root;
        }

        if (root.value < value)
        {
            return Search(root.right, value);
        }
        else if(root.value > value)
        {
            return Search(root.left, value);
        }

        return root;
    }

    public TreeNode Delete(TreeNode root, int value)
    {
        if (root == null)
        {
            return null;
        }
        return root;
    }

    public void Dispose() { }
}
