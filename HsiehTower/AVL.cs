using System;

public class AVL<TKey, TValue> where TKey : IComparable  // whatever data type is substituted for TKey can handle comparison operations
{
    internal class Node
    {
        public TKey Key;
        public TValue Value;
        public Node Left;
        public Node Right;
    }

    internal Node Root;

    public delegate void DisplayDel(TValue value);

    public AVL()
    {
        Root = null;
    }

    public void Clear()
    {
        Root = null;
    }

    public void Insert(TKey Key, TValue Value)
    {
        Root = Insert(Root, Key, Value);
    }
    private Node Insert(Node parent, TKey Key, TValue Value)
    {
        if (parent == null)
        {
            parent = new Node();
            parent.Key = Key;
            parent.Value = Value;
        }
        else if (Key.CompareTo(parent.Key) == -1)
        {
            parent.Left = Insert(parent.Left, Key, Value);
            parent = BalanceTree(parent);
        }
        else
        {
            parent.Right = Insert(parent.Right, Key, Value);
            parent = BalanceTree(parent);
        }
        return parent;
    }

    private Node BalanceTree(Node node)
    {
        int balanceFactor = GetBalanceFactor(node);
        if (balanceFactor > 1)  //Left subtree is taller than right subtree
        {
            if (GetBalanceFactor(node.Left) >= 0)  //Left.left is taller than left.right (balance factor = 1 or 0: single right rotation)
            {
                node = RotateRight(node);
            }
            else
            {
                node = RotateLeftRight(node);
            }
        }
        else if (balanceFactor < -1) //Right subtree is taller than left
        {
            if (GetBalanceFactor(node.Right) > 0)  // Right.left is taller than right.right.
            {
                node = RotateRightLeft(node);
            }
            else
            {
                node = RotateLeft(node);          // balance factor = -1 or 0: single left rotation
            }
        }
        return node;
    }

    private Node RotateRightLeft(Node node)
    {
        Node pivot = node.Right;
        node.Right = RotateRight(pivot);
        return RotateLeft(node);
    }

    private Node RotateLeft(Node node)
    {
        Node pivot = node.Right;
        node.Right = pivot.Left;
        pivot.Left = node;
        return pivot;
    }

    private Node RotateLeftRight(Node node)
    {
        Node pivot = node.Left;
        node.Left = RotateLeft(pivot);
        return RotateRight(node);
    }

    private Node RotateRight(Node node)
    {
        Node pivot = node.Left;
        node.Left = pivot.Right;
        pivot.Right = node;
        return pivot;
    }

    private int GetBalanceFactor(Node node)
    {
        int l = GetHeight(node.Left);
        int r = GetHeight(node.Right);
        int balanceFactor = l - r;
        return balanceFactor;
    }

    private int GetHeight(Node node)
    {
        int height = 0;
        int l = 0;
        int r = 0;
        int m = 0;
        if (node == null) return -1;   // Height of a Null node is -1 
        if (node != null)
        {
            l = GetHeight(node.Left);
            r = GetHeight(node.Right);
            m = l > r ? l : r;
            height = m + 1;
        }
        return height;
    }

    public void Traverse(DisplayDel method)
    {
        Traverse(Root, method);
    }
    private void Traverse(Node node, DisplayDel method)
    {
        if (node == null) return;
        Traverse(node.Left, method);
        //node.Left = null;
        method(node.Value);
        Traverse(node.Right, method);
        //node.Right = null;
    }

    public void TraverseReverse(DisplayDel method)
    {
        TraverseReverse(Root, method);
    }
    private void TraverseReverse(Node node, DisplayDel method)
    {
        if (node == null) return;
        TraverseReverse(node.Right, method);
        method(node.Value);
        TraverseReverse(node.Left, method);
    }

    public TValue Find(int key)
    {
        return Find(Root, key);
    }

    private TValue Find(Node node, int key)
    {
        if (node == null) return default(TValue); // return null if nothing is found.

        if (key.CompareTo(node.Key) == 0) return node.Value;

        if (key.CompareTo(node.Key) == 1)
        {
            node = node.Right;
            return Find(node, key);
        }
        else
        {

            node = node.Left;
            return Find(node, key);
        }
    }
}

