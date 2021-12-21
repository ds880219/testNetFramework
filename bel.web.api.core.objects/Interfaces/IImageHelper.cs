// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageHelper.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   The ImageHelper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Interfaces
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using bel.web.api.core.objects.Imaging;

    using ImageMagick;

    /// <summary>
    /// The ImageHelper interface.
    /// </summary>
    public interface IImageHelper
    {
        /// <summary>
        /// The arc angle.
        /// </summary>
        /// <param name="inputPath">
        /// The input path.
        /// </param>
        /// <param name="outputPath">
        /// The output path.
        /// </param>
        /// <param name="angle">
        /// The angle.
        /// </param>
        void ArcAngle(string inputPath, out string outputPath, double angle);

        /// <summary>The arc angle for full color without dot lines.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="angle">The angle.</param>
        void ArcAngleForFullColorWithoutDotLines(string inputPath, out string outputPath, double angle);

        /// <summary>The squish.</summary>
        /// <param name="image">The image.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        Image Squish(Image image, int percentage);

        /// <summary>
        /// The convert to one color.
        /// </summary>
        /// <param name="fileBytes">
        /// The file bytes.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="removeBg">
        /// The remove background.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] ConvertToOneColor(Stream fileBytes, string color, bool removeBg = true);

        /// <summary>
        /// The convert to one color.
        /// </summary>
        /// <param name="img">
        /// The image.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="removeBg">
        /// The remove background color.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] ConvertToOneColor(IMagickImage img, Color color, bool removeBg = true);

        /// <summary>
        /// The import text.
        /// </summary>
        /// <param name="pdfPath">
        /// The PDF file Path.
        /// </param>
        /// <param name="productImageSize">
        /// The product Image Size.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] AddImageToPdf(MemoryStream pdfPath, Size productImageSize, Image image, PointF point, SizeF size, string path, bool output = false);

        /// <summary>
        /// The get image format name.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetImageFormatName(ImageFormat format);

        /// <summary>
        /// The process histogram.
        /// </summary>
        /// <param name="imageHistogram">
        /// The image histogram.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<KeyValuePair<MagickColor, int>> ProcessHistogram(List<KeyValuePair<MagickColor, int>> imageHistogram);

        /// <summary>
        /// The get text image size.
        /// </summary>
        /// <param name="imageTextDetails">
        /// The image text details.
        /// </param>
        /// <returns>
        /// The <see cref="SizeF"/>.
        /// </returns>
        SizeF GetTextImageSize(TextDefinition imageTextDetails);

        /// <summary>
        /// The generate text image.
        /// </summary>
        /// <param name="imageTextDetails">
        /// The image text details.
        /// </param>
        /// <param name="destinationArea">
        /// The destination area.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="restrictions">
        /// The restrictions.
        /// </param>
        /// <returns>
        /// The <see cref="SizeF"/>.
        /// </returns>
        SizeF GenerateTextImage(
            TextDefinition imageTextDetails,
            SizeF? destinationArea,
            out Bitmap image,
            TextRestrictions restrictions = null);

        /// <summary>
        /// The make text image bytes.
        /// </summary>
        /// <param name="imageTextDetails">
        /// The image text details.
        /// </param>
        /// <param name="destinationArea">
        /// The destination area.
        /// </param>
        /// <param name="restrictions">
        /// The restrictions.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] MakeTextImageBytes(
            TextDefinition imageTextDetails,
            SizeF? destinationArea,
            TextRestrictions restrictions = null);

        /// <summary>
        /// The curve image text.
        /// </summary>
        /// <param name="imageText">
        /// The image text.
        /// </param>
        /// <param name="flip">
        /// The flip.
        /// </param>
        /// <param name="angle">
        /// The angle.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] CurveImageText(byte[] imageText, bool flip, double angle);

        /// <summary>
        /// The image to byte array.
        /// </summary>
        /// <param name="imageIn">
        /// The image in.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] ImageToByteArray(System.Drawing.Image imageIn);

        /// <summary>
        /// The trim image text.
        /// </summary>
        /// <param name="imageText">
        /// The image text.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] TrimImageText(byte[] imageText);

        /// <summary>
        /// The is vector file.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsVectorFile(MagickFormat format);

        /// <summary>
        /// The is EPS format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsEPS(MagickFormat format);

        /// <summary>
        /// The rotate image bytes.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="angle">
        /// The angle.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] RotateImageBytes(byte[] image, float angle);

        /// <summary>
        /// The rotate image.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="angle">
        /// The angle.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        Image RotateImage(byte[] image, float angle);

        /// <summary>
        /// The get background color.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="originalBgColor">
        /// The original background color.
        /// </param>
        /// <param name="cornerOffset">
        /// The corner offset.
        /// </param>
        /// <param name="colorDetailList">
        /// The color detail list.
        /// </param>
        /// <returns>
        /// The <see cref="Color?"/>.
        /// </returns>
        Color? GetBackgroundColor(
            IMagickImage image,
            out Color? originalBgColor,
            int cornerOffset = 1,
            List<ColorDetail> colorDetailList = null);

        /// <summary>
        /// The replace color.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="colorOld">
        /// The color old.
        /// </param>
        /// <param name="colorNew">
        /// The color new.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance.
        /// </param>
        /// <param name="totalPixelsReplaced">
        /// The total pixels replaced.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        Image ReplaceColor(Image image, Color colorOld, Color colorNew, int tolerance, out int totalPixelsReplaced);

        /// <summary>
        /// The fill background.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="colorNew">
        /// The color new.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        Image FillBackground(Image image, Color colorNew);

        /// <summary>
        /// The add image byte to image byte.
        /// </summary>
        /// <param name="imageSource">
        /// The image source.
        /// </param>
        /// <param name="productImageSource">
        /// The product image source.
        /// </param>
        /// <param name="imageToAdd">
        /// The image to add.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] AddImageByteToImageByte(
            MemoryStream imageSource,
            Size productImageSource,
            MemoryStream imageToAdd,
            PointF point,
            SizeF size);
    }
}
