#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading;
    using System.ComponentModel;
    using System.Globalization;
    using Syncfusion.Data.Helper;
    using System.Collections.ObjectModel;
#if !WP7
    using System.Dynamic;
    using Syncfusion.Dynamic;
#endif
#if WPF
    using System.Data;
#endif

    /// <summary>
    /// Provides extension methods for Queryable source. 
    /// <para></para>
    /// <para></para>
    /// <para>var fonts = FontFamily.Families.AsQueryable(); </para>
    /// <para></para>
    /// <para></para>
    /// <para>We would normally write Expressions as, </para>
    /// <para></para>
    /// <code lang="C#">var names = new string[] {&quot;Tony&quot;, &quot;Al&quot;,
    /// &quot;Sean&quot;, &quot;Elia&quot;}.AsQueryable();
    /// names.OrderBy(n=&gt;n);</code>
    /// <para></para>
    /// <para></para>
    /// <para>This would sort the names based on alphabetical order. Like so, the
    /// Queryable extensions are a set of extension methods that define functions which
    /// will generate expressions based on the supplied values to the functions.</para>
    /// </summary>
    public static class QueryableExtensions
    {
#if WinRT
        private static readonly Type[] EmptyTypes = new Type[] {};
#else
        private static readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif

        public static IEnumerable OfQueryable(this IEnumerable items)
        {
            var enumerator = items.GetEnumerator();
            if (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    var type = enumerator.Current.GetType();
                    IQueryable queryable = items.AsQueryable();
                    return queryable.OfType(type);
                }
            }

            return items;
        }

        public static IEnumerable OfQueryable(this IEnumerable items, Type sourceType)
        {
            IQueryable queryable = items.AsQueryable();
            return queryable.OfType(sourceType);
        }

        /// <summary>
        /// Generates an AND binary expression for the given Binary expressions.
        /// <para></para>
        /// </summary>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        public static BinaryExpression AndPredicate(this Expression expr1, Expression expr2)
        {
            return Expression.And(expr1, expr2);
        }

        public static BinaryExpression AndAlsoPredicate(this Expression expr1, Expression expr2)
        {
            return Expression.AndAlso(expr1, expr2);
        }

        public static BinaryExpression OrElsePredicate(this Expression expr1, Expression expr2)
        {
            return Expression.OrElse(expr1, expr2);
        }

        public static int Count(this IQueryable source)
        {
            var sourceType = source.ElementType;
            return (Int32) source.Provider.Execute(
                Expression.Call(
                    typeof (Queryable),
                    "Count",
                    new Type[] {sourceType},
                    new Expression[] {source.Expression}
                    ));
        }

        public static object ElementAt(this IQueryable source, int index, Type sourceType)
        {
            return source.Provider.Execute(
                Expression.Call(
                    typeof (Queryable),
                    "ElementAt",
                    new Type[] {sourceType},
                    new Expression[] {source.Expression, Expression.Constant(index)}));
        }

        public static object ElementAt(this IQueryable source, int index)
        {
            var sourceType = source.ElementType;
            return source.ElementAt(index, sourceType);
        }

        public static object ElementAtOrDefault(this IQueryable source, int index, Type sourceType)
        {
            return source.Provider.Execute(
                Expression.Call(
                    typeof (Queryable),
                    "ElementAtOrDefault",
                    new Type[] {sourceType},
                    new Expression[] {source.Expression, Expression.Constant(index)}));
        }

        public static object ElementAtOrDefault(this IQueryable source, int index)
        {
            var sourceType = source.ElementType;
            return source.ElementAtOrDefault(index, sourceType);
        }

#if !SILVERLIGHT

        public static IQueryable GroupBy(this IQueryable source, IEnumerable<SortColumn> groupByNames, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            var selector = GenerateNew(groupByNames.Select(s => s.ColumnName).ToList(), paramExpression);
            var lambda = Expression.Lambda(selector, paramExpression);
            var method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "GroupBy" && m.GetParameters().Count() == 2);
            var methodCallExp = Expression.Call(null, method.MakeGenericMethod(new
                                                                                   Type[] {source.ElementType, lambda.Body.Type}),
                                                new Expression[] {source.Expression, lambda});
            var groupedSource = source.Provider.CreateQuery(methodCallExp);

            //.OrderBy()
            var sortParamExpression = Expression.Parameter(groupedSource.ElementType, "o");
            var mExp = Expression.PropertyOrField(sortParamExpression, "Key");
            groupByNames.IterateIndex<SortColumn>((i, s) =>
                {
                    if (s.SortDirection == ListSortDirection.Ascending)
                    {
                        if (i == 0)
                        {
                            groupedSource = groupedSource.OrderBy(sortParamExpression, Expression.PropertyOrField(mExp, s.ColumnName));
                        }
                        else
                        {
                            groupedSource = groupedSource.ThenBy(sortParamExpression, Expression.PropertyOrField(mExp, s.ColumnName));
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            groupedSource = groupedSource.OrderByDescending(sortParamExpression, Expression.PropertyOrField(mExp,s.ColumnName));
                        }
                        else
                        {
                            groupedSource = groupedSource.ThenByDescending(sortParamExpression, Expression.PropertyOrField(mExp, s.ColumnName));
                        }
                    }
                });

            //.Select()
            var groupparamExpression = Expression.Parameter(groupedSource.ElementType, "g");
            var nExp = Expression.New(typeof (GroupContext));
            var bindings = new List<MemberBinding>();
            //bindings.Add(Expression.Bind(typeof(GroupContext).GetMember("Key")[0], Expression.PropertyOrField(groupparamExpression, "Key")));
            bindings.Add(Expression.Bind(typeof (GroupContext).GetMember("Details").FirstOrDefault(),
                                         groupparamExpression));
            Expression e = Expression.MemberInit(nExp, bindings.ToArray());
            //g=>prop.Propertyname
            var selectLambda = Expression.Lambda(e, groupparamExpression);
            return groupedSource.Provider.CreateQuery(Expression.Call(
                typeof (Queryable), "Select",
                new Type[] {groupedSource.ElementType, typeof (GroupContext)},
                new Expression[] {groupedSource.Expression, selectLambda}));
        }

        public static IQueryable GroupBy(this IQueryable source, IEnumerable<SortColumn> groupByNames)
        {
            var sourceType = source.ElementType;
            return source.GroupBy(groupByNames, sourceType);
        }

        /// <summary>
        /// Generates the GroupBy Expression
        /// </summary>
        /// <param name="groupByName"></param>
        public static IQueryable GroupBy(this IQueryable source, string groupByName, string sortAction, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            // code for convet complex property to simple property
            var propertyName = groupByName;
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var lambda = Expression.Lambda(memExp, paramExpression);
            //return this.Source.Provider.CreateQuery(
            //.GroupBy()
            var groupedSource = source.Provider.CreateQuery(Expression.Call(
                typeof (Queryable), "GroupBy",
                new Type[]
                    {
                        source.ElementType,
                        sourceType.GetProperty(groupByName).PropertyType
                    },
                source.Expression,
                Expression.Quote(lambda)));

            //.OrderBy()
            ParameterExpression sortParamExpression = Expression.Parameter(groupedSource.ElementType, "o");
            // Code Make complex property to simple
            var mExp = Expression.PropertyOrField(sortParamExpression, "Key");
            var sortLambda = Expression.Lambda(mExp, sortParamExpression);
            var orderedSource = groupedSource.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    sortAction,
                    new Type[] {groupedSource.ElementType, sortLambda.Body.Type},
                    groupedSource.Expression,
                    sortLambda));

            //.Select()
            var groupparamExpression = Expression.Parameter(orderedSource.ElementType, "g");
            var bindings = new List<MemberBinding>();
            //bindings.Add(Expression.Bind(typeof(GroupContext).GetMember("Key")[0], Expression.PropertyOrField(groupparamExpression, "Key")));
            bindings.Add(Expression.Bind(typeof (GroupContext).GetMember("Details").FirstOrDefault(),
                                         groupparamExpression));
            Expression e = Expression.MemberInit(Expression.New(typeof (GroupContext)), bindings);
            //g=>prop.Propertyname
            var selectLambda = Expression.Lambda(e, groupparamExpression);
            var result = orderedSource.Provider.CreateQuery(Expression.Call(
                typeof (Queryable),
                "Select",
                new Type[] {orderedSource.ElementType, selectLambda.Body.Type},
                new Expression[] {orderedSource.Expression, selectLambda}));
            return result;
        }

        public static IQueryable GroupBy(this IQueryable source, string groupByName, string sortAction)
        {
            var sourceType = source.ElementType;
            return source.GroupBy(groupByName, sortAction, sourceType);
        }

#endif

        public static IQueryable OfType(this IQueryable source, Type sourceType)
        {
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable), "OfType",
                    new Type[] {sourceType}, new Expression[] {source.Expression}));
        }

#if !WPF
        public static ObservableCollection<T> OfType<T, K>(this ObservableCollection<K> items)
            where K : T
#else
        public static ObservableCollection<T> OfType<T, K>(this System.Windows.FreezableCollection<K> items)
            where K :System.Windows.DependencyObject, T
