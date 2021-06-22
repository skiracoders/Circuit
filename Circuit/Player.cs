using System;

namespace Skira
{
    public class Player : Occupant
    {
        private DateTime lastMoveTime;
        public Vector2d Position
        {
            get
            {
                if (Cell == null)
                {
                    return null;
                }
                return new Vector2d(Cell.Y, Cell.X);
            }
        }
        public TimeSpan MoveTimeSpan
        {
            get;
        }
        public DateTime LastMoveTime
        {
            get => lastMoveTime;
            set => lastMoveTime = value;
        }
        public Player() : base(typeof(Player), "Unit-0x1")
        {
            MoveTimeSpan = TimeSpan.FromMilliseconds(128d);
            lastMoveTime = DateTime.MinValue;
        }
    }
}