using Android.Graphics;
using Android.Provider;
using System;
using System.IO;

namespace MediaFilesManager.Droid
{
    internal static class AssetImageService
    {
        internal static ImageStream GetImage(MediaFileImage mediaAssetImage, MediaFileGetImageOptions options)
        {
            var w = options.Width;
            var h = options.Height;
            
            Bitmap img;
            if (w == h && w > 0 && w <= 96 && h <= 96 && options.ResizeAspect == MediaFileGetImageOptions.ImageResizeAspect.AspectFill)
            {
                img = MediaStore.Images.Thumbnails.GetThumbnail(MediaFileManager.Context.ContentResolver, mediaAssetImage.Id, ThumbnailKind.MicroKind, null);
            }
            else if (w > 0 && w <= 512 && h > 0 && h <= 384)
            {
                img = MediaStore.Images.Thumbnails.GetThumbnail(MediaFileManager.Context.ContentResolver, mediaAssetImage.Id, ThumbnailKind.MiniKind, null);
            }
            else
            {
                img = BitmapFactory.DecodeFile(mediaAssetImage.Uri);
            }
            
            using (var finalImg = ApplyImageOptions(img, mediaAssetImage.Orientation, options, w, h))
            using (var stream = new MemoryStream())
            {
                finalImg.Compress(Bitmap.CompressFormat.Jpeg, options.Quality, stream);
                stream.Position = 0;
                
                return ImageStream.FromStream(stream, finalImg.Width, finalImg.Height);
            }
        }
        
        internal static ImageStream GetVideoImage(MediaFileVideo mediaAssetVideo, MediaFileGetImageOptions options)
        {
            var w = options.Width;
            var h = options.Height;

            Bitmap img;
            if (w == h && w > 0 && w <= 96 && h <= 96 && options.ResizeAspect == MediaFileGetImageOptions.ImageResizeAspect.AspectFill)
            {
                img = MediaStore.Video.Thumbnails.GetThumbnail(MediaFileManager.Context.ContentResolver, mediaAssetVideo.Id, VideoThumbnailKind.MicroKind, null);
            }
            else if ((w <= 512 && h <= 384) || w <= 0 || h <= 0)
            {
                img = MediaStore.Video.Thumbnails.GetThumbnail(MediaFileManager.Context.ContentResolver, mediaAssetVideo.Id, VideoThumbnailKind.MiniKind, null);
            }
            else
            {
                img = Android.Media.ThumbnailUtils.CreateVideoThumbnail(mediaAssetVideo.Uri, ThumbnailKind.FullScreenKind);
            }

            using (var finalImg = ApplyImageOptions(img, MediaFileImageOrientation.Up, options, w, h))
            using (var stream = new MemoryStream())
            {
                finalImg.Compress(Bitmap.CompressFormat.Jpeg, options.Quality, stream);
                stream.Position = 0;

                return ImageStream.FromStream(stream, finalImg.Width, finalImg.Height);
            }
        }

        private static Bitmap ApplyImageOptions(Bitmap img, MediaFileImageOrientation originalOrientation, MediaFileGetImageOptions options, float w, float h)
        {
            RotateDimension(w, h, originalOrientation, options.Orientation, out w, out h);

            if (w <= 0)
            {
                if (h > 0)
                {
                    w = img.Width * h / img.Height;
                }
                else
                {
                    w = img.Width;
                }
            }

            if (h <= 0)
            {
                h = img.Height * w / img.Width;
            }

            GetResizeDimensions(img.Width, img.Height, w, h, options.ResizeAspect, out var widthDest, out var heightDest, out var x, out var y, out w, out h);

            if (img.Width != widthDest ||
                img.Height != heightDest)
            {
                var newImg = Bitmap.CreateScaledBitmap(img, (int)widthDest, (int)heightDest, false);
                img.Dispose();
                img = newImg;
            }

            var matrix = GetOrientationMatrix(originalOrientation, options.Orientation);

            if (matrix != null ||
                x != 0 ||
                y != 0 ||
                w != img.Width ||
                h != img.Height)
            {
                var newImg = Bitmap.CreateBitmap(img, (int)x, (int)y, (int)w, (int)h, matrix, false);
                img.Dispose();
                return newImg;
            }

            return img;
        }
        
