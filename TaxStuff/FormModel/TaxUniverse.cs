using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TaxStuff.FormModel;

// TODO: support expressions that reach across years, to support things like 2020 Form 1040 Schedule J
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

    /// <summary>
    /// Type check all years in the universe.
    /// </summary>
    /// <returns>Any type check or loading errors, keyed by year.</returns>
    public Dictionary<int, Exception> TypeCheckAllYears()
    {
        var retLock = new Lock();
        var ret = new Dictionary<int, Exception>();
        Parallel.ForEach(_taxYears, kvp =>
        {
            try
            {
                _ = kvp.Value.Value;
            }
            catch (Exception ex)
            {
                using (retLock.EnterScope())
                {
                    ret[kvp.Key] = ex;
                }
            }
        });

        return ret;
    }
}
