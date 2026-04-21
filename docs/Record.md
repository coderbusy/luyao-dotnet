# Record

## 1. 目标

`Record` 类型聚焦于内存数据处理能力，面向"列存储"场景，强调：

- 实用
- 易用
- 可扩展

`RecordSet` 作为多表容器，用于组织和管理多个 `Record`。

---

## 2. 设计边界

### 2.1 `Record` 核心职责

- 负责内存中的表结构与数据处理。
- 保留与 `IDataReader`、`DataTable` 的互操作。
- 支持对象映射（`AddRow<T>`、`AddRows<T>`、`ToList<T>`、`To<T>`）。
- 支持二进制序列化（`WriteTo` / `ReadFrom`）。
- 支持服务端翻页元数据（`Page`、`PageSize`、`MaxCount`、`MaxPage`）。

### 2.2 `RecordSet` 核心职责

- 负责多个 `Record` 的命名管理。
- 保持与 `DataSet` 的双向互操作能力。

### 2.3 非核心职责

- JSON、文本等协议能力通过扩展模块提供（如扩展方法或独立包）。

### 2.4 线程安全

- `Record`、`RecordSet` 均为**非线程安全**类型（与 `DataTable` 一致）。
- 多线程场景下，调用方需自行同步。

---

## 3. 核心类型

### 3.1 `RecordRow`

`RecordRow` 是用户最常接触的类型之一，代表 `Record` 中的一行数据视图。

- 定义为 `struct`，持有所属 `Record` 引用与行索引（`Row`）。
- 提供类型安全的数据读取：`Field<T>(string name)`（命名对齐 `System.Data.DataRowExtensions.Field<T>`，便于与 `DataTable` 互操作的用户迁移）。
- 提供按列名写入：`Set<T>(string name, T value)`。**写入时若列不存在会按 `T` 自动建列**；同名列已存在但类型不一致时抛 `InvalidOperationException`。
- 支持隐式转换为 `int`（返回行索引）。
- 实现 `IDynamicMetaObjectProvider`：`dynamic` 成员/索引读取在列不存在时返回 `null`；写入时按 `value.GetType()` **自动建列**，若 `value` 为 `null` 且列不存在则跳过该次写入（无法推断列类型）。
- 作为 `Where` 谓词参数、`foreach` 遍历结果等场景的核心类型。

### 3.2 `RecordColumn` / `RecordColumn<T>`

`RecordColumn` 是列的抽象基类，`RecordColumn<T>` 是泛型实现。

- 每个列持有所属 `Record` 引用、列名（`Name`）和数据类型（`Type`）。列名可通过 `Record.RenameColumn` 修改。
- 数据以数组 `T[]` 形式列存储，支持自动扩容。
- 提供按行索引读写值：`GetValue(int row)` / `SetValue(object? value, int row)`。
- 泛型列额外提供强类型读写：`Get(int row)` / `Set(T value, int row)`。

### 3.4 列类型白名单

`RecordColumn<T>` 的类型参数 `T` 仅允许以下封闭白名单中的类型。调用 `Columns.Add<T>()` 或 `Columns.Add(name, type)` 时，若类型不在白名单内，抛出 `NotSupportedException`。

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
| 可空 | 以上所有值类型的 `Nullable<T>` |

#### 设计约束

- **白名单封闭，不可外部扩展。** 不提供 TypeHandler 或类型注册机制。
- 白名单覆盖 `IDataReader.GetFieldType()` 可能返回的所有常见类型，确保 `DataTable` / `DataSet` 互操作无损往返。
- `Convert.ChangeType` 能覆盖白名单内所有类型间的转换，不需要额外转换基础设施。
- 自定义类型（如 `Money`、`UserId`、`enum`）的转换职责在**对象映射层**（`AddRow<T>` / `ToList<T>`），不在列类型系统内。
- 序列化场景可穷举处理所有列类型，不存在"不知道怎么序列化"的问题。

### 3.3 `RecordColumnCollection`

`RecordColumnCollection` 管理 `Record` 的列集合。

