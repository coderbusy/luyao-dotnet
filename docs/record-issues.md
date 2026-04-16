# Record 待修复事项列表

> 请在每个条目后标注：✅ 需修复 / ❌ 不修复 / 🔄 调整方案，并附上修改意见。

---

## 一、正确性问题

### 1. ThenBy 多列排序不生效

**文件**: `RecordQuery.cs` — `ExecuteOrderBy`  
**现象**: `OrderBy("Age").ThenBy("Name")` 会产生两个独立排序 step，后者覆盖前者结果而非追加。  
**建议**: 将连续的 OrderBy/ThenBy 合并为一个复合排序 step，或在 ExecuteOrderBy 中检测前序排序状态并做稳定排序。

**标注**:需修复

---

### 2. BuildRowKey 字符串拼接存在哈希碰撞风险

**文件**: `RecordQuery.cs` — `BuildRowKey`  
**影响**: Distinct / Intersect / Except / GroupBy 均依赖此方法。若列值中包含与分隔符相同的字符，会产生误判（如 `("a|b", "c")` 与 `("a", "b|c")`）。  
**建议**: 对值进行转义或使用长度前缀编码；或改用结构化组合键。

**标注**:需修复

---

## 二、性能问题

### 3. 列数组扩容策略为线性增长

**文件**: `RecordColumn.T.cs` — `Extend` / `RecordColumnCollection.cs` — `OnAddRow`  
**现象**: `Extend(length)` 精确分配 length 大小的数组。当逐行 AddRow 超过初始 Capacity(20) 后，每次 AddRow 都可能触发数组拷贝（O(n²) 总拷贝量）。  
**建议**: 采用 2x 倍增策略：`int newLen = Math.Max(length, _data.Length * 2);`

**标注**:需修复

---

### 4. RecordQuery 每步产生完整中间 Record

**文件**: `RecordQuery.cs` — 所有 `Execute*` 方法  
**现象**: `Where().Select().OrderBy()` 链式调用会产生 N 个中间 Record，每个都含完整数组拷贝。  
**建议**: 可合并操作（如 Where+Select 一次遍历）；或采用索引传递方案（只传 `int[]` 行索引到最后一步物化）。

**标注**:先评估，我想知道方案细节。

**评估详情**:

当前每个 `Execute*` 方法都产生一个完整的 `Record` 中间结果。例如 `.Where().Select().OrderBy()` 链会依次产生 3 个独立的 Record，每个都包含完整的列数组拷贝。

**方案 A: 索引传递（推荐）**
- 核心思路：Where / OrderBy / Skip / Take 不产生新 Record，只返回 `int[]` 行索引数组
- 引入内部接口 `IQueryStep`，分两类：
  - `IndexStep`：输入 `int[]` → 输出 `int[]`（Where 过滤索引、OrderBy 排列索引、Skip/Take 截取索引）
  - `MaterializeStep`：输入 `Record + int[]` → 输出 `Record`（Select 投影、Join、GroupBy 必须物化）
- `ToRecord()` 执行时：连续的 IndexStep 合并为一趟索引变换，遇到 MaterializeStep 时才做一次物化
- 优点：Where→OrderBy→Take 只做 1 次物化而非 3 次；内存峰值降低 ~60%
- 缺点：需要重构 step 存储结构，影响所有 Execute 方法签名

**方案 B: 操作融合（局部优化）**
- 只合并特定相邻操作：Where+Select 融合为一趟遍历、Skip+Take 融合
- 在 `ToRecord()` 中分析 step 列表，发现可合并模式后调用融合版本
- 优点：改动小，向后兼容
- 缺点：优化有限，只覆盖常见模式

**方案 C: 延迟物化视图**
- `ExecuteWhere` 返回一个 "视图 Record"，内部持有源 Record 引用 + 行索引映射
- 下游操作读取视图时透传到源数据
- 优点：零拷贝
- 缺点：视图生命周期管理复杂，源数据被修改时语义不安全

**建议路径**: 先实施方案 B（低风险），积累性能数据后再考虑方案 A。

---

### 5. Delete 操作为 O(cols × rows)

