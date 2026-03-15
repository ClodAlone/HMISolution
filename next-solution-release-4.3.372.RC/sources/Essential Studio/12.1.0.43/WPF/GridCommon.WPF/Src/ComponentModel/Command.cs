#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Reflection;

namespace Syncfusion.Windows.ComponentModel
{
    /// <summary>
    /// An abstract class that encapsulates a command to be executed at a later point in time.
    /// </summary>
    public abstract class SyncfusionCommand
    {
        /// <summary>
        /// Execute the command this object represents.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Get a description for the command.
        /// </summary>
        public abstract string Description { get; }
    }



    /// <summary>
    /// Still being discussed if they should be included in shared.
    /// </summary>
    internal class MethodReflectionCommand : SyncfusionCommand
    {
        private object target;
        private object[] parameters;
        private MethodInfo methodInfo;
        private string description;

        public static Type[] GetTypes(object[] parameters)
        {
            Type[] types = new Type[parameters.Length];
            for (int n = 0; n < parameters.Length; n++)
                types[n] = parameters[n] != null ? parameters[n].GetType() : typeof(object);
            return types;
        }

        public MethodReflectionCommand(string description, object target, string methodName, params object[] parameters)
            : this(description, target, methodName, GetTypes(parameters), parameters)
        {
        }

        public MethodReflectionCommand(string description, object target, string methodName, Type[] types, params object[] parameters)
            : this(description, target,
            target.GetType().GetMethod(methodName,
            BindingFlags.Public | BindingFlags.Instance,
            null, types, null), parameters)
        {
        }

        public MethodReflectionCommand(string description, object target, MethodInfo methodInfo, params object[] parameters)
        {
            this.description = description;
            this.methodInfo = methodInfo;
            this.target = target;
            this.parameters = parameters;
        }

        /// <override/>
        public override string Description
        {
            get
            {
                return this.description;
            }
        }

        public object Target
        {
            get
            {
                return this.target;
            }
        }

        protected void SetDescription(string value)
        {
            this.description = value;
        }

        /// <override/>
        public override void Execute()
        {
            methodInfo.Invoke(target, parameters);
        }
    }

    /// <summary>
    /// Still being discussed if they should be included in shared.
    /// </summary>
    internal class PropertySetReflectionCommand : MethodReflectionCommand
    {
        public static MethodInfo GetSetMethod(object target, string propertyName)
        {
            PropertyInfo pi = target.GetType().GetProperty(propertyName);
            return pi.GetSetMethod();
        }

        public PropertySetReflectionCommand(string description, object target, string propertyName, object value)
            : base(description, target, GetSetMethod(target, propertyName), value)
        {
        }

        public PropertySetReflectionCommand(string description, object target, PropertyInfo pi, object value)
            : base(description, target, pi.GetSetMethod(), value)
        {
        }
    }
}

