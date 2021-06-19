namespace Skira
{
    public class World
    {
        private Cell[,] cells;
        private Vector2<uint> size;
        public World(uint width, uint height)
        {
            size = new Vector2<uint>(width, height);
            cells = new Cell[height, width];
            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    cells[y, x] = new Cell(y, x);
                }
            }
        }
        public uint Width
        {
            get { return size.X; }
        }
        public uint Height
        {
            get { return size.Y; }
        }
        public Cell this[uint y, uint x]
        {
            get { return cells[y, x]; }
        }
    }
}