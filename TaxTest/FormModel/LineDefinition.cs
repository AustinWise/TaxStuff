using System;
using System.Xml.Linq;
using TaxTest.ExpressionEvaluation;
using TaxTest.ExpressionParsing;

namespace TaxTest.FormModel
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
            var calc = node.OptionalAttributeValue("Calc");
            if (calc is null)
                return null;
            try
            {
                return MyExpressionParser.Parse(calc);
            }
            catch (Exception ex)
            {
                //TODO: ensure that the bad expression is part of the error message
                throw new FileLoadException(node, "Failed to parse Calc.", ex);
            }
        }

        public LineDefinition(XElement el)
            : this(el.AttributeValue("Number"),
                   el.AttributeValue("Name"),
                   GetExprType(el.Attribute("Type")),
                   el.OptionalBoolAttributeValue("AllowMultiple") ?? false,
                   ParseCalc(el))
        {
            if (AllowMultiple && Calc is not null)
                throw new FileLoadException(el, "Line cannot have a Calc when AllowMultiple is true.");
        }
    }
}
