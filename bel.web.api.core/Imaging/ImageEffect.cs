namespace bel.web.api.core.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using bel.web.api.core.Extensions;
    using bel.web.api.core.Imaging.Effect;
    using bel.web.api.core.objects.Enums;
    using bel.web.api.core.objects.ImageEffects;
    using bel.web.api.core.objects.Imaging;
    using bel.web.api.core.objects.Interfaces;
    using bel.web.api.core.objects.Product;

    using Emgu.CV.Structure;

    using ImageMagick;

    public class ImageEffect : IImageEffect
    {
        private IColorHelper _colorHelper;
        private IProductHelper _productHelper;
        private IImageHelper _imageHelper;
        private IImageConverter _imageConverter;
        private IImageResize _imageResize;

        public ImageEffect()
        {
        }

        public ImageEffect(IColorHelper colorHelper, IProductHelper productHelper, IImageHelper imageHelper, IImageConverter imageConverter, IImageResize imageResize)
        {
            this._colorHelper = colorHelper;
            this._productHelper = productHelper;
            this._imageHelper = imageHelper;
            this._imageConverter = imageConverter;
            this._imageResize = imageResize;
        }

        /// <summary>
        /// Apply embroidery effect over the image
        /// </summary>
        /// <param name="input">
        /// the input image.
        /// </param>
        /// <param name="bgColor">
        /// The bg Color.
        /// </param>
        /// <param name="bgRemoved">
        /// The bg Removed.
        /// </param>
        /// <param name="numberOfColors">
        /// The number Of Colors.
        /// </param>
        /// <returns>
        /// The image with the effect applied.
        /// </returns>
	    public IMagickImage DoEmbroidery(IMagickImage input, Color? bgColor = null, bool bgRemoved = false, int numberOfColors = 16)
        {
            var helper = new EmbroideryHelper(bgColor, numberOfColors, bgRemoved);
            var path = Path.Combine(Path.GetTempPath(), "Embroidery.PNG");
            input.Write(path, MagickFormat.Png);
            return helper.Execute(input);
        }

        /// <summary>
        /// The do debossing.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="isText">
        /// The is text.
        /// </param>
        /// <returns>
        /// The <see cref="IMagickImage"/>.
        /// </returns>
        public IMagickImage DoDebossing(IMagickImage input, bool isText = false)
        {
            // Change background transparent for white bg
            var image = this._imageHelper.FillBackground(input.ToBitmap(), Color.White);

            // Apply debossing
            var helper = new DebossingHelper();
            var bytes = helper.Execute(new MagickImage((Bitmap)image), isText);
            var result = new MagickImage(new MemoryStream(bytes));

            // Remove bg
            var final = result.ToBitmap().RemoveBackground(Imaging.BackgroundColorType.Light);
            return new MagickImage(final);
        }

        public void RemoveColors(Bitmap processedBitmap, List<Color> colorsToRemove)
        {
            unsafe
            {
                var bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
                var bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                var heightInPixels = bitmapData.Height;
                var widthInBytes = bitmapData.Width * bytesPerPixel;
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (var y = 0; y < heightInPixels; y++)
                {
                    var currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        // current px
                        var origColor = Color.FromArgb(currentLine[x + 3], currentLine[x + 2], currentLine[x + 1], currentLine[x]);

                        // ff000000
                        var transpColor = Color.Transparent;


                        if (!this._colorHelper.CheckColor(origColor, colorsToRemove, 20))
                        {
                            continue;
                        }

                        try
                        {
                            currentLine[x] = (byte)transpColor.B;
                            currentLine[x + 1] = (byte)transpColor.G;
                            currentLine[x + 2] = (byte)transpColor.R;
                            currentLine[x + 3] = (byte)transpColor.A;

                        }
                        catch (Exception ex)
                        {
                            currentLine[x] = (byte)0;
                            currentLine[x + 1] = (byte)0;
                            currentLine[x + 2] = (byte)255;

                        }
                    }
                };
                processedBitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// The apply image effect.
        /// </summary>
        /// <param name="originalImage">
        /// The original image.
        /// </param>
        /// <param name="effect">
        /// The effect.
        /// </param>
        /// <param name="productDetails">
        /// The product details.
        /// </param>
        /// <param name="removeBg">
        /// The remove bg.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] ApplyImageEffect(
            IMagickImage originalImage,
            objects.ImageEffects.Effect effect,
            ProductDetails productDetails,
            bool removeBg = true)

        {
            switch (effect)
            {
                case objects.ImageEffects.Effect.Embroidery:
                    {
                        // Remove Bg
                        var img = this.DoBgRemovalProcess(new ImageActions { Cropped = false, WhiteBgRemoved = true, BlackBgRemoved = true }, true, Color.White, originalImage.ToBitmap());
                        var resultImage = this.DoEmbroidery(new MagickImage(img));
                        return _imageConverter.ImageToByte(resultImage.ToBitmap());
                    }
                case objects.ImageEffects.Effect.Debossing:
                    {
                        try
                        {
                            // Download product image
                            var productImage = this._imageConverter.ImageFromUrl(productDetails.ProductImageUrl, out _, out var productImageBytes);

                            // Check if texture is enough to cover image.
                            if (originalImage.Width > productDetails.ImprintAreaSize.Width
                                || originalImage.Height > productDetails.ImprintAreaSize.Height)
                            {
                                var factorWidth = originalImage.Width / productDetails.ImprintAreaSize.Width;
                                var factorHeight = originalImage.Height / productDetails.ImprintAreaSize.Height;
                                var factor = Math.Max(factorWidth, factorHeight);
                                var productImageResized = this._imageResize.ResizeImageProportional(
                                    productImage.ToBitmap(),
                                    Convert.ToInt32(productImage.Width * factor),
                                    Convert.ToInt32(productImage.Height * factor));
                                productImageBytes = this._imageConverter.ImageToByte(productImageResized);
                                productDetails.ImprintAreaCoordinates = new PointF(productDetails.ImprintAreaCoordinates.X * factor, productDetails.ImprintAreaCoordinates.Y * factor);
                                productDetails.ImprintAreaSize = new SizeF(productDetails.ImprintAreaSize.Width * factor, productDetails.ImprintAreaSize.Height * factor);
                            }

                            // Extract texture from image using Imprint Area
                            var textureImage = this._imageResize.GetRectangleAreaFromImage(
                                productImageBytes,
                                productDetails.ImprintAreaSize,
                                productDetails.ImprintAreaCoordinates);

                            var texture = new MagickImage(textureImage);
                            originalImage.Tile(texture, CompositeOperator.Atop);
                            var resultImageDebossed = this.DoDebossing(originalImage);
                            return _imageConverter.ImageToByte(resultImageDebossed.ToBitmap());
                        }
                        catch
                        {
                            return _imageConverter.ImageToByte(originalImage.ToBitmap());
                        }
                    }
                case objects.ImageEffects.Effect.laser_engraved:
                    {
                        IMagickImage finalImg;
                        if (removeBg)
                        {
                            finalImg = new MagickImage(this.DoBgRemovalProcess(
                                new ImageActions { WhiteBgRemoved = true }, true, Color.White, originalImage.ToBitmap()));
                        }
                        else
                        {
                            finalImg = originalImage;
                        }

                        var effectParameters = this._productHelper.GetTexture(productDetails);
                        var textureName = effectParameters.FirstOrDefault(e => e.Action == EffectAction.Texture)?.Value.ToString();
                        if (!string.IsNullOrEmpty(textureName))
                        {
                            try
                            {
                                var textureImage = this._imageConverter.GetImageMagickFromFile(textureName);
                                var lasered = new MagickImage(finalImg);

                                // check if need to resize
                                if (lasered.Width > textureImage.Width || lasered.Height > textureImage.Height)
                                {
                                    var max = lasered.Width > lasered.Height ? lasered.Width : lasered.Height;
                                    textureImage.Resize(max, max);
                                }

                                lasered.Composite(textureImage, CompositeOperator.Atop);
                                return _imageConverter.ImageToByte(lasered.ToBitmap());
                            }
                            catch (Exception ex)
                            {
                                return _imageConverter.ImageToByte(originalImage.ToBitmap());
                            }
                        }

                        return _imageConverter.ImageToByte(originalImage.ToBitmap());
                    }
                case objects.ImageEffects.Effect.OneColor:
                    {
                        ApplyOneColor(new ImageActions { WhiteBgRemoved = true }, true, Color.White, ref originalImage);
                        return _imageConverter.ImageToByte(originalImage.ToBitmap());
                    }
                default:
                    return _imageConverter.ImageToByte(originalImage.ToBitmap());
            }
        }

        public bool ApplyImageEffect(
            IMagickImage originalImage,
            ImageActions imageActions,
            ProductDetails productDetails,
            bool newAsset,
            int actualColorCountReduced,
            ref Color? backgroundColor,
            bool bgRemoved,
            ref IMagickImage processedImage)
        {
            switch (imageActions.Effect)
            {
                case objects.ImageEffects.Effect.Embroidery:
                    {
                        // resize first to improve the performance
                        var resultImage = this.DoEmbroidery(processedImage, backgroundColor, bgRemoved);

                        if (bgRemoved || (backgroundColor.HasValue && backgroundColor.Value == Color.Transparent))
                        {
                            // Remove bg one more time
                            backgroundColor =
                                resultImage.ToBitmap().GetBackgroundColor(this._imageHelper, this._colorHelper, out _, 5)
                                ?? Color.Black;

                            var finalImg = this.DoBgRemovalProcess(
                                new ImageActions
                                {
                                    Cropped = false,
                                    WhiteBgRemoved = true,
                                    BlackBgRemoved = true
                                },
                                true,
                                backgroundColor,
                                resultImage.ToBitmap());
                            processedImage = new MagickImage(finalImg);
                        }
                        else
                        {
                            processedImage = resultImage;
                        }
                        break;
                    }

                case objects.ImageEffects.Effect.Debossing:
                    {
                        // Download product image
                        var productImage = this._imageConverter.ImageFromUrl(productDetails.ProductImageUrl, out _, out var productImageBytes);

                        // Update imprintAreaSizes base on Admin Panel Ratio
                        if (productDetails.OriginalImprintValues)
                        {
                            var adminPanelWidth = 530;
                            var adminPanelHeight = 530;
                            var factorWidthAP = (float)productImage.Width / adminPanelWidth;
                            var factorHeightAP = (float)productImage.Height / adminPanelHeight;
                            productDetails.ImprintAreaCoordinates = new PointF(productDetails.ImprintAreaCoordinates.X * factorWidthAP, productDetails.ImprintAreaCoordinates.Y * factorHeightAP);
                            productDetails.ImprintAreaSize = new SizeF(productDetails.ImprintAreaSize.Width * factorWidthAP, productDetails.ImprintAreaSize.Height * factorHeightAP);
                        }

                        // Check if texture is enough to cover image.
                        if (originalImage.Width > productDetails.ImprintAreaSize.Width
                            || originalImage.Height > productDetails.ImprintAreaSize.Height)
                        {
                            var factorWidth = originalImage.Width / productDetails.ImprintAreaSize.Width;
                            var factorHeight = originalImage.Height / productDetails.ImprintAreaSize.Height;
                            var factor = Math.Max(factorWidth, factorHeight);
                            var productImageResized = this._imageResize.ResizeImageProportional(
                                productImage.ToBitmap(),
                                Convert.ToInt32(productImage.Width * factor),
                                Convert.ToInt32(productImage.Height * factor));
                            productImageBytes = this._imageConverter.ImageToByte(productImageResized);
                            productDetails.ImprintAreaCoordinates = new PointF(productDetails.ImprintAreaCoordinates.X * factor, productDetails.ImprintAreaCoordinates.Y * factor);
                            productDetails.ImprintAreaSize = new SizeF(productDetails.ImprintAreaSize.Width * factor, productDetails.ImprintAreaSize.Height * factor);
                        }

                        // Extract texture from image using Imprint Area
                        var textureImage = this._imageResize.GetRectangleAreaFromImage(
                            productImageBytes,
                            productDetails.ImprintAreaSize,
                            productDetails.ImprintAreaCoordinates);

                        var texture = new MagickImage(textureImage);
                        originalImage.Tile(texture, CompositeOperator.Atop);
                        processedImage = originalImage;
                        var resultImageDebossed = this.DoDebossing(processedImage);
                        processedImage = resultImageDebossed;
                        break;
                    }

                case objects.ImageEffects.Effect.laser_engraved:
                    {
                        if (backgroundColor != null)
                        {
                            // Remove bg one more time
                            var finalImg = this.DoBgRemovalProcess(
                                imageActions,
                                newAsset,
                                backgroundColor,
                                processedImage.ToBitmap());
                            processedImage = new MagickImage(finalImg);
                            bgRemoved = true;
                        }

                        if (imageActions.EffectParameters == null || imageActions.EffectParameters.Count == 0)
                        {
                            imageActions.EffectParameters = this._productHelper.GetTexture(productDetails);
                        }

                        var textureName = imageActions.EffectParameters.FirstOrDefault(e => e.Action == EffectAction.Texture)?.Value.ToString();
                        if (!string.IsNullOrEmpty(textureName))
                        {
                            try
                            {
                                var textureImage = this._imageConverter.GetImageMagickFromFile(textureName);
                                processedImage.Composite(textureImage, CompositeOperator.Atop);
                            }
                            catch (Exception exception)
                            {
                            }
                        }

                        break;
                    }

                case objects.ImageEffects.Effect.OneColor:
                    {
                        bgRemoved = ApplyOneColor(imageActions, newAsset, backgroundColor, ref processedImage);
                        break;
                    }
            }

            return bgRemoved;
        }

        public bool ApplyOneColor(ImageActions imageActions, bool newAsset, Color? backgroundColor,
            ref IMagickImage processedImage)
        {
            var bgRemoved = false;
            var colorName = Color.Transparent;
            var value = imageActions.EffectParameters
                .FirstOrDefault(e => e.Action == EffectAction.Color)?.Value;
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                System.Drawing.Color col =
                    System.Drawing.ColorTranslator.FromHtml($"{value.ToString()}");
                colorName = col;
            }

            if (backgroundColor != null)
            {
                // Remove bg one more time
                var finalImg = this.DoBgRemovalProcess(
                    imageActions,
                    newAsset,
                    backgroundColor,
                    processedImage.ToBitmap());
                processedImage = new MagickImage(finalImg);
                bgRemoved = true;
            }

            var result = this._imageHelper.ConvertToOneColor(processedImage, colorName);
            processedImage = new MagickImage(new MemoryStream(result));
            return bgRemoved;
        }

        public Bitmap DoBgRemovalProcess(
            ImageActions actions,
            bool newAsset,
            Color? backgroundColor,
            Bitmap img,
            bool removeOnTransparent = false)
        {
            newAsset = false;
            switch (backgroundColor.Value.Name.ToLower())
            {
                case "000000":
                case "ff000000":
                case "black":
                    img = this.ProcessDarkBmp(actions, img, newAsset);
                    actions.WhiteBgRemoved = false;
                    break;
                case "white":
                case "ffffffff":
                case "ffffff":
                    img = this.ProcessLightBmp(actions, img, newAsset);
                    actions.BlackBgRemoved = false;
                    break;
                case "Transparent":
                    {
                        if (removeOnTransparent)
                        {
                            img = this.ProcessLightBmp(actions, img, newAsset);
                            actions.BlackBgRemoved = false;
                        }
                        else
                        {
                            actions.WhiteBgRemoved = false;
                            actions.BlackBgRemoved = false;
                        }

                        break;
                    }

                default:
                    {
                        actions.WhiteBgRemoved = false;
                        actions.BlackBgRemoved = false;
                        break;
                    }
            }

            return img;
        }

        public Bitmap ProcessLightBmp(ImageActions internalImageActions, Bitmap img1, bool newAsset, int lowerThreshold = 35, int upperThreshold = 255)
        {
            // light background color
            if (internalImageActions.Cropped)
            {
                try
                {
                    img1 = img1.CropImage(Imaging.BackgroundColorType.Light, lowerThreshold, upperThreshold, 5);
                    internalImageActions.Cropped = true;
                }
                catch
                {
                    internalImageActions.Cropped = false;
                }
            }

            if (!internalImageActions.WhiteBgRemoved && !newAsset)
            {
                return img1;
            }

                try
                {
                    img1 = img1.RemoveBackground(Imaging.BackgroundColorType.Light,
                        Imaging.BackgroundRemovalType.Fill, lowerThreshold, upperThreshold);
                    internalImageActions.WhiteBgRemoved = true;
                }
                catch
                {
                    internalImageActions.WhiteBgRemoved = false;
                }

            return img1;
        }

        public Bitmap ProcessDarkBmp(ImageActions internalImageActions, Bitmap bitmap, bool newAsset)
        {
            // dark background color
            if (internalImageActions.Cropped)
            {
                try
                {
                    bitmap = bitmap.CropImage(Imaging.BackgroundColorType.Dark, 100, 255, 5);
                    internalImageActions.Cropped = true;
                }
                catch
                {
                    internalImageActions.Cropped = false;
                }
            }

            if (!internalImageActions.BlackBgRemoved && !newAsset)
            {
                return bitmap;
            }

            try
            {
                bitmap = bitmap.RemoveBackground(Imaging.BackgroundColorType.Dark,
                    Imaging.BackgroundRemovalType.Fill, 50);
                internalImageActions.BlackBgRemoved = true;
            }
            catch
            {
                internalImageActions.BlackBgRemoved = false;
            }

            return bitmap;
        }

        public IMagickImage ProcessBackground(
            ImageActions actions,
            bool newAsset,
            Color? backgroundColor,
            IMagickImage image)
        {
            var format = image.ColorSpace == ColorSpace.CMYK ? ImageFormat.Jpeg : ImageFormat.Png;
            var img = image.ToBitmap(format, BitmapDensity.Use);

            if (backgroundColor != null)
            {
                img = this.DoBgRemovalProcess(actions, newAsset, backgroundColor, (Bitmap)img);
            }
            else
            {
                actions.WhiteBgRemoved = false;
                actions.BlackBgRemoved = false;
            }

            return new MagickImage((Bitmap)img);
        }
    }
}

