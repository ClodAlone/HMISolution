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
    using System.Text;
    using System.Threading;
    using System.ComponentModel;    

    /// <summary>
    /// Specifies the FilterType to be used in LINQ methods.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Performs LessThan operation.
        /// </summary>
        LessThan,
        /// <summary>
        /// Performs LessThan Or Equal operation.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Checks Equals on the operands.
        /// </summary>
        Equals,
        /// <summary>
        /// Checks for Not Equals on the operands.
        /// </summary>
        NotEquals,
        /// <summary>
        /// Checks for Greater Than or Equal on the operands.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// Checks for Greater Than on the operands.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Checks for StartsWith on the string operands.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Checks for EndsWith on the string operands.
        /// </summary>
        EndsWith,
        /// <summary>
        /// Checks for Contains on the string operands.
        /// </summary>
        Contains,
        /// <summary>
        /// Returns invalid type
        /// </summary>
        Undefined,
        /// <summary>
        /// Checks for Between two date on the operands.
        /// </summary>
        Between
    }

    /// <summary>
    /// Specifies the Filter Behaviour for the filter predicates.
    /// </summary>
    public enum FilterBehavior
    {
        /// <summary>
        /// Parses only StronglyTyped values.
        /// </summary>
        StronglyTyped,

        /// <summary>
        /// Parses all values by converting them as string.
        /// </summary>
        StringTyped
    }

    public class SortColumn
    {

        public SortColumn()
        {
            this.SortDirection = ListSortDirection.Ascending;
        }

        public string ColumnName
        {
            get;
            set;
        }

        public ListSortDirection SortDirection
        {
            get;
            set;
        }
    }

    #region Internal classes

    public class ClassFactory
    {  // Trigger
        private int classCount;
        private Dictionary<Signature, Type> classes;
        public static readonly ClassFactory Instance = new ClassFactory();

        private ModuleBuilder module;
#if !SILVERLIGHT
        private ReaderWriterLock rwLock;
#endif

        private ClassFactory()
        {
            AssemblyName name = new AssemblyName("DynamicClasses");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#if ENABLE_LINQ_PARTIAL_TRUST
            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            try
            {
                module = assembly.DefineDynamicModule("Module");
            }
            finally
            {
#if ENABLE_LINQ_PARTIAL_TRUST
                PermissionSet.RevertAssert();
#endif
            }
            classes = new Dictionary<Signature, Type>();
#if !SILVERLIGHT
            rwLock = new ReaderWriterLock(); 
#endif
        }

        static ClassFactory() { }  // Trigger lazy initialization of static fields

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
        {
#if !SILVERLIGHT
            rwLock.AcquireReaderLock(Timeout.Infinite); 
#endif
            try
            {
                Signature signature = new Signature(properties);
                Type type;
                if (!classes.TryGetValue(signature, out type))
                {
                    type = CreateDynamicClass(signature.properties);
                    classes.Add(signature, type);
                }
                return type;
            }
            finally
            {
#if !SILVERLIGHT
                rwLock.ReleaseReaderLock(); 
#endif
            }
        }

        Type CreateDynamicClass(DynamicProperty[] properties)
        {
#if !SILVERLIGHT
            LockCookie cookie = rwLock.UpgradeToWriterLock(Timeout.Infinite); 
#endif
            try
            {
                string typeName = "DynamicClass" + (classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    TypeBuilder tb = this.module.DefineType(typeName, TypeAttributes.Class |
                        TypeAttributes.Public, typeof(DynamicClass));
                    FieldInfo[] fields = GenerateProperties(tb, properties);
                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);
                    Type result = tb.CreateType();
                    classCount++;
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
#if !SILVERLIGHT
                rwLock.DowngradeFromWriterLock(ref cookie); 
#endif
            }
        }

        void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new Type[] { typeof(object) });
            ILGenerator gen = mb.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(tb);
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        void GenerateGetHashCode(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
        }

        FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
        {
            FieldInfo[] fields = new FieldBuilder[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                DynamicProperty dp = properties[i];
                FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                PropertyBuilder pb = tb.DefineProperty(dp.Name, System.Reflection.PropertyAttributes.HasDefault, dp.Type, null);
                MethodBuilder mbGet = tb.DefineMethod("get_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    dp.Type, Type.EmptyTypes);
                ILGenerator genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                MethodBuilder mbSet = tb.DefineMethod("set_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null, new Type[] { dp.Type });
                ILGenerator genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);
                pb.SetSetMethod(mbSet);
                fields[i] = fb;
            }
            return fields;
        } //lazy initialization of static fields
    }

    public abstract class DynamicClass
    {

        public override string ToString()
        {
            PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < props.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(props[i].Name);
                sb.Append("=");
                sb.Append(props[i].GetValue(this, null));
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class DynamicProperty
    {
        string name;
        Type type;

        public DynamicProperty(string name, Type type)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            this.name = name;
            this.type = type;
        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
        }
    }

    internal class Signature : IEquatable<Signature>
    {
        public int hashCode;
        public DynamicProperty[] properties;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            this.properties = properties.ToArray();
            hashCode = 0;
            foreach (DynamicProperty p in properties)
            {
                hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public bool Equals(Signature other)
        {
            if (properties.Length != other.properties.Length) return false;
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name != other.properties[i].Name ||
                    properties[i].Type != other.properties[i].Type) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Signature ? Equals((Signature)obj) : false;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }

#if !SILVERLIGHT
    public class GroupContext
    {

        public GroupContext()
        {
        }

        public List<GroupContext> ChildGroups
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public IEnumerable Details
        {
            get;
            set;
        }

        public object Key
        {
            get
            {
                if (this.Details != null)
                {
                    var type = this.Details.GetType();
                    var value = type.GetProperties().Where(pi => pi.Name == "Key").FirstOrDefault();
                    var result = value.GetValue(this.Details, null);
                    return result;
                }
                return null;
            }
        }
    }
#endif
    #endregion
}
