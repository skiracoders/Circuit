using System;
namespace Skira
{
    public class NegativeSource : CircuitSource
    {
        public NegativeSource() : base(typeof(NegativeSource), "Negative", "Negative-Flow")
        {
        }
    }
}