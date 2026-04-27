using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace LuYao.Data;

partial struct FrameRow
{
    /// <summary>
    /// 为 <see cref="FrameRow"/> 提供动态成员绑定支持。
    /// </summary>
    private sealed class FrameRowMetaObject : DynamicMetaObject
    {
        // dynamic 读取/写入走 this[string] 索引器
        private static readonly System.Reflection.PropertyInfo IndexerProperty =
            typeof(FrameRow).GetProperty("Item", new[] { typeof(string) })!;

        public FrameRowMetaObject(Expression expression, FrameRow value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        private Expression GetLimitedSelf()
            => Expression.Convert(Expression, typeof(FrameRow));

        /// <inheritdoc/>
        public override System.Collections.Generic.IEnumerable<string> GetDynamicMemberNames()
            => ((FrameRow)Value!).Frame.Columns.Select(c => c.Name);

        /// <inheritdoc/>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(FrameRow));
            var call = Expression.MakeIndex(GetLimitedSelf(), IndexerProperty, new[] { Expression.Constant(binder.Name) });
            return new DynamicMetaObject(call, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(FrameRow));
            var indexAccess = Expression.MakeIndex(GetLimitedSelf(), IndexerProperty, new[] { Expression.Constant(binder.Name) });
            var assign = Expression.Assign(indexAccess, Expression.Convert(value.Expression, typeof(object)));
            return new DynamicMetaObject(assign, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            if (indexes.Length != 1 || indexes[0].LimitType != typeof(string))
                return base.BindGetIndex(binder, indexes);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(FrameRow));
            var key = Expression.Convert(indexes[0].Expression, typeof(string));
            var call = Expression.MakeIndex(GetLimitedSelf(), IndexerProperty, new[] { key });
            return new DynamicMetaObject(call, restrictions);
        }

        /// <inheritdoc/>
        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            if (indexes.Length != 1 || indexes[0].LimitType != typeof(string))
                return base.BindSetIndex(binder, indexes, value);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, typeof(FrameRow));
            var key = Expression.Convert(indexes[0].Expression, typeof(string));
            var indexAccess = Expression.MakeIndex(GetLimitedSelf(), IndexerProperty, new[] { key });
            var assign = Expression.Assign(indexAccess, Expression.Convert(value.Expression, typeof(object)));
            return new DynamicMetaObject(assign, restrictions);
        }
    }
}
