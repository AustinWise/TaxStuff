using TaxStuff.ExpressionEvaluation;

namespace TaxStuff.FormModel;

record class AssertDefinition(string ExprStr, BaseExpression Expr, bool ExpectedValue);
