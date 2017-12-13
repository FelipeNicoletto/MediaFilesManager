using System;
using System.IO;
using System.Threading.Tasks;
using Android.Database;
using Android.Provider;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager.Droid
{
    public class MediaFile : IMediaFile
    {
        protected MediaFile(MediaFileType type)
        {
            Type = type;
        }
        
        public static MediaFile FromMediaType(MediaType type)
        {
            switch(type)
            {
                case MediaType.Image:
                    return new MediaFileImage();
                case MediaType.Video:
                    return new MediaFileVideo();
                default:
                    return null;
            }
        }

        internal long Id { get; private set; }

        internal string Uri { get; private set; }

        public string Name { get; private set; }

        public DateTime CreationDate { get; private set; }

        public long Size { get; private set; }

        internal string MimeType { get; private set; }

        public MediaFileType Type { get; }

        public Stream GetStream()
        {
            return new StreamReader(Uri).BaseStream;
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.Run(() =>
            {
                return GetStream();
            });
        }

        internal virtual void LoadCursor(ICursor cursor, MediaAssetQueryHelper helper)
        {
            Id = cursor.GetLong(helper.IdColumn);
            Name = cursor.GetString(helper.NameColumn);
            Uri = cursor.GetString(helper.UriColumn);
            
            if (helper.DateAddedColumn > -1)
            {
                CreationDate = DateTimeOffset.FromUnixTimeSeconds(cursor.GetLong(helper.DateAddedColumn)).DateTime;
            }

            if (helper.SizeColumn > -1)
            {
                Size = cursor.GetLong(helper.SizeColumn);
            }
            
            if (helper.MimeTypeColumn > -1)
            {
                MimeType = cursor.GetString(helper.MimeTypeColumn);
            }
        }
    }
}
