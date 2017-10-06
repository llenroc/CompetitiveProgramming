#region

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Windows;

using System.Runtime.InteropServices;
using Softperson.Mathematics;


#endregion

#pragma warning disable 253

namespace Softperson.ComputationalGeometry
{
	[StructLayout(LayoutKind.Sequential)]
	[DebuggerStepThrough]
	public struct Matrix2D
	{
		#region Variables

		private MatrixGroup _group;

		#endregion

		#region Construction

		public Matrix2D(double e11,
						double e12,
						double e21,
						double e22,
						double dx,
						double dy)
		{
			_e11 = e11;
            _e12 = e12;
            _e21 = e21;
            _e22 = e22;
            _offsetX = dx;
			_offsetY = dy;
		    _group = 0;
            _group = InferGroup();
		}

		public MatrixGroup InferGroup()
		{
			if (E12 != 0 || E21 != 0)
				return MatrixGroup.Affine;

			if (OffsetX == 0 && OffsetY == 0)
			{
				if (E11 != E21)
					return MatrixGroup.AnisotropicScale;
				if (E11 == 1.0)
					return MatrixGroup.Identity;
				if (E11 != 0.0)
					return MatrixGroup.IsotropicScale;
				return 0;
			}

			if (E11 != E22)
				return MatrixGroup.WindowToViewport;

			if (E11 == 1.0)
				return MatrixGroup.Translation;

			return MatrixGroup.AnglePreserving | MatrixGroup.WindowToViewport;
		}

		#endregion

		#region Elements

		private double _e11;
		public double E11
		{
			get { return _e11; }
			set { _e11 = value; }
		}

		private double _e12;
		public double E12
		{
			get { return _e12; }
			set { _e12 = value; }
		}

		private double _e21;
		public double E21
		{
			get { return _e21; }
			set { _e21 = value; }
		}

		private double _e22;
		public double E22
		{
			get { return _e22; }
			set { _e22 = value; }
		}

		private double _offsetX;
		public double OffsetX
		{
			get { return _offsetX; }
			set { _offsetX = value; }
		}

		private double _offsetY;
		public double OffsetY
		{
			get { return _offsetY; }
			set { _offsetY = value; }
		}


		#endregion

		#region Object Overloads

		public override bool Equals(object obj)
		{
			return obj is Matrix2D && Equals((Matrix2D)obj);
		}

		public bool Equals(Matrix2D matrix)
		{
			return E11 == matrix.E11 && E22 == matrix.E22
				   && E12 == matrix.E12 && E21 == matrix.E21
				   && OffsetX == matrix.OffsetX && OffsetY == matrix.OffsetY;
		}

		public static bool operator ==(Matrix2D matrix1, Matrix2D matrix2)
		{
			return matrix1.Equals(matrix2);
		}

		public static bool operator !=(Matrix2D matrix1, Matrix2D matrix2)
		{
			return !matrix1.Equals(matrix2);
		}

		public override int GetHashCode()
		{
			return E11.GetHashCode() ^ E12.GetHashCode()
				   ^ E21.GetHashCode() ^ E22.GetHashCode()
				   ^ OffsetX.GetHashCode() ^ OffsetY.GetHashCode();
		}

		public override string ToString()
		{
			return $"{E11},{E12};{E21},{E22};{OffsetX},{OffsetY}";
		}

		#endregion

		#region Properties

	    public static readonly Matrix2D Identity = new Matrix2D(1,0,0,1,0,0);

	    /// <summary>
		/// 	Indicates whether volume is conserved
		/// </summary>
		public bool IsOrthogonal => Math.Abs(Determinant).AreClose(1);

        public bool IsInvertible => !Numbers.IsZeroed(Determinant);

	    public bool IsIdentity
		{
			get {
			    return _group == MatrixGroup.Identity ||
			           E11 == 1 && E22 == 1 && E12 == 0 && E21 == 0 && OffsetX == 0 && OffsetY == 0;
			}
		}

		public double this[Axis row, Axis column]
		{
			get { return this[(int) row + 1, (int) column + 1]; }
			set { this[(int) row + 1, (int) column + 1] = value; }
		}

		public unsafe double this[int row, int column]
		{
			get
			{
				switch (row)
				{
				case 1:
					row = -1;
					break;
				case 2:
					row = 1;
					break;
				case 3:
					row = 3;
					break;
				default:
					throw (new IndexOutOfRangeException());
				}
				if ((uint) column - 1 >= 2)
					throw (new ArgumentOutOfRangeException());

				fixed (double* p = &_e11)
					return p[row + column];
			}
			set
			{
				switch (row)
				{
				case 1:
					row = -1;
					break;
				case 2:
					row = 1;
					break;
				case 3:
					row = 3;
					break;
				default:
					throw (new IndexOutOfRangeException());
				}
				if ((uint) column - 1 >= 2)
					throw (new ArgumentOutOfRangeException());

				fixed (double* p = &_e11)
					p[row + column] = value;
			}
		}


