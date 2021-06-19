using Microsoft.Xna.Framework;
using System;

namespace Circuit
{
    public struct Vector2D
    {
        public double x, y;
        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2(Vector2D vector)
        {
            return vector.ToVector2();
        }
        public Vector2 ToVector2()
        {
            return new Vector2((float)x, (float)y);
        }
        public static Vector2D operator /(Vector2D vector, double d)
        {
            if (d == 0)
            {
                throw new DivideByZeroException();
            }
            return new Vector2D(vector.x / d, vector.y / d);
        }
    }
}