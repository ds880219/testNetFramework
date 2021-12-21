using System.IO;
using iTextSharp.text.pdf;

namespace bel.web.api.core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;

    using bel.web.api.core.objects.Enums;
    using bel.web.api.core.objects.Imaging;
    using bel.web.api.core.objects.Interfaces;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    public static class BitmapExtension
    {
        /// <summary>
        /// Remove the background from an image
        /// </summary>
        /// <param name="image">The bitmap image to remove the background from</param>
        /// <param name="backgroundType">The type of background</param>
        /// <param name="removalType">Remove the entire background if fill, remove outside background if external</param>
        /// <param name="lowerThreshold">The lower color threshold (0-255) default:0</param>
        /// <param name="upperThreshold">The uppoer color threshold (0-255) default:255</param>
        /// <returns>The original bitmap image with the background removed</returns>
        public static Bitmap RemoveBackground(this Bitmap image, Imaging.BackgroundColorType backgroundType, Imaging.BackgroundRemovalType removalType = Imaging.BackgroundRemovalType.Fill, int lowerThreshold = 0, int upperThreshold = 255)
        {
            Image<Bgra, Byte> imgInput = new Image<Bgra, Byte>((Bitmap)image);
            Image<Gray, byte> imgGrayscale = null;


            //convert the image to grayscale
            switch (backgroundType)
            {
                case Imaging.BackgroundColorType.Dark:
                    imgGrayscale = imgInput.Convert<Gray, byte>().ThresholdBinary(new Gray(lowerThreshold), new Gray(upperThreshold));//50-255
                    break;
                case Imaging.BackgroundColorType.Light:
                    imgGrayscale = imgInput.Not().Convert<Gray, byte>().ThresholdBinary(new Gray(lowerThreshold), new Gray(upperThreshold));//35-255
                    break;
                case Imaging.BackgroundColorType.Unknown:
                    //cant distinguish background return original image
                    return image;
            }

            // create a mask image
            Image<Gray, byte> imgMask = new Image<Gray, byte>(imgInput.Width, imgInput.Height);
            // create an output image
            Image<Bgra, byte> imgout = new Image<Bgra, byte>(imgInput.Width, imgInput.Height, new Bgra(255, 0, 0, 0));

            //contours
            Mat hier = new Mat();
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();

            //find contours
            switch (removalType)
            {
                case Imaging.BackgroundRemovalType.External:
                    CvInvoke.FindContours(imgGrayscale, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxNone);
                    break;
                case Imaging.BackgroundRemovalType.Fill:
                    CvInvoke.FindContours(imgGrayscale, contours, hier, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);
                    break;
            }

            // draw the contours on the mask
            CvInvoke.DrawContours(imgMask, contours, -1, new MCvScalar(255), -1);

            //copy the data from the original image to the mask
            imgInput.Mat.CopyTo(imgout.Mat, imgMask);

            // erode
            Mat kernelOp1 = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(2, 2), new Point(-1, -1));
            CvInvoke.MorphologyEx(imgMask, imgMask, MorphOp.Erode, kernelOp1, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            //normalize
            CvInvoke.Normalize(imgMask.Mat.Clone(), imgMask, 0.0, 255.0, NormType.MinMax, DepthType.Cv8U);

            return imgout.Bitmap;
        }

        /// <summary>
        /// Crop an image using contours
        /// </summary>
        /// <param name="image">Input image</param>
        /// <param name="backgroundType">The type of background that the image uses</param>
        /// <param name="lowerThreshold">The lower color threshold (0-255) default:0</param>
        /// <param name="upperThreshold">The uppoer color threshold (0-255) default:255</param>
        /// <param name="padding">The amount of padding in pixels to put around the crop </param>
        /// <returns>A cropped bitmap</returns>
        public static Bitmap CropImage(this Bitmap image, Imaging.BackgroundColorType backgroundType, int lowerThreshold = 0, int upperThreshold = 255, int padding = 0)
        {
            Image<Bgra, byte> imgInput = new Image<Bgra, byte>(image);
            Image<Bgra, byte> imgCropped = null;

            switch (backgroundType)
            {
                case Imaging.BackgroundColorType.Dark:
                    {
                        // threshold 100 seems good
                        Image<Gray, byte> imgGrayscale = imgInput.Convert<Gray, byte>().ThresholdBinary(new Gray(lowerThreshold), new Gray(upperThreshold));

                        // find the contours in the image
                        Mat hier = new Mat();
                        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                        CvInvoke.FindContours(imgGrayscale, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxNone);

                        // put the contours bounding boxes in a list
                        List<Rectangle> recList = new List<Rectangle>();
                        for (var i = 0; i < contours.Size; i++)
                        {
                            Rectangle r = CvInvoke.BoundingRectangle(contours[i]);
                            recList.Add(r);
                        }
                        // make our main bounding box
                        Rectangle boundingBox = new Rectangle()
                        {
                            X = imgInput.Width,
                            Y = imgInput.Height,
                        };

                        //boundingBox.X = recList.Min(rc => rc.X);
                        //boundingBox.Y = recList.Min(rc => rc.Y);
                        //int right = recList.Max(rc => rc.X);
                        //int bottom = recList.Max(rc => rc.Y);
                        int right = 0;
                        int bottom = 0;
                        foreach (var rec in recList)
                        {
                            if (rec.X < boundingBox.X)
                            {
                                boundingBox.X = rec.X;
                            }

                            if (rec.Y < boundingBox.Y)
                            {
                                boundingBox.Y = rec.Y;
                            }

                            if (rec.Y + rec.Height > bottom)
                            {
                                bottom = rec.Y + rec.Height;
                            }

                            if (rec.X + rec.Width > right)
                            {
                                right = rec.X + rec.Width;
                            }
                        }

                        boundingBox.Width = right - boundingBox.X;
                        boundingBox.Height = bottom - boundingBox.Y;

                        if (boundingBox.X - (padding / 2) > 0 && boundingBox.Width + boundingBox.X + padding <= imgInput.Cols)
                        {
                            boundingBox.X = boundingBox.X - (padding / 2);
                            boundingBox.Width = boundingBox.Width + padding;
                        }//else do some calculations to get the max padding we can use

                        if (boundingBox.Y - (padding / 2) > 0 && boundingBox.Y + boundingBox.Height + padding <= imgInput.Rows)
                        {
                            boundingBox.Y = boundingBox.Y - (padding / 2);
                            boundingBox.Height = boundingBox.Height + padding;
                        }

                        //apply the bounding box to the section of the image we are cropping
                        imgInput.ROI = boundingBox;
                        imgCropped = imgInput.Copy();

                        break;
                    }

                case Imaging.BackgroundColorType.Light:
                    {
                        //threshold is good at 35
                        Image<Gray, byte> imgGrayscaleInverted = imgInput.Not().Convert<Gray, byte>().ThresholdBinary(new Gray(lowerThreshold), new Gray(upperThreshold));

                        //find the contours in the image
                        var hier = new Mat();
                        var contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                        CvInvoke.FindContours(imgGrayscaleInverted, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxNone);

                        //put the contours bounding boxes in a list
                        var recList = new List<Rectangle>();
                        for (var i = 0; i < contours.Size; i++)
                        {
                            Rectangle r = CvInvoke.BoundingRectangle(contours[i]);
                            recList.Add(r);
                        }
                        //make our main bounding box
                        var boundingBox = new Rectangle()
                        {
                            X = imgGrayscaleInverted.Width,
                            Y = imgGrayscaleInverted.Height,
                        };
                        //boundingBox.X = recList.Min(rc => rc.X);
                        //boundingBox.Y = recList.Min(rc => rc.Y);
                        //int right = recList.Max(rc => rc.X);
                        //int bottom = recList.Max(rc => rc.Y);
                        int right = 0;
                        int bottom = 0;
                        foreach (var rec in recList)
                        {
                            if (rec.X < boundingBox.X)
                                boundingBox.X = rec.X;

                            if (rec.Y < boundingBox.Y)
                                boundingBox.Y = rec.Y;

                            if (rec.Y + rec.Height > bottom)
                                bottom = rec.Y + rec.Height;

                            if (rec.X + rec.Width > right)
                                right = rec.X + rec.Width;
                        }
                        boundingBox.Width = right - boundingBox.X;
                        boundingBox.Height = bottom - boundingBox.Y;

                        if (boundingBox.X - (padding / 2) > 0 && boundingBox.Width + boundingBox.X + padding <= imgGrayscaleInverted.Cols)
                        {
                            boundingBox.X = boundingBox.X - (padding / 2);
                            boundingBox.Width = boundingBox.Width + padding;
                        }//else do some calculations to get the max padding we can use

                        if (boundingBox.Y - (padding / 2) > 0 && boundingBox.Y + boundingBox.Height + padding <= imgGrayscaleInverted.Rows)
                        {
                            boundingBox.Y = boundingBox.Y - (padding / 2);
                            boundingBox.Height = boundingBox.Height + padding;
                        }

                        //apply the bounding box to the section of the image we are cropping
                        imgInput.ROI = boundingBox;
                        //Image<Bgra, byte> imgCropped = imgInput.Copy();
                        imgCropped = imgInput.Copy();
                        break;
                    }
            }

            //return cropped image
            return new Bitmap(imgCropped.Bitmap);

        }

        /// <summary>
        /// Get the background color of an image by checking to see if 3 of four corners are the same color
        /// </summary>
        /// <param name="bmp">Source Bitmap</param>
        /// <param name="cornerOffset">Offset of pixels in which to look for the color (applies to both x and y)</param>
        /// <param name="colorDetailList">List of colors to select a match from, if null the default windows named colors are used</param>
        /// <returns>The background color if one is found, null if not</returns>
        public static Color? GetBackgroundColor(
            this Bitmap bmp,
            IImageHelper imageHelper,
            IColorHelper colorHelper,
            out Color? originalBgColor,
            int cornerOffset = 1,
            List<ColorDetail> colorDetailList = null)
        {
            // Getting The Background Color by checking Corners of Original Image
            var corners = new Point[]{
                new Point(cornerOffset, cornerOffset),
                new Point(cornerOffset, bmp.Height - cornerOffset),
                new Point(bmp.Width - cornerOffset, cornerOffset),
                new Point(bmp.Width - cornerOffset, bmp.Height - cornerOffset)
            }; // four corners (Top, Left), (Top, Right), (Bottom, Left), (Bottom, Right)

            for (int i = 0; i < 4; i++)
            {
                var cornerMatched = 0;
                var backColor = bmp.GetPixel(corners[i].X, corners[i].Y);
                originalBgColor = bmp.GetPixel(corners[i].X, corners[i].Y);
                if (backColor.Name == "0")
                {
                    backColor = Color.Transparent;
                }

                backColor = colorHelper.GetApproximateColorName(backColor);
                for (int j = 0; j < 4; j++)
                {
                    var cornerColor = bmp.GetPixel(corners[j].X, corners[j].Y);// Check RGB with some offset
                    if (cornerColor.Name == "0")
                        cornerColor = Color.Transparent;

                    cornerColor = colorHelper.GetApproximateColorName(cornerColor);

                    if (cornerColor.Equals(backColor))
                    {
                        cornerMatched++;
                    }
                }

                if (cornerMatched > 2)
                {
                    if (colorDetailList != null)
                    {
                        List<Color> CompareList = new List<Color>();
                        foreach (var item in colorDetailList)
                        {
                            Color mycolor = Color.FromArgb(item.MappedColor.A, item.MappedColor.R, item.MappedColor.G, item.MappedColor.B);
                            CompareList.Add(mycolor);
                        }

                        //find the closest color that we have in our list of colors
                        var distance = 0;
                        var matchedBackgroundColor = CompareList[colorHelper.ClosestRgbColor(CompareList, backColor, out distance)];

                        return matchedBackgroundColor;
                    }

                    return backColor;
                }
            }

            originalBgColor = null;
            return null;
        }

        public static Bitmap Erode(this Bitmap image, Imaging.BackgroundColorType backgroundType, Imaging.BackgroundRemovalType removalType = Imaging.BackgroundRemovalType.Fill, int lowerThreshold = 0, int upperThreshold = 255)
        {

            Image<Bgra, Byte> imgInput = new Image<Bgra, Byte>((Bitmap)image);
            Image<Gray, byte> imgGrayscale = null;


            //convert the image to grayscale
            switch (backgroundType)
            {
                case Imaging.BackgroundColorType.Dark:
                    imgGrayscale = imgInput.Convert<Gray, byte>().ThresholdBinary(new Gray(lowerThreshold), new Gray(upperThreshold));//50-255
                    break;
                case Imaging.BackgroundColorType.Light:
                    imgGrayscale = imgInput.Not().Convert<Gray, byte>().ThresholdBinary(new Gray(lowerThreshold), new Gray(upperThreshold));//35-255
                    break;
                case Imaging.BackgroundColorType.Unknown:
                    //cant distinguish background return original image
                    return image;
            }

            // create a mask image
            Image<Gray, byte> imgMask = new Image<Gray, byte>(imgInput.Width, imgInput.Height);
            // create an output image
            Image<Bgra, byte> imgout = new Image<Bgra, byte>(imgInput.Width, imgInput.Height, new Bgra(255, 0, 0, 0));

            //contours
            Mat hier = new Mat();
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();

            //find contours
            switch (removalType)
            {
                case Imaging.BackgroundRemovalType.External:
                    CvInvoke.FindContours(imgGrayscale, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxNone);
                    break;
                case Imaging.BackgroundRemovalType.Fill:
                    CvInvoke.FindContours(imgGrayscale, contours, hier, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);
                    break;
            }

            // draw the contours on the mask
            CvInvoke.DrawContours(imgMask, contours, -1, new MCvScalar(255), -1);

            imgMask.Bitmap.Save(System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}___000.PNG"), ImageFormat.Png);


            // Closing (dilate -> erode) para juntar regiones blancas.
            //Mat kernelCl = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(4, 4), new Point(-1, -1));
            //CvInvoke.MorphologyEx(imgMask, imgMask, MorphOp.Dilate, kernelCl, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            //Mat kernelOp = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3,3), new Point(-1, -1));
            //CvInvoke.MorphologyEx(imgMask, imgMask, MorphOp.Open, kernelOp, new Point(-1, -1), 3, BorderType.Default, new MCvScalar());

            Mat kernelOp1 = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(20, 20), new Point(-1, -1));
            CvInvoke.MorphologyEx(imgMask, imgMask, MorphOp.Erode, kernelOp1, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            //Mat kernelCl = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(2, 2), new Point(-1, -1));
            //CvInvoke.MorphologyEx(imgMask, imgMask, MorphOp.Dilate, kernelCl, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar());


            //copy the data from the original image to the mask
            imgInput.Mat.CopyTo(imgout.Mat, imgMask);


            //normalize
            CvInvoke.Normalize(imgMask.Mat.Clone(), imgMask, 0.0, 255.0, NormType.MinMax, DepthType.Cv8U);



            return imgout.Bitmap;
        }
        
        public static bool ContainsTouchingColors1(this Bitmap image, IImageHelper imageHelper, IColorHelper colorHelper, Color? backgroundColor, List<ColorDetail> imageColor, int reduceColorCount = 0, int tolerance = 0)
        {
            var imageOriginal = (Image)image;
            var image8Bit = imageOriginal.ImageTo8Bpp();
            
            var colors = new List<Color>();
            foreach (var colordetail in imageColor)
            {
                var color = Color.FromArgb(colordetail.MappedColor.A, colordetail.MappedColor.R, colordetail.MappedColor.G, colordetail.MappedColor.B);
                colors.Add(color);
            }

            ColorPalette pal = image8Bit.Palette;
            for (Int32 i = 0; i < pal.Entries.Length; i++)
            {
                Int32 foundIndex = GetClosestPaletteIndexMatch(pal.Entries[i], colors.ToArray());
                pal.Entries[i] = colors.ToArray()[foundIndex];
            }
            image8Bit.Palette = pal;

            var colorsstring = new Dictionary<string, Color>();
            image8Bit.Save(System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}___8bit.PNG"), ImageFormat.Png);

            var imageanalyse = ExpandBackgroundColor(imageHelper, backgroundColor.Value, (Bitmap)image8Bit, 1);
            imageanalyse.Save(System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}___TC.PNG"), ImageFormat.Png);

            var imgInput = new Image<Bgr, byte>(imageanalyse);
            var distance = 1;
            var maxpixelsTouchingPermitted = 10;
            var maxpixelsTouching = 0;
            for (var i = 0; i < imgInput.Rows - 1; i++)
            {
                for (var ii = 0; ii < imgInput.Cols - 1; ii++)
                {
                    //get this pixel color
                    var pixelColor = Color.FromArgb(0, imgInput.Data[i, ii, 2], imgInput.Data[i, ii, 1], imgInput.Data[i, ii, 0]);
                    pixelColor = pixelColor.Name == "0" ? Color.Transparent : colorHelper.GetApproximateColorName(pixelColor);

                    if (pixelColor == backgroundColor)
                    {
                        continue;
                    }

                    var isDifferentColor = CheckColorsAroundPixel(imageHelper, colorHelper, imgInput, pixelColor, backgroundColor, i, ii, distance, colorsstring);
                    if (isDifferentColor)
                    {
                        maxpixelsTouching++;
                        if (maxpixelsTouching == maxpixelsTouchingPermitted)
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        private static bool CheckColorsAroundPixel(IImageHelper imageHelper, IColorHelper colorHelper, Image<Bgr, byte> imageData,
            Color currentPixel, Color? bgColor, int currentRow, int currentCol, int distance, Dictionary<string,Color> colors)
        {
            Color pxlBelow = Color.Transparent;
            Color pxlRight = Color.Transparent;
            Color pxlAbove = Color.Transparent;
            Color pxlLeft = Color.Transparent;

            if (currentRow + distance < imageData.Rows)
            {
                pxlBelow = Color.FromArgb(0, imageData.Data[currentRow + distance, currentCol, 2],
                    imageData.Data[currentRow + distance, currentCol, 1],
                    imageData.Data[currentRow + distance, currentCol, 0]);

                var origName = pxlBelow.Name;

                if (colors.ContainsKey(origName))
                {
                    pxlBelow = colors[origName];
                }
                else
                {
                    pxlBelow = pxlBelow.Name == "0" ? Color.Transparent : colorHelper.GetApproximateColorName(pxlBelow);
                }

                if (!colors.ContainsKey(origName))
                {
                    colors.Add(origName, pxlBelow);
                }

                if (!(pxlBelow == Color.Transparent || (currentPixel == Color.Transparent) || currentPixel == bgColor || pxlBelow == bgColor) && !currentPixel.Equals(pxlBelow))
                {
                    return true;
                }
            }

            if (currentCol + distance >= imageData.Cols)
            {
                return false;
            }
            
            pxlRight = Color.FromArgb(0, imageData.Data[currentRow, currentCol + distance, 2], imageData.Data[currentRow, currentCol + distance, 1], imageData.Data[currentRow, currentCol + distance, 0]);
            var pxlRightOrigName = pxlRight.Name;
            if (colors.ContainsKey(pxlRightOrigName))
            {
                pxlRight = colors[pxlRightOrigName];
            }
            else
            {
                pxlRight = pxlRight.Name == "0" ? Color.Transparent : colorHelper.GetApproximateColorName(pxlRight);
            }

            if (!(pxlRight == Color.Transparent || (currentPixel == Color.Transparent) || currentPixel == bgColor || pxlRight == bgColor) && !currentPixel.Equals(pxlRight))
            {
               return true;
            }

            return false;
        }
        
        private static Bitmap ExpandBackgroundColor(IImageHelper imageHelper, Color bgColorRGB, Bitmap bmp, int distance)
        {
            var pixelsReplaced = new List<string>();
            Bitmap newBitmap = new Bitmap(bmp.Width, bmp.Height);
            var bgColorHex = ColorNameToHex(bgColorRGB.Name);
            
            for (var i = 0; i < bmp.Width; i++)
            {
                for (var j = 0; j < bmp.Height; j++)
                {
                    if (pixelsReplaced.Contains($"{i}-{j}"))
                    {
                        continue;
                    }


                    if (i == 488 && j == 979)
                    {

                    }

                    var pixel = bmp.GetPixel(i, j);
                    if (pixel.Name == bgColorHex)
                    {
                        newBitmap.SetPixel(i, j, bgColorRGB);
                        continue;
                    }

                    // left pixel
                    if (i - 1 > 0 && !pixelsReplaced.Contains($"{i - 1}-{j}"))
                    {
                        var leftpixel = bmp.GetPixel(i - 1, j);
                        if (leftpixel.Name == bgColorHex)
                        {
                            newBitmap.SetPixel(i, j, bgColorRGB);
                            pixelsReplaced.Add($"{i}-{j}");
                            continue;
                        }
                    }

                    // right pixel
                    if (i + 1 < bmp.Width && !pixelsReplaced.Contains($"{i + 1}-{j}"))
                    {
                        var rightpixel = bmp.GetPixel(i + 1, j);
                        if (rightpixel.Name == bgColorHex)
                        {
                            newBitmap.SetPixel(i, j, bgColorRGB);
                            pixelsReplaced.Add($"{i}-{j}");
                            continue;
                        }
                    }


                    // top pixel
                    if (j - 1 > 0 && !pixelsReplaced.Contains($"{i}-{j - 1}"))
                    {
                        var toppixel = bmp.GetPixel(i, j - 1);
                        if (toppixel.Name == bgColorHex)
                        {
                            newBitmap.SetPixel(i, j, bgColorRGB);
                            pixelsReplaced.Add($"{i}-{j}");
                            continue;
                        }
                    }

                    // bottom pixel
                    if (j + 1 < bmp.Height && !pixelsReplaced.Contains($"{i}-{j + 1}"))
                    {
                        var bottompixel = bmp.GetPixel(i, j + 1);
                        if (bottompixel.Name == bgColorHex)
                        {
                            newBitmap.SetPixel(i, j, bgColorRGB);
                            pixelsReplaced.Add($"{i}-{j}");
                            continue;
                        }
                    }

                    newBitmap.SetPixel(i, j, pixel);
                }
            }

            return newBitmap;
        }

        private static Int32 GetClosestPaletteIndexMatch(Color col, Color[] colorPalette)
        {
            Int32 colorMatch = 0;
            Int32 leastDistance = Int32.MaxValue;
            Int32 red = col.R;
            Int32 green = col.G;
            Int32 blue = col.B;
            for (Int32 i = 0; i < colorPalette.Length; i++)
            {
                Color paletteColor = colorPalette[i];
                Int32 redDistance = paletteColor.R - red;
                Int32 greenDistance = paletteColor.G - green;
                Int32 blueDistance = paletteColor.B - blue;
                Int32 distance = (redDistance * redDistance) + (greenDistance * greenDistance) + (blueDistance * blueDistance);
                if (distance >= leastDistance)
                    continue;
                colorMatch = i;
                leastDistance = distance;
                if (distance == 0)
                    return i;
            }
            return colorMatch;
        }

        public static Bitmap Skelatanize(Bitmap image)
        {
            Image<Rgba, byte> imgOld = new Image<Rgba, byte>(image);
            Image<Rgba, byte> img2 = (new Image<Rgba, byte>(imgOld.Width, imgOld.Height, new Rgba(255,0,0,0))).Sub(imgOld);
            Image<Rgba, byte> eroded = new Image<Rgba, byte>(img2.Size);
            Image<Rgba, byte> temp = new Image<Rgba, byte>(img2.Size);
            Image<Rgba, byte> skel = new Image<Rgba, byte>(img2.Size);
            skel.SetValue(0);
            CvInvoke.Threshold(img2, img2, 127, 256, 0);
            var element = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
            bool done = false;

            while (!done)
            {
                CvInvoke.Erode(img2, eroded, element, new Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
                CvInvoke.Dilate(eroded, temp, element, new Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
                CvInvoke.Subtract(img2, temp, temp);
                CvInvoke.BitwiseOr(skel, temp, skel);
                eroded.CopyTo(img2);
                if (CvInvoke.CountNonZero(img2) == 0) done = true;
            }
            return skel.Bitmap;
        }

        private static Color GetSystemDrawingColorFromHexString(string hexString)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(hexString, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
                throw new ArgumentException();
            int red = int.Parse(hexString.Substring(1, 2), NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(3, 2), NumberStyles.HexNumber);
            int blue = int.Parse(hexString.Substring(5, 2), NumberStyles.HexNumber);
            return Color.FromArgb(red, green, blue);
        }

        public static string ColorNameToHex(string colorName)
        {
            int ColorValue = Color.FromName(colorName).ToArgb();
            string ColorHex = string.Format("{0:x6}", ColorValue);
            return ColorHex;
        }
    }
}
