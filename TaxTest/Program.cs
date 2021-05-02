using System;
using TaxTest.ExpressionParsing;
using TaxTest.FormModel;

namespace TaxTest
{
    class Program
    {
        const string RETURN = @"d:\AustinWise\Desktop\Return.xml";
        static void Main(string[] args)
        {
            Console.WriteLine(MyExpressionParser.Parse("1 + 2"));

            var taxYear2020 = new TaxYearDefinition(@"C:\src\TaxTest\TaxTest\2020\");
            taxYear2020.TypeCheck();
        }
    }
}
