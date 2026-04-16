# Record

## 1. 目标

`Record` 类型聚焦于内存数据处理能力，面向"列存储"场景，强调：

- 实用
- 易用
- 可组合
- 可扩展

`RecordSet` 作为多表容器，用于组织和管理多个 `Record`。

---

## 2. 设计边界

### 2.1 `Record` 核心职责

- 负责内存中的表结构与数据处理。
- 保留与 `IDataReader`、`DataTable` 的互操作。
- 提供集合操作能力（筛选、连接、聚合、集合代数）。

### 2.2 `RecordSet` 核心职责

- 负责多个 `Record` 的命名管理。
- 保持与 `DataSet` 的双向互操作能力。

### 2.3 非核心职责

- 对象映射能力（`From<T>`、`FromList<T>`、`To<T>`、`ToList<T>`）不属于核心数据处理职责，后续下沉到扩展层。
- 通用序列化/反序列化（尤其是列类型不确定场景）不放在核心类型内。
- JSON、文本等协议能力通过扩展模块提供（如扩展方法或独立包）。

### 2.4 线程安全

- `Record`、`RecordQuery`、`RecordSet` 均为**非线程安全**类型（与 `DataTable` 一致）。
- 多线程场景下，调用方需自行同步。

---

## 3. 核心类型

### 3.1 `RecordRow`

`RecordRow` 是用户最常接触的类型之一，代表 `Record` 中的一行数据视图。

- 定义为 `struct`，持有所属 `Record` 引用与行索引（`Row`）。
- 提供类型安全的数据读取：`Get<T>(RecordColumn col)`、`Get<T>(string name)`。
- 提供基于列名的索引器读写：`this[string key]` { get; set; }。
- 支持隐式转换为 `int`（返回行索引）。
- 实现 `IRecordCursor` 接口。
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

`RecordColumnCollection` 管理 `Record` 的列集合，实现 `IReadOnlyList<RecordColumn>`。

已有能力：

- 按索引访问：`this[int index]`
- 按名称访问：`this[string name]`（返回 `null` 若不存在）
- 查找：`Find(string name)` / `Find<T>(string name)` / `Get(string name)`（不存在时抛异常）
- 判断：`Contains(string name)` / `IndexOf(string name)`
- 添加：`Add(string name, Type type)` / `Add<T>(string name)`
- 删除：`Remove(RecordColumn column)` / `Remove(string name)`
- 清空：`Clear()`
- 计数：`Count`

待补充能力（见 §4.6 Schema 操作）。

---

## 4. `Record` 功能需求

### 4.1 基础查询与变换

- `Where`：按条件过滤行（谓词参数为 `Func<RecordRow, bool>`）。
- `Select`：按列投影。
- `OrderBy` / `ThenBy`：排序。
- `Take` / `Skip`：分页基础能力。
- `Distinct`：去重（支持指定列）。

### 4.2 连接（Join）

- `Join`（等价内连接）
- `InnerJoin`
- `LeftJoin`
- `RightJoin`
- `FullOuterJoin`
- `CrossJoin`

连接需支持：

- 主键列选择（左键/右键）
- 重名列处理策略（异常、前缀、别名）
- 键比较策略（大小写、字符串比较）
- 空键匹配策略

### 4.3 集合代数

- `Union`（去重）
- `UnionAll`
- `Intersect`
- `Except`
- `Concat`

要求：

- 可进行 Schema 兼容检查。
- 错误时给出明确异常信息。

### 4.4 分组与聚合

- `GroupBy`
- 聚合函数：`Count`、`Sum`、`Min`、`Max`、`Avg`

要求：

- 支持多键分组。
- 支持输出列命名。

### 4.5 可靠性与一致性

- 统一边界行为（空表、越界、空列、类型不匹配）。
- 明确异常类型（`ArgumentException`、`ArgumentOutOfRangeException`、`InvalidOperationException` 等）。
- 所有数据访问基于行索引（`int row`）或 `RecordRow`，无隐式状态依赖。
- `SetValue` / `GetValue` 的边界检查一致：行索引超出 `[0, Count)` 时抛出 `ArgumentOutOfRangeException`。

### 4.6 Schema 操作

用于列结构的管理与传输场景。

- `RenameColumn(string oldName, string newName)`：列重命名。
- `CastColumn(string name, Type newType)`：列类型转换（数据按行逐值转换）。
- `ReorderColumns(params string[] names)`：按指定顺序排列列。
- `CloneSchema()`：仅复制列结构（零行），返回新 `Record`。
- `Clone()`：复制列结构与全部行数据，返回新 `Record`。
- `GetSchema()`：导出列定义信息（列名 + 类型），用于序列化、传输或 Schema 比较等场景。

