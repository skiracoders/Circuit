namespace Skira
{
    public class CellElement : Renderable
    {
        private Cell cell;
        public Cell Cell
        {
            get => cell;
            set => cell = value;
        }
        public CellElement(string image) : base(image)
        {
        }
        public CellElement(string image, Cell cell) : base(image)
        {
            this.cell = cell;
        }
    }
}