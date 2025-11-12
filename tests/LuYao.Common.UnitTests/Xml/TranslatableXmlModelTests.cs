using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Xml;

[TestClass]
public class TranslatableXmlModelTests
{
    [TestMethod]
    public void Transform_ValidXml_ShouldDeserializeCorrectly()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
<catalog>
	<cd>
		<title>Empire Burlesque</title>
		<artist>Bob Dylan</artist>
		<country>USA</country>
		<company>Columbia</company>
		<price>10.90</price>
		<year>1985</year>
	</cd>
	<cd>
		<title>Hide your heart</title>
		<artist>Bonnie Tyler</artist>
		<country>UK</country>
		<company>CBS Records</company>
		<price>9.90</price>
		<year>1988</year>
	</cd>
</catalog>";

        // Act
        var result = TestXmlModel.Transform(xml);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.TitleList);
        Assert.AreEqual(2, result.TitleList.Length);
    }

    [TestMethod]
    public void Transform_NoResource_ShouldThrowException()
    {
        // Arrange
        var xml = "<Invalid>XML</Invalid>";

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => TestXmlModelNoResource.Transform(xml));
    }
}