---

## 5. 查询执行模型

为兼顾性能与可读性，采用"延迟执行 + 物化输出"的模型。

- `Record.AsQuery(options)` 返回可链式组合的查询对象（`RecordQuery`）。
- `Where`、`Select`、`Join`、`OrderBy`、`GroupBy` 等操作默认只记录执行计划，不立即产出 `Record`。
- 通过 `ToRecord()` 触发执行并物化结果。

### 5.1 入口 API

- `AsQuery(QueryOptions? options = null)`

### 5.2 查询选项 `QueryOptions`

- `EnableIndexing`：是否启用索引优化。
- `Indexes`：显式声明索引列（支持单列和复合列）。
- `StringComparison`：字符串比较策略（用于键比较与筛选）。

### 5.3 索引行为约束

- 索引可以延迟构建（首次被 `Join/Where/GroupBy` 使用时构建）。
- `Select` 若移除了索引列，索引应自动失效并在需要时重建。
- 未命中索引时自动回退全表扫描，保证结果正确性。

### 5.4 输入来源

- 左表通过 `Record.AsQuery()` 进入查询。
- Join 等操作的右表同时支持传入 `Record`（直接引用）和 `RecordQuery`（延迟组合）。
  - 传入 `Record` 时，内部自动包装为查询节点，对调用方透明。
  - 传入 `RecordQuery` 时，右表查询作为子查询在物化时一并执行。

### 5.5 物化行为

- 一个 `RecordQuery` 可以多次调用 `ToRecord()`，每次产生独立的 `Record` 实例。
- 适用于阶段性过滤等场景：同一查询管道在不同条件下多次物化。
- 物化不改变原始 `Record` 数据。

### 5.6 错误时机

- 参数校验（如列名不存在、类型不兼容等）统一在 `ToRecord()` 物化时抛出。
- 链式调用阶段仅记录执行计划，不做校验。
- 这保证了查询组合的灵活性，同时将所有错误集中到一个可预测的时间点。

---

## 6. 数据访问模型

`Record` 采用基于行索引的无状态访问模型，不维护游标。

### 6.1 访问方式

- **行索引**：`record[int row]` 返回 `RecordRow`。
- **遍历**：`foreach (var row in record)` 按行索引顺序产生 `RecordRow`。
- **列级别**：`column.GetValue(int row)` / `column.SetValue(value, int row)` / `column.Get<T>(int row)` / `column.Set(T value, int row)`。
- **行级别**：`row.Get<T>(column)` / `row.Get<T>(name)` / `row[name]`。

### 6.2 设计约束

- 所有读写操作必须显式指定行索引，不存在依赖隐式位置的 API。
- `AddRow()` 返回新行的 `RecordRow`，不产生任何全局状态副作用。
- `RecordRow` 是轻量 `struct`，持有 `Record` 引用和行索引，本身不可变。

---

## 7. 空表行为约定

所有操作对空表（零行）的处理遵循统一规则：

- **返回值**：空表操作返回空 `Record`（零行但保留 Schema），**绝不返回 `null`**。
- `Where` 结果为空 → 返回零行 `Record`，列结构与原表一致。
- `GroupBy` 对空表 → 返回零行 `Record`，列结构为分组键 + 聚合结果列。
- `Join` 左右表一方为空 → 按 Join 语义返回相应结果（InnerJoin 返回空；LeftJoin 保留左表行，右侧填 null；以此类推）。
- 集合操作（`Union`、`Intersect`、`Except`、`Concat`）对空表 → 按集合语义返回，Schema 保留。
- `ToRecord()` 对空查询管道 → 返回零行 `Record`。

---

## 8. `RecordSet` 功能需求

`RecordSet` 应作为"命名 Record 集合"。`Record` 自身持有 `Name` 属性，`RecordSet` 以此作为管理键。

### 8.1 基础管理

- 按名称添加：`Add(name, record)`
- 按名称覆盖：`Set(name, record)`
- 获取：`Get(name)` / `TryGet(name, out record)`
- 删除：`Remove(name)`
- 判断存在：`Contains(name)`
- 重命名：`Rename(oldName, newName)`
- 清空：`Clear()`

### 8.2 集合信息与枚举

