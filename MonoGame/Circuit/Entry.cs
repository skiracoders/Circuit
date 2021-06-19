using System;

namespace Circuit
{
    public static class Entry
    {
        [STAThread]
        static void Main()
        {
            GameManager gameManager = new GameManager();
            gameManager.Run();
        }
    }
}
