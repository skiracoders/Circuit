using System;


namespace Skira
{
    public class Occupant : CellElement
    {
        Type type;
        public Type Type
        {
            get { return type; }
        }
        public Occupant(Type type, string imageName, Cell cell) : base(imageName, cell)
        {
            this.type = type;
        }
    }
}