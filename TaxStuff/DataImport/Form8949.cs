using System.Collections.Generic;
using TaxStuff.FormModel;

namespace TaxStuff.DataImport
{
    class Form8949 : IConvertibleToFormInstance
    {
        public Form8949Code Code { get; }
        public List<Form8949Line> Lines { get; }

        public Form8949(Form8949Code code, List<Form8949Line> lines)
        {
            this.Code = code;
            this.Lines = lines;
        }

        public FormInstance ConvertToFormInstance(TaxYearDefinition taxYear)
        {
            return new FormInstance(taxYear.Forms["8949"], Code, Lines);
        }
    }
}
