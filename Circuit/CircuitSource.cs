using System;
namespace Skira
{
    public abstract class CircuitSource : CircuitElement
    {
        public CircuitSource(Type type, string imageName, string flowImage) : base(type, imageName, flowImage)
        {
            connections = new bool[4] { true, true, true, true };
        }
    }
}