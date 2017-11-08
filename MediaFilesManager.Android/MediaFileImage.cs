using System;
using Android.Database;
using System.Threading.Tasks;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager.Droid
{
    public class MediaFileImage : MediaFile, IMediaFileImage
    {
        public MediaFileImage() : base(MediaFileType.Image)
        {
            
        }
        
        public DateTime TakenDate { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }
        
        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public MediaFileImageOrientation Orientation { get; private set; }
        
        public ImageStream GetImage()
        {
            return GetImage(Width, Height);
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
            return AssetImageService.GetImage(this, options);
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

            if (helper.OrientationColumn > -1)
            {
                switch (cursor.GetInt(helper.OrientationColumn))
                {
                    case 90:
                        Orientation = MediaFileImageOrientation.Left;
                        break;
                    case 180:
                        Orientation = MediaFileImageOrientation.Down;
                        break;
                    case 270:
                        Orientation = MediaFileImageOrientation.Right;
                        break;
                    default:
                        Orientation = MediaFileImageOrientation.Up;
                        break;
                }
            }
        }
    }
}
