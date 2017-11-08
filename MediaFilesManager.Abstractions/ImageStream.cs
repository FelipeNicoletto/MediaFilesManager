using System.IO;

namespace MediaFilesManager
{
    public class ImageStream : MemoryStream
    {
        public int Width { get; }

        public int Height { get; }
               
        private ImageStream(Stream stream, int width, int height)
        {
            stream.CopyTo(this);
            this.Position = 0;

            Width = width;
            Height = height;
        }

        public static ImageStream FromStream(Stream stream, int width, int height)
        {
            return new ImageStream(stream, width, height);
        }
    }
}
