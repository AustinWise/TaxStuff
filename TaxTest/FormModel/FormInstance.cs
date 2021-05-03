using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace TaxTest.FormModel
{
    class FormInstance
    {
        public string Name { get; }

        public Dictionary<string, List<decimal>> Values { get; }

        public FormDefinition Definition { get; }

        public FormInstance(XElement node, TaxYearDefinition taxYear)
        {
            this.Name = node.AttributeValue("Name");
            this.Values = new();
            this.Definition = taxYear.Forms[Name];

            foreach (var el in node.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "Line":
                        string number = el.OptionalAttributeValue("Number");
                        string name = el.OptionalAttributeValue("Name");
                        LineDefinition lineDef;
                        if (number is null && name is null)
                            throw new FileLoadException(el, "Missing Name and Number attributes on line.");
                        else if (number is not null && name is not null)
                            throw new FileLoadException(el, "Missing Name and Number attributes on line.");
                        else if (name is not null)
                            lineDef = Definition.Lines[name]; // TODO: nicer exception for missing line
                        else
                        {
                            Debug.Assert(number is not null);
                            lineDef = Definition.LinesByNumber[number]; // TODO: nicer exception for missing line
                        }

                        if (Values.TryGetValue(lineDef.Name, out List<decimal> lineValues))
                        {
                            if (!lineDef.AllowMultiple)
                                throw new FileLoadException(el, $"Multiple definitions for Form{name}.{lineDef.Name}.");
                        }
                        else
                        {
                            lineValues = new List<decimal>();
                            Values.Add(lineDef.Name, lineValues);
                        }
                        lineValues.Add(el.DecimalAttributeValue("Value"));

                        break;
                    default:
                        throw new FileLoadException(el, "Unkown node name: " + el.Name);
                }
            }
        }

    }
}
