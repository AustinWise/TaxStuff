using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace TaxTest.FormModel
{
    class FormDefinition
    {
        public static FormDefinition LoadFromFile(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            using var reader = new StreamReader(filePath);
            try
            {
                return new FormDefinition(name, reader);
            }
            catch (FileLoadException ex)
            {
                throw new FileLoadException($"Error loading '{filePath}'.", ex);
            }
        }

        private FormDefinition(string name, TextReader reader)
        {
            this.Name = name;

            var doc = XDocument.Load(reader, LoadOptions.SetLineInfo);

            this.AllowMultiple = doc.Root.GetOptionalBoolAttributeValue("AllowMultiple") ?? false;

            var enums = new Dictionary<string, EnumDefinition>();
            var structs = new Dictionary<string, StructDefinition>();
            var lines = new Dictionary<string, LineDefinition>();
            var lineNumbers = new HashSet<string>();
            foreach (var node in doc.Root.Elements())
            {
                switch (node.Name.LocalName)
                {
                    case "Enum":
                        enums.CheckNameAndAdd(node, new EnumDefinition(node));
                        break;
                    case "Struct":
                        structs.CheckNameAndAdd(node, new StructDefinition(node));
                        break;
                    case "Line":
                        var lineDef = lines.CheckNameAndAdd(node, new LineDefinition(node));
                        if (!lineNumbers.Add(lineDef.Number))
                            throw new FileLoadException(node, $"Duplicate line number '{lineDef.Number}'.");
                        break;
                    default:
                        throw new FileLoadException(node, $"Unexpected element '{node.Name}'.");
                }
            }


            this.Enums = new ReadOnlyDictionary<string, EnumDefinition>(enums);
            this.Structs = new ReadOnlyDictionary<string, StructDefinition>(structs);
            this.Lines = new ReadOnlyDictionary<string, LineDefinition>(lines);
        }

        public string Name { get; }

        public bool AllowMultiple { get; }

        public ReadOnlyDictionary<string, EnumDefinition> Enums { get; }
        public ReadOnlyDictionary<string, StructDefinition> Structs { get; }
        public ReadOnlyDictionary<string, LineDefinition> Lines { get; }
    }
}
