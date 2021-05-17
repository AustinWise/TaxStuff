using System;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace TaxStuff.FormModel
{
    record LineDefinition(string Number, string Name, ExpressionType Type, BaseExpression Calc) : IHasName
    {
        private static ExpressionType GetExprType(XAttribute attr)
        {
            if (attr is null || attr.Value == "Number")
                return NumberType.Instance;
            else if (attr.Value == "String")
                return StringType.Instance;
            else if (attr.Value == "Form8949Code")
                return EnumElementType.Form8949Code;
            else if (attr.Value == "Form8949Lines")
                return Form8949LineType.ArrayInstance;
            // TODO: implement support for other types
            // Currently we don't have a way of looking up what types are available,
            // so we can't tell if the type if valid or not.
            // Also the ExpressionType type system is structual, while these lines reference nominal types.
            // That is, ExpressionType describes a shape while Type here is just a name.
            // So these systems are not really compatible right now, opps.
            throw new FileLoadException(attr, "Unsupported line type: " + attr.Value);
        }

        public LineDefinition(ParsingEnvironment env, XElement el)
            : this(el.OptionalAttributeValue("Number"),
                   el.OptionalAttributeValue("Name"),
                   GetExprType(el.Attribute("Type")),
                   MyExpressionParser.Parse(env, el, "Calc"))
        {
            if (Name is null)
            {
                if (Number is null)
                    throw new FileLoadException(el, "Line is missing both Name and Number attributes.");
                Name = "Line" + Number;
            }
            else if (Name.StartsWith("Year") || Name.StartsWith("Form") || Name.StartsWith("Line"))
                throw new FileLoadException(el, "Line name cannot start with any of the follow: Year, Form, or Line.");

            if (Type != NumberType.Instance && Calc is not null)
            {
                // I'm not 100% sure this limitation is needed.
                throw new FileLoadException(el, "Line cannot have a Calc when its type is not a number.");
            }
        }
    }
}
