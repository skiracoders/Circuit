using System.Drawing;
using System.Drawing.Imaging;

namespace Skira
{
    public class Button
    {
        protected Rectangle bounds;
        public Rectangle Bounds
        {
            get => bounds;
        }
        protected ImagePack imagePack;
        protected ImageAttributes imageAttributes;
        public ImageAttributes ImageAttributes
        {
            set => imageAttributes = value;
        }

        public Button(ImagePack imagePack, Point origin, Size size, ImageAttributes imageAttributes)
        {
            this.imageAttributes = imageAttributes;
            this.imagePack = imagePack;
            this.bounds = new Rectangle(origin, size);
        }
        public void Draw(Graphics graphics)
        {
            GameManager.Instance.DrawImageWithAttributes(graphics, imagePack, imageAttributes, bounds);
        }
    }
}