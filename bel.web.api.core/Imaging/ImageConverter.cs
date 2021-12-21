// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageConverter.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the ImageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;

    using bel.web.api.core.objects.Interfaces;

    using ImageMagick;

    /// <summary>
    /// The image converter.
    /// </summary>
    public class ImageConverter : IImageConverter
    {
        /// <summary>
        /// The base 64 to byte array.
        /// </summary>
        /// <param name="base64String">
        /// The base 64 string.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] Base64ToByteArray(string base64String)
        {
            if (base64String.Contains("data:"))
            {
                // remove the data stuff to get just the image
                base64String = base64String.Remove(0, base64String.IndexOf(",") + 1);
            }

            // Convert base 64 string to byte[]
            return Convert.FromBase64String(base64String);
        }

        /// <summary>The get image.</summary>
        /// <param name="path">The path.</param>
        /// <param name="isLocal">The is local.</param>
        /// <returns>The <see cref="Bitmap"/>.</returns>
        public Bitmap GetImageFromFile(string path, bool isLocal = false)
        {
            MagickImage image;

            var readSettings = new MagickReadSettings();
            
            // generate new image base on url or base64 string
            if (isLocal)
            {
                image = new MagickImage(path, readSettings)
                {
                    Quality = 100
                };
            }
            else
            {
                var wc = new WebClient();
                var bytes = wc.DownloadData(path);
                var ms = new MemoryStream(bytes);
                if (path.IndexOf("svg-xml") > 0)
                {
                    readSettings.Format = MagickFormat.Svg;
                }

                image = new MagickImage(ms, readSettings) { Quality = 100 };
            }

            var format = image.ColorSpace == ColorSpace.CMYK ? ImageFormat.Jpeg : ImageFormat.Png;
            return image.ToBitmap(format);
        }

        public IMagickImage GetImageMagickFromFile(string path, bool isLocal = false)
        {
            IMagickImage image;

            var readSettings = new MagickReadSettings();

            // generate new image base on url or base64 string
            if (isLocal)
            {
                image = new MagickImage(path, readSettings)
                            {
                                Quality = 100
                            };
            }
            else
            {
                var wc = new WebClient();
                var bytes = wc.DownloadData(path);
                var ms = new MemoryStream(bytes);
                if (path.IndexOf("svg-xml") > 0)
                {
                    readSettings.Format = MagickFormat.Svg;
                }

                image = new MagickImage(ms, readSettings) { Quality = 100 };
            }

            return image;
        }

        /// <summary>The generate image from pdf 1.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        public byte[] GenerateImageFromPdf(MemoryStream inputPath)
        {
            // MagickNET.SetGhostscriptDirectory(@"C:\Program Files\gs1\gs9.25\bin");
            // MagickNET.SetGhostscriptFontDirectory(@"C:\Windows\Fonts");
            var settings = new MagickReadSettings
            {
                  Density = new Density(300, 300),
                  Format = MagickFormat.Pdf
            };

            using (var images = new MagickImageCollection())
            {
                images.Read(inputPath, settings);
                var img = images[0];
                
                // -fuzz XX%
                img.ColorFuzz = new Percentage(10);
                settings.ExtractArea = new MagickGeometry(img.Width - 1, img.Height - 1);
               
                // -transparent white
                img.Transparent(MagickColors.White);
                
                using (var ms = new MemoryStream())
                {
                    img.Write(ms, MagickFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// The generate image from pdf.
        /// </summary>
        /// <param name="inputPath">
        /// The input path.
        /// </param>
        /// <param name="outputPath">
        /// The output path.
        /// </param>
        /// <param name="outPut">
        /// The out put.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] GenerateImageFromPdf(string inputPath, string outputPath, bool outPut = false)
        {
            var readSettings = new MagickReadSettings
            {
                Density = new Density(300, 300)
            };
            
            var img = new MagickImage(inputPath, readSettings)
            {
                Quality = 300
            };

            readSettings.ExtractArea = new MagickGeometry(img.Width - 1, img.Height - 1);
            img.Format = MagickFormat.Png;
            
            using (var ms = new MemoryStream())
            {
                img.Write(ms, MagickFormat.Png);
                return ms.ToArray();
            }
        }
        

        /// <summary>The tif to pdf.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        public void TifToPdf(string inputPath, out string outputPath)
        {
            // Read image from file
            using (var image = new MagickImage(inputPath))
            {
                image.Format = MagickFormat.Pdf;
                outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                image.Write(outputPath);
            }
        }

        /// <summary>The byte array to image.</summary>
        /// <param name="byteArrayIn">The byte array in.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
            ms.Seek(0, SeekOrigin.Begin);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public byte[] ImageToByte(Image img)
        {
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public IMagickImage ImageFromUrl(string url, out bool isSVG, out byte[] bytes, bool excludeSvgValidation = false)
        {
            isSVG = false;
            var readSettings = new MagickReadSettings();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var wc = new WebClient();
            bytes = wc.DownloadData(url);

            if (url.IndexOf("svg-xml") > 0)
            {
                readSettings.Format = MagickFormat.Svg;
                isSVG = true;
            }
            else if (url.Trim().ToLower().EndsWith("svg") && !excludeSvgValidation)
            {
                readSettings.Format = MagickFormat.Svg;
                isSVG = true;
            }

            var image = new MagickImage(bytes, readSettings)
            {
                Quality = 100
            };

            return image;
        }
    }
}
