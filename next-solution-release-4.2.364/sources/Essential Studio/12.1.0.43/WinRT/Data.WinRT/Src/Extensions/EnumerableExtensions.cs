#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#if WinRT
using System.Runtime.CompilerServices;
#endif

namespace Syncfusion.Data.Extensions
{
    public static class EnumerableExtensions
    {
        #region Average

        public static double Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16>(Expression.Call(null,
                                                               ((MethodInfo)
                                                                typeof (TSource).GetTypeInfo()
                                                                                .GetDeclaredMethod(CallerName()))
                                                                   .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                               new Expression[]
                                                                   {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16> selector)
        {
            return source.Select<TSource, Int16>(selector).Average();
        }

        public static double Average(this IEnumerable<Int16> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            long num = 0L;
            long num2 = 0L;
            foreach (int num3 in source)
            {
                num += num3;
                num2 += 1L;
            }

            if (num2 <= 0L)
            {
                throw new InvalidOperationException("Not enough elements");
            }
            return (((double) num)/((double) num2));
        }

        public static double? Average<TSource>(this IQueryable<TSource> source,
                                               Expression<Func<TSource, Int16?>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16?>(Expression.Call(null,
                                                                ((MethodInfo)
                                                                 typeof (TSource).GetTypeInfo()
                                                                                 .GetDeclaredMethod(CallerName()))
                                                                    .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                                new Expression[]
                                                                    {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16?>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16?> selector)
        {
            return source.Select<TSource, Int16?>(selector).Average();
        }

        public static double? Average(this IEnumerable<Int16?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            long num = 0L;
            long num2 = 0L;
            foreach (Int16? nullable in source)
            {
                if (nullable.HasValue)
                {
                    num += (long) nullable.GetValueOrDefault();
                    num2 += 1L;
                }
            }

            if (num2 > 0L)
            {
                return new double?(((double) num)/((double) num2));
            }
            return null;
        }

        #endregion

        #region Sum

        public static Int16 Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16>(Expression.Call(null,
                                                               ((MethodInfo)
                                                                typeof (TSource).GetTypeInfo()
                                                                                .GetDeclaredMethod(CallerName()))
                                                                   .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                               new Expression[]
                                                                   {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static Int16 Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16> selector)
        {
            return source.Select<TSource, Int16>(selector).Sum();
        }

        public static Int16 Sum(this IEnumerable<Int16> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Int16 num = 0;
            foreach (var num2 in source)
            {
                num += num2;
            }
            return num;
        }

        public static Int16? Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16?>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16?>(Expression.Call(null,
                                                                ((MethodInfo)
                                                                 typeof (TSource).GetTypeInfo()
                                                                                 .GetDeclaredMethod(CallerName()))
                                                                    .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                                new Expression[]
                                                                    {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16?>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static Int16? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16?> selector)
        {
            return source.Select<TSource, Int16?>(selector).Sum();
        }

        public static Int16? Sum(this IEnumerable<Int16?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Int16 num = 0;
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    num += nullable.GetValueOrDefault();
                }
            }

            return new Int16?(num);
        }

        #endregion

        #region Max

        public static Int16 Max<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16>(Expression.Call(null,
                                                               ((MethodInfo)
                                                                typeof (TSource).GetTypeInfo()
                                                                                .GetDeclaredMethod(CallerName()))
                                                                   .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                               new Expression[]
                                                                   {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static Int16 Max<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16> selector)
        {
            return source.Select<TSource, Int16>(selector).Max();
        }

        public static Int16 Max(this IEnumerable<Int16> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Int16 num = 0;
            bool flag = false;
            foreach (var num2 in source)
            {
                if (flag)
                {
                    if (num2 > num)
                    {
                        num = num2;
                    }
                }
                else
                {
                    num = num2;
                    flag = true;
                }
            }
            if (!flag)
            {
                throw new InvalidOperationException("Not enough elements");
            }
            return num;
        }

        public static Int16? Max<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16?>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16?>(Expression.Call(null,
                                                                ((MethodInfo)
                                                                 typeof (TSource).GetTypeInfo()
                                                                                 .GetDeclaredMethod(CallerName()))
                                                                    .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                                new Expression[]
                                                                    {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16?>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static Int16? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16?> selector)
        {
            return source.Select<TSource, Int16?>(selector).Max();
        }

        public static Int16? Max(this IEnumerable<Int16?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Int16? nullable = null;
            foreach (var nullable2 in source)
            {
                if (nullable.HasValue)
                {
                    var nullable3 = nullable2;
                    var nullable4 = nullable;
                    if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault()) ||
                        !(nullable3.HasValue & nullable4.HasValue))
                    {
                        continue;
                    }
                }

                nullable = nullable2;
            }
            return nullable;
        }

        #endregion

        #region Min

        public static Int16 Min<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16>(Expression.Call(null,
                                                               ((MethodInfo)
                                                                typeof (TSource).GetTypeInfo()
                                                                                .GetDeclaredMethod(CallerName()))
                                                                   .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                               new Expression[]
                                                                   {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static Int16 Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16> selector)
        {
            return source.Select<TSource, Int16>(selector).Min();
        }

        public static Int16 Min(this IEnumerable<Int16> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Int16 num = 0;
            bool flag = false;
            foreach (var num2 in source)
            {
                if (flag)
                {
                    if (num2 < num)
                    {
                        num = num2;
                    }
                }
                else
                {
                    num = num2;
                    flag = true;
                }
            }

            if (!flag)
            {
                throw new InvalidOperationException("Not enough elements");
            }
            return num;
        }

        public static Int16? Min<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int16?>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
#if WinRT
            return
                source.Provider.Execute<Int16?>(Expression.Call(null,
                                                                ((MethodInfo)
                                                                 typeof (TSource).GetTypeInfo()
                                                                                 .GetDeclaredMethod(CallerName()))
                                                                    .MakeGenericMethod(new Type[] {typeof (TSource)}),
                                                                new Expression[]
                                                                    {source.Expression, Expression.Quote(selector)}));
#else
            return source.Provider.Execute<Int16?>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
#endif
        }

        public static Int16? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Int16?> selector)
        {
            return source.Select<TSource, Int16?>(selector).Min();
        }

        public static Int16? Min(this IEnumerable<Int16?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Int16? nullable = null;
            foreach (var nullable2 in source)
            {
                if (nullable.HasValue)
                {
                    long? nullable3 = nullable2;
                    long? nullable4 = nullable;
                    if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault()) ||
                        !(nullable3.HasValue & nullable4.HasValue))
                    {
                        continue;
                    }
                }
                nullable = nullable2;
            }

            return nullable;
        }

        #endregion
        
        #region Sorting

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.OrderBy(e => GetFunc(propertyName, e));
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.OrderBy(e => propertyInfo.GetValue(e, null));
            }
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.OrderByDescending(e => GetFunc(propertyName, e));
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.OrderByDescending(e => propertyInfo.GetValue(e, null));
            }
        }

        public static IEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.ThenBy(e => GetFunc(propertyName, e));
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.ThenBy(e => propertyInfo.GetValue(e, null));
            }
        }

        public static IEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.ThenByDescending(e => GetFunc(propertyName, e));
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.ThenByDescending(e => propertyInfo.GetValue(e, null));
            }
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc, IComparer<object> comparer)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.OrderBy(e => GetFunc(propertyName, e), comparer);
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.OrderBy(e => propertyInfo.GetValue(e, null), comparer);
            }
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc, IComparer<object> comparer)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;
            if (GetFunc != null)
                return entities.OrderByDescending(e => GetFunc(propertyName, e), comparer);
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.OrderByDescending(e => propertyInfo.GetValue(e, null), comparer);
            }
            
        }

        public static IEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc, IComparer<object> comparer)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.ThenBy(e => GetFunc(propertyName, e), comparer);
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.ThenBy(e => propertyInfo.GetValue(e, null), comparer);
            }
        }

        public static IEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> entities, string propertyName, Func<string, object, object> GetFunc, IComparer<object> comparer)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            if (GetFunc != null)
                return entities.ThenByDescending(e => GetFunc(propertyName, e), comparer);
            else
            {
                var propertyInfo = entities.First().GetType().GetProperty(propertyName);
                return entities.ThenByDescending(e => propertyInfo.GetValue(e, null), comparer);
            }
        }
     
        #endregion

