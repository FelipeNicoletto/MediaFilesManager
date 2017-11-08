namespace MediaFilesManager
{
    public class MediaFileGetImageOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Quality { get; set; }
        public MediaFileImageOrientation Orientation { get; set; }
        public ImageResizeAspect ResizeAspect { get; set; }

        public MediaFileGetImageOptions()
        {
            Orientation = MediaFileImageOrientation.Up;
            ResizeAspect = ImageResizeAspect.Fill;
            Quality = 100;
        }

        public static MediaFileGetImageOptions CreateDefaultThumb()
        {
            return new MediaFileGetImageOptions
            {
                Width = 200,
                Height = 200,
                Quality = 80,
                ResizeAspect = ImageResizeAspect.AspectFill
            };
        }

        public static MediaFileGetImageOptions CreateDefault()
        {
            return new MediaFileGetImageOptions
            {
                Quality = 90
            };
        }

        public enum ImageResizeAspect
        {
            /// <summary>
            /// Stretches the image to completely and exactly fill the display area. This may result in the image being distorted.
            /// </summary>
            Fill,
            /// <summary>
            /// Clips the image so that it fills the display area while preserving the aspect (ie.no distortion).
            /// </summary>
            AspectFill,
            /// <summary>
            /// Letterboxes the image(if required) so that the entire image fits into the display area, with blank space added to the top/bottom or sides depending on the whether the image is wide or tall.
            /// </summary>
            AspectFit
        }
    }
}
