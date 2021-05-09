using System;

namespace TaxStuff.ExpressionEvaluation
{
    enum BinaryOp
    {
        Add,
        Substract,
        Multiply,
        Divide,
    }

    record BinaryOpExpression(BaseExpression Left, BinaryOp Operation, BaseExpression Right) : BaseExpression
    {
        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            var left = Left.Evaluate(env).Number;
            var right = Right.Evaluate(env).Number;

            decimal ret;
            switch (Operation)
            {
                case BinaryOp.Add:
                    ret = left + right;
                    break;
                case BinaryOp.Substract:
                    ret = left - right;
                    break;
                case BinaryOp.Multiply:
                    ret = left * right;
                    break;
                case BinaryOp.Divide:
                    ret = left / right;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected operation: " + Operation);
            }
            return EvaluationResult.CreateNumber(ret);
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            Left.ValidateExpressionType(env, NumberType.Instance);
            Right.ValidateExpressionType(env, NumberType.Instance);
            return NumberType.Instance;
        }
    }
}
