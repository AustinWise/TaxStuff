using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace TaxStuff.FormModel;

// TODO: support expressions that reach across years, to support things like 2020 Form 1040 Schedule J
// TODO: add a mode for loading all years so we can get type checking and run it in CI
class TaxUniverse
{
    private readonly ReadOnlyDictionary<int, Lazy<TaxYearDefinition>> _taxYears;

    public TaxUniverse(string folder)
    {
        var years = new Dictionary<int, Lazy<TaxYearDefinition>>();
        foreach (var dir in new DirectoryInfo(folder).GetDirectories())
        {
            if (!int.TryParse(dir.Name, out int year))
                continue;
            years.Add(year, new Lazy<TaxYearDefinition>(() => new TaxYearDefinition(year, dir.FullName)));
        }
        this._taxYears = new(years);
    }

    public TaxYearDefinition GetYear(int year)
    {
        return _taxYears[year].Value;
    }
}
