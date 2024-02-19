using System;

namespace TaxStuff.ExpressionEvaluation;

class TypecheckException : Exception
{
    public TypecheckException(string message)
        : base(message)
    {
    }
}
