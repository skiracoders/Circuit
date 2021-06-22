using System;
namespace Skira
{
    public abstract class CircuitElement : Occupant
    {
        protected bool[] connections;
        string flowImage;
        public CircuitElement(Type type, string image, string flowImage) : base(type, image)
        {
            this.flowImage = flowImage;
        }
    }
}
