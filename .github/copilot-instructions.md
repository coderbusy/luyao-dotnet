# Copilot Instructions

## 项目指南
- 新增设计偏好：RecordSet 需支持与 DataSet 互操作；查询/连接链式调用倾向延迟执行并通过 ToRecord 物化；API 设计可参考 LINQ 风格。
- 当对 Record 相关代码（src/LuYao.Common/Data/ 下的 Record、RecordRow、RecordSet、RecordColumn 等）进行任何变更时，必须同步更新 docs/Record.md 文档，保持实现与文档一致。