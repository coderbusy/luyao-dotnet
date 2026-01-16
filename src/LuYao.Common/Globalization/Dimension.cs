using System.Collections.Generic;

namespace LuYao.Globalization;

public record struct DimensionItem(DimensionKind Kind, decimal Value);

public class Dimension
{
    public DimensionUnit Unit { get; set; } = DimensionUnit.Centimeter;
    public List<DimensionItem> Items { get; set; } = new List<DimensionItem>();
    public bool IsEmpty => Items.Count == 0;
}
