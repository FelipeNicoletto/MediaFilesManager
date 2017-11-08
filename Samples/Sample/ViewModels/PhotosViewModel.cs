using Sample.Helpers;
using System.Linq;
using Xamarin.Forms;
using MediaFilesManager.Abstractions;
using MediaFilesManager;

namespace Sample.ViewModels
{
    public class PhotosViewModel : ObservableObject
    {
        public PhotosViewModel(IMediaAlbum album)
        {
            album.GetAllMediaFilesAsync()
                 .ContinueWith((arg) =>
            {
                Photos = arg.Result.Select(a => new PhotoItem(a)).ToArray();
            });
        }

        private PhotoItem[] _photos;
        public PhotoItem[] Photos { get => _photos; set => SetProperty(ref _photos, value); }

        public class PhotoItem : ObservableObject
        {
            public IMediaFile Asset { get; }

            public PhotoItem(IMediaFile asset)
            {
                Asset = asset;
            }
            
            private ImageSource _image;
            public ImageSource Image
            {
                get
                {
                    if (_image == null)
                    {
                        var asset = Asset as IMediaFileWithImage;
                        if (asset != null)
                        {
                            _image = ImageSource.FromStream(() => asset.GetImage(MediaFileGetImageOptions.CreateDefaultThumb()));
                        }
                    }

                    return _image;
                }
            }
        }
    }
}
