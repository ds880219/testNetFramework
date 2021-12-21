// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebossingHelper.cs" company="BEL USA">
//   This product is property of BEL USA
// </copyright>
// <summary>
//   Applies an emboss or deboss effect to each color in an image. The image must have limited number of
//   colors or only the top most frequent colors will be used. Each color will get the same
//   pattern, but at different rotation angles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Imaging.Effect
{
    using System.Drawing.Imaging;
    using System.IO;

    using bel.web.api.core.Imaging.BitmapEffects;

    using Emgu.CV.CvEnum;

    using ImageMagick;

    /// <summary>
    /// Applies an emboss or deboss effect to each color in an image. The image must have limited number of
    /// colors or only the top most frequent colors will be used. Each color will get the same
    /// pattern, but at different rotation angles.
    /// </summary>
    public sealed class DebossingHelper
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="isText">
        /// The is text.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] Execute(IMagickImage input, bool isText = false)
        {
            var radius = 5;

            if (isText)
            {
                radius = 2;
            }

            input.Emboss(radius, 1.5);
            return input.ToByteArray(MagickFormat.Png);
        }
    }
}