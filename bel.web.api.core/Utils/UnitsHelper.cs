// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitsHelper.cs" company="BEL USA">
//   This is product property of BEL USA.
// </copyright>
// <summary>
//   Defines the UnitsHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace bel.web.api.core.Utils
{
    using System;

    /// <summary>The units helper.</summary>
    public class UnitsHelper
    {
        /// <summary>The point conversion factor to point.</summary>
        private const float PointConversionFactorToPoint = 1;

        /// <summary>The millimeter conversion factor to point.</summary>
        private const float MillimeterConversionFactorToPoint = 2.83465f;

        /// <summary>The inch conversion factor to point.</summary>
        private const float InchConversionFactorToPoint = 72;

        /// <summary>The pixels conversion factor to point.</summary>
        private const float PixelsConversionFactorToPoint = .07f;

        /// <summary>The point conversion factor to inch.</summary>
        private const float PointConversionFactorToInch = 0.0138889f;

        /// <summary>The millimeter conversion factor to inch.</summary>
        private const float MillimeterConversionFactorToInch = 0.0393701f;

        /// <summary>The inch conversion factor to inch.</summary>
        private const float InchConversionFactorToInch = 1;

        /// <summary>The to points.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="float"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public float ToPoints(float value, UnitType type)
        {
            switch (type)
            {
                case UnitType.Points:
                    return value * PointConversionFactorToPoint;
                case UnitType.Inch:
                    return value * InchConversionFactorToPoint;
                case UnitType.Milimeters:
                    return value * MillimeterConversionFactorToPoint;
                case UnitType.Pixels:
                    return value * PixelsConversionFactorToPoint;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>The to inch.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="float"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public float ToInch(float value, UnitType type)
        {
            switch (type)
            {
                case UnitType.Points:
                    return value * PointConversionFactorToInch;
                case UnitType.Inch:
                    return value * InchConversionFactorToInch;
                case UnitType.Milimeters:
                    return value * MillimeterConversionFactorToInch;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    /// <summary>The unit type.</summary>
    public enum UnitType
    {
        /// <summary>The points.</summary>
        Points = 1,

        /// <summary>The inch.</summary>
        Inch = 2,

        /// <summary>The milimeters.</summary>
        Milimeters = 3,

        /// <summary>The pixels.</summary>
        Pixels = 4
    }
}
