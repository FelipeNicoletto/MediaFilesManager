using System.Collections.Generic;
using Android.Provider;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using System.Linq;
using Android.Database;
using System.Threading.Tasks;
using Android.OS;
using Android.App;
using Android.Content;
using MediaFilesManager.Droid;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager
{
    public class MediaFileManager : IMediaFileManager
    {
        private const int RequestPermissionsId = 25;
        private TaskCompletionSource<bool> _currentAuthorizationRequestTask;
        internal static Context Context;

        public static void Initialize(Context context)
        {
            Context = context;
        }

        public static MediaFileManager Current => (MediaFileManager)CrossMediaFileManager.Current;

        public bool IsAuthorized()
        {
            return ContextCompat.CheckSelfPermission(Context, Manifest.Permission.ReadExternalStorage) == Android.Content.PM.Permission.Granted &&
                   ContextCompat.CheckSelfPermission(Context, Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Granted;
        }

        public async Task<bool> RequestAuthorizationAsync()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return true;
            }

            if (IsAuthorized())
            {
                return true;
            }

            var mainActivity = (Activity)Context;

            _currentAuthorizationRequestTask = new TaskCompletionSource<bool>();

            ActivityCompat.RequestPermissions(mainActivity,
                new[] {
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.ReadExternalStorage },
                RequestPermissionsId);

            return await _currentAuthorizationRequestTask.Task.ConfigureAwait(false);
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode != RequestPermissionsId)
            {
                return;
            }

            if (_currentAuthorizationRequestTask == null ||
                _currentAuthorizationRequestTask.Task.Status == TaskStatus.Canceled)
            {
                return;
            }

            bool result = false;
            for (var i = 0; i < permissions.Length; i++)
            {
                if (permissions[i] == Manifest.Permission.WriteExternalStorage ||
                    permissions[i] == Manifest.Permission.ReadExternalStorage)
                {
                    result = grantResults[i] == Android.Content.PM.Permission.Granted;

                    if (!result)
                    {
                        break;
                    }
                }
            }
            _currentAuthorizationRequestTask.SetResult(result);
        }

        public async Task<IMediaAlbum[]> GetAlbumsAsync()
        {
            if (!await RequestAuthorizationAsync())
            {
                return new IMediaAlbum[] { };
            }
                        
            var projection = new[]
            {
                MediaStore.Images.ImageColumns.BucketId,
                MediaStore.Images.ImageColumns.BucketDisplayName
            };
            
            var list = new List<IMediaAlbum>();

            var cur = Context.ContentResolver.Query(
                MediaStore.Files.GetContentUri("external"),
                projection,
                $"{new MediaAssetQueryHelper(MediaAlbumContentType.All).GetSelectionMediaType()}) GROUP BY ({MediaStore.Images.ImageColumns.BucketId}",
                null,
                null);
            
            if (cur != null && cur.Count > 0)
            {
                if (cur.MoveToFirst())
                {
                    do
                    {   
                        var album = new MediaAlbum(Context)
                        {
                            Id = cur.GetString(cur.GetColumnIndex(MediaStore.Images.ImageColumns.BucketId)),
                            Title = cur.GetString(cur.GetColumnIndex(MediaStore.Images.ImageColumns.BucketDisplayName)),
                        };

                        list.Add(album);
                        

                    } while (cur.MoveToNext());
                }

                cur.Close();
            }

            return list.OrderBy(a => a.Title).ToArray();
        }
    }

    internal class MediaAssetQueryHelper
    {
        public int IdColumn { get; private set; }
        public int NameColumn { get; private set; }
        public int UriColumn { get; private set; }
        public int DateAddedColumn { get; private set; }
        public int SizeColumn { get; private set; }
        public int MimeTypeColumn { get; private set; }
        public int MediaTypeColumn { get; private set; }

        public int DateTakenColumn { get; private set; }
        public int WidthColumn { get; private set; }
        public int HeightColumn { get; private set; }
        public int LatitudeColumn { get; private set; }
        public int LongitudeColumn { get; private set; }
        public int OrientationColumn { get; private set; }

        public int DurationColumn { get; private set; }
        public int ArtistColumn { get; private set; }

        private MediaAlbumContentType _mediaAlbumContentType;

        public MediaAssetQueryHelper(MediaAlbumContentType mediaAlbumContentType)
        {
            _mediaAlbumContentType = mediaAlbumContentType;
        }

        public string[] GetProjection()
        {
            var projection = new List<string>
            {
                MediaStore.Files.FileColumns.Id,
                MediaStore.Files.FileColumns.DisplayName,
                MediaStore.Files.FileColumns.Data,
                MediaStore.Files.FileColumns.DateAdded,
                MediaStore.Files.FileColumns.Size,
                MediaStore.Files.FileColumns.MimeType,
                MediaStore.Files.FileColumns.MediaType
            };
            
            if (_mediaAlbumContentType.HasFlag(MediaAlbumContentType.Images) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.Videos) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.All))
            {
                projection.AddRange(new []
                {
                    MediaStore.Images.ImageColumns.DateTaken,
                    MediaStore.Images.ImageColumns.Width,
                    MediaStore.Images.ImageColumns.Height,
                    MediaStore.Images.ImageColumns.Latitude,
                    MediaStore.Images.ImageColumns.Longitude,
                    MediaStore.Images.ImageColumns.Orientation
                });
            }

            if (_mediaAlbumContentType.HasFlag(MediaAlbumContentType.Videos) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.All))
            {
                projection.AddRange(new []
                {
                    MediaStore.Video.VideoColumns.Duration,
                    MediaStore.Video.VideoColumns.Artist
                });
            }   

            return projection.ToArray();
        }

        public string GetSelection(string albumId)
        {
            return $"{MediaStore.Images.ImageColumns.BucketId} = '{albumId}' AND ({GetSelectionMediaType()})";
        }

        public string GetSelectionMediaType()
        {
            var selection = new List<string>();

            if (_mediaAlbumContentType.HasFlag(MediaAlbumContentType.Images) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.All))
            {
                selection.Add($"{MediaStore.Files.FileColumns.MediaType}={(int)MediaType.Image}");
            }
            if (_mediaAlbumContentType.HasFlag(MediaAlbumContentType.Videos) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.All))
            {
                selection.Add($"{MediaStore.Files.FileColumns.MediaType}={(int)MediaType.Video}");
            }

            return string.Join(" OR ", selection);
        }

        public void LoadColumnsIndexes(ICursor cursor)
        {
            IdColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.Id);
            NameColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.DisplayName);
            UriColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.Data);
            DateAddedColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.DateAdded);
            SizeColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.Size);
            MimeTypeColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.MimeType);
            MediaTypeColumn = cursor.GetColumnIndex(MediaStore.Files.FileColumns.MediaType);

            if (_mediaAlbumContentType.HasFlag(MediaAlbumContentType.Images) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.Videos) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.All))
            {
                DateTakenColumn = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.DateTaken);
                WidthColumn = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Width);
                HeightColumn = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Height);
                LatitudeColumn = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Latitude);
                LongitudeColumn = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Longitude);
                OrientationColumn = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Orientation);
            }

            if (_mediaAlbumContentType.HasFlag(MediaAlbumContentType.Videos) ||
                _mediaAlbumContentType.HasFlag(MediaAlbumContentType.All))
            {
                DateTakenColumn = cursor.GetColumnIndex(MediaStore.Video.VideoColumns.DateTaken);
                DurationColumn = cursor.GetColumnIndex(MediaStore.Video.VideoColumns.Duration);
                ArtistColumn = cursor.GetColumnIndex(MediaStore.Video.VideoColumns.Artist);
            }
        }
    }
}