using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace Utilities
{
    public class VisualTreeEnumerator : NestedObjectEnumeratorBase
    {
        static IEnumerator<object> GetDependencyObjectEnumerator(DependencyObject dObject, int startIndex, int endIndex, int step)
        {
            for (int i = startIndex; i != endIndex; i += step)
            {
                yield return VisualTreeHelper.GetChild(dObject, i);
            }
        }
        EnumeratorDirection direction = EnumeratorDirection.Forward;
        public DependencyObject Current
        {
            get { return (DependencyObject)Enumerator.Current; }
        }
        public VisualTreeEnumerator(DependencyObject dObject)
            : this(dObject, EnumeratorDirection.Forward)
        {
        }
        protected VisualTreeEnumerator(DependencyObject dObject, EnumeratorDirection direction)
            : base(dObject)
        {
            this.direction = direction;
        }
        protected override IEnumerator GetNestedObjects(object obj)
        {
            DependencyObject dObject = (DependencyObject)obj;
            int count = (dObject is Visual) ? VisualTreeHelper.GetChildrenCount(dObject) : 0;
            return direction == EnumeratorDirection.Forward ?
                GetDependencyObjectEnumerator(dObject, 0, count, 1) :
                GetDependencyObjectEnumerator(dObject, count - 1, -1, -1);
        }
        public IEnumerable<DependencyObject> GetVisualParents()
        {
            return EnumeratorHelper.Convert<object, DependencyObject>(
                    GetParents(), (obj) => (DependencyObject)obj
                );
        }
    }
    public class LogicalTreeEnumerator : VisualTreeEnumerator
    {
        static IEnumerator GetVisualAndLogicalChilder(object obj, IEnumerator visualChildren, bool dependencyObjectsOnly)
        {
            while (visualChildren.MoveNext())
                yield return visualChildren.Current;
            foreach (object logicalChild in LogicalTreeHelper.GetChildren((DependencyObject)obj))
            {
                if (dependencyObjectsOnly && !(logicalChild is DependencyObject)) continue;
                yield return logicalChild;
            }
        }
        public LogicalTreeEnumerator(DependencyObject dObject)
            : base(dObject)
        {
        }
        protected virtual bool DependencyObjectsOnly { get { return false; } }
        protected override IEnumerator GetNestedObjects(object obj)
        {
            return GetVisualAndLogicalChilder(obj, base.GetNestedObjects(obj), DependencyObjectsOnly);
        }
    }
    public class SingleObjectEnumerator : VisualTreeEnumerator
    {
        public SingleObjectEnumerator(DependencyObject dObject)
            : base(dObject)
        {
        }
        protected override IEnumerator GetNestedObjects(object obj)
        {
            return EmptyEnumerator;
        }
    }
    public class SerializationEnumerator : LogicalTreeEnumerator
    {
        protected override bool DependencyObjectsOnly
        {
            get { return true; }
        }
        public SerializationEnumerator(DependencyObject dObject)
            : base(dObject)
        {
        }
    }
}
