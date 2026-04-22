using System.Dynamic;
using System.Linq.Expressions;

namespace LuYao.Data;

partial struct RecordRow
{
    /// <summary>
    /// 为 <see cref="RecordRow"/> 提供动态成员绑定支持。
    /// </summary>
    private sealed class RecordRowMetaObject : DynamicMetaObject
    {
        //// dynamic 读取走 GetValueOrDefault：列不存在返回 null。
        //private static readonly MethodInfo GetMethod =
        //    typeof(RecordRow).GetMethod(
        //        nameof(RecordRow.GetValueOrDefault),
        //        BindingFlags.Instance | BindingFlags.NonPublic)!;

        //// dynamic 写入走 SetAndEnsureColumn：列不存在时按运行时类型自动建列；null 值且列不存在则跳过。
        //private static readonly MethodInfo SetMethod =
        //    typeof(RecordRow).GetMethod(
        //        nameof(RecordRow.SetAndEnsureColumn),
        //        BindingFlags.Instance | BindingFlags.NonPublic)!;

        public RecordRowMetaObject(Expression expression, RecordRow value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        //private Expression GetLimitedSelf()
        //    => Expression.Convert(Expression, typeof(RecordRow));

        ///// <inheritdoc/>
        //public override System.Collections.Generic.IEnumerable<string> GetDynamicMemberNames()
        //    => ((RecordRow)Value!).Record.Columns.Select(c => c.Name);

        ///// <inheritdoc/>
        //public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        //{
        //    var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
        //    var call = Expression.Call(
        //        GetLimitedSelf(),
        //        GetMethod,
        //        Expression.Constant(binder.Name));
        //    return new DynamicMetaObject(call, restrictions);
        //}

        ///// <inheritdoc/>
        //public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        //{
        //    var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
        //    var param = Expression.Variable(typeof(object));
        //    var assign = Expression.Assign(param, Expression.Convert(value.Expression, typeof(object)));
        //    var call = Expression.Call(GetLimitedSelf(), SetMethod, Expression.Constant(binder.Name), param);
        //    // DLR requires the expression to return object, not void
        //    var block = Expression.Block(new[] { param }, assign, call, param);
        //    return new DynamicMetaObject(block, restrictions);
        //}

        ///// <inheritdoc/>
        //public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        //{
        //    if (indexes.Length != 1 || indexes[0].LimitType != typeof(string))
        //        return base.BindGetIndex(binder, indexes);
        //    var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
        //    var key = Expression.Convert(indexes[0].Expression, typeof(string));
        //    var call = Expression.Call(GetLimitedSelf(), GetMethod, key);
        //    return new DynamicMetaObject(call, restrictions);
        //}

        ///// <inheritdoc/>
        //public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        //{
        //    if (indexes.Length != 1 || indexes[0].LimitType != typeof(string))
        //        return base.BindSetIndex(binder, indexes, value);
        //    var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
        //    var key = Expression.Convert(indexes[0].Expression, typeof(string));
        //    var param = Expression.Variable(typeof(object));
        //    var assign = Expression.Assign(param, Expression.Convert(value.Expression, typeof(object)));
        //    var call = Expression.Call(GetLimitedSelf(), SetMethod, key, param);
        //    // DLR requires the expression to return object, not void
        //    var block = Expression.Block(new[] { param }, assign, call, param);
        //    return new DynamicMetaObject(block, restrictions);
        //}
    }
}
