namespace bel.web.api.core.objects.Interfaces
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface IColorHelper
    {
        int Argb(int R, int G, int B);

        int Argb(int A, int R, int G, int B);

        int[] Rgb(int argb);

        int ByteArrToInt(byte[] colorByteArr);

        int[] Rgb2Lab(int R, int G, int B);

        string Rgb2Hex(Color rgbColor);

        double GetColorDifference(Color a, Color b);

        int ClosestRgbColor(List<Color> colorList, Color sourceColor, out int distance);

        Color GetApproximateColorName(Color color);

        bool CheckColor(Color a, IEnumerable<Color> list, int tolerance);
    }
}
