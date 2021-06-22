namespace Skira
{
    public class Cell
    {
        Renderable ground;
        Occupant occupant;
        Vector2<uint> position;
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
            position = new Vector2<uint>(x, y);
            ground = new CellElement("Ground0x1", this);
            occupant = null;
        }
        public Cell(Renderable ground, Occupant occupant)
        {
            this.ground = ground;
            this.occupant = occupant;
        }
    }
}
