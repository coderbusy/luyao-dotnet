# Frame

## 1. 定位

`Frame` 是一个面向内存场景的列存储数据容器，适合：

- 轻量表结构数据处理
- 对象列表与表结构之间转换
- 与 ADO.NET 的 `IDataReader` / `DataTable` / `DataSet` 互操作
- 需要按列存储、按行访问的业务代码

`FrameSet` 是多个 `Frame` 的命名集合，用于统一管理多张内存表，并提供与 `DataSet` 的双向互操作。

设计重点：

- API 简单直接
- 类型白名单明确
- 与对象映射、ADO.NET 互通
- 支持二进制序列化

`Frame`、`FrameRow`、`FrameSet` 都不是线程安全容器。多线程场景需要调用方自行同步。

---

## 2. 当前实现范围

### 2.1 `Frame`

`Frame` 提供：

- 列结构管理：添加列、重命名列、类型转换、导出 schema
- 行管理：新增、删除、批量删除、清空、按索引访问
- 行枚举：实现 `IEnumerable<FrameRow>`
- 查询：按列值、Lambda、`dynamic` 条件查找
- 分组：单字段、多字段字符串分组，以及 2/3 字段元组分组
- 对象映射：对象/对象集合与 `Frame` 之间转换
- ADO.NET 互操作：`IDataReader`、`DataTable`、`DataSet`
- 二进制序列化：`Frame` / `FrameSet` 都支持字节流读写
- 服务端翻页元数据：`Page`、`PageSize`、`MaxCount`、`MaxPage`

### 2.2 `FrameSet`

`FrameSet` 提供：

- 按名称管理多个 `Frame`
- 名称比较器可配置
- 与 `DataSet` 双向互操作
- 二进制序列化

### 2.3 非目标

不包含以下能力：

- SQL 翻译
- JSON / CSV / 文本协议层
- 线程安全容器
- 类似 ORM 的复杂关系映射

---

## 3. 核心类型

### 3.1 `Frame`

常用构造：

- `new Frame()`
- `new Frame(string name)`
- `new Frame(string? name, int rows)`

常用属性：

- `Name`：表名
- `Columns`：`FrameColumnCollection`
- `Capacity`：当前容量
- `Count`：实际行数
- `IsEmpty`：是否空表
- `Page`：当前页，默认 `1`
- `PageSize`：每页数量
- `MaxCount`：总条数；未显式设置时返回 `Count`
- `MaxPage`：总页数；当 `_maxCount == 0` 时返回 `0`，当 `PageSize == 0` 时按 `20` 计算

说明：

- 构造参数 `rows` 只是初始容量预估，不是实际行数。
- 构造时容量最小会提升到 `20`。
- `AddRow()` 才会真正增加 `Count`。
- `CloneSchema()` 只复制结构，不复制分页元数据。
- `Clone()` 会复制结构、全部数据以及分页元数据。

### 3.2 `FrameRow`

`FrameRow` 是一个轻量 `struct`，表示某个 `Frame` 中的单行视图。

`FrameRow` 提供：

- `Frame`：所属 `Frame`
- `Row`：当前行号
- `implicit operator int`
- `this[string name]`：按列名读写当前行值
- `To<T>(string name)`：按列名读取并转换类型
- `ToDictionary()`：当前行转字典
- `CopyTo<T>(T data)`：当前行写入已有对象
- `CopyFrom<T>(T data)`：对象属性写入当前行
- `To<T>()`：当前行转新对象
- `ToString(string? name)`：按列名获取当前行该列的字符串值
- `IDynamicMetaObjectProvider`：支持 `dynamic` 成员读写

访问语义：

| 路径 | 列不存在时行为 |
|------|----------------|
| `row["Name"]` 读取 | 返回 `null` |
| `row.To<T>("Name")` | 返回 `default` |
| `row["Name"] = value` | `value != null` 时尝试按运行时类型自动建列；`value == null` 时忽略 |
| `dynamicRow.Name = value` | 与索引器写入语义一致 |

补充说明：

- `ToString(string? name)`：若 `name` 为 `null`/空白或列不存在，返回空字符串；否则等同于调用对应列的 `FrameColumn.ToString(int row)`。
- 索引器读取时，如果列名为 `null`、空字符串或空白字符串，也直接返回 `null`。
- 索引器写入时，如果列不存在且值非 `null`，会调用 `Columns.Add(name, value.GetType())` 自动建列；如果运行时类型不在支持白名单内，会抛出异常。
- `FrameRow` 的公开获取方式通常是 `record[row]` 或通过枚举 `foreach (var row in record)`。

