// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapColor.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the MapColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Imaging
{
    /// <summary>
    /// The map color.
    /// </summary>
    public class MapColor
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hex.
        /// </summary>
        public string Hex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom color.
        /// </summary>
        public bool CustomColor { get; set; }
    }
}