- `Count`
- `Names`
- 字符串索引器：`this[name]`
- 实现 `IEnumerable<Record>`：枚举所有 `Record`（不暴露名称键，`Record.Name` 已持有名称信息）。

### 8.3 与 `DataSet` 互操作

- `FromDataSet(DataSet ds)`：从 `DataSet` 创建 `RecordSet`。
- `ToDataSet()`：将当前 `RecordSet` 导出为 `DataSet`。
- `WriteTo(DataSet ds)`：将当前内容写入指定 `DataSet`。

要求：

- 以表名映射 `Record` 名称。
- 当前 `FromDataSet` 采用“抛异常”策略：若名称冲突则抛出 `ArgumentException`。后续可扩展支持覆盖或重命名策略。

### 8.4 行为约束

- 名称唯一。
- 名称比较策略可配置（区分/不区分大小写）。
- 对空名称、重复名称、空记录进行参数检查。

---

## 9. API 设计原则

- API 风格参考 LINQ：命名、参数顺序、链式体验保持一致。
- 查询操作优先在 `RecordQuery` 上延迟执行，最终通过 `ToRecord()` 物化。
- 可保留少量直接执行快捷方法，但语义需清晰且与查询模型一致。
- 方法命名语义清晰，避免"读取游标"和"加载数据"同名冲突。
- 同类操作保持一致的参数顺序与异常风格。
- 对高频路径预留优化点（索引、哈希连接、减少分配）。

---

## 10. 建议实现顺序

1. `RecordSet` 基础容器能力（命名管理 + 索引器 + 枚举 + 校验）
2. `RecordSet` 与 `DataSet` 双向互操作
3. Schema 操作（`RenameColumn`、`CastColumn`、`CloneSchema`、`Clone`、`GetSchema`）
4. `RecordQuery` 基础框架（`AsQuery` + `ToRecord` + `QueryOptions`）
5. `Where/Select/OrderBy/Distinct/Take/Skip`
6. `Join/InnerJoin/LeftJoin/RightJoin` + 索引优化
7. `Union/Intersect/Except/Concat`
8. `GroupBy + Aggregate`
9. `FullOuterJoin/CrossJoin`

---

## 11. 验收标准（面向实用）

- 常见数据处理场景可只用 `Record/RecordSet` 完成。
- 支持从 `DataSet` 导入并回写 `DataSet`，映射规则清晰。
- 链式查询默认延迟执行，`ToRecord()` 物化结果可预测。
- 同一 `RecordQuery` 可多次 `ToRecord()`，每次结果独立。
- Join 与聚合具备可预测行为，索引优化不影响正确性。
- 空表操作返回空 `Record`（零行保留 Schema），不返回 `null`。
- 异常信息清晰，便于排查问题。
- 多目标框架编译通过，并有覆盖关键行为的单元测试。

---

## 12. 最佳实践

### 12.1 创建与填充

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

### 12.2 读取数据

```csharp
// 推荐：通过 foreach 遍历 + 列引用读取
var idCol = record.Columns.Find<int>("Id")!;
var nameCol = record.Columns.Find<string>("Name")!;

foreach (var row in record)
{
    int id = row.Get<int>(idCol);
    string name = row.Get<string>(nameCol);
}
```

**要点**：

- 在循环外缓存列引用（`RecordColumn` / `RecordColumn<T>`），避免每行按名称查找。
- 使用 `row.Get<T>(RecordColumn)` 比 `row.Get<T>(string)` 更快（跳过名称查找）。
- 需要随机访问时使用 `record[index]` 索引器获取 `RecordRow`。

### 12.3 列引用的生命周期

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

### 12.4 Schema 操作

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
- `ReorderColumns` 要求传入全部列名，不支持部分排序。

### 12.5 与 ADO.NET 互操作

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

### 12.6 RecordSet 管理

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

### 12.7 异常处理

| 场景 | 异常类型 |
|------|----------|
| 行索引越界 | `ArgumentOutOfRangeException` |
| 列名不存在（`Columns.Get`） | `KeyNotFoundException` |
| 重复列名 | `DuplicateNameException` |
| 类型不匹配 | `InvalidCastException` |
| 空参数 | `ArgumentNullException` |
| 空/空白名称 | `ArgumentException` |

**要点**：

- `Columns.Find(name)` 不存在时返回 `null`（适合可选查找）。
- `Columns.Get(name)` 不存在时抛 `KeyNotFoundException`（适合必须存在的场景）。
- `record[row]` 索引越界时抛 `ArgumentOutOfRangeException`。

### 12.8 性能提示