### 3.3 `FrameColumn` / `FrameColumn<T>`

`FrameColumn` 是列的抽象基类，`FrameColumn<T>` 是泛型实现。

常用成员：

- `Name`
- `Type`
- `ColumnType`
- `IsNullable`
- `Capacity`
- `Get(int row)` / `Set(int row, object? value)`
- `To<T>(int row)`
- `ToString(int row)`：获取指定行列值的字符串表示；`null` 值返回空字符串
- `Delete(int row)` / `Clear()`
- `GetValue(int row)` / `SetValue(int row, T value)`（泛型列）

说明：

- 列绑定到所属 `Frame`。
- 列名可通过 `Frame.RenameColumn` 或 `Frame.Columns.Rename` 修改。
- `CastColumn()` 会创建新列替换旧列，旧列引用应视为失效。
- 列删除行时会把后续数据左移。

### 3.4 `FrameColumnCollection`

`Frame.Columns` 的类型是 `FrameColumnCollection`，其特性如下：

- 实现 `IReadOnlyList<FrameColumn>`
- 内部使用 `KeyedList<string, FrameColumn>` 存储
- 名称比较使用 `StringComparer.Ordinal`

常用 API：

| 操作 | 方法 | 行为 |
|------|------|------|
| 查找 | `Find(name)` | 不存在返回 `null` |
| 严格获取 | `Get(name)` | 不存在抛 `KeyNotFoundException` |
| 泛型查找 | `Find<T>(name)` | 不存在返回 `null`；存在但类型不匹配抛 `InvalidCastException` |
| 判断存在 | `Contains(name)` | 返回 `bool` |
| 查索引 | `IndexOf(name)` | 不存在返回 `-1` |
| 添加 | `Add(name, type)` / `Add<T>(name)` | 同名同类型返回已有列；同名不同类型抛 `InvalidOperationException` |
| 删除 | `Remove(name)` | 不存在返回 `false` |
| 清空 | `Clear()` | 清空列并把 `Frame.Count` 重置为 `0` |
| 重命名 | `Rename(oldName, newName)` | 新名重复时抛 `DuplicateNameException` |
| 按对象建列 | `AddFrom<T>()` 及相关重载 | 仅添加受支持属性 |

`AddFrom<T>()` 当前支持这些重载：

- `AddFrom<T>()`
- `AddFrom<T>(T template)`
- `AddFrom<T>(string[] names)`
- `AddFrom<T>(Action<NameFilter<T>> filter)`

其中：

- 只处理“可读 + 类型受支持”的属性
- 指定属性名时，不存在或不支持的属性会被忽略
- `filter` 方式本质上最终也是转成属性名列表后再加列

### 3.5 `FrameSchema`

`Frame.GetSchema()` 返回 `FrameSchema`，用于导出列定义。

每个 `ColumnDef` 包含：

- `Name`
- `ColumnType`
- `IsNullable`
- `Type`

适用场景：

- schema 比较
- 调试与诊断
- 序列化相关元信息输出

### 3.6 `FrameSet`

`FrameSet` 是 `Frame` 的命名集合，实现 `IEnumerable<Frame>`。

它提供：

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

说明：

- 内部使用 `SortedDictionary<string, Frame>`。
- 枚举顺序与 `Names` 顺序都是**按比较器排序后的名称顺序**，不是按添加顺序。
- `Add` / `Set` / `Rename` 都会同步更新 `record.Name`。

---

## 4. 支持的列类型

支持以下列类型：

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
- `enum` 在内部会按其基础数值类型参与列类型映射。
- 对象映射与自动建列都受该白名单限制。

---

## 5. `Frame` API 概览

### 5.1 建列与加行

最常见的用法是先建列，再加行：

```csharp
var record = new Frame("Orders", 1000);

var idCol = record.Columns.Add<int>("Id");
var nameCol = record.Columns.Add<string>("Name");
var amountCol = record.Columns.Add<decimal>("Amount");

var row = record.AddRow();
idCol.SetValue(row.Row, 1);
nameCol.SetValue(row.Row, "Order-1");
amountCol.SetValue(row.Row, 99.5m);
```

提供：

- `AddRow()`：新增一行并返回 `FrameRow`
- `AddRowFromValues(params object[] values)`：按列顺序填值，超出列数的值会被忽略

```csharp
var record = new Frame();
record.Columns.Add<int>("Id");
record.Columns.Add<string>("Name");

record.AddRowFromValues(1, "A");
record.AddRowFromValues(2, "B", "Ignored");
```

