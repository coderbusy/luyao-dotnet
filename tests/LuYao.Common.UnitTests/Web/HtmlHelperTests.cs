using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Web;

[TestClass]
public class HtmlHelperTests
{
    [TestMethod]
    public void ContainsHtml_WithNullInput_ReturnsFalse()
    {
        // Act
        var result = HtmlHelper.ContainsHtml(null);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ContainsHtml_WithEmptyString_ReturnsFalse()
    {
        // Act
        var result = HtmlHelper.ContainsHtml(string.Empty);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ContainsHtml_WithWhitespaceOnly_ReturnsFalse()
    {
        // Arrange
        var input = "   \t\n\r   ";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ContainsHtml_WithPlainText_ReturnsFalse()
    {
        // Arrange
        var input = "This is just plain text without any HTML.";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ContainsHtml_WithSimpleOpenTag_ReturnsTrue()
    {
        // Arrange
        var input = "Hello <div> world";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithSimpleCloseTag_ReturnsTrue()
    {
        // Arrange
        var input = "Hello </div> world";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithSelfClosingTag_ReturnsTrue()
    {
        // Arrange
        var input = "Image: <img src='test.jpg' />";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithTagAttributes_ReturnsTrue()
    {
        // Arrange
        var input = "<div class='container' id='main'>Content</div>";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithDoctype_ReturnsTrue()
    {
        // Arrange
        var input = "<!DOCTYPE html><html><head><title>Test</title></head></html>";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithDoctypeCaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var input = "<!doctype HTML>";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithHtmlComment_ReturnsTrue()
    {
        // Arrange
        var input = "Some text <!-- This is a comment --> more text";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithMultilineComment_ReturnsTrue()
    {
        // Arrange
        var input = @"Text before <!-- This is a 
        multiline 
        comment --> text after";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithNumericHtmlEntity_ReturnsTrue()
    {
        // Arrange
        var input = "Copyright &#169; 2023";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithHexHtmlEntity_ReturnsTrue()
    {
        // Arrange
        var input = "Copyright &#xa9; 2023";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithNamedHtmlEntity_ReturnsTrue()
    {
        // Arrange
        var input = "Hello &amp; goodbye";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithCommonHtmlEntities_ReturnsTrue()
    {
        // Arrange
        var testCases = new[]
        {
            "Text with &lt; symbol",
            "Text with &gt; symbol", 
            "Text with &quot; symbol",
            "Text with &apos; symbol",
            "Text with &nbsp; space"
        };

        foreach (var testCase in testCases)
        {
            // Act
            var result = HtmlHelper.ContainsHtml(testCase);

            // Assert
            Assert.IsTrue(result, $"Failed for input: {testCase}");
        }
    }

    [TestMethod]
    public void ContainsHtml_WithTagsCaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var testCases = new[]
        {
            "<DIV>",
            "<Div>",
            "</DIV>",
            "</Div>",
            "<IMG src='test.jpg'>",
            "<img SRC='test.jpg'>"
        };

        foreach (var testCase in testCases)
        {
            // Act
            var result = HtmlHelper.ContainsHtml(testCase);

            // Assert
            Assert.IsTrue(result, $"Failed for input: {testCase}");
        }
    }

    [TestMethod]
    public void ContainsHtml_WithTagsContainingWhitespace_ReturnsTrue()
    {
        // Arrange
        var testCases = new[]
        {
            "< div>",
            "<  div >",
            "< / div >",
            "</  div   >"
        };

        foreach (var testCase in testCases)
        {
            // Act
            var result = HtmlHelper.ContainsHtml(testCase);

            // Assert
            Assert.IsTrue(result, $"Failed for input: {testCase}");
        }
    }

    [TestMethod]
    public void ContainsHtml_WithComplexHtmlDocument_ReturnsTrue()
    {
        // Arrange
        var input = @"<!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <title>Test Page</title>
            <!-- This is a comment -->
        </head>
        <body>
            <h1>Welcome &amp; Hello</h1>
            <p>This is a paragraph with &#169; symbol.</p>
        </body>
        </html>";

        // Act
        var result = HtmlHelper.ContainsHtml(input);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ContainsHtml_WithIncompleteTag_ReturnsFalse()
    {
        // Arrange
        var testCases = new[]
        {
            "This has < but no closing",
            "This has > but no opening",
            "Mathematical expression: 5 < 10 > 3",
            "Text with & but not entity"
        };

        foreach (var testCase in testCases)
        {
            // Act
            var result = HtmlHelper.ContainsHtml(testCase);

            // Assert
            Assert.IsFalse(result, $"Should return false for input: {testCase}");
        }
    }

    [TestMethod]
    public void ContainsHtml_WithEdgeCases_ReturnsExpectedResult()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(HtmlHelper.ContainsHtml("<a>"), "Single character tag should return true");
        Assert.IsTrue(HtmlHelper.ContainsHtml("<h1>"), "Valid h1 tag should return true");
        Assert.IsTrue(HtmlHelper.ContainsHtml("<span123>"), "Tag with numbers should return true");
        Assert.IsFalse(HtmlHelper.ContainsHtml("<123>"), "Tag starting with number should return false");
        Assert.IsFalse(HtmlHelper.ContainsHtml("< >"), "Empty tag should return false");
    }
}