**文件**: `RecordColumn.T.cs` — `Delete`  
**现象**: 每列逐元素向前移位。频繁删除场景退化严重。  
**建议**: 改为标记删除 + 延迟压缩；或提供批量删除 API 减少移位次数。

**标注**:提供批量删除 API 先行，标记删除方案太复杂，先不考虑。

---

### 6. SetValue 对值类型的 boxing

**文件**: `RecordColumn.cs` — `SetValue(object?, int)`  
**现象**: 通过 `RecordRow[key] = value` 路径写入值类型时必然 box/unbox。  
**建议**: 在 RecordRow 上提供泛型 `Set<T>(string name, T value)` 方法直达 `RecordColumn<T>.Set`。

**标注**:同意修改。

---

### 7. ToString 中逐字符 UTF-8 字节计算

**文件**: `Record.cs` — `bLength` / `bSubstring`  
**现象**: 逐字符调用 `Encoding.UTF8.GetByteCount(chars, i, 1)`，大 Record 调试输出时性能差。  
**建议**: 改用 `Rune` API（.NET 5+）或一次性 `GetByteCount` 后估算；或限制 ToString 输出行数。

**标注**:留到后面修改，可能需要调整 ToString 输出格式，先不急于优化。

---

## 三、设计问题

### 8. 分页属性与数据模型耦合

**文件**: `Record.cs` — `Page` / `MaxCount` / `PageSize` / `MaxPage`  
**现象**: 分页是视图/展示层关注点，混入数据容器增加了职责。  
**建议**: 抽成独立 `PagingInfo` 类，或通过 Record 的元数据字典附加。

**标注**: 业务使然，暂不调整。

---

### 9. QueryOptions.Indexes 已声明但未实际使用

**文件**: `QueryOptions.cs` — `Indexes` / `EnableIndexing`  
**现象**: Join 中仍每次临时构建 Dictionary，未利用预声明的索引列。  
**建议**: 在 ToRecord() 执行管道启动时根据 Options 预建索引，供后续 Join/GroupBy 复用。

**标注**: 索引功能后续实现时再完善，目前先保留接口设计。

---

### 10. Join 中 right.Clone() 在查询构建阶段急切执行

**文件**: `RecordQuery.cs` — `InnerJoin(Record, ...)` 等  
**现象**: 传入 `Record right` 的重载在 `_steps.Add` 的 lambda 中调用 `right.Clone()`，但 lambda 捕获的是原始引用——实际是延迟的（✓）。但若传入的 right 后续被修改，clone 时机在执行时才发生，语义是否符合预期需确认。  
**建议**: 明确文档约定；或在构建时快照 schema 用于校验，执行时再 clone 数据。

**标注**: 我需要更详细的问题描述，先不修改。

---

## 四、测试覆盖缺口

### 11. 缺少 ThenBy 多列排序正确性测试

**建议**: 添加 `OrderBy("Age").ThenBy("Name")` 的测试用例验证排序稳定性。

**标注**: 同意添加测试用例。

---

### 12. 缺少边界 case 测试

**缺失场景**:
- 空列名 / 特殊字符列名
- 全 null 值列的 Distinct / Join / GroupBy
- 超大行数（>10000）扩容验证
- Delete 后遍历一致性
- Delete 后再 AddRow 的数据正确性

**标注**: 同意添加测试用例。

---

### 13. 缺少 QueryOptions.Indexes 相关测试

**建议**: 当索引功能实现后补充；当前至少验证传入 Indexes 不会导致异常。

**标注**: 同意添加测试用例，但索引功能实现后再完善测试，目前先验证接口稳定性。

---

## 五、小问题

### 14. RecordColumn.SetValue 中 row 参数命名

**文件**: `RecordColumn.cs`  
**现象**: `SetValue(object? value, int row)` 参数顺序为 (value, row)，而 `RecordRow[key] = value` 的 set 也调用此方法。参数顺序与多数 API（先 index 后 value）相反。  
**建议**: 保持现状（已有大量调用），但注意文档说明。

**标注**: 可以修改，船小好掉头，注意单元测试。

---

> 标注完成后请保存，我将根据标注结果制定修复计划并逐项实施。
