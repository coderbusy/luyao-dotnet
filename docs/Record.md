# Record

## 1. 定位

`Record` 是一个面向内存场景的列存储数据容器，适合：

- 轻量表结构数据处理
- 对象列表与表结构之间转换
- 与 ADO.NET 的 `IDataReader` / `DataTable` / `DataSet` 互操作
- 需要按列存储、按行访问的业务代码

`RecordSet` 是多个 `Record` 的命名集合，用于统一管理多张内存表，并提供与 `DataSet` 的双向互操作。

设计重点：

- API 简单直接
- 类型白名单明确
- 与对象映射、ADO.NET 互通
- 支持二进制序列化

`Record`、`RecordRow`、`RecordSet` 都不是线程安全容器。多线程场景需要调用方自行同步。

---

## 2. 当前实现范围

### 2.1 `Record`

`Record` 提供：

- 列结构管理：添加列、重命名列、类型转换、导出 schema
- 行管理：新增、删除、批量删除、清空、按索引访问
- 行枚举：实现 `IEnumerable<RecordRow>`
- 查询：按列值、Lambda、`dynamic` 条件查找
- 排序：原地多列排序，行为对齐 SQLite ORDER BY
- 分组：单字段、多字段字符串分组，以及 2/3 字段元组分组
- 数据补全：`Enrich` 按关联键从另一个 `Record` 追加当前不存在的列并填充值
- 对象映射：对象/对象集合与 `Record` 之间转换
- ADO.NET 互操作：`IDataReader`
- 二进制序列化：`Record` / `RecordSet` 都支持字节流读写
- 服务端翻页元数据：`Page`、`PageSize`、`MaxCount`、`MaxPage`

### 2.2 `RecordSet`

`RecordSet` 提供：

- 按名称管理多个 `Record`
- 名称比较器可配置
- 二进制序列化

### 2.3 非目标

不包含以下能力：

- SQL 翻译
- JSON / CSV / 文本协议层
- 线程安全容器
- 类似 ORM 的复杂关系映射

---

## 3. 核心类型

### 3.1 `Record`

常用构造：

- `new Record()`
- `new Record(string name)`
- `new Record(string? name, int rows)`

常用属性：

- `Name`：表名
- `Columns`：`RecordColumnCollection`
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

`Enrich` 相关语义：

- 重载：`Enrich(source, sharedColumn)`、`Enrich(source, selfColumn, sourceColumn)`、`Enrich(source, selfColumn, sourceColumn, columnsToEnrich)`。
- 匹配规则：按当前记录 `selfColumn` 与来源记录 `sourceColumn` 的值匹配来源中的第一行。
- 列处理：仅处理来源中当前记录尚不存在的列；已存在同名列不会被覆盖。
- 列过滤：`columnsToEnrich` 非空时仅补充其中列名（仍会跳过当前记录已存在列）。
- 参数校验：`source`、`sharedColumn`、`selfColumn`、`sourceColumn` 为 `null` 抛 `ArgumentNullException`；列名为空白抛 `ArgumentException`。

### 3.2 `RecordRow`

`RecordRow` 是一个轻量 `struct`，表示某个 `Record` 中的单行视图。

`RecordRow` 提供：

- `Record`：所属 `Record`
- `Row`：当前行号
- `implicit operator int`
- `this[string name]`：按列名读写当前行值
- `To<T>(string name)`：按列名读取并转换类型
- `ToDictionary()`：当前行转字典
- `CopyTo<T>(T data)`：当前行写入已有对象
- `CopyFrom<T>(T data)`：对象属性写入当前行
- `To<T>()`：当前行转新对象
- `Merge(RecordRow other)`：将另一行数据合并到当前行，同名列覆盖，新列追加
- `Merge<T>(T model)`：将对象属性合并到当前行，同名列覆盖，新列追加
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

- `ToString(string? name)`：若 `name` 为 `null`/空白或列不存在，返回空字符串；否则等同于调用对应列的 `RecordColumn.ToString(int row)`。
- 索引器读取时，如果列名为 `null`、空字符串或空白字符串，也直接返回 `null`。
- 索引器写入时，如果列不存在且值非 `null`，会调用 `Columns.Add(name, value.GetType())` 自动建列；如果运行时类型不在支持白名单内，会抛出异常。
- `RecordRow` 的公开获取方式通常是 `record[row]` 或通过枚举 `foreach (var row in record)`。

**Merge 方法语义**

`Merge` 与 `CopyFrom` 的核心区别在于对"目标列不存在"的处理方式：