- 实现 `IReadOnlyList<RecordColumn>`，**不再继承 `List<RecordColumn>`**：避免外部通过基类接口（`Add(RecordColumn)`、`Insert`、`Sort` 等）绕过列名/类型校验。
- 内部以 `KeyedList<string, RecordColumn>`（`StringComparer.Ordinal`）维护数据，按列名查找为 O(log n)。

方法语义遵循 “宽容 / 严格 / 补齐” 三档划分：

| 操作 | 方法 | 列不存在时的行为 |
|------|------|------------------|
| 宽容查找 | `Find(name)` / `Find<T>(name)` | 返回 `null` |
| 严格查找 | `Get(name)` | 抛 `KeyNotFoundException` |
| 按需创建 | `Add(name, type)` / `Add<T>(name)` | 创建新列；同名同类型返回已有；同名不同类型抛 `InvalidOperationException` |

其它能力：

- 按索引访问：`this[int index]`
- 按名称访问：`this[string name]`（返回 `null` 若不存在）
- 判断：`Contains(string name)` / `IndexOf(string name)`
- 删除：`Remove(string name)`
- 重命名：`Rename(oldName, newName)`
- 清空：`Clear()`
- 计数：`Count`

---

## 4. `Record` 功能需求

### 4.1 可靠性与一致性

- 统一边界行为（空表、越界、空列、类型不匹配）。
- 明确异常类型（`ArgumentException`、`ArgumentOutOfRangeException`、`InvalidOperationException` 等）。
- 所有数据访问基于行索引（`int row`）或 `RecordRow`，无隐式状态依赖。
- `SetValue` / `GetValue` 的边界检查一致：行索引超出 `[0, Count)` 时抛出 `ArgumentOutOfRangeException`。

### 4.2 Schema 操作

用于列结构的管理与传输场景。

- `RenameColumn(string oldName, string newName)`：列重命名。
- `CastColumn(string name, Type newType)`：列类型转换（数据按行逐值转换）。
- `CloneSchema()`：仅复制列结构（零行），返回新 `Record`。
- `Clone()`：复制列结构与全部行数据，返回新 `Record`。
- `GetSchema()`：导出列定义信息（列名 + 类型），用于序列化、传输或 Schema 比较等场景。

### 4.3 服务端翻页

`Record` 支持携带服务端翻页元数据，用于分页查询结果的传输。

- `Page`：当前页码（从 1 开始，默认值 1）。
- `PageSize`：每页数据条数。
- `MaxCount`：总数据条数。当值大于 0 时返回设置值，否则返回 `Count`。
- `MaxPage`：总页数（只读，由 `MaxCount` 和 `PageSize` 计算得出）。当 `MaxCount` 为 0 时返回 0；`PageSize` 为 0 时按默认值 20 计算。

翻页属性仅作为元数据，不影响 `Record` 的数据操作行为。`Clone()` 和序列化会保留翻页属性。

### 4.4 序列化

`Record` 和 `RecordSet` 支持通过 `WriteTo` / `ReadFrom` 方法进行二进制序列化。

#### 二进制序列化

通过 `BinaryWriter` / `BinaryReader` 实现，按固定顺序写入：

1. 格式版本号（`byte`）。
2. 表名（`string`）。
3. 翻页元数据：`Page`、`PageSize`、`MaxCount`（各 `int`）。
4. 列数量（`int`），随后每列写入：列名（`string`）、类型码（`RecordColumnType`，`byte`）、是否可空（`bool`）。
5. 行数量（`int`），随后按列顺序写入每列的数据。

`RecordSet` 序列化其包含的所有 `Record`。

### 4.5 查询与过滤

`Record` 提供了一组实用的查询方法，用于在内存中快速查找符合条件的数据行：

- **泛型精确匹配**：`Find<T>(string name, T value)` 和 `FindAll<T>(string name, T value)` 可按指定列名与值进行快速精确匹配。如果指定列不存在或找不到匹配项，则返回 `null` 或空序列。
- **Lambda 筛选**：`Find(Func<RecordRow, bool> filter)` 和 `FindAll(Func<RecordRow, bool> filter)` 允许通过自定义条件筛选 `RecordRow`。
- **动态类型筛选**：`FindByDynamic(Func<dynamic, bool> filter)` 和 `FindAllByDynamic(Func<dynamic, bool> filter)` 配合 `dynamic` 提供更灵活的表达式查询。

