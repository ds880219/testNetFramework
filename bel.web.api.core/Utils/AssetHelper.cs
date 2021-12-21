// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetHelper.cs" company="BEL USA">
//   This is product property of BEL USA.
// </copyright>
// <summary>
//   Defines the AssetHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Utils
{
    using bel.web.api.preview.core.@base.Exception;
    using System;
    using System.Linq;
    using System.Net;

    /// <summary>The asset helper.</summary>
    public class AssetHelper
    {
        /// <summary>The download stream file.</summary>
        /// <param name="url">The url.</param>
        /// <param name="attempt">The attempt.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static byte[] DownloadStreamFile(string url,  int attempt = 1, string details = "")
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var webClient = new WebClient();
            try
            {
                var stream = webClient.DownloadData(url);
                return stream.ToArray();
            }
            catch (WebException webException)
            {
                if (((HttpWebResponse)webException.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException($"Not found {details}. {url}");
                }
                
                throw new Exception(webException.Message);
            }
            catch (Exception ex)
            {
                if (attempt >= ConfigHelper.MaxDownloadAttempts())
                {
                    return null;
                }

                attempt++;
                DownloadStreamFile(url, attempt);
                return null;
            }
        }
    }
}
