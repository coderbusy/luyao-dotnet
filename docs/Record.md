# Record

## 1. 目标

`Record` 是面向内存列存储场景的数据容器，强调：

- 实用
- 易用
- 可与 ADO.NET 互操作
- 适合对象映射与轻量查询

`RecordSet` 是多个 `Record` 的命名集合，用于管理多张内存表，并提供与 `DataSet` 的双向互操作。

---

## 2. 当前实现范围

### 2.1 `Record`

当前源码中的 `Record` 已提供以下能力：

- 列存储结构管理：添加列、重命名列、转换列类型、导出 schema。
- 行操作：新增、删除单行、按条件批量删除、清空全部行。
- 行枚举：实现 `IEnumerable<RecordRow>`，支持 `foreach`。
- 查询：按列值、Lambda、`dynamic` 条件查找。
- 对象映射：`From<T>`、`FromList<T>`、`AddRow<T>`、`AddRows<T>`、`ToList<T>`、`To<T>`。
- ADO.NET 互操作：`IDataReader`、`DataTable`。
- 二进制序列化：`WriteTo` / `ReadFrom` / `ToBytes` / `FromBytes`。
- 服务端翻页元数据：`Page`、`PageSize`、`MaxCount`、`MaxPage`。

### 2.2 `RecordSet`

当前源码中的 `RecordSet` 已提供以下能力：

- 按名称管理多个 `Record`。
- 名称比较策略可配置（默认 `StringComparer.Ordinal`）。
- 与 `DataSet` 双向互操作。
- 二进制序列化。

### 2.3 非目标

以下能力当前不属于 `Record` / `RecordSet` 的核心实现：

- JSON、CSV、文本协议层。
- 线程安全容器。
- 数据库查询翻译。

### 2.4 线程安全

`Record`、`RecordRow`、`RecordSet` 都是**非线程安全**类型。多线程场景需要调用方自行同步。

---

## 3. 核心类型

### 3.1 `RecordRow`

`RecordRow` 是一个轻量 `struct`，表示某个 `Record` 中的单行视图。

当前实现包含：

- `Record`：所属 `Record`。
- `Row`：当前行号。
- `implicit operator int`：可隐式转为行号。
- `this[string name]`：按列名读取或写入当前行的值。
- `To<T>(string name)`：按列名读取并转换为目标类型；列不存在时返回 `default`。
- `ToDictionary()`：将当前行转为 `Dictionary<string, object?>`。
- `CopyTo<T>(T data)`：把当前行填充到已有对象。
- `CopyFrom<T>(T data)`：把对象属性复制到当前行。
- `To<T>()`：把当前行转换为新对象。
- `IDynamicMetaObjectProvider`：支持 `dynamic` 成员与索引器读写。

需要注意：当前 `RecordRow` **没有** `Field<T>(string name)` / `Set<T>(string name, value)` API；文档和示例应统一使用索引器、`To<T>(name)`、`CopyFrom`、`CopyTo` 等现有接口。

### 3.2 `RecordColumn` / `RecordColumn<T>`

`RecordColumn` 是列的抽象基类，`RecordColumn<T>` 是泛型实现。

当前实现中常用 API 为：

- 基类：`Get(int row)` / `Set(int row, object? value)`
- 基类：`To<T>(int row)`
- 基类：`Delete(int row)` / `Clear()`
- 泛型列：`GetValue(int row)` / `SetValue(int row, T value)`

说明：

- 列绑定到创建它的 `Record`。
- 列名可通过 `Record.RenameColumn` 或 `Record.Columns.Rename` 修改。
- `CastColumn` 会创建新的列实例替换旧列；旧列引用不再适用于新 schema。

### 3.3 `RecordColumnCollection`

`RecordColumnCollection` 当前实现：

- 实现 `IReadOnlyList<RecordColumn>`。
- 内部使用 `KeyedList<string, RecordColumn>` 维护数据。
- 名称比较使用 `StringComparer.Ordinal`。

常用 API：

| 操作 | 方法 | 行为 |
|------|------|------|
| 宽容查找 | `Find(name)` / `Find<T>(name)` | 不存在返回 `null` |
| 严格查找 | `Get(name)` | 不存在抛 `KeyNotFoundException` |
| 添加 | `Add(name, type)` / `Add<T>(name)` | 同名同类型返回已有列；同名不同类型抛 `InvalidOperationException` |
| 删除 | `Remove(name)` | 不存在返回 `false` |
| 重命名 | `Rename(oldName, newName)` | 重名抛 `DuplicateNameException` |
| 批量按对象建列 | `AddFrom<T>()` 等重载 | 仅添加受支持属性 |

### 3.4 `RecordSchema`

`Record.GetSchema()` 返回 `RecordSchema`，用于导出当前列定义。

每个 `ColumnDef` 包含：

