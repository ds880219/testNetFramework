// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageConverter.cs" company="BEL USA">
//   This product is property of BEL USA
// </copyright>
// <summary>
//   Defines the ImageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Interfaces
{
    using System.Drawing;
    using System.IO;

    using ImageMagick;

    public interface IImageConverter
    {
        byte[] Base64ToByteArray(string base64String);

        /// <summary>The get image.</summary>
        /// <param name="path">The path.</param>
        /// <param name="isLocal">The is local.</param>
        /// <returns>The <see cref="Bitmap"/>.</returns>
        Bitmap GetImageFromFile(string path, bool isLocal = false);

        /// <summary>
        /// The get image magick from file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="isLocal">
        /// The is local.
        /// </param>
        /// <returns>
        /// The <see cref="IMagickImage"/>.
        /// </returns>
        IMagickImage GetImageMagickFromFile(string path, bool isLocal = false);

        /// <summary>The generate image from pdf 1.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        byte[] GenerateImageFromPdf(MemoryStream inputPath);
        byte[] GenerateImageFromPdf(string inputPath, string outputPath, bool outPut = false);

        /// <summary>The tif to pdf.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        void TifToPdf(string inputPath, out string outputPath);

        /// <summary>The byte array to image.</summary>
        /// <param name="byteArrayIn">The byte array in.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        Image ByteArrayToImage(byte[] byteArrayIn);
        byte[] ImageToByte(Image img);

        IMagickImage ImageFromUrl(string url, out bool isSVG, out byte[] bytes, bool excludeSvgValidation = false);
    }
}
