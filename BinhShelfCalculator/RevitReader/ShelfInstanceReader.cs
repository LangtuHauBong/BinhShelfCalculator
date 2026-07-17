using Autodesk.Revit.DB;
using BinhShelfCalculator.Models;
using System;
using System.Globalization;

namespace BinhShelfCalculator.RevitReader
{
    public static class ShelfInstanceReader
    {
        public static ShelfInstanceInfo Read(Document doc, Element element)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (element == null) throw new ArgumentNullException(nameof(element));

            double lengthMm = ReadRequiredParameterMm(element, ShelfParameterValidator.ParameterLength);
            double widthMm = ReadRequiredParameterMm(element, ShelfParameterValidator.ParameterWidth);
            double heightMm = ReadRequiredParameterMm(element, ShelfParameterValidator.ParameterHeight);

            string familyName = "";
            string typeName = "";
            Element typeElement = doc.GetElement(element.GetTypeId());
            if (typeElement != null)
            {
                typeName = typeElement.Name;
                FamilySymbol symbol = typeElement as FamilySymbol;
                if (symbol != null && symbol.Family != null)
                {
                    familyName = symbol.Family.Name;
                }
            }

            return new ShelfInstanceInfo
            {
                ElementId = element.Id,
                FamilyName = familyName,
                TypeName = typeName,
                LengthMm = lengthMm,
                WidthMm = widthMm,
                HeightMm = heightMm
            };
        }

        private static double ReadRequiredParameterMm(Element element, string parameterName)
        {
            Parameter parameter = element.LookupParameter(parameterName);
            if (parameter == null)
            {
                throw new Exception("Đối tượng được chọn thiếu tham số bắt buộc: " + parameterName);
            }

            if (parameter.StorageType == StorageType.Double)
            {
                if (parameter.Definition != null && parameter.Definition.ParameterType == ParameterType.Length)
                {
                    return UnitUtils.ConvertFromInternalUnits(
                        parameter.AsDouble(),
                        DisplayUnitType.DUT_MILLIMETERS);
                }

                return parameter.AsDouble();
            }

            if (parameter.StorageType == StorageType.Integer)
            {
                return parameter.AsInteger();
            }

            if (parameter.StorageType == StorageType.String)
            {
                return ParseDouble(parameter.AsString(), parameterName);
            }

            throw new Exception("Tham số " + parameterName + " không phải dạng số đọc được.");
        }

        private static double ParseDouble(string text, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new Exception("Tham số " + parameterName + " đang trống.");
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

            throw new Exception("Không đọc được giá trị số của tham số " + parameterName + ": " + text);
        }
    }
}
