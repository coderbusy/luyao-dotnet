using System.Text.RegularExpressions;

namespace LuYao.Security;

/// <summary>
/// 密码强度枚举，表示密码的不同强度级别。
/// </summary>
public enum PasswordScore
{
    /// <summary>
    /// 空密码。
    /// </summary>
    Blank = 0,

    /// <summary>
    /// 非常弱的密码。
    /// </summary>
    VeryWeak = 1,

    /// <summary>
    /// 弱密码。
    /// </summary>
    Weak = 2,

    /// <summary>
    /// 中等强度的密码。
    /// </summary>
    Medium = 3,

    /// <summary>
    /// 强密码。
    /// </summary>
    Strong = 4,

    /// <summary>
    /// 非常强的密码。
    /// </summary>
    VeryStrong = 5
}

/// <summary>
/// 密码建议工具类，用于检查密码的强度。
/// </summary>
public static class PasswordAdvisor
{
    /// <summary>
    /// 检查密码的强度并返回对应的评分。
    /// </summary>
    /// <param name="password">需要检查的密码字符串。</param>
    /// <returns>密码强度评分，类型为 <see cref="PasswordScore"/>。</returns>
    public static PasswordScore CheckStrength(string password)
    {
        int score = 0;

        if (password.Length < 1) return PasswordScore.Blank;
        if (password.Length < 4) return PasswordScore.VeryWeak;

        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (Regex.Match(password, @"\d+").Success) score++;
        if (Regex.Match(password, @"[a-z]+").Success && Regex.Match(password, @"[A-Z]+").Success) score++;
        if (Regex.Match(password, @"[!@#$%^&*?_~-£()]+").Success) score++;

        return (PasswordScore)score;
    }
}
