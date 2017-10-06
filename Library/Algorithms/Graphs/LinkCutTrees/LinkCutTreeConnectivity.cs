using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Graphs
{
	// Based on Daniel Sleator's implementation http://www.codeforces.com/contest/117/submission/860934
	public class LinkCutTreeConnectivity
	{
		public LinkCutTreeConnectivity Left;
		public LinkCutTreeConnectivity Right;
		public LinkCutTreeConnectivity Parent;
		public bool Revert;

		// tests whether x is a root of a splay tree
		public virtual bool IsRoot()
		{
			return Parent == null || (Parent.Left != this && Parent.Right != this);
		}

		public virtual void Push()
		{
			if (Revert)
			{
				Revert = false;
				LinkCutTreeConnectivity t = Left;
				Left = Right;
				Right = t;
				if (Left != null) Left.Revert = !Left.Revert;
				if (Right != null) Right.Revert = !Right.Revert;
			}
		}

		public LinkCutTreeConnectivity Access()
		{
			LinkCutTreeConnectivity last = null;
			for (LinkCutTreeConnectivity y = this; y != null; y = y.Parent)
			{
				y.Splay();
				y.Left = last;
				last = y;
			}
			Splay();
			return last;
		}

		public void MakeRoot()
		{
			Access();
			Revert = !Revert;
		}

		public bool Connected(LinkCutTreeConnectivity y)
		{
			if (this == y)
				return true;
			Access();
			// now x.parent is null
			y.Access();
			return Parent != null;
		}

		public void Link(LinkCutTreeConnectivity y)
		{
			if (Connected(y)) return;
			MakeRoot();
			Parent = y;
		}

		public void Cut(LinkCutTreeConnectivity y)
		{
			MakeRoot();
			y.Access();
			// check that exposed path consists of a single edge (y,x)
			if (y.Right != this || Left != null || Right != null)
				return;
			y.Right.Parent = null;
			y.Right = null;
		}

		void AttachChild(LinkCutTreeConnectivity p, bool? isLeftChild)
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

		void Splay()
		{
			while (!IsRoot())
			{
				var p = Parent;
				var g = p.Parent;
				if (!p.IsRoot())
					g.Push();
				p.Push();
				Push();
				if (!p.IsRoot())
					((this == p.Left) == (p == g.Left) ? p : this).Rotate();
				Rotate();
			}
			Push();
		}
	}
}
