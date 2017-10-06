#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Diagnostics;

#endregion

namespace Softperson
{
	/// <summary>
	///     Summary description for Units.
	/// </summary>
	public struct Quantity : IComparable<Quantity>
	{
		#region Constants

		public const float Dpi = 100f;
		public const float Inch = Dpi;

		public static Unit DefaultUnit = Unit.Inch;

		#endregion

		#region Variables

		#endregion

		#region Constructor

		public Quantity(double number, Unit unit)
		{
			Number = number;
			Unit = unit;
		}

		public Quantity(double number)
		{
			Number = number;
			Unit = Unit.View;
		}

		public static explicit operator double(Quantity q)
		{
			return q.Number;
		}

		public static implicit operator Quantity(double Number)
		{
			return new Quantity((float) Number);
		}

		#endregion

		#region Properties

		[DefaultValue(0.0)]
		public double Number { get; set; }

		[DefaultValue(Unit.None)]
		public Unit Unit { get; set; }

		[DebuggerStepThrough]
		public double GetNumber(double reference)
		{
			if (Unit == Unit.Percent)
				return Number*reference;
			return Number;
		}

		[DebuggerStepThrough]
		public double GetNumber(Unit unit, double reference)
		{
			if (Unit == Unit.Percent)
				return Number*reference;
			if (unit == Unit.None || Unit == unit)
				return Number;

			var q = this;
			q.Convert(unit, true);
			return q.Number;
		}

		#endregion

		#region Presentation

		public override string ToString()
		{
			return ToString(null);
		}

		public string ToString(string format)
		{
			var number = format == null ? Number.ToString() : Number.ToString(format);

			switch (Unit)
			{
				default:
					return number;


				case Unit.View:
					var tmp = this;
					tmp.Convert(DefaultUnit, true);
					return tmp.ToString();

				case Unit.Emu:
					return string.Format("{0} emu", number);
				case Unit.Yard:
					return string.Format("{0} yd", number);
				case Unit.Kilometer:
					return string.Format("{0} km", number);
				case Unit.Meter:
					return string.Format("{0} m", number);
				case Unit.Foot:
					return string.Format("{0} ft", number);
				case Unit.Inch:
					return string.Format("{0}\"", number);
				case Unit.Centimeter:
					return string.Format("{0} cm", number);
				case Unit.Millimeter:
					return string.Format("{0} mm", number);
				case Unit.Point:
					return string.Format("{0} pt", number);
				case Unit.Picas:
					return string.Format("{0} pc", number);
				case Unit.Pixel:
					return string.Format("{0} px", number);
				case Unit.Line:
					return string.Format("{0} li", number);
				case Unit.Percent:
					return string.Format("{0}%", number);
				case Unit.Twips:
					return string.Format("{0} tw", number);
			}
		}

		#endregion

		#region Parsing

		public static bool Parse(string text, out Quantity q)
		{
			q = new Quantity();
			text = text.Trim();

			var len = text.Length;
			var i = 0;

			while (i < len)
			{
				var ch = text[i];
				if (!char.IsDigit(ch) && ch != '-' && ch != '.')
					break;
				i++;
			}

			var j = i;
			while (j < len)
			{
				var ch = text[j];
				if (ch != ' ')
					break;
				j++;
			}

			var Number = text.Substring(0, i);
			var units = text.Substring(j, len - j);

			if (Number.Length == 0)
				return false;

			try
			{
				q.Number = float.Parse(Number);
			}
			catch
			{
				return false;
			}

			q.Unit = GetUnits(units);

			return q.Unit != Unit.Invalid;
		}

		public static Unit GetUnits(string text)
		{
			if (text == null || text.Length == 0)
				return Unit.None;

			switch (text)
			{
				case "%":
					return Unit.Percent;

				case "px":
				case "pixel":
				case "pixels":
					return Unit.Pixel;

				case "\"":
				case "in":
				case "inch":
				case "inches":
					return Unit.Inch;

				case "mm":
				case "millimeter":
					return Unit.Millimeter;

				case "cm":
				case "centimeter":
					return Unit.Centimeter;

				case "pc":
				case "pica":
				case "picas":
					return Unit.Picas;

				case "tw":
				case "twips":
					return Unit.Twips;

				case "pt":
				case "point":
				case "points":
					return Unit.Point;

				case "li":
				case "line":
				case "lines":
					return Unit.Line;

				default:
					return Unit.Invalid;
			}
		}

		#endregion

		#region Conversion