#endif
        {
            var newItems = new ObservableCollection<T>();
            foreach (var item in items)
            {
                var typeCastedItem = (T) item;
                if (typeCastedItem != null)
                {
                    newItems.Add(typeCastedItem);
                }
            }
            return newItems;
        }

        /// <summary>
        /// Generates a OrderBy query for the Queryable source.
        /// <para></para>
        /// <code lang="C#">            DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///             var orders = db.Orders.Skip(0).Take(10).ToList();
        ///             var queryable = orders.AsQueryable();
        ///             var sortedOrders =
        /// queryable.OrderBy(&quot;ShipCountry&quot;);</code>
        /// <para></para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static IQueryable OrderBy(this IQueryable source, string propertyName, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            //Expression memExp = paramExpression.GetValueExpression(propertyName, source.ElementType);
            //LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);			
            var lambda = GetLambdaWithComplexPropertyNullCheck(source, propertyName, paramExpression, sourceType);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable), "OrderBy",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression, lambda));
        }

        private static LambdaExpression GetLambdaWithComplexPropertyNullCheck(IQueryable source, string propertyName,
                                                                              ParameterExpression paramExpression, Type sourceType)
        {
            LambdaExpression lambda = null;
            var properties = propertyName.Split(new char[] {'.'});

            if (properties.GetLength(0) > 1)
            {
                //has complex properties... need to check each level for null & return null if any level is null...
                var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
                //make memExp type object so it can be compared with null below
                if (memExp.Type != typeof (object))
                {
                    memExp = Expression.Convert(memExp, typeof (object));
                }
                Expression memExp2 = null;
                string name = string.Empty;
                int count = properties.GetLength(0);
                for (int i = 0; i < count; i++)
                {
                    if (i == 0) //the first one
                    {
                        memExp2 = Expression.Equal(
                            paramExpression.GetValueExpression(properties[i], sourceType), Expression.Constant(null));
                        name = properties[i];
                    }
                    else if (i < count - 1) //don't add the last inner property check as it will be added after this loop
                    {
                        name += '.' + properties[i];
                        memExp2 = Expression.OrElse(memExp2,
                                Expression.Equal(paramExpression.GetValueExpression(name, sourceType), Expression.Constant(null)));
                    }
                }
#if !SyncfusionFramework3_5 && !WP7
                memExp2 = Expression.Condition(memExp2, Expression.Constant(null), memExp, typeof (object));
#else
                memExp2 = Expression.Condition(memExp2, Expression.Constant(null), memExp);
#endif
                lambda = Expression.Lambda(memExp2, paramExpression);
            }
            else
            {
                Expression memExp = paramExpression.GetValueExpression(propertyName, sourceType);
                lambda = Expression.Lambda(memExp, paramExpression);
            }
            return lambda;
        }

        public static IQueryable OrderBy(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.OrderBy(propertyName, sourceType);
        }

        public static IQueryable OrderBy(this IQueryable source, string propertyName,
                                         Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            return OrderBy(source, paramExpression, iExp);
        }

        public static IQueryable OrderBy(this IQueryable source, string propertyName, IComparer<object> comparer,
                                         Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            var lambda = Expression.Lambda(iExp, paramExpression);
            var method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Count() == 3);
            var conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            var methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable OrderBy(this IQueryable source, ParameterExpression paramExpression, Expression mExp)
        {
            var lambda = Expression.Lambda(mExp, paramExpression);
            var orderedSource = source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "OrderBy",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda
                    )
                );
            return orderedSource;
        }

        /// <summary>
        /// Generates an OrderBy query for the IComparer defined. 
        /// <para></para>
        /// <para> </para>
        /// <code lang="C#">   public class OrdersComparer :
        /// IComparer&lt;Order&gt;
        ///     {
        ///         public int Compare(Order x, Order y)
        ///         {
        ///             return string.Compare(x.ShipCountry, y.ShipCountry);
        ///         }
        ///     }</code>
        /// <para></para>
        /// <para><code lang="C#">var sortedOrders =
        /// db.Orders.Skip(0).Take(5).ToList().OrderBy(o =&gt; o, new
        /// OrdersComparer());</code></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        public static IQueryable OrderBy<T>(this IQueryable source, IComparer<T> comparer, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<T>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable OrderBy<T>(this IQueryable source, IComparer<T> comparer)
        {
            var sourceType = source.ElementType;
            return source.OrderBy<T>(comparer, sourceType);
        }

        public static IQueryable OrderBy(this IQueryable source, string propertyName, IComparer<object> comparer,
                                         Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            //var memExp = Expression.PropertyOrField(paramExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        /// <summary>
        /// Generates an OrderByDescending query for the IComparer defined. 
        /// <para></para>
        /// <para> </para>
        /// <code lang="C#">   public class OrdersComparer :
        /// IComparer&lt;Order&gt;
        ///     {
        ///         public int Compare(Order x, Order y)
        ///         {
        ///             return string.Compare(x.ShipCountry, y.ShipCountry);
        ///         }
        ///     }</code>
        /// <para></para>
        /// <para><code lang="C#">var sortedOrders =
        /// db.Orders.Skip(0).Take(5).ToList().OrderByDescending(o =&gt; o, new
        /// OrdersComparer());</code></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        public static IQueryable OrderByDescending<T>(this IQueryable source, IComparer<T> comparer, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<T>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string propertyName,
                                                   IComparer<object> comparer, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            // var memExp = Expression.PropertyOrField(paramExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable OrderByDescending<T>(this IQueryable source, IComparer<T> comparer)
        {
            var sourceType = source.ElementType;
            return source.OrderByDescending<T>(comparer, sourceType);
        }

        /// <summary>
        /// Generates a OrderByDescending query for the Queryable source.
        /// <para></para>
        /// <code lang="C#">            DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///             var orders = db.Orders.Skip(0).Take(10).ToList();
        ///             var queryable = orders.AsQueryable();
        ///             var sortedOrders =
        /// queryable.OrderByDescending(&quot;ShipCountry&quot;);</code>
        /// <para></para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static IQueryable OrderByDescending(this IQueryable source, string propertyName, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            //Expression memExp = paramExpression.GetValueExpression(propertyName, source.ElementType);
            //LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);

            LambdaExpression lambda = GetLambdaWithComplexPropertyNullCheck(source, propertyName, paramExpression, sourceType);

            //Previously  source.ElementType passed as parameter. This will leads to conflict when we use different classes derived from one interface. Now passing sourType as parameter to resolve this issue.
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "OrderByDescending",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda));
            //new Type[] { source.GetElementType(sourceType), lambda.Body.Type },
        }

        public static Expression GetExpression(this ParameterExpression paramExpression, string propertyName)
        {
            return paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            //Expression exp = null;
            //string[] propertyNameList = propertyName.Split('.');
            //foreach (string property in propertyNameList)
            //{
            //    if (exp != null)
            //    {
            //        exp = Expression.PropertyOrField(exp, property);
            //    }
            //    else
            //    {
            //        exp = Expression.PropertyOrField(paramExpression, property);
            //    }
            //}
            //return exp;
        }

        /// <summary>
        /// Generate expression from simple and complex property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="sourceType"></param>
        /// <param name="paramExpression"></param>
        /// <returns></returns>
        public static Expression GetValueExpression(this ParameterExpression paramExpression, string propertyName,
                                                    Type sourceType)
        {
            Expression exp = null;
#if WPF
            if (typeof(ICustomTypeDescriptor).IsAssignableFrom(sourceType))
            {
                //covert expression to ICustomTypeDescriptor
                Expression exp1 = System.Linq.Expressions.Expression.Convert(paramExpression, typeof(ICustomTypeDescriptor));
                exp = (System.Linq.Expressions.Expression<Func<ICustomTypeDescriptor, object, object>>)((t, o) => t.GetProperties()[propertyName].GetValue(o));
                //return the value as an expression.
                //exp = System.Linq.Expressions.Expression.Invoke(exp, new Expression[] { exp1, paramExpression });
                return exp;
            }
            else
            {
#endif
            // split the complex property to simple property and generate member expression
            string[] propertyNameList = propertyName.Split('.');
            foreach (string property in propertyNameList)
            {
                if (exp != null)
                {
                    exp = Expression.PropertyOrField(exp, property);
                }
                else
                {
                    exp = paramExpression;
                    if (paramExpression.Type != sourceType)
                        exp = Expression.Convert(paramExpression, sourceType);
                    exp = Expression.PropertyOrField(exp, property);
                }
            }
#if WPF
            }
#endif
            return exp;
        }

        public static IQueryable OrderByDescending(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.OrderByDescending(propertyName, sourceType);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string propertyName,
                                                   IComparer<object> comparer,
                                                   Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            LambdaExpression lambda = Expression.Lambda(iExp, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string propertyName,
                                                   Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            return OrderByDescending(source, paramExpression, iExp);
        }

        public static IQueryable OrderByDescending(this IQueryable source, ParameterExpression paramExpression,
                                                   Expression mExp)
        {
            LambdaExpression lambda = Expression.Lambda(mExp, paramExpression);
            var orderedSource = source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "OrderByDescending",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda
                    )
                );
            return orderedSource;
        }

        /// <summary>
        /// Generates an OR binary expression for the given Binary expressions.
        /// <para></para>
        /// </summary>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        public static BinaryExpression OrPredicate(this Expression expr1, Expression expr2)
        {
            return Expression.Or(expr1, expr2);
        }

        /// <summary>
        /// Creates a ParameterExpression that is required when building a series of
        /// predicates for the WHERE filter.
        /// <para></para>
        /// <code lang="C#">        DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///         var orders = db.Orders.Skip(0).Take(100).ToList();
        ///         var queryable = orders.AsQueryable();
        ///         var parameter =
        /// queryable.Parameter();</code>
        /// <para></para>
        /// <para></para>Use this same parameter passed to generate different predicates and
        /// finally to generate the Lambda.
        /// </summary>
        /// <remarks>
        /// If we specify a parameter for every predicate, then the Lambda expression scope
        /// will be out of the WHERE query that gets generated.
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static ParameterExpression Parameter(this IQueryable source)
        {
            var sourceType = source.ElementType;
            ParameterExpression paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            return paramExpression;
        }

        public static ParameterExpression Parameter(this Type sourceType)
        {
            return Expression.Parameter(sourceType, sourceType.Name);
        }

        public static Expression Equal(this ParameterExpression paramExpression, string propertyName, object value)
        {
            var memExp = paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            var result = NullableHelperInternal.FixDbNUllasNull(value, memExp.Type);
            result = NullableHelperInternal.ChangeType(result, memExp.Type);
            BinaryExpression bExp = Expression.Equal(memExp,
                                                     System.Linq.Expressions.Expression.Constant(result, memExp.Type));
            return bExp;
        }

        public static BinaryExpression Equal(this ParameterExpression paramExpression, string propertyName,
                                             string propertyName2)
        {
            var memExp = paramExpression.GetExpression(propertyName);
            var memExp2 = paramExpression.GetExpression(propertyName);
            BinaryExpression bExp = Expression.Equal(memExp, memExp2);
            return bExp;
        }

        public static Expression Equal(this ParameterExpression paramExpression, string propertyName, object value,
                                       Type elementType, Expression<Func<string, object, object>> expressionFunc)
        {
            // constructing a wrapper Func that would return typed value
#if WinRT
            var methods =
                typeof (QueryableExtensions).GetTypeInfo()
                                            .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                            .ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {elementType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExpression, propertyName, expressionFunc});
            value = NullableHelperInternal.ChangeType(value, elementType);
            Expression rightOperandExpression = Expression.Constant(value);
            if (rightOperandExpression.Type != elementType)
                rightOperandExpression = Expression.Convert(rightOperandExpression, invokeExp.Type);
            var bExp = Expression.Equal(invokeExp, rightOperandExpression);
            return bExp;
        }

        public static Expression NotEqual(this ParameterExpression paramExpression, string propertyName, object value,
                                          Type elementType, Expression<Func<string, object, object>> expressionFunc)
        {
            // constructing a wrapper Func that would return typed value
#if WinRT
            var methods =
                typeof (QueryableExtensions).GetTypeInfo()
                                            .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                            .ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {elementType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExpression, propertyName, expressionFunc});
            value = NullableHelperInternal.ChangeType(value, elementType);
            Expression rightOperandExpression = Expression.Constant(value);
            if (rightOperandExpression.Type != elementType)
                rightOperandExpression = Expression.Convert(rightOperandExpression, invokeExp.Type);
            var bExp = Expression.NotEqual(invokeExp, rightOperandExpression);
            return bExp;
        }

        public static BinaryExpression NotEqual(this ParameterExpression paramExpression, string propertyName,
                                                object value)
        {
            var memExp = paramExpression.GetExpression(propertyName);
            var result = NullableHelperInternal.FixDbNUllasNull(value, memExp.Type);
            result = NullableHelperInternal.ChangeType(result, memExp.Type);
            BinaryExpression bExp = Expression.NotEqual(memExp,
                                                        System.Linq.Expressions.Expression.Constant(result, memExp.Type));
            return bExp;
        }

        public static BinaryExpression NotEqual(this ParameterExpression paramExpression, string propertyName,
                                                string propertyName2)
        {
            var memExp = paramExpression.GetExpression(propertyName);
            var memExp2 = paramExpression.GetExpression(propertyName2);
            BinaryExpression bExp = Expression.NotEqual(memExp, memExp2);
            return bExp;
        }

        public static BinaryExpression GreaterThanOrEqual(this ParameterExpression paramExpression, string propertyName,
                                                          object value)
        {
            var memExp = paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            var result = NullableHelperInternal.FixDbNUllasNull(value, memExp.Type);
            result = NullableHelperInternal.ChangeType(result, memExp.Type);
            BinaryExpression bExp = Expression.GreaterThanOrEqual(memExp,
                                                                  System.Linq.Expressions.Expression.Constant(result,
                                                                                                              memExp
                                                                                                                  .Type));
            return bExp;
        }

        public static BinaryExpression GreaterThanOrEqual(this ParameterExpression paramExpression, string propertyName,
                                                          string propertyName2)
        {
            var memExp = paramExpression.GetExpression(propertyName);
            var memExp2 = paramExpression.GetExpression(propertyName2);
            BinaryExpression bExp = Expression.GreaterThanOrEqual(memExp, memExp2);
            return bExp;
        }

        public static Expression GreaterThanOrEqual(this ParameterExpression paramExpression, string propertyName,
                                                    object value, Type elementType,
                                                    Expression<Func<string, object, object>> expressionFunc)
        {
            // constructing a wrapper Func that would return Int32 value
#if WinRT
            var methods =
                typeof (QueryableExtensions).GetTypeInfo()
                                            .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                            .ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {elementType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExpression, propertyName, expressionFunc});
            value = NullableHelperInternal.ChangeType(value, elementType);
            Expression rightOperandExpression = Expression.Constant(value);
            if (rightOperandExpression.Type != elementType)
                rightOperandExpression = Expression.Convert(rightOperandExpression, invokeExp.Type);
            var bExp = Expression.GreaterThanOrEqual(invokeExp, rightOperandExpression);
            return bExp;
        }

        public static BinaryExpression GreaterThan(this ParameterExpression paramExpression, string propertyName,
                                                   object value)
        {
            var memExp = paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            var result = NullableHelperInternal.FixDbNUllasNull(value, memExp.Type);
            result = NullableHelperInternal.ChangeType(result, memExp.Type);
            BinaryExpression bExp = Expression.GreaterThan(memExp,
                                                           System.Linq.Expressions.Expression.Constant(result,
                                                                                                       memExp.Type));
            return bExp;
        }

        public static BinaryExpression GreaterThan(this ParameterExpression paramExpression, string propertyName,
                                                   string propertyName2)
        {
            var memExp = paramExpression.GetExpression(propertyName);
            var memExp2 = paramExpression.GetExpression(propertyName2);
            BinaryExpression bExp = Expression.GreaterThan(memExp, memExp2);
            return bExp;
        }

        public static Expression GreaterThan(this ParameterExpression paramExpression, string propertyName, object value,
                                             Type elementType, Expression<Func<string, object, object>> expressionFunc)
        {
            // constructing a wrapper Func that would return typed value
#if WinRT
            var methods =
                typeof (QueryableExtensions).GetTypeInfo()
                                            .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                            .ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {elementType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExpression, propertyName, expressionFunc});
            value = NullableHelperInternal.ChangeType(value, elementType);
            Expression rightOperandExpression = Expression.Constant(value);
            if (rightOperandExpression.Type != elementType)
                rightOperandExpression = Expression.Convert(rightOperandExpression, invokeExp.Type);
            var bExp = Expression.GreaterThan(invokeExp, rightOperandExpression);
            return bExp;
        }

        public static BinaryExpression LessThan(this ParameterExpression paramExpression, string propertyName,
                                                object value)
        {
            var memExp = paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            var result = NullableHelperInternal.FixDbNUllasNull(value, memExp.Type);
            result = NullableHelperInternal.ChangeType(result, memExp.Type);
            BinaryExpression bExp = Expression.LessThan(memExp,
                                                        System.Linq.Expressions.Expression.Constant(result, memExp.Type));
            return bExp;
        }

        public static BinaryExpression LessThan(this ParameterExpression paramExpression, string propertyName,
                                                string propertyName2)
        {
            var memExp = paramExpression.GetExpression(propertyName);
            var memExp2 = paramExpression.GetExpression(propertyName2);
            BinaryExpression bExp = Expression.LessThan(memExp, memExp2);
            return bExp;
        }

        public static Expression LessThan(this ParameterExpression paramExpression, string propertyName, object value,
                                          Type elementType, Expression<Func<string, object, object>> expressionFunc)
        {
            // constructing a wrapper Func that would return Int32 value
#if WinRT
            var methods =
                typeof (QueryableExtensions).GetTypeInfo()
                                            .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                            .ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {elementType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExpression, propertyName, expressionFunc});
            value = NullableHelperInternal.ChangeType(value, elementType);
            Expression rightOperandExpression = Expression.Constant(value);
            if (rightOperandExpression.Type != elementType)
                rightOperandExpression = Expression.Convert(rightOperandExpression, invokeExp.Type);
            var bExp = Expression.LessThan(invokeExp, rightOperandExpression);
            return bExp;
        }

        public static BinaryExpression LessThanOrEqual(this ParameterExpression paramExpression, string propertyName,
                                                       object value)
        {
            var memExp = paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            var result = NullableHelperInternal.FixDbNUllasNull(value, memExp.Type);
            result = NullableHelperInternal.ChangeType(result, memExp.Type);
            BinaryExpression bExp = Expression.LessThanOrEqual(memExp,
                                                               System.Linq.Expressions.Expression.Constant(result,
                                                                                                           memExp.Type));
            return bExp;
        }

        public static BinaryExpression LessThanOrEqual(this ParameterExpression paramExpression, string propertyName,
                                                       string propertyName2)
        {
            var memExp = paramExpression.GetValueExpression(propertyName, paramExpression.Type);
            var memExp2 = paramExpression.GetExpression(propertyName2);
            BinaryExpression bExp = Expression.LessThanOrEqual(memExp, memExp2);
            return bExp;
        }

        public static Expression LessThanOrEqual(this ParameterExpression paramExpression, string propertyName,
                                                 object value, Type elementType,
                                                 Expression<Func<string, object, object>> expressionFunc)
        {
            // constructing a wrapper Func that would return Int32 value
#if WinRT
            var methods =
                typeof (QueryableExtensions).GetTypeInfo()
                                            .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                            .ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {elementType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExpression, propertyName, expressionFunc});
            value = NullableHelperInternal.ChangeType(value, elementType);
            Expression rightOperandExpression = Expression.Constant(value);
            if (rightOperandExpression.Type != elementType)
                rightOperandExpression = Expression.Convert(rightOperandExpression, invokeExp.Type);
            var bExp = Expression.LessThanOrEqual(invokeExp, rightOperandExpression);
            return bExp;
        }

        /// <summary>
        /// Predicate is a Binary expression that needs to be built for a single or a series
        /// of values that needs to be passed on to the WHERE expression. 
        /// <para></para>
        /// <para></para>
        /// <code lang="C#">var binaryExp = queryable.Predicate(parameter,
        /// &quot;EmployeeID&quot;, &quot;4&quot;, true);</code>
        /// </summary>
        /// <remarks>
        /// First create a ParameterExpression using the Parameter extension function, then
        /// use the same ParameterExpression to generate the predicates.
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="paramExpression"></param>
        /// <param name="propertyName"></param>
        /// <param name="constValue"></param>
        /// <param name="filterType"></param>
        public static Expression Predicate(this IQueryable source, ParameterExpression paramExpression,
                                           string propertyName, object constValue, FilterType filterType,
                                           FilterBehavior filterBehaviour, bool isCaseSensitive, Type sourceType) //Predicate1
        {
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            return Predicate(source, constValue, filterType, filterBehaviour, isCaseSensitive, memExp.Type, memExp);
        }

        public static Expression Predicate(this IQueryable source, ParameterExpression paramExpression,
                                           string propertyName, object constValue, FilterType filterType,
                                           FilterBehavior filterBehaviour, bool isCaseSensitive, Type sourceType,
                                           /*Expression<Func<string, object, object>>*/ Delegate expressionFunc)
            //Predicate2
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }
            // var checkDelg = expressionFunc.Compile();
            // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
            Type memberType = null;

            if (filterBehaviour == FilterBehavior.StringTyped)
                memberType = typeof (string);
            else
            {
                while (!enumerator.MoveNext())
                {
                    var returnValue = expressionFunc.DynamicInvoke(new object[] { propertyName, enumerator.Current });
                    if (returnValue != null)
                    {
                        memberType = returnValue.GetType();
                        break;
                    }
                }
                if (memberType == null && constValue != null)
                    memberType = constValue.GetType();
                else
                {
                    filterBehaviour = FilterBehavior.StringTyped;
                    memberType = typeof(string);
                }
            }
            return Predicate(source, paramExpression, propertyName, constValue, memberType, filterType, filterBehaviour,
                             isCaseSensitive, sourceType, expressionFunc);
        }

        public static Expression Predicate(this IQueryable source, ParameterExpression paramExpression,
                                           string propertyName, object constValue, Type memberType,
                                           FilterType filterType, FilterBehavior filterBehaviour, bool isCaseSensitive,
                                           Type sourceType, /*Expression<Func<string, object, object>>*/
                                           Delegate expressionFunc) ////Predicate2
        {

            if (filterBehaviour == FilterBehavior.StronglyTyped)
            {
#if WinRT
                var methods = typeof (QueryableExtensions).GetTypeInfo().DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic).ToArray();
#else
                var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
                var wrapperFuncMethod = methods.FirstOrDefault(
                        m => m.Name == "GetDelegateInvokeExpressionAggregateFunc"
                        && m.IsStatic && m.IsPrivate &&m.IsGenericMethod); 
                
                var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {memberType});
                var memExp = (Expression)genericWrapper.Invoke(null, new object[]
                        {
                            paramExpression, propertyName, expressionFunc
                        });
                return Predicate(source, constValue, filterType, filterBehaviour, isCaseSensitive, memberType, memExp);
            }
            else
            {
                Func<Delegate, string, object, string> fun = (lambda, prop, rec) =>
                {
                    // var lambda = func.Compile();
                    var val = lambda.DynamicInvoke(new object[] { prop, rec });
                    if (val != null)
                        return val.ToString();
                    return null;
                };
                var invokeExp = Expression.Invoke(Expression.Constant(fun),
                                              new Expression[]
                                                  {
                                                      Expression.Constant(expressionFunc),
                                                      Expression.Constant(propertyName), paramExpression
                                                  });

                return Predicate(source, constValue, filterType, filterBehaviour, isCaseSensitive, memberType, invokeExp);
            }    
        }

        private static Expression Predicate(this IQueryable source, object constValue, FilterType filterType,
                                           FilterBehavior filterBehaviour, bool isCaseSensitive, Type memberType, Expression memExp)
        {
            if (filterBehaviour == FilterBehavior.StronglyTyped && (filterType == FilterType.EndsWith
                || filterType == FilterType.StartsWith || filterType == FilterType.Contains))
                throw new InvalidOperationException("FilterBehaviour and FilterType are not correct");

            if(filterBehaviour == FilterBehavior.StringTyped && (filterType == FilterType.GreaterThan ||
                filterType == FilterType.GreaterThanOrEqual || filterType == FilterType.LessThan || 
                filterType == FilterType.LessThanOrEqual))
                throw new InvalidOperationException("FilterBehaviour and FilterType are not correct");

            var value = constValue;
            Expression bExp = null;
            if (filterBehaviour == FilterBehavior.StronglyTyped &&
                (filterType == FilterType.Equals || filterType == FilterType.NotEquals ||
                 filterType == FilterType.LessThan || filterType == FilterType.LessThanOrEqual ||
                 filterType == FilterType.GreaterThan || filterType == FilterType.GreaterThanOrEqual))
            {
                var underlyingType = memberType;
                if (NullableHelperInternal.IsNullableType(memberType))
                {
                    underlyingType = NullableHelperInternal.GetUnderlyingType(memberType);
                }

                if (value != null)
                {
                    try
                    {
                        value = ValueConvert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                    }
                }
                var nullablememberType = NullableHelperInternal.GetNullableType(memberType);

                switch (filterType)
                {
                    case FilterType.Equals:
                        if (isCaseSensitive || memberType != typeof (string))
                        {
                            if (value != null)
                            {
                                var exp = Expression.Constant(value, memberType);
                                if (nullablememberType == memberType && memberType != typeof(object))
                                    bExp = Expression.Equal(memExp, Expression.Constant(value, memberType));
                                else
                                    bExp = Expression.Call(exp, exp.Type.GetMethod("Equals", new[] { memExp.Type }), memExp);
                            }
                            else
                            {
                                memExp = Expression.Convert(memExp, nullablememberType);
                                bExp = Expression.Equal(memExp, Expression.Constant(value, nullablememberType));
                                //bExp = Expression.Call(exp, nullablememberType.GetMethod("Equals", new[] { nullablememberType }), Expression.Constant(memExp));
                            }
                        }
                        else
                        {
                            memExp = Expression.Coalesce(memExp, Expression.Constant(string.Empty));
                            var toLowerMethodCall = memExp.ToLowerMethodCallExpression();
                            bExp = Expression.Equal(toLowerMethodCall,
                                                    Expression.Constant(
                                                        value == null ? null : value.ToString().ToLower(),
                                                        typeof (string)));
                        }
                        break;
                    case FilterType.NotEquals:
                        if (isCaseSensitive || memberType != typeof(string))
                        {
                            if (value != null)
                                bExp = Expression.NotEqual(memExp, Expression.Constant(value, memberType));
                            else
                            {
                                memExp = Expression.Convert(memExp, nullablememberType);
                                bExp = Expression.NotEqual(memExp, Expression.Constant(value, nullablememberType));
                            }
                        }
                        else
                        {
                            memExp = Expression.Coalesce(memExp, Expression.Constant(string.Empty));
                            var toLowerMethodCall = memExp.ToLowerMethodCallExpression();
                            bExp = Expression.NotEqual(toLowerMethodCall,
                                                       Expression.Constant(
                                                           value == null ? null : value.ToString().ToLower(),
                                                           memberType));
                        }
                        break;
                    case FilterType.LessThan:
                        bExp = Expression.LessThan(memExp, Expression.Constant(value, memberType));
                        break;
                    case FilterType.LessThanOrEqual:
                        bExp = Expression.LessThanOrEqual(memExp, Expression.Constant(value, memberType));
                        break;
                    case FilterType.GreaterThan:
                        bExp = Expression.GreaterThan(memExp, Expression.Constant(value, memberType));
                        break;
                    case FilterType.GreaterThanOrEqual:
                        bExp = Expression.GreaterThanOrEqual(memExp, Expression.Constant(value, memberType));
                        break;
                }
                return bExp;
            }

            if (!isCaseSensitive && (filterType == FilterType.Equals || filterType == FilterType.NotEquals))
                value = NullableHelperInternal.FixDbNUllasNull(constValue, memberType);

            var toString = memExp.Type.GetMethods().FirstOrDefault(d => d.Name == "ToString");
            if (NullableHelperInternal.IsNullableType(memberType) || memberType == typeof(string))
                memExp = Expression.Coalesce(memExp, Expression.Constant("Blanks"));
            memExp = Expression.Call(memExp, toString);
            var stringvalue = value == null ? "Blanks" : value.ToString();
            switch (filterType)
            {
                case FilterType.NotEquals:
                    if(!isCaseSensitive)
                        memExp = ToLowerMethodCallExpression(memExp);
                    bExp = Expression.NotEqual(memExp,
                                                Expression.Constant(stringvalue.ToLower(), typeof(string)));
                    break;
                case FilterType.Equals:
                case FilterType.StartsWith:
                case FilterType.Contains:
                case FilterType.EndsWith:
                    var stringMethod = typeof (string).GetMethods().FirstOrDefault(m => m.Name == filterType.ToString());
                    if (isCaseSensitive)
                    {
                        bExp = Expression.Call(memExp, stringMethod, new Expression[] { Expression.Constant(stringvalue, typeof(string)) });
                    }
                    else
                    {
                        var toLowerMethod = ToLowerMethodCallExpression(memExp);
                        bExp = Expression.Call(toLowerMethod, stringMethod,
                                    new Expression[]
                                        {
                                            Expression.Constant(stringvalue.ToLower(),typeof (string))
                                        });
                    }
                    break;
            }
            return bExp;
        }

        public static Expression Predicate(this IQueryable source, ParameterExpression paramExpression,
                                           string propertyName, object constValue, FilterType filterType,
                                           FilterBehavior filteBehaviour, bool isCaseSensitive, Type sourceType,
                                           string format) //Predicate3
        {
            Expression memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var memberType = memExp.Type;
            var underlyingType = memberType;

            if (NullableHelperInternal.IsNullableType(memberType))
            {
                underlyingType = NullableHelperInternal.GetUnderlyingType(memberType);
            }
            var value = NullableHelperInternal.FixDbNUllasNull(constValue, memberType);
            if (value != null)
            {
                if (memberType.Name == "Boolean")
                {
                    if ("true".Contains(value.ToString().ToLower()))
                    {
                        value = "1";
                    }
                    else if ("false".Contains(value.ToString().ToLower()))
                    {
                        value = "0";
                    }
                }
            }
            if (filterType == FilterType.Equals || filterType == FilterType.NotEquals ||
                filterType == FilterType.LessThan || filterType == FilterType.LessThanOrEqual ||
                filterType == FilterType.GreaterThan || filterType == FilterType.GreaterThanOrEqual)
            {
                Expression bExp = null;
                switch (filterType)
                {
                    case FilterType.Equals:
                        if (underlyingType != typeof (string))
                        {
                            if (!string.IsNullOrEmpty(format))
                            {
                                var formatMethodCall = GetFormatMethodCallExpression(memExp, format);
                                bExp = Expression.Equal(formatMethodCall, Expression.Constant(value, typeof(string)));
                            }
                            else
                            {
                                ConstantExpression exp = Expression.Constant(value, memberType);
                                //Expression.Equal can't compare DBNull. So Expression.Call used to compare DBNull value.
                                if (value != null)
                                    bExp = Expression.Call(exp, exp.Type.GetMethod("Equals", new[] { memExp.Type }), memExp);
                                else
                                    bExp = Expression.Equal(memExp, exp);
                            }
                        }
                        else
                        {
                            if (isCaseSensitive)
                            {
                                ConstantExpression exp = Expression.Constant(value, memberType);
                                //Expression.Equal can't compare DBNull. So Expression.Call used to compare DBNull value.
                                if (value != null)
                                    bExp = Expression.Call(exp, exp.Type.GetMethod("Equals", new[] { memExp.Type }), memExp);
                                else
                                    bExp = Expression.Equal(memExp, Expression.Constant(value, memberType));
                            }
                            else
                            {
                                var toLowerMethodCall =
                                    ToLowerMethodCallExpression(Expression.Coalesce(memExp,
                                                                                       Expression.Constant(string.Empty)));
                                bExp = Expression.Equal(toLowerMethodCall,
                                                        Expression.Constant(
                                                            value == null ? null : value.ToString().ToLower(),
                                                            memExp.Type));
                            }
                        }
                        break;
                    case FilterType.NotEquals:
                        if (underlyingType != typeof (string))
                        {
                            if (!string.IsNullOrEmpty(format))
                            {
                                var formatMethodCall = GetFormatMethodCallExpression(memExp, format);
                                bExp = Expression.NotEqual(formatMethodCall, Expression.Constant(value, typeof (string)));
                            }
                            else
                                bExp = Expression.NotEqual(memExp, Expression.Constant(value, memberType));
                        }
                        else
                        {
                            if (isCaseSensitive)
                            {
                                bExp = Expression.NotEqual(memExp, Expression.Constant(value, memberType));
                            }
                            else
                            {
                                var toLowerMethodCall = ToLowerMethodCallExpression(Expression.Coalesce(memExp,Expression.Constant(string.Empty)));
                                bExp = Expression.NotEqual(toLowerMethodCall,Expression.Constant(value == null ? null : value.ToString().ToLower(),memberType));
                            }
                        }
                        break;
                    case FilterType.LessThan:
                        if (!string.IsNullOrEmpty(format))
                        {
                            value = ValueConvert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture, format, true);
                            bExp = Expression.LessThan(memExp, Expression.Constant(value, memberType));
                        }
                        else
                            bExp = Expression.LessThan(memExp, Expression.Constant(value, memberType));
                        break;
                    case FilterType.LessThanOrEqual:
                        if (!string.IsNullOrEmpty(format))
                        {
                            var formatMethodCall = GetFormatMethodCallExpression(memExp, format);
                            Expression eqbExp = Expression.Equal(formatMethodCall,
                                                                 Expression.Constant(value, typeof (string)));
                            value = ValueConvert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture, format,
                                                            true);
                            bExp = Expression.Or(Expression.LessThan(memExp, Expression.Constant(value, memberType)),
                                                 eqbExp);
                        }
                        else
                            bExp = Expression.LessThanOrEqual(memExp, Expression.Constant(value, memberType));
                        break;
                    case FilterType.GreaterThan:
                        if (!string.IsNullOrEmpty(format))
                        {
                            var formatMethodCall = GetFormatMethodCallExpression(memExp, format);
                            Expression notbExp = Expression.NotEqual(formatMethodCall,
                                                                     Expression.Constant(value, typeof (string)));
                            value = ValueConvert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture, format,
                                                            true);
                            bExp = Expression.And(
                                Expression.GreaterThan(memExp, Expression.Constant(value, memberType)), notbExp);
                        }
                        else
                            bExp = Expression.GreaterThan(memExp, Expression.Constant(value, memberType));

                        break;
                    case FilterType.GreaterThanOrEqual:
                        value = ValueConvert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture, format, true);
                        bExp = Expression.GreaterThanOrEqual(memExp, Expression.Constant(value, memberType));
                        break;
                }
                return bExp;
            }
            else
            {
                var toString = memExp.Type.GetMethods().FirstOrDefault(d => d.Name == "ToString");
                memExp = Expression.Call(memExp, toString);

                Expression coalesceExp = Expression.Coalesce(memExp, Expression.Constant(string.Empty));
                var stringMethod = typeof (string).GetMethods().FirstOrDefault(m => m.Name == filterType.ToString());

                if (isCaseSensitive)
                {
                    var methodCallExp = Expression.Call(coalesceExp, stringMethod,
                        new Expression[] {Expression.Constant(value, typeof (string))});
                    return methodCallExp;
                }
                else
                {
                    var toLowerMethodCall = ToLowerMethodCallExpression(coalesceExp);
                    var methodCallExp = Expression.Call(
                        toLowerMethodCall,
                        stringMethod,
                        new Expression[]
                            {Expression.Constant(value == null ? null : value.ToString().ToLower(), typeof (string))});
                    return methodCallExp;
                }

                //Unreachable code
                //if (underlyingType != typeof (string) || filteBehaviour == FilterBehavior.StronglyTyped)
                //{
                //    return null;
                //}
                //else
                //{
                //    throw new InvalidOperationException("Underlying type is not a string");
                //}
            }
        }

        private static MethodCallExpression ToLowerMethodCallExpression(this Expression memExp)
        {
            var tolowerMethod = typeof (string).GetMethods().FirstOrDefault(m => m.Name == "ToLower");
            var toLowerMethodCall = Expression.Call(memExp, tolowerMethod, new Expression[0]);
            return toLowerMethodCall;
        }

        private static MethodCallExpression ToStringMethodCallExpression(this Expression memExp)
        {
            var toString = memExp.Type.GetMethods().FirstOrDefault(d => d.Name == "ToString");
            var coalesceExp = Expression.Coalesce(memExp, Expression.Constant(string.Empty));
            return Expression.Call(coalesceExp, toString);
        }
        
        private static MethodCallExpression GetFormatMethodCallExpression(Expression memExp, string format)
        {
            if (memExp.Type.IsGenericType() && memExp.Type.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                memExp = Expression.Call(memExp, "GetValueOrDefault", EmptyTypes);
            }
            var formatMethod = typeof (DateTime).GetMethod("ToString",
                                                           new Type[] {typeof (string), typeof (IFormatProvider)});
            if (memExp.Type == typeof (Decimal))
                formatMethod = typeof (Decimal).GetMethod("ToString",
                                                          new Type[] {typeof (string), typeof (IFormatProvider)});
            else if (memExp.Type == typeof (Double))
                formatMethod = typeof (Double).GetMethod("ToString",
                                                         new Type[] {typeof (string), typeof (IFormatProvider)});
            else if (memExp.Type == typeof (float))
                formatMethod = typeof (float).GetMethod("ToString",
                                                        new Type[] {typeof (string), typeof (IFormatProvider)});
            else if (memExp.Type == typeof (Int16))
                formatMethod = typeof (Int16).GetMethod("ToString",
                                                        new Type[] {typeof (string), typeof (IFormatProvider)});
            else if (memExp.Type == typeof (Int32))
                formatMethod = typeof (Int32).GetMethod("ToString",
                                                        new Type[] {typeof (string), typeof (IFormatProvider)});
            else if (memExp.Type == typeof (Int64))
                formatMethod = typeof (Int64).GetMethod("ToString",
                                                        new Type[] {typeof (string), typeof (IFormatProvider)});

            var formatMethodCall = Expression.Call(
                memExp,
                formatMethod, Expression.Constant(format), Expression.Constant(CultureInfo.CurrentCulture));
            return formatMethodCall;
        }

        /// <summary>
        /// Generates a Select query for a single property value.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static IQueryable Select(this IQueryable source, string propertyName, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            Expression memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "Select",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda));
        }

        public static IQueryable Select(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.Select(propertyName, sourceType);
        }

