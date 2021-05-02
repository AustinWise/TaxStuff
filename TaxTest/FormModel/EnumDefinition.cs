using System.Xml.Linq;

namespace TaxTest.FormModel
{
    class EnumDefinition : CompoundDefinition<EnumElement>
    {
        public EnumDefinition(XElement node)
            : base(node, "Value", el => new EnumElement(el))
        {
        }
    }
}
