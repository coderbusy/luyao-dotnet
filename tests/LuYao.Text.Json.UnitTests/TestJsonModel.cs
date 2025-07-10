using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Json;

public class TestJsonModel : TranslatableJsonModel<TestJsonModel>
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
