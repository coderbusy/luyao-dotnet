using LuYao.Data;
using LuYao.Data.Mapping;
using LuYao.Data.Meta;

namespace LuYao.Data.Meta
{
    [TestClass]
    public class XCopyTests
    {
        #region Test helper types

        private class Base
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            // Unsupported type, should be skipped
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

        // ── MapTo ───────────────────────────────────────────────────────────

        [TestMethod]
        public void CopyTo_Base_WritesBaseProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Base { Id = 1, Name = "Alice" };

            XCopy.MapTo(obj, row);

            Assert.AreEqual(1, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Alice", table.Columns.Get("Name").Get(row));
        }

        [TestMethod]
        public void CopyTo_Derived_WritesBaseAndDerivedProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 2, Name = "Bob", Score = 9.5 };

            XCopy.MapTo(obj, row);

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

            // Passed as base reference; runtime type is still Derived
            XCopy.MapTo(obj, row);

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

            // Name column does not exist; should not throw
            XCopy.MapTo(obj, row);

            Assert.AreEqual(7, table.Columns.Get("Id").Get(row));
        }

        [TestMethod]
        public void CopyTo_NullData_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try
            {
                XCopy.MapTo(null!, row);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException) { }
        }

        // ── MapFrom ─────────────────────────────────────────────────────────

        [TestMethod]
        public void CopyFrom_Base_ReadsBaseProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 10);
            table.Columns.Get("Name").Set(row, "Dave");

            var obj = new Base();
            XCopy.MapFrom(obj, row);

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
            XCopy.MapFrom(obj, row);

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
            XCopy.MapFrom(obj, row);

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
                XCopy.MapFrom(null!, row);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException) { }
        }

        // ── XCopy<T> thin wrapper ──────────────────────────────────────────

        [TestMethod]
        public void Generic_CopyTo_DelegatesToNonGeneric()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 5, Name = "Frank", Score = 3.14 };

            // XCopy<Base> uses compile-time type typeof(Base), so only Base properties are copied
            XCopy<Base>.MapTo(obj, row);

            Assert.AreEqual(5, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Frank", table.Columns.Get("Name").Get(row));
            // Score belongs to Derived, outside typeof(Base) scope; remains default
            Assert.AreEqual(0.0, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void Generic_CopyFrom_DelegatesToNonGeneric()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 99);
            table.Columns.Get("Score").Set(row, 1.23);

            var obj = new Derived();
            // XCopy<Base> uses compile-time type typeof(Base), so only Base properties are written back
            XCopy<Base>.MapFrom(obj, row);

            Assert.AreEqual(99, obj.Id);
            // Score belongs to Derived, outside typeof(Base) scope; remains default
            Assert.AreEqual(0.0, obj.Score);
        }

        // ── WriteTo ────────────────────────────────────────────────────────

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

            // Score column should be created automatically
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

            // Ignored property type object is unsupported; no column should be created and no exception thrown
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

            // XCopy<Base> uses compile-time type typeof(Base), so only Base property columns are created
            XCopy<Base>.WriteTo(obj, row);

            Assert.AreEqual(7, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Frank", table.Columns.Get("Name").Get(row));
            // Score belongs to Derived, outside typeof(Base) scope; column should not be created
            Assert.IsNull(table.Columns.Find("Score"));
        }

        // ── MapTo(Type, ...) explicit type overloads ─────────────────────────────────────

        [TestMethod]
        public void CopyTo_ExplicitBaseType_OnlyWritesBaseProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 10, Name = "Grace", Score = 5.0 };

            // Explicitly specify Base type; Score property should not be written
            XCopy.MapTo(typeof(Base), obj, row);

            Assert.AreEqual(10, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Grace", table.Columns.Get("Name").Get(row));
            Assert.AreEqual(0.0, table.Columns.Get("Score").Get(row));
        }

        [TestMethod]
        public void CopyTo_NullType_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try { XCopy.MapTo(null!, new Base(), row); Assert.Fail("Expected ArgumentNullException"); }
            catch (ArgumentNullException) { }
        }

        [TestMethod]
        public void CopyTo_ExplicitType_NullData_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try { XCopy.MapTo(typeof(Base), null!, row); Assert.Fail("Expected ArgumentNullException"); }
            catch (ArgumentNullException) { }
        }

        // ── MapFrom(Type, ...) explicit type overloads ───────────────────────────────────

        [TestMethod]
        public void CopyFrom_ExplicitBaseType_OnlyReadsBaseProps()
        {
            var table = BuildTable();
            var row = table.AddRow();
            table.Columns.Get("Id").Set(row, 20);
            table.Columns.Get("Name").Set(row, "Hank");
            table.Columns.Get("Score").Set(row, 4.5);

            var obj = new Derived();
            // Explicitly specify Base type; Score property should not be written back
            XCopy.MapFrom(typeof(Base), obj, row);

            Assert.AreEqual(20, obj.Id);
            Assert.AreEqual("Hank", obj.Name);
            Assert.AreEqual(0.0, obj.Score);
        }

        [TestMethod]
        public void CopyFrom_NullType_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try { XCopy.MapFrom(null!, new Base(), row); Assert.Fail("Expected ArgumentNullException"); }
            catch (ArgumentNullException) { }
        }

        [TestMethod]
        public void CopyFrom_ExplicitType_NullData_ThrowsArgumentNullException()
        {
            var table = BuildTable();
            var row = table.AddRow();

            try { XCopy.MapFrom(typeof(Base), null!, row); Assert.Fail("Expected ArgumentNullException"); }
            catch (ArgumentNullException) { }
        }

        // ── WriteTo(Type, ...) explicit type overloads ───────────────────────────────────

        [TestMethod]
        public void WriteTo_ExplicitBaseType_OnlyCreatesBaseColumns()
        {
            var table = new RecordTable();
            var row = table.AddRow();
            var obj = new Derived { Id = 30, Name = "Ivy", Score = 2.0 };

            // Explicitly specify Base type; Score column should not be created
            XCopy.WriteTo(typeof(Base), obj, row);

            Assert.AreEqual(30, table.Columns.Get("Id").Get(row));
            Assert.AreEqual("Ivy", table.Columns.Get("Name").Get(row));
            Assert.IsNull(table.Columns.Find("Score"));
        }

        [TestMethod]
        public void WriteTo_NullType_ThrowsArgumentNullException()
        {
            var table = new RecordTable();
            var row = table.AddRow();

            try { XCopy.WriteTo(null!, new Base(), row); Assert.Fail("Expected ArgumentNullException"); }
            catch (ArgumentNullException) { }
        }

        [TestMethod]
        public void WriteTo_ExplicitType_NullData_ThrowsArgumentNullException()
        {
            var table = new RecordTable();
            var row = table.AddRow();

            try { XCopy.WriteTo(typeof(Base), null!, row); Assert.Fail("Expected ArgumentNullException"); }
            catch (ArgumentNullException) { }
        }

        // ── Cache consistency ────────────────────────────────────────────────────────────

        [TestMethod]
        public void CopyTo_CalledTwice_ProducesConsistentResults()
        {
            var table = BuildTable();
            var row1 = table.AddRow();
            var row2 = table.AddRow();
            var obj = new Base { Id = 40, Name = "Jack" };

            XCopy.MapTo(obj, row1);
            XCopy.MapTo(obj, row2);

            Assert.AreEqual(40, table.Columns.Get("Id").Get(row1));
            Assert.AreEqual(40, table.Columns.Get("Id").Get(row2));
        }
    }
}
