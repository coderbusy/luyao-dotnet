using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Net.Http;

[TestClass]
public class HttpResponseMessageExtensionsTests
{
    [TestMethod]
    public async Task ReadAsHtmlAsync_ResponseIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        HttpResponseMessage response = null;

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            await HttpResponseMessageExtensions.ReadAsHtmlAsync(response);
        });
    }

    [TestMethod]
    public async Task ReadAsHtmlAsync_CharsetInHeader_UsesHeaderEncoding()
    {
        // Arrange
        var content = new StringContent("测试内容", Encoding.Unicode);
        content.Headers.ContentType.CharSet = "utf-16";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content
        };

        // Act
        var result = await response.ReadAsHtmlAsync();

        // Assert
        Assert.AreEqual("测试内容", result);
    }

    [TestMethod]
    public async Task ReadAsHtmlAsync_CharsetInHtml_UsesHtmlEncoding()
    {
        // Arrange
        var html = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\"><div>中文内容</div>";
        var bytes = Encoding.GetEncoding("gb2312").GetBytes(html);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content
        };

        // Act
        var result = await response.ReadAsHtmlAsync();

        // Assert
        Assert.IsTrue(result.Contains("中文内容"));
    }

    [TestMethod]
    public async Task ReadAsHtmlAsync_NoCharset_UsesUtf8ByDefault()
    {
        // Arrange
        var text = "默认UTF8内容";
        var bytes = Encoding.UTF8.GetBytes(text);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content
        };

        // Act
        var result = await response.ReadAsHtmlAsync();

        // Assert
        Assert.AreEqual(text, result);
    }
}
