using System;
using System.Xml.Linq;
using TaxTest.ExpressionEvaluation;
using TaxTest.ExpressionParsing;

namespace TaxTest.FormModel
{
    record LineDefinition(string Number, string Name, string Type, bool AllowMultiple, BaseExpression Calc) : IHasName
    {
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
                   el.OptionalAttributeValue("Type"),
                   el.GetOptionalBoolAttributeValue("AllowMultiple") ?? false,
                   ParseCalc(el))
        {
            if (AllowMultiple && Calc is not null)
                throw new FileLoadException(el, "Line cannot have a Calc when AllowMultiple is true.");
        }
    }
}
