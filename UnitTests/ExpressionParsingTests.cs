using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace UnitTests;

[TestClass]
public class ExpressionParsingTests
{
    static void ValidateParse(string input, BaseExpression expectedParse)
    {
        var actual = MyExpressionParser.Parse(null, input);
        Assert.AreEqual(expectedParse, actual, $"\nFrom input '{input}'\nexpected:\n{expectedParse}\nactual:\n{actual}");
    }

    [TestMethod]
    public void TestExpressionParsing()
    {
        ValidateParse("1", new NumberExpression(1));
        ValidateParse("-1", new BinaryOpExpression(new NumberExpression(-1), BinaryOp.Multiply, new NumberExpression(1)));
        ValidateParse("1+2", new BinaryOpExpression(new NumberExpression(1), BinaryOp.Add, new NumberExpression(2)));
        ValidateParse("1-2", new BinaryOpExpression(new NumberExpression(1), BinaryOp.Substract, new NumberExpression(2)));
        ValidateParse("1*2", new BinaryOpExpression(new NumberExpression(1), BinaryOp.Multiply, new NumberExpression(2)));
        ValidateParse("1/2", new BinaryOpExpression(new NumberExpression(1), BinaryOp.Divide, new NumberExpression(2)));
        ValidateParse("1+2/3", new BinaryOpExpression(new NumberExpression(1), BinaryOp.Add, new BinaryOpExpression(new NumberExpression(2), BinaryOp.Divide, new NumberExpression(3))));
        ValidateParse("(1+2)/3", new BinaryOpExpression(new BinaryOpExpression(new NumberExpression(1), BinaryOp.Add, new NumberExpression(2)), BinaryOp.Divide, new NumberExpression(3)));
        ValidateParse("SUM(1)", new SumExpression(new NumberExpression(1)));
    }
}
