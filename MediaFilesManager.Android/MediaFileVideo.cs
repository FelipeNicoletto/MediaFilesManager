using System;
using Android.Database;
using System.Threading.Tasks;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager.Droid
{
    public class MediaFileVideo : MediaFile, IMediaFileVideo
    {
        public MediaFileVideo() : base(MediaFileType.Video)
        {

        }

        public DateTime TakenDate { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public TimeSpan Duration { get; private set; }

        public string Artist { get; private set; }

        public ImageStream GetImage()
        {
            return GetImage(MediaFileGetImageOptions.CreateDefaultThumb());
        }

        public ImageStream GetImage(int width, int height)
        {
            var options = MediaFileGetImageOptions.CreateDefaultThumb();
            options.Width = width;
            options.Height = height;
            return GetImage(options);
        }

        public ImageStream GetImage(MediaFileGetImageOptions options)
        {
            return AssetImageService.GetVideoImage(this, options);
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

        internal override void LoadCursor(ICursor cursor, MediaAssetQueryHelper helper)
        {
            base.LoadCursor(cursor, helper);

            if (helper.DateTakenColumn > -1)
            {
                TakenDate = DateTimeOffset.FromUnixTimeMilliseconds(cursor.GetLong(helper.DateTakenColumn)).DateTime;
            }

            if (helper.WidthColumn > -1)
            {
                Width = cursor.GetInt(helper.WidthColumn);
            }

            if (helper.HeightColumn > -1)
            {
                Height = cursor.GetInt(helper.HeightColumn);
            }

            if (helper.LatitudeColumn > -1)
            {
                Latitude = cursor.GetDouble(helper.LatitudeColumn);
            }

            if (helper.LongitudeColumn > -1)
            {
                Longitude = cursor.GetDouble(helper.LongitudeColumn);
            }

            if (helper.DurationColumn > -1)
            {
                Duration = TimeSpan.FromMilliseconds(cursor.GetLong(helper.DurationColumn));
            }

            if (helper.ArtistColumn > -1)
            {
                Artist = cursor.GetString(helper.ArtistColumn);
            }
        }
    }
}
