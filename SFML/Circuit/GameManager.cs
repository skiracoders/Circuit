using System;

namespace Skira
{
    public sealed class GameManager
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
        public void Initialize()
        {
            imageManager = new ImageManager();
            imageManager.Add("Ground0x1", new string[] { "Content", "Sprites", "Ground0x1.png" });
            imageManager.Add("Unit0x1", new string[] { "Content", "Sprites", "Unit0x1.png" });
            world = new World(20u, 10u);
            CreatePlayer(5u, 10u);
            camera = new Camera(world, 32d, 3d, 0d, 0d);
            backend = new SFMLBackend("Circuit", 1024u, 512u);
            backend.LoadSprites();
            backend.Scrolled += Scrolled;
            backend.KeyPressed += KeyPressed;
            backend.Update += Update;
        }
        private SFMLBackend backend;
        private World world;
        private Camera camera;
        private ImageManager imageManager;
        private Player player;
        public SFMLBackend Backend
        {
            get => backend;
        }
        public Player Player
        {
            get => player;
        }
        public Camera Camera
        {
            get { return camera; }
        }
        public World World
        {
            get { return world; }
        }
        public ImageManager ImageManager
        {
            get { return imageManager; }
        }

        private void CreatePlayer(uint y, uint x)
        {
            Cell cell = world[y, x];
            player = new Player(typeof(Player), "Unit0x1", cell);
            cell.Occupant = player;

        }
        public void Run()
        {
            backend.Run();
        }
        private void tryMove(Direction direction, Occupant occupant)
        {
            Cell cell = occupant.Cell;
            Cell newCell = null;
            switch (direction)
            {
                case Direction.Down:
                    if (cell.Y == world.Height - 1)
                    {
                        return;
                    }
                    newCell = world[cell.Y + 1, cell.X];
                    break;
                case Direction.Right:
                    if (cell.X == world.Width - 1)
                    {
                        return;
                    }
                    newCell = world[cell.Y, cell.X + 1];
                    break;
                case Direction.Up:
                    if (cell.Y == 0)
                    {
                        return;
                    }
                    newCell = world[cell.Y - 1, cell.X];
                    break;
                case Direction.Left:
                    if (cell.X == 0)
                    {
                        return;
                    }
                    newCell = world[cell.Y, cell.X - 1];
                    break;
            }
            cell.Occupant = null;
            newCell.Occupant = occupant;
            occupant.Cell = newCell;
            /*
            if (occupant.Type == typeof(Player))
            {
                camera.X = newCell.X;
                camera.Y = newCell.Y;
                backend.AdjustView();
            }
            */
            backend.RequestDrawing();
        }
        private void KeyPressed(Key key, bool alt, bool control, bool shift, bool system)
        {
            switch (key)
            {
                case Key.S:
                    tryMove(Direction.Down, player);
                    return;
                case Key.D:
                    tryMove(Direction.Right, player);
                    return;
                case Key.W:
                    tryMove(Direction.Up, player);
                    return;
                case Key.A:
                    tryMove(Direction.Left, player);
                    return;
            }
        }
        private void Scrolled(double delta)
        {
            camera.Zoom -= delta;
            backend.AdjustView();
        }
        private void Update(TimeSpan timeSpan)
        {
            camera.Update(timeSpan);
            //timeSpan.TotalMilliseconds;
        }
    }
}
