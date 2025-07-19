using System.Collections.Generic;
using TaxStuff.FormModel;

namespace TaxStuff.DataImport;

class Form8949(Form8949Code code, List<Form8949Line> lines) : IConvertibleToFormInstance
{
    public Form8949Code Code { get; } = code;
    public List<Form8949Line> Lines { get; } = lines;

    public FormInstance ConvertToFormInstance(TaxYearDefinition taxYear)
    {
        return new FormInstance(taxYear.Forms["8949"], Code, Lines);
    }
}