#if !SILVERLIGHT

        /// <summary>
        /// Generates a Select query based on the properties passed. 
        /// <para></para>
        /// <code lang="C#">            DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///             var orders = db.Orders.Skip(0).Take(10).ToList();
        ///             var queryable = orders.AsQueryable();
        ///             var selector = queryable.Select(new string[]{
        /// &quot;OrderID&quot;, &quot;ShipCountry&quot; });</code>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="properties"></param>
        public static IQueryable Select(this IQueryable source, params string[] properties)
        {
            return Select(source, properties.ToList());
        }

        /// <summary>
        /// Generates a Select query based on the properties passed. 
        /// <para></para>
        /// <code lang="C#">            DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///             var orders = db.Orders.Skip(0).Take(10).ToList();
        ///             var queryable = orders.AsQueryable();
        ///             var selector = queryable.Select(new List&lt;string&gt;() {
        /// &quot;OrderID&quot;, &quot;ShipCountry&quot; });</code>
        /// <para></para>
        /// <para>It returns a dynamic class generated thru ReflectionEmit, Use reflection
        /// to identify the properties and values.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="properties"></param>
        public static IQueryable Select(this IQueryable source, IEnumerable<string> properties, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            var selector = GenerateNew(properties, paramExpression);
            var lambda = Expression.Lambda(selector, paramExpression);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "Select",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda));
        }

        public static IQueryable Select(this IQueryable source, IEnumerable<string> properties)
        {
            var sourceType = source.ElementType;
            return source.Select(properties, sourceType);
        }