| 方法              | 目标列不存在时         | 来源为 null 时             |
|-------------------|----------------------|--------------------------|
| `CopyFrom<T>(T)`  | 静默跳过，不建列       | 会写入 null 到已有目标列    |
| `Merge(RecordRow)`| 自动按来源列类型建列   | 会写入 null（覆盖已有值）   |
| `Merge<T>(T)`     | 自动按属性类型建列     | 会写入 null（覆盖已有值）   |

`Merge<T>(T model)` 的额外规则：

- 仅处理**可读属性**（`CanRead == true`）。
- 属性类型须在支持白名单内（`bool`、`sbyte`/`byte`、`short`/`ushort`、`int`/`uint`、`long`/`ulong`、`float`/`double`/`decimal`、`char`、`string`、`DateTime`、`DateTimeOffset`、`TimeSpan`、`Guid`、`byte[]`，以及上述类型的枚举/Nullable 形式）；不在白名单内的属性会被**静默跳过**，不会建列也不会抛出异常。
- `model` 为 `null` 时抛出 `ArgumentNullException`。

`Merge(RecordRow other)` 的额外规则：

- 遍历 `other` 所属 `Record` 的所有列，按列名在当前行所属 `Record` 中查找或新建同名列（类型与来源列一致），然后将来源值写入。
- `other` 可来自不同的 `Record` 实例，不要求列结构一致。
- 当前行与 `other` 指向同一行时，结果等同于原地覆盖（无实质变化）。

### 3.3 `RecordColumn` / `RecordColumn<T>`

`RecordColumn` 是列的抽象基类，`RecordColumn<T>` 是泛型实现。

常用成员：

- `Name`
- `Type`
- `ColumnType`
- `IsNullable`
- `Capacity`
- `Get(int row)` / `Set(int row, object? value)`
- `To<T>(int row)`
- `ToList<T>()`：将整列数据转为 `List<T?>`，长度等于 `Record.Count`；`null` 值对应 `default(T)`
- `ToString(int row)`：获取指定行列值的字符串表示；`null` 值返回空字符串
- `Delete(int row)` / `Clear()`
- `GetValue(int row)` / `SetValue(int row, T value)`（泛型列）

说明：

- 列绑定到所属 `Record`。
- 列名可通过 `Record.RenameColumn` 或 `Record.Columns.Rename` 修改。
- `CastColumn()` 会创建新列替换旧列，旧列引用应视为失效。
- 列删除行时会把后续数据左移。

### 3.4 `RecordColumnCollection`

`Record.Columns` 的类型是 `RecordColumnCollection`，其特性如下：

- 实现 `IReadOnlyList<RecordColumn>`
- 内部使用 `KeyedList<string, RecordColumn>` 存储
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
| 清空 | `Clear()` | 清空列并把 `Record.Count` 重置为 `0` |
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

### 3.5 `RecordSchema`

`Record.GetSchema()` 返回 `RecordSchema`，用于导出列定义。

每个 `ColumnDef` 包含：

- `Name`
- `ColumnType`
- `IsNullable`
- `ArrayRank`（0 表示非数组，1 表示一维数组，2 表示二维数组）
- `Type`

适用场景：

- schema 比较
- 调试与诊断
- 序列化相关元信息输出

### 3.6 `RecordSet`

`RecordSet` 是 `Record` 的命名集合，实现 `IEnumerable<Record>`。

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

- 内部使用 `SortedDictionary<string, Record>`。
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
| 数组 | 上述类型的一维或多维数组（如 `int[]`, `string[,]`） |

说明：

- 列类型白名单是封闭的。
- `enum` 在内部会按其基础数值类型参与列类型映射。
- 对象映射与自动建列都受该白名单限制。
- **数组支持**：支持任意维度数组（一维 `int[]`、二维 `int[,]` 等），但 `Write(DataTable)` 时数组列会被跳过（`byte[]` 除外，因为 `DataTable` 原生支持）。
- **数组元素可空性**：`int?[]`（可空元素数组）是合法类型，序列化时会保留每个元素的 null 状态。
- **PostgreSQL 互操作**：从 PostgreSQL 的 `IDataReader` 读取数组列（如 `int[]`, `text[]`）时，Npgsql 驱动会自动映射为 CLR 数组类型，`RecordTable.Read(IDataReader)` 可以直接识别并创建对应的数组列。

---

## 5. `Record` API 概览

### 5.1 建列与加行

