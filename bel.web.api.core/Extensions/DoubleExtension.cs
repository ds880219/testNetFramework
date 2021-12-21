namespace bel.web.api.core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DoubleExtension
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            if (values == null)
            {
                return 0;
            }

            var avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

        }
    }
}