#endif

        /// <summary>
        /// Generates a SKIP expression in the IQueryable source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="constValue">The const value.</param>
        /// <returns></returns>
        public static IQueryable Skip(this IQueryable source, int constValue, Type sourceType)
        {
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "Skip",
                    new Type[] {source.ElementType},
                    new Expression[] {source.Expression, Expression.Constant(constValue)}));
        }

        public static IQueryable Skip(this IQueryable source, int constValue)
        {
            var sourceType = source.ElementType;
            return source.Skip(constValue, sourceType);
        }

        #region Aggregate extenions

        private static Expression GetInvokeExpressionAggregateFunc<TResult>(ParameterExpression paramExp,
                                                                            string propertyName,
                                                                            Expression<Func<string, object, object>>
                                                                                expressionFunc)
        {
            // constructing a wrapper Func that would return a generic value
            Func<Expression<Func<string, object, object>>, string, object, TResult> fun = (func, prop, rec) =>
                {
                    var lambda = func.Compile();
                    TResult val = (TResult) lambda.DynamicInvoke(new object[] {prop, rec});
                    return val;
                };

            //Expression<Func<Expression<Func<string, object, object>>, string, object, TResult>> eIFunc =
            //    (func, prop, rec) => fun(func, prop, rec);
            var invokeExp = Expression.Invoke(Expression.Constant(fun),
                                              new Expression[]
                                                  {
                                                      Expression.Constant(expressionFunc),
                                                      Expression.Constant(propertyName), paramExp
                                                  });
            return invokeExp;
        }

        /// <summary>
        /// Use this method with a cached delegate, this improves performance when using complex Expressions.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="paramExp"></param>
        /// <param name="propertyName"></param>
        /// <param name="expressionFunc"></param>
        /// <returns></returns>
        private static Expression GetDelegateInvokeExpressionAggregateFunc<TResult>(ParameterExpression paramExp, string propertyName, Delegate expressionFunc)
        {
            //constructing a wrapper Func that would return a generic value
            Func<Delegate, string, object, TResult> fun = (lambda, prop, rec) =>
                {
                    // var lambda = func.Compile();
                    var val = lambda.DynamicInvoke(new object[] {prop, rec});
                    if (val != null)
                        return (TResult) val;
                    return default(TResult);
                };

            //Expression<Func<Delegate, string, object, TResult>> eIFunc = (func, prop, rec) => fun(func, prop, rec);
            var invokeExp = Expression.Invoke(Expression.Constant(fun),
                                              new Expression[]
                                                  {
                                                      Expression.Constant(expressionFunc),
                                                      Expression.Constant(propertyName) , paramExp
                                                  });
            return invokeExp;
        }

        //MethodInfo[] collection is calculated frequently whenever the summary value changes.
        //To prevernt, this collection is calculated and it it used whenever the summary value changes.
        private static MethodInfo[] tMethod;
        private static MethodInfo[] tmethod
        {
            get
            {
                if (tMethod == null)
                {
                    return tMethod = typeof (Queryable).GetMethods().Where(m => m.Name == "Sum" && m.GetParameters().Count() == 2).ToArray();
                }
                return tMethod;
            }
        }

        //MethodInfo[] collection is calculated frequently whenever the summary value changes.
        //To prevernt, this collection is calculated and it it used whenever the summary value changes.
        private static MethodInfo[] emethods;

        private static MethodInfo[] eMethods
        {
            get
            {
                if (emethods == null)
                {
                    return
                        emethods =
                        typeof (EnumerableExtensions).GetMethods()
                                                     .Where(m => m.Name == "Sum" && m.GetParameters().Count() == 2)
                                                     .ToArray();
                }
                return emethods;
            }
        }

        #region Sum

        public static object Sum(this IQueryable source, string propertyName, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            //var memExp = Expression.PropertyOrField(paramExpression, propertyName);
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var lambda = Expression.Lambda(memExp, paramExpression);
            //Commented the below lines since it was declared as an property and used whenever needed.
            //var tmethod = typeof(Queryable).GetMethods()
            //    .Where(m => m.Name == "Sum" && m.GetParameters().Count() == 2).ToArray();
            var bodyType = lambda.Body.Type;
#if WPF
            if (typeof(ICustomTypeDescriptor).IsAssignableFrom(sourceType))
            {
                var enumerator = source.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    var current = enumerator.Current as ICustomTypeDescriptor;
                    var pdc = current.GetProperties();
                    if (pdc != null)
                    {
                        var pd = pdc.GetPropertyDescriptor(propertyName);
                        if (pd != null)
                        {
                            bodyType = pd.PropertyType;
                        }
                    }
                }
            }
