using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace TaxStuff.FormModel
{
    // TODO: support expressions that reach across years, to support things like 2020 Form 1040 Schedule J
    class TaxUniverse
    {
        public ReadOnlyDictionary<int, TaxYearDefinition> TaxYears { get; }

        public TaxUniverse(string folder)
        {
            var years = new Dictionary<int, TaxYearDefinition>();
            foreach (var dir in new DirectoryInfo(folder).GetDirectories())
            {
                if (!int.TryParse(dir.Name, out int year))
                    continue;
                years.Add(year, new TaxYearDefinition(year, dir.FullName));
            }
            this.TaxYears = new(years);
        }
    }
}
