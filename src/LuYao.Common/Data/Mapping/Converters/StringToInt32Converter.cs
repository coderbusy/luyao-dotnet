using System;

namespace LuYao.Data.Mapping.Converters;

public class StringToInt32Converter : XConverter
{
    public StringToInt32Converter() : base(typeof(string), typeof(int))
    {
    }

    public override Func<object?, object?> ConvertToSource => throw new NotImplementedException();

    public override Func<object?, object?> ConvertFromSource => throw new NotImplementedException();
}
