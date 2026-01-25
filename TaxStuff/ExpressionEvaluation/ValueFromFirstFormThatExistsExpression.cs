using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionParsing;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

record ValueFromFirstFormThatExistsExpression(ReadOnlyCollection<(string, BaseExpression)> Forms, decimal? ValueIfNoMatchingForm) : BaseExpression
{
    static (string, BaseExpression) ParseNode(ParsingEnvironment env, XElement node)
    {
        string formName = node.AttributeValue("Name");
        var expr = node.ExpressionAttributeValue(env, "ValueExpr");
        return (formName, expr);
    }

    public ValueFromFirstFormThatExistsExpression(ParsingEnvironment env, XElement node)
        : this(new ReadOnlyCollection<(string, BaseExpression)>([.. node.Elements("Form").Select(n => ParseNode(env, n))]), node.OptionalDecimalAttributeValue("ValueIfNoMatchingForm"))
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
        if (ValueIfNoMatchingForm.HasValue)
        {
            return EvaluationResult.CreateNumber(ValueIfNoMatchingForm.Value);
        }
        throw new Exception("Could not find any forms that matched.");
    }
}
