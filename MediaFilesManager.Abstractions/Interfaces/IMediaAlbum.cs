using System.Threading.Tasks;

namespace MediaFilesManager.Abstractions
{
    public interface IMediaAlbum
    {
        string Title { get; }

        int GetMediaFilesCount(MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
        Task<int> GetMediaFilesCountAsync(MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
        MediaAlbumContentType GetContentTypes();
        Task<MediaAlbumContentType> GetContentTypesAsync();
        IMediaFile GetMediaFileAtIndex(int index, MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
        Task<IMediaFile> GetMediaFileAtIndexAsync(int index, MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
        IMediaFile[] GetAllMediaFiles(MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
        Task<IMediaFile[]> GetAllMediaFilesAsync(MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
    }
}
