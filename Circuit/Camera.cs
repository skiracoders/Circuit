using System;
namespace Skira
{
    public class Camera
    {
        private Vector2d position, difference;
        private double cellSize;
        private Vector2d epsillonVector;
        private double epsillon, distance, speed;
        public double CellSize
        {
            get => cellSize;
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
            Vector2d a = position, b = GameManager.Instance.Player.Position * cellSize;
            difference = b - a;
            distance = difference.Magnitude;
            Vector2d absolute = difference.Absoulte;
            //Console.WriteLine("absolute {0}", absolute * (1d / cellSize));
            //Console.WriteLine("epsillonVector {0}", epsillonVector);
            //Console.WriteLine("GameManager.Instance.Player.Position * cellSize {0}", GameManager.Instance.Player.Position * cellSize);
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
            double d = distance / cellSize;
            double multiplier = speed * d * d * timeSpan.TotalMilliseconds;
            multiplier /= 4096d;
            //Console.WriteLine("multiplier");
            //Console.WriteLine(multiplier);
            if (multiplier > 1d)
            {
                multiplier = 1d;
            }
            //Console.WriteLine("difference {0}", difference);
            //Console.WriteLine("multiplier {0}", multiplier);
            //Console.WriteLine("difference * multiplier {0}", difference * multiplier);
            //position += difference * new Vector2d(1d, 1d / 8d) * multiplier;
            position += difference * multiplier;
        }

        public void Update(TimeSpan timeSpan)
        {
            if (MustFollow())
            {
                Follow(timeSpan);
                //GameManager.Instance.Backend.AdjustView();
            }
        }
        public Camera(double cellSize, double x, double y)
        {
            speed = 2d;
            this.cellSize = cellSize;
            this.position = new Vector2d(x, y);
            epsillon = cellSize * 1d; //+ (zoom - 1) / 2d;
            epsillonVector = new Vector2d(epsillon, epsillon);
        }
    }
}