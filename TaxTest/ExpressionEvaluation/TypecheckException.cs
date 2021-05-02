using System;

namespace TaxTest.ExpressionEvaluation
{
    class TypecheckException : Exception
    {
        public TypecheckException(string message)
            : base(message)
        {
        }
    }
}
