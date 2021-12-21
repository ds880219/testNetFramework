namespace bel.web.api.core.objects.Interfaces
{
    using System.Collections.Generic;

    using bel.web.api.core.objects.ImageEffects;
    using bel.web.api.core.objects.Product;

    public interface IProductHelper
    {
        List<EffectParameter> GetDebossingColor(ProductDetails productDetails);
        List<EffectParameter> GetTexture(ProductDetails productDetails);
    }
}
