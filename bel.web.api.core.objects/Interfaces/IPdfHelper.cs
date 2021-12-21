using System.Drawing;
using System.IO;
using PdfSharp.Drawing;

namespace bel.web.api.core.objects.Interfaces
{
    public interface IPdfHelper
    {
        /// <summary>The create blank pdf template.</summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        byte[] CreateBlankPdfTemplate(float width, float height);

        /// <summary>The add image to pdf.</summary>
        /// <param name="pdfPath">The pdf path.</param>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <param name="isTest">The is test.</param>
        /// <returns>The <see cref="string"/>.</returns>
        byte[] AddImageToPdf(MemoryStream pdfPath, XImage image, PointF point, SizeF size, string path,
            bool output = false);

        byte[] GenerateImageFromPdf(string inputPath, string outputPath, bool outPut = false);
    }
}
