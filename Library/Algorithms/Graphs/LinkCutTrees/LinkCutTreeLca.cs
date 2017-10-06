using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Graphs
{
	// Based on Daniel Sleator's implementation http://www.codeforces.com/contest/117/submission/860934

	public class LinkCutTreeLca
	{
		public int Label;
		public int Size;
		public LinkCutTreeLca Left;
		public LinkCutTreeLca Right;
		public LinkCutTreeLca Parent;

		// tests whether x is a root of a splay tree
		public virtual bool IsRoot()
		{
			return Parent == null || (Parent.Left != this && Parent.Right != this);
		}

		public void MakeRoot()
		{
			Access();
		}

		public bool Connected(LinkCutTreeLca y)
		{
			if (this == y)
				return true;
			Access();
			// now x.parent is null
			y.Access();
			return Parent != null;
		}

		public void Link(LinkCutTreeLca y)
		{
			if (FindRoot() == y.FindRoot())
				return;
			Access();
			Debug.Assert(Right == null); //x.Right==null <=> x is rootnode
			Parent = y;
		}

		public void Cut()
		{
			Access();
			Debug.Assert(Right == null); //x.Right==null <=> x is rootnode
			Right.Parent = null;
			Right = null;
		}

		public LinkCutTreeLca FindRoot()
		{
			LinkCutTreeLca x = this;
			x.Access();
			while (x.Right != null)
				x = x.Right;
			x.Splay();
			return x;
		}

		public LinkCutTreeLca Lca(LinkCutTreeLca y)
		{
			if (FindRoot() != y.FindRoot())
				return null;
			Access();
			return y.Access();
		}

		public LinkCutTreeLca Access()
		{
			LinkCutTreeLca last = null;
			for (LinkCutTreeLca y = this; y != null; y = y.Parent)
			{
				y.Splay();
				y.Left = last;
				last = y;
			}
			Splay();
			return last;
		}

		void Splay()
		{
			while (!IsRoot())
			{
				var p = Parent;
				var g = p.Parent;
				if (!p.IsRoot())
					((this == p.Left) == (p == g.Left) ? p : this).Rotate();
				Rotate();
			}
		}

		void Rotate()
		{
			var p = Parent;
			var g = p.Parent;
			bool isRootP = p.IsRoot();
			bool leftChildX = this == p.Left;
			(leftChildX ? Right : Left).AttachChild(p, leftChildX);
			p.AttachChild(this, !leftChildX);
			AttachChild(g, !isRootP ? p == g.Left : (bool?)null);
		}

		void AttachChild(LinkCutTreeLca p, bool? isLeftChild)
		{
			if (this != null)
				Parent = p;
			if (isLeftChild != null)
			{
				if (isLeftChild.Value)
					p.Left = this;
				else
					p.Right = this;
			}
		}
	}
}


