using Photos;
using System.Threading.Tasks;
using MediaFilesManager.Abstractions;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace MediaFilesManager.iOS
{
    public class MediaAlbum : IMediaAlbum
    {
        private Dictionary<MediaAlbumContentType, int> _countByMediaFileType;

        internal MediaAlbum(PHAssetCollection assetCollection)
        {
            AssetCollection = assetCollection;
        }

        private PHAssetCollection AssetCollection { get; }

        private string Id { get => AssetCollection.LocalIdentifier; }

        public string Title { get => AssetCollection.LocalizedTitle; }

        public MediaAlbumContentType GetContentTypes()
        {
            UpdateCountAndContentTypes();

            var contentType = MediaAlbumContentType.None;

            foreach (var type in _countByMediaFileType.Where(v => v.Value > 0))
            {
                if (contentType == MediaAlbumContentType.None)
                {
                    contentType = type.Key;
                }
                else
                {
                    contentType |= type.Key;
                }
            }

            return contentType;
        }
        public Task<MediaAlbumContentType> GetContentTypesAsync()
        {
            return Task.Run(() =>
            {
                return GetContentTypes();
            });
        }

        public int GetMediaFilesCount(MediaAlbumContentType contentTypes = MediaAlbumContentType.All)
        {
            UpdateCountAndContentTypes();

            if (contentTypes == MediaAlbumContentType.All)
            {
                return _countByMediaFileType.Sum(v => v.Value);
            }

            return _countByMediaFileType.Where(v => contentTypes.HasFlag(v.Key)).Sum(v => v.Value);
        }

        public Task<int> GetMediaFilesCountAsync(MediaAlbumContentType contentTypes = MediaAlbumContentType.All)
        {
            return Task.Run(() =>
            {
                return GetMediaFilesCount(contentTypes);
            });
        }

        public IMediaFile GetMediaFileAtIndex(int index, MediaAlbumContentType contentTypes = MediaAlbumContentType.All)
        {
            if (index >= 0)
            {
                return GetMediaAssetsFromAlbums(contentTypes, index).FirstOrDefault();
            }
            return null;
        }

        public Task<IMediaFile> GetMediaFileAtIndexAsync(int index, MediaAlbumContentType contentTypes = MediaAlbumContentType.All)
        {
            return Task.Run(() =>
            {
                return GetMediaFileAtIndex(index, contentTypes);
            });
        }

        public IMediaFile[] GetAllMediaFiles(MediaAlbumContentType contentTypes)
        {
            return GetMediaAssetsFromAlbums(contentTypes);
        }

        public Task<IMediaFile[]> GetAllMediaFilesAsync(MediaAlbumContentType contentTypes)
        {
            return Task.Run(() =>
            {
                return GetAllMediaFiles(contentTypes);
            });
        }

        private void UpdateCountAndContentTypes()
        {
            _countByMediaFileType = new Dictionary<MediaAlbumContentType, int>();

            var mediaTypes = new[] { MediaAlbumContentType.Images, MediaAlbumContentType.Videos };

            foreach (var mediaType in mediaTypes)
            {
                var options = new PHFetchOptions
                {
                    Predicate = GetPredicateToMediaAssetType(mediaType)
                };

                var ct = (int)PHAsset.FetchAssets(AssetCollection, options).Count;

                _countByMediaFileType.Add(mediaType, ct);
            }
        }

        private IMediaFile[] GetMediaAssetsFromAlbums(MediaAlbumContentType contentTypes, int index = -1)
        {
            var list = new List<IMediaFile>();
            var fetchOptions = GetFetchOptions(contentTypes);
            if (index >= 0)
            {
                fetchOptions.FetchLimit = (uint)index + 1;
            }

            var assets = PHAsset.FetchAssets(AssetCollection, fetchOptions);

            if (index >= 0)
            {
                var asset = assets.ElementAtOrDefault(index);

                if (asset != null)
                {
                    return new[] { MediaFile.FromAsset((PHAsset)asset) };
                }
                return new IMediaFile[] { };
            }

            return assets.Select(asset => MediaFile.FromAsset((PHAsset)asset)).ToArray();
        }

        private static PHFetchOptions GetFetchOptions(MediaAlbumContentType contentTypes)
        {
            var options = new PHFetchOptions
            {
                SortDescriptors = new[] { new NSSortDescriptor("creationDate", false) }
            };

            options.Predicate = GetPredicateToMediaAssetType(contentTypes);

            return options;
        }

        private static NSPredicate GetPredicateToMediaAssetType(MediaAlbumContentType contentTypes)
        {
            if (contentTypes.HasFlag(MediaAlbumContentType.Images) && !contentTypes.HasFlag(MediaAlbumContentType.Videos))
            {
                return NSPredicate.FromFormat(string.Format("(mediaType == {0})", (int)PHAssetMediaType.Image));
            }
            else if (!contentTypes.HasFlag(MediaAlbumContentType.Images) && contentTypes.HasFlag(MediaAlbumContentType.Videos))
            {
                return NSPredicate.FromFormat(string.Format("(mediaType == {0})", (int)PHAssetMediaType.Video));
            }

            return null;
        }
    }
}