### 5.2 删除与清空

提供：

- `Delete(int row)`：删除指定行，越界返回 `false`
- `DeleteWhere(Func<FrameRow, bool>)`：按条件批量删除
- `DeleteRows(IEnumerable<int>)`：按索引集合批量删除
- `ClearRows()`：清空全部行，保留列结构

行为说明：

- `DeleteWhere` 从后向前删除，避免索引偏移。
- `DeleteRows` 会先去重，再按从大到小顺序删除。
- `ClearRows()` 会清空每列数据并把 `Count` 设为 `0`。

### 5.3 行访问与枚举

提供：

- `record[row]`
- `foreach (var row in record)`
- `GetEnumerator()`

行为说明：

- `record[row]` 越界时抛 `ArgumentOutOfRangeException`
- 枚举时按 `0..Count-1` 顺序返回 `FrameRow`

### 5.4 查询

提供：

- `Find<T>(string name, T value)`
- `FindAll<T>(string name, T value)`
- `Find(Func<FrameRow, bool> filter)`
- `FindAll(Func<FrameRow, bool> filter)`
- `FindByDynamic(Func<dynamic, bool> filter)`
- `FindAllByDynamic(Func<dynamic, bool> filter)`

行为说明：

- `Find*` 未命中时返回 `null`
- `FindAll*` 未命中时返回空序列
- `FindAll*` 使用 `yield return`，属于延迟执行枚举
- `Find<T>(name, value)` 中列不存在时直接返回 `null`
- `FindAll<T>(name, value)` 中列不存在时直接返回空序列
- 以委托过滤的 `Find` / `FindAll` 在 `filter == null` 时抛 `ArgumentNullException`

示例：

```csharp
var first = record.Find<int>("Id", 100);

var largeOrders = record.FindAll(r => r.To<decimal>("Amount") > 1000m);

var active = record.FindByDynamic(d => d.Status == "Active");
```

### 5.5 分组

当前实现提供：

- `Group<T>(string fld)`，其中 `T : struct`
- `Group(string fld)`
- `Group(params string[] flds)`
- `Group<T1, T2>(string fld1, string fld2)`
- `Group<T1, T2, T3>(string fld1, string fld2, string fld3)`

#### 5.5.1 单字段分组

```csharp
var groups = record.Group("Category");
foreach (var pair in groups)
{
    Console.WriteLine($"{pair.Key}: {pair.Value.Count}");
}
```

说明：

- `Group<T>(string fld)` 的返回类型是 `IDictionary<T, List<FrameRow>>`
- `Group(string fld)` 的返回类型是 `IDictionary<string, IList<FrameRow>>`
- `Group<T>(string fld)` 仅支持值类型键；字符串键请使用 `Group(string fld)`

#### 5.5.2 多字段字符串分组

```csharp
var groups = record.Group("Category", "Status");
```

说明：

- 键使用 `-` 连接多个字段转换后的字符串值
- 某列不存在时，该列会按空字符串参与拼接

#### 5.5.3 元组分组

在 `NETSTANDARD2.0+` / `NET6.0+` 目标下，还提供 2～3 字段元组分组：

- `Group<T1, T2>(fld1, fld2)` -> `IDictionary<(T1?, T2?), IList<FrameRow>>`
- `Group<T1, T2, T3>(fld1, fld2, fld3)` -> `IDictionary<(T1?, T2?, T3?), IList<FrameRow>>`

```csharp
var groups = record.Group<int, int>("Year", "Month");
```

实际行为要点：

- 分组 API 会遍历当前所有行并立即构建字典，不是延迟执行。
- 如果指定列不存在，不会返回空字典；缺失列会按默认值参与分组：
  - `Group<T>(fld)`：缺失列键为 `default(T)`
  - `Group(string fld)`：缺失列键为 `string.Empty`
  - 元组分组：缺失列对应分量为 `default`

### 5.6 Schema 操作

当前实现提供：

- `RenameColumn(string oldName, string newName)`
- `CastColumn(string name, Type newType)`
- `CloneSchema()`
- `Clone()`
- `GetSchema()`

行为说明：

- `RenameColumn` 实际转发到 `Columns.Rename`
- `CastColumn` 会逐行转换旧值，然后用新列替换原列
- `CastColumn` 中列不存在时抛 `KeyNotFoundException`
- `CastColumn` 中 `newType == null` 时抛 `ArgumentNullException`
- `CloneSchema()` 复制表名和列结构，不复制数据，也不复制分页元数据
- `Clone()` 复制表名、结构、全部行数据和分页元数据

