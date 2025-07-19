using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TaxStuff.DataImport;

static class DataImporterFactory
{
    private static readonly Dictionary<string, ConstructorInfo> s_importers = [];

    static void AddImporter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : IDataImporter
    {
        var type = typeof(T);
        var ctor = type.GetConstructor([typeof(string)]) ?? throw new Exception($"Type {type.FullName} does not have a public constructor that takes one string.");
        const string IMPORTER = "Importer";
        if (!type.Name.EndsWith(IMPORTER))
            throw new Exception($"Type {type.FullName}'s name does not end with '{IMPORTER}'.");
        s_importers.Add(type.Name[..^IMPORTER.Length], ctor);
    }

    static DataImporterFactory()
    {
        AddImporter<OfxXmlImporter>();
    }

    public static IDataImporter Create(string name, string fileName)
    {
        if (!s_importers.TryGetValue(name, out ConstructorInfo? ctor))
            throw new Exception($"Unknown importer name: {name}");
        return (IDataImporter)ctor.Invoke([fileName]);
    }
}
