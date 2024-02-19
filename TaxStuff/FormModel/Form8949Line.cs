using System;

namespace TaxStuff.FormModel;

/// <remarks>
/// A null Acquired means "various".
/// </remarks>
record Form8949Line(string Description, DateTime? Acquired, DateTime Sold, decimal CostBasis, decimal SalePrice, decimal Adjustment)
{
}
