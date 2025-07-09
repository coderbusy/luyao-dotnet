using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.CharacterFilters;


[TestClass]
public class RemoveAccentsCharacterFilterTests
{
    [TestMethod]
    public void Filter_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        var filter = new RemoveAccentsCharacterFilter();

        // Act
        var result = filter.Filter(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var filter = new RemoveAccentsCharacterFilter();

        // Act
        var result = filter.Filter(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputContainsAccents_ReplacesAccentsWithBaseCharacters()
    {
        // Arrange
        var filter = new RemoveAccentsCharacterFilter();

        // Act
        var result = filter.Filter("ÀÁÂÃÄÅàáâãäå");

        // Assert
        Assert.AreEqual("AAAAAAaaaaaa", result);
    }

    [TestMethod]
    public void Filter_InputContainsNonAccentCharacters_ReturnsOriginalString()
    {
        // Arrange
        var filter = new RemoveAccentsCharacterFilter();

        // Act
        var result = filter.Filter("123ABC");

        // Assert
        Assert.AreEqual("123ABC", result);
    }

    [TestMethod]
    public void Filter_InputContainsMixedCharacters_ReplacesAccentsAndKeepsOthers()
    {
        // Arrange
        var filter = new RemoveAccentsCharacterFilter();

        // Act
        var result = filter.Filter("À123ÁâBCçÑ");

        // Assert
        Assert.AreEqual("A123AaBCcN", result);
    }

    [TestMethod]
    public void Filter_InputContainsPinyinAccents_ReplacesAccentsWithBaseCharacters()
    {
        // Arrange
        var filter = new RemoveAccentsCharacterFilter();

        // Act
        var result = filter.Filter("āǎēěīǐōǒūǔǖǘǚǜ");

        // Assert
        Assert.AreEqual("aaeeiioouuuuuu", result);
    }
}