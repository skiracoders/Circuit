using System;

namespace Skira
{
    public class Player : Occupant
    {
        public Vector2d Position
        {
            get
            {
                return new Vector2d(Cell.Y, Cell.X);
            }
        }
        public Player(Type type, string image, Cell cell) : base(type, image, cell)
        {

        }
    }
}