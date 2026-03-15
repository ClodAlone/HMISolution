using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities
{
    public abstract class NestedObjectEnumeratorBase : IEnumerator
    {
        #region inner class
        protected class EnumStack : Stack<IEnumerator>
        {
            public IEnumerator TopEnumerator
            {
                get { return (IsEmpty == false) ? Peek() : null; }
            }
            public bool IsEmpty
            {
                get { return Count == 0; }
            }
            public EnumStack()
            {
            }
        }
        #endregion
        protected static readonly IEnumerator EmptyEnumerator = (new object[0]).GetEnumerator();
        IEnumerator objects;
        protected EnumStack stack;
        protected IEnumerator Enumerator { get { return (IEnumerator)this; } }
        public object CurrentParent
        {
            get { return EnumeratorHelper.GetFirst<object>(GetParents()); }
        }
        public IEnumerable<object> GetParents()
        {
            IEnumerator<IEnumerator> en = stack.GetEnumerator();
            if (en.MoveNext())
            {
                while (en.MoveNext())
                {
                    yield return en.Current.Current;
                }
            }
        }
        public int Level { get { return stack.Count; } }
        protected NestedObjectEnumeratorBase(object obj)
        {
            this.objects = new object[] { obj }.GetEnumerator();
            stack = new EnumStack();
            Reset();
        }
        object IEnumerator.Current
        {
            get { return stack.TopEnumerator.Current; }
        }
        public virtual bool MoveNext()
        {
            if (stack.IsEmpty)
            {
                stack.Push(objects);
                return stack.TopEnumerator.MoveNext();
            }
            IEnumerator nestedObjects = GetNestedObjects(Enumerator.Current);
            if (nestedObjects.MoveNext())
            {
                stack.Push(nestedObjects);
                return true;
            }
            while (stack.TopEnumerator.MoveNext() == false)
            {
                stack.Pop();
                if (stack.IsEmpty)
                    return false;
            }
            return true;
        }
        protected abstract IEnumerator GetNestedObjects(object obj);
        public virtual void Reset()
        {
            stack.Clear();
            objects.Reset();
        }
    }
    public delegate void EnumerateDelegate<OfType>(OfType item);
    public static class EnumeratorHelper
    {
        public static int Count<T>(IEnumerable<T> list)
        {
            int i = 0;
            foreach (T item in list)
            {
                i++;
            }
            return i;
        }
        public static int Count(IEnumerable list)
        {
            int i = 0;
            foreach (object item in list)
            {
                i++;
            }
            return i;
        }
        public static IEnumerable<T> Combine<T>(params IEnumerable<T>[] enumerables)
        {
            foreach (IEnumerable<T> enumerable in enumerables)
            {
                foreach (T item in enumerable)
                {
                    yield return item;
                }
            }
        }
        public static T GetFirst<T>(IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                return item;
            }
            return default(T);
        }
        public static bool Exists<T>(IEnumerable<T> enumerable, T toFind)
        {
            return null != Find<T>(enumerable,
                delegate(T item)
                {
                    return Object.ReferenceEquals(item, toFind);
                });
        }
        public static bool Exists(IEnumerable enumerable, object toFind)
        {
            return null != Find(enumerable,
                delegate(object item)
                {
                    return Object.ReferenceEquals(item, toFind);
                });
        }
        public static IEnumerable<T> Filter<T>(IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            foreach (T item in enumerable)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
        public static T Find<T>(IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            foreach (T item in enumerable)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default(T);
        }
        public static object Find(IEnumerable enumerable, Predicate<object> predicate)
        {
            foreach (object item in enumerable)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return null;
        }
        public static IEnumerable<TOutput> Convert<TOutput, TInput>(IEnumerable<TInput> source) where TInput : TOutput
        {
            foreach (TInput item in source)
            {
                yield return (TOutput)item;
            }
        }
        public static IEnumerable<TOutput> Convert<TInput, TOutput>(IEnumerable<TInput> source, Converter<TInput, TOutput> castDelegate)
        {
            foreach (TInput item in source)
            {
                yield return castDelegate(item);
            }
        }
        public static T[] ToArray<T>(IEnumerable<T> source)
        {
            return new List<T>(source).ToArray();
        }
        public static void SafeEnumerate<OfType>(IEnumerable<OfType> source, EnumerateDelegate<OfType> handler)
        {
            int index = 0;
            List<OfType> list = new List<OfType>(source);
            while (index < list.Count)
            {
                handler(list[index]);
                list = new List<OfType>(source);
                index++;
            }
        }
        public static void ForEach(IEnumerator enumerator, Func<object, bool> handler)
        {
            while (enumerator.MoveNext())
                if (!handler(enumerator.Current))
                    return;
        }
        public static void ForEach<T>(IEnumerator<T> enumerator, Func<object, bool> handler)
        {
            while (enumerator.MoveNext())
                if (!handler(enumerator.Current))
                    return;
        }
    }
    public class EmptyEnumerator : IEnumerator
    {
        EmptyEnumerator() { }
        public bool MoveNext() { return false; }
        public void Reset() { }
        public object Current { get { throw new InvalidOperationException(); } }
        static IEnumerator instanceCore;
        public static IEnumerator Instance
        {
            get
            {
                if (instanceCore == null) instanceCore = new EmptyEnumerator();
                return instanceCore;
            }
        }
    }
}
