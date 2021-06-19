using System;
using Raylib_cs;

namespace Circuit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GameManager gameManager = new GameManager();
            gameManager.Loop();
        }
    }
}
