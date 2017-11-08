using MediaFilesManager.Abstractions;
using Photos;
using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using AVFoundation;

namespace MediaFilesManager.iOS
{
    public class MediaFileVideo : MediaFile, IMediaFileVideo
    {
        public MediaFileVideo(PHAsset asset) : base(asset, MediaFileType.Video)
        {
            
        }

        public DateTime TakenDate { get => (DateTime)Asset.CreationDate; }

        public int Width { get => (int)Asset.PixelWidth; }

        public int Height { get => (int)Asset.PixelHeight; }

        public double Latitude { get => Asset.Location?.Coordinate.Latitude ?? 0d; }

        public double Longitude { get => Asset.Location?.Coordinate.Longitude ?? 0d; }

        public TimeSpan Duration { get => TimeSpan.FromSeconds(Asset.Duration); }

        public string Artist { get; }

        public ImageStream GetImage()
        {
            return GetImage(MediaFileGetImageOptions.CreateDefault());
        }

        public ImageStream GetImage(int width, int height)
        {
            var options = MediaFileGetImageOptions.CreateDefault();
            options.Width = width;
            options.Height = height;
            return GetImage(options);
        }

        public ImageStream GetImage(MediaFileGetImageOptions options)
        {
            return AssetImageService.GetImage(Asset, options);
        }

        public Task<ImageStream> GetImageAsync()
        {
            return Task.Run(() =>
            {
                return GetImage();
            });
        }

        public Task<ImageStream> GetImageAsync(int width, int height)
        {
            return Task.Run(() =>
            {
                return GetImage(width, height);
            });
        }

        public Task<ImageStream> GetImageAsync(MediaFileGetImageOptions options)
        {
            return Task.Run(() =>
            {
                return GetImage(options);
            });
        }

        public override Stream GetStream()
        {
            Stream stream = null;
            var wait = true;

            new PHCachingImageManager().RequestAvAsset(Asset, null, (assetx, audioMix, info) =>
            {
                stream = NSData.FromUrl((assetx as AVUrlAsset).Url).AsStream();
                wait = false;
            });

            while (wait) { }

            return stream;
        }

        public override Task<Stream> GetStreamAsync()
        {
            return Task.Run(() =>
            {
                return GetStream();
            });
        }
    }
}
