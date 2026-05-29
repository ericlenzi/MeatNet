using System;

namespace Meat.Application.Shared
{
    public static class MathHelper
    {
        public static double Truncate2Decimals(double value)
        {
            return Math.Truncate(100 * value) / 100;
        }
    }
}