using System;
using System.Globalization;

namespace BinhShelfCalculator.Utils
{
    public static class NumberParser
    {
        public static double ToDouble(string text, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new Exception(fieldName + " đang trống.");
            }

            string value = text.Trim().Replace("mm", "").Replace("MM", "").Trim();
            double result;

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            value = value.Replace(',', '.');
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            throw new Exception("Không đọc được số ở ô: " + fieldName + " = " + text);
        }

        public static int ToInt(string text, string fieldName)
        {
            double value = ToDouble(text, fieldName);
            return (int)Math.Round(value);
        }
    }
}
