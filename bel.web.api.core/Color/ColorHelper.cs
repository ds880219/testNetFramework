namespace bel.web.api.core.Color
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    using bel.web.api.core.objects.Interfaces;

    public class ColorHelper : IColorHelper
    {
        public int Argb(int R, int G, int B)
        {
            return this.Argb(Byte.MaxValue, R, G, B);
        }

        public int Argb(int A, int R, int G, int B)
        {
            byte[] colorByteArr = { (byte)A, (byte)R, (byte)G, (byte)B };
            return this.ByteArrToInt(colorByteArr);
        }

        public int[] Rgb(int argb)
        {
            return new int[] { (argb >> 16) & 0xFF, (argb >> 8) & 0xFF, argb & 0xFF };
        }

        public int ByteArrToInt(byte[] colorByteArr)
        {
            return (colorByteArr[0] << 24) + ((colorByteArr[1] & 0xFF) << 16) + ((colorByteArr[2] & 0xFF) << 8)
                   + (colorByteArr[3] & 0xFF);
        }

        public int[] Rgb2Lab(int R, int G, int B)
        {
            //http://www.brucelindbloom.com

            float r, g, b, X, Y, Z, fx, fy, fz, xr, yr, zr;
            float Ls, _as, bs;
            float eps = 216f / 24389f;
            float k = 24389f / 27f;

            float Xr = 0.964221f; // reference white D50
            float Yr = 1.0f;
            float Zr = 0.825211f;

            // RGB to XYZ
            r = R / 255f; //R 0..1
            g = G / 255f; //G 0..1
            b = B / 255f; //B 0..1

            // assuming sRGB (D65)
            if (r <= 0.04045)
                r = r / 12;
            else
                r = (float)Math.Pow((r + 0.055) / 1.055, 2.4);

            if (g <= 0.04045)
                g = g / 12;
            else
                g = (float)Math.Pow((g + 0.055) / 1.055, 2.4);

            if (b <= 0.04045)
                b = b / 12;
            else
                b = (float)Math.Pow((b + 0.055) / 1.055, 2.4);


            X = 0.436052025f * r + 0.385081593f * g + 0.143087414f * b;
            Y = 0.222491598f * r + 0.71688606f * g + 0.060621486f * b;
            Z = 0.013929122f * r + 0.097097002f * g + 0.71418547f * b;

            // XYZ to Lab
            xr = X / Xr;
            yr = Y / Yr;
            zr = Z / Zr;

            if (xr > eps)
                fx = (float)Math.Pow(xr, 1 / 3f);
            else
                fx = (float)((k * xr + 16f) / 116f);

            if (yr > eps)
                fy = (float)Math.Pow(yr, 1 / 3f);
            else
                fy = (float)((k * yr + 16f) / 116f);

            if (zr > eps)
                fz = (float)Math.Pow(zr, 1 / 3f);
            else
                fz = (float)((k * zr + 16f) / 116);

            Ls = (116 * fy) - 16;
            _as = 500 * (fx - fy);
            bs = 200 * (fy - fz);

            int[] lab = new int[3];
            lab[0] = (int)(2.55 * Ls + .5);
            lab[1] = (int)(_as + .5);
            lab[2] = (int)(bs + .5);
            return lab;
        }

        public string Rgb2Hex(Color rgbColor)
        {
            return $"#{rgbColor.A:X2}{rgbColor.R:X2}{rgbColor.G:X2}{rgbColor.B:X2}";
        }

        /**
         * Computes the difference between two RGB colors by converting them to the L*a*b scale and
         * comparing them using the CIE76 algorithm { http://en.wikipedia.org/wiki/Color_difference#CIE76}
         */
        public double GetColorDifference(Color a, Color b)
        {
            int r1, g1, b1, r2, g2, b2;
            r1 = a.R;
            g1 = a.G;
            b1 = a.B;
            r2 = b.R;
            g2 = b.G;
            b2 = b.B;
            int[] lab1 = this.Rgb2Lab(r1, g1, b1);
            int[] lab2 = this.Rgb2Lab(r2, g2, b2);
            return Math.Sqrt(
                Math.Pow(lab2[0] - lab1[0], 2) + Math.Pow(lab2[1] - lab1[1], 2) + Math.Pow(lab2[2] - lab1[2], 2));
        }

        public int ClosestRgbColor(List<Color> colorList, Color sourceColor, out int distance)
        {
            var closestColor = sourceColor;

            var cursor = 0;
            var indexRgbDiff = -1;
            var rgbDiffs = 1000;
            foreach (var diff in colorList.Select(color => this.ColorDiff(color, closestColor)))
            {
                if (diff < rgbDiffs)
                {
                    indexRgbDiff = cursor;
                    rgbDiffs = diff;
                }

                cursor++;
            }

            // Lavender logic
            if (colorList[indexRgbDiff].Name == "ffc3abd3" 
                || colorList[indexRgbDiff].Name == "ffdddbd3"
                || colorList[indexRgbDiff].Name == "ff717073")
            {
                cursor = 0;
                var indexDeltaEDiff = -1;
                var deltaEDiff = 1000;
                foreach (var diff in colorList.Select(n => ColorFormulas.DoFullCompare(n.R, n.G, n.B, closestColor.R, closestColor.G, closestColor.B)))
                {
                    if (diff < deltaEDiff)
                    {
                        indexDeltaEDiff = cursor;
                        deltaEDiff = diff;
                    }

                    cursor++;
                }

                distance = deltaEDiff;
                return indexDeltaEDiff;
            }

            //cursor = 0;
            //var indexDeltaEDiff = -1;
            //var deltaEDiff = 1000;
            //foreach (var diff in colorList.Select(n => ColorFormulas.DoFullCompare(n.R, n.G, n.B, closestColor.R, closestColor.G, closestColor.B)))
            //{
            //    if (diff < deltaEDiff)
            //    {
            //        indexDeltaEDiff = cursor;
            //        deltaEDiff = diff;
            //    }
            //
            //    cursor++;
            //}

            //if (indexRgbDiff != indexDeltaEDiff)
            //{
            //    // get values for different metrics for each different index
            //    var colorByRgbDistance = colorList[indexRgbDiff];
            //    var indexRgbDiffDeltaEDiff = ColorFormulas.DoFullCompare(
            //        colorByRgbDistance.R,
            //        colorByRgbDistance.G,
            //        colorByRgbDistance.B,
            //        closestColor.R,
            //        closestColor.G,
            //        closestColor.B);
            //
            //    var colorByDeltaEDistance = colorList[indexDeltaEDiff];
            //    var indexDeltaEDiffRgbDistance = this.ColorDiff(colorByDeltaEDistance, closestColor);
            //}

            distance = rgbDiffs;
            return indexRgbDiff;
        }

        /// <summary>
        /// Get the closest named windows color
        /// </summary>
        /// <param name="color">Source Color</param>
        /// <returns></returns>
        public Color GetApproximateColorName(Color color)
        {
            int minDistance = int.MaxValue;
            Color minColor = Color.Empty;

            foreach (var colorProperty in this._colorProperties)
            {
                var colorPropertyValue = (Color)colorProperty.GetValue(null, null);
                if (
                    colorPropertyValue.R == color.R
                    && colorPropertyValue.G == color.G
                    && colorPropertyValue.B == color.B
                    && colorPropertyValue.A == color.A

                )
                {
                    return colorPropertyValue;
                }

                int distance = Math.Abs(colorPropertyValue.R - color.R) +
                               Math.Abs(colorPropertyValue.G - color.G) +
                               Math.Abs(colorPropertyValue.B - color.B) +
                               Math.Abs(colorPropertyValue.A - color.A);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minColor = colorPropertyValue;
                }
            }

            return minColor;
        }

        // distance in RGB space
        private int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt(Math.Pow((c1.R - c2.R), 2) + Math.Pow((c1.G - c2.G), 2) + Math.Pow((c1.B - c2.B), 2));
        }

        public bool CheckColor(Color a, IEnumerable<Color> list, int tolerance)
        {
            foreach (var b in list)
            {
                var sum = 0;

                var diff = a.R - b.R;
                sum += (1 + diff * diff) * a.A / 256;

                diff = a.G - b.G;
                sum += (1 + diff * diff) * a.A / 256;

                diff = a.B - b.B;
                sum += (1 + diff * diff) * a.A / 256;

                diff = a.A - b.A;
                sum += diff * diff;


                if (sum <= tolerance * tolerance * 4)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// List of windows colors 
        /// </summary>
        private readonly IEnumerable<PropertyInfo> _colorProperties =
            typeof(Color)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color));
    }

    public class ColorFormulas
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double CieL { get; set; }

        public double CieA { get; set; }

        public double CieB { get; set; }

        public ColorFormulas(int R, int G, int B)
        {
            this.RGBtoLAB(R, G, B);
        }

        public void RGBtoLAB(int R, int G, int B)
        {
            this.RGBtoXYZ(R, G, B);
            this.XYZtoLAB();
        }

        public void RGBtoXYZ(int RVal, int GVal, int BVal)
        {
            double R = Convert.ToDouble(RVal) / 255.0; //R from 0 to 255
            double G = Convert.ToDouble(GVal) / 255.0; //G from 0 to 255
            double B = Convert.ToDouble(BVal) / 255.0; //B from 0 to 255

            if (R > 0.04045)
            {
                R = Math.Pow(((R + 0.055) / 1.055), 2.4);
            }
            else
            {
                R = R / 12.92;
            }

            if (G > 0.04045)
            {
                G = Math.Pow(((G + 0.055) / 1.055), 2.4);
            }
            else
            {
                G = G / 12.92;
            }

            if (B > 0.04045)
            {
                B = Math.Pow(((B + 0.055) / 1.055), 2.4);
            }
            else
            {
                B = B / 12.92;
            }

            R = R * 100;
            G = G * 100;
            B = B * 100;

            //Observer. = 2°, Illuminant = D65
            this.X = R * 0.4124 + G * 0.3576 + B * 0.1805;
            this.Y = R * 0.2126 + G * 0.7152 + B * 0.0722;
            this.Z = R * 0.0193 + G * 0.1192 + B * 0.9505;
        }

        public void XYZtoLAB()
        {
            // based upon the XYZ - CIE-L*ab formula at easyrgb.com (http://www.easyrgb.com/index.php?X=MATH&H=07#text7)
            double ref_X = 95.047;
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            double var_X = this.X / ref_X; // Observer= 2°, Illuminant= D65
            double var_Y = this.Y / ref_Y;
            double var_Z = this.Z / ref_Z;

            if (var_X > 0.008856)
            {
                var_X = Math.Pow(var_X, (1 / 3.0));
            }
            else
            {
                var_X = (7.787 * var_X) + (16 / 116.0);
            }

            if (var_Y > 0.008856)
            {
                var_Y = Math.Pow(var_Y, (1 / 3.0));
            }
            else
            {
                var_Y = (7.787 * var_Y) + (16 / 116.0);
            }

            if (var_Z > 0.008856)
            {
                var_Z = Math.Pow(var_Z, (1 / 3.0));
            }
            else
            {
                var_Z = (7.787 * var_Z) + (16 / 116.0);
            }

            this.CieL = (116 * var_Y) - 16;
            this.CieA = 500 * (var_X - var_Y);
            this.CieB = 200 * (var_Y - var_Z);
        }

        ///
        /// The smaller the number returned by this, the closer the colors are
        ///
        ///
        /// 
        public int CompareTo(ColorFormulas oComparisionColor)
        {
            // Based upon the Delta-E (1976) formula at easyrgb.com (http://www.easyrgb.com/index.php?X=DELT&H=03#text3)
            double DeltaE = Math.Sqrt(
                Math.Pow((this.CieL - oComparisionColor.CieL), 2) + Math.Pow((this.CieA - oComparisionColor.CieA), 2)
                                                             + Math.Pow((this.CieB - oComparisionColor.CieB), 2));
            return Convert.ToInt16(Math.Round(DeltaE));
        }

        public static int DoFullCompare(int R1, int G1, int B1, int R2, int G2, int B2)
        {
            ColorFormulas oColor1 = new ColorFormulas(R1, G1, B1);
            ColorFormulas oColor2 = new ColorFormulas(R2, G2, B2);
            return oColor1.CompareTo(oColor2);
        }

        public static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                  + (c1.G - c2.G) * (c1.G - c2.G)
                                  + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public static float HueDistance(Color color1, Color color2)
        {
            var avghue = (color1.GetHue() + color2.GetHue()) / 2;
            var distance = Math.Abs(color1.GetHue() - avghue);
            return distance;
        }
    }
}
