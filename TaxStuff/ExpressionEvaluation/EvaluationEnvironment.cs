using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

record EvaluationEnvironment(TaxReturn Return, FormInstance CurrentForm)
{
    public TaxRates Rates => Return.TaxYearDef.Rates;
}