最常见的用法是先建列，再加行：

```csharp
var record = new Record("Orders", 1000);

var idCol = record.Columns.Add<int>("Id");
var nameCol = record.Columns.Add<string>("Name");
var amountCol = record.Columns.Add<decimal>("Amount");

var row = record.AddRow();
idCol.SetValue(row.Row, 1);
nameCol.SetValue(row.Row, "Order-1");
amountCol.SetValue(row.Row, 99.5m);
```

提供：

- `AddRow()`：新增一行并返回 `RecordRow`
- `AddRowFromValues(params object[] values)`：按列顺序填值，超出列数的值会被忽略

```csharp
var record = new Record();
record.Columns.Add<int>("Id");
record.Columns.Add<string>("Name");

record.AddRowFromValues(1, "A");
record.AddRowFromValues(2, "B", "Ignored");
```

**数组列示例：**

```csharp
var record = new Record("Products");

// 添加数组列
record.Columns.Add<string[]>("Tags");
record.Columns.Add<int[]>("MonthlyScores");
record.Columns.Add<decimal[,]>("PriceMatrix");

var row = record.AddRow();
row["Tags"] = new[] { "VIP", "Premium" };
row["MonthlyScores"] = new[] { 85, 90, 88, 92 };
row["PriceMatrix"] = new decimal[,] { { 1.1m, 2.2m }, { 3.3m, 4.4m } };

// ToString() 输出 JSON 格式
Console.WriteLine(row.ToString("Tags"));          // ["VIP", "Premium"]
Console.WriteLine(row.ToString("MonthlyScores")); // [85, 90, 88, 92]
Console.WriteLine(row.ToString("PriceMatrix"));   // [[1.1, 2.2], [3.3, 4.4]]

// 序列化与反序列化
byte[] data = record.ToBytes();
var restored = RecordTable.FromBytes(data);
string[] tags = restored[0].To<string[]>("Tags");
```

### 5.2 删除与清空

提供：

- `Delete(int row)`：删除指定行，越界返回 `false`
- `DeleteWhere(Func<RecordRow, bool>)`：按条件批量删除
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
- 枚举时按 `0..Count-1` 顺序返回 `RecordRow`

### 5.4 查询

提供：

- `Find<T>(string name, T value)`
- `FindAll<T>(string name, T value)`
- `Find(string name, object? value)`
- `FindAll(string name, object? value)`
- `Find(Func<RecordRow, bool> filter)`
- `FindAll(Func<RecordRow, bool> filter)`
- `FindByDynamic(Func<dynamic, bool> filter)`
- `FindAllByDynamic(Func<dynamic, bool> filter)`
- `GetList<T>(string colName)`：将指定列的所有值提取为 `List<T?>`，长度等于 `Count`；列不存在时抛 `KeyNotFoundException`，列名为空时抛 `ArgumentException`，列值无法转换为目标类型时抛 `InvalidCastException`

行为说明：

- `Find*` 未命中时返回 `null`
- `FindAll*` 未命中时返回空序列
- `FindAll*` 使用 `yield return`，属于延迟执行枚举
- `Find<T>(name, value)` 中列不存在时直接返回 `null`
- `FindAll<T>(name, value)` 中列不存在时直接返回空序列
- `Find(string, object?)` / `FindAll(string, object?)` 使用 `object.Equals` 进行匹配，支持 `null` 值比较；列不存在时分别返回 `null` 或空序列
- 以委托过滤的 `Find` / `FindAll` 在 `filter == null` 时抛 `ArgumentNullException`

示例：

```csharp
var first = record.Find<int>("Id", 100);

// 使用 object 重载（支持运行时类型与 null）
var row = record.Find("Name", (object?)"Bob");
var nullRows = record.FindAll("Name", (object?)null);

var largeOrders = record.FindAll(r => r.To<decimal>("Amount") > 1000m);

var active = record.FindByDynamic(d => d.Status == "Active");
```

### 5.5 原地排序

`Record.Sort` 对当前记录进行**原地多列排序**，行为与 SQLite 的 `ORDER BY` 子句相同。

**核心设计**：利用列存储的天然优势——先对一个行索引数组排序，再以该置换一次性重排各列底层数据数组，避免逐行搬移对象的开销。

#### 两个重载

**字符串重载**（与 SQL ORDER BY 子句同构）：

```csharp
void Sort(string orderBy)
```

**键集合重载**（避免字符串解析，适合程序化构造）：

```csharp
void Sort(params RecordSortKey[] keys)
```