---

## 5. 数据访问模型

`Record` 采用基于行索引的无状态访问模型，不维护游标。

### 5.1 访问方式

- **行索引**：`record[int row]` 返回 `RecordRow`。
- **遍历**：`foreach (var row in record)` 按行索引顺序产生 `RecordRow`。
- **列级别**：`column.GetValue(int row)` / `column.SetValue(value, int row)` / `column.Get<T>(int row)` / `column.Set(T value, int row)`。
- **行级别（强类型）**：`row.Field<T>(column)` / `row.Field<T>(name)` 读取；`row.Set<T>(name, value)` 写入。
- **dynamic**：`dynamic d = row;` 之后 `d.Foo` / `d["Foo"]` 进行读写。

### 5.2 列与值访问语义约定（重要）

| 路径 | 读取（列不存在） | 写入（列不存在） |
|------|------------------|------------------|
| `row.Field<T>(name)` | 返回 `default(T)` | — |
| `row.Set<T>(name, value)` | — | **自动按 `T` 建列** |
| `dynamic`（成员或索引器） | 返回 `null` | **按 `value.GetType()` 自动建列**；`value == null` 时跳过 |

约定优于配置：

- 自动建列只发生在显式的 `Set<T>` 与 dynamic 写入路径上。
- Mapping（`Fill<T>` / `CopyFrom<T>` / `XCopy`）**不建列**。需要从 DTO 灌入数据前，请先 `Columns.AddFrom<T>()` 或手动 `Add` 声明 schema。

### 5.3 设计约束

- 所有读写操作必须显式指定行索引，不存在依赖隐式位置的 API。
- `AddRow()` 返回新行的 `RecordRow`，不产生任何全局状态副作用。
- `RecordRow` 是轻量 `struct`，持有 `Record` 引用和行索引，本身不可变。

---

## 6. 空表行为约定

所有操作对空表（零行）的处理遵循统一规则：

- **返回值**：空表操作返回空 `Record`（零行但保留 Schema），**绝不返回 `null`**。

---

## 7. `RecordSet` 功能需求

`RecordSet` 应作为"命名 Record 集合"。`Record` 自身持有 `Name` 属性，`RecordSet` 以此作为管理键。

### 7.1 基础管理

- 按名称添加：`Add(name, record)`
- 按名称覆盖：`Set(name, record)`
- 获取：`Get(name)` / `TryGet(name, out record)`
- 删除：`Remove(name)`
- 判断存在：`Contains(name)`
- 重命名：`Rename(oldName, newName)`
- 清空：`Clear()`

### 7.2 集合信息与枚举

- `Count`
- `Names`
- 字符串索引器：`this[name]`
- 实现 `IEnumerable<Record>`：枚举所有 `Record`（不暴露名称键，`Record.Name` 已持有名称信息）。

### 7.3 与 `DataSet` 互操作

- `FromDataSet(DataSet ds)`：从 `DataSet` 创建 `RecordSet`。
- `ToDataSet()`：将当前 `RecordSet` 导出为 `DataSet`。
- `WriteTo(DataSet ds)`：将当前内容写入指定 `DataSet`。

要求：

- 以表名映射 `Record` 名称。
- 当前 `FromDataSet` 采用“抛异常”策略：若名称冲突则抛出 `ArgumentException`。后续可扩展支持覆盖或重命名策略。

### 7.4 行为约束

- 名称唯一。
- 名称比较策略可配置（区分/不区分大小写）。
- 对空名称、重复名称、空记录进行参数检查。

---

## 8. API 设计原则

- 方法命名语义清晰，避免"读取游标"和"加载数据"同名冲突。
- 同类操作保持一致的参数顺序与异常风格。
- 对高频路径预留优化点（减少分配）。

---

## 9. 建议实现顺序

