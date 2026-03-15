#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SyncfusionFramework4_0
#if !Orubase
namespace Syncfusion.Dynamic
#else
namespace Orubase.Dynamic
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Dynamic;
    using Microsoft.CSharp.RuntimeBinder;
    using System.Runtime.CompilerServices;
    using System.Linq.Expressions;
    using System.Collections;
#if !Orubase
    using Syncfusion.Linq;
#else
    using Orubase.Linq;
#endif

    public class DynamicHelper : IDisposable
    {
        public DynamicHelper()
        {
            this.objectGraph = new Dictionary<Type, Dictionary<string, CallSiteMember>>();
        }

        public void Dispose()
        {
            if (this.objectGraph.Count > 0)
            {
                this.objectGraph.Clear();
                this.objectGraph = null;
            }
        }

        public static bool CheckIsDynamicObject(Type t)
        {
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(t))
            {
                return true;
            }

            return false;
        }

        public static bool ValidateProperty(IDynamicMetaObjectProvider dyn, string propName)
        {
            if (dyn == null)
            {
                return false;
            }

            var metatype = dyn.GetType();
            var metaData = dyn.GetMetaObject(Expression.Parameter(metatype, metatype.Name));
            var result = metaData.GetDynamicMemberNames().FirstOrDefault(m => m == propName) != null;
            return result;
        }

        public static bool IsComplexCollection(IDynamicMetaObjectProvider dyn, string propName)
        {
            if (!IsPythonType(propName))
            {
                var getterSite = CreateGetterSite(dyn, propName);
                object result = getterSite.Target.Invoke(getterSite, dyn);
                if (result != null)
                {
                    var isComplexCollection = NullableHelperInternal.IsComplexType(result.GetType());
                    return isComplexCollection;
                }
            }

            return false;
        }

        public static bool IsPythonType(string propName)
        {
            return propName.StartsWith("__") && propName.EndsWith("__");
        }

        public object GetValue(object dyn, string propName)
        {
            if (ValidateProperty((IDynamicMetaObjectProvider)dyn, propName))
            {
                var getterSite = this.GetGetterSite(dyn.GetType(), propName);
                object result = getterSite.Target.Invoke(getterSite, dyn);
                return result;
            }
            return null;
        }

        public void SetValue(object dyn, string propName, object value)
        {
            if (ValidateProperty((IDynamicMetaObjectProvider)dyn, propName))
            {
                var setterSite = this.GetSetterSite(dyn.GetType(), propName);
                setterSite.Target.Invoke(setterSite, dyn, value);
            }
        }

        private Dictionary<Type, Dictionary<string, CallSiteMember>> objectGraph;
        private CallSite<Func<CallSite, object, object>> GetGetterSite(Type dyn, string propName)
        {
            if (!this.objectGraph.ContainsKey(dyn))
            {
                var getterSite = CreateGetterSite(dyn, propName);
                var callSiteTree = new Dictionary<string, CallSiteMember>();
                var member = new CallSiteMember() { GetterSite = getterSite };
                callSiteTree.Add(propName, member);
                this.objectGraph.Add(dyn, callSiteTree);
            }
            else if (this.objectGraph.ContainsKey(dyn))
            {
                var callSiteTree = this.objectGraph[dyn];
                if (!callSiteTree.ContainsKey(propName))
                {
                    var getterSite = CreateGetterSite(dyn, propName);
                    var member = new CallSiteMember() { GetterSite = getterSite };
                    callSiteTree.Add(propName, member);
                }
                else if (callSiteTree.ContainsKey(propName) && callSiteTree[propName].GetterSite == null)
                {
                    var getterSite = CreateGetterSite(dyn, propName);
                    callSiteTree[propName].GetterSite = getterSite;
                }
            }

            var callSiteTreeFromGraph = this.objectGraph[dyn];
            var callSite = callSiteTreeFromGraph[propName].GetterSite;
            return callSite;
        }

        private CallSite<Func<CallSite, object, object, object>> GetSetterSite(Type dyn, string propName)
        {
            if (!this.objectGraph.ContainsKey(dyn))
            {
                var setterSite = CreateSetterSite(dyn, propName);
                var callSiteTree = new Dictionary<string, CallSiteMember>();
                var member = new CallSiteMember() { SetterSite = setterSite };
                callSiteTree.Add(propName, member);
                this.objectGraph.Add(dyn, callSiteTree);
            }
            else if (this.objectGraph.ContainsKey(dyn))
            {
                var callSiteTree = this.objectGraph[dyn];
                if (!callSiteTree.ContainsKey(propName))
                {
                    var setterSite = CreateSetterSite(dyn, propName);
                    var member = new CallSiteMember() { SetterSite = setterSite };
                    callSiteTree.Add(propName, member);
                }
                else if (callSiteTree.ContainsKey(propName) && callSiteTree[propName].SetterSite == null)
                {
                    var setterSite = CreateSetterSite(dyn, propName);
                    callSiteTree[propName].SetterSite = setterSite;
                }
            }

            var callSiteTreeFromGraph = this.objectGraph[dyn];
            var callSite = callSiteTreeFromGraph[propName].SetterSite;
            return callSite;
        }

        private static CallSite<Func<CallSite, object, object>> CreateGetterSite(object dyn, string propName)
        {
            var binder = Binder.GetMember(CSharpBinderFlags.None, propName, dyn.GetType(), new List<CSharpArgumentInfo> { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var getterSite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return getterSite;
        }

        private static CallSite<Func<CallSite, object, object, object>> CreateSetterSite(object dyn, string propName)
        {
            var binder = Binder.SetMember(CSharpBinderFlags.None, propName, dyn.GetType(), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null) });
            var setterSite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
            return setterSite;
        }

        private class CallSiteMember
        {
            public CallSite<Func<CallSite, object, object>> GetterSite
            {
                get;
                set;
            }

            public CallSite<Func<CallSite, object, object, object>> SetterSite
            {
                get;
                set;
            }
        }
    }
}
#endif
