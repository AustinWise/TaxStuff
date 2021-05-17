using System;
using System.Collections.Generic;

namespace TaxStuff.ExpressionEvaluation
{
    enum BinaryOp
    {
        Add,
        Substract,
        Multiply,
        Divide,
        Equal,
        NotEqual,
    }

    record BinaryOpExpression(BaseExpression Left, BinaryOp Operation, BaseExpression Right) : BaseExpression
    {
        private static readonly Dictionary<BinaryOp, ExpressionType[]> ValidTypes = new()
        {
            { BinaryOp.Add, new ExpressionType[] { NumberType.Instance } },
            { BinaryOp.Substract, new ExpressionType[] { NumberType.Instance } },
            { BinaryOp.Multiply, new ExpressionType[] { NumberType.Instance } },
            { BinaryOp.Divide, new ExpressionType[] { NumberType.Instance } },
            { BinaryOp.Equal, new ExpressionType[] { NumberType.Instance, BoolType.Instance, EnumElementType.Form8949Code } },
            { BinaryOp.NotEqual, new ExpressionType[] { NumberType.Instance, BoolType.Instance, EnumElementType.Form8949Code } },
        };

        private static readonly Dictionary<BinaryOp, ExpressionType> ResultTypes = new()
        {
            { BinaryOp.Add, NumberType.Instance },
            { BinaryOp.Substract, NumberType.Instance },
            { BinaryOp.Multiply, NumberType.Instance },
            { BinaryOp.Divide, NumberType.Instance },
            { BinaryOp.Equal, BoolType.Instance },
            { BinaryOp.NotEqual, BoolType.Instance },
        };

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            var left = Left.Evaluate(env);
            var right = Right.Evaluate(env);

            return left.PerformBinOp(Operation, right);
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            var lhs = Left.CheckType(env);
            var rhs = Right.CheckType(env);
            if (lhs != rhs)
                throw new Exception($"Left type {lhs} does not match right type {rhs}.");
            foreach (var valid in ValidTypes[Operation])
            {
                if (lhs == valid)
                    return ResultTypes[Operation];
            }
            throw new Exception($"Type {lhs} is not valid for operation {Operation}.");
        }
    }
}