		public void Convert(Unit target, bool force)
		{
			if (Unit == target)
				return;

			if (force)
			{
				if (Unit == Unit.Line)
					Unit = Unit.Picas;
				else if (Unit == Unit.None)
					Unit = DefaultUnit;
			}

			Normalize();

			if (force)
			{
				if (Unit == Unit.None)
				{
					Unit = DefaultUnit;
					return;
				}
			}

			switch (target)
			{
				case Unit.None:
					Unit = target;
					break;

				case Unit.Centimeter:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= 2.54f;
					break;

				case Unit.Millimeter:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= 25.4f;
					break;

				case Unit.Twips:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= 1440f;
					break;

				case Unit.Picas:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= 6;
					break;

				case Unit.Point:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= 72;
					break;

				case Unit.View:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= Dpi;
					break;

				case Unit.Percent:
					if (Unit != Unit.None) break;
					Unit = target;
					break;

				case Unit.Line:
					if (!force || Unit != Unit.Inch) break;
					Unit = target;
					Number *= 6;
					break;

				case Unit.Pixel:
					if (Unit != Unit.Inch) break;
					Unit = target;
					Number *= 96;
					break;
			}
		}


		public void Normalize()
		{
			switch (Unit)
			{
				case Unit.None:
				case Unit.Inch:
				case Unit.Character:
				case Unit.Line:
					break;

				case Unit.Percent:
					break;

				case Unit.Mile:
					Number *= 1760*36;
					Unit = Unit.Inch;
					break;

				case Unit.Yard:
					Number *= 36;
					Unit = Unit.Inch;
					break;

				case Unit.Foot:
					Number *= 12;
					Unit = Unit.Inch;
					break;

				case Unit.Kilometer:
					Number *= 100000/2.54f;
					Unit = Unit.Inch;
					break;

				case Unit.Meter:
					Number *= 100/2.54f;
					Unit = Unit.Inch;
					break;

				case Unit.Centimeter:
					Number /= 2.54f;
					Unit = Unit.Inch;
					break;

				case Unit.Millimeter:
					Number /= 25.4f;
					Unit = Unit.Inch;
					break;

				case Unit.Fixed:
					Number /= 65536;
					Unit = Unit.None;
					break;

				case Unit.Emu:
					Number /= 914400;
					Unit = Unit.Inch;
					break;

				case Unit.Twips:
					Number /= 1440;
					Unit = Unit.Inch;
					break;

				case Unit.WordPerfect:
					Number /= 1200;
					Unit = Unit.Inch;
					break;

				case Unit.Printer:
					Number /= 100;
					Unit = Unit.Inch;
					break;

				case Unit.View:
					Number /= Dpi;
					Unit = Unit.Inch;
					break;

				case Unit.Picas:
					Number /= 6;
					Unit = Unit.Inch;
					break;

				case Unit.Point:
					Number /= 72;
					Unit = Unit.Inch;
					break;

				case Unit.Pixel:
					Number /= 96;
					Unit = Unit.Inch;
					break;
			}
		}

		#endregion

		#region Operator Overrides

		public override bool Equals(object obj)
		{
			if (obj is Quantity)
				return Equals((Quantity) obj);
			return false;
		}

		public bool Equals(Quantity q)
		{
			return this == q;
		}

		public override int GetHashCode()
		{
			Convert(Unit.View, false);
			return Number.GetHashCode() ^ Unit.GetHashCode();
		}

		#endregion

		#region Operators

		public static bool operator ==(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return q1.Number == q2.Number;
		}

		public static bool operator !=(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return q1.Number != q2.Number;
		}

		public static bool operator >(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return q1.Number > q2.Number;
		}

		public static bool operator <(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return q1.Number < q2.Number;
		}

		public static bool operator >=(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return q1.Number >= q2.Number;
		}

		public static bool operator <=(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return q1.Number <= q2.Number;
		}

		public static Quantity operator +(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return new Quantity(q1.Number + q2.Number, q1.Unit);
		}

		public static Quantity operator -(Quantity q1, Quantity q2)
		{
			q2.Convert(q1.Unit, true);
			return new Quantity(q1.Number - q2.Number, q1.Unit);
		}

		public static Quantity operator -(Quantity q)
		{
			return new Quantity(-q.Number, q.Unit);
		}

		public static Quantity operator *(Quantity q, double d)
		{
			return new Quantity((float) (q.Number*d), q.Unit);
		}

		public static Quantity operator /(Quantity q, double d)
		{
			return new Quantity((float) (q.Number/d), q.Unit);
		}

		public static Quantity operator %(Quantity q, double d)
		{
			return new Quantity((float) Math.IEEERemainder(q.Number, d), q.Unit);
		}

		#endregion

		#region IComparable<Quantity> Members

		public int CompareTo(Quantity quantity)
		{
			quantity.Convert(Unit, true);
			return Number.CompareTo(quantity.Number);
		}

		#endregion
	}
}