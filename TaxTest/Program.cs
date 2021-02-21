using iText.Kernel.Pdf;
using System;

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
        }
    }
}