`RecordSortKey` 是不可变 `readonly struct`，在 `.NET Standard 2.0+` / `.NET 6+` 目标下支持从 `(string column, bool descending)` 元组隐式转换。

#### 排序语法（字符串重载）

格式：`列名 [ASC|DESC]`，多个键以英文逗号分隔：

```
"name"                      // 单列升序（默认）
"name DESC"                 // 单列降序
"dept ASC, salary DESC"     // 多列混合方向
"dept, salary DESC"         // 第一列升序，第二列降序
```

语法约定：

- 列名区分大小写（Ordinal 比较），与 `Columns.Get(name)` 一致
- 方向关键字 `ASC` / `DESC` 不区分大小写
- 列名中不能含空格或引号（不支持引号标识符）

#### NULL 排序

NULL 值视为最小值（与 SQLite 默认行为相同）：

- 升序（ASC）：NULL 排在最前
- 降序（DESC）：NULL 排在最后

#### 值比较规则

非 NULL 值的比较规则与 SQLite 默认行为一致：

- `string` 列：使用 **Ordinal（二进制）** 比较，大小写敏感，与区域性无关（`A` < `B` < `a` < `b`）
- `byte[]` 列：逐字节比较，长度不同时较短者排在前面
- 其他类型：使用系统默认的 `IComparable` 比较

#### 稳定性

当所有键均相等时，保持行的原始相对顺序（通过把原始行索引作为最终 tie-breaker 实现）。

#### 示例

```csharp
// 字符串重载
record.Sort("salary DESC");
record.Sort("dept ASC, salary DESC");

// 键集合重载（显式构造）
record.Sort(new RecordSortKey("dept"), new RecordSortKey("salary", true));

// 元组隐式转换（.NET Standard 2.0+ / .NET 6+）
record.Sort(("dept", false), ("salary", true));
```

#### 复杂度

- 比较阶段：O(N log N)，其中每次比较最多遍历所有排序键
- 重排阶段：O(列数 × N)，各列独立重排底层数组
- 额外内存：O(N)（行索引数组 + 各列临时重排缓冲区）

#### 异常

| 场景 | 行为 |
|------|------|
| `orderBy` 为 null / 空白 | 直接返回，不修改数据 |
| `keys` 为 null / 空数组 | 直接返回，不修改数据 |
| 段内 token 数超过 2 | 抛 `FormatException` |
| 方向关键字不是 ASC / DESC | 抛 `FormatException` |
| 列名不存在 | 抛 `KeyNotFoundException`（即使表为空或只有一行） |
| 同一列名出现多次 | 忽略后续重复项，仅以首次出现的方向进行排序（与 SQLite ORDER BY 行为一致） |

#### 约束

- 不产生新 `Record`，直接修改当前实例
- `Count`、`Capacity`、列对象引用均保持不变
- 不支持表达式排序、`COLLATE`、`NULLS FIRST/LAST` 显式语法或引号标识符

---

### 5.6 分组

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

- `Group<T>(string fld)` 的返回类型是 `IDictionary<T, List<RecordRow>>`
- `Group(string fld)` 的返回类型是 `IDictionary<string, IList<RecordRow>>`
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

- `Group<T1, T2>(fld1, fld2)` -> `IDictionary<(T1?, T2?), IList<RecordRow>>`
- `Group<T1, T2, T3>(fld1, fld2, fld3)` -> `IDictionary<(T1?, T2?, T3?), IList<RecordRow>>`

```csharp
var groups = record.Group<int, int>("Year", "Month");
```

实际行为要点：

- 分组 API 会遍历当前所有行并立即构建字典，不是延迟执行。
- 如果指定列不存在，不会返回空字典；缺失列会按默认值参与分组：
  - `Group<T>(fld)`：缺失列键为 `default(T)`
  - `Group(string fld)`：缺失列键为 `string.Empty`
  - 元组分组：缺失列对应分量为 `default`

### 5.7 Schema 操作

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

### 5.8 `ToString()`

会输出调试友好的文本内容：

- 先输出表名、行数、列数
- 当仅有一行时，以“列名 | 值”的方式逐列输出
- 多行时以表格形式输出
- 对宽字符做显示宽度处理
- 单元格内容过长时按显示宽度截断
- 单元格字符串值统一通过 `RecordColumn.ToString(int row)` 获取（`null` 渲染为空字符串）

这个输出更适合调试、日志和快速查看数据，而不是稳定的序列化格式。

---

## 6. 对象映射

