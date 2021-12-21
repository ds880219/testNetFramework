// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageActions.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the ImageActions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Imaging
{
    using System.Collections.Generic;

    using bel.web.api.core.objects.ImageEffects;

    /// <summary>
    /// The image actions.
    /// </summary>
    public class ImageActions
    {
        /// <summary>
        /// Gets or sets a value indicating whether cropped.
        /// </summary>
        public bool Cropped { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether black background removed.
        /// </summary>
        public bool BlackBgRemoved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether white background removed.
        /// </summary>
        public bool WhiteBgRemoved { get; set; }

        /// <summary>
        /// Gets or sets the effect.
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether one color.
        /// </summary>
        public bool OneColor { get; set; }

        /// <summary>
        /// Gets or sets the one color code.
        /// </summary>
        public string OneColorCode { get; set; }

        /// <summary>
        /// Gets or sets the effect parameters.
        /// </summary>
        public List<EffectParameter> EffectParameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom mapping.
        /// </summary>
        public bool UseCustomMapping { get; set; }

        /// <summary>
        /// Gets or sets the map colors book.
        /// </summary>
        public List<MapColor> MapColorsBook { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trim.
        /// </summary>
        public bool Trim { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether detect colors.
        /// </summary>
        public bool DetectColors { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether replace color.
        /// </summary>
        public bool ReplaceColor { get; set; }

        /// <summary>
        /// Gets or sets the color details to use to replace colors.
        /// </summary>
        public List<ColorDetail> ColorDetails { get; set; }
    }
}
