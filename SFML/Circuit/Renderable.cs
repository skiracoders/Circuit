namespace Skira
{
    public class Renderable
    {
        protected string imageName;
        public string ImageName
        {
            get { return imageName; }
        }
        public Renderable(string imageName)
        {
            this.imageName = imageName;
        }
    }
}