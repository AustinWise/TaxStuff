#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace TaxStuff.FormModel
{
    static class Extensions
    {
        public static T CheckNameAndAdd<T>(this Dictionary<string, T> dic, XElement sourceNode, T value) where T : IHasName
        {
            string name = value.Name;
            if (string.IsNullOrEmpty(name))
                throw new FileLoadException(sourceNode, $"Missing or empty Name attribute.");
            try
            {
                dic.Add(name, value);
            }
            catch (ArgumentException)
            {
                throw new FileLoadException(sourceNode, $"Duplicate Name: " + name);
            }
            return value;
        }

        public static string? OptionalAttributeValue(this XElement el, string attributeName)
        {
            var attr = el.Attribute(attributeName);
            if (attr is null)
                return null;
            return attr.Value;
        }

        public static string AttributeValue(this XElement el, string attributeName)
        {
            var ret = OptionalAttributeValue(el, attributeName);
            if (ret is null)
                throw new FileLoadException(el, $"Missing attribute '{attributeName}'.");
            if (string.IsNullOrWhiteSpace(ret))
                throw new FileLoadException(el, $"Empty attribute '{attributeName}'.");
            return ret;
        }

        public static bool? OptionalBoolAttributeValue(this XElement el, string attributeName)
        {
            var attr = el.Attribute(attributeName);
            if (attr is null)
                return null;
            return bool.Parse(attr.Value);
        }

        public static T? OptionalEnumAttributeValue<T>(this XElement el, string attributeName) where T : struct, Enum
        {
            var attr = el.Attribute(attributeName);
            if (attr is null)
                return null;
            return Enum.Parse<T>(attr.Value);
        }

        public static T EnumAttributeValue<T>(this XElement el, string attributeName) where T : struct, Enum
        {
            return OptionalEnumAttributeValue<T>(el, attributeName) ?? throw new FileLoadException(el, "Missing attribute: " + attributeName);
        }

        public static int IntAttributeValue(this XElement el, string attributeName)
        {
            return int.Parse(AttributeValue(el, attributeName), CultureInfo.InvariantCulture);
        }

        public static decimal? OptionalDecimalAttributeValue(this XElement el, string attributeName)
        {
            var attr = el.Attribute(attributeName);
            if (attr is null)
                return null;
            return decimal.Parse(attr.Value, CultureInfo.InvariantCulture);
        }

        public static decimal DecimalAttributeValue(this XElement el, string attributeName)
        {
            return OptionalDecimalAttributeValue(el, attributeName) ?? throw new FileLoadException(el, $"Missing attribute '{attributeName}'.");
        }

        public static DateTime? OptionalDateAttributeValue(this XElement el, string attributeName)
        {
            var attr = el.Attribute(attributeName);
            if (attr is null)
                return null;
            return DateTime.ParseExact(attr.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static DateTime DateAttributeValue(this XElement el, string attributeName)
        {
            var ret = el.OptionalDateAttributeValue(attributeName);
            if (!ret.HasValue)
                throw new FileLoadException(el, $"Missing attribute '{attributeName}'.");
            return ret.Value;
        }

        public static BaseExpression ExpressionAttributeValue(this XElement el, ParsingEnvironment env, string attributeName)
        {
            XAttribute? attr = el.Attribute(attributeName);
            if (attr is null)
                throw new FileLoadException(el, $"Missing {attributeName} attribute");
            try
            {
                return MyExpressionParser.Parse(env, attr.Value);
            }
            catch (Exception ex)
            {
                throw new FileLoadException(attr, "Failed to parse ValueExpr", ex);
            }
        }
    }
}
