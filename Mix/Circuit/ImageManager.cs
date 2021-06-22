using System.Collections.Generic;
using System.IO;

namespace Skira
{
    public class ImageManager
    {
        Dictionary<string, string> paths;
        public ImageManager()
        {
            paths = new Dictionary<string, string>();
        }
        public void Add(string name, string[] path)
        {
            paths.Add(name, Path.Combine(path));
        }
        public Dictionary<string, string> Paths
        {
            get { return paths; }
        }
        public string this[string name]
        {
            get { return paths[name]; }
        }
    }
}