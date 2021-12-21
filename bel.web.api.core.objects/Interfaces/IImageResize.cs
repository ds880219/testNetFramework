// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageResize.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the IImageResize type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Interfaces
{
    using System.Drawing;

    /// <summary>
    /// The ImageResize interface.
    /// </summary>
    public interface IImageResize
    {
        /// <summary>
        /// The resize image.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="maxWidth">
        /// The max width.
        /// </param>
        /// <param name="maxHeight">
        /// The max height.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        Bitmap ResizeImage(Image image, int width, int height, int maxWidth, int maxHeight);

        /// <summary>The resize image.</summary>
        /// <param name="image">The image.</param>
        /// <param name="maxWidth">The max width.</param>
        /// <param name="maxHeight">The max height.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        Image ResizeImage(Image image, int maxWidth, int maxHeight);

        /// <summary>
        /// Resize an image proportional
        /// </summary>
        /// <param name="image">the image to resize</param>
        /// <param name="maxWidth">maximum width to resize</param>
        /// <param name="maxHeight">maximum height to resize</param>
        /// <returns>returns the resized image</returns>
        Image ResizeImageProportional(Image image, int maxWidth, int maxHeight);

        /// <summary>The get resized dimensions.</summary>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        /// <param name="maxWidth">The max width.</param>
        /// <param name="maxHeight">The max height.</param>
        /// <returns>The <see cref="SizeF"/>.</returns>
        SizeF GetResizedDimensions(float actualWidth, float actualHeight, float maxWidth, float maxHeight, bool min = true);

        /// <summary>
        /// The get rectangle area from image.
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
        Bitmap GetRectangleAreaFromImage(byte[] imageBytes, SizeF size, PointF point);

        /// <summary>
        /// The crop circular image.
        /// </summary>
        /// <param name="img">
        /// The img.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        Bitmap CropCircularImage(Bitmap img);
    }
}
