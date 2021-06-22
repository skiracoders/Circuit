using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System;
using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace Skira
{
    public class ImageManager : IDisposable
    {
        Dictionary<string, ImagePack> packs;
        Dictionary<string, Bitmap> bitmaps;
        public ImageManager()
        {
            bitmaps = new Dictionary<string, Bitmap>();
            packs = new Dictionary<string, ImagePack>();
        }
        public void Initialize()
        {
            Add("Ground-0x1", new string[] { "Content", "Sprites", "Ground-0x1.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Unit-0x1", new string[] { "Content", "Sprites", "Unit-0x1.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Place", new string[] { "Content", "Sprites", "Place.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Delete", new string[] { "Content", "Sprites", "Delete.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Positive", new string[] { "Content", "Sprites", "Circuit", "Positive.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Positive-Flow", new string[] { "Content", "Sprites", "Circuit", "Positive-Flow.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Negative", new string[] { "Content", "Sprites", "Circuit", "Negative.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Negative-Flow", new string[] { "Content", "Sprites", "Circuit", "Negative-Flow.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Arrow-Up", new string[] { "Content", "Sprites", "Arrow-0x1.png" }, RotateFlipType.RotateNoneFlipY);
            Add("Arrow-Right", new string[] { "Content", "Sprites", "Arrow-0x1.png" }, RotateFlipType.Rotate270FlipNone);
            Add("Arrow-Down", new string[] { "Content", "Sprites", "Arrow-0x1.png" }, RotateFlipType.Rotate180FlipNone);
            Add("Arrow-Left", new string[] { "Content", "Sprites", "Arrow-0x1.png" }, RotateFlipType.Rotate90FlipNone);
            Add("Connector-Full", new string[] { "Content", "Sprites", "Circuit", "Connector", "Connector-Full.png" },
                RotateFlipType.RotateNoneFlipY);
            Add("Connector-Full-Flow", new string[] { "Content", "Sprites", "Circuit", "Connector", "Connector-Full-Flow.png" },
                           RotateFlipType.RotateNoneFlipY);
        }
        public void Add(string name, string[] path, RotateFlipType rotateFlipType)
        {
            string pathString = Path.Combine(path);
            Bitmap bitmap;
            if (bitmaps.ContainsKey(pathString))
            {
                bitmap = new Bitmap(bitmaps[pathString]);
            }
            else
            {
                bitmap = new Bitmap(pathString);
                bitmaps.Add(pathString, bitmap);
            }
            //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bitmap.RotateFlip(rotateFlipType);
            Vector2D<int> size = new Vector2D<int>(bitmap.Width, bitmap.Height);
            byte[] pixels = new byte[size.Y * size.X * 4];
            uint index = 0;
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    pixels[index + 0] = color.R;
                    pixels[index + 1] = color.G;
                    pixels[index + 2] = color.B;
                    pixels[index + 3] = color.A;
                    index += 4;
                }
            }
            packs.Add(name, new ImagePack(pathString, bitmap, pixels, size));
        }
        public ImagePack this[string name]
        {
            get { return packs[name]; }
        }
        public void Dispose()
        {
            foreach (ImagePack imagePack in packs.Values)
            {
                imagePack.Dispose();
            }
        }
    }
}