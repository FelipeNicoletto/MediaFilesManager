using System;
using System.IO;
using Foundation;
using Photos;
using System.Threading.Tasks;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager.iOS
{
    public class MediaFile : IMediaFile
    {
        protected MediaFile(PHAsset asset, MediaFileType type)
        {
            Asset = asset;
        }

        public static MediaFile FromAsset(PHAsset asset)
        {
            switch (asset.MediaType)
            {
                case PHAssetMediaType.Image:
                    return new MediaFileImage(asset);
                case PHAssetMediaType.Video:
                    return new MediaFileVideo(asset);
                default:
                    return null;
            }
        }

        protected PHAsset Asset { get; }

        public string Name { get; private set; }

        public DateTime CreationDate { get => (DateTime)Asset.CreationDate; }

        private long? _size;
        public long Size
        {
            get
            {
                if (!_size.HasValue)
                {
                    var resource = PHAssetResource.GetAssetResources(Asset)[0];

                    _size = ((NSNumber)resource.ValueForKey(new NSString("fileSize"))).Int64Value;
                }
                return _size.Value;
            }
        }
        
        public MediaFileType Type { get; internal set; }

        public virtual Stream GetStream()
        {
            return null;
        }

        public virtual Task<Stream> GetStreamAsync()
        {
            return null;
        }
    }
}
