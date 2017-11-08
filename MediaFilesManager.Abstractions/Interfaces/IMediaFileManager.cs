using System.Threading.Tasks;

namespace MediaFilesManager.Abstractions
{
    public interface IMediaFileManager
    {
        bool IsAuthorized();
        Task<bool> RequestAuthorizationAsync();
        Task<IMediaAlbum[]> GetAlbumsAsync();
    }
}