### 5.7 `ToString()`

会输出调试友好的文本内容：

- 先输出表名、行数、列数
- 当仅有一行时，以“列名 | 值”的方式逐列输出
- 多行时以表格形式输出
- 对宽字符做显示宽度处理
- 单元格内容过长时按显示宽度截断
- 单元格字符串值统一通过 `FrameColumn.ToString(int row)` 获取（`null` 渲染为空字符串）

这个输出更适合调试、日志和快速查看数据，而不是稳定的序列化格式。

---

## 6. 对象映射

### 6.1 `Frame` 级别映射

提供：

- `Frame.From<T>(T data)`
- `Frame.FromList<T>(IEnumerable<T> items)`
- `record.AddRowFrom<T>(T item)`
- `record.AddRowsFromList<T>(IEnumerable<T> items)`
- `record.ToList<T>()`
- `record.To<T>()`

示例：

```csharp
var record = Frame.From(new OrderDto
{
    Id = 1,
    Name = "Order-1"
});

List<OrderDto> list = record.ToList<OrderDto>();
OrderDto first = record.To<OrderDto>();
```

### 6.2 `FrameRow` 级别映射

提供：

- `row.CopyTo<T>(T data)`
- `row.CopyFrom<T>(T data)`
- `row.To<T>()`

示例：

```csharp
var row = record[0];
var dto = row.To<OrderDto>();

var existing = new OrderDto();
row.CopyTo(existing);
```

### 6.3 映射规则

实现上的关键点：

- `Frame.From<T>` / `FromList<T>` 会先 `Columns.AddFrom<T>()`，再写入数据
- `AddRowFrom<T>` 本身**不会自动建列**，它只是新增一行后执行 `CopyFrom`
- `CopyFrom` / `AddRowFrom` / `AddRowsFromList` 写入时，仅对已存在列赋值；缺失列会被忽略
- 对象映射只处理支持白名单中的属性类型

因此，若不是使用 `Frame.From<T>` / `FromList<T>`，通常应先显式建列：

```csharp
var record = new Frame();
record.Columns.AddFrom<OrderDto>();
record.AddRowFrom(dto);
```

---

## 7. ADO.NET 互操作

### 7.1 `Frame` 与 `IDataReader` / `DataTable`

提供：

- `void Read(IDataReader dr)`
- `static Frame Read(DataTable dt)`
- `void Write(DataTable dt)`
- `DataTable ToDataTable()`

行为说明：

- `Read(IDataReader)` 会先 `Columns.Clear()`，然后根据 reader 的字段结构重建整张表
- `Read(IDataReader)` 读取到 `DBNull.Value` 时会跳过赋值，目标列保持默认值
- `Write(DataTable dt)` 会把当前列结构和所有行写入到传入 `DataTable`
- `ToDataTable()` 会创建新表，表名取 `Frame.Name`

示例：

```csharp
var record = new Frame();
record.Read(dataReader);

var fromTable = Frame.Read(dataTable);
DataTable table = fromTable.ToDataTable();
```

补充说明：

- `Write(DataTable dt)` 是向目标表追加列和行，通常应传入空表。

### 7.2 `FrameSet` 与 `DataSet`

提供：

- `FrameSet.FromDataSet(DataSet ds)`
- `ToDataSet()`
- `WriteTo(DataSet ds)`

示例：

```csharp
var set = FrameSet.FromDataSet(dataSet);
DataSet ds = set.ToDataSet();
```

行为说明：

- `FromDataSet` 会把每个 `DataTable` 转成 `Frame`
- `WriteTo(DataSet ds)` 会把每个 `Frame` 作为新表追加到目标 `DataSet`
- 目标 `DataSet` 中表名冲突时，`DataSet.Tables.Add` 会抛异常，因此通常应传入空 `DataSet` 或自行保证不重名

---

## 8. 二进制序列化

### 8.1 `Frame`

提供：

- `WriteTo(Stream)` / `WriteTo(BinaryWriter)`
- `ReadFrom(Stream)` / `ReadFrom(BinaryReader)`
- `ToBytes()` / `FromBytes(byte[])`
- `FromStream(Stream)`
- `IsBinaryPayload(byte[])`

当前格式大致包含：

1. payload header
2. 格式版本号
3. `Name`、`Page`、`PageSize`、`MaxCount`
4. 列定义：列名、`FrameColumnType`、`IsNullable`
5. 行数
6. 每列的顺序数据

