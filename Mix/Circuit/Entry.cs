using System;

namespace Skira
{
    public static class Entry
    {
        static void Main()
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.Initialize();
            gameManager.Run();
        }
    }
}
