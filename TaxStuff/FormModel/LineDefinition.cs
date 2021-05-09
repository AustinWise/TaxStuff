using System;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace TaxStuff.FormModel
{
    record LineDefinition(string Number, string Name, ExpressionType Type, bool AllowMultiple, BaseExpression Calc) : IHasName
    {
        private static ExpressionType GetExprType(XAttribute attr)
        {
            if (attr is null || attr.Value == "Number")
                return NumberType.Instance;
            // TODO: implement support for other types
            // Currently we don't have a way of looking up what types are available,
            // so we can't tell if the type if valid or not.
            // Also the ExpressionType type system is structual, while these lines reference nominal types.
            // That is, ExpressionType describes a shape while Type here is just a name.
            // So these systems are not really compatible right now, opps.
            throw new FileLoadException(attr, "Unsupported line type: " + attr.Value);
        }

        private static BaseExpression ParseCalc(XElement node)
        {
            var calcStr = node.OptionalAttributeValue("Calc");
            var calcNode = node.Elements().ToArray();

            //make sure there is at most one Calc definition
            if (calcStr is null && calcNode.Length == 0)
                return null;
            if (calcStr is object && calcNode.Length > 0)
                throw new FileLoadException(node, "Line contains both and Calc attribute and a Calc node.");

            // do some light validation
            if (calcStr is not null)
            {
                if (string.IsNullOrWhiteSpace(calcStr))
                {
                    if (calcStr is null)
                        return null;
                    throw new FileLoadException(node, "Empty expression.");
                }
            }
            else if (calcNode.Length > 1)
                throw new FileLoadException(calcNode[1], "Multiple calculation elements.");

            try
            {
                return calcStr is not null ? MyExpressionParser.Parse(calcStr) : XmlExpressionParser.Parse(calcNode[0]);
            }
            catch (Exception ex)
            {
                //TODO: ensure that the bad expression is part of the error message
                throw new FileLoadException(node, "Failed to parse Calc.", ex);
            }
        }

        public LineDefinition(XElement el)
            : this(el.AttributeValue("Number"),
                   el.OptionalAttributeValue("Name"),
                   GetExprType(el.Attribute("Type")),
                   el.OptionalBoolAttributeValue("AllowMultiple") ?? false,
                   ParseCalc(el))
        {
            if (Name is null)
                Name = "Line" + Number;
            else if (Name.StartsWith("Year") || Name.StartsWith("Form") || Name.StartsWith("Line"))
                throw new FileLoadException(el, "Line name cannot start with any of the follow: Year, Form, or Line.");

            if (AllowMultiple && Calc is not null)
                throw new FileLoadException(el, "Line cannot have a Calc when AllowMultiple is true.");
        }
    }
}
