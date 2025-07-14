using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.IO.Updating;

public static class UpdatePackageHelper
{

    public static async Task<string> Hash(Stream fs)
    {
        using var provider = SHA1.Create();
        var bytes = provider.ComputeHash(fs);
        return BitConverter.ToString(bytes).Replace("-", string.Empty);
    }

    public static async Task<string> Hash(string fn)
    {
        using var fs = File.OpenRead(fn);
        return await Hash(fs);
    }

}