- `Name`
- `ColumnType`
- `IsNullable`
- `Type`

`RecordSchema` 反映的是当前实现使用的列类型编码信息，适合做 schema 比较、调试和序列化相关场景。

---

## 4. 支持的列类型

当前实现支持以下列类型：

| 类别 | 类型 |
|------|------|
| 布尔 | `bool` |
| 有符号整数 | `sbyte`, `short`, `int`, `long` |
| 无符号整数 | `byte`, `ushort`, `uint`, `ulong` |
| 浮点 | `float`, `double`, `decimal` |
| 文本 | `char`, `string` |
| 时间 | `DateTime`, `DateTimeOffset`, `TimeSpan` |
| 标识 | `Guid` |
| 二进制 | `byte[]` |
| 可空值类型 | 上述值类型的 `Nullable<T>` |
| 枚举 | `enum` 及 `Nullable<enum>` |

说明：

- 列类型白名单是封闭的。
- 对象映射只处理白名单内属性。
- 枚举列当前是被接受的，但底层类型信息会按其基础数值类型参与类型码与部分序列化逻辑。

---

## 5. `Record` API 概览

### 5.1 构造与基础属性

常用构造方式：

- `new Record()`
- `new Record(string name)`
- `new Record(string? name, int rows)`

常用属性：

- `Name`
- `Columns`
- `Capacity`
- `Count`
- `IsEmpty`

说明：

- 构造函数中的 `rows` 用于初始容量预估，不是实际行数。
- `Count` 表示当前实际行数。
- `Capacity` 会在增加行时按列容量自动扩展。

### 5.2 行操作

当前实现提供：

- `AddRow()`：新增一行并返回 `RecordRow`
- `Delete(int row)`：删除指定行，越界返回 `false`
- `DeleteWhere(Func<RecordRow, bool>)`：按条件批量删除
- `DeleteRows(IEnumerable<int>)`：按索引集合批量删除
- `ClearRows()`：清空所有行，保留列结构
- `this[int row]`：按索引获取 `RecordRow`
- `GetEnumerator()`：按行号顺序枚举行

### 5.3 Schema 操作

当前实现提供：

- `RenameColumn(string oldName, string newName)`
- `CastColumn(string name, Type newType)`
- `CloneSchema()`
- `Clone()`
- `GetSchema()`

说明：

- `CloneSchema()` 仅复制表名和列结构，不复制行数据，也不复制翻页元数据。
- `Clone()` 复制列结构、全部行数据和翻页元数据。
- `CastColumn()` 会逐行转换旧列值，并替换底层列实例。

### 5.4 服务端翻页元数据

`Record` 当前支持：

- `Page`：当前页，默认 `1`
- `PageSize`
- `MaxCount`
- `MaxPage`

行为说明：

- `MaxCount` 取值大于 0 时返回设置值，否则返回 `Count`。
- `MaxPage` 在 `_maxCount == 0` 时返回 `0`。
- `MaxPage` 在 `PageSize == 0` 时按 `20` 计算。
- 翻页属性不会影响数据操作本身。

### 5.5 查询

当前实现提供：

- `Find<T>(string name, T value)`
- `FindAll<T>(string name, T value)`
- `Find(Func<RecordRow, bool> filter)`
- `FindAll(Func<RecordRow, bool> filter)`
- `FindByDynamic(Func<dynamic, bool> filter)`
- `FindAllByDynamic(Func<dynamic, bool> filter)`
- `Group<T>(string fld)`（扩展方法）

行为说明：

- `Find*` 未找到时返回 `null`。
- `FindAll*` 未找到时返回空序列。
- `FindAll*` 是延迟执行枚举。
- `Group<T>` 按列值聚合并返回 `Dictionary<T, List<RecordRow>>`。
- 当列不存在时：若 `T` 存在非空默认键（如值类型），返回单组（默认键）并包含全部行；若默认键为 `null`（如 `string`），抛 `InvalidOperationException`。

### 5.6 对象映射

当前实现提供：

- `Record.From<T>(T data)`
- `Record.FromList<T>(IEnumerable<T> items)`
- `record.AddRow<T>(T item)`
- `record.AddRows<T>(IEnumerable<T> items)`
- `record.ToList<T>()`
- `record.To<T>()`

同时 `RecordRow` 提供：

- `row.CopyTo<T>(T data)`
- `row.CopyFrom<T>(T data)`
- `row.To<T>()`

重要约束：

- Mapping 路径**不会自动建列**。
- `CopyFrom` / `AddRow<T>` / `AddRows<T>` 写入时，仅对已存在列赋值；缺失列会被静默跳过。
- 建议在写入前先使用 `Columns.AddFrom<T>()` 或手动声明 schema。

### 5.7 ADO.NET 互操作

当前实现提供：

