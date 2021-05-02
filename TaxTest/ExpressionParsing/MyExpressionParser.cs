using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TaxTest.ExpressionEvaluation;

namespace TaxTest.ExpressionParsing
{
    class MyExpressionParser : ExpressionBaseVisitor<BaseExpression>, IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
    {
        public static BaseExpression Parse(string input)
        {
            var vistor = new MyExpressionParser();
            ICharStream stream = CharStreams.fromString(input);
            ExpressionLexer lexer = new ExpressionLexer(stream);
            lexer.AddErrorListener(vistor);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new ExpressionParser(tokens);
            parser.AddErrorListener(vistor);
            parser.BuildParseTree = true;
            var tree = parser.complete_expression();
            // TODO: the default error listens write to the console
            BaseExpression result = vistor.Visit(tree);
            if (result is null)
            {
                // Probably missed an override of ExpressionBaseVisitor if this is thrown
                throw new Exception("Programming error, no expression returned.");
            }
            return result;
        }

        void IAntlrErrorListener<int>.SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new Exception("lexer error");
        }

        void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new Exception("parser error");
        }

        public override BaseExpression VisitFloat_num([NotNull] ExpressionParser.Float_numContext context)
        {
            return new NumberExpression(decimal.Parse(context.GetText()));
        }

        BaseExpression VisitBinaryOp(ParserRuleContext context)
        {
            BaseExpression ret = Visit(context.children[0]);

            for (int i = 1; i < context.children.Count; i += 2)
            {
                var op = ((ITerminalNode)(context.children[i])).Symbol;
                var right = Visit(context.children[i + 1]);
                BinaryOp binOp;
                switch (op.Type)
                {
                    case ExpressionLexer.PLUS:
                        binOp = BinaryOp.Add;
                        break;
                    case ExpressionLexer.MINUS:
                        binOp = BinaryOp.Substract;
                        break;
                    case ExpressionLexer.TIMES:
                        binOp = BinaryOp.Multiply;
                        break;
                    case ExpressionLexer.DIVIDE:
                        binOp = BinaryOp.Divide;
                        break;
                    default:
                        throw new Exception("Unexpected op: " + op.Text);
                }
                ret = new BinaryOpExpression(ret, binOp, right);
            }

            return ret;
        }

        public override BaseExpression VisitTerm([NotNull] ExpressionParser.TermContext context)
        {
            return VisitBinaryOp(context);
        }

        public override BaseExpression VisitSimple([NotNull] ExpressionParser.SimpleContext context)
        {
            return VisitBinaryOp(context);
        }

        public override BaseExpression VisitUnary([NotNull] ExpressionParser.UnaryContext context)
        {
            BaseExpression ret = Visit(context.factor());
            if (context.MINUS() != null)
                ret = new BinaryOpExpression(new NumberExpression(decimal.MinusOne), BinaryOp.Multiply, ret);
            return ret;
        }

        public override BaseExpression VisitFactor([NotNull] ExpressionParser.FactorContext context)
        {
            if (context.identifier() != null)
            {
                if (context.parameter_list() != null)
                {
                    var functionName = context.identifier().GetText();
                    var parameters = context.parameter_list();
                    var arguments = new List<BaseExpression>();
                    for (int i = 0; i < parameters.ChildCount; i += 2)
                    {
                        arguments.Add(Visit(parameters.children[i]));
                    }
                    return FunctionFactory.CreateFunction(functionName, arguments);
                }
                return new VariableExpression(context.GetText());
            }
            else if (context.LPAREN() != null)
                return Visit(context.simple());
            else
                return Visit(context.float_num());
        }

        public override BaseExpression VisitComplete_expression([NotNull] ExpressionParser.Complete_expressionContext context)
        {
            return Visit(context.simple());
        }

        protected override BaseExpression AggregateResult(BaseExpression aggregate, BaseExpression nextResult)
        {
            Debug.Fail("This should not be called.");
            return base.AggregateResult(aggregate, nextResult);
        }

        protected override bool ShouldVisitNextChild(IRuleNode node, BaseExpression currentResult)
        {
            Debug.Fail("This should not be called.");
            return base.ShouldVisitNextChild(node, currentResult);
        }
    }
}
