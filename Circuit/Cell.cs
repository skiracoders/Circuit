using Silk.NET.Maths;
using System;

namespace Skira
{
    public class Cell
    {
        Renderable ground;
        Occupant occupant;
        Vector2D<uint> position;
        public uint X
        {
            get => position.X;
        }
        public uint Y
        {
            get => position.Y;
        }
        public Occupant Occupant
        {
            get => occupant;
            set => occupant = value;
        }
        public Renderable Ground
        {
            get => ground;
        }
        public Cell(uint y, uint x)
        {
            position = new Vector2D<uint>(x, y);
            ground = new CellElement("Ground-0x1", this);
            occupant = null;
        }
        public Cell(Renderable ground, Occupant occupant)
        {
            this.ground = ground;
            this.occupant = occupant;
        }
        public Occupant Remove()
        {
            Occupant occupant = Occupant;
            Occupant = null;
            return occupant;
        }
        public void Place(Type type)
        {
            if (type == typeof(PositiveSource))
            {
                Occupant = new PositiveSource();
                return;
            }
            if (type == typeof(NegativeSource))
            {
                Occupant = new NegativeSource();
                return;
            }
            if (type == typeof(Player))
            {
                Occupant = new Player();
                return;
            }
        }
    }
}