- `void Read(IDataReader dr)`
- `static Record Read(DataTable dt)`
- `void Write(DataTable dt)`
- `DataTable ToDataTable()`

行为说明：

- `Read(IDataReader)` 会先 `Columns.Clear()`，然后用 reader 结构重建整张表。
- `DBNull.Value` 会被跳过，目标列保持默认值。
- `Write(DataTable dt)` 会向传入的 `DataTable` 追加列和行；通常应传入空表。
- `ToDataTable()` 会创建新 `DataTable`，表名取自 `Record.Name`。

### 5.8 二进制序列化

当前实现提供：

- `WriteTo(Stream)` / `WriteTo(BinaryWriter)`
- `ReadFrom(Stream)` / `ReadFrom(BinaryReader)`
- `ToBytes()` / `FromBytes(byte[])`
- `FromStream(Stream)`
- `IsBinaryPayload(byte[])`

当前二进制格式特点：

1. 先写入带类型标识的 payload header。
2. 再写格式版本号。
3. 写 `Name`、`Page`、`PageSize`、`MaxCount`。
4. 写列定义：列名、`RecordColumnType`、`IsNullable`。
5. 写行数。
6. 按列顺序写列数据。

说明：

- `Record` 和 `RecordSet` 使用不同的 payload type。
- `FromBytes` / `ReadFrom` 会校验 payload type 与版本号。
- `Clone()` 会保留翻页元数据，二进制序列化也会保留这些字段。
- 枚举列做二进制往返时，当前反序列化结果会恢复为其基础数值列，而不是枚举列。

---

## 6. `RecordRow` 访问语义

### 6.1 按名称读取

| 路径 | 列不存在时的行为 |
|------|------------------|
| `row["Name"]` | 返回 `null` |
| `row.To<T>("Name")` | 返回 `default(T)` |
| `record.Columns.Get("Name")` | 抛 `KeyNotFoundException` |

### 6.2 按名称写入

| 路径 | 列不存在时的行为 |
|------|------------------|
| `row["Name"] = value` | `value != null` 时按运行时类型自动建列；`value == null` 时跳过 |
| `dynamicRow.Name = value` | 与索引器写入语义一致 |
| `row.CopyFrom(dto)` | 不自动建列；缺失列静默跳过 |

### 6.3 dynamic 行为

`RecordRow` 的 dynamic 支持基于 `this[string]` 索引器实现：

- `d.Foo` 等价于 `row["Foo"]`
- `d.Foo = 1` 等价于 `row["Foo"] = 1`
- `d["Foo"]` / `d["Foo"] = 1` 也受支持

---

## 7. `RecordSet` API 概览

### 7.1 基础管理

当前实现提供：

- `Add(name, record)`
- `Set(name, record)`
- `Get(name)`
- `TryGet(name, out record)`
- `Remove(name)`
- `Contains(name)`
- `Rename(oldName, newName)`
- `Clear()`
- `this[name]`
- `Names`
- `Count`

行为说明：

- `Add` 重名时报 `ArgumentException`。
- `Set` 重名时覆盖。
- `Rename` 会同步更新 `record.Name`。
- 名称比较策略由构造函数中的 `StringComparer` 决定。

### 7.2 枚举顺序

当前 `RecordSet` 内部使用 `SortedDictionary<string, Record>`。

这意味着：

- `Names` 与 `foreach` 的枚举顺序是**按比较器排序后的名称顺序**。
- 当前实现**不是**按添加顺序枚举。

### 7.3 `DataSet` 互操作

当前实现提供：

- `RecordSet.FromDataSet(DataSet ds)`
- `ToDataSet()`
- `WriteTo(DataSet ds)`

说明：

- `FromDataSet` 会把每个 `DataTable` 转成 `Record`，并以 `TableName` 为键。
- 如果写入的名称冲突，`Add` 会抛出异常。
- `WriteTo(DataSet ds)` 会向传入 `DataSet` 追加新表；通常应传入空 `DataSet` 或确保名称不冲突。

### 7.4 二进制序列化

当前实现提供：

- `WriteTo(Stream)` / `WriteTo(BinaryWriter)`
- `ReadFrom(Stream)` / `ReadFrom(BinaryReader)`
- `ToBytes()` / `FromBytes(byte[])`
- `FromStream(Stream)`
- `IsBinaryPayload(byte[])`

`RecordSet` 在序列化时会按键名写出每个 `Record`，以避免 `Record.Name` 被外部修改后与集合键不一致。

---

## 8. 异常与边界行为

