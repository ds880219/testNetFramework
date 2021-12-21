namespace bel.web.api.core.objects.Imaging
{
    using System.Collections.Generic;
    using System.Drawing;

    public class Result
    {
        public Color ColorCompared { get; set; }
        public int DistanceRGB { get; set; }
        public double DeltaE { get; set; }
        public double Hue { get; set; }
        public float Percentage { get; set; }
    }

    public class ColorCompareData
    {
        public List<SimilarColor> ParentColor { get; set; }
        public List<SimilarColor> ParentColorDeltaE { get; set; }
        public List<SimilarColor> ParentColorRgb { get; set; }
        public List<SimilarColor> ParentColorHue { get; set; }
        public Color OriginalColor { get; set; }
        public Color BelColor { get; set; }
        public List<Result> ResultColors { get; set; }
        public float PixelCount { get; set; }
        public float Percentage { get; set; }
        public int HexDecimal { get; set; }
        public bool PassStdDev { get; set; }
        public bool Primary { get; set; } = true;
        public Color? PredominantColor { get; set; }
    }
    
    public class SimilarColor
    {
        public Color Color { get; set; }
        public float Hue {get; set; }
        public float DeltaE {get; set; }
        public float RgbDistance {get; set; }
        public Color ParentColor { get; set; }
    }
}
