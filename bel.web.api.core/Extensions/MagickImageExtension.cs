// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MagickImageExtension.cs" company="BEL USA">
//   This product is property of BEL USA.
// </copyright>
// <summary>
//   Defines the MagickImageExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using bel.web.api.core.Color;
    using bel.web.api.core.objects.Imaging;
    using bel.web.api.core.objects.Interfaces;

    using ImageMagick;

    /// <summary>
    /// The MagickImage extension.
    /// </summary>
    public static class MagickImageExtension
    {
        /// <summary>
        /// The get color list.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="imageHelper">
        /// The image helper.
        /// </param>
        /// <param name="colorHelper">
        /// The color helper.
        /// </param>
        /// <param name="tileCount">
        /// The tile count.
        /// </param>
        /// <param name="bg">
        /// The bg.
        /// </param>
        /// <param name="deltaECut">
        /// The delta e cut.
        /// </param>
        /// <param name="hueCut">
        /// The hue cut.
        /// </param>
        /// <param name="rgbCut">
        /// The rgb cut.
        /// </param>
        /// <param name="percentageCut">
        /// The percentage cut.
        /// </param>
        /// <param name="deltaECutPassSD">
        /// The delta e cut pass sd.
        /// </param>
        /// <param name="hueCutPassSD">
        /// The hue cut pass sd.
        /// </param>
        /// <param name="rgbCutPassSD">
        /// The rgb cut pass sd.
        /// </param>
        /// <param name="percentageCutPassSD">
        /// The percentage cut pass sd.
        /// </param>
        /// <param name="mapColors">
        /// The map colors.
        /// </param>
        /// <param name="similarColorResults">
        /// The similar color results.
        /// </param>
        /// <param name="histogramColors">
        /// The histogram colors.
        /// </param>
        /// <param name="histogramList">
        /// The histogram list.
        /// </param>
        /// <param name="histogramListSorted">
        /// The histogram list sorted.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<ColorDetail> GetColorList(
            this IMagickImage image,
            IImageHelper imageHelper,
            IColorHelper colorHelper,
            int tileCount,
            ColorDetail bg,
            float deltaECut,
            float hueCut,
            float rgbCut,
            float percentageCut,
            float deltaECutPassSD,
            float hueCutPassSD,
            float rgbCutPassSD,
            float percentageCutPassSD,
            Dictionary<string, string> mapColors,
            int totalColors,
            out List<ColorCompareData> similarColorResults,
            out int histogramColors,
            out List<Color> histogramList, out List<Color> histogramListSorted)
        {
            List<KeyValuePair<MagickColor, int>> histogram;
            List<KeyValuePair<MagickColor, int>> histogramFull;

            if (totalColors >= 1000)
            {
                tileCount = 1;
            }

            var qs = new QuantizeSettings()
            {
                 Colors = 24, // 128
                 DitherMethod = DitherMethod.No,
            };

            image.Quantize(qs);
            histogram = image.Histogram().ToList();
            histogramFull = new List<KeyValuePair<MagickColor, int>>(histogram);
            histogramList = histogram.Select(c => c.Key.ToColor()).ToList();
            histogramListSorted = histogram.OrderByDescending(c => c.Value).Select(c => c.Key.ToColor()).ToList();
            similarColorResults = new List<ColorCompareData>();
            histogramColors = histogramList.Count;
            return tileCount != 1 ? GenerateResultsUsingCroppingStrategy(image, imageHelper, colorHelper, tileCount, bg, deltaECut, hueCut, rgbCut, percentageCut, deltaECutPassSD, percentageCutPassSD, mapColors, out similarColorResults, histogramFull, histogram) : 
                GenerateResultsUsingNonCroppingStrategy(image, imageHelper, colorHelper, bg, deltaECut, hueCut, rgbCut, percentageCut, deltaECutPassSD, hueCutPassSD, rgbCutPassSD, percentageCutPassSD, mapColors, histogram, out similarColorResults, out histogramColors, out histogramList);
        }

        /// <summary>
        /// The generate results using non cropping strategy.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="imageHelper">
        /// The image helper.
        /// </param>
        /// <param name="colorHelper">
        /// The color helper.
        /// </param>
        /// <param name="bg">
        /// The bg.
        /// </param>
        /// <param name="deltaECut">
        /// The delta e cut.
        /// </param>
        /// <param name="hueCut">
        /// The hue cut.
        /// </param>
        /// <param name="rgbCut">
        /// The rgb cut.
        /// </param>
        /// <param name="percentageCut">
        /// The percentage cut.
        /// </param>
        /// <param name="deltaECutPassSD">
        /// The delta e cut pass sd.
        /// </param>
        /// <param name="hueCutPassSD">
        /// The hue cut pass sd.
        /// </param>
        /// <param name="rgbCutPassSD">
        /// The rgb cut pass sd.
        /// </param>
        /// <param name="percentageCutPassSD">
        /// The percentage cut pass sd.
        /// </param>
        /// <param name="mapColors">
        /// The map colors.
        /// </param>
        /// <param name="similarColorResults">
        /// The similar color results.
        /// </param>
        /// <param name="histogramColors">
        /// The histogram colors.
        /// </param>
        /// <param name="histogramList">
        /// The histogram list.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<ColorDetail> GenerateResultsUsingNonCroppingStrategy(
            IMagickImage image,
            IImageHelper imageHelper,
            IColorHelper colorHelper,
            ColorDetail bg,
            float deltaECut,
            float hueCut,
            float rgbCut,
            float percentageCut,
            float deltaECutPassSD,
            float hueCutPassSD,
            float rgbCutPassSD,
            float percentageCutPassSD,
            Dictionary<string, string> mapColors,
            List<KeyValuePair<MagickColor, int>> histogramNonSorted,
            out List<ColorCompareData> similarColorResults,
            out int histogramColors,
            out List<Color> histogramList)
        {
            List<KeyValuePair<MagickColor, int>> histogram;
            List<KeyValuePair<MagickColor, int>> histogramFull;

            histogramList = histogramNonSorted.Select(c => c.Key.ToColor()).ToList();
            histogram = histogramNonSorted.OrderByDescending(c => c.Value).ToList();
            histogramColors = histogram.Count();
            histogramFull = new List<KeyValuePair<MagickColor, int>>(histogram);
            var totalPixelCount = histogramFull.Sum(hc => hc.Value);

            // Remove BG or predominant color almost always is the BG even if it's transparent.
            RemoveMostLikelyBg(imageHelper, colorHelper, bg, image.BackgroundColor, histogramFull);

            // Remove colors with a percentage less than 0.1.
            foreach (var hc in histogram)
            {
                var percentage = ((float)hc.Value / (float)totalPixelCount) * 100;
                if (percentage < 0.1)
                {
                    histogramFull.Remove(hc);
                }
            }

            // Can happen when the image is fully white
            if (histogramFull.Count == 0)
            {
                similarColorResults = null;
                return new List<ColorDetail>();
            }

            // Apply standard deviation base on the pixel count.
            var histogramPassColors = imageHelper.ProcessHistogram(histogramFull);

            // Generate comparision metrics between each color on the histogram.
            var results = GenerateMetrics(
                colorHelper,
                histogramFull,
                histogramFull,
                imageHelper,
                totalPixelCount,
                histogramPassColors.Count,
                deltaECut,
                hueCut,
                rgbCut,
                percentageCut,
                mapColors);
            
            foreach (var result in results)
            {
                if (histogramPassColors.Exists(hc => hc.Key.ToColor().Name == result.OriginalColor.Name))
                {
                    result.PassStdDev = true;
                }
            }

            similarColorResults = new List<ColorCompareData>(results);

            // Delete the similar colors.
            DeleteSimilarColors(imageHelper, colorHelper, results, deltaECutPassSD, hueCutPassSD, rgbCutPassSD, percentageCutPassSD);

            var compareList = new List<Color>();

            foreach (var color in mapColors)
            {
                var newColor = ColorTranslator.FromHtml(color.Key);
                compareList.Add(newColor);
            }

            compareList.Add(Color.Transparent);
            var distance = 0;
            
            if (bg != null)
            {
                var bgSystemColor = Color.FromArgb(Convert.ToInt32(bg.OriginalColor.A), Convert.ToInt32(bg.OriginalColor.R), Convert.ToInt32(bg.OriginalColor.G), Convert.ToInt32((int)bg.OriginalColor.B));
                var mapped = compareList[colorHelper.ClosestRgbColor(compareList, bgSystemColor, out distance)];
                var bgMappedColor = new ARGBColor()
                                        {
                                            A = mapped.A,
                                            R = mapped.R,
                                            G = mapped.G,
                                            B = mapped.B,
                                            Name = mapped.Name,
                                            Hex = bg.OriginalColor.Hex
                                        };
                bg.MappedColor = bgMappedColor;
            }

            // Map the colors to return.
            var colorDetails = new List<ColorDetail>();
            foreach (var e in results)
            {
                var originalColor = new ARGBColor()
                                        {
                                            A = e.OriginalColor.A,
                                            R = e.OriginalColor.R,
                                            G = e.OriginalColor.G,
                                            B = e.OriginalColor.B,
                                            Name = e.OriginalColor.Name
                                        };
                var mapped = compareList[colorHelper.ClosestRgbColor(compareList, e.OriginalColor, out distance)];

                var mappedColor = new ARGBColor()
                                      {
                                          A = mapped.A,
                                          R = mapped.R,
                                          G = mapped.G,
                                          B = mapped.B,
                                          Name = mapped.Name
                                      };

                var existMap = colorDetails.FirstOrDefault(c => c.MappedColor.Name == mappedColor.Name);
                var uuid = (existMap == null) ? Guid.NewGuid().ToString().Replace("-", string.Empty) : existMap.Uuid;
                var visible = existMap == null;

                if (!e.Primary && e.PredominantColor.HasValue && (bg == null || bg.MappedColor.Name.ToLower() != mappedColor.Name.ToLower()))
                {
                    var existOriginal = colorDetails.FirstOrDefault(c => c.OriginalColor.Name == e.PredominantColor.Value.Name);
                    if (existOriginal != null)
                    {
                        uuid = existOriginal.Uuid;
                        mappedColor = existOriginal.MappedColor;
                        visible = false;
                    }
                }

                colorDetails.Add(
                    new ColorDetail
                        {
                            OriginalColor = originalColor, 
                            MappedColor = mappedColor, 
                            Percentage = e.Percentage,
                            Uuid = uuid,
                            Visible = visible,
                            Primary = e.Primary
                        });
            }

            return colorDetails;
        }

        /// <summary>
        /// The generate results using cropping strategy.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="imageHelper">
        /// The image helper.
        /// </param>
        /// <param name="colorHelper">
        /// The color helper.
        /// </param>
        /// <param name="tileCount">
        /// The tile count.
        /// </param>
        /// <param name="bg">
        /// The bg.
        /// </param>
        /// <param name="deltaECut">
        /// The delta e cut.
        /// </param>
        /// <param name="hueCut">
        /// The hue cut.
        /// </param>
        /// <param name="rgbCut">
        /// The rgb cut.
        /// </param>
        /// <param name="percentageCut">
        /// The percentage cut.
        /// </param>
        /// <param name="deltaECutPassSD">
        /// The delta e cut pass sd.
        /// </param>
        /// <param name="percentageCutPassSD">
        /// The percentage cut pass sd.
        /// </param>
        /// <param name="mapColors">
        /// The map colors.
        /// </param>
        /// <param name="similarColorResults">
        /// The similar color results.
        /// </param>
        /// <param name="histogramFull">
        /// The histogram full.
        /// </param>
        /// <param name="histogram">
        /// The histogram.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<ColorDetail> GenerateResultsUsingCroppingStrategy(
            IMagickImage image,
            IImageHelper imageHelper,
            IColorHelper colorHelper,
            int tileCount,
            ColorDetail bg,
            float deltaECut,
            float hueCut,
            float rgbCut,
            float percentageCut,
            float deltaECutPassSD,
            float percentageCutPassSD,
            Dictionary<string,string> mapColors,
            out List<ColorCompareData> similarColorResults,
            List<KeyValuePair<MagickColor, int>> histogramFull,
            List<KeyValuePair<MagickColor, int>> histogram)
        {
            var tiles = image.CropToTiles(image.Width / tileCount, image.Height / tileCount);
            var colorList = new List<Color>();

            // do process for full image 
            var tileHistogram = histogramFull.ToList();
            RemoveMostLikelyBg(imageHelper, colorHelper, bg, image.BackgroundColor, tileHistogram);
            if (tileHistogram.Any())
            {
                var passTileHistogram = imageHelper.ProcessHistogram(tileHistogram);
                foreach (var color in passTileHistogram)
                {
                    if (!colorList.Exists(c => c.Name == color.Key.ToColor().Name))
                    {
                        colorList.Add(color.Key.ToColor());
                    }
                }
            }

            // do process for tiles
            foreach (var tile in tiles)
            {
                tileHistogram = tile.Histogram().ToList();
                RemoveMostLikelyBg(imageHelper, colorHelper, bg, image.BackgroundColor, tileHistogram);
                if (tileHistogram.Any())
                {
                    var passTileHistogram = imageHelper.ProcessHistogram(tileHistogram);
                    if (!colorList.Exists(c => c.Name == passTileHistogram[0].Key.ToColor().Name))
                    {
                        colorList.Add(passTileHistogram[0].Key.ToColor());
                    }
                }
            }

            var totalPixelCount = histogramFull.Sum(hc => hc.Value);

            // Get only the selected colors
            foreach (var color in histogram)
            {
                if (!colorList.Exists(c => c.Name == color.Key.ToColor().Name))
                {
                    histogramFull.Remove(color);
                }
            }

            MagickColor bgMagickColor = null;
            if (bg != null && colorList.Count > 1)
            {
                var bgHistogramColor = histogram.FirstOrDefault(c => bg.OriginalColor.Name == c.Key.ToColor().Name);
                if (bgHistogramColor.Key != null)
                {
                    histogramFull.Add(bgHistogramColor);
                }
            }

            // Generate comparision metrics between each color on the histogram.
            var results = GenerateMetrics(
                colorHelper,
                histogramFull,
                histogramFull,
                imageHelper,
                totalPixelCount,
                histogramFull.Count,
                deltaECutPassSD,
                hueCut,
                rgbCut,
                percentageCut,
                mapColors);

            foreach (var result in results)
            {
                result.PassStdDev = true;
            }

            similarColorResults = new List<ColorCompareData>(results);

            // Delete the similar colors.
            var originalResults = DeleteSimilarColors(imageHelper, colorHelper, results, deltaECut, hueCut, rgbCut, percentageCutPassSD);

            // Check if the background can delete all colors
            if (results.Count == 0)
            {
                results = new List<ColorCompareData>();
                foreach (var colorResult in similarColorResults)
                {
                    if (results.Any(c => c.BelColor.Name == colorResult.BelColor.Name))
                    {
                        continue;
                    }

                    results.Add(colorResult);
                }
            }

            // Check if the only color that left was the background to add the second one with more percentage.
            else if (results.Count == 1 && bg != null && bg.OriginalColor.Name == results[0].OriginalColor.Name)
            {
                try
                {
                    results.Add(originalResults[1]);
                }
                catch
                {
                }

            }

            // Remove bg
            else if (bg != null && colorList.Count > 1)
            {
                var resultBgColor = results.FirstOrDefault(r => r.OriginalColor.Name == bg.OriginalColor.Name);
                results.Remove(resultBgColor);
            }

            var compareList = new List<Color>();

            foreach (var color in mapColors)
            {
                var newColor = ColorTranslator.FromHtml(color.Key);
                compareList.Add(newColor);
            }

            compareList.Add(Color.Transparent);

            // Map the color for the bg
            var distance = 0;
            if (bg != null)
            {
                var bgSystemColor = Color.FromArgb(Convert.ToInt32(bg.OriginalColor.A), Convert.ToInt32(bg.OriginalColor.R), Convert.ToInt32(bg.OriginalColor.G), Convert.ToInt32((int)bg.OriginalColor.B));
                var mapped = compareList[colorHelper.ClosestRgbColor(compareList, bgSystemColor, out distance)];
                var bgMappedColor = new ARGBColor()
                                      {
                                          A = mapped.A,
                                          R = mapped.R,
                                          G = mapped.G,
                                          B = mapped.B,
                                          Name = mapped.Name,
                                          Hex = bg.OriginalColor.Hex
                };
                bg.MappedColor = bgMappedColor;
            }

            // Map the colors to return.
            var colorDetails = new List<ColorDetail>();
            foreach (var e in results)
            {
                var originalColor = new ARGBColor()
                                        {
                                            A = e.OriginalColor.A,
                                            R = e.OriginalColor.R,
                                            G = e.OriginalColor.G,
                                            B = e.OriginalColor.B,
                                            Name = e.OriginalColor.Name
                                        };
                var mapped = compareList[colorHelper.ClosestRgbColor(compareList, e.OriginalColor, out distance)];

                var mappedColor = new ARGBColor()
                                      {
                                          A = mapped.A,
                                          R = mapped.R,
                                          G = mapped.G,
                                          B = mapped.B,
                                          Name = mapped.Name
                                      };

                var existMap = colorDetails.FirstOrDefault(c => c.MappedColor.Name == mappedColor.Name);
                var uuid = (existMap == null) ? Guid.NewGuid().ToString().Replace("-", string.Empty) : existMap.Uuid;
                var visible = existMap == null;

                colorDetails.Add(
                    new ColorDetail()
                        {
                            OriginalColor = originalColor, 
                            MappedColor = mappedColor, 
                            Percentage = e.Percentage,
                            Uuid = uuid,
                            Visible = visible,
                            Primary = e.Primary
                    });
            }

            return colorDetails;
        }

        /// <summary>Remove th background or the element with more pixels on the image.</summary>
        /// <param name="bg">Background.</param>
        /// <param name="histogramFull">The histogram with the colors sorted by pixel count.</param>
        /// <returns>True if we remove th bg.</returns>
        private static bool RemoveMostLikelyBg(IImageHelper imageHelper, IColorHelper colorHelper, ColorDetail bg, MagickColor bgImageMagick, IList<KeyValuePair<MagickColor, int>> histogramFull)
        {
            var bgRemoved = false;
            if (bg != null)
            {
                // Remove transparent, black or white the possible BG that the API detects.
                var colorName = string.Empty;
                var bgColor = new KeyValuePair<MagickColor, int>();
                switch (bg.OriginalColor.Name.ToUpper())
                {
                    case "TRANSPARENT":
                        bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == "0");
                        break;
                    case "BLACK":
                    case "FF000000":
                        bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == "0");
                        if (bgColor.Key == null)
                        {
                            bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == bg.OriginalColor.Name
                                                                        || c.Key.ToColor().Name.ToUpper() == "FF000000" || c.Key.ToColor().Name.ToUpper() == "000000");
                        }
                        break;
                    case "WHITE":
                    case "FFFFFFFF":
                    
                        bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == "0");
                        if (bgColor.Key == null)
                        {
                            bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == bg.OriginalColor.Name
                                                                        || c.Key.ToColor().Name.ToUpper() == "FFFFFF" ||
                                                                        c.Key.ToColor().Name.ToUpper() == "FFFFFFFF");
                        }
                        break;
                    default:
                        bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == "0");
                        if (bgColor.Key == null)
                        {
                            bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == bg.OriginalColor.Name);
                            if (bgColor.Key == null)
                            {
                                var bgSystemColor = Color.FromArgb(Convert.ToInt32(bg.OriginalColor.A), Convert.ToInt32(bg.OriginalColor.R), Convert.ToInt32(bg.OriginalColor.G), Convert.ToInt32((int)bg.OriginalColor.B));
                                var colorApproximate = colorHelper.GetApproximateColorName(bgSystemColor);
                                bgColor = histogramFull.FirstOrDefault(c => colorHelper.GetApproximateColorName(c.Key.ToColor()).Name == colorApproximate.Name);
                            }
                        }

                        break;
                }
                
                if (bgColor.Key != null)
                {
                    histogramFull.Remove(bgColor);
                }
                else
                {
                    // Remove the first one for the standard deviation assuming that's the bg
                    bgRemoved = true;
                    histogramFull.RemoveAt(0);
                }
            }
            //else
            //{
            //    // Remove the first one for the standard deviation assuming that's the bg
            //    var bgColor = histogramFull.FirstOrDefault(c => c.Key.ToColor().Name == "0");
            //    if (bgColor.Key == null)
            //    {
            //        bgColor = histogramFull.FirstOrDefault(
            //            c => c.Key.ToColor().Name == bgImageMagick.ToColor().Name);
            //        if (bgColor.Key == null)
            //        {
            //            histogramFull.RemoveAt(0);
            //        }
            //        else
            //        {
            //            histogramFull.Remove(bgColor);
            //        }
            //    }
            //}

            return bgRemoved;
        }

        /// <summary>
        /// The generate metrics.
        /// </summary>
        /// <param name="colorHelper">
        /// The color helper.
        /// </param>
        /// <param name="histogram">
        /// The histogram.
        /// </param>
        /// <param name="histogramCompare">
        /// The histogram compare.
        /// </param>
        /// <param name="imageHelper">
        /// The image helper.
        /// </param>
        /// <param name="totalPixelCount">
        /// The total pixel count.
        /// </param>
        /// <param name="totalColorsPassed">
        /// The total colors passed.
        /// </param>
        /// <param name="deltaECut">
        /// The delta e cut.
        /// </param>
        /// <param name="hueCut">
        /// The hue cut.
        /// </param>
        /// <param name="rgbCut">
        /// The rgb cut.
        /// </param>
        /// <param name="percentageCut">
        /// The percentage cut.
        /// </param>
        /// <param name="mapColors">
        /// The map colors.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<ColorCompareData> GenerateMetrics(IColorHelper colorHelper, List<KeyValuePair<MagickColor, int>> histogram, List<KeyValuePair<MagickColor, int>> histogramCompare, IImageHelper imageHelper, int totalPixelCount, int totalColorsPassed,
                                                              float deltaECut, float hueCut, float rgbCut, float percentageCut, Dictionary<string, string> mapColors)
        {
            float avgPercentageNotPass = percentageCut;
            if (histogram.Count != totalColorsPassed)
            {
                var notPassedHistogram = histogram.GetRange(totalColorsPassed, histogram.Count - totalColorsPassed);
                var totalpixelsnotpassed = notPassedHistogram.Sum(hc => ((float)hc.Value / totalPixelCount) * 100);
                avgPercentageNotPass = totalpixelsnotpassed / (histogram.Count - totalColorsPassed);
                percentageCut = avgPercentageNotPass;
            }

            var compareList = new List<Color>();

            foreach (var color in mapColors)
            {
                var newColor = ColorTranslator.FromHtml(color.Key);
                compareList.Add(newColor);
            }

            compareList.Add(Color.Transparent);

            var results = new List<ColorCompareData>();
            foreach (var color in histogram)
            {
                var distance = 0;
                var result = new ColorCompareData()
                                 {
                                     OriginalColor = color.Key.ToColor(),
                                     HexDecimal = int.Parse(color.Key.ToColor().Name, System.Globalization.NumberStyles.HexNumber),
                                     PixelCount = color.Value,
                                     BelColor = compareList[colorHelper.ClosestRgbColor(compareList, color.Key.ToColor(), out distance)],
                                     Percentage = ((float)color.Value / (float)totalPixelCount) * 100
                                 };
                var colorToCompare = histogramCompare.Where(c => c.Key != color.Key).ToList();
                result.ResultColors = new List<Result>();
                foreach (var colorCompare in colorToCompare)
                {
                    var resultCompare = new Result
                                            {
                                                ColorCompared = colorCompare.Key.ToColor(),
                                                DistanceRGB = ColorFormulas.ColorDiff(result.OriginalColor, colorCompare.Key.ToColor()),
                                                DeltaE = ColorFormulas.DoFullCompare(color.Key.ToColor().R, color.Key.ToColor().G, color.Key.ToColor().B, colorCompare.Key.ToColor().R, colorCompare.Key.ToColor().G, colorCompare.Key.ToColor().B),
                                                Hue = ColorFormulas.HueDistance(color.Key.ToColor(), colorCompare.Key.ToColor()),
                                                Percentage = ((float)colorCompare.Value / (float)totalPixelCount) * 100
                                            };
                    result.ResultColors.Add(resultCompare);
                }

                GenerateSimilarColorsList(result, deltaECut, hueCut, rgbCut, percentageCut);
                results.Add(result);
            }

            return results.OrderByDescending(c => c.Percentage).ToList();
        }

        /// <summary>
        /// The generate similar colors list.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="deltaECut">
        /// The delta e cut.
        /// </param>
        /// <param name="hueCut">
        /// The hue cut.
        /// </param>
        /// <param name="rgbCut">
        /// The rgb cut.
        /// </param>
        /// <param name="percentageCut">
        /// The percentage cut.
        /// </param>
        private static void GenerateSimilarColorsList(ColorCompareData source, float deltaECut, float hueCut, float rgbCut, float percentageCut)
        {
            source.ParentColorDeltaE = source.ResultColors.Where(c => c.DeltaE < deltaECut).OrderBy(c => c.DeltaE)
                .Select(c => new SimilarColor() { Color = c.ColorCompared, DeltaE = (float)c.DeltaE, RgbDistance = (float)c.DistanceRGB, Hue = (float)c.Hue }).ToList();
            source.ParentColorHue = source.ResultColors.Where(c => c.Hue < hueCut).OrderBy(c => c.Hue)
                .Select(c => new SimilarColor() { Color = c.ColorCompared, Hue = (float)c.Hue, RgbDistance = (float)c.DistanceRGB, DeltaE = (float)c.DeltaE }).ToList();
            source.ParentColorRgb = source.ResultColors.Where(c => c.DistanceRGB < rgbCut).OrderBy(c => c.DistanceRGB)
                .Select(c => new SimilarColor() { Color = c.ColorCompared, RgbDistance = (float)c.DistanceRGB, Hue = (float)c.Hue, DeltaE = (float)c.DeltaE }).ToList();
            var index = 0;
            source.ParentColor = new List<SimilarColor>();
            foreach (var deltaColor in source.ParentColorDeltaE)
            {
                Color? rgbColor = null;
                if (index < source.ParentColorRgb.Count)
                {
                    rgbColor = source.ParentColorRgb[index].Color;
                }

                if (rgbColor.HasValue)
                {
                    if (deltaColor.Color == rgbColor.Value)
                    {
                        source.ParentColor.Add(deltaColor);
                    }
                }
                else
                {
                    source.ParentColor.Add(deltaColor);
                }
                
                index++;
            }

            foreach (var hueColor in source.ParentColorHue)
            {
                var possibleHueToRemove =
                    source.ResultColors.FirstOrDefault(c => c.ColorCompared.Name == hueColor.Color.Name);
                if ((possibleHueToRemove.Percentage - source.Percentage) > percentageCut)
                {
                    continue;
                }

                var existColor = source.ParentColor.FirstOrDefault(c => c.Color == hueColor.Color);
                if (existColor != null)
                {
                    existColor.Hue = (float)hueColor.Hue;
                    continue;
                }

                // if (parentColorDeltaE.Any() || parentColorRgb.Any())
                // {
                source.ParentColor.Add(hueColor);

                // }
            }
        }

        /// <summary>
        /// The delete similar colors.
        /// </summary>
        /// <param name="imageHelper">
        /// The image helper.
        /// </param>
        /// <param name="colorHelper">
        /// The color helper.
        /// </param>
        /// <param name="colorData">
        /// The color data.
        /// </param>
        /// <param name="deltaECutPassSD">
        /// The delta e cut pass sd.
        /// </param>
        /// <param name="hueCutPassSD">
        /// The hue cut pass sd.
        /// </param>
        /// <param name="rgbCutPassSD">
        /// The rgb cut pass sd.
        /// </param>
        /// <param name="percentageCutPassSD">
        /// The percentage cut pass sd.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<ColorCompareData> DeleteSimilarColors(IImageHelper imageHelper, IColorHelper colorHelper, List<ColorCompareData> colorData, float deltaECutPassSD, float hueCutPassSD, float rgbCutPassSD, float percentageCutPassSD)
        {
            var returnList = new List<ColorCompareData>(colorData);
            var listColorsDelete = new Dictionary<Color, Color?>();
            foreach (var result in colorData)
            {
                var approximateColor = colorHelper.GetApproximateColorName(result.OriginalColor);
                if (result.ParentColor.Any())
                {
                    foreach (var parentColor in result.ParentColor)
                    {
                        var parentColorData = colorData.FirstOrDefault(c => parentColor.Color == c.OriginalColor);
                        if (result.PassStdDev)
                        {
                            if (!parentColorData.PassStdDev)
                            {
                                if (listColorsDelete.Keys.Contains(parentColor.Color))
                                {
                                }
                                else
                                {
                                    listColorsDelete.Add(parentColor.Color, result.OriginalColor);
                                }
                            }
                            else
                            {
                                if ((result.OriginalColor.Name == "ffffffff" || result.OriginalColor.Name == "ff000000"
                                                                         || parentColor.Color.Name == "ffffffff"
                                                                         || parentColor.Color.Name == "ff000000" 
                                                                         || approximateColor.Name.ToLower() == "black"
                                                                         || approximateColor.Name.ToLower() == "white") 
                                    && (colorData.Count > 2)) 
                                                                         
                                {
                                    if ((parentColor.DeltaE <= deltaECutPassSD || parentColor.RgbDistance <= rgbCutPassSD) && result.PixelCount != parentColorData.PixelCount)
                                    {
                                        if (Math.Abs(parentColorData.Percentage - result.Percentage) < 1.5 && parentColorData.Percentage < 2 && result.Percentage < 2 )
                                        {
                                            if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                            {
                                                listColorsDelete.Add(parentColor.Color, null);
                                            }

                                            if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                            {
                                                listColorsDelete.Add(result.OriginalColor, null);
                                            }
                                            
                                        }
                                        else
                                        {
                                            if (result.PixelCount > parentColorData.PixelCount)
                                            {
                                                if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                                {
                                                    listColorsDelete.Add(parentColor.Color, result.OriginalColor);
                                                }
                                            }
                                            else
                                            {
                                                if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                                {
                                                    listColorsDelete.Add(result.OriginalColor, parentColor.Color);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ((parentColor.DeltaE <= deltaECutPassSD || parentColor.RgbDistance <= rgbCutPassSD || parentColor.Hue <= hueCutPassSD) && result.PixelCount != parentColorData.PixelCount)
                                {
                                    if (Math.Abs(parentColorData.Percentage - result.Percentage) < 1.5)
                                    {
                                        var colorNames = result.ParentColor.Select(c => c.Color.Name.ToLower()).ToList();
                                        colorNames.AddRange(result.ParentColor.Select(c => colorHelper.GetApproximateColorName(c.Color).Name.ToLower()).ToList());
                                        if (colorNames.Contains("ffffffff") || colorNames.Contains("ff000000") 
                                            || colorNames.Contains("white") || colorNames.Contains("black"))
                                        {
                                            if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                            {
                                                listColorsDelete.Add(parentColor.Color, null);
                                            }

                                            if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                            {
                                                listColorsDelete.Add(result.OriginalColor, null);
                                            }
                                        }
                                        else if (parentColorData.Percentage < 2 && result.Percentage < 2)
                                        {
                                            if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                            {
                                                listColorsDelete.Add(parentColor.Color, null);
                                            }

                                            if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                            {
                                                listColorsDelete.Add(result.OriginalColor, null);
                                            }
                                        }
                                        else
                                        {
                                            if (result.PixelCount > parentColorData.PixelCount)
                                            {
                                                if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                                {
                                                    listColorsDelete.Add(parentColor.Color, result.OriginalColor);
                                                }
                                            }
                                            else
                                            {
                                                if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                                {
                                                    listColorsDelete.Add(result.OriginalColor, parentColor.Color);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (result.PixelCount > parentColorData.PixelCount)
                                        {
                                            if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                            {
                                                listColorsDelete.Add(parentColor.Color, result.OriginalColor);
                                            }
                                        }
                                        else
                                        {
                                            if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                            {
                                                listColorsDelete.Add(result.OriginalColor, parentColor.Color);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (parentColorData.PassStdDev)
                            {
                                if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                {
                                    listColorsDelete.Add(result.OriginalColor, parentColor.Color);
                                }
                            }
                            else
                            {
                                if (result.PixelCount > parentColorData.PixelCount)
                                {
                                    if (!listColorsDelete.Keys.Contains(parentColor.Color))
                                    {
                                        listColorsDelete.Add(parentColor.Color, result.OriginalColor);
                                    }
                                }
                                else
                                {
                                    if (!listColorsDelete.Keys.Contains(result.OriginalColor))
                                    {
                                        listColorsDelete.Add(result.OriginalColor, parentColor.Color);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            foreach (var color in listColorsDelete)
            {
                colorData.FirstOrDefault(c => c.OriginalColor == color.Key).Primary = false;
                colorData.FirstOrDefault(c => c.OriginalColor == color.Key).PredominantColor = color.Value.HasValue ? color.Value : null;
            }

            return returnList;
        }
    }
}