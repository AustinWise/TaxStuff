using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionParsing;

/// <summary>
/// For more complex expressions, an XML representation can be used.
/// </summary>
static class XmlExpressionParser
{
    public static BaseExpression Parse(ParsingEnvironment env, XElement node)
    {
        return node.Name.LocalName switch
        {
            "ValueFromFirstFormThatExists" => new ValueFromFirstFormThatExistsExpression(env, node),
            "SelectBasedOnStatus" => new SelectBasedOnStatusExpression(env, node),
            "Form8949Lines" => new Form8949LinesLiteralExpression(node),
            "ExcessSocialSecurityTaxWithheld" => new ExcessSocialSecurityTaxWithheldExpression(node),
            _ => throw new FileLoadException(node, "Unknown node type: " + node.Name),
        };
    }
}
