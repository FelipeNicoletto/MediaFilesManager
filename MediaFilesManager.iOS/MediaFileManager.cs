using System.Linq;
using System.Collections.Generic;
using Photos;
using MediaFilesManager.iOS;
using System.Threading.Tasks;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager
{
    public class MediaFileManager : IMediaFileManager
    {
        public static MediaFileManager Current => (MediaFileManager)CrossMediaFileManager.Current;

        public bool IsAuthorized()
        {
            return PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.Authorized;
        }

        public async Task<bool> RequestAuthorizationAsync()
        {
            if (IsAuthorized())
            {
                return true;
            }

            var status = await PHPhotoLibrary.RequestAuthorizationAsync();

            return status == PHAuthorizationStatus.Authorized;
        }

        public async Task<IMediaAlbum[]> GetAlbumsAsync()
        {
            if (!await RequestAuthorizationAsync())
            {
                return new IMediaAlbum[] { };
            }

            var list = new List<MediaAlbum>();

            var albums = new List<PHAssetCollection>();

            albums.AddRange(PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.Album, PHAssetCollectionSubtype.Any, null).Cast<PHAssetCollection>());
            albums.AddRange(PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.SmartAlbum, PHAssetCollectionSubtype.Any, null).Cast<PHAssetCollection>());

            return albums.Select(col => new MediaAlbum(col))
                         .OrderBy(a => a.Title)
                         .ToArray();
        }

        public Task<IMediaAlbum> CreateAlbum(string title)
        {
            return Task.Run(() =>
            {
                IMediaAlbum album = null;
                string id = null;
                if (PHPhotoLibrary.SharedPhotoLibrary.PerformChangesAndWait(() =>
                {
                    var request = PHAssetCollectionChangeRequest.CreateAssetCollection(title);
                    id = request.PlaceholderForCreatedAssetCollection.LocalIdentifier;
                }, out var error))
                {
                    var col = (PHAssetCollection)PHAssetCollection.FetchAssetCollections(new[] { id }, null).First();
                    album = new MediaAlbum(col);
                }

                return album;
            });
        }
    }
}