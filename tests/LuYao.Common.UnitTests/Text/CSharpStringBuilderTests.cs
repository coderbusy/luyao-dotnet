using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text;

[TestClass]
public class CSharpStringBuilderTests
{
    [TestMethod]
    public void ToString_EmptyBuilder_ReturnsEmptyString()
    {
        var builder = new CSharpStringBuilder();
        Assert.AreEqual(string.Empty, builder.ToString());
    }

    [TestMethod]
    public void AppendLine_WithValue_AppendsLineWithNoIndent()
    {
        var builder = new CSharpStringBuilder();
        builder.AppendLine("abc");
        Assert.AreEqual("abc" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void AppendLine_WithIndent_AppendsLineWithIndent()
    {
        var builder = new CSharpStringBuilder();
        builder.AddTab();
        builder.AppendLine("abc");
        Assert.AreEqual("    abc" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void AddUsing_SingleNamespace_AppendsUsingStatement()
    {
        var builder = new CSharpStringBuilder();
        builder.AddUsing("System");
        Assert.AreEqual("using System;" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void AddUsing_MultipleNamespaces_AppendsAllUsingStatements()
    {
        var builder = new CSharpStringBuilder();
        builder.AddUsing("System", "System.Text");
        Assert.AreEqual("using System;" + Environment.NewLine + "using System.Text;" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void AppendLine_WithoutValue_AppendsEmptyLine()
    {
        var builder = new CSharpStringBuilder();
        builder.AppendLine();
        Assert.AreEqual(Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void AddTab_Once_IncreasesIndent()
    {
        var builder = new CSharpStringBuilder();
        builder.AddTab();
        builder.AppendLine("abc");
        Assert.AreEqual("    abc" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void RemoveTab_AfterAddTab_DecreasesIndent()
    {
        var builder = new CSharpStringBuilder();
        builder.AddTab();
        builder.RemoveTab();
        builder.AppendLine("abc");
        Assert.AreEqual("abc" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void Tab_UsingBlock_AutoManagesIndent()
    {
        var builder = new CSharpStringBuilder();
        using (builder.Tab())
        {
            builder.AppendLine("abc");
        }
        builder.AppendLine("def");
        Assert.AreEqual("    abc" + Environment.NewLine + "def" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void Scope_Default_AppendsBracesAndManagesIndent()
    {
        var builder = new CSharpStringBuilder();
        using (builder.Scope())
        {
            builder.AppendLine("abc");
        }
        var expected = "{" + Environment.NewLine + "    abc" + Environment.NewLine + "}" + Environment.NewLine;
        Assert.AreEqual(expected, builder.ToString());
    }

    [TestMethod]
    public void Scope_WithFlagTrue_AppendsBracesWithSemicolon()
    {
        var builder = new CSharpStringBuilder();
        using (builder.Scope(true))
        {
            builder.AppendLine("abc");
        }
        var expected = "{" + Environment.NewLine + "    abc" + Environment.NewLine + "};" + Environment.NewLine;
        Assert.AreEqual(expected, builder.ToString());
    }

    [TestMethod]
    public void NamespaceScope_ValidName_AppendsNamespaceAndBraces()
    {
        var builder = new CSharpStringBuilder();
        using (builder.NamespaceScope("TestNs"))
        {
            builder.AppendLine("abc");
        }
        var expected = "namespace TestNs" + Environment.NewLine + "{" + Environment.NewLine + "    abc" + Environment.NewLine + "}" + Environment.NewLine;
        Assert.AreEqual(expected, builder.ToString());
    }

    [TestMethod]
    public void SetNamespace_NullOrWhitespace_ThrowsArgumentNullException()
    {
        var builder = new CSharpStringBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.SetNamespace(" "));
    }

    [TestMethod]
    public void SetNamespace_ValidName_AppendsFileScopedNamespace()
    {
        var builder = new CSharpStringBuilder();
        builder.SetNamespace("TestNs");
        Assert.AreEqual("namespace TestNs;" + Environment.NewLine, builder.ToString());
    }

    [TestMethod]
    public void PublicClassScope_ValidName_AppendsClassAndBraces()
    {
        var builder = new CSharpStringBuilder();
        using (builder.PublicClassScope("MyClass"))
        {
            builder.AppendLine("abc");
        }
        var expected = "public class MyClass" + Environment.NewLine + "{" + Environment.NewLine + "    abc" + Environment.NewLine + "}" + Environment.NewLine;
        Assert.AreEqual(expected, builder.ToString());
    }

    [TestMethod]
    public void ClassScope_WithModifiers_AppendsModifiersClassAndBraces()
    {
        var builder = new CSharpStringBuilder();
        using (builder.ClassScope("MyClass", "internal sealed"))
        {
            builder.AppendLine("abc");
        }
        var expected = "internal sealed class MyClass" + Environment.NewLine + "{" + Environment.NewLine + "    abc" + Environment.NewLine + "}" + Environment.NewLine;
        Assert.AreEqual(expected, builder.ToString());
    }

    [TestMethod]
    public void ClassScope_WithoutModifiers_AppendsClassAndBraces()
    {
        var builder = new CSharpStringBuilder();
        using (builder.ClassScope("MyClass"))
        {
            builder.AppendLine("abc");
        }
        var expected = "class MyClass" + Environment.NewLine + "{" + Environment.NewLine + "    abc" + Environment.NewLine + "}" + Environment.NewLine;
        Assert.AreEqual(expected, builder.ToString());
    }
}