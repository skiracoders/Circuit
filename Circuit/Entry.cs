namespace Skira
{
    public class Entry
    {
        public static void Main(string[] args)
        {
            using (GameManager gameManager = GameManager.Instance)
            {
                gameManager.Initialize();
                gameManager.Run();
            }
        }
    }
}