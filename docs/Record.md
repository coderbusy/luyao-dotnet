# Record 体系文档

## 概述

Record 体系是一套**列存储**的轻量数据容器，位于 `LuYao.Data` 命名空间。其核心组件为：

| 类型 | 说明 |
|---|---|
| `RecordTable` | 列存储数据表，持有列集合与行数据 |
| `RecordRow` | 对某一行的引用（`struct`），通过列索引访问数据 |
| `RecordColumn` / `RecordColumn<T>` | 单列定义，携带类型信息与读写能力 |
| `RecordColumnCollection` | 列集合，维护列名到列对象的映射 |
| `RecordSet` | 命名 `RecordTable` 集合，以表名为键 |

---

## 核心类型

### RecordTable

列存储数据表，实现 `IEnumerable<RecordRow>`。

```csharp
var table = new RecordTable("Orders");
table.Columns.Add<int>("Id");
table.Columns.Add<string>("Name");

var row = table.AddRow();
table.Columns.Get("Id").Set(row, 1);
```

**从对象创建：**

```csharp
var table = RecordTable.From(myObj);               // 单对象，推断列结构
var table = RecordTable.FromList(myList);           // 集合，推断列结构
```

**转换回对象：**

```csharp
var obj  = table.To<MyClass>();                    // 取第一行
var list = table.ToList<MyClass>();                // 全部行
var dict = table.ToDictionary<int, MyClass>();     // 以第一列为键
```

---

### RecordRow

`struct`，是对 `RecordTable` 中某一行的轻量引用，不独立持有数据。

```csharp
RecordRow row = table[0];

// 动态访问（支持 dynamic）
dynamic d = row;
string name = d.Name;

// 与对象互转
row.CopyTo(myObj);          // 行 → 对象（填充已有实例）
var obj = row.To<MyClass>(); // 行 → 对象（新建实例）
row.CopyFrom(myObj);         // 对象 → 行
```

---

### RecordColumn / RecordColumnCollection

`RecordColumn` 持有列名、列类型及对行数据的读写能力。

```csharp
var col = table.Columns.Add<int>("Age");   // 同名同类型幂等，同名不同类型抛异常
var col = table.Columns.Find("Age");       // 不存在返回 null
var col = table.Columns.Get("Age");        // 不存在抛 KeyNotFoundException

col.Set(row, 18);
var val = col.Get(row);
```

支持从对象类型批量推断列：

```csharp
table.Columns.AddFrom<MyClass>();          // 按属性批量建列
```

---

### RecordSet

命名 `RecordTable` 的容器，以 `RecordTable.Name` 为键，默认大小写敏感。

```csharp
var set = new RecordSet();
set.Add(table);
var t = set["Orders"];
set.Remove("Orders");
```

---

## Meta 工具层

位于 `LuYao.Data.Meta` 命名空间，为 Record 体系提供反射加速与对象映射支持。

### XProp

封装单个公共实例属性，通过编译委托（`Expression.Lambda`）实现接近直接调用性能的读写访问。属性列表按类型缓存，全局唯一。

```csharp
var props = XProp.GetAll(typeof(MyClass));
foreach (var p in props)
{
    object? val = p.GetValue(obj);
    p.SetValue(obj, val);
}
```

---

### XData\<T\>

按属性名对强类型对象进行读写，底层使用 `XProp`。

```csharp
XData<MyClass>.Set(obj, "Name", "Alice");
var val = XData<MyClass>.Get(obj, "Name");
```

> 属性名不存在时抛 `ArgumentException`，属性不可读/写时抛 `InvalidOperationException`。

---

### XCopy

在**对象**与 **`RecordRow`** 之间进行属性双向复制的核心工具类。

以运行时实际类型（`data.GetType()`）为缓存键，**天然支持派生类**——无论对象声明类型是基类还是派生类，派生类新增的属性均能被正确处理。

```csharp
// 非泛型版本（推荐直接使用，无需指定类型参数）
XCopy.CopyTo(obj, row);    // 对象 → 行
XCopy.CopyFrom(obj, row);  // 行 → 对象
```

**泛型薄封装（`XCopy<T>`）** 提供编译期类型约束，逻辑完全委托给非泛型 `XCopy`：

```csharp
XCopy<MyClass>.CopyTo(obj, row);
XCopy<MyClass>.CopyFrom(obj, row);
```

两者使用**全局唯一**的 `ConcurrentDictionary<Type, IReadOnlyList<XProp>>` 缓存，不同泛型闭合类型（`XCopy<Base>`、`XCopy<Derived>`）共享同一份缓存，不会重复构建。

**规则：**
- 仅处理类型受支持（见 `Helpers.IsSupportedForReading` / `IsSupportedForWriting`）的公共实例属性
- 列不存在时静默跳过，**不自动建列**
- `data` 为 `null` 时抛 `ArgumentNullException`

---

### XCopy\<TSource, TTarget\>

在两个强类型对象之间按属性名进行浅拷贝。仅映射同名、类型完全一致且均受支持的可读/可写属性对。映射关系在首次访问时按 `(TSource, TTarget)` 组合构建并缓存。

```csharp
// 新建目标实例并填充
TargetDto dto = XCopy<MyClass, TargetDto>.Copy(source);

// 填充已有实例
XCopy<MyClass, TargetDto>.CopyTo(source, target);
```

---

## 序列化

### JSON

通过 `System.Text.Json` 的自定义转换器支持，分别注册 `RecordTableJsonConverter` 和 `RecordSetJsonConverter`。

### 二进制

通过 `RecordBinaryPayloadCodec` 提供高效的二进制序列化/反序列化，支持压缩（`RecordPayloadCompression`）。

```csharp
// RecordTable
var bytes = RecordTable.ToBinary(table);
var table = RecordTable.FromBinary(bytes);

// RecordSet
var bytes = RecordSet.ToBinary(set);
var set   = RecordSet.FromBinary(bytes);
```
