namespace LuYao.Data;

[TestClass]
public class RecordNamingPolicyTests
{
    // ─── CamelCase ────────────────────────────────────────────────────────────────

    [TestMethod]
    public void CamelCase_SimpleWord() =>
        Assert.AreEqual("myProperty", RecordNamingPolicy.CamelCase.ConvertName("MyProperty"));

    [TestMethod]
    public void CamelCase_MultiWord() =>
        Assert.AreEqual("myPropertyName", RecordNamingPolicy.CamelCase.ConvertName("MyPropertyName"));

    [TestMethod]
    public void CamelCase_SingleChar() =>
        Assert.AreEqual("id", RecordNamingPolicy.CamelCase.ConvertName("Id"));

    [TestMethod]
    public void CamelCase_AlreadyCamel() =>
        Assert.AreEqual("myProperty", RecordNamingPolicy.CamelCase.ConvertName("myProperty"));

    [TestMethod]
    public void CamelCase_AcronymPrefix_XMLParser() =>
        Assert.AreEqual("xmlParser", RecordNamingPolicy.CamelCase.ConvertName("XMLParser"));

    [TestMethod]
    public void CamelCase_AcronymSuffix_ParseXML() =>
        Assert.AreEqual("parseXml", RecordNamingPolicy.CamelCase.ConvertName("ParseXML"));

    [TestMethod]
    public void CamelCase_EmptyString() =>
        Assert.AreEqual("", RecordNamingPolicy.CamelCase.ConvertName(""));

    // ─── SnakeCaseLower ───────────────────────────────────────────────────────────

    [TestMethod]
    public void SnakeCaseLower_SimpleWord() =>
        Assert.AreEqual("my_property", RecordNamingPolicy.SnakeCaseLower.ConvertName("MyProperty"));

    [TestMethod]
    public void SnakeCaseLower_MultiWord() =>
        Assert.AreEqual("my_property_name", RecordNamingPolicy.SnakeCaseLower.ConvertName("MyPropertyName"));

    [TestMethod]
    public void SnakeCaseLower_Id() =>
        Assert.AreEqual("id", RecordNamingPolicy.SnakeCaseLower.ConvertName("Id"));

    [TestMethod]
    public void SnakeCaseLower_XMLParser() =>
        Assert.AreEqual("xml_parser", RecordNamingPolicy.SnakeCaseLower.ConvertName("XMLParser"));

    // ─── SnakeCaseUpper ───────────────────────────────────────────────────────────

    [TestMethod]
    public void SnakeCaseUpper_SimpleWord() =>
        Assert.AreEqual("MY_PROPERTY", RecordNamingPolicy.SnakeCaseUpper.ConvertName("MyProperty"));

    [TestMethod]
    public void SnakeCaseUpper_MultiWord() =>
        Assert.AreEqual("MY_PROPERTY_NAME", RecordNamingPolicy.SnakeCaseUpper.ConvertName("MyPropertyName"));

    [TestMethod]
    public void SnakeCaseUpper_Id() =>
        Assert.AreEqual("ID", RecordNamingPolicy.SnakeCaseUpper.ConvertName("Id"));

    [TestMethod]
    public void SnakeCaseUpper_XMLParser() =>
        Assert.AreEqual("XML_PARSER", RecordNamingPolicy.SnakeCaseUpper.ConvertName("XMLParser"));

    // ─── KebabCaseLower ───────────────────────────────────────────────────────────

    [TestMethod]
    public void KebabCaseLower_SimpleWord() =>
        Assert.AreEqual("my-property", RecordNamingPolicy.KebabCaseLower.ConvertName("MyProperty"));

    [TestMethod]
    public void KebabCaseLower_MultiWord() =>
        Assert.AreEqual("my-property-name", RecordNamingPolicy.KebabCaseLower.ConvertName("MyPropertyName"));

    [TestMethod]
    public void KebabCaseLower_Id() =>
        Assert.AreEqual("id", RecordNamingPolicy.KebabCaseLower.ConvertName("Id"));

    [TestMethod]
    public void KebabCaseLower_XMLParser() =>
        Assert.AreEqual("xml-parser", RecordNamingPolicy.KebabCaseLower.ConvertName("XMLParser"));

    // ─── KebabCaseUpper ───────────────────────────────────────────────────────────

    [TestMethod]
    public void KebabCaseUpper_SimpleWord() =>
        Assert.AreEqual("MY-PROPERTY", RecordNamingPolicy.KebabCaseUpper.ConvertName("MyProperty"));

    [TestMethod]
    public void KebabCaseUpper_MultiWord() =>
        Assert.AreEqual("MY-PROPERTY-NAME", RecordNamingPolicy.KebabCaseUpper.ConvertName("MyPropertyName"));

    [TestMethod]
    public void KebabCaseUpper_Id() =>
        Assert.AreEqual("ID", RecordNamingPolicy.KebabCaseUpper.ConvertName("Id"));

    [TestMethod]
    public void KebabCaseUpper_XMLParser() =>
        Assert.AreEqual("XML-PARSER", RecordNamingPolicy.KebabCaseUpper.ConvertName("XMLParser"));
}
