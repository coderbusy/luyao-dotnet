# Copilot Instructions

## 项目指南
- 新增设计偏好：RecordSet 需支持与 DataSet 互操作；查询/连接链式调用倾向延迟执行并通过 ToRecord 物化；API 设计可参考 LINQ 风格。
- AsQuery 应支持传入 Options，并可在 Options 中声明需建立索引的列，以提升 Join 等操作性能。