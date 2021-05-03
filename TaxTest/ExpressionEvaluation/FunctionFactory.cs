using System;
using System.Collections.Generic;

namespace TaxTest.ExpressionEvaluation
{
    static class FunctionFactory
    {
        static void CheckArgCount(string functionName, List<BaseExpression> arguments, int expected)
        {
            if (arguments.Count != expected)
                throw new Exception($"Expected {expected} argument for {functionName}, found " + arguments.Count);
        }

        public static BaseExpression CreateFunction(string functionName, List<BaseExpression> arguments)
        {
            switch (functionName.ToLowerInvariant())
            {
                case "sum":
                    CheckArgCount(functionName, arguments, 1);
                    return new SumExpression(arguments[0]);
                case "max":
                    CheckArgCount(functionName, arguments, 2);
                    return new MaxExpression(arguments[0], arguments[1]);
                case "min":
                    CheckArgCount(functionName, arguments, 2);
                    return new MinExpression(arguments[0], arguments[1]);
                default:
                    throw new Exception("Unknown function '" + functionName + "'.");
            }
        }
    }
}
