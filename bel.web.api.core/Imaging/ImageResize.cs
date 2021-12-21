// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageResize.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the ImageResize type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;

    using bel.web.api.core.objects.Interfaces;

    public class ImageResize : IImageResize
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height, int maxWidth, int maxHeight)
        {
            var size = this.GetResizedDimensions(width, height, maxWidth, maxHeight);

            var destRect = new Rectangle(0, 0, Convert.ToInt32(size.Width), Convert.ToInt32(size.Height));
            var destImage = new Bitmap(destRect.Width, destRect.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>The resize image.</summary>
        /// <param name="image">The image.</param>
        /// <param name="maxWidth">The max width.</param>
        /// <param name="maxHeight">The max height.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        public Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;

            var newWidth = (int)(image.Width * ratioX);
            var newHeight = (int)(image.Height * ratioY);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>
        /// Resize an image proportional
        /// </summary>
        /// <param name="image">the image to resize</param>
        /// <param name="maxWidth">maximum width to resize</param>
        /// <param name="maxHeight">maximum height to resize</param>
        /// <returns>returns the resized image</returns>
        public Image ResizeImageProportional(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>The get resized dimensions.</summary>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        /// <param name="maxWidth">The max width.</param>
        /// <param name="maxHeight">The max height.</param>
        /// <returns>The <see cref="SizeF"/>.</returns>
        public SizeF GetResizedDimensions(float actualWidth, float actualHeight, float maxWidth, float maxHeight, bool min = true)
        {
            var ratioX = (double)maxWidth / actualWidth;
            var ratioY = (double)maxHeight / actualHeight;
            var ratio = min ? Math.Min(ratioX, ratioY) : Math.Max(ratioX, ratioY);

            var newWidth = (int)(actualWidth * ratio);
            var newHeight = (int)(actualHeight * ratio);

            return new SizeF(newWidth, newHeight);
        }

        /// <summary>
        /// The get area from image.
        /// </summary>
        /// <param name="imageBytes">
        /// The image bytes.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetRectangleAreaFromImage(byte[] imageBytes, SizeF size, PointF point)
        {
            var full = new Bitmap(new MemoryStream(imageBytes));

            // Validations
            if (point.X < 0)
            {
                point.X = 0;
            }

            if (point.Y < 0)
            {
                point.Y = 0;
            }

            if (point.X + size.Width > full.Width)
            {
                size.Width = full.Width - point.X - 1;
            }

            if (point.Y + size.Height > full.Height)
            {
                size.Height = full.Height - point.Y - 1;
            }

            var srcRect = new RectangleF(point, size);
            var partial = (Bitmap)full.Clone(srcRect, full.PixelFormat);
            return partial;
        }

        /// <summary>
        /// The crop circular image.
        /// </summary>
        /// <param name="img">
        /// The img.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Bitmap CropCircularImage(Bitmap img)
        {
            int x = img.Width / 2;
            int y = img.Height / 2;
            int r = Math.Min(x, y);

            Bitmap tmp = null;
            tmp = new Bitmap(2 * r, 2 * r);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(tmp.Width / 2, tmp.Height / 2);
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0 - r, 0 - r, 2 * r, 2 * r);
                Region rg = new Region(gp);
                g.SetClip(rg, CombineMode.Replace);
                Bitmap bmp = new Bitmap(img);
                g.DrawImage(bmp, new Rectangle(-r, -r, 2 * r, 2 * r), new Rectangle(x - r, y - r, 2 * r, 2 * r), GraphicsUnit.Pixel);

            }


            return tmp;
        }
    }
}
