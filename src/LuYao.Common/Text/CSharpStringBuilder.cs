using System;
using System.Text;

namespace LuYao.Text;

/// <summary>
/// 用于辅助生成 C# 代码的字符串构建器。
/// </summary>
public class CSharpStringBuilder
{
    private readonly StringBuilder _builder = new StringBuilder();

    /// <summary>
    /// 返回当前构建的字符串内容。
    /// </summary>
    /// <returns>生成的字符串。</returns>
    public override string ToString() => _builder.ToString();

    /// <summary>
    /// 添加一行带有当前缩进的字符串。
    /// </summary>
    /// <param name="value">要添加的内容。</param>
    public void AppendLine(string value)
    {
        _builder.Append(this._tabString);
        _builder.AppendLine(value);
    }

    /// <summary>
    /// 添加 using 指令。
    /// </summary>
    /// <param name="ns">命名空间名称数组。</param>
    public void AddUsing(params string[] ns)
    {
        foreach (var n in ns) _builder.AppendLine($"using {n};");
    }

    /// <summary>
    /// 添加一个空行。
    /// </summary>
    public void AppendLine() => _builder.AppendLine();

    private string _tabString = string.Empty;
    private int _tabs;
    private void SetTabs()
    {
        if (_tabs > 0)
        {
            _tabString = new string(' ', _tabs);
        }
        else
        {
            _tabString = string.Empty;
        }
    }

    /// <summary>
    /// 增加一个缩进（4 个空格）。
    /// </summary>
    public void AddTab()
    {
        _tabs += 4;
        SetTabs();
    }

    /// <summary>
    /// 减少一个缩进（4 个空格）。
    /// </summary>
    public void RemoveTab()
    {
        _tabs -= 4;
        if (_tabs < 0) _tabs = 0;
        SetTabs();
    }

    /// <summary>
    /// 临时增加一个缩进，返回 IDisposable，在释放时自动减少缩进。
    /// </summary>
    /// <returns>IDisposable 对象，用于自动管理缩进。</returns>
    public IDisposable Tab()
    {
        this.AddTab();
        return new DisposeAction(() => this.RemoveTab());
    }

    /// <summary>
    /// 添加一个作用域（大括号），并自动管理缩进。
    /// </summary>
    /// <param name="flag">为 true 时作用域结尾添加分号。</param>
    /// <returns>IDisposable 对象，用于自动关闭作用域。</returns>
    public IDisposable Scope(bool flag = false)
    {
        this.AppendLine("{");
        this.AddTab();
        return new DisposeAction(() =>
        {
            this.RemoveTab();
            this.AppendLine(flag ? "};" : "}");
        });
    }

    /// <summary>
    /// 添加命名空间作用域，并自动管理缩进。
    /// </summary>
    /// <param name="name">命名空间名称。</param>
    /// <returns>IDisposable 对象，用于自动关闭命名空间作用域。</returns>
    public IDisposable NamespaceScope(string name)
    {
        this.AppendLine($"namespace {name}");
        return this.Scope();
    }

    /// <summary>
    /// 设置命名空间（C# 10 文件作用域命名空间）。
    /// </summary>
    /// <param name="name">命名空间名称。</param>
    public void SetNamespace(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.AppendLine($"namespace {name};");
    }

    /// <summary>
    /// 添加 public class 作用域，并自动管理缩进。
    /// </summary>
    /// <param name="name">类名。</param>
    /// <returns>IDisposable 对象，用于自动关闭类作用域。</returns>
    public IDisposable PublicClassScope(string name)
    {
        this.AppendLine($"public class {name}");
        return this.Scope();
    }

    /// <summary>
    /// 添加类作用域（可指定修饰符），并自动管理缩进。
    /// </summary>
    /// <param name="name">类名。</param>
    /// <param name="modifiers">类修饰符（如 public、internal 等）。</param>
    /// <returns>IDisposable 对象，用于自动关闭类作用域。</returns>
    public IDisposable ClassScope(string name, string? modifiers = null)
    {
        string str = string.Join(" ", modifiers, "class", name);
        this.AppendLine(str.Trim());
        return this.Scope();
    }
}