- **预分配容量**：`new Record(name, expectedRows)` 减少列数组扩容。
- **缓存列引用**：循环内避免反复调用 `Columns.Find` 或 `Columns["name"]`。
- **优先泛型 API**：`RecordColumn<T>.Get(row)` / `Set(value, row)` 避免装箱拆箱，比 `GetValue` / `SetValue` 更高效。
- **批量构建**：先添加所有列，再逐行填充。不要在行循环中动态添加列。

---

## 13. 对象映射

对象映射能力位于扩展层，提供 `Record` / `RecordQuery` 与 CLR 对象之间的双向转换。

### 13.1 设计定位

- 对象映射不属于 `Record` 核心职责（见 §2.3），以扩展方法形式提供。
- 映射层负责 CLR 属性与列之间的名称匹配、类型转换、构造函数选择。
- 列类型系统保持封闭白名单（见 §3.4），自定义类型的转换仅在映射边界发生。

### 13.2 API

#### 写入（对象 → Record）

- `Record.AddRow<T>(T item)`：将对象的属性按名称匹配写入已有列。
- `Record.AddRow<T>(T item, RecordMappingOptions options)`：带选项写入。
- `Record.AddRows<T>(IEnumerable<T> items)`：批量写入。
- `Record.AddRows<T>(IEnumerable<T> items, RecordMappingOptions options)`：批量带选项写入。

#### 读取（Record / RecordQuery → 对象）

- `Record.ToList<T>()`
- `Record.ToList<T>(RecordMappingOptions options)`
- `RecordRow.To<T>()`
- `RecordRow.To<T>(RecordMappingOptions options)`
- `RecordQuery.ToList<T>()`：直接从查询物化为对象列表，不产生中间 `Record`。
- `RecordQuery.ToList<T>(RecordMappingOptions options)`

### 13.3 名称匹配

- 匹配方式为鸭子类型：不绑定具体 CLR 类型，只关心属性名称。
- 不同类型只要属性名重叠就能写入同一张表。
- 默认按 `OrdinalIgnoreCase` 比较属性名与列名。

#### 名称转换（属性名 → 列名，单向）

名称转换仅支持**属性名 → 列名**的单向映射，通过 `RecordMappingOptions.NameTransform` 配置：

- **AddRow\<T\>**：对每个属性名调用 `NameTransform` 得到列名，再按列名在 `Record` 中查找匹配列。
- **ToList\<T\> / To\<T\>**：对 `T` 的每个属性名调用 `NameTransform` 得到列名，再按列名在 `Record` 中查找匹配列。
- 即：映射始终以「属性名经转换后的列名」为桥梁，`Record` 侧的列名不做反向转换。

```csharp
// 示例：属性名 PascalCase → 列名 snake_case
var options = new RecordMappingOptions
{
    NameTransform = name => ToSnakeCase(name) // "OrderId" → "order_id"
};
record.AddRow(order, options);           // order.OrderId → 列 "order_id"
var list = record.ToList<Order>(options); // 列 "order_id" → order.OrderId
```

- 若 `NameTransform` 为 `null`（默认），直接使用属性名按 `NameComparison` 匹配。
- `NameTransform` 的结果再按 `NameComparison` 与列名比较（转换后仍可不区分大小写）。

### 13.4 AddRow\<T\> 行为

| 场景 | 默认行为 | 可选覆盖 |
|------|----------|----------|
| 属性名匹配到列 | 写入值 | — |
| 属性名未匹配到列 | 忽略 | `AutoAddColumns = true` 时自动添加列（已有行填默认值） |
| 属性类型与列类型一致 | 直接写入 | — |
| 属性类型与列类型不一致 | `Convert.ChangeType` + checked 上下文 | `SerializeValue` 钩子优先 |
| 属性为枚举类型 | 内置转换：列为整数类型时取底层值，列为 `string` 时取名称 | — |
| 窄化转换溢出 | 抛 `OverflowException` | — |
| 不兼容转换 | 抛 `InvalidCastException`（含列名、源类型、目标类型、行号） | — |
| 自动拓宽列类型 | 不拓宽（列类型一旦确定即为契约） | `AutoWidenColumns`（预留，默认关闭） |

### 13.5 ToList\<T\> / To\<T\> 行为

