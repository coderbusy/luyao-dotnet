using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(String? value)
    {
        if (value is null) return default;
        switch (value.ToLowerInvariant())
        {
            case "0":
            case "f":
            case "n":
            case "no":
            case "false":
                return false;
            case "1":
            case "t":
            case "y":
            case "yes":
            case "true":
                return true;
        }
        return Convert.ToBoolean(value);
    }
}
