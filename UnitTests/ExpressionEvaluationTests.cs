using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace UnitTests;

public class ExpressionEvaluationTests
{

    [Fact]
    public void TestArithmeticExpressions()
    {
        Test("1 + 2", 3m);
        Test("3 * 4", 12m);
        Test("1 + 3 * 4", 13m);
        Test("3 - 4", -1m);

        static void Test(string expression, decimal expected)
        {
            var expr = MyExpressionParser.Parse(null!, expression);
            var result = expr.Evaluate(new EvaluationEnvironment(null!, null!));
            Assert.IsType<NumberResult>(result);
            decimal actual = ((NumberResult)result).Value;
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void TestLogicalExpressions()
    {
        Test("1 < 2", true);
        Test("1 > 2", false);
        Test("1 <= 2", true);
        Test("1 >= 2", false);

        Test("1 < 1", false);
        Test("1 <= 1", true);

        Test("1 > 1", false);
        Test("1 >= 1", true);

        static void Test(string expression, bool expected)
        {
            var expr = MyExpressionParser.Parse(null!, expression);
            var result = expr.Evaluate(new EvaluationEnvironment(null!, null!));
            Assert.IsType<BoolResult>(result);
            bool actual = ((BoolResult)result).Value;
            Assert.Equal(expected, actual);
        }
    }
}
