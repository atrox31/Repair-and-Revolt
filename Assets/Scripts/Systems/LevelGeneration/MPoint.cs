using System;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class MPoint
    {
        public int x { get; set; }
        public int y { get; set; }
        public MPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(MPoint mPoint)
        {
            return (float)Math.Sqrt(Math.Pow(this.x - mPoint.x, 2) + Math.Pow(this.y - mPoint.y, 2));
        }

        public static MPoint operator +(MPoint a, MPoint b)
        {
            return new MPoint(a.x + b.x, a.y + b.y);
        }
        public static MPoint operator -(MPoint a, MPoint b)
        {
            return new MPoint(a.x - b.x, a.y - b.y);
        }
        public static bool operator ==(MPoint a, MPoint b)
        {
            return (a.x == b.x && a.y == b.y);
        }
        public static bool operator !=(MPoint a, MPoint b)
        {
            return (a.x != b.x && a.y != b.y);
        }

        public override bool Equals(object obj)
        {
            return obj is MPoint point &&
                   x == point.x &&
                   y == point.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}