using System;
using System.Collections.Generic;

namespace TaxTest.ExpressionEvaluation
{
    static class FunctionFactory
    {
        public static BaseExpression CreateFunction(string functionName, List<BaseExpression> arguments)
        {
            switch (functionName.ToLowerInvariant())
            {
                case "sum":
                    if (arguments.Count != 1)
                        throw new Exception("Expected 1 argument for SUM, found " + arguments.Count);
                    return new SumExpression(arguments[0]);
                default:
                    throw new Exception("Unknown function '" + functionName + "'.");
            }
        }
    }
}
