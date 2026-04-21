using System.Dynamic;
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
            typeof(RecordRow).GetMethod(nameof(RecordRow.Get), new[] { typeof(string) })!
                             .MakeGenericMethod(typeof(object));

        private static readonly MethodInfo SetValueMethod =
            typeof(RecordRow).GetMethod(nameof(RecordRow.Set), new[] { typeof(string), typeof(object) })!;

        public RecordRowMetaObject(Expression expression, RecordRow value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        private Expression GetLimitedSelf()
            => Expression.Convert(Expression, typeof(RecordRow));

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
            var indexerSetter = typeof(RecordRow).GetProperty("Item")!.GetSetMethod()!;
            var convertedValue = Expression.Convert(value.Expression, typeof(object));
            var call = Expression.Call(
                GetLimitedSelf(),
                indexerSetter,
                Expression.Constant(binder.Name),
                convertedValue);
            // DLR requires the expression to return object, not void
            var block = Expression.Block(call, convertedValue);
            return new DynamicMetaObject(block, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
            var key = Expression.Convert(indexes[0].Expression, typeof(string));
            var call = Expression.Call(GetLimitedSelf(), GetValueMethod, key);
            return new DynamicMetaObject(call, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(RecordRow));
            var indexerSetter = typeof(RecordRow).GetProperty("Item")!.GetSetMethod()!;
            var key = Expression.Convert(indexes[0].Expression, typeof(string));
            var convertedValue = Expression.Convert(value.Expression, typeof(object));
            var call = Expression.Call(
                GetLimitedSelf(),
                indexerSetter,
                key,
                convertedValue);
            // DLR requires the expression to return object, not void
            var block = Expression.Block(call, convertedValue);
            return new DynamicMetaObject(block, restrictions);
        }
    }
}
