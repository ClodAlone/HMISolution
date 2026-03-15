using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoNotifyPropertyChange
{
    public static class TypeFactory
    {
        public static Type AutoNotifier(Type t)
        {
            if (NothingToDo.With(t))
                return t;
            Prerequisites.ThrowIfNotSatisfiedBy(t);
            var influences = PropertyDependencyAnalyzer.GetPropertyInfluences(t);
            return ProxyGen.GetFor(t,influences);
        }
        public static Type AutoNotifier<T>()
        {
            return AutoNotifier(typeof(T));
        }
    }
}
