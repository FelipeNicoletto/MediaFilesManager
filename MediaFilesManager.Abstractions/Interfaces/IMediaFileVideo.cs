using System;

namespace MediaFilesManager.Abstractions
{
    public interface IMediaFileVideo : IMediaFile, IMediaFileWithImage
    {
        DateTime TakenDate { get; }
        int Width { get; }
        int Height { get; }
        double Latitude { get; }
        double Longitude { get; }
        TimeSpan Duration { get; }
        string Artist { get; }
    }
}
