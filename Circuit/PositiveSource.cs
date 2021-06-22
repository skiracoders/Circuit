using System;
namespace Skira
{
    public class PositiveSource : CircuitSource
    {
        public PositiveSource() : base(typeof(PositiveSource), "Positive", "Positive-Flow")
        {
        }
    }
}