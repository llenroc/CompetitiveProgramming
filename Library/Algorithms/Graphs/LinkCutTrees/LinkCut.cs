using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Graphs
{
    public class LinkCut
    {
        static void RotateRight(Node p)
        {
            Node q = p.Parent;
            Node r = q.Parent;
            q.Normalize();
            p.Normalize();
            if ((q.Left = p.Right) != null) q.Left.Parent = q;
            p.Right = q;
            q.Parent = p;
            if ((p.Parent = r) != null)
            {
                if (r.Left == q) r.Left = p;
                else if (r.Right == q) r.Right = p;
            }
            q.Update();
        }

        static void RotateLeft(Node p)
        {
            Node q = p.Parent;
            Node r = q.Parent;
            q.Normalize();
            p.Normalize();
            if ((q.Right = p.Left) != null) q.Right.Parent = q;
            p.Left = q;
            q.Parent = p;
            if ((p.Parent = r) != null)
            {
                if (r.Left == q) r.Left = p;
                else if (r.Right == q) r.Right = p;
            }
            q.Update();
        }

        static void Splay(Node p)
        {
            while (!p.IsRoot)
            {
                Node q = p.Parent;
                if (q.IsRoot)
                {
                    if (q.Left == p) RotateRight(p);
                    else RotateLeft(p);
                }
                else
                {
                    Node r = q.Parent;
                    if (r.Left == q)
                    {
                        if (q.Left == p)
                        {
                            RotateRight(q);
                            RotateRight(p);
                        }
                        else
                        {
                            RotateLeft(p);
                            RotateRight(p);
                        }
                    }
                    else
                    {
                        if (q.Right == p)
                        {
                            RotateLeft(q);
                            RotateLeft(p);
                        }
                        else
                        {
                            RotateRight(p);
                            RotateLeft(p);
                        }
                    }
                }
            }
            p.Normalize(); // only useful if p was already a root.
            p.Update(); // only useful if p was not already a root
        }

        /* This makes node q the root of the virtual tree, and also q is the 
           leftmost node in its splay tree */

        static void Expose(Node q)
        {
            Node r = null;
            for (Node p = q; p != null; p = p.Parent)
            {
                Splay(p);
                p.Left = r;
                p.Update();
                r = p;
            }
            ;
            Splay(q);
        }

        /* assuming p and q are nodes in different trees and  
           that p is a root of its tree, this links p to q */

        static void Link(Node p, Node q)
        {
            Expose(p);
            if (p.Right != null)
                throw new Exception("non-root link");
            p.Parent = q;
        }

        /* Toggle all the edges on the path from p to the root
       return the count after - count before */

        static int Toggle(Node p)
        {
            Expose(p);
            int before = p.On;
            p.Flip = !p.Flip;
            p.Normalize();
            int after = p.On;
            return after - before;
        }

        /* this returns the id of the node that is the root of the tree containing p */

        static int RootId(Node p)
        {
            Expose(p);
            while (p.Right != null) p = p.Right;
            Splay(p);
            return p.Id;
        }


        class Node
        {
            #region Variables
            private int _s;
            private readonly int _myS;
            bool _myFlip;

            public readonly int Id;
            public Node Left;
            public Node Right;
            public Node Parent;
            public int On;
            public bool Flip;
            #endregion Variables

            #region Construction
            public Node(int c, int i)
            {
                Id = i;
                _s = _myS = c;
                On = 0;
                Left = Right = Parent = null;
                Flip = _myFlip = false;
            }
            #endregion

            public bool IsRoot => Parent == null || (Parent.Left != this && Parent.Right != this);

            /* If this node is flipped, we unflip it, and push the change
               down the tree, so that it represents the same thing. */
            public void Normalize()
            {
                if (Flip)
                {
                    Flip = false;
                    On = _s - On;
                    _myFlip = !_myFlip;
                    if (Left != null)
                        Left.Flip = !Left.Flip;
                    if (Right != null)
                        Right.Flip = !Right.Flip;
                }
            }

            /* The tree structure has changed in the vicinity of this node
               (for example, if this node is linked to a different left
               child in a rotation).  This function fixes up the data fields
               in the node to maintain invariants. */
            public void Update()
            {
                _s = _myS;
                On = (_myFlip) ? _myS : 0;

                if (Left != null)
                {
                    _s += Left._s;
                    if (Left.Flip) On += Left._s - Left.On; else On += Left.On;
                }

                if (Right != null)
                {
                    _s += Right._s;
                    if (Right.Flip) On += Right._s - Right.On; else On += Right.On;
                }
            }
        }

    }

}
