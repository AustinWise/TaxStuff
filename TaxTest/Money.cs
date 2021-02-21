using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxTest
{
    readonly struct Money
    {
        readonly int _pennies;

        public Money(int pennies)
        {
            this._pennies = pennies;
        }
    }
}
