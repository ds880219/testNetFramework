// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Effect.cs" company="BEL USA">
//   This product is property of BEL USA
// </copyright>
// <summary>
//   Defines the Effect type to apply over an image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.ImageEffects
{
    /// <summary>The image print method.</summary>
    public enum Effect
    {
        /// <summary>The none.</summary>
        None = 1,

        /// <summary>The one color.</summary>
        OneColor = 2,

        /// <summary>The embroidery print method.</summary>
        Embroidery = 3,

        /// <summary>The debossing print method.</summary>
        Debossing = 4,

        /// <summary>The laser print method.</summary>
        laser_engraved = 5,

        /// <summary>The screen print method.</summary>
        screen_print = 6
    }
}
