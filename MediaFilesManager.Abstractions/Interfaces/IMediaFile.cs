using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaFilesManager.Abstractions
{
    public interface IMediaFile
    {
        DateTime CreationDate { get; }
        long Size { get; }
        MediaFileType Type { get; }

        Stream GetStream();
        Task<Stream> GetStreamAsync();
    }
}
