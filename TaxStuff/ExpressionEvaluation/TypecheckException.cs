using System;

namespace TaxStuff.ExpressionEvaluation;

class TypecheckException(string message) : Exception(message)
{
}
