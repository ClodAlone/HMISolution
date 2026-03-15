using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace AutoNotifyPropertyChange
{
    class Prerequisites
    {
        public static void ThrowIfNotSatisfiedBy(Type t)
        {
            if (t.GetInterfaces().Any( I=> I==typeof(INotifyPropertyChanged)))
            {
                var mi = t.GetMethod(ProxyGen.PropertyChangedFunctionName, BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
                             

                if (mi == null)
                    throw new ProxyGenerationException(t.Name + " does not implement a function named " + ProxyGen.PropertyChangedFunctionName + "(string) to raise the property changed event.");
                if (mi.IsPrivate)
                    throw new ProxyGenerationException(t.Name + "." + ProxyGen.PropertyChangedFunctionName + " must be public or protected.");
            }
        }
    }
}
