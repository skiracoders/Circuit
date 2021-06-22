namespace Skira
{
    public class Renderable
    {
        protected string image;
        public string Image
        {
            get { return image; }
        }
        public Renderable(string image)
        {
            this.image = image;
        }
    }
}