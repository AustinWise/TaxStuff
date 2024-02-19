using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace UnitTests;

[TestClass]
public class ExpressionEvaluationTests
{

    [TestMethod]
    public void TestArithmaticExpressions()
    {
        Test("1 + 2", 3m);
        Test("3 * 4", 12m);
        Test("1 + 3 * 4", 13m);
        Test("3 - 4", -1m);

        static void Test(string expression, decimal expected)
        {
            var expr = MyExpressionParser.Parse(null, expression);
            var result = expr.Evaluate(new EvaluationEnvironment(null, null));
            Assert.IsInstanceOfType<NumberResult>(result, $"Expresion '{expression}' did not evaluate to a number type.");
            decimal actual = ((NumberResult)result).Value;
            Assert.AreEqual(expected, actual, $"Expression '{expression}' did not evaluate to the expected value.");
        }
    }

    [TestMethod]
    public void TestLogicalExpressiosn()
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
            var expr = MyExpressionParser.Parse(null, expression);
            var result = expr.Evaluate(new EvaluationEnvironment(null, null));
            Assert.IsInstanceOfType<BoolResult>(result, $"Expresion '{expression}' did not evaluate to a boolean type.");
            bool actual = ((BoolResult)result).Value;
            Assert.AreEqual(expected, actual, $"Expression '{expression}' did not evaluate to the expected value.");
        }
    }
}