		public Point2D Offset => new Point2D((double) OffsetX, (double) OffsetY);

	    public double Determinant => E11*E22 - E12*E21;

	    public Matrix2D Inverse
		{
			[DebuggerStepThrough]
			get
			{
				Matrix2D result = this;
				result.Invert();
				return result;
			}
		}

		#endregion

		#region Operations

		public bool IsInGroup(MatrixGroup group)
		{
			return (_group & group) == group;
		}

		public void Invert()
		{
			if (IsInGroup(MatrixGroup.WindowToViewport))
			{
				if (!IsInGroup(MatrixGroup.Translation))
				{
					E11 = 1.0/E11;
					E22 = 1.0/E22;
				}

				OffsetX *= -E11;
				OffsetY *= -E22;
				return;
			}

			// Get Determinant
			double m11 = E22;
			double m21 = -E12;
			double det = E21*m21 + E11*m11;
			if (Numbers.IsZeroed(det))
				throw new InvalidOperationException();

			// Get Adjoint
			double f = (1.0/det);
			double m22 = E11;
			double m12 = -E21;

			// Handle Translation
			double t13 = E11*OffsetY - OffsetX*E12;
			double t23 = E21*OffsetY - OffsetX*E22;

			// Transpose
			E11 = m11*f;
			E12 = m21*f;
			E21 = m12*f;
			E22 = m22*f;

			// REVIEW: are Dx and Dy adequately represented
			OffsetX = t13;
			OffsetY = t23;
		}

		public void Multiply(Matrix2D matrix, ApplyOrder order = ApplyOrder.Append)
		{
			if (matrix._group == MatrixGroup.Identity)
				return;

			if (order == ApplyOrder.Prepend)
				AssignProduct(matrix, this);
			else
				AssignProduct(this, matrix);
		}

		public void Assign(ref Matrix2D matrix)
		{
			E11 = matrix.E11;
			E12 = matrix.E12;
			E21 = matrix.E21;
			E22 = matrix.E22;
			OffsetX = matrix.OffsetX;
			OffsetY = matrix.OffsetY;
			_group = matrix._group;
		}

		private void AssignProduct(Matrix2D m1, Matrix2D m2)
		{
			if (m2._group == MatrixGroup.Identity)
			{
				Assign(ref m1);
				return;
			}
			else if (m1._group == MatrixGroup.Identity)
			{
				Assign(ref m2);
				return;
			}

			double t11 = m1.E11*m2.E11 + m1.E12*m2.E21;
			double t12 = m1.E11*m2.E12 + m1.E12*m2.E22;
			double t21 = m1.E21*m2.E11 + m1.E22*m2.E21;
			double t22 = m1.E21*m2.E12 + m1.E22*m2.E22;
			double tx = m1.OffsetX*m2.E11 + m1.OffsetY*m2.E21 + m2.OffsetX;
			double ty = m1.OffsetX*m2.E12 + m1.OffsetY*m2.E22 + m2.OffsetY;

			E11 = t11;
			E12 = t12;
			E21 = t21;
			E22 = t22;
			OffsetX = tx;
			OffsetY = ty;
			_group = m1._group & m2._group;
		}

		[DebuggerStepThrough]
		public void Reset()
		{
			OffsetY = OffsetX = 0.0;

			if (!IsInGroup(MatrixGroup.Translation))
			{
				E12 = E21 = 0.0;
				E11 = E22 = 1.0;
			}

			_group = MatrixGroup.Identity;
		}

		public void Rotate(double angle, Point2D pt, ApplyOrder order = ApplyOrder.Append)
		{
			Multiply(RotationMatrix(angle).CenterAt(pt), order);
		}

		public void Scale(double sx, double sy, Point2D point = default(Point2D))
		{
			Scale(sx, sy, point, ApplyOrder.Append);
		}

		public void Scale(double sx, double sy, Point2D point, ApplyOrder order)
		{
			Multiply(ScaleMatrix(sx, sy).CenterAt(point), order);
		}

        
        public Point2D Transform(Point2D point)
        {
            return point * this;
        }

        public Vector2D Transform(Vector2D vector)
        {
            return vector * this;
        }
        

        public void Transform(Point2D[] points)
		{
			for (int i = 0; i < points.Length; i++)
				points[i] *= this;
		}

        public void Transform(Vector2D[] vectors)
		{
			for (int i = 0; i < vectors.Length; i++)
				vectors[i] *= this;
		}