#endif
            MethodInfo method = null;
            if (NullableHelperInternal.IsNullableType(bodyType))
            {
                var originalType = NullableHelperInternal.GetUnderlyingType(bodyType);
                switch (originalType.Name)
                {
                    case "Int32":
                        method = tmethod[1];
                        break;
                    case "Int64":
                        method = tmethod[3];
                        break;
                    case "Single":
                        method = tmethod[5];
                        break;
                    case "Double":
                        method = tmethod[7];
                        break;
                    case "Decimal":
                        method = tmethod[9];
                        break;
                }
            }
            else
            {
                switch (bodyType.Name)
                {
                    case "Int32":
                        method = tmethod[0];
                        break;
                    case "Int64":
                        method = tmethod[2];
                        break;
                    case "Single":
                        method = tmethod[4];
                        break;
                    case "Double":
                        method = tmethod[6];
                        break;
                    case "Decimal":
                        method = tmethod[8];
                        break;
                }
            }

            if (method == null &&
                (bodyType.Name == "Int16" || NullableHelperInternal.GetUnderlyingType(bodyType).Name == "Int16"))
            {
                //Commented the below lines since it was declared as an property and used whenever needed.
                //var eMethods = typeof(EnumerableExtensions).GetMethods().Where(m => m.Name == "Sum" && m.GetParameters().Count() == 2).ToArray();
                if (NullableHelperInternal.IsNullableType(bodyType))
                {
                    method = eMethods[2];
                }
                else
                {
                    method = eMethods[0];
                }
            }

            if (method != null)
            {
                return
                    source.Provider.Execute(Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType}), 
                    new Expression[] {source.Expression, Expression.Quote(lambda) } ));
            }

            return null;
        }

        public static object Sum(this IQueryable source, string propertyName)
        {
            return source.Sum(propertyName, source.ElementType);
        }

        public static object Sum(this IQueryable source, string propertyName, Expression<Func<string, object, object>> expressionFunc)
        {
            //Commented the below lines since it was declared as an property and used whenever needed.
            ////var tmethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Sum" && m.GetParameters().Count() == 2).ToArray();
            // determine the return type, we expect the sequence to be of same type from the expression and hence get the first value and simply take its type
            var enumerator = source.GetEnumerator();
            if (enumerator == null)
                return null;

            Type bodyType = null;
            var checkDelg = expressionFunc.Compile();
            // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.

            while (enumerator.MoveNext())
            {
                var returnValue = checkDelg.DynamicInvoke(new object[] { propertyName, enumerator.Current });
                if (returnValue != null)
                    bodyType = returnValue.GetType();
            }
            if (bodyType == null)
                return null;

            MethodInfo method = null;
#if !WP7
            var isDynamicBound = DynamicHelper.CheckIsDynamicObject(enumerator.Current.GetType());
            if (isDynamicBound && !NullableHelperInternal.IsNullableType(bodyType))
                bodyType = typeof(Nullable<>).MakeGenericType(bodyType);
#endif

            if (NullableHelperInternal.IsNullableType(bodyType))
            {
                var originalType = NullableHelperInternal.GetUnderlyingType(bodyType);
                switch (originalType.Name)
                {
                    case "Int32":
                        method = tmethod[1];
                        break;
                    case "Int64":
                        method = tmethod[3];
                        break;
                    case "Single":
                        method = tmethod[5];
                        break;
                    case "Double":
                        method = tmethod[7];
                        break;
                    case "Decimal":
                        method = tmethod[9];
                        break;
                }
            }
            else
            {
                switch (bodyType.Name)
                {
                    case "Int32":
                        method = tmethod[0];
                        break;
                    case "Int64":
                        method = tmethod[2];
                        break;
                    case "Single":
                        method = tmethod[4];
                        break;
                    case "Double":
                        method = tmethod[6];
                        break;
                    case "Decimal":
                        method = tmethod[8];
                        break;
                }
            }

            if (method == null && bodyType.Name == "Int16")
            {
                var eMethods =
                    typeof (EnumerableExtensions).GetMethods()
                                                 .Where(m => m.Name == "Sum" && m.GetParameters().Count() == 2)
                                                 .ToArray();
                if (NullableHelperInternal.IsNullableType(bodyType))
                {
                    method = eMethods[1];
                }
                else
                {
                    method = eMethods[0];
                }
            }

            if (method != null)
            {
                var sourceType = source.ElementType;
                var paramExp = Expression.Parameter(sourceType, sourceType.Name);
                var cExp = Expression.Constant(propertyName);
                var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExp});

                // constructing a wrapper Func that would return Int32 value
#if WinRT
                var methods =
                    typeof (QueryableExtensions).GetTypeInfo()
                                                .DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic)
                                                .ToArray();
#else
                var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
                var wrapperFuncMethod =
                    methods.FirstOrDefault(
                        m =>
                        m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
                var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {bodyType});
                var invokeExp =
                    (Expression) genericWrapper.Invoke(null, new object[] {paramExp, propertyName, expressionFunc});
                LambdaExpression lExp = Expression.Lambda(invokeExp, paramExp);
                var sumMethodCallExp = Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType}), new
                                                                                                                    Expression
                                                                                                                    []
                    {source.Expression, Expression.Quote(lExp)});
                var result = source.Provider.Execute(sumMethodCallExp);
                return result;
            }

            return 0;
        }

        #endregion

        #region Average

        public static object Average(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.Average(propertyName, sourceType);
        }

        public static object Average(this IQueryable source, string propertyName, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            //var memExp = Expression.PropertyOrField(paramExpression, propertyName);
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var lambda = Expression.Lambda(memExp, paramExpression);
            var tmethod = typeof (Queryable).GetMethods()
                                            .Where(m => m.Name == "Average" && m.GetParameters().Count() == 2).ToArray();
            var bodyType = lambda.Body.Type;
            MethodInfo method = null;
            if (NullableHelperInternal.IsNullableType(bodyType))
            {
                var originalType = NullableHelperInternal.GetUnderlyingType(bodyType);
                switch (originalType.Name)
                {
                    case "Int32":
                        method = tmethod[1];
                        break;
                    case "Single":
                        method = tmethod[3];
                        break;
                    case "Int64":
                        method = tmethod[5];
                        break;
                    case "Double":
                        method = tmethod[7];
                        break;
                    case "Decimal":
                        method = tmethod[9];
                        break;
                }
            }
            else
            {
                switch (bodyType.Name)
                {
                    case "Int32":
                        method = tmethod[0];
                        break;
                    case "Single":
                        method = tmethod[2];
                        break;
                    case "Int64":
                        method = tmethod[4];
                        break;
                    case "Double":
                        method = tmethod[6];
                        break;
                    case "Decimal":
                        method = tmethod[8];
                        break;
                }
            }

            if (method == null &&
                (bodyType.Name == "Int16" || NullableHelperInternal.GetUnderlyingType(bodyType).Name == "Int16"))
            {
                var eMethods = typeof (EnumerableExtensions).GetMethods().Where(m => m.Name == "Average" && m.GetParameters().Count() == 2).ToArray();
                if (NullableHelperInternal.IsNullableType(bodyType))
                {
                    method = eMethods[2];
                }
                else
                {
                    method = eMethods[0];
                }
            }

            if (method != null)
            {
                return source.Provider.Execute(Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType}),
                            new Expression[] {source.Expression, Expression.Quote(lambda)}));
            }

            return null;
        }

        public static object Average(this IQueryable source, string propertyName,
                                     Expression<Func<string, object, object>> expressionFunc)
        {
            var tmethod = typeof (Queryable).GetMethods().Where(m => m.Name == "Average" && m.GetParameters().Count() == 2).ToArray();
            // determine the return type, we expect the sequence to be of same type from the expression and hence get the first value and simply take its type
            var enumerator = source.GetEnumerator();
            if (enumerator == null)
                return null;

            Type bodyType = null;
            // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
            var checkDelg = expressionFunc.Compile();
            while (enumerator.MoveNext())
            {
                var returnValue = checkDelg.DynamicInvoke(new object[] { propertyName, enumerator.Current });
                if (returnValue != null)
                    bodyType = returnValue.GetType();
            }
            if (bodyType == null)
                return null;
#if !WP7
            var isDynamicBound = DynamicHelper.CheckIsDynamicObject(enumerator.Current.GetType());
            if (isDynamicBound && !NullableHelperInternal.IsNullableType(bodyType))
                bodyType = typeof(Nullable<>).MakeGenericType(bodyType);
#endif
            MethodInfo method = null;
            if (NullableHelperInternal.IsNullableType(bodyType))
            {
                var originalType = NullableHelperInternal.GetUnderlyingType(bodyType);
                switch (originalType.Name)
                {
                    case "Int32":
                        method = tmethod[1];
                        break;
                    case "Single":
                        method = tmethod[3];
                        break;
                    case "Int64":
                        method = tmethod[5];
                        break;
                    case "Double":
                        method = tmethod[7];
                        break;
                    case "Decimal":
                        method = tmethod[9];
                        break;
                }
            }
            else
            {
                switch (bodyType.Name)
                {
                    case "Int32":
                        method = tmethod[0];
                        break;
                    case "Single":
                        method = tmethod[2];
                        break;
                    case "Int64":
                        method = tmethod[4];
                        break;
                    case "Double":
                        method = tmethod[6];
                        break;
                    case "Decimal":
                        method = tmethod[8];
                        break;
                }
            }

            if (method == null && bodyType.Name == "Int16")
            {
                var eMethods = typeof (EnumerableExtensions).GetMethods().Where(m => m.Name == "Average" && m.GetParameters().Count() == 2).ToArray();
                if (NullableHelperInternal.IsNullableType(bodyType))
                {
                    method = eMethods[1];
                }
                else
                {
                    method = eMethods[0];
                }
            }

            if (method != null)
            {
                var sourceType = source.ElementType;
                var paramExp = Expression.Parameter(sourceType, sourceType.Name);
                var cExp = Expression.Constant(propertyName);
                var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExp});

                // constructing a wrapper Func that would return Int32 value
