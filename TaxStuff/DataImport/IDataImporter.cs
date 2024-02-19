using System.Collections.Generic;

namespace TaxStuff.DataImport;

interface IDataImporter
{
    public List<IConvertibleToFormInstance> GetForms(int taxYear);
}
