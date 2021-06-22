using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System;
using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace Skira
{
    public class ImagePack : IDisposable
    {
        public string Path;
        public Bitmap Bitmap;
        public byte[] Pixels;
        public Vector2D<int> Size;
        public ImagePack(string path, Bitmap bitmap, byte[] pixels, Vector2D<int> size)
        {
            Path = path;
            Bitmap = bitmap;
            Pixels = pixels;
            Size = size;
        }
        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}