#if WinRT
                var methods = typeof (QueryableExtensions).GetTypeInfo().DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic).ToArray();
#else
                var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
                var wrapperFuncMethod = methods.FirstOrDefault(m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
                var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {bodyType});
                var invokeExp = (Expression) genericWrapper.Invoke(null, new object[] {paramExp, propertyName, expressionFunc});
                LambdaExpression lExp = Expression.Lambda(invokeExp, paramExp);
                var avgMethodCallExp = Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType}), 
                    new Expression[] {source.Expression, Expression.Quote(lExp)});
                var result = source.Provider.Execute(avgMethodCallExp);
                return result;
            }

            return 0;
        }

        #endregion

        #region Max

        public static object Max(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.Max(propertyName, sourceType);
        }

        public static object Max(this IQueryable source, string propertyName, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var lambda = Expression.Lambda(memExp, paramExpression);
            var method = typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "Max" && m.GetParameters().Count() == 2);
            var methodExp = Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType, lambda.Body.Type}),
                                            new Expression[] {source.Expression, lambda});
            return source.Provider.Execute(methodExp);
        }

        public static object Max(this IQueryable source, string propertyName,
                                 Expression<Func<string, object, object>> expressionFunc)
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }
            var checkDelg = expressionFunc.Compile();
            // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
            var returnValue = checkDelg.DynamicInvoke(new object[] {propertyName, enumerator.Current});
            var bodyType = returnValue.GetType();
            var sourceType = source.ElementType;
            var paramExp = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExp});

            // constructing a wrapper Func that would return Int32 value
#if WinRT
            var methods = typeof (QueryableExtensions).GetTypeInfo().DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic).ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod =
                methods.FirstOrDefault(
                    m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {bodyType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExp, propertyName, expressionFunc});
            LambdaExpression lExp = Expression.Lambda(invokeExp, paramExp);
            var method = typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "Max" && m.GetParameters().Count() == 2);
            var maxMethodCallExp = Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType, bodyType}),
                                                   new
                                                       Expression[] {source.Expression, Expression.Quote(lExp)});
            var result = source.Provider.Execute(maxMethodCallExp);
            return result;
        }

        #endregion

        #region Min

        public static object Min(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.Min(propertyName, sourceType);
        }

        public static object Min(this IQueryable source, string propertyName, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var lambda = Expression.Lambda(memExp, paramExpression);
            var method = typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "Min" && m.GetParameters().Count() == 2);
            var methodExp = Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType, lambda.Body.Type}),
                                            new Expression[] {source.Expression, lambda});
            return source.Provider.Execute(methodExp);
        }

        public static object Min(this IQueryable source, string propertyName,
                                 Expression<Func<string, object, object>> expressionFunc)
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }
            var checkDelg = expressionFunc.Compile();
            // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
            var returnValue = checkDelg.DynamicInvoke(new object[] {propertyName, enumerator.Current});
            var bodyType = returnValue.GetType();

            var sourceType = source.ElementType;
            var paramExp = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExp});

            // constructing a wrapper Func that would return Int32 value
#if WinRT
            var methods = typeof (QueryableExtensions).GetTypeInfo().DeclaredMethods.Where(m => m.IsStatic || !m.IsPublic).ToArray();
#else
            var methods = typeof(QueryableExtensions).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).ToArray();
