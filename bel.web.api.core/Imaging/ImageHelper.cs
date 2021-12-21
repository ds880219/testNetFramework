// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageHelper.cs" company="BEL USA">
//   This is product property of BEL USA.
// </copyright>
// <summary>
//   Defines the ImageHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using bel.web.api.core.Extensions;
    using bel.web.api.core.Font;
    using bel.web.api.core.objects.Imaging;
    using bel.web.api.core.objects.Interfaces;
    using bel.web.api.core.Utils;

    using ImageMagick;

    using ColorConverter = System.Windows.Media.ColorConverter;

    /// <summary>The image helper.</summary>
    public class ImageHelper : IImageHelper
    {
        /// <summary>
        /// The image resize.
        /// </summary>
        private readonly IImageResize imageResize;

        /// <summary>
        /// The color helper.
        /// </summary>
        private readonly IColorHelper colorHelper;

        /// <summary>
        /// Dictionary of known image formats
        /// </summary>
        private readonly Dictionary<Guid, string> knownImageFormats =
            (from p in typeof(ImageFormat).GetProperties(BindingFlags.Static | BindingFlags.Public)
             where p.PropertyType == typeof(ImageFormat)
             let value = (ImageFormat)p.GetValue(null, null)
             select new { value.Guid, Name = value.ToString() })
            .ToDictionary(p => p.Guid, p => p.Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageHelper"/> class.
        /// </summary>
        public ImageHelper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageHelper"/> class.
        /// </summary>
        /// <param name="imageResize">
        /// The image resize.
        /// </param>
        /// <param name="colorHelper">
        /// The color helper.
        /// </param>
        public ImageHelper(IImageResize imageResize, IColorHelper colorHelper)
        {
            this.imageResize = imageResize;
            this.colorHelper = colorHelper;
        }

        /// <summary>The arc angle.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="angle">The angle.</param>
        public void ArcAngle(string inputPath, out string outputPath, double angle)
        {
            var settings = new MagickReadSettings
            {
                  // Settings the density to 300 dpi will create an image with a better quality
                  Density = new Density(1000, 1000),
                  ColorSpace = ColorSpace.CMYK,

                  // ColorType = ColorType.TrueColor,
                  UseMonochrome = true
            };

            outputPath = string.Empty;

            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                var fileInfo = new FileInfo(inputPath);
                if (!fileInfo.Exists)
                {
                    return;
                }

                images.Read(fileInfo.FullName, settings);
                var first = images[0];

                // set the background of this image magic image to transparent
                first.BackgroundColor = MagickColors.White;

                // if any black gets added to the image magic image, lets also make
                // it transparent    
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // Apply the arc
                if (angle > 0)
                {
                    first.Rotate(180);
                    first.Distort(DistortMethod.Arc, new DistortSettings { Bestfit = true }, angle);
                    first.Rotate(180);
                }
                else if (angle < 0)
                {
                    angle = -1 * angle;
                    first.Distort(DistortMethod.Arc, new DistortSettings { Bestfit = true }, angle);
                }

                first.BackgroundColor = MagickColors.White;
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // first.BlackPointCompensation = true;
                first.Format = MagickFormat.Png64;

                // Save result as a pdf
                outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
                first.Depth = 1;

                first.Write(outputPath);
            }
        }

        /// <summary>The arc angle for full color without dot lines.</summary>
        /// <param name="inputPath">The input path.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="angle">The angle.</param>
        public void ArcAngleForFullColorWithoutDotLines(string inputPath, out string outputPath, double angle)
        {
            var settings = new MagickReadSettings
            {
                // Settings the density to 300 dpi will create an image with a better quality
                Density = new Density(1000, 1000)
            };

            // Read image from file
            using (var images = new MagickImageCollection())
            {
                images.Read(inputPath, settings);
                using (var image = (MagickImage)images[0])
                {
                    // Apply the arc
                    if (angle > 0)
                    {
                        image.Rotate(180);
                        image.Distort(DistortMethod.Arc, new DistortSettings { Bestfit = true }, angle);
                        image.Rotate(180);
                    }
                    else if (angle < 0)
                    {
                        angle = -1 * angle;
                        image.Distort(DistortMethod.Arc, new DistortSettings { Bestfit = true }, angle);
                        image.RePage();
                    }

                    image.BackgroundColor = MagickColors.Transparent;
                    image.VirtualPixelMethod = VirtualPixelMethod.Transparent;
                    image.Format = MagickFormat.Pdf;
                    
                    outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                    image.Write(outputPath);
                }
            }
        }

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
        public byte[] CurveImageText(byte[] imageText, bool flip, double angle)
        {
            var settings = new MagickReadSettings
            {
                // Settings the density to 300 dpi will create an image with a better quality
                Density = new Density(1000, 1000),
                ColorSpace = ColorSpace.CMYK,
                UseMonochrome = true
            };

            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(imageText, settings);
                var first = images[0];

                // set the background of this image magic image to transparent
                first.BackgroundColor = MagickColors.White;

                // if any black gets added to the image magic image, lets also make
                // it transparent
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // Apply the arc
                if (flip)
                {
                    first.Rotate(180);
                }

                first.Distort(DistortMethod.Arc, new DistortSettings {Bestfit = true}, angle);

                if (flip)
                {
                    first.Rotate(180);
                }

                first.BackgroundColor = MagickColors.White;
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                first.Trim();
                
                first.Format = MagickFormat.Png64;

                // Save result as a pdf
                first.Depth = 1;

                return first.ToByteArray();
            }
        }

        /// <summary>
        /// The trim image text.
        /// </summary>
        /// <param name="imageText">
        /// The image text.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] TrimImageText(byte[] imageText)
        {
            var settings = new MagickReadSettings
            {
                // Settings the density to 300 dpi will create an image with a better quality
                Density = new Density(1000, 1000),
                ColorSpace = ColorSpace.CMYK,
                UseMonochrome = true
            };

            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(imageText, settings);
                var first = images[0];

                // set the background of this image magic image to transparent
                first.BackgroundColor = MagickColors.White;

                // if any black gets added to the image magic image, lets also make
                // it transparent  
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;
               
                first.BackgroundColor = MagickColors.White;
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                first.Format = MagickFormat.Png64;
                first.Trim(new Percentage(100));

                // Save result as a pdf
                first.Depth = 1;

                return first.ToByteArray();
            }
        }

        /// <summary>The squish.</summary>
        /// <param name="image">The image.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns>The <see cref="Image"/>.</returns>
        public Image Squish(Image image, int percentage)
        {
            var currentHeight = image.Height;
            var squishHeight = currentHeight * ((100 - (float)percentage) / 100);
            var squishImage = this.imageResize.ResizeImage(image, image.Width, Convert.ToInt32(squishHeight));
            squishImage.Save(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png"), ImageFormat.Png);
            return squishImage;
        }

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
        public byte[] ConvertToOneColor(Stream fileBytes, string color, bool removeBg = true)
        {
            var readSettings = new MagickReadSettings
                                   {
                                       Density = new Density(300, 300)          
                                   };
            
            var img = new MagickImage(fileBytes, readSettings)
                          {
                              Quality = 300
                          };

            readSettings.ExtractArea = new MagickGeometry(img.Width - 1, img.Height - 1);
            img.Format = MagickFormat.Png;

            if (removeBg)
            {
                img.FloodFill(MagickColors.Transparent, 0, 0);
            }

            img.ColorFuzz = new Percentage(100);
            img.Colorize(new MagickColor(color), new Percentage(100));

            using (var ms = new MemoryStream())
            {
                img.Write(ms, MagickFormat.Png);
                return ms.ToArray();
            }
        }

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
        /// The remove background.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] ConvertToOneColor(IMagickImage img, Color color, bool removeBg = true)
        {
            img.Format = MagickFormat.Png;

            if (removeBg)
            {
                img.FloodFill(MagickColors.Transparent, 0, 0);
            }

            img.ColorFuzz = new Percentage(100);
            img.Colorize(new MagickColor(color), new Percentage(100));

            using (var ms = new MemoryStream())
            {
                img.Write(ms, MagickFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// The import text.
        /// </summary>
        /// <param name="pdfPath">
        /// The PDF Path.
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
        public byte[] AddImageToPdf(MemoryStream pdfPath, Size productImageSize, Image image, PointF point, SizeF size, string path, bool output = false)
        {
            var bmpOriginal = Image.FromStream(pdfPath);
            var bmp = new Bitmap(bmpOriginal, productImageSize);
            using (var g = Graphics.FromImage(bmp)) 
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(image, new RectangleF(point, size));
                path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.PNG");
                bmp.Save(path, ImageFormat.Png);
                g.Dispose();
            }

            // Save the output document locally.
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                bmp.Dispose();
                return ms.ToArray();
            }
        }

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
        public byte[] AddImageByteToImageByte(MemoryStream imageSource, Size productImageSource, MemoryStream imageToAdd, PointF point, SizeF size)
        {

            var bmpSource = Image.FromStream(imageSource);
            var bmp = new Bitmap(bmpSource, productImageSource);

            var imgToAdd = Image.FromStream(imageToAdd);
            var newSize = this.imageResize.GetResizedDimensions(
                imgToAdd.Width,
                imgToAdd.Height,
                size.Width,
                size.Height,
                false);

            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(imgToAdd, new RectangleF(point, newSize));
                g.Dispose();
            }

            // Save the output document locally.
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                bmp.Dispose();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// The get image format name.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetImageFormatName(ImageFormat format)
		{
			string name;
			if (this.knownImageFormats.TryGetValue(format.Guid, out name))
				return name;
			return null;
		}

        /// <summary>
        /// Gets a list of the main histogram colors using Standard Deviation
        /// </summary>
        /// <param name="imageHistogram">
        /// Source Histogram
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<KeyValuePair<MagickColor, int>> ProcessHistogram(List<KeyValuePair<MagickColor, int>> imageHistogram)
        {
            var stdDeviationNew = imageHistogram.Select(v => (double)v.Value).StandardDeviation();
            return imageHistogram.Where(c => c.Value >= stdDeviationNew).ToList();

            // order the list by color pixel amount
            var originalHistogram = imageHistogram.OrderByDescending(v => v.Value).ToList();

            // store the primary color
            var primaryColor = originalHistogram.Take(1).ToList();

            // remove the primary color (we will add it back in later)
            originalHistogram.RemoveAt(0);

            // reverse the list so it is lowest first
            originalHistogram.Reverse();

            // find the biggest difference between the colors
            int max = int.MinValue;
            int delta = 0;
            int index = 0;
            for (int i = 1; i < originalHistogram.Count(); i++)
            {
                delta = originalHistogram[i].Value - originalHistogram[i - 1].Value;
                if (delta > max)
                {
                    max = delta;
                    index = i - 1;
                }
            }

            // create a list of primary colors
            var primaryColorsHistogram = originalHistogram.Take(originalHistogram.Count()).ToList();

            // remove the colors that are not primary
            primaryColorsHistogram.RemoveRange(0, index);

            // create a list of all the secondary colors
            var otherColors = originalHistogram.Take(index).ToList();

            // find the standard deviation of the secondary colors
            var stdDeviation = otherColors.Select(v => (double)v.Value).StandardDeviation();

            // create a list of secondary colors
            var secondaryHistogram = otherColors.Where(v => v.Value > stdDeviation).ToList();

            // use the primary histogram for our return
            var retHistogram = primaryColorsHistogram;

            // add the secondary colors back in
            retHistogram.AddRange(secondaryHistogram);

            // add the primary color back in
            retHistogram.AddRange(primaryColor);

            return retHistogram;
        }

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
        public byte[] MakeTextImageBytes(TextDefinition imageTextDetails, SizeF? destinationArea,
            TextRestrictions restrictions = null)
	    {
	        var size = this.GenerateTextImage(imageTextDetails, destinationArea, out var image, restrictions);
            return ImageToByteArray(image);
        }

        /// <summary>
        /// The get text image size.
        /// </summary>
        /// <param name="imageTextDetails">
        /// The image text details.
        /// </param>
        /// <returns>
        /// The <see cref="SizeF"/>.
        /// </returns>
        public SizeF GetTextImageSize(TextDefinition imageTextDetails)
	    {
            var converter = new UnitsHelper();

	        // Checking font and style
	        Font font = null;
	        var alignment = StringAlignment.Near;
	       
	        // Text Font
	        var sizeFloat = (float)Convert.ToDouble(imageTextDetails.FontSize);
	        imageTextDetails.FontSize = converter.ToPoints(sizeFloat, UnitType.Pixels);
	        imageTextDetails.FontFamily = FontHelper.GetFont(imageTextDetails.FontFamily);

	        if (imageTextDetails.FontWeight == "bold")
	        {
	            imageTextDetails.FontStyle = imageTextDetails.FontStyle.Trim().ToLower() == "italic" ? "bolditalic" : "bold";
            }
            
	        // Font Family
	        if (!string.IsNullOrEmpty(imageTextDetails.FontFamily))
	        {
	            if (!string.IsNullOrEmpty(imageTextDetails.FontStyle))
	            {
	                switch (imageTextDetails.FontStyle.Trim().ToLower())
	                {
	                    case "regular":
	                        {
	                            font = new Font(
	                                $"{imageTextDetails.FontFamily}",
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Regular);
                                break;
	                        }

	                    case "bold":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Bold);
                                break;
	                        }

	                    case "bolditalic":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Bold | FontStyle.Italic);
                                break;
	                        }

	                    case "italic":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Italic);
                                break;
	                        }

	                    case "strikeout":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Strikeout);
                                break;
	                        }

	                    case "underline":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Underline);
                                break;
	                        }

	                    default:
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Regular);
	                            break;
	                        }
	                }
	            }
	            else
	            {
	                font = new Font(
	                    imageTextDetails.FontFamily,
	                    (float)Convert.ToDouble(imageTextDetails.FontSize),
	                    FontStyle.Regular);
	            }
	        }

	        // Text Alignment
            if (!string.IsNullOrEmpty(imageTextDetails.TextAlign))
            {
                switch (imageTextDetails.TextAlign.ToUpper())
                {
                    case "C":
                    case "CENTER":
                        alignment = StringAlignment.Center;
                        break;
                    case "R":
                    case "RIGHT":
                        alignment = StringAlignment.Far;
                        break;
                    case "L":
                    case "LEFT":
                        alignment = StringAlignment.Near;
                        break;
                }
            }

            var sf = new StringFormat { Alignment = alignment};

            // Create a blank bitmap.
            // first, create a dummy bitmap just to get a graphics object
            var img = new Bitmap(1, 1);
            img.SetResolution(1000, 1000);
            var drawing = Graphics.FromImage(img);

            // measure the string to see how big the image needs to be
            var textSize = drawing.MeasureString(imageTextDetails.Text, font, new PointF(0, 0), sf);

            // free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            return textSize;
        }

        /// <summary>
        /// The get text image size.
        /// </summary>
        /// <param name="imageTextDetails">
        /// The image text details.
        /// </param>
        /// <param name="alignment">
        /// The alignment.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <returns>
        /// The <see cref="SizeF"/>.
        /// </returns>
        public SizeF GetTextImageSize(TextDefinition imageTextDetails, out StringAlignment alignment, out Font font)
	    {
            var converter = new UnitsHelper();

	        // Checking font and style
	        font = null;
	        alignment = StringAlignment.Near;
	       
	        // Text Font
	        var sizeFloat = (float)Convert.ToDouble(imageTextDetails.FontSize);
	        imageTextDetails.FontSize = converter.ToPoints(sizeFloat, UnitType.Pixels);
	        imageTextDetails.FontFamily = FontHelper.GetFont(imageTextDetails.FontFamily);

	        if (imageTextDetails.FontWeight == "bold")
	        {
	            imageTextDetails.FontStyle = imageTextDetails.FontStyle.Trim().ToLower() == "italic" ? "bolditalic" : "bold";
            }
            
	        // Font Family
	        if (!string.IsNullOrEmpty(imageTextDetails.FontFamily))
	        {
	            if (!string.IsNullOrEmpty(imageTextDetails.FontStyle))
	            {
	                switch (imageTextDetails.FontStyle.Trim().ToLower())
	                {
	                    case "regular":
	                        {
	                            font = new Font(
	                                $"{imageTextDetails.FontFamily}",
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Regular);
                                break;
	                        }

	                    case "bold":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Bold);
                                break;
	                        }

	                    case "bolditalic":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Bold | FontStyle.Italic);
                                break;
	                        }

	                    case "italic":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Italic);
                                break;
	                        }

	                    case "strikeout":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Strikeout);
                                break;
	                        }

	                    case "underline":
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Underline);
                                break;
	                        }

	                    default:
	                        {
	                            font = new Font(
	                                imageTextDetails.FontFamily,
	                                (float)Convert.ToDouble(imageTextDetails.FontSize),
	                                FontStyle.Regular);
	                            break;
	                        }
	                }
	            }
	            else
	            {
	                font = new Font(
	                    imageTextDetails.FontFamily,
	                    (float)Convert.ToDouble(imageTextDetails.FontSize),
	                    FontStyle.Regular);
	            }
	        }

	        // Text Alignment
            if (!string.IsNullOrEmpty(imageTextDetails.TextAlign))
            {
                switch (imageTextDetails.TextAlign.ToUpper())
                {
                    case "C":
                    case "CENTER":
                        alignment = StringAlignment.Center;
                        break;
                    case "R":
                    case "RIGHT":
                        alignment = StringAlignment.Far;
                        break;
                    case "L":
                    case "LEFT":
                        alignment = StringAlignment.Near;
                        break;
                }
            }

            var sf = new StringFormat { Alignment = alignment};

            // Create a blank bitmap.
            // first, create a dummy bitmap just to get a graphics object
            var img = new Bitmap(1, 1);
            img.SetResolution(1000, 1000);
            var drawing = Graphics.FromImage(img);

            // Fix fonts with issues
            var fontToCalculateSize = FontHelper.ReviseFontToCalculateSize(font);

            // measure the string to see how big the image needs to be
            var textSize = drawing.MeasureString(imageTextDetails.Text, fontToCalculateSize, new PointF(0, 0), sf);

            // free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            return textSize;
        }

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
        public SizeF GenerateTextImage(TextDefinition imageTextDetails, SizeF? destinationArea, out Bitmap image,
            TextRestrictions restrictions = null)
	    {
            StringAlignment alignment;
            Font font;
            var originalFontSize = imageTextDetails.FontSize;

            // validate I receive a fontSize minimum than the restricted one
            if (restrictions != null && restrictions.SameFontFormat && imageTextDetails.FontSize < restrictions.MinFontSize)
            {
                imageTextDetails.FontSize = restrictions.MinFontSize;
            }

            // measure the string to see how big the image needs to be
            var textSize = GetTextImageSize(imageTextDetails, out alignment, out font);
            
            // trying to adjust new text to fit on destinationArea
            if (destinationArea.HasValue)
            {
                // we need decrease font
                while (textSize.Width > destinationArea.Value.Width || textSize.Height > destinationArea.Value.Height)
                {
                    // validate 
                    if (restrictions != null && (originalFontSize - 2 < restrictions.MinFontSize))
                    {
                        if (restrictions.SameFontFormat)
                        {
                            originalFontSize = restrictions.MinFontSize;
                            imageTextDetails.FontSize = restrictions.MinFontSize;
                            textSize = GetTextImageSize(imageTextDetails, out alignment, out font);
                        }

                        break;
                    }

                    originalFontSize = originalFontSize - 2;
                    imageTextDetails.FontSize = originalFontSize;
                    textSize = GetTextImageSize(imageTextDetails, out alignment, out font);
                }
            }

            // Color
            Brush brush = null;
            if (!string.IsNullOrEmpty(imageTextDetails.ConvertedColor?.HexValue))
            {
                if (imageTextDetails.ConvertedColor.HexValue.ToLower().Contains("selectedcolor"))
                {
                    var newColor = imageTextDetails.ConvertedColor.HexValue.Replace("selectedcolor", string.Empty);
                    imageTextDetails.ConvertedColor.HexValue = newColor.Trim();
                }

                try
                {
                    var convertedColor =
                        (System.Windows.Media.Color)ColorConverter.ConvertFromString(
                            imageTextDetails.ConvertedColor.HexValue);
                    brush = new SolidBrush(
                        Color.FromArgb(convertedColor.A, convertedColor.R, convertedColor.G, convertedColor.B));
                }
                catch (Exception exception)
                {
                }
            }
            
            // create a new image of the right size
            var img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            img.SetResolution(1000, 1000);
            using (var g = Graphics.FromImage(img))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.TextContrast = 4;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingMode = CompositingMode.SourceOver;
                g.PageUnit = GraphicsUnit.Display;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;

                var sf = new StringFormat { Alignment = alignment};
                g.DrawString(imageTextDetails.Text, font, brush, new RectangleF(new PointF(0, 0), textSize), sf);
                g.Save();
                image = img;
            }

            imageTextDetails.FontSize = originalFontSize;
            return image.Size;
        }

        /// <summary>
        /// The image to byte array.
        /// </summary>
        /// <param name="imageIn">
        /// The image in.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// The is vector file.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsVectorFile(MagickFormat format)
        {
            return format != MagickFormat.Png && format != MagickFormat.Jpg
                                              && format != MagickFormat.Jpeg
                                              && format != MagickFormat.Gif;
        }

        /// <summary>
        /// The is eps.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEPS(MagickFormat format)
        {
            return format == MagickFormat.Ept || format == MagickFormat.Ept2
                                              || format == MagickFormat.Ept3;
        }

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
        public byte[] RotateImageBytes(byte[] image, float angle)
        {

            var settings = new MagickReadSettings
            {
                // Settings the density to 300 dpi will create an image with a better quality
                Density = new Density(1000, 1000),
                ColorSpace = ColorSpace.CMYK,
                UseMonochrome = true
            };

            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(image, settings);
                var first = images[0];

                // set the background of this image magic image to transparent
                first.BackgroundColor = MagickColors.Transparent;

                // if any black gets added to the image magic image, lets also make
                // it transparent    `
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // Apply the arc
                first.Rotate(angle);

                first.BackgroundColor = MagickColors.White;
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // first.Trim();
                // first.BlackPointCompensation = true;
                first.Format = MagickFormat.Png64;

                // Save result as a pdf
                first.Depth = 1;

                return first.ToByteArray();
            }
        }

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
        public Image RotateImage(byte[] image, float angle)
        {

            var settings = new MagickReadSettings
            {
                // Settings the density to 300 dpi will create an image with a better quality
                Density = new Density(1000, 1000),
                ColorSpace = ColorSpace.CMYK,
                UseMonochrome = true
            };

            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(image, settings);
                var first = images[0];

                // set the background of this image magic image to transparent
                first.BackgroundColor = MagickColors.Transparent;

                // if any black gets added to the image magic image, lets also make
                // it transparent    `
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // Apply the arc
                first.Rotate(angle);

                first.BackgroundColor = MagickColors.White;
                first.VirtualPixelMethod = VirtualPixelMethod.Transparent;

                // first.Trim();
                // first.BlackPointCompensation = true;
                first.Format = MagickFormat.Png64;

                // Save result as a pdf
                first.Depth = 1;

                return first.ToBitmap();
            }
        }

        /// <summary>
        /// The replace color.
        /// </summary>
        /// <param name="image">
        /// The _image.
        /// </param>
        /// <param name="colorOld">
        /// The _color old.
        /// </param>
        /// <param name="colorNew">
        /// The _color new.
        /// </param>
        /// <param name="tolerance">
        /// The _tolerance.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image ReplaceColor(Image image, Color colorOld, Color colorNew, int tolerance, out int totalPixelsReplaced)
        {
            var bitmap = (Bitmap)image.Clone();
            totalPixelsReplaced = 0;

            // Defining Tolerance
            // R
            var iRMin = Math.Max(colorOld.R - tolerance, 0);
            var iRMax = Math.Min(colorOld.R + tolerance, 255);

            // G
            var iGMin = Math.Max(colorOld.G - tolerance, 0);
            var iGMax = Math.Min(colorOld.G + tolerance, 255);

            // B
            var iBMin = Math.Max(colorOld.B - tolerance, 0);
            var iBMax = Math.Min(colorOld.B + tolerance, 255);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var c = bitmap.GetPixel(x, y);

                    // Determining Color Match
                    if (c.R < iRMin || c.R > iRMax || c.G < iGMin || c.G > iGMax || c.B < iBMin || c.B > iBMax)
                    {
                        continue;
                    }

                    totalPixelsReplaced++;
                    bitmap.SetPixel(
                        x,
                        y,
                        colorNew == Color.Transparent ? Color.FromArgb(0, colorNew.R, colorNew.G, colorNew.B) : Color.FromArgb(c.A, colorNew.R, colorNew.G, colorNew.B));
                }
            }

            return (Image)bitmap.Clone();
        }

        public Image FillBackgorund(Image image, Color c)
        {
            var bmp = (Bitmap)image.Clone();

            Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(bmp2))
            {
                g.FillRectangle(
                    Brushes.White, 0, 0, bmp.Width, bmp.Height);

                for (var x = 0; x < bmp.Width; x++)
                {
                    for (var y = 0; y < bmp.Height; y++)
                    {
                        var pixel = bmp.GetPixel(x, y);

                        // Determining Color Match
                        if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0 && pixel.A == 0)
                        {
                            continue;
                        }

                        bmp2.SetPixel(x, y, pixel);
                    }
                }
            }

            return (Image)bmp2.Clone();
        }

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
        public Color? GetBackgroundColor(
            IMagickImage image,
            out Color? originalBgColor,
            int cornerOffset = 1,
            List<ColorDetail> colorDetailList = null)
        {
            var pixels = image.GetPixels();

            // Getting The Background Color by checking Corners of Original Image
            var corners = new[]
                              {
                                  new Point(cornerOffset, cornerOffset),
                                  new Point(cornerOffset, image.Height - cornerOffset),
                                  new Point(image.Width - cornerOffset, cornerOffset),
                                  new Point(image.Width - cornerOffset, image.Height - cornerOffset)
                              }; // four corners (Top, Left), (Top, Right), (Bottom, Left), (Bottom, Right)

            for (var i = 0; i < 4; i++)
            {
                var cornerMatched = 0;
                var backColor = pixels.GetPixel(corners[i].X, corners[i].Y).ToColor().ToColor();
                originalBgColor = pixels.GetPixel(corners[i].X, corners[i].Y).ToColor().ToColor();
                if (backColor.Name == "0")
                {
                    backColor = Color.Transparent;
                }

                backColor = this.colorHelper.GetApproximateColorName(backColor);
                for (var j = 0; j < 4; j++)
                {
                    var cornerColor =
                        pixels.GetPixel(corners[j].X, corners[j].Y).ToColor().ToColor(); // Check RGB with some offset
                    if (cornerColor.Name == "0")
                    {
                        cornerColor = Color.Transparent;
                    }

                    cornerColor = this.colorHelper.GetApproximateColorName(cornerColor);

                    if (cornerColor.Equals(backColor))
                    {
                        cornerMatched++;
                    }
                }

                if (cornerMatched <= 2)
                {
                    continue;
                }

                if (colorDetailList == null)
                {
                    return backColor;
                }

                var compareList = colorDetailList.Select(item => Color.FromArgb(item.MappedColor.A, item.MappedColor.R, item.MappedColor.G, item.MappedColor.B)).ToList();

                // find the closest color that we have in our list of colors
                var matchedBackgroundColor =
                    compareList[this.colorHelper.ClosestRgbColor(compareList, backColor, out _)];

                return matchedBackgroundColor;
            }

            originalBgColor = null;
            return null;
        }

        /// <summary>
        /// The fill background.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image FillBackground(Image image, Color c)
        {
            var bmp = (Bitmap)image.Clone();

            var bmp2 = new Bitmap(bmp.Width, bmp.Height);
            using (var g = Graphics.FromImage(bmp2))
            {
                g.FillRectangle(
                    Brushes.White, 0, 0, bmp.Width, bmp.Height);

                for (var x = 0; x < bmp.Width; x++)
                {
                    for (var y = 0; y < bmp.Height; y++)
                    {
                        var pixel = bmp.GetPixel(x, y);

                        // Determining Color Match
                        if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0 && pixel.A == 0)
                        {
                            continue;
                        }

                        bmp2.SetPixel(x, y, pixel);
                    }
                }
            }

            return (Image)bmp2.Clone();
        }
    }   
}
