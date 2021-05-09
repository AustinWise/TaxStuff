using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation
{
    record ValueFromFirstFormThatExistsExpression(ReadOnlyCollection<(string, BaseExpression)> Forms) : BaseExpression
    {
        static (string, BaseExpression) ParseNode(XElement node)
        {
            return (node.AttributeValue("Name"), node.ExpressionAttributeValue("ValueExpr"));
        }

        public ValueFromFirstFormThatExistsExpression(XElement node)
            : this(new ReadOnlyCollection<(string, BaseExpression)>(node.Elements("Form").Select(ParseNode).ToList()))
        {
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            foreach (var (formName, formExpr) in Forms)
            {
                if (!env.Forms.ContainsKey(formName))
                    throw new Exception("Unknown form name: " + formName);
                formExpr.ValidateExpressionType(env, NumberType.Instance);
            }
            return NumberType.Instance;
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            foreach (var (formName, formExpr) in Forms)
            {
                if (env.Return.Forms.ContainsKey(formName))
                    return formExpr.Evaluate(env);
            }
            throw new Exception("Could not find any forms that matched.");
        }
    }
}
