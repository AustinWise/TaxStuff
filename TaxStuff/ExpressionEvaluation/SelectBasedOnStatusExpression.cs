using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionParsing;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

record SelectBasedOnStatusExpression(ReadOnlyDictionary<FilingStatus, BaseExpression> Values) : BaseExpression
{
    public SelectBasedOnStatusExpression(ParsingEnvironment env, XElement node)
        : this(new ReadOnlyDictionary<FilingStatus, BaseExpression>(node.Elements("Choice").ToDictionary(n => n.EnumAttributeValue<FilingStatus>("Status"), n => n.ExpressionAttributeValue(env, "ValueExpr"))))
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
