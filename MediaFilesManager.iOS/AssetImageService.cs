using System;
using Photos;
using UIKit;

namespace MediaFilesManager.iOS
{
    internal static class AssetImageService
    {
        internal static ImageStream GetImage(PHAsset asset, MediaFileGetImageOptions options)
        {
            nfloat w = options.Width;
            nfloat h = options.Height;

            switch (options.Orientation)
            {
                case MediaFileImageOrientation.Left:
                case MediaFileImageOrientation.LeftMirrored:
                case MediaFileImageOrientation.Right:
                case MediaFileImageOrientation.RightMirrored:
                    var wT = w;
                    w = h;
                    h = wT;
                    break;
            }

            if (w <= 0)
            {
                if (h > 0)
                {
                    w = asset.PixelWidth * h / asset.PixelHeight;
                }
                else
                {
                    w = asset.PixelWidth;
                }
            }

            if (h <= 0)
            {
                h = asset.PixelHeight * w / asset.PixelWidth;
            }

            ImageStream image = null;

            PHImageManager.DefaultManager.RequestImageForAsset(
                asset,
                new CoreGraphics.CGSize(w, h),
                ImageResizeAspectToPH(options.ResizeAspect),
                new PHImageRequestOptions { Synchronous = true, ResizeMode = PHImageRequestOptionsResizeMode.Exact },
                (requestedImage, info) =>
                {
                    if (options.Orientation != MediaFileImageOrientation.Up ||
                        requestedImage.Size.Width != w ||
                        requestedImage.Size.Height != h)
                    {
                        var destW = w;
                        var destH = h;

                        if (options.ResizeAspect == MediaFileGetImageOptions.ImageResizeAspect.AspectFit)
                        {
                            var widthScale = w / requestedImage.Size.Width;
                            var heightScale = h / requestedImage.Size.Height;
                            var scale = (nfloat)Math.Min(widthScale, heightScale);

                            switch (options.Orientation)
                            {
                                case MediaFileImageOrientation.Left:
                                case MediaFileImageOrientation.LeftMirrored:
                                case MediaFileImageOrientation.Right:
                                case MediaFileImageOrientation.RightMirrored:
                                    h = requestedImage.Size.Width * scale;
                                    w = requestedImage.Size.Height * scale;
                                    destW = h;
                                    destH = w;
                                    break;

                                default:
                                    w = requestedImage.Size.Width * scale;
                                    h = requestedImage.Size.Height * scale;
                                    destW = w;
                                    destH = h;
                                    break;
                            }
                        }
                        else
                        {
                            switch (options.Orientation)
                            {
                                case MediaFileImageOrientation.Left:
                                case MediaFileImageOrientation.LeftMirrored:
                                case MediaFileImageOrientation.Right:
                                case MediaFileImageOrientation.RightMirrored:
                                    var wT = w;
                                    w = h;
                                    h = wT;
                                    break;
                            }
                        }

                        var cg = requestedImage.CGImage;
                        int bytesPerRow = (int)w * 4;
                        var ctx = new CoreGraphics.CGBitmapContext(null, (int)w, (int)h, 8, bytesPerRow, cg.ColorSpace, CoreGraphics.CGImageAlphaInfo.PremultipliedLast);

                        Func<float, float> radians = (degrees) =>
                        {
                            return degrees * ((float)Math.PI / 180f);
                        };

                        switch (options.Orientation)
                        {
                            case MediaFileImageOrientation.UpMirrored:
                                ctx.TranslateCTM(w, 0);
                                ctx.ScaleCTM(-1f, 1f);
                                break;
                            case MediaFileImageOrientation.Down:
                                ctx.TranslateCTM(w, h);
                                ctx.RotateCTM(radians(180f));
                                break;
                            case MediaFileImageOrientation.DownMirrored:
                                ctx.TranslateCTM(0, h);
                                ctx.RotateCTM(radians(180f));
                                ctx.ScaleCTM(-1f, 1f);
                                break;
                            case MediaFileImageOrientation.Left:
                                ctx.TranslateCTM(w, 0);
                                ctx.RotateCTM(radians(90f));
                                break;
                            case MediaFileImageOrientation.LeftMirrored:
                                ctx.TranslateCTM(w, h);
                                ctx.RotateCTM(radians(270f));
                                ctx.ScaleCTM(1f, -1f);
                                break;
                            case MediaFileImageOrientation.Right:
                                ctx.TranslateCTM(0, h);
                                ctx.RotateCTM(radians(270f));
                                break;
                            case MediaFileImageOrientation.RightMirrored:
                                ctx.TranslateCTM(0, 0);
                                ctx.RotateCTM(radians(90f));
                                ctx.ScaleCTM(1f, -1f);
                                break;
                            default:
                                break;
                        }

                        ctx.DrawImage(new CoreGraphics.CGRect(0, 0, destW, destH), cg);

                        requestedImage = UIImage.FromImage(ctx.ToImage());

                        ctx.Dispose();
                    }

                    using (var stream = requestedImage.AsJPEG(options.Quality / 100f).AsStream())
                    {
                        image = ImageStream.FromStream(stream, (int)requestedImage.Size.Width, (int)requestedImage.Size.Height);
                    }
                    requestedImage.Dispose();
                });

            return image;
        }

        private static PHImageContentMode ImageResizeAspectToPH(MediaFileGetImageOptions.ImageResizeAspect resizeAspect)
        {
            switch (resizeAspect)
            {
                case MediaFileGetImageOptions.ImageResizeAspect.AspectFill:
                    return PHImageContentMode.AspectFill;
                case MediaFileGetImageOptions.ImageResizeAspect.AspectFit:
                    return PHImageContentMode.AspectFit;
                default:
                    return PHImageContentMode.Default;
            }
        }
    }
}
