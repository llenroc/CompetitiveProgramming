﻿#region Copyright

//  This source code may not be reviewed, copied, or redistributed without
//  the expressed permission of Wesner Moise.
//  
//  File: Margin.cs
//  Created: 11/11/2012 
//  Modified: 05/05/2014
// 
//  Copyright (C) 2012 - 2014, Wesner Moise.

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Diagnostics;
using Softperson.ComputationalGeometry;

#endregion

namespace Softperson
{
	/// <summary>
	///     Summary description for Margin.
	/// </summary>
	[DebuggerStepThrough]
	public struct Margin
	{
		#region Variables

		#endregion

		#region Constructor

		public Margin(double margin)
		{
			if (margin < 0) throw new ArgumentOutOfRangeException();
			Left = margin;
			Top = margin;
			Right = margin;
			Bottom = margin;
		}

		public Margin(double xMargin, double yMargin)
		{
			if (xMargin < 0 || yMargin < 0) throw new ArgumentOutOfRangeException();
			Left = xMargin;
			Top = yMargin;
			Right = xMargin;
			Bottom = yMargin;
		}

		public Margin(double left, double top, double right, double bottom)
		{
			if (left < 0 || top < 0 || right < 0 || bottom < 0) throw new ArgumentOutOfRangeException();
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		#endregion

		#region Object Overloads

		public override bool Equals(object obj)
		{
			return obj is Margin && Equals((Margin) obj);
		}

		public bool Equals(Margin margin)
		{
			return margin.Top == Top && margin.Left == Left && margin.Right == Right && margin.Bottom == Bottom;
		}

		public static bool operator ==(Margin margin1, Margin margin2)
		{
			return margin1.Equals(margin2);
		}

		public static bool operator !=(Margin margin1, Margin margin2)
		{
			return !margin1.Equals(margin2);
		}

		public override string ToString()
		{
			if (Top != Bottom || Left != Right)
				return $"{Left}, {Top}, {Right}, {Bottom}";
			if (Top != Left)
				return $"{Left}, {Top}";
			return Left.ToString();
		}

		public override int GetHashCode()
		{
			return Top.GetHashCode() ^ (Left + 100).GetHashCode() ^ (Right + 1000).GetHashCode() ^
				   (Bottom + 10000).GetHashCode();
		}

		#endregion

		#region Properties

		[Browsable(false)]
		public static Margin Empty => new Margin();

		[Browsable(false)]
		public bool IsEmpty => Left == 0 && Right == 0 && Top == 0 && Bottom == 0;

		public double Left { get; set; }

		public double Right { get; set; }

		public double Top { get; set; }

		public double Bottom { get; set; }

		[Browsable(false)]
		public double Width
		{
			get { return Left + Right; }
		}

		[Browsable(false)]
		public double Height
		{
			get { return Top + Bottom; }
		}

		[Browsable(false)]
		public double HorizontalSpacing => Math.Max(Left, Right);

		[Browsable(false)]
		public double VerticalSpacing => Math.Max(Top, Bottom);

		public Vector2D Size => new Vector2D(Left + Right, Top + Bottom);

		#endregion

		#region Resizing

		public void Inflate(double margin)
		{
			Top += margin;
			Bottom += margin;
			Left += margin;
			Right += margin;
		}

		public void Inflate(double xMargin, double yMargin)
		{
			Top += yMargin;
			Bottom += yMargin;
			Left += xMargin;
			Right += xMargin;
		}

		public void Inflate(double leftMargin, double topMargin, double rightMargin, double bottomMargin)
		{
			Left += leftMargin;
			Top += topMargin;
			Right += rightMargin;
			Bottom += bottomMargin;
		}

		public Vector2D Inflate(Vector2D v)
		{
			return new Vector2D(v.X + Left + Right, v.Y + Top + Bottom);
		}

		public Box2D Deflate(Box2D rect)
		{
			return Box2D.FromWidthHeight(rect.Left + Left, rect.Top+ Top, 
                Math.Max(0, rect.Width-Width), Math.Max(0, rect.Height - Height));
		}

		public Vector2D Deflate(Vector2D size)
		{
			return new Vector2D(Math.Max(0, size.X - Width), Math.Max(0, size.Y - Height));
		}

		public Box2D Inflate(Box2D rect)
		{
			return new Box2D(rect.Left - Left, rect.Top- Top, rect.Right + Right, rect.Bottom + Bottom);
		}

		public static Margin Max(Margin margin1, Margin margin2)
		{
			return new Margin(Math.Max(margin1.Left, margin2.Left), Math.Max(margin1.Top, margin2.Top),
				Math.Max(margin1.Right, margin2.Right), Math.Max(margin1.Bottom, margin2.Bottom));
		}

		public static Margin Min(Margin margin1, Margin margin2)
		{
			return new Margin(Math.Min(margin1.Left, margin2.Left), Math.Min(margin1.Top, margin2.Top),
				Math.Min(margin1.Right, margin2.Right), Math.Min(margin1.Bottom, margin2.Bottom));
		}

		public bool Contains(Margin margin)
		{
			return margin.Left <= Left && margin.Right <= Right && margin.Top <= Top && margin.Bottom <= Bottom;
		}

		public static Margin operator +(Margin margin1, Margin margin2)
		{
			return new Margin(margin1.Left + margin2.Left, margin1.Top + margin2.Top, margin1.Right + margin2.Right,
				margin1.Bottom + margin2.Bottom);
		}

		public static Margin operator -(Margin margin1, Margin margin2)
		{
			return new Margin(Math.Max(0, margin1.Left - margin2.Left), Math.Max(0, margin1.Top - margin2.Top),
				Math.Max(0, margin1.Right - margin2.Right), Math.Max(0, margin1.Bottom - margin2.Bottom));
		}

		#endregion
	}
}