### 6.1 `Record` 级别映射

提供：

- `Record.From<T>(T data)`
- `Record.FromList<T>(IEnumerable<T> items)`
- `record.AddRowFrom<T>(T item)`
- `record.AddRowsFromList<T>(IEnumerable<T> items)`
- `record.ToList<T>()`
- `record.To<T>()`
- `record.ToDictionary<TKey, T>()`

示例：

```csharp
var record = Record.From(new OrderDto
{
    Id = 1,
    Name = "Order-1"
});

List<OrderDto> list = record.ToList<OrderDto>();
OrderDto first = record.To<OrderDto>();
Dictionary<int, OrderDto> dict = record.ToDictionary<int, OrderDto>();
```

`ToDictionary<TKey, T>` 使用第一列的值作为字典键；若存在重复键，则后者覆盖前者。若无行或无列，返回空字典。

### 6.2 `RecordRow` 级别映射

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

- `Record.From<T>` / `FromList<T>` 会先 `Columns.AddFrom<T>()`，再写入数据
- `AddRowFrom<T>` 本身**不会自动建列**，它只是新增一行后执行 `CopyFrom`
- `CopyFrom` / `AddRowFrom` / `AddRowsFromList` 写入时，仅对已存在列赋值；缺失列会被忽略
- 对象映射只处理支持白名单中的属性类型

因此，若不是使用 `Record.From<T>` / `FromList<T>`，通常应先显式建列：

```csharp
var record = new Record();
record.Columns.AddFrom<OrderDto>();
record.AddRowFrom(dto);
```

---

## 7. ADO.NET 互操作

`Record` 提供与 `IDataReader` 的互操作：

- `void Read(IDataReader dr)`

行为说明：

- `Read(IDataReader)` 会先 `Columns.Clear()`，然后根据 reader 的字段结构重建整张表
- `Read(IDataReader)` 读取到 `DBNull.Value` 时会跳过赋值，目标列保持默认值

示例：

```csharp
var record = new Record();
record.Read(dataReader);
```

---

## 8. 二进制序列化

### 8.1 `Record`

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
4. 列定义：列名、`RecordColumnType`、`IsNullable`
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
Record copy = Record.FromBytes(bytes);
```

### 8.2 `RecordSet`

当前实现提供：

- `WriteTo(Stream)` / `WriteTo(BinaryWriter)`
- `ReadFrom(Stream)` / `ReadFrom(BinaryReader)`
- `ToBytes()` / `FromBytes(byte[])`
- `FromStream(Stream)`
- `IsBinaryPayload(byte[])`

说明：

- `RecordSet` 序列化时会先写自己的 payload header 与版本号
- 内部逐个写出每个 `Record`
- 以字典键作为权威名称，避免外部修改 `Record.Name` 后与集合键不一致

```csharp
byte[] bytes = set.ToBytes();
RecordSet copy = RecordSet.FromBytes(bytes);
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
| `Record.Delete(row)` 越界 | 返回 `false` |
| `DeleteWhere(null)` | 抛 `ArgumentNullException` |
| `DeleteRows(null)` | 抛 `ArgumentNullException` |
| `Find(null)` / `FindAll(null)` | 抛 `ArgumentNullException` |
| `FindByDynamic(null)` / `FindAllByDynamic(null)` | 抛 `ArgumentNullException` |
| `Sort("")` / `Sort("  ")` | 直接返回，不修改数据 |
| `Sort("col UPWARD")` 等非法方向 | 抛 `FormatException` |
| `Sort("col token1 token2")` 段内 token 过多 | 抛 `FormatException` |
| `Sort("missing ASC")` 列不存在 | 抛 `KeyNotFoundException` |
| `Sort(Array.Empty<RecordSortKey>())` | 直接返回，不修改数据 |
| `Sort(new RecordSortKey("col"), new RecordSortKey("col"))` 列名重复 | 忽略重复列名，仅以首次出现的方向进行排序（与 SQLite 行为一致） |
| `row["Missing"]` | 返回 `null` |
| `row.To<T>("Missing")` | 返回 `default` |
| `row["New"] = null` | 不建列，直接忽略 |
| `RecordSet.Get(name)` 不存在 | 抛 `KeyNotFoundException` |
| `RecordSet.Add(null/空白, record)` | 抛 `ArgumentException` |
| `RecordSet.Add(name, null)` | 抛 `ArgumentNullException` |

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

- 热路径优先使用 `RecordColumn<T>.SetValue` / `GetValue`
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
