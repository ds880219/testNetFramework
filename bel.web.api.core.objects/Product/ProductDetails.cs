// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductDetails.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the ProductDetails type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Product
{
    using System.Drawing;

    /// <summary>
    /// The product details.
    /// </summary>
    public class ProductDetails
    {
        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product color.
        /// </summary>
        public string ProductColor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product material.
        /// </summary>
        public string ProductMaterial { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product image url.
        /// </summary>
        public string ProductImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the imprint area coordinates.
        /// </summary>
        public PointF ImprintAreaCoordinates { get; set; }

        /// <summary>
        /// Gets or sets the imprint area size.
        /// </summary>
        public SizeF ImprintAreaSize { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether original imprint values.
        /// </summary>
        public bool OriginalImprintValues { get; set; } = false;
    }
}