说明：

- `ReadFrom` 会校验 payload type 和版本号
- 分页元数据会参与序列化和反序列化
- `string`、`byte[]`、可空列都会写入是否有值的标记
- 枚举值写入时会转成基础数值类型；反序列化后恢复的是基础数值列，而不是原始枚举列

示例：

```csharp
byte[] bytes = record.ToBytes();
Frame copy = Frame.FromBytes(bytes);
```

### 8.2 `FrameSet`

当前实现提供：

- `WriteTo(Stream)` / `WriteTo(BinaryWriter)`
- `ReadFrom(Stream)` / `ReadFrom(BinaryReader)`
- `ToBytes()` / `FromBytes(byte[])`
- `FromStream(Stream)`
- `IsBinaryPayload(byte[])`

说明：

- `FrameSet` 序列化时会先写自己的 payload header 与版本号
- 内部逐个写出每个 `Frame`
- 以字典键作为权威名称，避免外部修改 `Frame.Name` 后与集合键不一致

```csharp
byte[] bytes = set.ToBytes();
FrameSet copy = FrameSet.FromBytes(bytes);
```

---

## 9. 异常与边界行为

行为如下：

| 场景 | 当前行为 |
|------|----------|
| `record[row]` 越界 | 抛 `ArgumentOutOfRangeException` |
| `Columns.Get(name)` 不存在 | 抛 `KeyNotFoundException` |
| `Columns.Find(name)` 不存在 | 返回 `null` |
| `Columns.Find<T>(name)` 类型不匹配 | 抛 `InvalidCastException` |
| `Columns.Add(name, type)` 同名同类型 | 返回已有列 |
| `Columns.Add(name, type)` 同名不同类型 | 抛 `InvalidOperationException` |
| `Columns.Rename(old, new)` 新名已存在 | 抛 `DuplicateNameException` |
| `Frame.Delete(row)` 越界 | 返回 `false` |
| `DeleteWhere(null)` | 抛 `ArgumentNullException` |
| `DeleteRows(null)` | 抛 `ArgumentNullException` |
| `Find(null)` / `FindAll(null)` | 抛 `ArgumentNullException` |
| `FindByDynamic(null)` / `FindAllByDynamic(null)` | 抛 `ArgumentNullException` |
| `row["Missing"]` | 返回 `null` |
| `row.To<T>("Missing")` | 返回 `default` |
| `row["New"] = null` | 不建列，直接忽略 |
| `FrameSet.Get(name)` 不存在 | 抛 `KeyNotFoundException` |
| `FrameSet.Add(null/空白, record)` | 抛 `ArgumentException` |
| `FrameSet.Add(name, null)` | 抛 `ArgumentNullException` |

特别说明：

- `Group*` 缺少列时不会返回空字典，而是使用默认键把现有行分组。
- `Write(DataTable dt)` / `WriteTo(DataSet ds)` 通常应写入空目标对象，避免名称或列结构冲突。

---

## 10. 最佳实践

### 10.1 性能敏感场景优先缓存列对象

```csharp
var idCol = record.Columns.Add<int>("Id");
var nameCol = record.Columns.Add<string>("Name");

for (int i = 0; i < count; i++)
{
    var row = record.AddRow();
    idCol.SetValue(row.Row, i + 1);
    nameCol.SetValue(row.Row, names[i]);
}
```

要点：

- 热路径优先使用 `FrameColumn<T>.SetValue` / `GetValue`
- 避免在循环中反复按名称查列

### 10.2 schema 已知时优先先建列

```csharp
record.Columns.Add<int>("Id");
record.Columns.Add<string>("Name");
```

要点：

- `row["Name"] = value` 虽然支持自动建列，但更适合快速脚本式写法
- 正式业务代码中，显式建列更清晰，也更容易控制列类型

### 10.3 对象映射前先确认 schema

```csharp
record.Columns.AddFrom<OrderDto>();
record.AddRowFrom(dto);
```

要点：

- `AddRowFrom` 不自动补齐缺失列
- DTO 中有属性但表中缺列时，该值会被忽略

### 10.4 与 ADO.NET 互操作时尽量使用新对象

```csharp
DataTable dt = record.ToDataTable();
DataSet ds = set.ToDataSet();
```

要点：

- 这样最不容易与已有列名、表名发生冲突

---

后续如源码有调整，请同步更新本文，并以 `src/LuYao.Common/Data/` 下实现与单元测试为准。
