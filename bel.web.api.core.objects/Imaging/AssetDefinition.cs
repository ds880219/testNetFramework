// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetDefinition.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the AssetDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.objects.Imaging
{
    public class AssetDefinition
    {
        public string DesignId { get; set; }
        public string Id { get; set; }
        public bool IsBase64 { get; set; } = false;
        public string Base64Image { get; set; }
        public string ImageUrl { get; set; }
        public float MaxWidth { get; set; }
        public float MaxHeight { get; set; }
        public float SetWidth { get; set; }
        public float SetHeight { get; set; }
    }
}
