using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TaxTest.FormModel
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

        public static string OptionalAttributeValue(this XElement el, string attributeName)
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

        public static bool? GetOptionalBoolAttributeValue(this XElement el, string attributeName)
        {
            var attr = el.Attribute(attributeName);
            if (attr is null)
                return null;
            return bool.Parse(attr.Value);
        }
    }
}
