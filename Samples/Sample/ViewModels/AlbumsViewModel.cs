using MediaFilesManager;
using MediaFilesManager.Abstractions;
using Sample.Helpers;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class AlbumsViewModel : ObservableObject
    {
        public AlbumsViewModel()
        {
            var task = CrossMediaFileManager.Current.GetAlbumsAsync();

            task.ContinueWith((albums) => 
            {
                Albums = albums.Result.Select(a => new AlbumItem(a)).ToArray();
            });
        }

        private AlbumItem[] _albums;
        public AlbumItem[] Albums
        {
            get => _albums;
            set => SetProperty(ref _albums, value);
        }

        public class AlbumItem : ObservableObject
        {
            public IMediaAlbum AssetsAlbum { get; }
            private byte[] _bytes;

            private static MediaFileGetImageOptions _imageOptions = MediaFileGetImageOptions.CreateDefaultThumb();

            public AlbumItem(IMediaAlbum assetsAlbum)
            {
                AssetsAlbum = assetsAlbum;

                AssetsAlbum.GetMediaFilesCountAsync().ContinueWith((count) =>
                {
                    Description = $"{count.Result} files";
                });
            }

            public string Title { get => AssetsAlbum.Title; }

            private string _description;
            public string Description
            {
                get => _description;
                set => SetProperty(ref _description, value);
            }

            public void LoadImage()
            {
                if (Image != null)
                {
                    return;
                }
            }
            
            private ImageSource _image;
            public ImageSource Image
            {
                get
                {
                    if (_image == null)
                    {
                        var asset = AssetsAlbum.GetMediaFileAtIndex(0) as IMediaFileWithImage;
                        if (asset != null)
                        {
                            var stream = asset.GetImage(_imageOptions);
                            _bytes = stream.ToArray();
                            _image = ImageSource.FromStream(() => new MemoryStream(_bytes));
                        }
                    }

                    return _image;
                }
            }
        }
    }
}
