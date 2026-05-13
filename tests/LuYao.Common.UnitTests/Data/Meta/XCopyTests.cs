using LuYao.Data;
using LuYao.Data.Meta;

namespace LuYao.Data.Meta
{
    [TestClass]
    public class XCopyTests
    {
        #region 测试用类型

        private class Base
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            // 不支持的类型，应被跳过
            public object? Ignored { get; set; }
        }

        private class Derived : Base
        {
            public double Score { get; set; }
        }

        private static RecordTable BuildTable()
        {
            var t = new RecordTable();
            t.Columns.Add<int>("Id");
            t.Columns.Add<string>("Name");
            t.Columns.Add<double>("Score");
            return t;
        }

        #endregion

        // ── CopyTo ──────────────────────────────────────────────────────────

        [TestMethod]
        public void CopyTo_Base_WritesBaseProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Base { Id = 1, Name = "Alice" };

            XCopy.CopyTo(obj, row);

            Assert.AreEqual(1, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Alice", table.Columns.Get("Name").Get(row));
        }

        [TestMethod]
        public void CopyTo_Derived_WritesBaseAndDerivedProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 2, Name = "Bob", Score = 9.5 };

            XCopy.CopyTo(obj, row);

            Assert.AreEqual(2, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Bob", table.Columns.Get("Name").Get(row));
            Assert.AreEqual(9.5, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void CopyTo_DerivedViaBaseRef_WritesDerivedProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            Base obj = new Derived { Id = 3, Name = "Carol", Score = 8.0 };

            // 以基类引用传入，运行时类型仍是 Derived
            XCopy.CopyTo(obj, row);

            Assert.AreEqual(3, table.Columns.Get("Id").Get(row));
            Assert.AreEqual(8.0, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void CopyTo_MissingColumn_SilentlySkips()
        {
            var table = new RecordTable();
            table.Columns.Add<int>("Id");
            var row = table.AddRow();
            var obj = new Base { Id = 7, Name = "X" };

            // Name 列不存在，不应抛异常
            XCopy.CopyTo(obj, row);

            Assert.AreEqual(7, table.Columns.Get("Id").Get(row));
        }

        [TestMethod]
        public void CopyTo_NullData_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try
            {
                XCopy.CopyTo(null!, row);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException) { }
        }

        // ── CopyFrom ─────────────────────────────────────────────────────────

        [TestMethod]
        public void CopyFrom_Base_ReadsBaseProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 10);
            table.Columns.Get("Name").Set(row, "Dave");

            var obj = new Base();
            XCopy.CopyFrom(obj, row);

            Assert.AreEqual(10, obj.Id);
            Assert.AreEqual("Dave", obj.Name);
        }

        [TestMethod]
        public void CopyFrom_Derived_ReadsBaseAndDerivedProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 20);
            table.Columns.Get("Name").Set(row, "Eve");
            table.Columns.Get("Score").Set(row, 7.5);

            var obj = new Derived();
            XCopy.CopyFrom(obj, row);

            Assert.AreEqual(20, obj.Id);
            Assert.AreEqual("Eve", obj.Name);
            Assert.AreEqual(7.5, obj.Score);
        }

        [TestMethod]
        public void CopyFrom_DerivedViaBaseRef_SetsDerivedProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 30);
            table.Columns.Get("Score").Set(row, 6.0);

            Base obj = new Derived();
            XCopy.CopyFrom(obj, row);

            Assert.AreEqual(30, ((Derived)obj).Id);
            Assert.AreEqual(6.0, ((Derived)obj).Score);
        }

        [TestMethod]
        public void CopyFrom_NullData_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try
            {
                XCopy.CopyFrom(null!, row);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException) { }
        }

        // ── XCopy<T> 薄封装 ──────────────────────────────────────────────────

        [TestMethod]
        public void Generic_CopyTo_DelegatesToNonGeneric()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 5, Name = "Frank", Score = 3.14 };

            XCopy<Base>.CopyTo(obj, row);

            Assert.AreEqual(5, table.Columns.Get("Id").Get(row));
            Assert.AreEqual(3.14, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void Generic_CopyFrom_DelegatesToNonGeneric()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 99);
            table.Columns.Get("Score").Set(row, 1.23);

            var obj = new Derived();
            XCopy<Base>.CopyFrom(obj, row);

            Assert.AreEqual(99, obj.Id);
            Assert.AreEqual(1.23, obj.Score);
        }

        // ── WriteTo ──────────────────────────────────────────────────────────

        [TestMethod]
        public void WriteTo_Base_CreatesColumnsAndWritesValues()
        {
            var table = new RecordTable();
            var row = table.AddRow();
            var obj = new Base { Id = 1, Name = "Alice" };

            XCopy.WriteTo(obj, row);

            Assert.AreEqual(1, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Alice", table.Columns.Get("Name").Get(row));
        }

        [TestMethod]
        public void WriteTo_Derived_CreatesAllColumnsIncludingDerivedProps()
        {
            var table = new RecordTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 2, Name = "Bob", Score = 9.5 };

            XCopy.WriteTo(obj, row);

            Assert.AreEqual(2, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Bob", table.Columns.Get("Name").Get(row));
            Assert.AreEqual(9.5, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void WriteTo_DerivedViaBaseRef_CreatesDerivedColumns()
        {
            var table = new RecordTable();
            var row = table.AddRow();
            Base obj = new Derived { Id = 3, Name = "Carol", Score = 8.0 };

            XCopy.WriteTo(obj, row);

            // Score 列应被自动创建
            Assert.IsNotNull(table.Columns.Find("Score"));
            Assert.AreEqual(8.0, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void WriteTo_ExistingColumns_OverwritesValues()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 99);

            var obj = new Base { Id = 42, Name = "New" };
            XCopy.WriteTo(obj, row);

            Assert.AreEqual(42, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("New", table.Columns.Get("Name").Get(row));
        }

        [TestMethod]
        public void WriteTo_UnsupportedTypeProperty_IsSkipped()
        {
            var table = new RecordTable();
            var row = table.AddRow();
            var obj = new Base { Id = 5, Name = "Eve", Ignored = new object() };

            // Ignored 属性类型 object 不受支持，不应建列也不应抛异常
            XCopy.WriteTo(obj, row);

            Assert.IsNull(table.Columns.Find("Ignored"));
            Assert.AreEqual(5, table.Columns.Get("Id").Get(row));
        }

        [TestMethod]
        public void WriteTo_NullData_ThrowsArgumentNullException()
        {
            var table = new RecordTable();
            var row = table.AddRow();

            try
            {
                XCopy.WriteTo(null!, row);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException) { }
        }

        [TestMethod]
        public void Generic_WriteTo_DelegatesToNonGeneric()
        {
            var table = new RecordTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 7, Name = "Frank", Score = 3.14 };

            XCopy<Base>.WriteTo(obj, row);

            Assert.IsNotNull(table.Columns.Find("Score"));
            Assert.AreEqual(3.14, table.Columns.Get("Score").Get(row));
        }
    }
}
