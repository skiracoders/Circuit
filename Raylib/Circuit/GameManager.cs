using System.Numerics;
using System;
using Raylib_cs;

namespace Circuit
{
    public struct Vector2D
    {
        public double x, y;
        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2(Vector2D vector)
        {
            return vector.ToVector2();
        }
        public Vector2 ToVector2()
        {
            return new Vector2((float)x, (float)y);
        }
        public static Vector2D operator /(Vector2D vector, double d)
        {
            if (d == 0)
            {
                throw new DivideByZeroException();
            }
            return new Vector2D(vector.x / d, vector.y / d);
        }
    }
    public class GameManager
    {
        private Vector2D viewportSize;
        private Vector2D cameraPosition;
        private Camera2D camera2D;
        public GameManager()
        {
            viewportSize = new Vector2D(1024, 512);
            cameraPosition = new Vector2D(0, 0);
            camera2D = new Camera2D();
            camera2D.offset = viewportSize / 2;
            camera2D.target = cameraPosition;
            camera2D.rotation = 0f;
            camera2D.zoom = 1f;
        }
        public void Loop()
        {
            Raylib.InitWindow((int)viewportSize.x, (int)viewportSize.y, "Circuit");
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);
                Raylib.DrawText("Hello, world!", 512, 256, 32, Color.WHITE);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
    }
}