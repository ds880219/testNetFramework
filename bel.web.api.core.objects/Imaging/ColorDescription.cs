// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorDescription.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the ColorDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Imaging
{
    using Newtonsoft.Json;

    /// <summary>
    /// The color description.
    /// </summary>
    public class ColorDescription
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hex value.
        /// </summary>
        [JsonProperty(PropertyName = "hexValue")]
        public string HexValue { get; set; }
    }
}