using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionParsing
{
    /// <summary>
    /// For more complex expressions, an XML representation can be used.
    /// </summary>
    static class XmlExpressionParser
    {
        public static BaseExpression Parse(ParsingEnvironment env, XElement node)
        {
            switch (node.Name.LocalName)
            {
                case "ValueFromFirstFormThatExists":
                    return new ValueFromFirstFormThatExistsExpression(env, node);
                case "SelectBasedOnStatus":
                    return new SelectBasedOnStatusExpression(env, node);
                case "Form8949Lines":
                    return new Form8949LinesLiteralExpression(node);
                case "ExcessSocialSecurityTaxWithheld":
                    return new ExcessSocialSecurityTaxWithheldExpression(node);
                default:
                    throw new FileLoadException(node, "Unknown node type: " + node.Name);
            }
        }
    }
}
