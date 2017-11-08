using System;

namespace MediaFilesManager.Abstractions
{
    public interface IMediaFileImage : IMediaFile, IMediaFileWithImage
    {
        DateTime TakenDate { get; }
        int Width { get; }
        int Height { get; }
        double Latitude { get; }
        double Longitude { get; }
    }
}
