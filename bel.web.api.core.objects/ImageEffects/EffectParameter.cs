namespace bel.web.api.core.objects.ImageEffects
{
    public class EffectParameter
    {
        public EffectAction Action { get; set; }
        public object Value { get; set; }
    }

    public enum EffectAction
    {
        Color = 1,
        Border = 2,
        Texture = 3,
    }
}
