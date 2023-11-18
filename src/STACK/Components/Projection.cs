using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Represents a projection matrix that maps the unit square to a given quadliteral.
	/// Used for perspective projections.
	/// </summary>
	[Serializable]
	public class Projection2D
	{
		private const double _epsilon = 1e-13;
		private float _m11, _m12, _m13;
		private float _m21, _m22, _m23;
		private float _m31, _m32, _m33;
		private Projection2D _inverse;

		public Projection2D()
		{
			SetMatrix(0, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		public Projection2D(
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33)
		{
			SetMatrix(m11, m12, m13, m21, m22, m23, m31, m32, m33);
		}

		private void SetMatrix(
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33)
		{
			_m11 = m11; _m12 = m12; _m13 = m13;
			_m21 = m21; _m22 = m22; _m23 = m23;
			_m31 = m31; _m32 = m32; _m33 = m33;
		}

		private Projection2D Adjugate()
		{
			var m11 = Det2(_m22, _m23, _m32, _m33);
			var m12 = Det2(_m32, _m33, _m12, _m13);
			var m13 = Det2(_m12, _m13, _m22, _m23);

			var m21 = Det2(_m23, _m21, _m33, _m31);
			var m22 = Det2(_m33, _m31, _m13, _m11);
			var m23 = Det2(_m13, _m11, _m23, _m21);

			var m31 = Det2(_m21, _m22, _m31, _m32);
			var m32 = Det2(_m31, _m32, _m11, _m12);
			var m33 = Det2(_m11, _m12, _m21, _m22);

			return new Projection2D(
				m11, m12, m13,
				m21, m22, m23,
				m31, m32, m33);
		}

		/// <summary>
		/// Transforms a vector from the quadliteral space to the unit square space.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public Vector2 TransformInverse(Vector2 vector)
		{
			_inverse = _inverse ?? Adjugate();
			return _inverse.Transform(vector);
		}

		/// <summary>
		/// Transforms a vector from the unit square space to the quadliteral space.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public Vector2 Transform(Vector2 vector)
		{
			var nx = _m11 * vector.X + _m21 * vector.Y + _m31;
			var ny = _m12 * vector.X + _m22 * vector.Y + _m32;
			var w = _m13 * vector.X + _m23 * vector.Y + _m33;

			if (w != 0)
			{
				nx /= w;
				ny /= w;
			}

			return new Vector2(nx, ny);
		}

		private static float Det2(float a, float b, float c, float d)
		{
			return a * d - b * c;
		}

		/// <summary>
		/// Counterclockwise
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		public Projection2D SetQuadliteral(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
		{
			var dx0 = a.X;
			var dx1 = b.X;
			var dx2 = c.X;
			var dx3 = d.X;

			var dy0 = a.Y;
			var dy1 = b.Y;
			var dy2 = c.Y;
			var dy3 = d.Y;

			double ax = dx0 - dx1 + dx2 - dx3;
			double ay = dy0 - dy1 + dy2 - dy3;

			if ((ax < _epsilon && ax > -_epsilon) &&
				(ay < _epsilon && ay > -_epsilon))
			{
				SetMatrix(
					dx1 - dx0, dy1 - dy0, 0,
					dx2 - dx1, dy2 - dy1, 0,
					dx0, dy0, 1);
			}
			else
			{
				double ax1 = dx1 - dx2;
				double ax2 = dx3 - dx2;
				double ay1 = dy1 - dy2;
				double ay2 = dy3 - dy2;

				var gtop = ax * ay2 - ax2 * ay;
				var htop = ax1 * ay - ax * ay1;
				var bottom = ax1 * ay2 - ax2 * ay1;

				float ma, mb, mc, md, me, mf, mg, mh, mi;

				if (bottom == 0)
				{
					SetMatrix(0, 0, 0, 0, 0, 0, 0, 0, 0);
				}

				mg = (float)(gtop / bottom);
				mh = (float)(htop / bottom);
				mi = 1f;

				ma = dx1 - dx0 + mg * dx1;
				mb = dx3 - dx0 + mh * dx3;
				mc = dx0;
				md = dy1 - dy0 + mg * dy1;
				me = dy3 - dy0 + mh * dy3;
				mf = dy0;

				SetMatrix(
					ma, md, mg,
					mb, me, mh,
					mc, mf, mi);
			}

			return this;
		}

		public static bool operator ==(Projection2D a, Projection2D b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if ((a is null) || (b is null))
			{
				return false;
			}

			// Return true if the fields match
			return a._m11 == b._m11 && a._m12 == b._m12 && a._m13 == b._m13 &&
				   a._m21 == b._m21 && a._m22 == b._m22 && a._m23 == b._m23 &&
				   a._m31 == b._m31 && a._m32 == b._m32 && a._m33 == b._m33;
		}

		public static bool operator !=(Projection2D a, Projection2D b)
		{
			return !(a == b);
		}

		public override bool Equals(object other)
		{
			var projection = (Projection2D)other;

			if (projection == null)
			{
				return false;
			}

			return (projection == this);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = 17;

				var elements = new float[]
				{
					_m11, _m12, _m13,
					_m21, _m22, _m23,
					_m31, _m32, _m33
				};

				for (var i = 0; i < elements.Length; i++)
				{
					result = result * 23 + elements[i].GetHashCode();
				}

				return result;
			}
		}

		public readonly static Projection2D Zero = new Projection2D();
	}
}
