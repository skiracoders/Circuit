using Silk.NET.Maths;

namespace Skira
{
    public class World
    {
        private Cell[,] cells;
        private Vector2D<uint> size;
        public World(uint width, uint height)
        {
            size = new Vector2D<uint>(width, height);
            cells = new Cell[height, width];
            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    cells[y, x] = new Cell(y, x);
                }
            }
        }
        public void SetOccupant(uint y, uint x, Occupant occupant)
        {
            Cell cell = cells[y, x];
            occupant.Cell = cell;
            cell.Occupant = occupant;
        }
        public Cell GetNeighbour(Cell cell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (cell.Y != Height - 1)
                    {
                        return GameManager.Instance.World[cell.Y + 1u, cell.X];
                    }
                    break;
                case Direction.Right:
                    if (cell.X != Width - 1)
                    {
                        return GameManager.Instance.World[cell.Y, cell.X + 1u];
                    }
                    break;

                case Direction.Down:
                    if (cell.Y != 0)
                    {
                        return GameManager.Instance.World[cell.Y - 1u, cell.X];
                    }
                    break;

                case Direction.Left:
                    if (cell.X != 0)
                    {
                        return GameManager.Instance.World[cell.Y, cell.X - 1u];
                    }
                    break;
            }
            return null;
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