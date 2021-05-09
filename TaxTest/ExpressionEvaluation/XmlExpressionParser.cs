using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    /// <summary>
    /// For more complex expressions, an XML representation can be used.
    /// </summary>
    static class XmlExpressionParser
    {
        public static BaseExpression Parse(XElement calcElement)
        {
            switch (calcElement.Name.LocalName)
            {
                default:
                    throw new FileLoadException(calcElement, "Unknown node type: " + calcElement.Name);
            }
        }
    }
}
