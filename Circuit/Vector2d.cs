using System;
namespace Skira
{
    public class Vector2d
    {
        public double Y, X;
        public Vector2d(double y, double x)
        {
            this.Y = y;
            this.X = x;
        }
        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.Y - b.Y, a.X - b.X);
        }
        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.Y + b.Y, a.X + b.X);
        }
        public static Vector2d operator *(Vector2d vector, double scalar)
        {
            return new Vector2d(vector.Y * scalar, vector.X * scalar);
        }
        public static Vector2d operator *(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.Y * b.Y, a.X * b.X);
        }
        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }
        public Vector2d Absoulte
        {
            get { return new Vector2d(Math.Abs(Y), Math.Abs(X)); }
        }
        public override string ToString()
        {
            return "Vector: [" + Y + ", " + X + "]";
        }
    }
}