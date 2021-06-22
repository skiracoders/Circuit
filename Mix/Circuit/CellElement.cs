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
        public CellElement(string imageName, Cell cell) : base(imageName)
        {
            this.cell = cell;
        }
    }
}