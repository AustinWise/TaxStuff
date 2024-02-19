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
        var rightNumber = rhs as NumberResult;
        if (rightNumber is null)
            throw new Exception("Expected number.");
        switch (op)
        {
            case BinaryOp.Add:
                return new NumberResult(Value + rightNumber.Value);
            case BinaryOp.Substract:
                return new NumberResult(Value - rightNumber.Value);
            case BinaryOp.Multiply:
                return new NumberResult(Value * rightNumber.Value);
            case BinaryOp.Divide:
                return new NumberResult(Value / rightNumber.Value);
            case BinaryOp.Equal:
                return new BoolResult(Value == rightNumber.Value);
            case BinaryOp.NotEqual:
                return new BoolResult(Value != rightNumber.Value);
            case BinaryOp.LessThan:
                return new BoolResult(Value < rightNumber.Value);
            case BinaryOp.GreaterThan:
                return new BoolResult(Value > rightNumber.Value);
            case BinaryOp.LessThanOrEqual:
                return new BoolResult(Value <= rightNumber.Value);
            case BinaryOp.GreaterThanOrEqual:
                return new BoolResult(Value >= rightNumber.Value);
            default:
                throw new NotSupportedException("Unsupported binop: " + op);
        }
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
        var rightBool = rhs as BoolResult;
        if (rightBool is null)
            throw new Exception("Expected bool.");
        switch (op)
        {
            case BinaryOp.Equal:
                return new BoolResult(Value == rightBool.Value);
            case BinaryOp.NotEqual:
                return new BoolResult(Value != rightBool.Value);
            default:
                throw new NotSupportedException("Unsupported binop: " + op);
        }
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
        : this(values.ToList())
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
        var right = rhs as Form8949EnumElementResult;
        if (right is null)
            throw new Exception("Expected Form8949EnumElementResult.");
        switch (op)
        {
            case BinaryOp.Equal:
                return new BoolResult(this.Code == right.Code);
            case BinaryOp.NotEqual:
                return new BoolResult(this.Code != right.Code);
            default:
                throw new NotSupportedException("Unsupported binop: " + op);
        }
    }
}

record Form8949LineResult(Form8949Line Line) : EvaluationResult
{
    public override EvaluationResult GetFieldValue(EvaluationEnvironment env, string fieldName)
    {
        switch (fieldName)
        {
            case nameof(Form8949Line.CostBasis):
                return new NumberResult(Line.CostBasis);
            case nameof(Form8949Line.SalePrice):
                return new NumberResult(Line.SalePrice);
            case nameof(Form8949Line.Adjustment):
                return new NumberResult(Line.Adjustment);
            case nameof(Form8949Line.Description):
            case nameof(Form8949Line.Acquired):
            case nameof(Form8949Line.Sold):
                throw new NotImplementedException($"Support for field named {fieldName} not yet implemented.");
            default:
                throw new Exception("Unknown field name: " + fieldName);
        }
    }
}