| 场景 | 默认行为 | 可选覆盖 |
|------|----------|----------|
| 列匹配到属性 | 赋值 | — |
| 列未匹配到属性（多余列） | 忽略 | — |
| 属性未匹配到列（缺失列） | 属性保持 `default` | `RequireAllProperties = true` 时抛异常 |
| 列类型与属性类型一致 | 直接赋值 | — |
| 列类型与属性类型不一致 | `Convert.ChangeType` + checked | `DeserializeValue` 钩子优先 |
| 列为 `string`/整数 → 属性为枚举 | 内置转换：`Enum.Parse` 或直接强转 | — |
| 列值为 `null` → 可空属性 | 赋 `null` | — |
| 列值为 `null` → 不可空值类型属性 | **待定**（见下方讨论） | `NullToDefault` 选项 |
| 转换失败 | 抛异常（含列名、目标属性名、行号） | — |

### 13.6 构造函数选择策略

按以下优先级依次查找，命中即停止：

1. **`[RecordConstructor]` 标记的构造函数**：若存在，直接使用。若标记了多个，抛 `AmbiguousMatchException`。标记的构造函数按参数名（`OrdinalIgnoreCase`）匹配列名，未匹配到列的参数使用参数类型的 `default` 值。
2. **无参构造函数**：若存在公开无参构造函数，使用它创建实例，然后逐属性赋值。
3. **单参数 `RecordRow` 构造函数**：若存在接受单个 `RecordRow` 参数的公开构造函数，使用它。此模式将完整的行数据交给类型自行处理，映射层不再进行属性赋值。适用于需要自定义映射逻辑的复杂类型。
4. **以上均未命中**：抛 `MissingMethodException`，提示无可用构造函数。

```csharp
// 模式 1：显式标记
public class Order
{
    [RecordConstructor]
    public Order(int id, string name) { ... }
    public Order() { ... } // 被忽略，因为有标记的构造函数
}

// 模式 2：无参构造 + 属性赋值（最常见）
public class Order
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// 模式 3：RecordRow 构造（完全自定义）
public class Order
{
    public Order(RecordRow row)
    {
        Id = row.Get<int>("Id");
        Name = row.Get<string>("Name");
    }
}
```

### 13.7 嵌套与复杂类型

- 第一版仅映射扁平属性（只处理公开可读/可写的简单属性）。
- 遇到嵌套/复杂类型属性直接跳过，不报错。
- 与 Dapper 行为一致。

### 13.8 RecordMappingOptions

```csharp
public class RecordMappingOptions
{
    /// 名称比较策略（默认 OrdinalIgnoreCase）
    public StringComparison NameComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    /// 属性名 → 列名 单向转换函数（默认 null，不转换）
    public Func<string, string>? NameTransform { get; set; }

    /// AddRow 时是否自动添加不存在的列（默认 false）
    public bool AutoAddColumns { get; set; }

    /// ToList/To 时是否要求 T 的所有属性都有对应列（默认 false）
    public bool RequireAllProperties { get; set; }

    /// null 映射到不可空值类型时的行为（待定，见 §13.12）
    public bool NullToDefault { get; set; }

    /// 自定义写入转换：(列名, 列类型, 属性值) → 列值
    public Func<string, Type, object?, object?>? SerializeValue { get; set; }

    /// 自定义读取转换：(列名, 属性类型, 列值) → 属性值
    public Func<string, Type, object?, object?>? DeserializeValue { get; set; }

    /// 自定义映射器，优先级最高（见 §13.11）
    public IRecordMapper? Mapper { get; set; }
}
```

### 13.9 性能

映射层需要在高频场景（万行级遍历）下保持高效。核心策略：

#### 编译委托

- 首次映射时通过表达式树（`Expression<T>`）编译生成强类型委托，后续调用零反射。
- **AddRow\<T\>**：编译为 `Action<T, RecordColumn[], int>`，直接按列索引写入，无装箱。
- **ToList\<T\> / To\<T\>**：编译为 `Func<RecordColumn[], int, T>`，直接按列索引读取并构造对象。

#### 缓存策略

- 映射计划按 `(typeof(T), Schema 签名, RecordMappingOptions 关键字段)` 缓存。
- Schema 签名 = 列名 + 列类型的有序组合哈希（列顺序变化 = 不同签名）。
- 缓存使用 `ConcurrentDictionary`，线程安全，生命周期为进程级。
- `IRecordMapper` 被设置时跳过缓存（用户完全接管）。

#### 热路径优化

- 编译后的委托直接操作 `RecordColumn<T>._data` 数组（通过 `internal` 访问），跳过边界检查和虚方法调用。
- 避免 `object` 装箱：值类型列使用泛型 `RecordColumn<T>.Get/Set` 路径。
- 批量写入（`AddRows<T>`）一次编译、循环调用，避免重复查找列。
- 名称转换结果在编译阶段求值并固化到委托中，运行时不再调用 `NameTransform`。

