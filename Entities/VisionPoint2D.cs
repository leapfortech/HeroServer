using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class VisionPoint2D
    {
#pragma warning disable IDE1006
        public float x { get; set; }
        public float y { get; set; }
#pragma warning restore IDE1006

        public VisionPoint2D()
        {
        }

        public VisionPoint2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // Infinity
        public static readonly VisionPoint2D Zero = new VisionPoint2D(0f, 0f);
        public static readonly VisionPoint2D One = new VisionPoint2D(1f, 1f);
        public static readonly VisionPoint2D PositiveInfinity = new VisionPoint2D(float.PositiveInfinity, float.PositiveInfinity);
        public static readonly VisionPoint2D NegativeInfinity = new VisionPoint2D(float.NegativeInfinity, float.NegativeInfinity);
        public bool IsPositiveInfinity() => float.IsPositiveInfinity(x);
        public bool IsNegativeInfinity() => float.IsNegativeInfinity(x);
        public bool IsInfinity() => float.IsInfinity(x);
        public bool IsIndefined() => float.IsInfinity(x);

        // Operations
        public static VisionPoint2D operator -(VisionPoint2D v)
        {
            return new VisionPoint2D(-v.x, -v.y);
        }

        public static VisionPoint2D operator +(VisionPoint2D v1, VisionPoint2D v2)
        {
            return new VisionPoint2D(v1.x + v2.x, v1.y + v2.y);
        }

        public static VisionPoint2D operator -(VisionPoint2D v1, VisionPoint2D v2)
        {
            return new VisionPoint2D(v1.x - v2.x, v1.y - v2.y);
        }

        public static VisionPoint2D operator *(float k, VisionPoint2D v)
        {
            return new VisionPoint2D(k * v.x, k * v.y);
        }

        public static VisionPoint2D operator *(VisionPoint2D v, float k)
        {
            return new VisionPoint2D(v.x * k, v.y * k);
        }

        public static VisionPoint2D operator *(VisionPoint2D v1, VisionPoint2D v2)
        {
            return new VisionPoint2D(v1.x * v2.x, v1.y * v2.y);
        }

        public static VisionPoint2D operator /(VisionPoint2D v, float k)
        {
            return new VisionPoint2D(v.x / k, v.y / k);
        }

        // Transformations
        [JsonIgnore]
        public VisionPoint2D Normalized
        {
            get { float magnitude = Magnitude; return new VisionPoint2D(x / magnitude, y / magnitude); }
        }

        [JsonIgnore]
        public float Magnitude => (float)Math.Sqrt(x * x + y * y);

        public float Cross(VisionPoint2D v)
        {
            return x * v.y - y * v.x;
        }

        public VisionPoint2D Rotate(float cosa, float sina)
        {
            return new VisionPoint2D(x * cosa - y * sina, x * sina + y * cosa);
        }

        public VisionPoint2D Rotate(float cosa, float sina, VisionPoint2D center)
        {
            return (this - center).RotateAdd(cosa, sina, center);
        }

        public VisionPoint2D RotateAdd(float cosa, float sina, VisionPoint2D d)
        {
            return new VisionPoint2D(x * cosa - y * sina + d.x, x * sina + y * cosa + d.y);
        }

        // Intersections
        public static bool IxLines(VisionPoint2D A1, VisionPoint2D A2, VisionPoint2D B1, VisionPoint2D B2, out VisionPoint2D P, bool segment = false)
        {
            VisionPoint2D VA = A2 - A1;
            VisionPoint2D VB = B2 - B1;

            float q = VA.Cross(VB);
            if (Math.Abs(q) < 0.000001f)   // Parallel
            {
                P = VisionPoint2D.PositiveInfinity;
                return false;
            }

            float t = (A1.Cross(VB) + VB.Cross(B1)) / q;
            P = A1 + t * VA;

            if (segment)
            {
                if (t < 0f || t > 1f)
                    return false;

                float u = (A1.Cross(VA) + VA.Cross(B1)) / q;
                if (u < 0f || u > 1f)
                    return false;
            }

            return true;
        }

        public static bool IxSegments(VisionPoint2D A1, VisionPoint2D A2, VisionPoint2D B1, VisionPoint2D B2, out VisionPoint2D P)
        {
            VisionPoint2D VA = A2 - A1;
            VisionPoint2D VB = B2 - B1;
            float detAB = VA.Cross(VB);

            if (Math.Abs(detAB) < 0.000001f)  // Parallel
            {
                P = VisionPoint2D.PositiveInfinity;
                return false;
            }

            VisionPoint2D VC = B1 - A1;
            float t = VC.Cross(VB) / detAB;
            P = A1 + t * VA;

            if (t < 0f || t > 1f)
                return false;

            float u = VC.Cross(VA) / detAB;
            if (u < 0f || u > 1f)
                return false;

            return true;
        }

        private static bool IxSegmentDist(float dst1, float dst2, VisionPoint2D S1, VisionPoint2D S2, out VisionPoint2D P)
        {
            if (dst1 * dst2 >= 0.0f || dst1 == dst2)
            {
                P = VisionPoint2D.NegativeInfinity;
                return false;
            }

            P = S1 + (S2 - S1) * dst1 / (dst1 - dst2);
            return true;
        }

        public static bool IxSegmentBBox(VisionPoint2D S1, VisionPoint2D S2, VisionPoint2D BB1, VisionPoint2D BB2, out VisionPoint2D P)
        {
            if ((S2.x < BB1.x && S1.x < BB1.x) || (S2.x > BB2.x && S1.x > BB2.x) || (S2.y < BB1.y && S1.y < BB1.y) || (S2.y > BB2.y && S1.y > BB2.y))
            {
                P = VisionPoint2D.PositiveInfinity;
                return false;
            }

            if (S1.x > BB1.x && S1.x < BB2.x && S1.y > BB1.y && S1.y < BB2.y)
            {
                P = S1;
                return false;
            }

            if (IxSegmentDist(S1.x - BB1.x, S2.x - BB1.x, S1, S2, out P) && P.y > BB1.y && P.y < BB2.y)
                return true;
            if (IxSegmentDist(S1.y - BB1.y, S2.y - BB1.y, S1, S2, out P) && P.x > BB1.x && P.x < BB2.x)
                return true;
            if (IxSegmentDist(S1.x - BB2.x, S2.x - BB2.x, S1, S2, out P) && P.y > BB1.y && P.y < BB2.y)
                return true;
            if (IxSegmentDist(S1.y - BB2.y, S2.y - BB2.y, S1, S2, out P) && P.x > BB1.x && P.x < BB2.x)
                return true;

            return false;
        }
    }
}