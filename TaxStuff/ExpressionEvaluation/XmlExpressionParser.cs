using System.Xml.Linq;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    /// <summary>
    /// For more complex expressions, an XML representation can be used.
    /// </summary>
    static class XmlExpressionParser
    {
        public static BaseExpression Parse(XElement node)
        {
            switch (node.Name.LocalName)
            {
                case "ValueFromFirstFormThatExists":
                    return new ValueFromFirstFormThatExistsExpression(node);
                case "SelectBasedOnStatus":
                    return new SelectBasedOnStatusExpression(node);
                default:
                    throw new FileLoadException(node, "Unknown node type: " + node.Name);
            }
        }
    }
}
