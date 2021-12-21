// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmbroideryPattern.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the EmbroideryPattern type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Enums
{
    /// <summary>The embroidery pattern enumerator.</summary>
    public enum EmbroideryPattern 
    {
           /// <summary>
           /// Linear.
           /// </summary>
           Linear,

           /// <summary>
           /// Crosshatch.
           /// </summary>
           Crosshatch
    }

    /// <summary> The debossing method.</summary>
    public enum DebossingMethod
    {
        /// <summary>The GIM approach.</summary>
        GIM = 1,

        /// <summary>The Photoshop approach.</summary>
        Photoshop = 2
    }

    /// <summary>The debossing compose.</summary>
    public enum DebossingCompose
    {
        /// <summary>TODO The no composite.</summary>
        NoComposite = 1
    }
}
