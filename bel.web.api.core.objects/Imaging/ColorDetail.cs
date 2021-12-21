// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorDetail.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   The color detail.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Imaging
{
    /// <summary>
    /// The color detail.
    /// </summary>
    public class ColorDetail
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the original color.
        /// </summary>
        public ARGBColor OriginalColor { get; set; }

        /// <summary>
        /// Gets or sets the mapped color.
        /// </summary>
        public ARGBColor MappedColor { get; set; }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public double Percentage { get; set; }

        /// <summary>
        /// Gets or sets the replace color.
        /// </summary>
        public MapColor ReplaceColor { get; set; }

        /// <summary>
        /// Gets or sets the visible color flag.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the primary color flag.
        /// </summary>
        public bool Primary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether background.
        /// </summary>
        public bool Background { get; set; }
    }
}
