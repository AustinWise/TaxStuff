using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

abstract record EvaluationResult
{
    public static NumberResult Zero { get; } = new NumberResult(decimal.Zero);

    public static EvaluationResult CreateNumber(decimal value) => new NumberResult(value);

    public virtual EvaluationResult PerformBinOp(BinaryOp op, EvaluationResult rhs)
    {
        throw new NotSupportedException();
    }

    public virtual EvaluationResult GetFieldValue(EvaluationEnvironment env, string fieldName)
    {
        throw new NotSupportedException();
    }

    public virtual decimal AsNumber() => throw new NotSupportedException();
}

record NumberResult(decimal Value) : EvaluationResult
{
    public override EvaluationResult PerformBinOp(BinaryOp op, EvaluationResult rhs)
    {
        var rightNumber = rhs as NumberResult ?? throw new Exception("Expected number.");
        return op switch
        {
            BinaryOp.Add => new NumberResult(Value + rightNumber.Value),
            BinaryOp.Substract => new NumberResult(Value - rightNumber.Value),
            BinaryOp.Multiply => new NumberResult(Value * rightNumber.Value),
            BinaryOp.Divide => new NumberResult(Value / rightNumber.Value),
            BinaryOp.Equal => new BoolResult(Value == rightNumber.Value),
            BinaryOp.NotEqual => new BoolResult(Value != rightNumber.Value),
            BinaryOp.LessThan => new BoolResult(Value < rightNumber.Value),
            BinaryOp.GreaterThan => new BoolResult(Value > rightNumber.Value),
            BinaryOp.LessThanOrEqual => new BoolResult(Value <= rightNumber.Value),
            BinaryOp.GreaterThanOrEqual => new BoolResult(Value >= rightNumber.Value),
            _ => throw new NotSupportedException("Unsupported binop: " + op),
        };
    }

    public override decimal AsNumber() => Value;

    protected override bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("{0:c}", Value);
        return true;
    }
}

record BoolResult(bool Value) : EvaluationResult
{
    public override EvaluationResult PerformBinOp(BinaryOp op, EvaluationResult rhs)
    {
        var rightBool = rhs as BoolResult ?? throw new Exception("Expected bool.");
        return op switch
        {
            BinaryOp.Equal => new BoolResult(Value == rightBool.Value),
            BinaryOp.NotEqual => new BoolResult(Value != rightBool.Value),
            _ => throw new NotSupportedException("Unsupported binop: " + op),
        };
    }

    protected override bool PrintMembers(StringBuilder builder)
    {
        builder.Append(Value);
        return true;
    }
}

record StringResult(string Value) : EvaluationResult
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("\"{0}\"", Value);
        return true;
    }
}

record ArrayResult(List<EvaluationResult> Values) : EvaluationResult
{
    public ArrayResult(IEnumerable<EvaluationResult> values)
        : this([.. values])
    {
    }

    public override EvaluationResult GetFieldValue(EvaluationEnvironment env, string fieldName)
    {
        var ret = new List<EvaluationResult>();
        foreach (var val in Values)
        {
            ret.Add(val.GetFieldValue(env, fieldName));
        }
        return new ArrayResult(ret);
    }
}

record FormResult(FormDefinition Def, IHasFieldEvaluation Value) : EvaluationResult
{
    public override EvaluationResult GetFieldValue(EvaluationEnvironment env, string fieldName)
    {
        return Value.EvaluateField(env, fieldName);
    }
}

record Form8949EnumNameResult() : EvaluationResult
{
    public override EvaluationResult GetFieldValue(EvaluationEnvironment env, string fieldName)
    {
        return new Form8949EnumElementResult(Enum.Parse<Form8949Code>(fieldName));
    }
}

record Form8949EnumElementResult(Form8949Code Code) : EvaluationResult
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        builder.Append(Code);
        return true;
    }

    public override EvaluationResult PerformBinOp(BinaryOp op, EvaluationResult rhs)
    {
        var right = rhs as Form8949EnumElementResult ?? throw new Exception("Expected Form8949EnumElementResult.");
        return op switch
        {
            BinaryOp.Equal => new BoolResult(this.Code == right.Code),
            BinaryOp.NotEqual => new BoolResult(this.Code != right.Code),
            _ => throw new NotSupportedException("Unsupported binop: " + op),
        };
    }
}

record Form8949LineResult(Form8949Line Line) : EvaluationResult
{
    public override EvaluationResult GetFieldValue(EvaluationEnvironment env, string fieldName)
    {
        return fieldName switch
        {
            nameof(Form8949Line.CostBasis) => new NumberResult(Line.CostBasis),
            nameof(Form8949Line.SalePrice) => new NumberResult(Line.SalePrice),
            nameof(Form8949Line.Adjustment) => new NumberResult(Line.Adjustment),
            nameof(Form8949Line.Description) or nameof(Form8949Line.Acquired) or nameof(Form8949Line.Sold) => throw new NotImplementedException($"Support for field named {fieldName} not yet implemented."),
            _ => throw new Exception("Unknown field name: " + fieldName),
        };
    }
}
