﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

abstract record ExpressionType
{
}

sealed record NumberType : ExpressionType
{
    public static NumberType Instance { get; } = new();
    public static ArrayType ArrayInstance { get; } = new(Instance);
}

sealed record BoolType : ExpressionType
{
    public static BoolType Instance { get; } = new();
}

record StringType : ExpressionType
{
    public static StringType Instance { get; } = new();
}

sealed record FormType(FormDefinition Form) : ExpressionType, IHasFieldTypes
{
    ExpressionType IHasFieldTypes.GetFieldType(TypecheckEnvironment env, string fieldName)
    {
        LineDefinition lineDef;
        try
        {
            if (fieldName.StartsWith("Line"))
                lineDef = Form.LinesByNumber[fieldName.Substring(4)];
            else
                lineDef = Form.Lines[fieldName];
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"Could not find field '{fieldName}' on form '{Form.Name}'.");
        }

        if (lineDef.Calc is object)
        {
            env.PushRecursionCheck(Form, lineDef);
            //ignore result, we are only doing this for the side effect of the recursion check
            lineDef.Calc.CheckType(env);
            env.PopRecursionCheck();
        }

        return lineDef.Type;
    }
}

sealed record ArrayType(ExpressionType ElementType) : ExpressionType, IHasFieldTypes
{
    ExpressionType IHasFieldTypes.GetFieldType(TypecheckEnvironment env, string fieldName)
    {
        var elementFields = ElementType as IHasFieldTypes;
        if (elementFields is null)
            throw new NotSupportedException($"Array element type {ElementType} does not support fields.");
        return new ArrayType(elementFields.GetFieldType(env, fieldName));
    }
}

sealed record EnumNameType(Type ClrType) : ExpressionType, IHasFieldTypes
{
    ExpressionType IHasFieldTypes.GetFieldType(TypecheckEnvironment env, string fieldName)
    {
        Enum.Parse(ClrType, fieldName);
        return new EnumElementType(ClrType);
    }
}

sealed record EnumElementType(Type ClrType) : ExpressionType
{
    public static EnumElementType Form8949Code = new(typeof(Form8949Code));
}

sealed record Form8949LineType() : ExpressionType, IHasFieldTypes
{
    public static Form8949LineType Instance { get; } = new();
    public static ArrayType ArrayInstance { get; } = new(Instance);

    ExpressionType IHasFieldTypes.GetFieldType(TypecheckEnvironment env, string fieldName)
    {
        switch (fieldName)
        {
            case nameof(Form8949Line.CostBasis):
                return NumberType.Instance;
            case nameof(Form8949Line.SalePrice):
                return NumberType.Instance;
            case nameof(Form8949Line.Adjustment):
                return NumberType.Instance;
            case nameof(Form8949Line.Description):
            case nameof(Form8949Line.Acquired):
            case nameof(Form8949Line.Sold):
                throw new NotImplementedException($"Support for field named {fieldName} not yet implemented.");
            default:
                throw new Exception("Unknown field name: " + fieldName);
        }
    }
}