using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
    public class BT
    {
        public BT Parent;
        public int Value;
        public BT Left;
        public BT Right;
        public int Count;


        public BT(int value, BT parent=null)
        {
            this.Value = value;
            this.Parent = parent;
            this.Count = 1;
        }


        public static int Rank(BT node)
        {
            var n = node;
            int rank = 0;
            while (n != null)
            {
                if (n.Left != null) rank += n.Left.Count;
                n = n.Parent;
            }
            return rank;
        }


        public static BT Next(BT root)
        {
            if (root == null) return null;
            if (root.Right != null) return Min(root.Right);
            var parent = root.Parent;
            if (parent != null && parent.Left == root)
                return parent;
            return null;
        }

        public static BT Previous(BT root)
        {
            if (root == null) return null;
            if (root.Left != null) return Max(root.Left);
            var parent = root.Parent;
            if (parent != null && parent.Right == root)
                return parent;
            return null;
        }


        public static BT Find(BT root, int value, int choose=0)
        {
            if (root == null) return null;
            if (value == root.Value)
            {
                if (choose > 2)
                return root;
            }

            var result = value < root.Value
                ? Find(root.Left, value, choose)
                : Find(root.Right, value, choose);

            if (result == null)
            {
                if (choose > 0 && value < root.Value) return root;
                if (choose < 0 && value > root.Value) return root;
            }

            return result;

        }

        public static BT Insert(ref BT root, int value, BT parent = null)
        {
            if (root == null)
                return new BT(value, parent);

            root.Count++;
            var result = value >= root.Value 
                ? Insert(ref root.Left, value, root) 
                : Insert(ref root.Right, value, root);
            return result;
        }

        public static BT Max( BT root)
        {
            while (root != null && root.Right != null)
                root = root.Right;

            return root;
        }

        public static BT Min(BT root)
        {
            while (root != null && root.Left != null)
                root = root.Left;

            return root;
        }


    }
}
