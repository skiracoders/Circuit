using System;
namespace Skira
{
    public delegate void ScrolledHandler(double delta);
    public delegate void KeyPressedHandler(Key key, bool alt, bool control, bool shift, bool system);
    public delegate void UpdateHandler(TimeSpan timeSpan);
}