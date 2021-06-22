using System;
using System.Threading;
using System.Runtime.InteropServices;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
namespace Skira
{
    public sealed class GameManager : IDisposable
    {
        private static GameManager instance = null;
        private static readonly object padlock = new object();
        public static GameManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameManager();
                    }
                    return instance;
                }
            }
        }
        private GameManager()
        {
        }
        private IWindow window;
        private GL gl;
        public GL GL
        {
            get => gl;
        }
        private GameInterface gameInterface;
        private ImageManager imageManager;
        private uint texture, framebuffer;
        private Vector2D<int> size;
        public Vector2D<int> Size
        {
            get => size;
        }

        private double proportion;
        private System.Drawing.Rectangle bounds;
        private Bitmap bitmap;
        private byte[] pixels;
        private IMonitor monitor;
        private IWindowPlatform platform;
        private World world;
        public World World
        {
            get => world;
        }
        private Camera camera;
        private Player player;
        public ImageManager ImageManager
        {
            get => imageManager;
        }
        public Player Player
        {
            get => player;
        }
        private DateTime lastTime;
        private TimeSpan minimumTime;
        System.Drawing.Rectangle rectangle;
        private ImageAttributes defualtAtrributes;
        public ImageAttributes DefualtAtrributes
        {
            get => defualtAtrributes;
        }

        private ImageAttributes darkAttributes;
        public ImageAttributes DarkAttributes
        {
            get => darkAttributes;
        }
        private ImageAttributes brightAttributes;
        public ImageAttributes BrightAttributes
        {
            get => brightAttributes;
        }

        private ImageAttributes CreateAttributes(float red, float green, float blue, float alpha)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            float[][] elements = new float[5][];
            for (int i = 0; i < 5; i++)
            {
                elements[i] = new float[5];
                for (int j = 0; j < 5; j++)
                {
                    elements[i][j] = 0;
                }
            }
            elements[0][0] = red;
            elements[1][1] = green;
            elements[2][2] = blue;
            elements[3][3] = alpha;
            elements[4][4] = 1;
            imageAttributes.SetColorMatrix(new ColorMatrix(elements));
            return imageAttributes;
        }
        private void CreateImageAttributes()
        {
            darkAttributes = CreateAttributes(1f / 2f, 1f / 2f, 1f / 2f, 1f);
            defualtAtrributes = CreateAttributes(1f, 1f, 1f, 1f);
            brightAttributes = CreateAttributes(2f, 2f, 2f, 1f);
        }
        public void Initialize()
        {
            CreateImageAttributes();
            minimumTime = TimeSpan.FromMilliseconds(16d);
            size = new Vector2D<int>(32 * 12, 32 * 6);
            bounds = new System.Drawing.Rectangle();
            bitmap = new Bitmap(size.X, size.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            rectangle = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var length = bitmapData.Stride * bitmapData.Height;
            //Console.WriteLine("bitmapData.Stride");
            //Console.WriteLine(bitmapData.Stride / bitmap.Width);
            bitmap.UnlockBits(bitmapData);
            pixels = new byte[length];
            FillPixels();
            imageManager = new ImageManager();
            imageManager.Initialize();
            //Console.WriteLine(imageManager["Ground0x1"]);
            world = new World(16u, 8u);
            player = new Player();
            world.SetOccupant(0u, 0u, player);
            world.SetOccupant(0u, 8u, new PositiveSource());
            camera = new Camera(32d, 0d, 0d);
            gameInterface = new GameInterface();
            gameInterface.Revalidate(player.Cell);
            platform = Window.GetWindowPlatform(false);
            Console.WriteLine(platform);
            monitor = platform.GetMainMonitor();
            //Console.WriteLine(monitor.VideoMode.Resolution);
            //Create a window.

            WindowOptions options = WindowOptions.Default;
            //options.WindowState = WindowState.Maximized;
            options.WindowBorder = WindowBorder.Resizable;
            //options.Size = monitor.VideoMode.Resolution.Value;
            options.Size = monitor.VideoMode.Resolution.Value * 3 / 4;
            options.Title = "Circuit";
            window = Window.Create(options);

            AdjustView();

            //Assign events.
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += Closing;
            window.Resize += Resize;
        }
        private void Resize(Vector2D<int> newSize)
        {
            //Console.WriteLine(window.Size);
            //Console.WriteLine(newSize);
            AdjustView();
        }
        private void FillPixels()
        {
            var bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            // Copy bitmap to pixels[]
            Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
        }
        private unsafe void UpdateTexture()
        {
            fixed (void* pixelsPointer = &pixels[0])
            {
                gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (uint)size.X, (uint)size.Y, Silk.NET.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
                    pixelsPointer);
            }
        }
        public void Run()
        {
            //Run the window.
            window.Run();
        }
        private unsafe void OnLoad()
        {
            window.Position = monitor.VideoMode.Resolution.Value / 2 - window.Size / 2;
            //Set-up input context.
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Scroll += Scroll;
                input.Mice[i].MouseDown += MouseDown;
            }
            //Getting the opengl api for drawing to the screen.
            gl = GL.GetApi(window);
            //imageManager.LoadTexturesAndFramebuffers();
            framebuffer = gl.GenFramebuffer();
            //Console.WriteLine("GenFramebuffer {0}", gl.GetError());
            texture = gl.GenTexture();
            //Console.WriteLine("GenTexture {0}", gl.GetError());
            gl.BindTexture(TextureTarget.Texture2D, texture);
            //Console.WriteLine("BindTexture {0}", gl.GetError());
            fixed (void* pixelsPointer = &pixels[0])
            {
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)size.X,
                    (uint)size.Y, 0, Silk.NET.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, pixelsPointer);
            }
            //Console.WriteLine("TexSubImage2D {0}", gl.GetError());
            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, framebuffer);
            //Console.WriteLine("BindFramebuffer {0}", gl.GetError());
            gl.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
            //Console.WriteLine("FramebufferTexture2D {0}", gl.GetError());

        }
        public void DrawImageWithAttributes(Graphics graphics, ImagePack imagePack, ImageAttributes imageAttributes, System.Drawing.Rectangle bounds)
        {
            graphics.DrawImage(imagePack.Bitmap, bounds, 0, 0, imagePack.Bitmap.Width, imagePack.Bitmap.Height, GraphicsUnit.Pixel, imageAttributes);
        }
        private void DrawInterface(Graphics graphics)
        {
            gameInterface.Draw(graphics);
        }
        private void DrawWorld(Graphics graphics)
        {
            ImagePack imagePack;
            Point point = new Point();
            for (uint y = 0; y < world.Height; y++)
            {
                for (uint x = 0; x < world.Width; x++)
                {
                    Cell cell = world[y, x];
                    imagePack = imageManager[cell.Ground.Image];
                    point.X = (int)(x * camera.CellSize - camera.X / 1d + size.X / 2d - camera.CellSize / 2d);
                    point.Y = (int)(y * camera.CellSize - camera.Y / 1d + size.Y / 2d - camera.CellSize / 2d);
                    graphics.DrawImage(imagePack.Bitmap, point);
                    Occupant occupant = cell.Occupant;
                    if (occupant != null)
                    {
                        imagePack = imageManager[cell.Occupant.Image];
                        graphics.DrawImage(imagePack.Bitmap, point);
                    }
                }
            }
        }
        private void OnRender(double obj)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.DarkGoldenrod);
                DrawWorld(graphics);
                DrawInterface(graphics);
            }
            FillPixels();
            UpdateTexture();
            //Here all rendering should be done.
            gl.ClearColor(0f, 0f, 0f, 1f);
            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, framebuffer);
            gl.BlitFramebuffer(0, 0, size.X, size.Y, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height,
                ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0u);
        }
        private void OnUpdate(double obj)
        {
            //Here all updates to the program should be done.
            TimeSpan timeSpan = DateTime.Now - lastTime;
            if (timeSpan < minimumTime)
            {
                Thread.Sleep(minimumTime - timeSpan);
                timeSpan = DateTime.Now - lastTime;
            }
            lastTime = DateTime.Now;
            camera.Update(timeSpan);
        }
        private void TryPlayerMove(Direction direction)
        {
            if (DateTime.Now - player.LastMoveTime > player.MoveTimeSpan)
            {
                bool moved = TryMove(direction, player);
                if (moved)
                {
                    player.LastMoveTime = DateTime.Now;
                    gameInterface.Revalidate(player.Cell);
                }
            }
        }
        private void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Space:
                    window.Close();
                    return;
                case Key.S:
                    TryPlayerMove(Direction.Down);
                    return;
                case Key.D:
                    TryPlayerMove(Direction.Right);
                    return;
                case Key.W:
                    TryPlayerMove(Direction.Up);
                    return;
                case Key.A:
                    TryPlayerMove(Direction.Left);
                    return;
            }
        }
        private void Scroll(IMouse mouse, ScrollWheel scrollWheel)
        {
            //Console.WriteLine("scrollWheel.X {0}", scrollWheel.X);
            Console.WriteLine("scrollWheel.Y {0}", scrollWheel.Y);
        }
        private void MouseDown(IMouse mouse, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                Point mousePoint = new Point((int)mouse.Position.X, (int)mouse.Position.Y);
                if (bounds.Contains(mousePoint))
                {
                    mousePoint.X -= bounds.X;
                    mousePoint.Y -= bounds.Y;
                    Vector2D<double> normalPosition = new Vector2D<double>((double)mousePoint.X / (double)bounds.Width,
                        (double)mousePoint.Y / (double)bounds.Height);
                    normalPosition.Y = 1d - normalPosition.Y;
                    Point point = new Point((int)(normalPosition.X * (double)size.X), (int)(normalPosition.Y * (double)size.Y));
                    gameInterface.LeftMouseDown(point);
                }
            }
        }
        private void Closing()
        {
        }
        public void Dispose()
        {
            Console.WriteLine("Dispose");
            bitmap.Dispose();
            imageManager.Dispose();
        }
        public void AdjustView()
        {
            double windowHeight = window.Size.Y;
            double windowWidth = window.Size.X;
            double width, height, difference, half; //scale;
            //float zoom = (float)GameManager.Instance.Camera.Zoom;
            //Vector2f size = new Vector2f((float)viewSize.X * zoom, (float)viewSize.Y * zoom);
            //FloatRect viewport;
            //FloatRect guiVieport;
            proportion = size.X / size.Y;
            double inverse = 1d / proportion;
            if (windowHeight > inverse * windowWidth)
            {
                width = windowWidth;
                height = width * inverse;
                difference = windowHeight - height;
                half = difference / 2d;
                //scale = height / windowHeight;
                bounds.X = 0;
                bounds.Y = (int)half;
                //viewport = new FloatRect(0f, (float)top, 1f / 2f, (float)scale);
                //guiVieport = new FloatRect(1f / 2f, (float)top, 1f / 2f, (float)scale);
            }
            else
            {
                height = windowHeight;
                width = height * proportion;
                difference = windowWidth - width;
                half = difference / 2d;
                //scale = width / windowWidth;
                //double left = half / windowWidth;
                bounds.X = (int)half;
                bounds.Y = 0;
                //viewport = new FloatRect((float)left, 0f, (float)scale / 2f, 1f);
                //guiVieport = new FloatRect((float)left + (float)scale / 2f, 0f, (float)scale / 2f, 1f);
            }
            bounds.Width = (int)width;
            bounds.Height = (int)height;

            //Vector2f center = new Vector2f((float)GameManager.Instance.Camera.X * (float)GameManager.Instance.Camera.CellSize + (float)GameManager.Instance.Camera.CellSize / 2f,
            //    (float)GameManager.Instance.Camera.Y * (float)GameManager.Instance.Camera.CellSize + (float)GameManager.Instance.Camera.CellSize / 2f);

        }
        private bool TryMove(Direction direction, Occupant occupant)
        {
            Cell cell = occupant.Cell;
            Cell newCell = world.GetNeighbour(cell, direction);
            if (newCell == null)
            {
                return false;
            }
            cell.Occupant = null;
            newCell.Occupant = occupant;
            occupant.Cell = newCell;
            return true;
        }
    }
}