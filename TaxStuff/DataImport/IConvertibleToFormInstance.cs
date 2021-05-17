using TaxStuff.FormModel;

namespace TaxStuff.DataImport
{
    interface IConvertibleToFormInstance
    {
        FormInstance ConvertToFormInstance(TaxYearDefinition taxYear);
    }
}
