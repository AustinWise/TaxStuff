#nullable enable

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionParsing
{
    class MyExpressionParser(ParsingEnvironment _environment) : ExpressionBaseVisitor<BaseExpression>, IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
    {
        public static BaseExpression? Parse(ParsingEnvironment env, XElement node, string attributeName)
        {
            var calcStr = node.Attribute(attributeName)?.Value;
            var calcNode = node.Elements().ToArray();

            //make sure there is at most one Calc definition
            if (calcStr is null && calcNode.Length == 0)
                return null;
            if (calcStr is object && calcNode.Length > 0)
                throw new FileLoadException(node, $"Line contains both and {attributeName} attribute and a {attributeName} node.");

            // do some light validation
            if (calcStr is not null)
            {
                if (string.IsNullOrWhiteSpace(calcStr))
                {
                    if (calcStr is null)
                        return null;
                    throw new FileLoadException(node, "Empty expression.");
                }
            }
            else if (calcNode.Length > 1)
                throw new FileLoadException(calcNode[1], $"Multiple {attributeName} sub-elements.");

            try
            {
                return calcStr is not null ? Parse(env, calcStr) : XmlExpressionParser.Parse(env, calcNode[0]);
            }
            catch (Exception ex)
            {
                throw new FileLoadException(node, $"Failed to parse {attributeName}.", ex);
            }
        }

        public static BaseExpression Parse(ParsingEnvironment env, string input)
        {
            var vistor = new MyExpressionParser(env);
            ICharStream stream = CharStreams.fromString(input);
            ExpressionLexer lexer = new ExpressionLexer(stream);
            lexer.AddErrorListener(vistor);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new ExpressionParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(vistor);
            parser.BuildParseTree = true;
            var tree = parser.completeExpression();
            BaseExpression result = vistor.Visit(tree);
            if (result is null)
            {
                // Probably missed an override of ExpressionBaseVisitor if this is thrown
                throw new Exception("Programming error, no expression returned.");
            }
            return result;
        }

        static Exception createException(string recognizerName, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string exceptionMessage = $"{recognizerName} error parsing expression at line ({line}:{charPositionInLine}): {msg}";
            if (e is null)
                return new Exception(exceptionMessage);
            else
                return new Exception(exceptionMessage, e);
        }

        void IAntlrErrorListener<int>.SyntaxError(System.IO.TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw createException("Lexer", line, charPositionInLine, msg, e);
        }

        void IAntlrErrorListener<IToken>.SyntaxError(System.IO.TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw createException("Parser", line, charPositionInLine, msg, e);
        }

        public override BaseExpression VisitFloatNum([NotNull] ExpressionParser.FloatNumContext context)
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
                    case ExpressionLexer.EQUAL:
                        binOp = BinaryOp.Equal;
                        break;
                    case ExpressionLexer.NEQUAL:
                        binOp = BinaryOp.NotEqual;
                        break;
                    case ExpressionLexer.LT:
                        binOp = BinaryOp.LessThan;
                        break;
                    case ExpressionLexer.GT:
                        binOp = BinaryOp.GreaterThan;
                        break;
                    case ExpressionLexer.LTEQ:
                        binOp = BinaryOp.LessThanOrEqual;
                        break;
                    case ExpressionLexer.GTEQ:
                        binOp = BinaryOp.GreaterThanOrEqual;
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

        public override BaseExpression VisitPlusMinus([NotNull] ExpressionParser.PlusMinusContext context)
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

        public override BaseExpression VisitFunctionInvoke([NotNull] ExpressionParser.FunctionInvokeContext context)
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

        public override BaseExpression VisitSelector([NotNull] ExpressionParser.SelectorContext context)
        {
            if (context.DOT() is not null)
            {
                var baseExpression = Visit(context.selector());
                var fieldName = context.identifier().GetText();
                return new FieldSelectorExpression(baseExpression, fieldName);
            }
            else if (context.LBRACKET() is not null)
            {
                throw new NotImplementedException();
            }
            else if (context.functionInvoke() is not null)
            {
                return Visit(context.functionInvoke());
            }
            else
            {
                var thingName = context.identifier().GetText();
                if (thingName.StartsWith("Year"))
                {
                    throw new NotImplementedException("Year-qualified references not yet implemented.");
                }
                else if (thingName == "Form8949Code")
                {
                    return new Form8949CodeReferenceExpression();
                }
                else if (thingName.StartsWith("Form"))
                {
                    return new FormReferenceExpression(thingName.Substring(4));
                }
                else
                {
                    var fieldName = context.identifier().GetText();
                    if (_environment.CurrentFormName is null)
                    {
                        throw new Exception($"Found '{fieldName}', which looks like a form field reference, but we are currently processing the Return.");
                    }
                    return new FieldSelectorExpression(new FormReferenceExpression(_environment.CurrentFormName), fieldName);
                }
            }
        }

        public override BaseExpression VisitCompleteExpression([NotNull] ExpressionParser.CompleteExpressionContext context)
        {
            return Visit(context.simple());
        }

        protected override BaseExpression AggregateResult(BaseExpression aggregate, BaseExpression nextResult)
        {
            if (aggregate is not null && nextResult is not null)
                throw new Exception("Aggregation of multiple values is not supported.");
            return (aggregate ?? nextResult)!;
        }
    }
}
