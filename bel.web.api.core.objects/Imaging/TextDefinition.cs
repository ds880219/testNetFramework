namespace bel.web.api.core.objects.Imaging
{
    public class TextDefinition
    {
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontWeight { get;set; }  
        public string FontStyle { get; set; }
        public string Text { get; set; }
        public bool TextShapeFlip { get; set; }
        public int TextShapeOption { get; set; }
        public string TextAlign { get; set; }
        public ColorDescription ConvertedColor { get; set; }
    }
}