#endif
            var wrapperFuncMethod = methods.FirstOrDefault(m => m.Name == "GetInvokeExpressionAggregateFunc" && m.IsStatic && m.IsPrivate && m.IsGenericMethod);
            var genericWrapper = wrapperFuncMethod.MakeGenericMethod(new Type[] {bodyType});
            var invokeExp =
                (Expression) genericWrapper.Invoke(null, new object[] {paramExp, propertyName, expressionFunc});
            LambdaExpression lExp = Expression.Lambda(invokeExp, paramExp);
            var method = typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "Min" && m.GetParameters().Count() == 2);
            var minMethodCallExp = Expression.Call(null, method.MakeGenericMethod(new Type[] {sourceType, bodyType}), new Expression[] {source.Expression, Expression.Quote(lExp)});
            var result = source.Provider.Execute(minMethodCallExp);
            return result;
        }

        #endregion

        #endregion

        /// <summary>
        /// Generates a TAKE expression in the IQueryable source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="constValue">The const value.</param>
        /// <returns></returns>
        public static IQueryable Take(this IQueryable source, int constValue, Type sourceType)
        {
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "Take",
                    new Type[] {sourceType},
                    new Expression[] {source.Expression, Expression.Constant(constValue)}));
        }

        public static IQueryable Take(this IQueryable source, int constValue)
        {
            var sourceType = source.ElementType;
            return source.Take(constValue, sourceType);
        }

        /// <summary>
        /// Generates a ThenBy query for the Queryable source. 
        /// <para></para>
        /// <code lang="C#">            DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///             var orders = db.Orders.Skip(0).Take(10).ToList();
        ///             var queryable = orders.AsQueryable();
        ///             var sortedOrders = queryable.OrderBy(&quot;ShipCountry&quot;);
        ///             sortedOrders = sortedOrders.ThenBy(&quot;ShipCity&quot;);</code>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static IQueryable ThenBy(this IQueryable source, string propertyName, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            Expression memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "ThenBy",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda));
        }

        public static IQueryable ThenBy(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.ThenBy(propertyName, sourceType);
        }

        public static IQueryable ThenBy(this IQueryable source, string propertyName, IComparer<object> comparer,
                                        Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            LambdaExpression lambda = Expression.Lambda(iExp, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenBy" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable ThenBy(this IQueryable source, string propertyName,
                                        Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            return ThenBy(source, paramExpression, iExp);
        }

        public static IQueryable ThenBy(this IQueryable source, ParameterExpression paramExpression, Expression mExp)
        {
            LambdaExpression lambda = Expression.Lambda(mExp, paramExpression);
            var orderedSource = source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "ThenBy",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda
                    )
                );
            return orderedSource;
        }

        /// <summary>
        /// Generates an ThenBy query for the IComparer defined. 
        /// <para></para>
        /// <para> </para>
        /// <code lang="C#">   public class OrdersComparer :
        /// IComparer&lt;Order&gt;
        ///     {
        ///         public int Compare(Order x, Order y)
        ///         {
        ///             return string.Compare(x.ShipCountry, y.ShipCountry);
        ///         }
        ///     }</code>
        /// <para></para>
        /// <para><code lang="C#">var sortedOrders =
        /// db.Orders.Skip(0).Take(5).ToList().ThenBy(o =&gt; o, new
        /// OrdersComparer());</code></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        public static IQueryable ThenBy<T>(this IQueryable source, IComparer<T> comparer, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenBy" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<T>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable ThenBy(this IQueryable source, string propertyName, IComparer<object> comparer,
                                        Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            // var memExp = Expression.PropertyOrField(paramExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenBy" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable ThenBy<T>(this IQueryable source, IComparer<T> comparer)
        {
            var sourceType = source.ElementType;
            return source.ThenBy<T>(comparer, sourceType);
        }

        /// <summary>
        /// Generates an ThenByDescending query for the IComparer defined. 
        /// <para></para>
        /// <para> </para>
        /// <code lang="C#">   public class OrdersComparer :
        /// IComparer&lt;Order&gt;
        ///     {
        ///         public int Compare(Order x, Order y)
        ///         {
        ///             return string.Compare(x.ShipCountry, y.ShipCountry);
        ///         }
        ///     }</code>
        /// <para></para>
        /// <para><code lang="C#">var sortedOrders =
        /// db.Orders.Skip(0).Take(5).ToList().ThenByDescending(o =&gt; o, new
        /// OrdersComparer());</code></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        public static IQueryable ThenByDescending<T>(this IQueryable source, IComparer<T> comparer, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenByDescending" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<T>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable ThenByDescending(this IQueryable source, string propertyName,
                                                  IComparer<object> comparer, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            // var memExp = Expression.PropertyOrField(paramExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(paramExpression, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenByDescending" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable ThenByDescending<T>(this IQueryable source, IComparer<T> comparer)
        {
            var sourceType = source.ElementType;
            return source.ThenByDescending<T>(comparer, sourceType);
        }

        public static IQueryable ThenByDescending(this IQueryable source, string propertyName,
                                                  IComparer<object> comparer,
                                                  Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            LambdaExpression lambda = Expression.Lambda(iExp, paramExpression);
            MethodInfo method =
                typeof (Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenByDescending" && m.GetParameters().Count() == 3);
            ConstantExpression conExp = Expression.Constant(comparer, typeof (IComparer<object>));
            MethodCallExpression methodExp = Expression.Call(null,
                                                             method.MakeGenericMethod(new Type[]
                                                                 {source.ElementType, lambda.Body.Type}),
                                                             new Expression[] {source.Expression, lambda, conExp});
            return source.Provider.CreateQuery(methodExp);
        }

        public static IQueryable ThenByDescending(this IQueryable source, string propertyName,
                                                  Expression<Func<string, object, object>> expressionFunc)
        {
            var sourceType = source.ElementType;
            var paramExpression = Expression.Parameter(sourceType, sourceType.Name);
            var cExp = Expression.Constant(propertyName);
            var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, paramExpression});
            return ThenByDescending(source, paramExpression, iExp);
        }

        public static IQueryable ThenByDescending(this IQueryable source, ParameterExpression paramExpression,
                                                  Expression mExp)
        {
            LambdaExpression lambda = Expression.Lambda(mExp, paramExpression);
            var orderedSource = source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "ThenByDescending",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda
                    )
                );
            return orderedSource;
        }

        /// <summary>
        /// Generates a ThenByDescending query for the Queryable source. 
        /// <para></para>
        /// <code lang="C#">            DataClasses1DataContext db = new
        /// DataClasses1DataContext();
        ///             var orders = db.Orders.Skip(0).Take(10).ToList();
        ///             var queryable = orders.AsQueryable();
        ///             var sortedOrders = queryable.OrderBy(&quot;ShipCountry&quot;);
        ///             sortedOrders = sortedOrders.ThenByDescending(&quot;ShipCity&quot;);</code>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static IQueryable ThenByDescending(this IQueryable source, string propertyName, Type sourceType)
        {
            ParameterExpression paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            Expression memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            //MemberExpression memExp = Expression.PropertyOrField(paramExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "ThenByDescending",
                    new Type[] {source.ElementType, lambda.Body.Type},
                    source.Expression,
                    lambda));
        }

        public static IQueryable ThenByDescending(this IQueryable source, string propertyName)
        {
            var sourceType = source.ElementType;
            return source.ThenByDescending(propertyName, sourceType);
        }

        /// <summary>
        /// Generates the where expression.
        /// <para></para>
        /// <code lang="C#">            var nw = new Northwind(@&quot;Data Source =
        /// Northwind.sdf&quot;);
        ///             IQueryable queryable = nw.Orders.AsQueryable();
        ///             var filters = queryable.Where(&quot;ShipCountry&quot;,
        /// &quot;z&quot;, FilterType.Contains);
        ///             foreach (Orders item in filters)
        ///             {
        ///                 Console.WriteLine(&quot;{0}/{1}&quot;, item.OrderID,
        /// item.ShipCountry);
        ///             }</code>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value"></param>
        /// <param name="filterType"></param>
        public static IQueryable Where(this IQueryable source, string propertyName, object value, FilterType filterType,
                                       bool isCaseSensitive, Type sourceType)
        {
            var paramExpression = Expression.Parameter(source.ElementType, sourceType.Name);
            // Code for convert complex property to simple property            
            var memExp = paramExpression.GetValueExpression(propertyName, sourceType);
            var underlyingType = memExp.Type;
            if (NullableHelperInternal.IsNullableType(memExp.Type))
            {
                underlyingType = NullableHelperInternal.GetUnderlyingType(memExp.Type);
            }
            if (filterType == FilterType.Equals || filterType == FilterType.NotEquals ||
                filterType == FilterType.LessThan || filterType == FilterType.LessThanOrEqual ||
                filterType == FilterType.GreaterThan || filterType == FilterType.GreaterThanOrEqual)
            {
                BinaryExpression bExp = null;
                switch (filterType)
                {
                    case FilterType.Equals:
                        if (underlyingType != typeof (string))
                        {
                            bExp = Expression.Equal(memExp, Expression.Constant(value, memExp.Type));
                        }
                        else
                        {
                            if (isCaseSensitive)
                            {
                                bExp = Expression.Equal(memExp, Expression.Constant(value, memExp.Type));
                            }
                            else
                            {
                                var toLowerMethodCall = ToLowerMethodCallExpression(memExp);
                                bExp = Expression.Equal(toLowerMethodCall,
                                                        Expression.Constant(
                                                            value == null ? null : value.ToString().ToLower(),
                                                            memExp.Type));
                            }
                        }
                        break;
                    case FilterType.NotEquals:
                        if (underlyingType != typeof (string))
                        {
                            bExp = Expression.NotEqual(memExp, Expression.Constant(value, memExp.Type));
                        }
                        else
                        {
                            if (isCaseSensitive)
                            {
                                bExp = Expression.NotEqual(memExp, Expression.Constant(value, memExp.Type));
                            }
                            else
                            {
                                var toLowerMethodCall = ToLowerMethodCallExpression(memExp);
                                bExp = Expression.NotEqual(toLowerMethodCall,
                                                           Expression.Constant(
                                                               value == null ? null : value.ToString().ToLower(),
                                                               memExp.Type));
                            }
                        }
                        break;
                    case FilterType.LessThan:
                        bExp = Expression.LessThan(memExp, Expression.Constant(value, memExp.Type));
                        break;
                    case FilterType.LessThanOrEqual:
                        bExp = Expression.LessThanOrEqual(memExp, Expression.Constant(value, memExp.Type));
                        break;
                    case FilterType.GreaterThan:
                        bExp = Expression.GreaterThan(memExp, Expression.Constant(value, memExp.Type));
                        break;
                    case FilterType.GreaterThanOrEqual:
                        bExp = Expression.GreaterThanOrEqual(memExp, Expression.Constant(value, memExp.Type));
                        break;
                }

                LambdaExpression lambda = Expression.Lambda(bExp, paramExpression);
                return source.Provider.CreateQuery(
                    Expression.Call(
                        typeof (Queryable),
                        "Where",
                        new Type[] {source.ElementType},
                        source.Expression,
                        lambda
                        )
                    );
            }
            else
            {
                var stringMethod =
                    typeof (string).GetMethods().Where(m => m.Name == filterType.ToString()).FirstOrDefault();
                Expression methodCallExp = null;
                if (isCaseSensitive)
                {
                    methodCallExp = Expression.Call(
                        memExp,
                        stringMethod,
                        new Expression[] {Expression.Constant(value, typeof (string))});
                }
                else
                {
                    var toLowerMethodCall = ToLowerMethodCallExpression(memExp);
                    methodCallExp = Expression.Call(
                        toLowerMethodCall,
                        stringMethod,
                        new Expression[]
                            {Expression.Constant(value == null ? null : value.ToString().ToLower(), typeof (string))});
                }
                var lambda = Expression.Lambda(methodCallExp, paramExpression);
                return source.Provider.CreateQuery(
                    Expression.Call(
                        typeof (Queryable),
                        "Where",
                        new Type[] {source.ElementType},
                        source.Expression,
                        lambda
                        )
                    );
            }
        }

        public static IQueryable Where(this IQueryable source, string propertyName, object value, FilterType filterType,
                                       bool isCaseSensitive)
        {
            var sourceType = source.ElementType;
            return source.Where(propertyName, value, filterType, isCaseSensitive, sourceType);
        }

        public static IQueryable Page(this IQueryable source, int pageIndex, int pageSize)
        {
            IQueryable tempSource = source;
            if (pageIndex > 0)
            {
                tempSource = tempSource.Skip(pageIndex*pageSize);
            }
            if (pageSize > 0)
            {
                tempSource = tempSource.Take(pageSize);
            }
            return tempSource;
        }

        /// <summary>
        /// Use this function to generate WHERE expression based on Predicates. The
        /// AndPredicate and OrPredicate should be used in combination to build the
        /// predicate expression which is finally passed on to this function for creating a
        /// Lambda. 
        /// <para></para>
        /// <para></para>
        /// <para></para>DataClasses1DataContext db = new DataClasses1DataContext();
        /// <para></para>            var orders = db.Orders.Skip(0).Take(100).ToList();
        /// <para></para>            var queryable = orders.AsQueryable();
        /// <para></para>            var parameter =
        /// queryable.Parameter(&quot;ShipCountry&quot;);
        /// <para></para>            var binaryExp = queryable.Predicate(parameter,
        /// <para></para>&quot;ShipCountry&quot;, &quot;USA&quot;, true);
        /// <para></para>            var filteredOrders = queryable.Where(parameter,
        /// binaryExp);
        /// <para></para>            foreach (var order in filteredOrders)
        /// <para></para>            {
        /// <para></para>                Console.WriteLine(order);
        /// <para></para>            }
        /// <para></para>
        /// <para></para>
        /// <para></para>Build Predicates for Contains / StartsWith / EndsWith,
        /// <para></para>
        /// <para></para>            IQueryable queryable = nw.Orders.AsQueryable();
        /// <para></para>            var parameter = queryable.Parameter();
        /// <para></para>            var exp1 = queryable.Predicate(parameter,
        /// &quot;ShipCountry&quot;, &quot;h&quot;, FilterType.Contains);
        /// <para></para>            var exp2 = queryable.Predicate(parameter,
        /// &quot;ShipCountry&quot;, &quot;a&quot;, FilterType.StartsWith);
        /// <para></para>            var andExp = exp2.OrPredicate(exp1);
        /// <para></para>            var filters = queryable.Where(parameter, andExp);
        /// <para></para>            foreach (Orders item in filters)
        /// <para></para>            {
        /// <para></para>                Console.WriteLine(&quot;{0}/{1}&quot;,
        /// item.OrderID, item.ShipCountry);
        /// <para></para>            }
        /// <para></para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="paramExpression"></param>
        /// <param name="predicateExpression"></param>
        public static IQueryable Where(this IQueryable source, ParameterExpression paramExpression,
                                       Expression predicateExpression)
        {
            var lambda = Expression.Lambda(predicateExpression, paramExpression);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof (Queryable),
                    "Where",
                    new Type[] {source.ElementType},
                    source.Expression,
                    lambda
                    )
                );
        }

        #region GroupByMany extensions

        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> elements,
                                                                     List<SortDescription> sortFields,
                                                                     IEnumerable<Func<TElement, object>> groupSelectors)
        {
            return GroupByMany<TElement>(elements, sortFields, groupSelectors.ToArray());
        }

        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> elements,
                                                                     List<SortDescription> sortFields,
                                                                     Dictionary<string, IComparer<object>> sortComparers,
                                                                     string[] properties,
                                                                     IEnumerable<Func<TElement, object>> groupSelectors)
        {
            return GroupByMany<TElement>(elements, sortFields, sortComparers, properties.ToList(),
                                         groupSelectors.ToArray());
        }

        //var orders = Orders.GroupBy(o => o.ShipCountry).Select(g => new { Key = g.Key, Items = g.OrderBy(o1 => o1.ShipCountry) });
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> elements,
                                                                     List<SortDescription> sortFields,
                                                                     Dictionary<string, IComparer<object>> sortComparers,
                                                                     List<string> properties,
                                                                     params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray();

                var groupBy =
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                            {
                                Key = g.Key,
                                Count = g.Count(),
                                Items = g,
                                SubGroups =
                                    g.GroupByMany(sortFields.Count > 0 ? sortFields.Skip(1).ToList() : sortFields,
                                                  sortComparers,
                                                  properties.Count() > 0 ? properties.Skip(0).ToList() : properties,
                                                  nextSelectors)
                            });

                if (sortFields.Count > 0)
                {
                    var sortKey = sortFields.FirstOrDefault(d => d.PropertyName == properties[0]);
                    if (sortKey.PropertyName != null && sortKey != default(SortDescription)) // && sortKey.Index == 0)
                    {
                        IComparer<object> customComparer = null;
                        sortComparers.TryGetValue(sortKey.PropertyName, out customComparer);

                        if (sortKey.Direction == ListSortDirection.Ascending)
                        {
                            if (customComparer == null)
                            {
                                groupBy = groupBy.OrderBy(g => g.Key);
                            }
                            else
                            {
                                groupBy = groupBy.OrderBy(g => g.Key, customComparer);
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                groupBy = groupBy.OrderByDescending(g => g.Key);
                            }
                            else
                            {
                                groupBy = groupBy.OrderByDescending(g => g.Key, customComparer);
                            }
                        }
                    }
                }

                return groupBy;
            }
            else
                return null;
        }

        //var orders = Orders.GroupBy(o => o.ShipCountry).Select(g => new { Key = g.Key, Items = g.OrderBy(o1 => o1.ShipCountry) });
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> elements,
                                                                     List<SortDescription> sortFields,
                                                                     params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray();

                var groupBy =
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                            {
                                Key = g.Key,
                                Count = g.Count(),
                                Items = g,
                                SubGroups =
                                    g.GroupByMany(sortFields.Count > 0 ? sortFields.Skip(1).ToList() : sortFields,
                                                  nextSelectors)
                            });

                if (sortFields.Count > 0)
                {
                    var sortKey = sortFields.First();
                    if (sortKey.PropertyName != null) // && sortKey.Index == 0)
                    {
                        if (sortKey.Direction == ListSortDirection.Ascending)
                        {
                            groupBy = groupBy.OrderBy(g => g.Key);
                        }
                        else
                        {
                            groupBy = groupBy.OrderByDescending(g => g.Key);
                        }
                    }
                }

                return groupBy;
            }
            else
                return null;
        }

        //var orders = Orders.GroupBy(o => o.ShipCountry).Select(g => new { Key = g.Key, Items = g.OrderBy(o1 => o1.ShipCountry) });
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> elements,
                                                                     params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray();

                return
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                            {
                                Key = g.Key,
                                Count = g.Count(),
                                Items = g,
                                SubGroups = g.GroupByMany(nextSelectors)
                            });
            }
            else
                return null;
        }

        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> elements,
                                                                     IEnumerable<Func<TElement, object>> groupSelectors)
        {
            return GroupByMany<TElement>(elements, groupSelectors.ToArray());
        }

        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, IEnumerable<string> properties)
        {
            return GroupByMany(source, properties.ToArray());
        }

        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, Type sourceType,
                                                           params string[] properties)
        {
            return source.GroupByMany(null, sourceType, properties);
        }

        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, Dictionary<string, string> formatColl,
                                                           Type sourceType, params string[] properties)
        {
            if (properties.Length == 0)
            {
                return null;
            }
            string format = string.Empty;
            var lambdas = new List<LambdaExpression>();
            foreach (var property in properties)
            {
                format = string.Empty;
                var param = Expression.Parameter(source.ElementType, sourceType.Name);
                // Code to convert complex property to simple property
                var propertyExp = param.GetValueExpression(property, sourceType);
                var conv = Expression.Convert(propertyExp, typeof (Object));
                if (formatColl != null)
                {
                    if (formatColl.Keys.Contains(property))
                    {
                        format = formatColl.Where(key => key.Key == property).ToList().FirstOrDefault().Value;
                        if (format.Contains(property))
                            format = format.Replace(property, "0");
                        if (!string.IsNullOrEmpty(format))
                            format = System.Text.RegularExpressions.Regex.Match(format, @"{0:(.*?)}").Groups[1].Value;
                    }
                    if (!string.IsNullOrEmpty(format) && propertyExp.Type != typeof (string))
                    {
                        var formatMethodCall = GetFormatMethodCallExpression(propertyExp, format);
                        conv = Expression.Convert(formatMethodCall, typeof (Object));
                    }
                }

                var lambdaExp = Expression.Lambda(conv, new ParameterExpression[] {param});
                lambdas.Add(lambdaExp);
            }
            var values = CreateGeneric(typeof (List<>), lambdas[0].Type);
            foreach (var lambdaExp in lambdas)
            {
                values.Add(lambdaExp.Compile());
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = GetGroupByManyMethod();
                //typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var genericArgs = new Type[] {source.ElementType};
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] {source, values}) as IEnumerable<GroupResult>;
            return result;
        }

        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, Type sourceType,
                                                           List<SortDescription> sortFields, params string[] properties)
        {
            if (properties.Length == 0)
            {
                return null;
            }

            var lambdas = new List<LambdaExpression>();
            foreach (var property in properties)
            {
                var param = Expression.Parameter(source.ElementType, sourceType.Name);
                // Code to convert complex property to simple property
                var propertyExp = param.GetValueExpression(property, sourceType);
                var conv = Expression.Convert(propertyExp, typeof (Object));
                var lambdaExp = Expression.Lambda(conv, new ParameterExpression[] {param});
                lambdas.Add(lambdaExp);
            }
            var values = CreateGeneric(typeof (List<>), lambdas[0].Type);
            foreach (var lambdaExp in lambdas)
            {
                values.Add(lambdaExp.Compile());
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = GetGroupByManyMethod2();
                //typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var genericArgs = new Type[] {source.ElementType};
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result =
                genericMethodInfo.Invoke(null, new object[] {source, sortFields, values}) as IEnumerable<GroupResult>;
            return result;
        }

        public static IEnumerable<GroupResult> GroupByMany(this IEnumerable source, Type sourceType,
                                                           Func<string, Expression> GetExpressionFunc,
                                                           params string[] properties)
        {
            if (properties.Length == 0)
            {
                return null;
            }

            var elementType = source.GetElementType();
            var lambdas = new List<LambdaExpression>();
            foreach (var property in properties)
            {
                var param = Expression.Parameter(elementType, sourceType.Name);
                // Code to convert complex property to simple property
                var expressionFunc = GetExpressionFunc(property);
                if (expressionFunc == null)
                {
                    var propertyExp = param.GetValueExpression(property, sourceType);
                    var conv = Expression.Convert(propertyExp, typeof (Object));
                    var lambdaExp = Expression.Lambda(conv, new ParameterExpression[] {param});
                    lambdas.Add(lambdaExp);
                }
                else
                {
                    var cExp = Expression.Constant(property);
                    var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, param});
                    var lambdaExp = Expression.Lambda(iExp, param);
                    lambdas.Add(lambdaExp);
                }
            }
            var values = CreateGeneric(typeof (List<>), lambdas[0].Type);
            foreach (var lambdaExp in lambdas)
            {
                values.Add(lambdaExp.Compile());
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = GetGroupByManyMethod();
                //typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var genericArgs = new Type[] {elementType};
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] {source, values}) as IEnumerable<GroupResult>;
            return result;
        }

        public static IEnumerable<GroupResult> GroupByMany(this IEnumerable source, Type sourceType,
                                                           List<SortDescription> sortFields,
                                                           Dictionary<string, IComparer<object>> sortComparers,
                                                           Func<string, Expression> GetExpressionFunc,
                                                           params string[] properties)
        {
            if (properties.Length == 0)
            {
                return null;
            }

            var elementType = source.GetElementType();
            var lambdas = new List<LambdaExpression>();
            foreach (var property in properties)
            {
                var param = Expression.Parameter(elementType, sourceType.Name);
                // Code to convert complex property to simple property
                var expressionFunc = GetExpressionFunc(property);
                if (expressionFunc == null)
                {
                    var propertyExp = param.GetValueExpression(property, sourceType);
                    var conv = Expression.Convert(propertyExp, typeof (Object));
                    var lambdaExp = Expression.Lambda(conv, new ParameterExpression[] {param});
                    lambdas.Add(lambdaExp);
                }
                else
                {
                    var cExp = Expression.Constant(property);
                    var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, param});
                    var lambdaExp = Expression.Lambda(iExp, param);
                    lambdas.Add(lambdaExp);
                }
            }
            var values = CreateGeneric(typeof (List<>), lambdas[0].Type);
            foreach (var lambdaExp in lambdas)
            {
                values.Add(lambdaExp.Compile());
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = GetGroupByManyMethod3();
                //typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var genericArgs = new Type[] {elementType};
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result =
                genericMethodInfo.Invoke(null, new object[] {source, sortFields, sortComparers, properties, values}) as
                IEnumerable<GroupResult>;
            return result;
            //foreach (var groupResult in result)
            //{
            //    yield return groupResult;
            //}
        }

        public static IEnumerable<GroupResult> GroupByMany(this IEnumerable source, Type sourceType,
                                                           List<SortDescription> sortFields,
                                                           Func<string, Expression> GetExpressionFunc,
                                                           params string[] properties)
        {
            if (properties.Length == 0)
            {
                return null;
            }

            var elementType = source.GetElementType();
            var lambdas = new List<LambdaExpression>();
            foreach (var property in properties)
            {
                var param = Expression.Parameter(elementType, sourceType.Name);
                // Code to convert complex property to simple property
                var expressionFunc = GetExpressionFunc(property);
                if (expressionFunc == null)
                {
                    var propertyExp = param.GetValueExpression(property, sourceType);
                    var conv = Expression.Convert(propertyExp, typeof (Object));
                    var lambdaExp = Expression.Lambda(conv, new ParameterExpression[] {param});
                    lambdas.Add(lambdaExp);
                }
                else
                {
                    var cExp = Expression.Constant(property);
                    var iExp = Expression.Invoke(expressionFunc, new Expression[] {cExp, param});
                    var lambdaExp = Expression.Lambda(iExp, param);
                    lambdas.Add(lambdaExp);
                }
            }
            var values = CreateGeneric(typeof (List<>), lambdas[0].Type);
            foreach (var lambdaExp in lambdas)
            {
                values.Add(lambdaExp.Compile());
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            var methodInfo = GetGroupByManyMethod2();
                //typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var genericArgs = new Type[] {elementType};
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result =
                genericMethodInfo.Invoke(null, new object[] {source, sortFields, values}) as IEnumerable<GroupResult>;
            return result;
            //foreach (var groupResult in result)
            //{
            //    yield return groupResult;
            //}
        }

        private static MethodInfo GetGroupByManyMethod()
        {
            MethodInfo method = null;
            var methods =
                typeof (QueryableExtensions).GetMethods()
                                            .Where(
                                                m =>
                                                m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod);
            foreach (var m in methods)
            {
                var pInfo = m.GetParameters();
                if (pInfo.Length > 1)
                {
                    var p = pInfo[1];
                    if (typeof (IEnumerable<>).Name == p.ParameterType.Name)
                    {
                        method = m;
                        break;
                    }
                }
                if (method != null)
                {
                    break;
                }
            }
            return method;
        }

        private static MethodInfo GetGroupByManyMethod2()
        {
            MethodInfo method = null;
            var methods =
                typeof (QueryableExtensions).GetMethods()
                                            .Where(
                                                m =>
                                                m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod);
            foreach (var m in methods)
            {
                var pInfo = m.GetParameters();
                if (pInfo.Length > 2)
                {
                    var p = pInfo[2];
                    if (typeof (IEnumerable<>).Name == p.ParameterType.Name)
                    {
                        method = m;
                        break;
                    }
                }
                if (method != null)
                {
                    break;
                }
            }
            return method;
        }

        private static MethodInfo GetGroupByManyMethod3()
        {
            MethodInfo method = null;
            var methods =
                typeof (QueryableExtensions).GetMethods()
                                            .Where(
                                                m =>
                                                m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod);
            foreach (var m in methods)
            {
                var pInfo = m.GetParameters();
                if (pInfo.Length > 3)
                {
                    var p = pInfo[2];
                    if (typeof (Dictionary<string, IComparer<object>>).Name == p.ParameterType.Name)
                    {
                        method = m;
                        break;
                    }
                }
                if (method != null)
                {
                    break;
                }
            }
            return method;
        }

        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, params string[] properties)
        {
            var sourceType = source.ElementType;
            return source.GroupByMany(sourceType, properties);
        }

        public static IEnumerable<GroupResult> GroupByMany(this IQueryable source, Dictionary<string, string> formatcoll,
                                                           params string[] properties)
        {
            var sourceType = source.ElementType;
            return source.GroupByMany(formatcoll, sourceType, properties);
        }

