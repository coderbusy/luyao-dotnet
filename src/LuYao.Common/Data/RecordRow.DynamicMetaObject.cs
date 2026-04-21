using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LuYao.Data;

partial struct RecordRow
{
    /// <summary>
    /// 为 <see cref="RecordRow"/> 提供动态成员绑定支持。
    /// </summary>
    private sealed class RecordRowMetaObject : DynamicMetaObject
    {
        private static readonly MethodInfo GetValueMethod =
            typeof(RecordRow).GetProperty("Item")!.GetGetMethod()!;

        private static readonly MethodInfo SetValueMethod =
            typeof(RecordRow).GetProperty("Item")!.GetSetMethod()!;

        public RecordRowMetaObject(Expression expression, RecordRow value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        private Expression GetLimitedSelf()
            => Expression.Convert(Expression, typeof(RecordRow));

        /// <inheritdoc/>
        public override System.Collections.Generic.IEnumerable<string> GetDynamicMemberNames()
            => ((RecordRow)Value!).Record.Columns.Select(c => c.Name);

        /// <inheritdoc/>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
            var call = Expression.Call(
                GetLimitedSelf(),
                GetValueMethod,
                Expression.Constant(binder.Name));
            return new DynamicMetaObject(call, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
            var param = Expression.Variable(typeof(object));
            var assign = Expression.Assign(param, Expression.Convert(value.Expression, typeof(object)));
            var call = Expression.Call(GetLimitedSelf(), SetValueMethod, Expression.Constant(binder.Name), param);
            // DLR requires the expression to return object, not void
            var block = Expression.Block(new[] { param }, assign, call, param);
            return new DynamicMetaObject(block, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            if (indexes.Length != 1 || indexes[0].LimitType != typeof(string))
                return base.BindGetIndex(binder, indexes);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
            var key = Expression.Convert(indexes[0].Expression, typeof(string));
            var call = Expression.Call(GetLimitedSelf(), GetValueMethod, key);
            return new DynamicMetaObject(call, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            if (indexes.Length != 1 || indexes[0].LimitType != typeof(string))
                return base.BindSetIndex(binder, indexes, value);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
            var key = Expression.Convert(indexes[0].Expression, typeof(string));
            var param = Expression.Variable(typeof(object));
            var assign = Expression.Assign(param, Expression.Convert(value.Expression, typeof(object)));
            var call = Expression.Call(GetLimitedSelf(), SetValueMethod, key, param);
            // DLR requires the expression to return object, not void
            var block = Expression.Block(new[] { param }, assign, call, param);
            return new DynamicMetaObject(block, restrictions);
        }
    }
}
