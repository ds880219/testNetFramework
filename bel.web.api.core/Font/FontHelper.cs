// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontHelper.cs" company="BEL USA">
//   This product is property of BEL USA
// </copyright>
// <summary>
//   Defines the FontHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace bel.web.api.core.Font
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The font helper.</summary>
    public class FontHelper
    {
        /// <summary>The fonts mapper between the preview generator and the design lab.</summary>
        private static Dictionary<string, string> FontsMapper = new Dictionary<string, string>
        {
            { "Coda", "Coda ExtraBold" },
            { "Coustard", "Coustard Black" },
            { "Shadows Into", "Shadows Into Light Two" },
        };

        private static List<string> FontsIssueSizes = new List<string>
        {
             "teko",
             "lobster" 
        };

        /// <summary>Get font map if exist a mapping font.</summary>
        /// <param name="font">The font.</param>
        /// <returns>The <see cref="string"/> mapped font.</returns>
        public static string GetFont(string font)
        {
            return FontsMapper.Any(f => f.Key == font) ? FontsMapper[font] : font;
        }

        public static System.Drawing.Font ReviseFontToCalculateSize(System.Drawing.Font fontSource)
        {
            if (FontsIssueSizes.Contains(fontSource.FontFamily.Name.ToLower()))
            {
                var newFont = new System.Drawing.Font(fontSource.FontFamily, fontSource.Size, System.Drawing.FontStyle.Bold);
                return newFont;
            }
            else
            {                
                return fontSource;
            }
        }
    }
}