#if WPF
        public static IEnumerable<GroupResult> GroupByMany(this DataTable source, params string[] properties)
        {
            //return GroupByMany(source, new List<SortDescription>(), properties);
            var enumerable = source.AsEnumerable();
            var sourceType = typeof(DataRow);
            var lambdas = new List<Func<DataRow, object>>();
            foreach (var property in properties)
            {
                // have a local reference of p, otherwise the Func lambda will refer to the last property finished by this iterator (this is by C# design)
                var p = property;
                Func<DataRow, object> functor = r => r.ItemArray[source.Columns.IndexOf(p)];
                lambdas.Add(functor);
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            //var methodInfo = typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var methodInfo = GetGroupByManyMethod();
            var genericArgs = new Type[] { sourceType };
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] { enumerable, lambdas }) as IEnumerable<GroupResult>;
            return result;
        }

        public static IEnumerable<GroupResult> GroupByMany(this DataTable source, List<SortDescription> sortFields, params string[] properties)
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
            //var methodInfo = typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var methodInfo = GetGroupByManyMethod();
            var genericArgs = new Type[] { sourceType };
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] { enumerable, sortFields, lambdas }) as IEnumerable<GroupResult>;
            return result;
        }

        public static IEnumerable<GroupResult> GroupByMany(this DataView source, params string[] properties)
        {
            var enumerable = source.Cast<DataRowView>();
            var sourceType = typeof(DataRowView);
            var lambdas = new List<Func<DataRowView, object>>();
            foreach (var property in properties)
            {
                // have a local reference of p, otherwise the Func lambda will refer to the last property finished by this iterator (this is by C# design)
                var p = property;
                Func<DataRowView, object> functor = (r) =>
                {
                    var data = r[source.Table.Columns.IndexOf(p)];
                    return data;
                };
                lambdas.Add(functor);
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            //var methodInfo = typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var methodInfo = GetGroupByManyMethod();
            var genericArgs = new Type[] { sourceType };
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] { enumerable, lambdas }) as IEnumerable<GroupResult>;
            return result;
        }

        public static IEnumerable<GroupResult> GroupByMany(this DataView source, List<SortDescription> sortFields, params string[] properties)
        {
            //var enumerable = source.AsEnumerable();
            var enumerable = source.Cast<DataRowView>();
            var sourceType = typeof(DataRowView);
            var lambdas = new List<Func<DataRowView, object>>();
            foreach (var property in properties)
            {
                // have a local reference of p, otherwise the Func lambda will refer to the last property finished by this iterator (this is by C# design)
                var p = property;
                Func<DataRowView, object> functor = (r) =>
                {
                    var data = r[source.Table.Columns.IndexOf(p)];
                    return data;
                };
                lambdas.Add(functor);
            }
            // ElementAt(1) is the GroupByMany method with IEnumerable<T> properties
            //var methodInfo = typeof(QueryableExtensions).GetMethods().Where(m => m.Name == "GroupByMany" && m.IsStatic && m.IsPublic && m.IsGenericMethod).ElementAt(1);
            var methodInfo = GetGroupByManyMethod2();
            var genericArgs = new Type[] { sourceType };
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgs);
            var result = genericMethodInfo.Invoke(null, new object[] { enumerable, sortFields, lambdas }) as IEnumerable<GroupResult>;
            return result;
        }
#endif

        #endregion

        private static IList CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            System.Type specificType = generic.MakeGenericType(new System.Type[] {innerType});
            return (IList) Activator.CreateInstance(specificType, args);
        }

        public static Type GetObjectType(this IQueryable source)
        {
            var enumerable = source.ElementType;
            return enumerable;
        }

        internal static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
#if WinRT || WP
            return null;
#else
            return ClassFactory.Instance.GetDynamicClass(properties);
#endif
        }

        internal static Type CreateClass(params DynamicProperty[] properties)
        {
#if WinRT || WP
            return null;
#else
            return ClassFactory.Instance.GetDynamicClass(properties);
#endif
        }

        private static Expression GenerateNew(IEnumerable<string> properties, ParameterExpression paramExpression)
        {
            var expressions = new List<Expression>();
            var dynamicProperties = new List<DynamicProperty>();
            foreach (var property in properties)
            {
                var exp = Expression.PropertyOrField(paramExpression, property);
                expressions.Add(exp);
                dynamicProperties.Add(new DynamicProperty(property, exp.Type));
            }
            var classType = CreateClass(dynamicProperties);
            var bindings = new List<MemberBinding>();
            for (int i = 0; i < dynamicProperties.Count; i++)
            {
                bindings.Add(Expression.Bind(classType.GetProperty(dynamicProperties[i].Name), expressions[i]));
            }
            return Expression.MemberInit(Expression.New(classType), bindings);
        }
    }

    public class GroupResult
    {
        public object Key { get; set; }
        public int Count { get; set; }
        public IEnumerable Items { get; set; }
        public IEnumerable<GroupResult> SubGroups { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Key, Count);
        }
    }

    public class SortDescriptionIndex
    {
        public int Index { get; set; }
        public SortDescription SortDescription { get; set; }
    }
}