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

- 每个列持有所属 `Record` 引用、列名（`Name`）和数据类型（`Type`）。
- 数据以数组 `T[]` 形式列存储，支持自动扩容。
- 提供按行索引或游标位置读写值：`GetValue(int row)` / `SetValue(object? value, int row)`。
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
- 尽量减少隐式状态副作用（例如游标状态影响操作结果）。

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

## 6. 游标模型

### 6.1 游标定位

`Record` 保留游标（`Cursor`）属性，用于兼容基于游标的旧 API（如 `Read()`、`MoveFirst()`、`MoveLast()` 等）。

### 6.2 使用边界

- 游标仅在用户主动遍历（`Read()` 循环）或显式操作游标位置时生效。
- 内部实现（查询引擎、集合操作等）不依赖游标状态，全部基于行索引操作。
- 新增操作应基于行索引或 `RecordRow`，不引入对游标的隐式依赖。

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
- 对表名冲突提供明确策略（抛异常 / 覆盖 / 重命名）。

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
