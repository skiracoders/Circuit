using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;

namespace Skira
{
    public class SFMLBackend
    {
        public event UpdateHandler Update;
        public event ScrolledHandler Scrolled;
        public event KeyPressedHandler KeyPressed;
        private bool mustDraw;
        private Vector2d viewSize;
        private Vector2d guiViewSize;
        private Vector2d guiViewCenter;
        private Vector2d proportion;
        private Vector2<uint> windowSize;
        private string title;
        private RenderWindow renderWindow;
        private Font font;
        private View view;
        private View guiView;
        private Dictionary<string, Texture> textures;
        private Dictionary<string, Sprite> sprites;
        private DateTime lastTime;
        private TimeSpan minimumTime;
        private Text text;
        RectangleShape guiClearRectangle;
        public SFMLBackend(string title, uint width, uint height)
        {
            minimumTime = new TimeSpan(0, 0, 0, 0, 16);
            this.title = title;
            viewSize = new Vector2d(64d, 64d);
            guiViewSize = new Vector2d(1d, 1d) * 2048d;
            guiViewCenter = guiViewSize * (1d / 2d);
            Vector2f guiViewSizeV2f = new Vector2f((float)guiViewSize.X, (float)guiViewSize.Y);
            guiView = new View(new Vector2f((float)guiViewCenter.X, (float)guiViewCenter.Y), guiViewSizeV2f);
            proportion = new Vector2d(1d, 2d);
            guiClearRectangle = new RectangleShape(guiViewSizeV2f);
            guiClearRectangle.FillColor = Color.White;
            font = new Font(Path.Combine("Content", "Fonts", "ShareTechMono.ttf"));
            text = new Text("Welcome to GUI", font, 64);
            text.FillColor = Color.Black;
            textures = new Dictionary<string, Texture>();
            sprites = new Dictionary<string, Sprite>();
        }
        public void Run()
        {
            renderWindow = new RenderWindow(new VideoMode(1280u, 720u), title, Styles.Default);
            //renderWindow = new RenderWindow(VideoMode.DesktopMode, title, Styles.None);
            AdjustView();
            //AdjustRenderSprite(renderWindow.Size.X, renderWindow.Size.Y);
            renderWindow.KeyPressed += BackendKeyPressed;
            renderWindow.Closed += BackendClosed;
            renderWindow.Resized += BackendResized;
            renderWindow.MouseWheelScrolled += BackendScrolled;
            //Text text = new Text("Welcome Circuit", font, 32);
            //text.FillColor = Color.Black;
            mustDraw = true;
            lastTime = DateTime.Now;
            while (renderWindow.IsOpen)
            {
                //Thread.Sleep(1500);
                renderWindow.DispatchEvents();
                BackendUpdate();
                if (mustDraw)
                {
                    mustDraw = false;
                    BackendDraw();
                }
            }
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
        public void LoadSprites()
        {
            foreach (KeyValuePair<string, string> entry in GameManager.Instance.ImageManager.Paths)
            {
                textures.Add(entry.Key, new Texture(entry.Value));
                sprites.Add(entry.Key, new Sprite(textures[entry.Key]));
            }
        }
        public void RequestDrawing()
        {
            mustDraw = true;
        }
        public void AdjustView()
        {
            windowSize = new Vector2<uint>(renderWindow.Size.X, renderWindow.Size.Y);
            double windowHeight = windowSize.Y;
            double windowWidth = windowSize.X;
            double width, height, difference, half, scale;
            float zoom = (float)GameManager.Instance.Camera.Zoom;
            Vector2f size = new Vector2f((float)viewSize.X * zoom, (float)viewSize.Y * zoom);
            FloatRect viewport;
            FloatRect guiVieport;
            if (windowHeight > (proportion.Y / proportion.X) * windowWidth)
            {
                width = windowWidth;
                height = width * (proportion.Y / proportion.X);
                difference = windowHeight - height;
                half = difference / 2d;
                scale = height / windowHeight;
                double top = half / windowHeight;
                viewport = new FloatRect(0f, (float)top, 1f / 2f, (float)scale);
                guiVieport = new FloatRect(1f / 2f, (float)top, 1f / 2f, (float)scale);
            }
            else
            {
                height = windowHeight;
                width = height * (proportion.X / proportion.Y);
                difference = windowWidth - width;
                half = difference / 2d;
                scale = width / windowWidth;
                double left = half / windowWidth;
                viewport = new FloatRect((float)left, 0f, (float)scale / 2f, 1f);
                guiVieport = new FloatRect((float)left + (float)scale / 2f, 0f, (float)scale / 2f, 1f);
            }
            Vector2f center = new Vector2f((float)GameManager.Instance.Camera.X * (float)GameManager.Instance.Camera.CellSize + (float)GameManager.Instance.Camera.CellSize / 2f,
                (float)GameManager.Instance.Camera.Y * (float)GameManager.Instance.Camera.CellSize + (float)GameManager.Instance.Camera.CellSize / 2f);
            view = new View(center, size);
            //view = new View(new FloatRect())
            guiView.Viewport = guiVieport;
            view.Viewport = viewport;
            mustDraw = true;
        }
        private void DrawWorld()
        {
            renderWindow.SetView(view);
            Vector2f position = new Vector2f();
            Sprite sprite;
            for (uint y = 0; y < GameManager.Instance.World.Height; y++)
            {
                position.Y = (float)y * (float)GameManager.Instance.Camera.CellSize;
                for (uint x = 0; x < GameManager.Instance.World.Width; x++)
                {
                    Cell cell = GameManager.Instance.World[y, x];
                    sprite = sprites[cell.Ground.ImageName];
                    position.X = (float)x * (float)GameManager.Instance.Camera.CellSize;
                    sprite.Position = position;
                    renderWindow.Draw(sprite);
                    Occupant occupant = cell.Occupant;
                    if (occupant != null)
                    {
                        sprite = sprites[cell.Occupant.ImageName];
                        sprite.Position = position;
                        renderWindow.Draw(sprite);
                    }
                }
            }
        }
        private void DrawGUI()
        {
            renderWindow.SetView(guiView);
            renderWindow.Draw(guiClearRectangle);
            renderWindow.Draw(text);
        }
        private void BackendDraw()
        {
            renderWindow.Clear(Color.Black);
            DrawWorld();
            DrawGUI();
            renderWindow.Display();
        }
        private void BackendResized(object sender, SizeEventArgs sizeEventArgs)
        {
            RenderWindow window = (RenderWindow)sender;
            AdjustView();
            mustDraw = true;
        }
        private void BackendScrolled(object sender, MouseWheelScrollEventArgs mouseWheelScrollEventArgs)
        {
            Scrolled?.Invoke(mouseWheelScrollEventArgs.Delta);
        }
        private void BackendClosed(object sender, EventArgs eventArgs)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
        private void BackendKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            KeyPressed?.Invoke((Key)keyEventArgs.Code, keyEventArgs.Alt, keyEventArgs.Control, keyEventArgs.Shift, keyEventArgs.System);
            /*
            RenderWindow window = (RenderWindow)sender;
            if (keyEventArgs.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
            */
        }
    }
}