		public void Translate(double dx, double dy, ApplyOrder order = ApplyOrder.Append)
		{
			Multiply(TranslationMatrix(dx, dy), order);
		}

		public void Transpose2x2()
		{
			double tmp = E12;
			E12 = E21;
			E21 = tmp;
		}

		#endregion

		#region Operation

		public Matrix2D CenterAt(double x, double y)
		{
			if (x != 0 || y != 0)
			{
				OffsetX += x - x*E11 - y*E21;
				OffsetY += y - x*E12 - y*E22;
				_group &= MatrixGroup.Translation;
			}
			return this;
		}

		public Matrix2D CenterAt(Point2D point)
		{
			return CenterAt(point.X, point.Y);
		}

		public static Matrix2D RotationMatrix(double angle)
		{
			double sin = Math.Sin(angle);
			double cos = Math.Cos(angle);
			var matrix = new Matrix2D
			{
				E11 = cos,
				E12 = sin,
				E21 = -sin,
				E22 = cos,
				_group = MatrixGroup.Rotation
			};
			return matrix;
		}

		public static Matrix2D ScaleMatrix(double sx, double sy)
		{
		    return new Matrix2D
		    {
		        E11 = sx,
		        E22 = sy,
		        _group = (sx != sy)
		            ? MatrixGroup.AnisotropicScale
		            : (sx != 1)
		                ? MatrixGroup.IsotropicScale
		                : MatrixGroup.Identity
		    };
		}

		public static Matrix2D TranslationMatrix(double dx, double dy)
		{
			return new Matrix2D
			{
                E11 = 1,
                E22 = 1,
				OffsetX = dx,
				OffsetY = dy,
                _group = dx!=0 || dy!=0 ? MatrixGroup.Translation : MatrixGroup.Identity
			};
		}

		public static Matrix2D ShearMatrix(double shearX, double shearY)
		{
			var matrix = new Matrix2D
			{
                E11 = 1,
                E22 = 1,
				E12 = shearX,
				E21 = shearY,
                _group = MatrixGroup.Affine,

			};
			if (shearX == 0 && shearY == 0)
				matrix._group = MatrixGroup.Identity;
			return matrix;
		}

		public static Matrix2D SkewMatrix(double skewX, double skewY)
		{
			return ShearMatrix(Math.Tan(skewY), Math.Tan(skewX));
		}

		#endregion

		#region Operators

		public static Vector2D operator *(Vector2D vector, Matrix2D matrix)
		{
			return new Vector2D(
				vector.X*matrix.E11 + vector.Y*matrix.E21,
				vector.X*matrix.E12 + vector.Y*matrix.E22
				);
		}

		public static Point2D operator *(Point2D point, Matrix2D matrix)
		{
			return new Point2D(
				point.X*matrix.E11 + point.Y*matrix.E21 + matrix.OffsetX,
				point.X*matrix.E12 + point.Y*matrix.E22 + matrix.OffsetY
				);
		}

		public static Matrix2D operator *(Matrix2D m1, Matrix2D m2)
		{
			if (m1._group == MatrixGroup.Identity)
				return m2;

			if (m2._group == MatrixGroup.Identity)
				return m1;

			var m = new Matrix2D();
			m.AssignProduct(m1, m2);
			return m;
		}

		#endregion

		#region Apply To Functions

		public Box2D ApplyTo(Box2D rectangle)
		{
			if (!IsInGroup(MatrixGroup.WindowToViewport))
				return RotateRectangle(rectangle);

			double x = rectangle.Left*E11 + rectangle.Top*E12 + OffsetX;
			double y = rectangle.Left*E21 + rectangle.Top*E22 + OffsetY;
			double width = rectangle.Width*E11 + rectangle.Height*E12;
			double height = rectangle.Width*E21 + rectangle.Height*E22;

			if (width < 0)
			{
				x += width;
				width = -width;
			}

			if (height < 0)
			{
				y += height;
				height = -height;
			}

			return Box2D.FromWidthHeight(x, y, width, height);
		}

		Box2D RotateRectangle(Box2D rect)
		{
			var point2Ds = rect.GetPoints();
			Transform(point2Ds);
			return Box2D.From(point2Ds);
		}

		[DebuggerStepThrough]
		public Point2D ApplyTo(Point2D point2D)
		{
			return new Point2D(point2D.X*E11 + point2D.Y*E12 + OffsetX,
							  point2D.X*E21 + point2D.Y*E22 + OffsetY);
		}

		[DebuggerStepThrough]
		public Vector2D ApplyTo(Vector2D size)
		{
			return new Vector2D(size.X*E11 + size.Y*E12,
							 size.X*E21 + size.Y*E22);
		}

		#endregion
	}
}