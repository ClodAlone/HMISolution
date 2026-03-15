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
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Syncfusion.Windows.Chart
{
    class FastReflection
    {
    }
    /// <summary>
    /// Class implementation for FastReflectionCaches
    /// </summary>
    public static class FastReflectionCaches
    {
        static FastReflectionCaches()
        {
            MethodInvokerCache = new MethodInvokerCache();
            PropertyAccessorCache = new PropertyAccessorCache();
            //FieldAccessorCache = new FieldAccessorCache();
            //ConstructorInvokerCache = new ConstructorInvokerCache();
        }

        /// <summary>
        /// Get or Set MethodInvokerCache
        /// </summary>
        public static IFastReflectionCache<MethodInfo, IMethodInvoker> MethodInvokerCache { get; set; }

        /// <summary>
        /// Get or Set PropertyAccessorCache property
        /// </summary>
        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache { get; set; }

        //public static IFastReflectionCache<FieldInfo, IFieldAccessor> FieldAccessorCache { get; set; }

        //public static IFastReflectionCache<ConstructorInfo, IConstructorInvoker> ConstructorInvokerCache { get; set; }
    }
    /// <summary>
    /// Class implementation for MethodInvokerCache
    /// </summary>
    public class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
    {
        /// <summary>
        /// Return the IMethodInvoker Value from the given MethodInfo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override IMethodInvoker Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodInvokerFactory.Create(key);
        }
    }
    /// <summary>
    /// Class implementation for FastReflectionFactories
    /// </summary>
    public static class FastReflectionFactories
    {
        static FastReflectionFactories()
        {
            MethodInvokerFactory = new MethodInvokerFactory();
            //PropertyAccessorFactory = new PropertyAccessorFactory();
            //FieldAccessorFactory = new FieldAccessorFactory();
            //ConstructorInvokerFactory = new ConstructorInvokerFactory();
        }
        /// <summary>
        /// Get or Set MethodInvokerFactory property
        /// </summary>
        public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }

        //public static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> PropertyAccessorFactory { get; set; }

        //public static IFastReflectionFactory<FieldInfo, IFieldAccessor> FieldAccessorFactory { get; set; }

        //public static IFastReflectionFactory<ConstructorInfo, IConstructorInvoker> ConstructorInvokerFactory { get; set; }
    }
    /// <summary>
    /// Class implementation for MethodInvokerFactory
    /// </summary>
    public class MethodInvokerFactory : IFastReflectionFactory<MethodInfo, IMethodInvoker>
    {
        /// <summary>
        /// Return IMethodInvoker value from the given MethodInfo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IMethodInvoker Create(MethodInfo key)
        {
            return new MethodInvoker(key);
        }

       

        IMethodInvoker IFastReflectionFactory<MethodInfo, IMethodInvoker>.Create(MethodInfo key)
        {
            return this.Create(key);
        }

       
    }
    /// <summary>
    /// Interface implementation for IFastReflectionFactory
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IFastReflectionFactory<TKey, TValue>
    {
        /// <summary>
        /// Create method declaration for IFastReflectionFactory interface
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Create(TKey key);
    }
    /// <summary>
    /// Interface implementation for IFastReflectionCache
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IFastReflectionCache<TKey, TValue>
    {
        /// <summary>
        /// Get method declaration for IFastReflectionCache method 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Get(TKey key);
    }
    /// <summary>
    /// Interface IPropertyAccessor implementation 
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// GetValue method declaration in IPropertyAccessor interface
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetValue(object instance);

        /// <summary>
        /// SetValue method declaration in IPropertyAccessor interface
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        void SetValue(object instance, object value);
    }
    /// <summary>
    /// Represents PropertyAccessorCache class 
    /// </summary>
    public class PropertyAccessorCache : FastReflectionCache<PropertyInfo, IPropertyAccessor>
    {
        /// <summary>
        /// Return the IPropertyAccessor Value from the given PropertyInfo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override IPropertyAccessor Create(PropertyInfo key)
        {
            return new PropertyAccessor(key);
        }
    }
    /// <summary>
    ///  Represents PropertyAccessor class 
    /// </summary>
    public class PropertyAccessor : IPropertyAccessor
    {
        private Func<object, object> m_getter;
        private MethodInvoker m_setMethodInvoker;

        /// <summary>
        /// Get and Set PropertyInfoProperty
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Called when instance created for PropertyAccessor
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.InitializeGet(propertyInfo);
            this.InitializeSet(propertyInfo);
        }

        private void InitializeGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
                System.Linq.Expressions.Expression.Convert(instance, propertyInfo.ReflectedType);

            // ((TInstance)instance).Property
            var propertyAccess = System.Linq.Expressions.Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = System.Linq.Expressions.Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            this.m_getter = lambda.Compile();
        }

        private void InitializeSet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite) return;
            this.m_setMethodInvoker = new MethodInvoker(propertyInfo.GetSetMethod(true));// .GetSetMethod(true));
        }

        /// <summary>
        /// return object value from the given object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public object GetValue(object o)
        {
            if (this.m_getter == null)
            {
                throw new NotSupportedException("Get method is not defined for this property.");
            }

            return this.m_getter(o);
        }

        /// <summary>
        /// Non-return method implementation for Setvalue
        /// </summary>
        /// <param name="o"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException"></exception>
        public void SetValue(object o, object value)
        {
            if (this.m_setMethodInvoker == null)
            {
                throw new NotSupportedException("Set method is not defined for this property.");
            }

            this.m_setMethodInvoker.Invoke(o, new object[] { value });
        }

        #region IPropertyAccessor Members

        object IPropertyAccessor.GetValue(object instance)
        {
            return this.GetValue(instance);
        }

        void IPropertyAccessor.SetValue(object instance, object value)
        {
            this.SetValue(instance, value);
        }

        #endregion
    }
    /// <summary>
    /// Interface implementation for IMethodInvoker
    /// </summary>
    public interface IMethodInvoker
    {
        /// <summary>
        /// Invoke Method declaration for IMethodInvoker Interface
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object Invoke(object instance, params object[] parameters);
    }
    /// <summary>
    /// Class implementation for MethodInvoker
    /// </summary>
    public class MethodInvoker : IMethodInvoker
    {
        private Func<object, object[], object> m_invoker;

        /// <summary>
        /// Get or Set MethodInfo property
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// Called when instance created for MethodInvoker
        /// </summary>
        /// <param name="methodInfo"></param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            this.MethodInfo = methodInfo;
            this.m_invoker = CreateInvokeDelegate(methodInfo);
        }

        /// <summary>
        /// Return object from the given objects
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Invoke(object instance, params object[] parameters)
        {
            return this.m_invoker(instance, parameters);
        }

        private static Func<object, object[], object> CreateInvokeDelegate(MethodInfo methodInfo)
        {
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");
            var parametersParameter = System.Linq.Expressions.Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<System.Linq.Expressions.Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = System.Linq.Expressions.Expression.ArrayIndex(
                    parametersParameter, System.Linq.Expressions.Expression.Constant(i));
                UnaryExpression valueCast = System.Linq.Expressions.Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                System.Linq.Expressions.Expression.Convert(instanceParameter, methodInfo.ReflectedType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = System.Linq.Expressions.Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = System.Linq.Expressions.Expression.Lambda<Action<object, object[]>>(
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
                var castMethodCall = System.Linq.Expressions.Expression.Convert(methodCall, typeof(object));
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object[], object>>(
                    castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }

        #region IMethodInvoker Members

        object IMethodInvoker.Invoke(object instance, params object[] parameters)
        {
            return this.Invoke(instance, parameters);
        }

        #endregion
    }
    /// <summary>
    /// Class implementation for FastReflectionCache 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_cache = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Get method declaration for IFastReflectionCache method 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            TValue value = default(TValue);
            if (this.m_cache.TryGetValue(key, out value))
            {
                return value;
            }

            lock (key)
            {
                if (!this.m_cache.TryGetValue(key, out value))
                {
                    value = this.Create(key);
                    this.m_cache[key] = value;
                }
            }

            return value;
        }
        /// <summary>
        /// FastReflectionCache(TKey, TValue).Create Method 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract TValue Create(TKey key);
    }
}
