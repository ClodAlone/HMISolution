#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !Orubase
namespace Syncfusion.Linq
#else
namespace Orubase.Linq
#endif
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Functional method extensions
    /// </summary>
    public static class FunctionalExtensions
    {        

#if !SILVERLIGHT
        /// <summary>
        /// Creates the new instance constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fullName">The full name.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static Delegate CreateNewInstanceConstructor(this Type type, string fullName, Type targetType)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                fullName = fullName ?? type.FullName;
                throw new InvalidOperationException("No constructor found " + fullName);
            }
            DynamicMethod method = new DynamicMethod("invoke_constructor", targetType, Type.EmptyTypes, false);
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Newobj, constructor);
            if (targetType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box);
            }
            iLGenerator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<>).MakeGenericType(new Type[] { targetType }));
        }
#endif

        public static T CreateNew<T>(this Type type)
        {
            return (T)type.CreateNew();
        }

        public static object CreateNew(this Type type)
        {
            var paramExp = Expression.Parameter(type, type.Name);
            var instance = Expression.MemberInit(Expression.New(type), new List<MemberBinding>());
            var lambda = Expression.Lambda(instance, paramExp);
            var delg = lambda.Compile();
            return delg.DynamicInvoke(new object[] { null });
        }

        /// <summary>
        /// Iterates over an IEnumerable instance to a delegated function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable items, Action<T> action)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (var item in items)
            {
                action((T)item);
            }
        }

        /// <summary>
        /// Iterates over a generic IEnumerable instance to a delegated function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in items)
            {
#if SILVERLIGHT
                if (item != null && (item.ToString().Equals("System.ServiceModel.DomainServices.Client.EntityConflict EntityConflict") ||
                    item.ToString().Equals("System.Collections.Generic.ICollection`1[System.ComponentModel.DataAnnotations.ValidationResult] ValidationErrors") ||
                    item.ToString().Equals("Boolean HasValidationErrors") ||
                    item.ToString().Equals("System.ServiceModel.DomainServices.Client.EntityState EntityState") ||
                    item.ToString().Equals("Boolean HasChanges") ||
                    item.ToString().Equals("Boolean IsReadOnly") ||
                    item.ToString().Equals("System.Collections.Generic.IEnumerable`1[System.ServiceModel.DomainServices.Client.EntityAction] EntityActions")))
                {

                }
                else
                {
                    action(item);
                }
#else
                action(item);
#endif
            }
        }

        public static IEnumerable<T> ToList<T>(this IEnumerable items)
        {
            foreach (var item in items)
            {
                yield return (T)item;
            }
        }

        /// <summary>
        /// Iterates the index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void IterateIndex<T>(this T[] items, Action<int, T> action)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (action == null)
                throw new ArgumentNullException("action");

            int lower = items.GetLowerBound(0);
            int upper = items.GetUpperBound(0);
            for (int idx = lower; idx < upper; idx++)
                action(idx, items[idx]);
        }

        /// <summary>
        /// Iterates the index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void IterateIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            IterateIndex(items, action, 0);
        }

        public static void IterateIndex<T>(this IEnumerable<T> items, Action<int, T> action, int idx)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (action == null)
                throw new ArgumentNullException("action");

            IEnumerator<T> enumerator = items.GetEnumerator();
            //Adjusting values
            for (int count = 0; count < idx; count++)
                enumerator.MoveNext();
            while (enumerator.MoveNext())
            {
                action(idx, enumerator.Current);
                idx++;
            }
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>
    (this IEnumerable<TFirst> first,
    IEnumerable<TSecond> second,
    Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) { throw new ArgumentNullException("first"); }
            if (second == null) { throw new ArgumentNullException("second"); }
            if (resultSelector == null) { throw new ArgumentNullException("resultSelector"); }

            return ZipIterator(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>
            (IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            {
                using (IEnumerator<TSecond> e2 = second.GetEnumerator())
                {
                    while (e1.MoveNext() && e2.MoveNext())
                    {
                        yield return resultSelector(e1.Current, e2.Current);
                    }
                }
            }
        }



        /// <summary>
        /// Trues this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        /// <summary>
        /// Falses this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// Ors the specified expr1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Ands the specified expr1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Folds the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="func">The func.</param>
        /// <param name="acc">The acc.</param>
        /// <returns></returns>
        public static T Fold<T, U>(this IEnumerable<U> list, Func<T, U, T> func, T acc)
        {
            foreach (var item in list)
                acc = func(acc, item);

            return acc;
        }

        /// <summary>
        /// Folds the left.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="func">The func.</param>
        /// <param name="acc">The acc.</param>
        /// <returns></returns>
        public static T FoldLeft<T, U>(this IEnumerable<U> list, Func<T, U, T> func, T acc)
        {
            IEnumerator<U> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                acc = func(acc, enumerator.Current);
            }
            return acc;
        }

        /// <summary>
        /// Folds the right.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="func">The func.</param>
        /// <param name="acc">The acc.</param>
        /// <returns></returns>
        public static T FoldRight<T, U>(this IEnumerable<U> list, Func<T, U, T> func, T acc)
        {
            IEnumerator<U> enumerator = list.GetEnumerator();
            List<U> value = new List<U>();
            while (enumerator.MoveNext())
                value.Add(enumerator.Current);
            for (int index = value.Count - 1; index >= 0; index--)
                acc = func(acc, value[index]);

            return acc;
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="src">The SRC.</param>
        /// <param name="dest">The dest.</param>
        public static void MoveTo(this IList list, int src, int dest)
        {
            object value = list[src];
            list.RemoveAt(src);
            list.Insert(dest, value);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in items)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}
