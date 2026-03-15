using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
#if WINDOWS_UWP
using DevExpress.Core;
#endif

namespace ViewModelLib
{
    /// <summary>
    /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
    /// and executes callback methods (i.e. handlers) registered for properties of that object.
    /// </summary>
    /// <typeparam name="TPropertySource">The type of object to monitor for property changes.</typeparam>
    public class PropertyObserver<TPropertySource> : /*IWeakEventListener, */IDisposable
        where TPropertySource : INotifyPropertyChanged
    {
#region Constructor

        /// <summary>
        /// Initializes a new instance of PropertyObserver, which
        /// observes the 'propertySource' object for property changes.
        /// </summary>
        /// <param name="propertySource">The object to monitor for property changes.</param>
        public PropertyObserver(TPropertySource propertySource)
        {
            if (propertySource == null)
                throw new ArgumentNullException("propertySource");

            _propertySource = propertySource;
            //_propertySourceRef = new WeakReference(propertySource);
            _propertyNameToHandlerMap = new Dictionary<string, Action<TPropertySource>>();
            _propertySource.PropertyChanged += PropertySource_PropertyChanged;
        }

        private void PropertySource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_propertyNameToHandlerMap.Count == 0)
                return;

            Action<TPropertySource> handler = null;
            lock (_propertyNameToHandlerMap)
            {
                if (!_propertyNameToHandlerMap.ContainsKey(e.PropertyName))
                    return;
                handler = _propertyNameToHandlerMap[e.PropertyName];
            }

            handler((TPropertySource)sender);
        }

#endregion // Constructor

#region Public Methods

#region RegisterHandler

            /// <summary>
            /// Registers a callback to be invoked when the PropertyChanged event has been raised for the specified property.
            /// </summary>
            /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
            /// <param name="handler">The callback to invoke when the property has changed.</param>
            /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
            public PropertyObserver<TPropertySource> RegisterHandler(
            Expression<Func<TPropertySource, object>> expression,
            Action<TPropertySource> handler)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            if (handler == null)
                throw new ArgumentNullException("handler");

            lock (_propertyNameToHandlerMap)
            {
                //TPropertySource propertySource = this.GetPropertySource();
                //if (propertySource != null)
                {
                    Debug.Assert(!_propertyNameToHandlerMap.ContainsKey(propertyName), "Why is the '" + propertyName + "' property being registered again?");

                    if (!_propertyNameToHandlerMap.ContainsKey(propertyName))
                        _propertyNameToHandlerMap[propertyName] = handler;
                    /*
#if !WINDOWS_UWP
                    PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
#else
                    PropertyChangedEventManager.AddListener(propertySource, this, null);
#endif
                    */
                }
            }

            return this;
        }

#endregion // RegisterHandler

#region UnregisterHandler

        /// <summary>
        /// Removes the callback associated with the specified property.
        /// </summary>
        /// <param name="propertyName">A lambda expression like 'n => n.PropertyName'.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<TPropertySource> UnregisterHandler(Expression<Func<TPropertySource, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            lock (_propertyNameToHandlerMap)
            {
                //TPropertySource propertySource = this.GetPropertySource();
                //if (propertySource != null)
                {
                    if (_propertyNameToHandlerMap.ContainsKey(propertyName))
                    {
                        _propertyNameToHandlerMap.Remove(propertyName);
                        /*
                        // _propertyNameToHandlerMap[propertyName] = (a) => { };//replace with a dummy instead of removing.
#if !WINDOWS_UWP
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
#else
                        PropertyChangedEventManager.RemoveListener(propertySource, this, null);
#endif
                        */
                    }
                }
            }

            return this;
        }

#endregion // UnregisterHandler

#region ForceCallBack

        public void ForceCallBack(Expression<Func<TPropertySource, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            Action<TPropertySource> handler = null;
            lock (_propertyNameToHandlerMap)
            {
                if (!_propertyNameToHandlerMap.ContainsKey(propertyName))
                    return;
                handler = _propertyNameToHandlerMap[propertyName];
            }

            handler((TPropertySource)_propertySource);
        }

#endregion // ForceCallBack

#endregion // Public Methods

#region IWeakEventListener Members

        /*
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            bool handled = true;

            Action<TPropertySource> handler = null;
            Action<TPropertySource>[] handlerArray = null;
            TPropertySource propertySource = (TPropertySource)sender;
            lock (_propertyNameToHandlerMap)
            {
                if (managerType == typeof(PropertyChangedEventManager))
                {
                    PropertyChangedEventArgs args = e as PropertyChangedEventArgs;
                    if (args != null && sender is TPropertySource)
                    {
                        string propertyName = args.PropertyName;

                        if (String.IsNullOrEmpty(propertyName))
                        {
                            // When the property name is empty, all properties are considered to be invalidated.
                            // Iterate over a copy of the list of handlers, in case a handler is registered by a callback.
                            handlerArray = _propertyNameToHandlerMap.Values.ToArray();

                            handled = true;
                        }
                        else
                        {
                            if (_propertyNameToHandlerMap.TryGetValue(propertyName, out handler))
                            {
                                handled = true;
                            }
                        }
                    }
                }
            }

            if (handlerArray != null)
            {
                foreach (Action<TPropertySource> h in handlerArray)
                    h(propertySource);
            }
            else if (handler != null)
                handler(propertySource);

            return handled;
        }
        */
#endregion // IWeakEventListener Members

#region Private Helpers

#region GetPropertyName

        static string GetPropertyName(Expression<Func<TPropertySource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo.Name;
            }

            return null;
        }

#endregion // GetPropertyName

#region GetPropertySource

        TPropertySource GetPropertySource()
        {
            try
            {
                return (TPropertySource)_propertySourceRef.Target;
            }
            catch
            {
                return default(TPropertySource);
            }
        }

#endregion // GetPropertySource

#endregion // Private Helpers

#region Fields

        readonly Dictionary<string, Action<TPropertySource>> _propertyNameToHandlerMap;
        readonly WeakReference _propertySourceRef;
        readonly TPropertySource _propertySource;

#endregion // Fields

        #region IDisposable Members

        public void Dispose()
        {
            _propertySource.PropertyChanged -= PropertySource_PropertyChanged;

            // if (_propertyNameToHandlerMap != null)
            {
                lock (_propertyNameToHandlerMap)
                {
                    _propertyNameToHandlerMap.Clear();
                    /*
                    TPropertySource propertySource = GetPropertySource();
                    var propertyNames = _propertyNameToHandlerMap.Keys.ToArray();
                    foreach (var propertyName in propertyNames)
                    {
                        _propertyNameToHandlerMap.Remove(propertyName);
#if !WINDOWS_UWP
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
#else
                        PropertyChangedEventManager.RemoveListener(propertySource, this, null);
#endif
                    }
                    */
                }
            }
        }

#endregion
    }
}