#if !SyncfusionFramework3_5 && !SILVERLIGHT && !WP

        public static ParallelQuery<T> GetParallelQueryFor<T>(IEnumerable source)
        {
            var enumerable = source as IEnumerable<T>;
            return enumerable.AsParallel();
        }

        public static ParallelQuery GetParallelQuery(this IEnumerable source, Type sourceType)
        {
            sourceType = source.GetElementType();
#if WinRT
            var genericWrapper =
                typeof (EnumerableExtensions).GetTypeInfo()
                                             .DeclaredMethods.FirstOrDefault(
                                                 m => m.Name == "GetParallelQueryFor" && m.IsStatic && m.IsGenericMethod);
#else
            var genericWrapper = typeof(EnumerableExtensions).GetMethods().FirstOrDefault(m => m.Name == "GetParallelQueryFor" && m.IsStatic && m.IsGenericMethod);
#endif
            genericWrapper = genericWrapper.MakeGenericMethod(sourceType);
            var parallelQuery = (ParallelQuery) genericWrapper.Invoke(null, new object[] {source});
            return parallelQuery;
        }

#if WinRT
        private static string CallerName([CallerMemberName] string caller = "")
        {
            return caller;
        }
#endif
#endif

        public static Type GetElementType(this IEnumerable source)
        {
            var list = source;
            //var prop = list.GetType().GetProperty("Item");
            var prop = list.GetItemPropertyInfo();
            return prop != null ? prop.PropertyType : GetItemType(source, false);
        }

        public static PropertyInfo GetItemPropertyInfo(this IEnumerable list)
        {
            var prop = list.GetType().GetProperties().Where(p => p.Name.Equals("Item"));
            if (prop.Count() > 1)
            {
                return prop.FirstOrDefault(p =>
                {
#if !WinRT
                    ParameterInfo[] para = p.GetGetMethod().GetParameters();
#else
                    ParameterInfo[] para = p.GetMethod.GetParameters();
#endif
                    if (para.Count() > 0)
                    {
                        return para[0].ParameterType == typeof(int);
                    }
                    return false;
                });
            }
            else
                return list.GetType().GetProperty("Item");
        }

        internal static Type GetElementType(this IEnumerable source, ref bool isEmpty)
        {
            var firstItem = GetRepresentativeItem(source);
            if (firstItem != null)
            {
                isEmpty = false;
                var castType = CastToSourceType(source);
                isEmpty = false;
                return castType ?? firstItem.GetType();
            }
            else
            {
                isEmpty = true;
                var list = source;
                // var prop = list.GetType().GetProperty("Item");
                var prop = list.GetItemPropertyInfo();
                isEmpty = true;
                return prop != null ? prop.PropertyType : GetItemType(source, true);
            }
        }

        internal static Type CastToSourceType(IEnumerable source)
        {
            var type = source.GetType();
            return GetBaseGenericInterfaceType(type, false);
        }

        private static Type GetBaseGenericInterfaceType(Type type, bool canreturn)
        {
            if (type.IsGenericType())
            {
                Type[] genericArguments = type.GetGenericArguments();
                if (genericArguments.Length == 1)
                {
                    if (genericArguments[0].IsInterface() || genericArguments[0].IsAbstract())
                    {
                        return genericArguments[0];
                    }
                    if (canreturn)
                        return genericArguments[0];
                }
            }
            else if (type.BaseType() != null)
            {
                return GetBaseGenericInterfaceType(type.BaseType(), canreturn);
            }
            return null;
        }

        internal static Type GetItemType(this IEnumerable source, bool useRepresentativeItem)
        {
            var type = source.GetType();
            if (type.IsGenericType())
            {
                var generictype = GetBaseGenericInterfaceType(type, true);
                if (generictype != null && (generictype.IsInterface() || generictype.IsAbstract()))
                {
                    if (useRepresentativeItem)
                    {
                        var representativeItem = GetRepresentativeItem(source);
                        if (representativeItem != null)
                            return representativeItem.GetType();
                    }
                }
                return generictype;
            }
            else if (useRepresentativeItem)
            {
                var representativeItem = GetRepresentativeItem(source);
                if (representativeItem != null)
                {
                    return representativeItem.GetType();
                }
                else if (type.BaseType().IsGenericType())
                {
                    return type.BaseType().GetGenericArguments()[0];
                }
            }
            return null;
        }

        private static object GetRepresentativeItem(IEnumerable source)
        {
            var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            return null;
        }

    }

}