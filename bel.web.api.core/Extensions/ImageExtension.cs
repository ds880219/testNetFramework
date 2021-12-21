namespace bel.web.api.core.Extensions
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    public static class ImageExtension
    {
        public static Image ImageTo8Bpp(this Image image)
        {
            using (var bitmap = new Bitmap(image))
            using (var stream = new MemoryStream())
            {
                var parameters =
                    new EncoderParameters(1) {Param = {[0] = new EncoderParameter(Encoder.ColorDepth, 8L)}};

                var info = GetEncoderInfo("image/tiff");
                bitmap.Save(stream, info, parameters);

                return Image.FromStream(stream);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}