1. `RecordSet` 基础容器能力（命名管理 + 索引器 + 枚举 + 校验）
2. `RecordSet` 与 `DataSet` 双向互操作
3. Schema 操作（`RenameColumn`、`CastColumn`、`CloneSchema`、`Clone`、`GetSchema`）
4. 服务端翻页属性
5. 二进制序列化（`WriteTo` / `ReadFrom`）

---

## 10. 验收标准（面向实用）

- 常见数据处理场景可只用 `Record/RecordSet` 完成。
- 支持从 `DataSet` 导入并回写 `DataSet`，映射规则清晰。
- 空表操作返回空 `Record`（零行保留 Schema），不返回 `null`。
- 异常信息清晰，便于排查问题。
- 多目标框架编译通过，并有覆盖关键行为的单元测试。
- `Record` / `RecordSet` 可通过 `WriteTo` / `ReadFrom` 正确往返二进制序列化。
- 翻页属性在序列化、`Clone` 中得到保留。

---

## 11. 最佳实践

### 11.1 创建与填充

```csharp
// 推荐：先定义列结构，再逐行添加数据
var record = new Record("Orders", expectedRows);
var idCol = record.Columns.Add<int>("Id");
var nameCol = record.Columns.Add<string>("Name");
var amountCol = record.Columns.Add<decimal>("Amount");

for (int i = 0; i < count; i++)
{
    var row = record.AddRow();
    idCol.Set(i + 1, row.Row);
    nameCol.Set($"Order-{i + 1}", row.Row);
    amountCol.Set(amounts[i], row.Row);
}
```

**要点**：

- 构造时传入预估行数 `expectedRows` 可减少列数组扩容次数。
- 优先使用泛型 `Add<T>()` 添加列，返回 `RecordColumn<T>` 以获得强类型 `Set` / `Get`。
- 使用 `AddRow()` 返回的 `RecordRow` 获取行索引，避免手动维护索引变量。

### 11.2 读取数据

```csharp
foreach (var row in record)
{
    int id = row.Field<int>("Id");
    string name = row.Field<string>("Name");
}
```

**要点**：

- `Field<T>` 在列不存在时返回 `default(T)`；如果你需要"列必须存在"的语义，请改用 `record.Columns.Get(name)` 取列后再读。

### 11.3 dynamic 与自动建列

```csharp
var re = new Record();
dynamic dto = re.AddRow();
dto.Id = 100;        // 自动按 int 建列
dto.Name = "abc";    // 自动按 string 建列
dto.Tag = null;      // 列不存在 + 值为 null → 跳过，不建列
```

dynamic 写入是为快速原型/动态结构而生；如果你的 schema 在编译时已知，**优先**显式 `Columns.Add<T>()` + `row.Set<T>()`，可获得：

- 更早暴露列名拼写错误（同名不同类型直接抛 `InvalidOperationException`）；
- 强类型路径，避免装箱与运行时类型推断；
- 更好的可读性与重构友好度。

### 11.4 对象映射

```csharp
// 推荐：先按 DTO 结构声明 Schema，再批量灌入
record.Columns.AddFrom<MyDto>();
foreach (var dto in dtos)
{
    var row = record.AddRow();
    row.CopyFrom(dto);
}
```

**注意**：`Fill<T>` / `CopyFrom<T>` **不会自动建列**。若 DTO 有 `Foo` 属性而 Record 未声明 `Foo` 列，该属性的值会被静默丢弃。这是预期行为——schema 应由调用方显式声明。
- 需要随机访问时使用 `record[index]` 索引器获取 `RecordRow`。

### 11.3 列引用的生命周期

```csharp
// 列引用在 Record 生命周期内有效
var col = record.Columns.Find<int>("Id")!;

// ✅ 正确：同一 Record 内使用列引用
int val = col.Get(0);

// ⚠️ 注意：Clone / CloneSchema 产生的新 Record 有独立的列实例
var clone = record.Clone();
// col 仍指向原 Record，不能用于 clone
var cloneCol = clone.Columns.Find<int>("Id")!;
```

**要点**：

- `RecordColumn` 绑定到创建它的 `Record` 实例，不可跨 `Record` 使用。
- `Clone()` / `CloneSchema()` 返回全新 `Record`，列引用不互通。
- `RecordRow.Get<T>(RecordColumn)` 在检测到列不属于当前 `Record` 时，会自动回退到按名称查找。

