using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao;

[TestClass]
public class TestInitializer
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // 在所有测试运行前执行的代码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // GBK编码需要注册
    }
}
