using System;
using Sample.Helpers;
using Xamarin.Forms;
using MediaFilesManager.Abstractions;
using MediaFilesManager;

namespace Sample.ViewModels
{
    public class PhotoViewModel : ObservableObject
    {
        private IMediaFileImage _asset;
        private ImageStream _img;

        public PhotoViewModel(IMediaFileImage asset)
        {
            _asset = asset;

            Width = 500;
            Height = 500;
            Quality = 90;

            Orientation = MediaFileImageOrientation.Up.ToString();
            ResizeMode = MediaFileGetImageOptions.ImageResizeAspect.AspectFit.ToString();

            Apply();
        }

        public void Apply()
        {
            try
            {
                var options = new MediaFileGetImageOptions
                {
                    Width = Width,
                    Height = Height,
                    Quality = Quality,
                    Orientation = (MediaFileImageOrientation)Enum.Parse(typeof(MediaFileImageOrientation), Orientation, true),
                    ResizeAspect = (MediaFileGetImageOptions.ImageResizeAspect)Enum.Parse(typeof(MediaFileGetImageOptions.ImageResizeAspect), ResizeMode, true),
                };

                _img?.Dispose();
                _img = _asset.GetImage(options);

                Details = $"{_img.Width} x {_img.Height} - {Math.Round(_img.Length / 1024d, 2)}KB";
                
                Image = ImageSource.FromStream(() => _img);
            }
            catch
            {
                Image = null;
                Details = string.Empty;
            }
        }

        private ImageSource _image;
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        private int _width;
        public int Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private int _height;
        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private int _quality;
        public int Quality
        {
            get => _quality;
            set => SetProperty(ref _quality, value);
        }

        private string _orientation;
        public string Orientation
        {
            get => _orientation;
            set => SetProperty(ref _orientation, value);
        }

        private string _resizeMode;
        public string ResizeMode
        {
            get => _resizeMode;
            set => SetProperty(ref _resizeMode, value);
        }

        private string _details;
        public string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
    }
}
