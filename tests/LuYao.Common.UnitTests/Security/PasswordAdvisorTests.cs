using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuYao.Security;

namespace LuYao.Tests.Security
{
    /// <summary>
    /// PasswordAdvisor 的单元测试类。
    /// </summary>
    [TestClass]
    public class PasswordAdvisorTests
    {
        /// <summary>
        /// 测试空字符串时返回 Blank。
        /// </summary>
        [TestMethod]
        public void CheckStrength_EmptyString_ReturnsBlank()
        {
            var result = PasswordAdvisor.CheckStrength(string.Empty);
            Assert.AreEqual(PasswordScore.Blank, result);
        }

        /// <summary>
        /// 测试长度小于 4 的字符串时返回 VeryWeak。
        /// </summary>
        [TestMethod]
        public void CheckStrength_ShortPassword_ReturnsVeryWeak()
        {
            var result = PasswordAdvisor.CheckStrength("abc");
            Assert.AreEqual(PasswordScore.VeryWeak, result);
        }

        /// <summary>
        /// 测试长度为 8 且无其他复杂性时返回 VeryWeak。
        /// </summary>
        [TestMethod]
        public void CheckStrength_LengthEightNoComplexity_ReturnsVeryWeak()
        {
            var result = PasswordAdvisor.CheckStrength("abcdefgh");
            Assert.AreEqual(PasswordScore.VeryWeak, result);
        }

        /// <summary>
        /// 测试长度为 12 且无其他复杂性时返回 Weak。
        /// </summary>
        [TestMethod]
        public void CheckStrength_LengthTwelveNoComplexity_ReturnsWeak()
        {
            var result = PasswordAdvisor.CheckStrength("abcdefghijkl");
            Assert.AreEqual(PasswordScore.Weak, result);
        }

        [TestMethod]
        public void CheckStrength_ContainsDigit_ReturnsWeak()
        {
            var result = PasswordAdvisor.CheckStrength("abc12345");
            Assert.AreEqual(PasswordScore.Weak, result);
        }
    }
}