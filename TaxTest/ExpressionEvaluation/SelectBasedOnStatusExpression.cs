using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    record SelectBasedOnStatusExpression(ReadOnlyDictionary<FilingStatus, BaseExpression> Values) : BaseExpression
    {
        public SelectBasedOnStatusExpression(XElement node)
            : this(new ReadOnlyDictionary<FilingStatus, BaseExpression>(node.Elements("Choice").ToDictionary(n => n.EnumAttributeValue<FilingStatus>("Status"), n => n.ExpressionAttributeValue("ValueExpr"))))
        {
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            foreach (var expr in Values.Values)
            {
                expr.ValidateExpressionType(env, NumberType.Instance);
            }
            return NumberType.Instance;
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return Values[env.Return.Status].Evaluate(env);
        }
    }
}
