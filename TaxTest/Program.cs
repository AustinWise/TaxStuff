using iText.Forms.Fields;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TaxTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using var reader = new PdfReader(@"D:\AustinWise\Downloads\f8949.pdf");
            using var writer = new PdfWriter(@"c:\temp\out.pdf");
            using var doc = new PdfDocument(reader, writer);

            var form = iText.Forms.PdfAcroForm.GetAcroForm(doc, false);
            var field = form.GetField("topmostSubform[0].Page1[0].Table_Line1[0].Row1[0].f1_9[0]");
            field.SetValue("lolol");

            string fieldFormat = "topmostSubform[0].Page1[0].c1_1[{0}]";

            var boxes = new List<PdfButtonFormField>();
            for (int i = 0; i < 3; i++)
            {
                boxes.Add((PdfButtonFormField)form.GetField(string.Format(fieldFormat, i)));
            }

            for (int i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                var checkState = box.GetAppearanceStates()[0];
                box.SetFieldFlag(32768, true);
                box.SetValue(checkState);
                box.Put(PdfName.AS, new PdfName(checkState));
            }
        }

        private static void PrintButtonInfo(List<PdfButtonFormField> boxes, List<Expression<Func<PdfButtonFormField, object>>> accessors)
        {
            foreach (var expr in accessors)
            {
                Console.Write("{0,20}", getMethodCallName(expr.Body));
                var func = expr.Compile();
                for (int i = 0; i < boxes.Count; i++)
                {
                    Console.Write("{0,20}", formatValue(func(boxes[i])));
                }
                Console.WriteLine();
            }
        }

        static string getMethodCallName(Expression expr)
        {
            switch (expr)
            {
                case MethodCallExpression m:
                    return m.Method.Name;
                case UnaryExpression u:
                    return getMethodCallName(u.Operand);
                default:
                    throw new NotSupportedException("Unknown type: " + expr.GetType());
            }
        }

        static string formatValue(object val)
        {
            if (val is null)
                return "<null>";
            if (val is string[] stringArray)
            {
                return "[" + string.Join(", ", stringArray) + "]";
            }
            return val.ToString();
        }
    }
}
