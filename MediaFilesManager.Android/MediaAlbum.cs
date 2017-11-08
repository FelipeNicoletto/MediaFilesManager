using System.Threading.Tasks;
using MediaFilesManager.Abstractions;
using Android.Provider;
using System.Collections.Generic;
using Android.Content;
using System.Linq;

namespace MediaFilesManager.Droid
{
    public class MediaAlbum : IMediaAlbum
    {
        private Dictionary<MediaAlbumContentType, int> _countByMediaFileType;

        internal MediaAlbum(Context context)
        {
            Context = context;
        }
        
        internal string Id { get; set; }

        private Context Context { get; set; }

        public string Title { get; internal set; }
        
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

        private void UpdateCountAndContentTypes()
        {
            _countByMediaFileType = new Dictionary<MediaAlbumContentType, int>();

            var projection = new[]
            {
                "COUNT(*) as count",
                MediaStore.Files.FileColumns.MediaType
            };

            var cur = Context.ContentResolver.Query(
                MediaStore.Files.GetContentUri("external"),
                projection,
                $"{MediaStore.Images.ImageColumns.BucketId} = '{Id}'" +
                $" AND ({MediaStore.Files.FileColumns.MediaType} = {(int)MediaType.Image}" +
                    $" OR {MediaStore.Files.FileColumns.MediaType} = {(int)MediaType.Video})" +
                $") GROUP BY ({MediaStore.Files.FileColumns.MediaType}",
                null,
                null);
                
            if (cur != null && cur.Count > 0)
            {
                if (cur.MoveToFirst())
                {
                    do
                    {
                        var type = (MediaType)cur.GetInt(cur.GetColumnIndex(MediaStore.Files.FileColumns.MediaType));
                        var count = cur.GetInt(cur.GetColumnIndex("count"));

                        switch (type)
                        {
                            case MediaType.Image:
                                _countByMediaFileType.Add(MediaAlbumContentType.Images, count);
                                break;
                            case MediaType.Video:
                                _countByMediaFileType.Add(MediaAlbumContentType.Videos, count);
                                break;
                            default:
                                break;
                        }

                    } while (cur.MoveToNext());
                }

                cur.Close();
            }
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
                return GetMediaFileAtIndex(index);
            });
        }

        public IMediaFile[] GetAllMediaFiles(MediaAlbumContentType contentTypes = MediaAlbumContentType.All)
        {
            return GetMediaAssetsFromAlbums(contentTypes);
        }

        public Task<IMediaFile[]> GetAllMediaFilesAsync(MediaAlbumContentType contentTypes = MediaAlbumContentType.All)
        {
            return Task.Run(() =>
            {
                return GetAllMediaFiles(contentTypes);
            });
        }
        
        private IMediaFile[] GetMediaAssetsFromAlbums(MediaAlbumContentType contentTypes, int index = -1)
        {
            var helper = new MediaAssetQueryHelper(contentTypes);

            var queryUri = MediaStore.Files.GetContentUri("external");

            var list = new List<IMediaFile>();

            var cur = Context.ContentResolver.Query(
                queryUri,
                helper.GetProjection(),
                helper.GetSelection(Id),
                null,
                $"{MediaStore.Files.FileColumns.DateAdded} DESC" +
                (index >= 0 ? " LIMIT 1" : "") +
                (index > 0 ? $" OFFSET {index}" : ""));
            
            if (cur != null)
            {
                if (cur.Count > 0 && cur.MoveToFirst())
                {
                    helper.LoadColumnsIndexes(cur);

                    do
                    {
                        var mediaType = (MediaType)cur.GetInt(helper.MediaTypeColumn);

                        MediaFile mediaAsset = MediaFile.FromMediaType(mediaType);

                        mediaAsset.LoadCursor(cur, helper);

                        list.Add(mediaAsset);

                    } while (cur.MoveToNext());
                }

                cur.Close();
            }
            
            return list.ToArray();
        }
    }
}
