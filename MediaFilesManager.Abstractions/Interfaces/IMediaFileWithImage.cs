using System.Threading.Tasks;

namespace MediaFilesManager.Abstractions
{
    public interface IMediaFileWithImage
    {
        ImageStream GetImage();
        ImageStream GetImage(int width, int height);
        ImageStream GetImage(MediaFileGetImageOptions options);
        Task<ImageStream> GetImageAsync();
        Task<ImageStream> GetImageAsync(int width, int height);
        Task<ImageStream> GetImageAsync(MediaFileGetImageOptions options);
    }
}
