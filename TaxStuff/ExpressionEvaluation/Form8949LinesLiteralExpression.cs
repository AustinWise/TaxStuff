using System.Collections.Generic;
using System.Xml.Linq;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

record Form8949LinesLiteralExpression(List<Form8949LineResult> Lines) : BaseExpression
{
    static List<Form8949LineResult> Parse(XElement node)
    {
        var ret = new List<Form8949LineResult>();
        foreach (var el in node.Elements("Line"))
        {
            var line = new Form8949Line(
                el.AttributeValue(nameof(Form8949Line.Description)),
                el.OptionalDateAttributeValue(nameof(Form8949Line.Acquired)),
                el.DateAttributeValue(nameof(Form8949Line.Sold)),
                el.DecimalAttributeValue(nameof(Form8949Line.CostBasis)),
                el.DecimalAttributeValue(nameof(Form8949Line.SalePrice)),
                el.OptionalDecimalAttributeValue(nameof(Form8949Line.Adjustment)) ?? 0);
            ret.Add(new Form8949LineResult(line));
        }
        return ret;
    }


    public Form8949LinesLiteralExpression(XElement node)
        : this(Parse(node))
    {
    }

    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        return Form8949LineType.ArrayInstance;
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        return new ArrayResult(Lines);
    }
}
