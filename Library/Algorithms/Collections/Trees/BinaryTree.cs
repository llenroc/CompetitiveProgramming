using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
    class BinaryTree
    {
    }


    public class OST
    {
        #region Variables
        public OST Left;
        public OST Right;
        public int Dup;
        public int NodeCount;
        public int Key;
        #endregion

        public OST(int key, OST left=null, OST right=null)
        {
            Key = key;
            Dup = 1;
            NodeCount = 1;
            Left = left;
            Right = right;
        }

        void Recalc()
        {
            NodeCount = Dup + Count(Left) + Count(Right);
        }

        public static int Count(OST ost)
        {
            return ost?.NodeCount ?? 0;
        }

        public static void Insert(ref OST root, int key)
        {
            if (root == null)
                root = new OST(key);
            else if (key < root.Key)
                Insert(ref root.Left, key);
            else if (key > root.Key)
                Insert(ref root.Right, key);
            else
                root.Dup++;
            root.Recalc();
        }

        public static int Rank(OST root, int key, bool upper=false)
        {
            if (root == null)
                return 0;

            if (key < root.Key)
                return Rank(root.Left, key, upper);

            int bound = Count(root.Left);
            if (key == root.Key)
                return bound + (upper?root.Dup:0);
            return bound + root.Dup + Rank(root.Right, key, upper);
        }

        public static OST Select(OST root, int index)
        {
            while (root != null)
            {
                int count = Count(root.Left);
                if (index < count)
                    root = root.Left;
                else
                {
                    index -= count - root.Dup;
                    if (index < 0)
                        return root;
                    root = root.Right;
                }
            }
            return null;
        }

        public static void Update(OST root, int index, int value)
        {
            
        }

        public OST Rebalance(ref OST root)
        {
            if (root == null)
                return null;

            var left = root.Left;
            var right = root.Right;
            if (IsGreater(left, right))
            {
                var ll = left.Left;
                var lr = left.Right;
                if (Count(ll) >= Count(lr))
                    root = Reconstruct(left, ll, Reconstruct(root, lr, right));
                else
                    root = Reconstruct(lr, 
                        Reconstruct(left, ll, lr.Left),
                        Reconstruct(root, lr.Right, right));
            }

            if (IsGreater(right, left))
            {
                var rl = right.Left;
                var rr = right.Right;
                if (Count(rl) <= Count(rr))
                    root = Reconstruct(right, Reconstruct(root, left, rl), rr);
                else
                    root = Reconstruct(rl,
                        Reconstruct(root, left, rl.Left),
                        Reconstruct(right, rl.Right, rr));
            }

            return root;
        }


        public bool IsGreater(OST left, OST right)
        {
            int leftCount = Count(left)+1;
            int rightCount = Count(right)+1;
            return leftCount > 3*(rightCount + 1);
        }

        public OST Reconstruct(OST parent, OST left, OST right)
        {
            parent.Left = left;
            parent.Right = right;
            return parent;
        }

		
    }
}
