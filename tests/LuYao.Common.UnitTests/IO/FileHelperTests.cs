using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.IO;

[TestClass]
public class FileHelperTests
{
    private string _tempFilePath;
    
    [TestInitialize]
    public void Initialize()
    {
        _tempFilePath = Path.GetTempFileName();
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }
    
    [TestMethod]
    public void IsEmpty_FileDoesNotExist_ReturnsTrue()
    {
        // Arrange
        string nonExistingFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        // Act
        bool result = FileHelper.IsEmpty(nonExistingFilePath);
        
        // Assert
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsEmpty_FileExistsWithZeroLength_ReturnsTrue()
    {
        // Arrange - temp file is created with zero length by default
        
        // Act
        bool result = FileHelper.IsEmpty(_tempFilePath);
        
        // Assert
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsEmpty_FileExistsWithContent_ReturnsFalse()
    {
        // Arrange
        File.WriteAllText(_tempFilePath, "Some content");
        
        // Act
        bool result = FileHelper.IsEmpty(_tempFilePath);
        
        // Assert
        Assert.IsFalse(result);
    }
}
