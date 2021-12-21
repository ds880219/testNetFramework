// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdfHelper.cs" company="BEL USA">
//   This is product property of BEL USA.
// </copyright>
// <summary>
//   Defines the PdfHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing.Imaging;
using bel.web.api.core.objects.Interfaces;
using ImageMagick;

namespace bel.web.api.core.Pdf
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.IO;

    using bel.web.api.core.Utils;

    using iTextSharp.text;
    using iTextSharp.text.pdf;

    using PdfSharp.Drawing;
    using PdfSharp.Pdf.IO;

    using PdfDocument = PdfSharp.Pdf.PdfDocument;
    using PdfReader = PdfSharp.Pdf.IO.PdfReader;
    using Rectangle = iTextSharp.text.Rectangle;

    /// <summary>The pdf helper.</summary>
    public class PdfHelper : IPdfHelper
    {
        /// <summary>The create blank pdf template.</summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="savePath">The save path.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public byte[] CreateBlankPdfTemplate(float width, float height)
        {
            try
            {
                // Creating a blank pdf document based on the size in the template header object.

                // Validating the template has a valid size
                if (width <= 0 || height <= 0)
                {
                    // return false;
                    return null;
                }

                // Validating page size
                if (width > 14400 || height > 14400)
                {
                    return null;
                }
                
                using (MemoryStream ms = new MemoryStream())
                {
                    var pageSize = new Rectangle(width, height)
                    {
                       BackgroundColor = BaseColor.WHITE
                    };
                    var doc = new Document(pageSize);
                    var writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    doc.NewPage();
                    doc.Add(new Chunk());
                    doc.Close();
                    writer.Close();

                    return ms.ToArray();
                }

            }
            catch (Exception exception)
            {
                return null;
            }
        }

        /// <summary>The add image to pdf.</summary>
        /// <param name="pdfPath">The pdf path.</param>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <param name="isTest">The is test.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public byte[] AddImageToPdf(MemoryStream pdfPath, XImage image, PointF point, SizeF size, string path, bool output = false)
        {
            // Open the blank pdf.
            var baseDocument = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);

            // Creating the output pdf.
            var outputDocument = new PdfDocument();
            outputDocument.Info.Title = "Preview";

            outputDocument.Options.CompressContentStreams = true;
            outputDocument.Options.NoCompression = false;

            // Unit conversion instance helper
            var unitConversion = new UnitsHelper();

            // Creating page in the output pdf based on tje  input pdf dimensions.
            var page = outputDocument.AddPage(baseDocument.Pages[0]);
            var gfx = XGraphics.FromPdfPage(page);

            gfx.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            gfx.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            gfx.SmoothingMode = XSmoothingMode.HighQuality;
            gfx.Graphics.TextContrast = 4;
            gfx.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            if (image != null)
            {
                image.Interpolate = true;
                gfx.DrawImage(image, new RectangleF(point, size));
            }
            
            // Save the output document locally
            outputDocument.Version = 15;
            if (output)
            {
                outputDocument.Save(path);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                outputDocument.Save(ms, false);
                return ms.ToArray();
            }
        }

        public byte[] GenerateImageFromPdf(string inputPath, string outputPath, bool outPut = false)
        {

            try
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

                if (outPut)
                {
                    img.ToBitmap().Save(outputPath, ImageFormat.Jpeg);
                }

                using (var ms = new MemoryStream())
                {
                    img.Write(ms, MagickFormat.Png);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
