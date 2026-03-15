#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Syncfusion.UI.Xaml.Schedule
{
    #region Interfaces

    public interface IFastReflectionFactory<in TKey, out TValue>
    {
        TValue Create(TKey key);
    }

    public interface IFastReflectionCache<in TKey, out TValue>
    {
        TValue Get(TKey key);
    }

    public interface IPropertyAccessor
    {
        object GetValue(object instance);

        void SetValue(object instance, object value);
    }

    public interface IMethodInvoker
    {
        object Invoke(object instance, params object[] parameters);
    }

    #endregion

    #region FastReflectionCaches

    public static class FastReflectionCaches
    {
        #region Constructor

        static FastReflectionCaches()
        {
            MethodInvokerCache = new MethodInvokerCache();
            PropertyAccessorCache = new PropertyAccessorCache();
        }

        #endregion

        #region Static Fields

        public static IFastReflectionCache<MethodInfo, IMethodInvoker> MethodInvokerCache { get; set; }

        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache { get; set; }

        #endregion
    }

    #endregion

    #region MethodInvokerCache

    public class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
    {
        protected override IMethodInvoker Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodInvokerFactory.Create(key);
        }
    }

    #endregion

    #region FastReflectionFactories

    public static class FastReflectionFactories
    {
        static FastReflectionFactories()
        {
            MethodInvokerFactory = new MethodInvokerFactory();
        }

        public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }
    }

    #endregion

    #region MethodInvokerFactory

    public class MethodInvokerFactory : IFastReflectionFactory<MethodInfo, IMethodInvoker>
    {
        public IMethodInvoker Create(MethodInfo key)
        {
            return new MethodInvoker(key);
        }

        IMethodInvoker IFastReflectionFactory<MethodInfo, IMethodInvoker>.Create(MethodInfo key)
        {
            return Create(key);
        }
    }

    #endregion

    #region PropertyAccessorCache

    public class PropertyAccessorCache : FastReflectionCache<PropertyInfo, IPropertyAccessor>
    {
        protected override IPropertyAccessor Create(PropertyInfo key)
        {
            return new PropertyAccessor(key);
        }
    }

    #endregion

    #region PropertyAccessor

    public class PropertyAccessor : IPropertyAccessor
    {
        private Func<object, object> m_getter;
        private MethodInvoker m_setMethodInvoker;

        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            InitializeGet(propertyInfo);
            InitializeSet(propertyInfo);
        }

        private void InitializeGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
#if WINRT
            var instanceCast = propertyInfo.GetMethod.IsStatic ? null :
#else
            var instanceCast = propertyInfo.GetSetMethod().IsStatic ? null :
#endif
 Expression.Convert(instance, propertyInfo.DeclaringType);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            m_getter = lambda.Compile();
        }

        private void InitializeSet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite) return;
#if WINRT
            m_setMethodInvoker = new MethodInvoker(propertyInfo.SetMethod);// .GetSetMethod(true));
#else
            m_setMethodInvoker = new MethodInvoker(propertyInfo.GetSetMethod());// .GetSetMethod(true));
#endif
        }

        public object GetValue(object o)
        {
            if (m_getter == null)
            {
                throw new NotSupportedException("Get method is not defined for this property.");
            }

            return m_getter(o);
        }

        public void SetValue(object o, object value)
        {
            if (m_setMethodInvoker == null)
            {
                throw new NotSupportedException("Set method is not defined for this property.");
            }

            m_setMethodInvoker.Invoke(o, new[] { value });
        }

        #region IPropertyAccessor Members

        object IPropertyAccessor.GetValue(object instance)
        {
            return GetValue(instance);
        }

        void IPropertyAccessor.SetValue(object instance, object value)
        {
            SetValue(instance, value);
        }

        #endregion
    }

    #endregion

    #region MethodInvoker

    public class MethodInvoker : IMethodInvoker
    {
        private readonly Func<object, object[], object> m_invoker;

        public MethodInfo MethodInfo { get; private set; }

        public MethodInvoker(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            m_invoker = CreateInvokeDelegate(methodInfo);
        }

        public object Invoke(object instance, params object[] parameters)
        {
            return m_invoker(instance, parameters);
        }

        private static Func<object, object[], object> CreateInvokeDelegate(MethodInfo methodInfo)
        {
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodInfo.DeclaringType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<Func<object, object[], object>>(
                    castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }

        #region IMethodInvoker Members

        object IMethodInvoker.Invoke(object instance, params object[] parameters)
        {
            return Invoke(instance, parameters);
        }

        #endregion
    }

    #endregion

    #region FastReflectionCache

    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_cache = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key)
        {
            TValue value;
            if (m_cache.TryGetValue(key, out value))
            {
                return value;
            }

            lock (key)
            {
                if (!m_cache.TryGetValue(key, out value))
                {
                    value = Create(key);
                    m_cache[key] = value;
                }
            }

            return value;
        }

        protected abstract TValue Create(TKey key);
    }

    #endregion
}