### 11.4 Schema 操作

```csharp
// 重命名列
record.RenameColumn("OldName", "NewName");

// 类型转换（逐行转换）
record.CastColumn("Price", typeof(decimal));

// 复制结构用于构建新表
var template = record.CloneSchema();
```

**要点**：

- `RenameColumn` 会使已持有的列引用名称同步更新（同一对象）。
- `CastColumn` 会替换底层列实例。此前缓存的列引用将失效，需重新获取。

### 11.5 与 ADO.NET 互操作

```csharp
// 从 IDataReader 填充
var record = new Record();
record.Read(dataReader);

// 从 DataTable 创建
var record = Record.Read(dataTable);

// 导出为 DataTable
var dt = record.ToDataTable();

// RecordSet <-> DataSet
var set = RecordSet.FromDataSet(dataSet);
var ds = set.ToDataSet();
```

**要点**：

- `Record.Read(IDataReader)` 会清空现有列和数据，完全用 Reader 的内容替换。
- `Record.Read(DataTable)` 是静态方法，返回新实例；`record.Read(IDataReader)` 是实例方法，就地填充。
- `DBNull.Value` 和 `null` 均映射为列类型的默认值（值类型为 `default(T)`，引用类型为 `null`）。

### 11.6 RecordSet 管理

```csharp
var set = new RecordSet();
set.Add("Orders", ordersRecord);
set.Add("Customers", customersRecord);

// 安全获取
if (set.TryGet("Orders", out var orders))
{
    // 使用 orders
}

// 遍历（按添加顺序）
foreach (var record in set)
{
    Console.WriteLine($"{record.Name}: {record.Count} rows");
}
```

**要点**：

- `RecordSet` 默认区分大小写（`StringComparer.Ordinal`）。如需不区分大小写，构造时传入 `StringComparer.OrdinalIgnoreCase`。
- `Add` 名称重复时抛异常；`Set` 名称重复时覆盖。根据场景选择合适的方法。
- `Rename` 会同步更新 `Record.Name` 属性。
- 枚举顺序与添加顺序一致。

### 11.7 异常处理

| 场景 | 异常类型 |
|------|----------|
| 行索引越界 | `ArgumentOutOfRangeException` |
| 列名不存在（`Columns.Get`） | `KeyNotFoundException` |
| 类型不匹配 | `InvalidCastException` |
| 空参数 | `ArgumentNullException` |
| 空/空白名称 | `ArgumentException` |

**要点**：

- `Columns.Find(name)` 不存在时返回 `null`（适合可选查找）。
- `Columns.Get(name)` 不存在时抛 `KeyNotFoundException`（适合必须存在的场景）。
- `record[row]` 索引越界时抛 `ArgumentOutOfRangeException`。

### 11.8 性能提示

- **预分配容量**：`new Record(name, expectedRows)` 减少列数组扩容。
- **缓存列引用**：循环内避免反复调用 `Columns.Find` 或 `Columns["name"]`。
- **优先泛型 API**：`RecordColumn<T>.Get(row)` / `Set(value, row)` 避免装箱拆箱，比 `GetValue` / `SetValue` 更高效。
- **批量构建**：先添加所有列，再逐行填充。不要在行循环中动态添加列。

### 11.9 数据查询

```csharp
// 按强类型列值查找单条
var row = record.Find<string>("Name", "Alice");

// 使用 Lambda 过滤多条记录
var activeRows = record.FindAll(r => r.Field<bool>("IsActive"));

// 使用 dynamic 动态查询
var dynamicRow = record.FindByDynamic(d => d.Id == 100);
```

**要点**：

- `Find` / `FindByDynamic` 若未找到匹配项或指定的列不存在，会直接返回 `null`。
- `FindAll` 系列方法返回延迟执行的 `IEnumerable<RecordRow>`。

---

## 12. 对象映射

对象映射能力属于 `Record` 核心职责（见 §2.1），提供 `AddRow<T>`、`AddRows<T>`、`ToList<T>`、`To<T>` 等扩展方法。
