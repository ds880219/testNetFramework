// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmbroideryHelper.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Applies an embroidery effect to each color in an image. The image must have limited number of
//   colors or only the top most frequent colors will be used. Each color will get the same
//   pattern, but at different rotation angles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Imaging.Effect
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using bel.web.api.core.objects.Enums;

    using ImageMagick;

    /// <summary>
    /// Applies an embroidery effect to each color in an image. The image must have limited number of
    /// colors or only the top most frequent colors will be used. Each color will get the same
    /// pattern, but at different rotation angles.
    /// </summary>
    public sealed class EmbroideryHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbroideryHelper"/> class.
        /// </summary>
        public EmbroideryHelper()
        {
            this.Reset();
        }

        public EmbroideryHelper(Color? bgColor, int numberOfColors, bool bgRemoved)
        {
            this.Reset();
            //if (bgRemoved)
            //{
            //    BackgroundColor = Color.Transparent;
            //}
            //else if (bgColor.HasValue)
            //{
                BackgroundColor = bgColor.Value;
            //}

            //this.NumberOfColors = numberOfColors;
        }

        /// <summary>
        /// Gets or sets the initial pattern angle for background color.
        /// Default is 90.
        /// </summary>
        public int Angle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bevel azimuth angle.
        /// Default is 130.
        /// </summary>
        public double Azimuth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actual background color in image.
        /// Default is most the frequent color.
        /// </summary>
        public MagickColor BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pattern bevel amount.
        /// Default is 4.
        /// </summary>
        public int Bevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fuzz value for recoloring near black and near white
        /// Default is 20.
        /// </summary>
        public Percentage ColorFuzz
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bevel sigmoidal-contrast amount.
        /// Default is 0 (no added contrast).
        /// </summary>
        public double Contrast
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bevel elevation angle.
        /// Default is 30.
        /// </summary>
        public double Elevation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shadow extent.
        /// Default is 2.
        /// </summary>
        public double Extent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value to limit colors near black and near white to gray(graylimit%) and gray(100%-graylimit%).
        /// Default is 20.
        /// </summary>
        public int GrayLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shadow intensity (higher is darker).
        /// Default is 25.
        /// </summary>
        public Percentage Intensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mixing between before and after spread result.
        /// Default is 100.
        /// </summary>
        public int Mix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of desired or actual colors in image.
        /// Default is 8.
        /// </summary>
        public int NumberOfColors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the wave pattern.
        /// Default is Linear.
        /// </summary>
        public EmbroideryPattern Pattern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range of pattern angles over all the colors.
        /// Default is 90.
        /// </summary>
        public int Range
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pattern spread (diffusion).
        /// Default is 1
        /// </summary>
        public double Spread
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the weave thickness.
        /// Default is 2.
        /// </summary>
        public int Thickness
        {
            get;
            set;
        }

        /// <summary>
        /// Applies an embroidery effect to each color in an image. The image must have limited number
        /// of colors or only the top most frequent colors will be used. Each color will get the same
        /// pattern, but at different rotation angles.
        /// </summary>
        /// <param name="input">The image to execute the script on.</param>
        /// <returns>The resulting image.</returns>
        public IMagickImage Execute(IMagickImage input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            this.CheckSettings();
            using (var image = input.Clone())
            {
                var colors = image.Histogram().OrderByDescending(kv => kv.Value).Select(kv => kv.Key).Take(this.NumberOfColors).ToArray();

                RemapColors(image, colors);
                image.Write(Path.Combine(Path.GetTempPath(), "EmbroideryRemapped.PNG"));

                using (var texture = this.CreateTexture())
                {
                    var pattern = this.CreatePattern(image.Width * 2, image.Height * 2, texture);
                    pattern.Write(Path.Combine(Path.GetTempPath(), "EmbroideryPattern.PNG"));

                    using (IMagickImage nearBlackWhite = this.ToNearBlackWhite(image))
                    {
                        nearBlackWhite.Write(Path.Combine(Path.GetTempPath(), "EmbroideryBW.PNG"));
                        using (var images = new MagickImageCollection())
                        {
                            double angle = (this.Pattern == EmbroideryPattern.Linear ? -45 : -90) + this.Angle;

                            Parallel.ForEach(colors, color =>
                            {
                                bool useBevel = this.Bevel != 0 && color != colors.First();

                                using (var croppedPattern = CreateCroppedPattern(image, pattern, angle))
                                {
                                    using (var alpha = ExtractAlpha(image, color))
                                    {
                                        var colorImage = this.CreateColor(alpha, croppedPattern, nearBlackWhite,
                                            useBevel);
                                        colorImage.Write(Path.Combine(Path.GetTempPath(), "EmbroideryColorImage.PNG"));
                                        images.Add(colorImage);
                                    }
                                }

                                angle += this.Range / (double) colors.Length;
                            });

                            var result = images.Flatten();
                            result.Crop(input.Width, input.Height, Gravity.Center);
                            return result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the script to the default settings.
        /// </summary>
        public void Reset()
        {
            this.Angle = 0;
            this.Azimuth = 130;
            this.BackgroundColor = null;
            this.Bevel = 4;
            this.ColorFuzz = (Percentage)5;
            this.Contrast = 0;
            this.Elevation = 30;
            this.Extent = 2;
            this.GrayLimit = 20;
            this.Intensity = (Percentage)25;
            this.Mix = 100;
            this.NumberOfColors = 8;
            this.Pattern = EmbroideryPattern.Linear;
            this.Range = 90;
            this.Spread = 1.0;
            this.Thickness = 2;
        }

        private static IMagickImage CreateCroppedPattern(IMagickImage image, IMagickImage pattern, double angle)
        {
            var croppedPattern = pattern.Clone();
            croppedPattern.Rotate(angle);
            croppedPattern.RePage();
            croppedPattern.Crop(image.Width, image.Height, Gravity.Center);
            croppedPattern.RePage();
            return croppedPattern;
        }

        private static IMagickImage CreateRolled(IMagickImage image, int thickness)
        {
            IMagickImage rolled = image.Clone();
            rolled.Roll(thickness, 0);
            return rolled;
        }

        private static IMagickImage ExtractAlpha(IMagickImage image, MagickColor color)
        {
            var alpha = image.Clone();
            alpha.InverseTransparent(color);
            alpha.Alpha(AlphaOption.Extract);
            return alpha;
        }

        private static void RemapColors(IMagickImage image, IEnumerable<MagickColor> colors)
        {
            using (var images = new MagickImageCollection())
            {
                foreach (var color in colors)
                    images.Add(new MagickImage(color, 1, 1));

                using (IMagickImage colorMap = images.AppendHorizontally())
                {
                    image.Map(colorMap, new QuantizeSettings()
                    {
                        DitherMethod = DitherMethod.No
                    });
                }
            }
        }

        private void AddBevel(IMagickImage image)
        {
            using (var alphaTexture = image.Clone())
            {
                alphaTexture.Alpha(AlphaOption.Extract);
                alphaTexture.Blur(0, this.Bevel);
                alphaTexture.Shade(this.Azimuth, this.Elevation);
                alphaTexture.Composite(image, CompositeOperator.CopyAlpha);
                alphaTexture.Alpha(AlphaOption.On);
                alphaTexture.Alpha(AlphaOption.Background);
                alphaTexture.Alpha(AlphaOption.Deactivate);
                alphaTexture.AutoLevel(Channels.Composite);
                alphaTexture.Evaluate(Channels.Composite, EvaluateFunction.Polynomial, new double[] { 3.5, -5.05, 2.05, 0.25 });
                alphaTexture.SigmoidalContrast(this.Contrast);
                alphaTexture.Alpha(AlphaOption.On);

                image.Composite(alphaTexture, CompositeOperator.HardLight);
            }
        }

        private void CheckSettings()
        {
            if (this.Angle < -360 || this.Angle > 360)
                throw new InvalidOperationException("Invalid angle specified, value must be between -360 and 360.");

            if (this.Azimuth < -360.0 || this.Azimuth > 360.0)
                throw new InvalidOperationException("Invalid azimuth specified, value must be between -360 and 360.");

            if (this.ColorFuzz < (Percentage)0 || this.ColorFuzz > (Percentage)100)
                throw new InvalidOperationException("Invalid color fuzz specified, value must be between 0 and 100.");

            if (this.Contrast < 0.0)
                throw new InvalidOperationException("Invalid contrast specified, value must be zero or higher.");

            if (this.Elevation < 0.0 || this.Elevation > 90.0)
                throw new InvalidOperationException("Invalid elevation specified, value must be between 0 and 90.");

            if (this.Extent < 0.0)
                throw new InvalidOperationException("Invalid extent specified, value must be zero or higher.");

            if (this.GrayLimit < 0 || this.GrayLimit > 100)
                throw new InvalidOperationException("Invalid gray limit specified, value must be between 0 and 100.");

            if (this.Intensity < (Percentage)0 || this.Intensity > (Percentage)100)
                throw new InvalidOperationException("Invalid intensity specified, value must be between 0 and 100.");

            if (this.Mix < 0 || this.Mix > 100)
                throw new InvalidOperationException("Invalid mix specified, value must be between 0 and 100.");

            if (this.NumberOfColors <= 0)
                throw new InvalidOperationException("Invalid number of colors specified, value must be higher than zero.");

            if (this.Pattern != EmbroideryPattern.Crosshatch && this.Pattern != EmbroideryPattern.Linear)
                throw new InvalidOperationException("Invalid pattern specified.");

            if (this.Range < 0 || this.Range > 360)
                throw new InvalidOperationException("Invalid range specified, value must be between 0 and 360.");

            if (this.Spread < 0.0)
                throw new InvalidOperationException("Invalid spread specified, value must be zero or higher.");

            if (this.Thickness <= 0)
                throw new InvalidOperationException("Invalid thickness specified, value must be higher than zero.");
        }

        private IMagickImage CreateColor(IMagickImage alpha, IMagickImage croppedPattern, IMagickImage nearBlackWhite, bool useBevel)
        {
            using (var alphaCopy = nearBlackWhite.Clone())
            {
                alphaCopy.Composite(croppedPattern, CompositeOperator.SoftLight);
                alphaCopy.Alpha(AlphaOption.Off);
                alphaCopy.Composite(alpha, CompositeOperator.CopyAlpha);

                if (useBevel)
                    this.AddBevel(alphaCopy);

                var result = alphaCopy.Clone();
                result.BackgroundColor = MagickColors.Black;
                result.Shadow(0, 0, this.Extent, this.Intensity);
                result.RePage();
                result.Level((Percentage)0, (Percentage)50, Channels.Alpha);
                result.Composite(alphaCopy, Gravity.Center, CompositeOperator.Over);

                return result;
            }
        }

        private IMagickImage CreateCrosshatchTexture()
        {
            var gradient = new MagickImage("gradient:", this.Thickness + 3, this.Thickness + 3);
            gradient.Rotate(270);

            IMagickImage flopped = gradient.Clone();
            flopped.Flop();

            using (MagickImageCollection images = new MagickImageCollection())
            {
                images.Add(gradient);
                images.Add(flopped);

                return images.AppendVertically();
            }
        }

        private IMagickImage CreateLinearTexture()
        {
            var gradient = new MagickImage("gradient:", this.Thickness, this.Thickness * 4);
            gradient.Rotate(270);

            IMagickImage thick1 = CreateRolled(gradient, this.Thickness);
            IMagickImage thick2 = CreateRolled(gradient, this.Thickness * 2);
            IMagickImage thick3 = CreateRolled(gradient, this.Thickness * 3);

            using (MagickImageCollection images = new MagickImageCollection())
            {
                images.Add(gradient);
                images.Add(thick1);
                images.Add(thick2);
                images.Add(thick3);

                return images.AppendVertically();
            }
        }

        private IMagickImage CreatePattern(int width, int height, IMagickImage texture)
        {
            var size = Math.Max(width, height);
            var pattern = new MagickImage(MagickColors.None, size, size);
            pattern.Texture(texture);

            if (this.Spread == 0.0)
                return pattern;

            if (this.Spread == 100.0)
            {
                pattern.Spread(this.Spread);
                return pattern;
            }

            using (IMagickImage mix = pattern.Clone())
            {
                mix.Spread(this.Spread);

                pattern.Composite(mix, CompositeOperator.Blend, this.Mix.ToString(CultureInfo.InvariantCulture));
                return pattern;
            }
        }

        private IMagickImage CreateTexture()
        {
            if (this.Pattern == EmbroideryPattern.Linear)
                return this.CreateLinearTexture();

            return this.CreateCrosshatchTexture();
        }

        private IMagickImage ToNearBlackWhite(IMagickImage image)
        {
            IMagickImage result = image.Clone();
            if (this.GrayLimit == 0 && this.ColorFuzz == (Percentage)0)
                return result;

            result.ColorFuzz = this.ColorFuzz;
            result.Opaque(MagickColors.White, new MagickColor("gray(" + (100 - this.GrayLimit) + "%)"));
            result.Opaque(MagickColors.Black, new MagickColor("gray(" + this.GrayLimit + "%)"));
            result.ColorFuzz = (Percentage)0;

            return result;
        }
    }
}