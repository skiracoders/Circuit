using System.IO;
using System.Drawing;

namespace Circuit
{
    public class Cell
    {
        
    }
    public class World
    {
        private Cell[][] cells;
    }
    public class GameManager
    {
        private Bitmap bitmap;
        private MonoGameBackend backend;
        public GameManager()
        {
            bitmap = new Bitmap(Path.Combine("Content", "Unit0x1.png"));
            bitmap.Save("bitmap.png");
            backend = new MonoGameBackend();
        }
        public void Run()
        {
            backend.Run();
        }
    }
}