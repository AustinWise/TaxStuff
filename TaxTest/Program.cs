using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Diagnostics;
using System.IO;
using TaxTest.ExpressionParsing;

namespace TaxTest
{
    class Program : ExpressionBaseVisitor<float>, IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
    {
        const string RETURN = @"d:\AustinWise\Desktop\Return.xml";
        static void Main(string[] args)
        {
            new Program().Run("asdf-fff.asd - fff");
        }

        void IAntlrErrorListener<int>.SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new Exception("lexer error");
        }

        void IAntlrErrorListener<IToken>.SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new Exception("parser error");
        }

        public void Run(string input)
        {
            ICharStream stream = CharStreams.fromString(input);
            ExpressionLexer lexer = new ExpressionLexer(stream);
            lexer.AddErrorListener(this);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new ExpressionParser(tokens);
            parser.AddErrorListener(this);
            parser.BuildParseTree = true;
            Console.WriteLine(parser.ErrorHandler.GetType().FullName);
            var tree = parser.complete_expression();
            // TODO: the default error listens write to the console
            float result = Visit(tree);
            Console.WriteLine($"{input} = {result}");
        }

        public override float VisitComplete_expression([NotNull] ExpressionParser.Complete_expressionContext context)
        {
            return Visit(context.simple());
        }

        public override float VisitSimple([Antlr4.Runtime.Misc.NotNull] ExpressionParser.SimpleContext context)
        {
            float ret = Visit(context.children[0]);

            for (int i = 1; i < context.children.Count; i += 2)
            {
                var op = ((ITerminalNode)(context.children[i])).Symbol.Type;
                var value = Visit(context.children[i + 1]);
                if (op == ExpressionLexer.PLUS)
                {
                    ret += value;
                }
                else
                {
                    Debug.Assert(op == ExpressionLexer.MINUS);
                    ret -= value;
                }
            }

            return ret;
        }

        public override float VisitTerm([Antlr4.Runtime.Misc.NotNull] ExpressionParser.TermContext context)
        {
            float ret = Visit(context.children[0]);

            for (int i = 1; i < context.children.Count; i += 2)
            {
                var op = ((ITerminalNode)(context.children[i])).Symbol.Type;
                var value = Visit(context.children[i + 1]);
                if (op == ExpressionLexer.TIMES)
                {
                    ret *= value;
                }
                else
                {
                    Debug.Assert(op == ExpressionLexer.DIVIDE);
                    ret /= value;
                }
            }

            return ret;
        }

        public override float VisitFactor([Antlr4.Runtime.Misc.NotNull] ExpressionParser.FactorContext context)
        {
            if (context.LPAREN() != null)
                return Visit(context.simple());
            else if (context.identifier() != null)
                return Visit(context.identifier());
            else
                return Visit(context.float_num());
        }

        public override float VisitUnary([Antlr4.Runtime.Misc.NotNull] ExpressionParser.UnaryContext context)
        {
            float ret = Visit(context.factor());
            if (context.MINUS() != null)
                ret = -ret;
            return ret;
        }

        public override float VisitFloat_num([Antlr4.Runtime.Misc.NotNull] ExpressionParser.Float_numContext context)
        {
            return float.Parse(context.GetText());
        }

        public override float VisitIdentifier([NotNull] ExpressionParser.IdentifierContext context)
        {
            Console.WriteLine("ident: " + context.GetText());
            return 42;
        }
    }
}