        private static Matrix GetOrientationMatrix(MediaFileImageOrientation originalOrientation, MediaFileImageOrientation orientationDest)
        {
            var rotation = 0f;
            var scaleX = 1f;
            var scaleY = 1f;
            switch (originalOrientation)
            {
                case MediaFileImageOrientation.Down:
                    rotation = 180f;
                    break;
                case MediaFileImageOrientation.Left:
                    rotation = 90f;
                    break;
                case MediaFileImageOrientation.Right:
                    rotation = 270f;
                    break;
            }

            if (orientationDest != MediaFileImageOrientation.Up)
            {
                switch (orientationDest)
                {
                    case MediaFileImageOrientation.Up:
                        break;
                    case MediaFileImageOrientation.UpMirrored:
                        scaleX = -1f;
                        break;
                    case MediaFileImageOrientation.Down:
                        rotation += 180f;
                        break;
                    case MediaFileImageOrientation.DownMirrored:
                        rotation += 180f;
                        scaleX = -1f;
                        break;
                    case MediaFileImageOrientation.Left:
                        rotation += 270f;
                        break;
                    case MediaFileImageOrientation.LeftMirrored:
                        rotation += 270f;
                        scaleY = -1f;
                        break;
                    case MediaFileImageOrientation.Right:
                        rotation += 90f;
                        break;
                    case MediaFileImageOrientation.RightMirrored:
                        rotation += 90f;
                        scaleY = -1f;
                        break;
                    default:
                        break;
                }
            }

            Matrix matrix = null;

            if (rotation != 0f)
            {
                matrix = new Matrix();
                matrix.PostRotate(rotation);
            }

            if (scaleX != 1f || scaleY != 1f)
            {
                matrix = matrix ?? new Matrix();
                matrix.PostScale(scaleX, scaleY);
            }

            return matrix;
        }

        private static void RotateDimension(float width, float height, MediaFileImageOrientation originalOrientation, MediaFileImageOrientation orientationDest, out float w, out float h)
        {
            var retW = width;
            var retH = height;

            Action rotate = () =>
            {
                var wT = retW;
                retW = retH;
                retH = wT;
            };

            switch (originalOrientation)
            {
                case MediaFileImageOrientation.Left:
                case MediaFileImageOrientation.LeftMirrored:
                case MediaFileImageOrientation.Right:
                case MediaFileImageOrientation.RightMirrored:
                    rotate();
                    break;
            }

            switch (orientationDest)
            {
                case MediaFileImageOrientation.Left:
                case MediaFileImageOrientation.LeftMirrored:
                case MediaFileImageOrientation.Right:
                case MediaFileImageOrientation.RightMirrored:
                    rotate();
                    break;
            }

            w = retW;
            h = retH;
        }
        
        private static void GetResizeDimensions(float origWidth, float origHeight, float destWidth, float destHeight, MediaFileGetImageOptions.ImageResizeAspect resizeAspect, out float resWidth, out float resHeight, out float x, out float y, out float width, out float height)
        {
            switch (resizeAspect)
            {
                case MediaFileGetImageOptions.ImageResizeAspect.AspectFill:
                case MediaFileGetImageOptions.ImageResizeAspect.AspectFit:

                    resHeight = origHeight * destWidth / origWidth;
                    resWidth = destWidth;

                    if (resizeAspect == MediaFileGetImageOptions.ImageResizeAspect.AspectFit)
                    {
                        if (resHeight > destHeight)
                        {
                            resHeight = destHeight;
                            resWidth = origWidth * destHeight / origHeight;
                        }

                        width = resWidth;
                        height = resHeight;
                        x = 0;
                        y = 0;
                    }
                    else
                    {
                        if (resHeight < destHeight)
                        {
                            resHeight = destHeight;
                            resWidth = origWidth * destHeight / origHeight;
                        }

                        width = destWidth;
                        height = destHeight;
                        x = (resWidth - width) / 2;
                        y = (resHeight - height) / 2;
                    }

                    break;
                default:
                    x = 0;
                    y = 0;
                    width = destWidth;
                    height = destHeight;
                    resWidth = destWidth;
                    resHeight = destHeight;
                    break;
            }
        }
    }
}
