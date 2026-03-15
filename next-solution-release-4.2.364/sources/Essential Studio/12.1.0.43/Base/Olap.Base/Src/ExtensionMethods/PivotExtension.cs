#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;


#if !SILVERLIGHT
using System.Data;
using Syncfusion.Olap.Reports;
#else
using Syncfusion.OlapSilverlight.Reports;
#endif
namespace Syncfusion.Olap.Engine.Extension
{
#if !SILVERLIGHT
    [Serializable]
#endif
    /// <summary>
    /// Extension class for pivot operations.
    /// </summary>
    public static class PivotExtension
    {
        internal static Type GetObjectType(this IQueryable source)
        {
            var enumerable = source.GetEnumerator();
            var hasItem = enumerable.MoveNext();
            if (hasItem)
            {
                return enumerable.Current.GetType();
            }

            return null;
        }

        private static IList CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            System.Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return (IList)Activator.CreateInstance(specificType, args);
        }

        /// <summary>
        /// Groups many elements to one group.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="groupSelectors">The group selectors.</param>
        /// <returns>IEnumerable of group result.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements
            , int currentLevel
            , params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                //int level = currentLevel;


                return
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Level = currentLevel,
                            Items = g,
                            SubGroups = g.GroupByMany(currentLevel + 1, nextSelectors)
                        });
            }
            else
                return null;
        }

        /// <summary>
        /// Groups many elements to one group.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="groupSelectors">The group selectors.</param>
        /// <returns>IEnumerable of group result.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements
            , IEnumerable<Func<TElement, object>> groupSelectors)
        {
            return GroupByMany<TElement>(elements, 0, groupSelectors.ToArray());
        }


#if !SILVERLIGHT
        /// <summary>
        /// Group by rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="groupSelectors">The group selectors.</param>
        /// <returns>IEnumerable of group result.</returns>
        public static IEnumerable<GroupResult> GroupBy(
            this IEnumerable<DataRow> rows
            , params Expression<Func<DataRow, object>>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var funcDelg = selector.Compile();
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                return
                    rows.GroupBy(funcDelg).Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Items = g,

                            SubGroups = g.GroupBy(nextSelectors)
                        });
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Group by data table with properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>IEnumerable of group result.</returns>
        public static IEnumerable<GroupResult> GroupBy(this DataTable source, params string[] properties)
        {
            var enumerable = source.AsEnumerable();
            var sourceType = typeof(DataRow);
            List<Func<DataRow, object>> lambdas = new List<Func<DataRow, object>>();
            foreach (var property in properties)
            {
                // have a local reference of p, otherwise the Func lambda will refer to the last property finished by this iterator (this is by C# design)
                var p = property;
                Func<DataRow, object> functor = r => r.ItemArray[source.Columns.IndexOf(p)];
                lambdas.Add(functor);
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = typeof(PivotExtension).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var genericArgs = new Type[] { sourceType };
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] { enumerable, lambdas }) as IEnumerable<GroupResult>;

            foreach (var item in result)
            {
                yield return item;
            }
        }
#endif
        /// <summary>
        /// Groups the many elements to one group.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>IEnumerable of group result.</returns>
        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, IEnumerable<string> properties)
        {
            return GroupByMany(source, properties.ToArray());
        }

        /// <summary>
        /// Group by IQueryable source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>IEnumerable of group result.</returns>
        public static IEnumerable<GroupResult> GroupBy(this IQueryable source, params string[] properties)
        {
            if (properties.Length == 0)
            {
                yield return null;
            }

            var sourceType = source.GetObjectType();
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            foreach (var property in properties)
            {
                var param = Expression.Parameter(sourceType, sourceType.Name);
                var propertyExp = Expression.Property(param, property);
                var conv = Expression.Convert(propertyExp, typeof(Object));
                var lambdaExp = Expression.Lambda(conv, new ParameterExpression[] { param });
                lambdas.Add(lambdaExp);
            }
            var values = CreateGeneric(typeof(List<>), lambdas[0].Type);
            foreach (var lambdaExp in lambdas)
            {
                values.Add(lambdaExp.Compile());
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = typeof(PivotExtension).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic).ElementAt(1);
            var genericArgs = new Type[] { sourceType };
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] { source, values }) as IEnumerable<GroupResult>;
            foreach (var groupResult in result)
            {
                yield return groupResult;
            }
        }
    }

    /// <summary>
    /// Represents header information.
    /// </summary>
    public class HeaderInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderInfo"/> class.
        /// </summary>
        public HeaderInfo()
        {
            this.IsSummaryCell = false;
            this.HeaderCaptions = new List<string>();
            this.RowHeaderCaptions = new List<string>();
            this.ColumnHeaderCaptions = new List<string>();
        }

        /// <summary>
        /// Gets or sets the header captions.
        /// </summary>
        /// <value>The header captions.</value>
        public List<string> HeaderCaptions { get; set; }

        /// <summary>
        /// Gets or sets the row header captions.
        /// </summary>
        /// <value>The row header captions.</value>
        public List<string> RowHeaderCaptions { get; set; }

        /// <summary>
        /// Gets or sets the column header captions.
        /// </summary>
        /// <value>The column header captions.</value>
        public List<string> ColumnHeaderCaptions { get; set; }

        /// <summary>
        /// Gets or sets the summary info.
        /// </summary>
        /// <value>The summary info.</value>
        public SummaryInfo SummaryInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is summary cell.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is summary cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsSummaryCell { get; set; }

    }

#if !SILVERLIGHT
    [Serializable]
#endif
    /// <summary>
    /// Represents the group results.
    /// </summary>
    public class GroupResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupResult"/> class.
        /// </summary>
        public GroupResult()
        {
            this.Summary = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public object Key { get; set; }

        /// <summary>
        /// Gets or sets the group key.
        /// </summary>
        /// <value>The group key.</value>
        public string GroupKey { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level { get; set; }

        int _ElementsCount = -1;

        bool _HasChildren = false;

        /// <summary>
        /// Gets or sets the elements count.
        /// </summary>
        /// <value>The elements count.</value>
        public int ElementsCount
        {
            get
            {
                if (_ElementsCount == -1)
                {
                    if (this.SubGroups != null)
                    {
                        _ElementsCount += this.SubGroups.Count() + 1;
                        foreach (var item in this.SubGroups)
                        {
                            if (item.ElementsCount > 0)
                                _ElementsCount += item.ElementsCount;
                        }
                    }
                    if (_ElementsCount == -1)
                    {
                        this.HasChildren = false;
                        return 0;
                    }
                    if (_ElementsCount > 0)
                    {
                        this.HasChildren = true;
                    }
                    return _ElementsCount;
                }
                else
                    return _ElementsCount;
            }
            set
            {
                _ElementsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the SubGroups as list collection for manipulation.
        /// </summary>
        internal List<GroupResult> SubNodes { get; set; }

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        public object UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable Items { get; set; }

        /// <summary>
        /// Gets or sets the sub groups.
        /// </summary>
        /// <value>The sub groups.</value>
        public IEnumerable<GroupResult> SubGroups { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public Dictionary<string, object> Summary { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        { return string.Format("{0} ({1})", Key, ElementsCount); }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        internal bool HasChildren
        {
            get
            {
                return _HasChildren;
            }
            set
            {
                _HasChildren = value;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the state of the expandable.
        /// </summary>
        /// <value>The state of the expandable.</value>
        public Syncfusion.Olap.Engine.ExpandableState ExpandableState
        {
            get;
            set;
        }
#else
        public Syncfusion.OlapSilverlight.Engine.ExpandableState ExpandableState
        {
            get;
            set;
        }
#endif
    }
}
