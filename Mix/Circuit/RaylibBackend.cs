using Raylib_cs;
using System;
using System.Threading;
using System.Numerics;
using System.Collections.Generic;
namespace Skira
{
    public class RaylibBackend
    {
        public event UpdateHandler Update;
        public event ScrolledHandler Scrolled;
        public event KeyPressedHandler KeyPressed;
        private string title;
        private Vector2<uint> windowSize;
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, Image> images;
        private Camera2D camera2D;
        private DateTime lastTime;
        private TimeSpan minimumTime;
        private bool shift, control, alt, system;
        public RaylibBackend(string title, uint width, uint height)
        {
            minimumTime = new TimeSpan(0, 0, 0, 0, 16);
            this.title = title;
            windowSize = new Vector2<uint>(width, height);
            textures = new Dictionary<string, Texture2D>();
            images = new Dictionary<string, Image>();
            AdjustView();
        }
        public void Run()
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow((int)windowSize.X, (int)windowSize.Y, title);
            Raylib.MaximizeWindow();
            foreach (KeyValuePair<string, Image> entry in images)
            {
                textures.Add(entry.Key, Raylib.LoadTextureFromImage(entry.Value));
            }
            camera2D = new Camera2D(new Vector2(Raylib.GetScreenWidth() * 1f / 2f, Raylib.GetScreenHeight() / 2f), new Vector2(0, 0), 0f, 1f);
            lastTime = DateTime.Now;
            while (!Raylib.WindowShouldClose())
            {
                double wheelDelta = Raylib.GetMouseWheelMove();
                if (wheelDelta != 0d)
                {
                    Scrolled?.Invoke(wheelDelta);
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT))
                {
                    alt = true;
                }
                else
                {
                    alt = false;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL))
                {
                    control = true;
                }
                else
                {
                    control = false;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT))
                {
                    control = true;
                }
                else
                {
                    control = false;
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_S))
                {
                    KeyPressed?.Invoke(Key.S, alt, control, shift);
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_D))
                {
                    KeyPressed?.Invoke(Key.D, alt, control, shift);
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_W))
                {
                    KeyPressed?.Invoke(Key.W, alt, control, shift);
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))
                {
                    KeyPressed?.Invoke(Key.A, alt, control, shift);
                }
                if (Raylib.IsWindowResized())
                {
                    camera2D.offset = new Vector2(Raylib.GetScreenWidth() * 1f / 2f, Raylib.GetScreenHeight() / 2f);
                }
                BackendUpdate();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);
                DrawWorld();
                Raylib.DrawText("Hello, world!", 12, 12, 32, Color.WHITE);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
        public void LoadSprites()
        {
            foreach (KeyValuePair<string, string> entry in GameManager.Instance.ImageManager.Paths)
            {
                images.Add(entry.Key, Raylib.LoadImage(entry.Value));
            }
        }
        private void DrawWorld()
        {
            Raylib.BeginMode2D(camera2D);
            Vector2d position = new Vector2d(0d, 0d);
            Texture2D texture;
            for (uint y = 0; y < GameManager.Instance.World.Height; y++)
            {
                position.Y = (double)y * GameManager.Instance.Camera.CellSize;
                for (uint x = 0; x < GameManager.Instance.World.Width; x++)
                {
                    Cell cell = GameManager.Instance.World[y, x];
                    texture = textures[cell.Ground.ImageName];
                    position.X = (double)x * GameManager.Instance.Camera.CellSize;
                    Raylib.DrawTexture(texture, (int)position.X, (int)position.Y, Color.WHITE);
                    Occupant occupant = cell.Occupant;
                    if (occupant != null)
                    {
                        texture = textures[cell.Occupant.ImageName];
                        Raylib.DrawTexture(texture, (int)position.X, (int)position.Y, Color.WHITE);
                    }
                }
            }
            Raylib.EndMode2D();
        }
        public void AdjustView()
        {
            camera2D.zoom = (float)GameManager.Instance.Camera.Zoom;
            camera2D.target = new Vector2(
                (float)GameManager.Instance.Camera.X * (float)GameManager.Instance.Camera.CellSize +
                (float)GameManager.Instance.Camera.CellSize / 2f,
                (float)GameManager.Instance.Camera.Y * (float)GameManager.Instance.Camera.CellSize +
                (float)GameManager.Instance.Camera.CellSize / 2f);
        }
        private void BackendUpdate()
        {
            TimeSpan timeSpan = DateTime.Now - lastTime;
            if (timeSpan < minimumTime)
            {
                Thread.Sleep(minimumTime - timeSpan);
                timeSpan = DateTime.Now - lastTime;
            }
            lastTime = DateTime.Now;
            Update?.Invoke(timeSpan);
        }
    }
}