// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigHelper.cs" company="BEL USA">
//   This is product property of BEL USA.
// </copyright>
// <summary>
//   The config helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Utils
{
    using System;
    using System.Configuration;

    /// <summary>The config helper.</summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// The product image path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ProductImagePath()
        {
            return ConfigurationManager.AppSettings["ProductImagePath"];
        }

        /// <summary>The aws access key.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string AwsAccessKey()
        {
            return ConfigurationManager.AppSettings["AWS_ACCESS_KEY"];
        }

        /// <summary>The aws secret key.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string AwsSecretKey()
        {
            return ConfigurationManager.AppSettings["AWS_SECRET_KEY"];
        }

        /// <summary>The aws bucket name.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string AwsBucketName()
        {
            return ConfigurationManager.AppSettings["BUCKET_NAME"];
        }

        /// <summary>The max download attempts.</summary>
        /// <returns>The <see cref="int"/>.</returns>
        public static int MaxDownloadAttempts()
        {
            return Convert.ToUInt16(ConfigurationManager.AppSettings["MaxDownloadAttempts"]);
        }

        /// <summary>The get pdf clip art source.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetPdfClipArtSource()
        {
            return ConfigurationManager.AppSettings["pdf_clip_art_source"];
        }
        
        /// <summary>
        /// Path for temp path folder
        /// </summary>
        /// <returns>temp path folder</returns>
        public static string GetTempFilePath()
        {
            return System.IO.Path.GetTempPath();
        }

        /// <summary>
        /// The addon image path.
        /// </summary>
        public static string AddonImagePath => ConfigurationManager.AppSettings["AddonImagePath"];
    }
}
