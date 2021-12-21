using bel.web.api.core.objects.Enums;
using bel.web.api.core.objects.ImageEffects;

namespace bel.web.api.core.PrintMethod
{
    public class PrintMethodHelper
    {
        public static PrintMethodName GetPrintMethodName(int PrintMethodID)
        {
            if (PrintMethodID != 0)
            {
                return (PrintMethodName)PrintMethodID;
            }

            return PrintMethodName.Default;
        }

        public static Effect GetEffectMapping(int PrintMethodID)
        {
            if (PrintMethodID != 0)
            {
                return Effect.None;
            }

            var pmn = GetPrintMethodName(PrintMethodID);
            switch (pmn)
            {
                case PrintMethodName.Laser:
                {
                    return Effect.laser_engraved;
                }

                case PrintMethodName.Embroidery:
                {
                    return Effect.Embroidery;
                }

                default:
                {
                    return Effect.None;
                }
            }
        }

        public static Effect GetEffectMapping(PrintMethodName printMethodName)
        {
            if (printMethodName == PrintMethodName.Default)
            {
                return Effect.None;
            }

            switch (printMethodName)
            {
                case PrintMethodName.Laser:
                {
                    return Effect.laser_engraved;
                }
                case PrintMethodName.Embroidery:
                {
                    return Effect.Embroidery;
                }
                case PrintMethodName.Debossing:
                {
                    return Effect.Debossing;
                }
                default:
                {
                    return Effect.None;
                }
            }
        }
    }
}
