using System;
namespace Skira
{
    public class Camera
    {
        private Vector2d position, difference;
        private double cellSize;
        private double zoom;
        private World world;
        private Vector2d epsillonVector;
        private double epsillon, speedFromZoom, distance, speed = 2d;
        public double CellSize
        {
            get => cellSize;
        }
        public double Zoom
        {
            get => zoom;
            set
            {
                if (value >= 2 && value <= 4)
                {
                    zoom = value;
                    speedFromZoom = (5d - zoom) / zoom;
                    //speedFromZoom = zoom + baseSpeed;
                    epsillon = zoom * (1d / 2d); //+ (zoom - 1) / 2d;
                    epsillonVector = new Vector2d(epsillon * (2d / 3d), epsillon * (2d / 3d));
                }
            }
        }
        public double X
        {
            get => position.X;
            set => position.X = value;
        }
        public double Y
        {
            get => position.Y;
            set => position.Y = value;
        }

        private bool MustFollow()
        {
            Vector2d a = position, b = GameManager.Instance.Player.Position;
            difference = b - a;
            distance = difference.Magnitude;
            Vector2d absolute = difference.Absoulte;
            //Console.WriteLine("absolute {0}", absolute);
            //Console.WriteLine("epsillonVector {0}", epsillonVector);
            if (absolute.Y > epsillonVector.Y || absolute.X > epsillonVector.X)
            {
                //Console.WriteLine(true);
                return true;
            }
            return false;
        }

        private void Follow(TimeSpan timeSpan)
        {
            //position = Vector2d.MoveTowards(transform.position, player.transform.position, Time.deltaTime * (speed / 64.0f) * distance * distance);
            double multiplier = speedFromZoom * speed * distance * distance * timeSpan.TotalMilliseconds;
            multiplier /= 4096d;
            if (multiplier > 1d)
            {
                multiplier = 1d;
            }
            //Console.WriteLine("difference {0}", difference);
            //Console.WriteLine("multiplier {0}", multiplier);
            //Console.WriteLine("difference * multiplier {0}", difference * multiplier);
            //position += difference * new Vector2d(1d, 1d / 8d) * multiplier;
            position += difference * new Vector2d(1d, 1d) * multiplier;
        }

        public void Update(TimeSpan timeSpan)
        {
            if (MustFollow())
            {
                Follow(timeSpan);
                GameManager.Instance.Backend.AdjustView();
            }
        }
        public Camera(World world, double cellSize, double zoom, double x, double y)
        {
            Zoom = zoom;
            this.world = world;
            this.cellSize = cellSize;
            this.position = new Vector2d(x, y);
        }
    }
}