using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaxTest.ExpressionEvaluation
{
    record FunctionExpression(string Name, ReadOnlyCollection<BaseExpression> Arguments, Delegate ClrFunction) : BaseExpression
    {
        public FunctionExpression(string name, IList<BaseExpression> arguments, Delegate clrFunction)
            : this(name, new ReadOnlyCollection<BaseExpression>(arguments), clrFunction)
        {
            var parameters = clrFunction.Method.GetParameters();
            if (parameters.Length != arguments.Count)
                throw new Exception($"Expected {parameters.Length} arguments for function '{name}', found {arguments.Count}.");
            throw new NotImplementedException();
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            throw new NotImplementedException();
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            throw new NotImplementedException();
        }
    }
}
