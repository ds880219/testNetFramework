// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="BEL USA">
//   This product is property of BEL  USA.
// </copyright>
// <summary>
//   Defines the Enums type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Enums
{
    /// <summary>The enumerators.</summary>
    public class DesignLab
    {
        /// <summary>
        /// The Type of the imprint object type (Image or Text)
        /// </summary>
        public enum ImprintObjectType
        {
            /// <summary>The text.</summary>
            Text,

            /// <summary>The image.</summary>
            Image
        }

        /// <summary>
        /// The locations available for imprint
        /// </summary>
        public enum ImprintLocation
        {
            /// <summary>The front.</summary>
            Front,

            /// <summary>The back.</summary>
            Back,

            /// <summary>The left.</summary>
            Left,

            /// <summary>The right.</summary>
            Right,

            /// <summary>The logo.</summary>
            Logo,

            /// <summary>The wrap.</summary>
            Wrap,

            /// <summary>The unknown.</summary>
            Unknown,

            /// <summary>The front rb.</summary>
            FrontRB,

            /// <summary>The front lb.</summary>
            FrontLB,

            /// <summary>The back upper.</summary>
            BackUpper,

            /// <summary>The back full.</summary>
            BackFull,

            /// <summary>The back lower.</summary>
            BackLower
        }

        /// <summary>The return type.</summary>
        public enum ReturnType
        {
            /// <summary>The url.</summary>
            URL = 1,

            /// <summary>The base 64.</summary>
            Base64 = 2,

            /// <summary>The bytes.</summary>
            Bytes = 3,

            /// <summary>The image.</summary>
            Image = 4
        }
    }
}
