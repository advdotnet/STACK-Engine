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
        const double EPSILON = 1e-13;

        float M11, M12, M13;
        float M21, M22, M23;
        float M31, M32, M33;

        Projection2D Inverse;

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

        void SetMatrix(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            M11 = m11; M12 = m12; M13 = m13;
            M21 = m21; M22 = m22; M23 = m23;
            M31 = m31; M32 = m32; M33 = m33;
        }

        private Projection2D Adjugate()
        {
            float m11 = Det2(M22, M23, M32, M33);
            float m12 = Det2(M32, M33, M12, M13);
            float m13 = Det2(M12, M13, M22, M23);

            float m21 = Det2(M23, M21, M33, M31);
            float m22 = Det2(M33, M31, M13, M11);
            float m23 = Det2(M13, M11, M23, M21);

            float m31 = Det2(M21, M22, M31, M32);
            float m32 = Det2(M31, M32, M11, M12);
            float m33 = Det2(M11, M12, M21, M22);

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
            Inverse = Inverse ?? Adjugate();
            return Inverse.Transform(vector);
        }

        /// <summary>
        /// Transforms a vector from the unit square space to the quadliteral space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2 Transform(Vector2 vector)
        {
            float nx = M11 * vector.X + M21 * vector.Y + M31;
            float ny = M12 * vector.X + M22 * vector.Y + M32;
            float w = M13 * vector.X + M23 * vector.Y + M33;

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
            float dx0 = a.X;
            float dx1 = b.X;
            float dx2 = c.X;
            float dx3 = d.X;

            float dy0 = a.Y;
            float dy1 = b.Y;
            float dy2 = c.Y;
            float dy3 = d.Y;

            double ax = dx0 - dx1 + dx2 - dx3;
            double ay = dy0 - dy1 + dy2 - dy3;

            if ((ax < EPSILON && ax > -EPSILON) &&
                (ay < EPSILON && ay > -EPSILON))
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

                double gtop = ax * ay2 - ax2 * ay;
                double htop = ax1 * ay - ax * ay1;
                double bottom = ax1 * ay2 - ax2 * ay1;

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
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match
            return a.M11 == b.M11 && a.M12 == b.M12 && a.M13 == b.M13 &&
                   a.M21 == b.M21 && a.M22 == b.M22 && a.M23 == b.M23 &&
                   a.M31 == b.M31 && a.M32 == b.M32 && a.M33 == b.M33;
        }

        public static bool operator !=(Projection2D a, Projection2D b)
        {
            return !(a == b);
        }

        public override bool Equals(object other)
        {
            Projection2D Projection = (Projection2D)other;

            if (Projection == null)
            {
                return false;
            }

            return (Projection == this);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int Result = 17;

                var Elements = new float[]
                {
                    M11, M12, M13,
                    M21, M22, M23,
                    M31, M32, M33
                };

                for (int i = 0; i < Elements.Length; i++)
                {
                    Result = Result * 23 + Elements[i].GetHashCode();
                }

                return Result;
            }
        }

        public static Projection2D Zero = new Projection2D();
    }
}