| 场景 | 当前行为 |
|------|----------|
| `record[row]` 越界 | 抛 `ArgumentOutOfRangeException` |
| `new RecordRow(record, row)` 越界 | 抛 `ArgumentOutOfRangeException` |
| `Columns.Get(name)` 不存在 | 抛 `KeyNotFoundException` |
| `Columns.Find(name)` 不存在 | 返回 `null` |
| `Record.Delete(row)` 越界 | 返回 `false` |
| `DeleteWhere(null)` | 抛 `ArgumentNullException` |
| `DeleteRows(null)` | 抛 `ArgumentNullException` |
| `RecordSet.Get(name)` 不存在 | 抛 `KeyNotFoundException` |
| `RecordSet.Add(null/空白, record)` | 抛 `ArgumentException` |
| `RecordSet.Add(name, null)` | 抛 `ArgumentNullException` |

说明：

- 当前实现中，“查找”类 API 多使用 `null` / 空序列表示未命中。
- “严格获取”类 API 才抛异常。
- 文档中不应再使用“空表操作总是返回空 `Record`”这类旧约定描述当前实现。

---

## 9. 最佳实践

### 9.1 创建与填充

```csharp
var record = new Record("Orders", expectedRows);
var idCol = record.Columns.Add<int>("Id");
var nameCol = record.Columns.Add<string>("Name");
var amountCol = record.Columns.Add<decimal>("Amount");

for (int i = 0; i < count; i++)
{
    var row = record.AddRow();
    idCol.SetValue(row.Row, i + 1);
    nameCol.SetValue(row.Row, $"Order-{i + 1}");
    amountCol.SetValue(row.Row, amounts[i]);
}
```

要点：

- 优先先建列，再批量加行。
- 热路径优先缓存 `RecordColumn<T>`。
- 强类型写入优先使用 `SetValue`。

### 9.2 读取当前行

```csharp
foreach (var row in record)
{
    int id = row.To<int>("Id");
    string name = row.To<string>("Name");
    object? rawAmount = row["Amount"];
}
```

要点：

- `row.To<T>(name)` 适合容错读取。
- 如果列必须存在，先用 `record.Columns.Get(name)` 做显式校验。

### 9.3 dynamic 与自动建列

```csharp
var record = new Record();
dynamic row = record.AddRow();

row.Id = 100;
row.Name = "abc";
row.Tag = null;
```

要点：

- `Id`、`Name` 会自动建列。
- `Tag = null` 不会建列。
- schema 已知时，仍建议显式 `Columns.Add<T>()`。

### 9.4 对象映射

```csharp
record.Columns.AddFrom<MyDto>();

foreach (var dto in dtos)
{
    record.AddRow(dto);
}

List<MyDto> list = record.ToList<MyDto>();
MyDto first = record.To<MyDto>();
```

要点：

- `AddRow(dto)` 本质上仍然依赖既有列结构。
- DTO 属性存在但列不存在时，该值会被忽略。

### 9.5 Schema 操作

```csharp
record.RenameColumn("OldName", "NewName");
record.CastColumn("Price", typeof(decimal));

var schemaOnly = record.CloneSchema();
var clone = record.Clone();
var schema = record.GetSchema();
```

要点：

- `CloneSchema()` 只复制结构。
- `Clone()` 复制结构、数据和翻页信息。
- `CastColumn()` 后应重新获取列引用。

### 9.6 与 ADO.NET 互操作

```csharp
var record = new Record();
record.Read(dataReader);

var fromTable = Record.Read(dataTable);
DataTable table = fromTable.ToDataTable();

var set = RecordSet.FromDataSet(dataSet);
DataSet ds = set.ToDataSet();
```

### 9.7 RecordSet 管理

```csharp
var set = new RecordSet();
set.Add("Orders", ordersRecord);
set.Set("Customers", customersRecord);

if (set.TryGet("Orders", out var orders))
{
    Console.WriteLine(orders.Count);
}

foreach (var item in set)
{
    Console.WriteLine(item.Name);
}
```

要点：

- 枚举顺序按名称排序，不是按添加顺序。
- 如需不区分大小写，可使用 `new RecordSet(StringComparer.OrdinalIgnoreCase)`。

### 9.8 二进制序列化

```csharp
byte[] bytes = record.ToBytes();
Record copy = Record.FromBytes(bytes);

byte[] setBytes = set.ToBytes();
RecordSet setCopy = RecordSet.FromBytes(setBytes);
```

---

## 10. 当前文档与实现的关键对齐点

为避免继续误导，以下旧说法已不再适用于当前源码：

- `RecordRow.Field<T>(name)` / `Set<T>(name, value)`：当前实现中不存在。
- `RecordColumn.Field` / `SetField`：已重命名为 `GetValue` / `SetValue`。
- `RecordSet` 按添加顺序枚举：当前实现中不成立，实际按名称排序枚举。
- 空表操作统一返回空 `Record`：当前实现并无这一统一规则。
- 枚举列完全按枚举类型做二进制往返：当前实现中会还原为基础数值列。

后续如再调整实现，请优先以 `src/LuYao.Common/Data` 下源码与单元测试为准更新本文。