### 13.10 异常汇总

| 场景 | 异常类型 |
|------|----------|
| 类型转换失败 | `InvalidCastException`（含列名、源类型、目标类型、行号） |
| 窄化溢出 | `OverflowException` |
| `null` → 不可空值类型 | 待定（见 §13.12） |
| `RequireAllProperties` 且属性缺失列 | `InvalidOperationException`（含缺失属性名） |
| `[RecordConstructor]` 标记了多个构造函数 | `AmbiguousMatchException` |
| 无可用构造函数（无标记、无无参、无 RecordRow 参数） | `MissingMethodException` |

### 13.11 IRecordMapper

`IRecordMapper` 提供完全自定义的映射能力，优先级高于所有内置映射逻辑。当 `RecordMappingOptions.Mapper` 被设置时，映射层将所有工作委托给它，不再进行反射、名称匹配或类型转换。

```csharp
/// 非泛型基接口
public interface IRecordMapper
{
    /// 将对象的值写入 Record 的指定行
    void Write(object item, Record record, int row);

    /// 从 Record 的指定行创建对象
    object Read(Record record, int row);
}

/// 泛型接口，提供强类型支持
public interface IRecordMapper<T> : IRecordMapper
{
    /// 将对象的值写入 Record 的指定行
    void Write(T item, Record record, int row);

    /// 从 Record 的指定行创建对象
    new T Read(Record record, int row);
}
```

#### 职责边界

- **由 Mapper 负责**：属性-列绑定、类型转换、对象构造——全部由实现者控制。
- **由映射层负责**：调用 `AddRow` 时分配行（`record.AddRow()`），将行索引传给 Mapper；调用 `ToList<T>` 时遍历行并收集结果。
- Mapper 不负责 `Record` 的 Schema 管理（不添加/删除列）。

#### 使用场景

- 需要极致性能，避免内置映射的开销（虽然内置已使用编译委托，但 Mapper 可以做到零抽象）。
- 映射逻辑非常规（如一个属性映射到多列、条件映射等）。
- 需要复用已有的映射逻辑（如项目中已有 ORM 映射基础设施）。

```csharp
// 示例
public class OrderMapper : IRecordMapper<Order>
{
    public void Write(Order item, Record record, int row)
    {
        record.Columns.Get("Id").SetValue(item.Id, row);
        record.Columns.Get("Name").SetValue(item.Name, row);
    }

    public Order Read(Record record, int row)
    {
        return new Order
        {
            Id = record.Columns.Find<int>("Id")!.Get(row),
            Name = record.Columns.Find<string>("Name")!.Get(row),
        };
    }

    // 显式接口实现
    void IRecordMapper.Write(object item, Record record, int row) => Write((Order)item, record, row);
    object IRecordMapper.Read(Record record, int row) => Read(record, row);
}

// 使用
var options = new RecordMappingOptions { Mapper = new OrderMapper() };
record.AddRow(order, options);
var list = record.ToList<Order>(options);
```

#### 约束

- `Mapper` 与 `SerializeValue` / `DeserializeValue` 互斥：设置 `Mapper` 后，钩子被忽略。
- `Mapper` 被设置时跳过内置缓存（Mapper 自行管理性能）。
- `NameTransform`、`NullToDefault`、`RequireAllProperties` 等选项在 Mapper 模式下不生效（Mapper 完全接管）。

### 13.12 待定：null → 不可空值类型的默认行为

当列值为 `null` 映射到不可空值类型属性（如 `int`、`DateTime`）时，默认行为尚未确定。

候选方案：

| 方案 | 行为 | 优点 | 缺点 |
|------|------|------|------|
| A. 默认抛异常 | 抛 `InvalidOperationException` | 安全，不会静默丢失 null 信息 | 从 DB 导入含 NULL 的数据时频繁报错 |
| B. 默认赋 `default` | `int` → `0`，`DateTime` → `0001-01-01` | 与 `DataTable` / `IDataReader` 行为一致；与 Record 自身 `Read(IDataReader)` 的 null 处理一致 | 静默丢失 null 语义，`0` 和 null 不可区分 |
| C. 按属性类型区分 | 数值类型赋 `default`，其他抛异常 | 折中 | 规则不一致，增加学习成本 |

通过 `NullToDefault` 选项可覆盖默认行为，但默认行为需要确定。
