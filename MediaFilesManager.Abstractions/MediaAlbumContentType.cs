using System;

namespace MediaFilesManager
{
    [Flags]
    public enum MediaAlbumContentType
    {
        /// <summary>
        /// All media files. Only for query
        /// </summary>
        All = 1,
        /// <summary>
        /// No content available in the album
        /// </summary>
        None = 2,
        /// <summary>
        /// Images
        /// </summary>
        Images = 4,
        /// <summary>
        /// Videos
        /// </summary>
        Videos = 8,
        //Audios = 16,
        //Files = 32,
    }
}
