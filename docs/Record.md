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
- `FullOuterJoin`（可在第二阶段实现）
- `CrossJoin`（可在第二阶段实现）

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
9. 评估并补充 `FullOuterJoin/CrossJoin`

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
