using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Graphs.TreeGraphs
{
	public class LinkCutTreePathQueries
	{
		public int nodeValue;
		internal int SubTreeValue;
		internal int Delta; // delta affects nodeValue, subTreeValue, left.delta and right.delta
		internal int Size;
		public bool Revert;
		public LinkCutTreePathQueries Left;
		public LinkCutTreePathQueries Right;
		public LinkCutTreePathQueries Parent;

		internal LinkCutTreePathQueries(int value)
		{
			nodeValue = value;
			SubTreeValue = value;
			Delta = GetNeutralDelta();
			Size = 1;
		}

		internal virtual bool IsRoot()
		{
			return Parent == null || (Parent.Left != this && Parent.Right != this);
		}

		internal virtual void Push()
		{
			if (Revert)
			{
				Revert = false;
				LinkCutTreePathQueries t = Left;
				Left = Right;
				Right = t;
				if (Left != null)
					Left.Revert = !Left.Revert;
				if (Right != null)
					Right.Revert = !Right.Revert;
			}

			nodeValue = JoinValueWithDelta(nodeValue, Delta);
			SubTreeValue = JoinValueWithDelta(SubTreeValue, DeltaEffectOnSegment(Delta, Size));
			if (Left != null)
				Left.Delta = JoinDeltas(Left.Delta, Delta);
			if (Right != null)
				Right.Delta = JoinDeltas(Right.Delta, Delta);
			Delta = GetNeutralDelta();
		}

		internal virtual void Update()
		{
			SubTreeValue = QueryOperation(QueryOperation(GetSubTreeValue(Left), JoinValueWithDelta(nodeValue, Delta)),
				GetSubTreeValue(Right));
			Size = 1 + GetSize(Left) + GetSize(Right);
		}

		internal void Rotate()
		{
			LinkCutTreePathQueries p = Parent;
			LinkCutTreePathQueries g = p.Parent;
			bool isRootP = p.IsRoot();
			bool leftChildX = (this == p.Left);

			// create 3 edges: (x.r(l),p), (p,x), (x,g)
			Connect(leftChildX ? Right : Left, p, leftChildX);
			Connect(p, this, !leftChildX);
			Connect(this, g, isRootP ? (bool?) null : p == g.Left);
			p.Update();
		}

		internal void Splay()
		{
			while (!IsRoot())
			{
				LinkCutTreePathQueries p = Parent;
				LinkCutTreePathQueries g = p.Parent;
				if (!p.IsRoot())
					g.Push();
				p.Push();
				Push();
				if (!p.IsRoot())
					((this == p.Left) == (p == g.Left) ? p : this).Rotate();
				Rotate();
			}
			Push();
			Update();
		}

		public void MakeRoot()
		{
			Expose();
			Revert = !Revert;
		}

		public bool Connected(LinkCutTreePathQueries y)
		{
			if (this == y)
				return true;
			Expose();
			// now x.parent is null
			y.Expose();
			return Parent != null;
		}

		public bool Link(LinkCutTreePathQueries y)
		{
			if (Connected(y))
				// x and y are already connected
				return false;
			MakeRoot();
			Parent = y;
			return true;
		}

		public bool Cut(LinkCutTreePathQueries y)
		{
			MakeRoot();
			y.Expose();
			if (y.Right != this || Left != null)
				// no edge (x,y)
				return false;
			y.Right.Parent = null;
			y.Right = null;
			return true;
		}

		public int Query(LinkCutTreePathQueries to)
		{
			MakeRoot();
			to.Expose();
			return GetSubTreeValue(to);
		}

		public void Modify(LinkCutTreePathQueries to, int delta)
		{
			MakeRoot();
			to.Expose();
			to.Delta = JoinDeltas(to.Delta, delta);
		}

		internal LinkCutTreePathQueries Expose()
		{
			LinkCutTreePathQueries last = null;
			for (LinkCutTreePathQueries y = this; y != null; y = y.Parent)
			{
				y.Splay();
				y.Left = last;
				last = y;
			}
			Splay();
			return last;
		}

		static int ModifyOperation(int x, int y)
		{
			return x + y;
		}

		static int QueryOperation(int leftValue, int rightValue)
		{
			return leftValue + rightValue;
		}

		static int DeltaEffectOnSegment(int delta, int segmentLength)
		{
			if (delta == GetNeutralDelta())
				return GetNeutralDelta();
			return delta * segmentLength;
		}

		static int GetNeutralDelta()
		{
			return 0;
		}

		static int GetNeutralValue()
		{
			return 0;
		}

		// generic code
		static int JoinValueWithDelta(int @value, int delta)
		{
			if (delta == GetNeutralDelta())
				return @value;
			return ModifyOperation(@value, delta);
		}

		static int JoinDeltas(int delta1, int delta2)
		{
			if (delta1 == GetNeutralDelta())
				return delta2;
			if (delta2 == GetNeutralDelta())
				return delta1;
			return ModifyOperation(delta1, delta2);
		}

		internal static int GetSize(LinkCutTreePathQueries root)
		{
			return root?.Size ?? 0;
		}

		internal static int GetSubTreeValue(LinkCutTreePathQueries root)
		{
			return root == null
				? GetNeutralValue()
				: JoinValueWithDelta(root.SubTreeValue, DeltaEffectOnSegment(root.Delta, root.Size));
		}

		internal static void Connect(LinkCutTreePathQueries ch, LinkCutTreePathQueries p, bool? isLeftChild)
		{
			if (ch != null)
				ch.Parent = p;
			if (isLeftChild != null)
			{
				if (isLeftChild == true)
					p.Left = ch;
				else
					p.Right = ch;
			}
		}

	}
}