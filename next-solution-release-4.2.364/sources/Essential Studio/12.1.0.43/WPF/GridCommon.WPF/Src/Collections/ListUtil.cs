#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

namespace Syncfusion.Windows.Collections
{
    using System;
    using System.Collections;
    using System.Data;
    using System.ComponentModel;
    using System.Text;
    using System.Reflection;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Diagnostics;
    using System.Windows.Interop;

    /// <summary>
	/// Provides helper routines for exploring properties in a collection. Essential Grid uses
	/// this routine to find out about columns and relations to be displayed in the grid when
	/// a collection is specified as datasource.
	/// </summary>
	public class ListUtil
	{
		/// <summary>
		/// Checks IBindingList.SortDirection.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static ListSortDirection GetSortDirection(IList list)
		{
			if (list is IBindingList && ((IBindingList)list).SupportsSorting)
				return ((IBindingList)list).SortDirection;
			return ListSortDirection.Ascending;
		}

		/// <summary>
		/// Checks IBindingList.SortProperty.
		/// </summary>
		/// <param name="list">The list to check.</param>
		/// <returns></returns>
		public static PropertyDescriptor GetSortProperty(IList list)
		{
			if (list is IBindingList && ((IBindingList)list).SupportsSorting)
				return ((IBindingList)list).SortProperty;
			return null;
		}

		/// <summary>
		/// Calls IBindingList.ApplySort.
		/// </summary>
		/// <param name="list">List to be sorted.</param>
		/// <param name="property"></param>
		/// <param name="sortDirection"></param>
		public static void SetSort(IList list, PropertyDescriptor property, ListSortDirection sortDirection)
		{
			if (list is IBindingList && ((IBindingList)list).SupportsSorting)
				((IBindingList)list).ApplySort(property, sortDirection);
		}

		/// <summary>
		/// Checks IBindingList.SupportsSorting.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool SupportsSort(IList list)
		{
			return list is IBindingList && ((IBindingList)list).SupportsSorting;
		}


        /// <summary>
        /// Checks IBindingList.AllowRemove for IBindingList or IList.IsReadOnly and IList.IsFixedSize for IList collections.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
		public static bool GetAllowRemove(IList list)
		{
			if (list is System.ComponentModel.IBindingList)
				return ((IBindingList)(list)).AllowRemove;
			if (list == null)
				return false;
			if (!list.IsReadOnly)
				return !(list.IsFixedSize);
			return false;
		}

		/// <summary>
		/// Indicates whether the property represents a relation or nested collection.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static bool PropertyDescriptorIsARelation(PropertyDescriptor prop)
		{
            if (prop.PropertyType != typeof(string)
                && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
				return !(typeof(Array).IsAssignableFrom(prop.PropertyType));
			return false;
		}

		/// <summary>
		/// Compares the two PropertyDescriptorCollection and indicates whether they are equal.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool IsEqualItemProperties(PropertyDescriptorCollection x, PropertyDescriptorCollection y)
		{
			if (x.Count != y.Count)
				return false;

			for (int n = 0; n < x.Count; n++)
				if (!x[n].Equals(y[n]))
					return false;

			return true;
		}

		//static PropertyInfo drpi = null;
		static MethodInfo mInfo = null;
		static MethodInfo mInfo2 = null;
		static MethodInfo mInfo3 = null;

		/// <summary>
		/// Returns the PropertyDescriptorCollection for the relation or nested collection.
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public static System.Data.DataRelation GetDataRelation(PropertyDescriptor pd)
		{
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (pd != null && pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
                {
                    if (mInfo == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Relation",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo = pi.GetGetMethod(true);

                    }

                    if (mInfo != null)
                    {
                        System.Data.DataRelation dr = mInfo.Invoke(pd, new object[0]) as System.Data.DataRelation;
                        return dr;
                    }
                }
            }

			return null;
		}

		/// <summary>
		/// Overloaded. Returns the PropertyDescriptorCollection for the relation or nested collection.
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public static PropertyDescriptorCollection GetRelatedItemProperties(PropertyDescriptor pd)
		{
			string name;
			return GetRelatedItemProperties(pd, out name);
		}

		/// <summary>
		/// Returns the PropertyDescriptorCollection for the relation or nested collection.
		/// </summary>
        public static PropertyDescriptorCollection GetRelatedItemProperties(PropertyDescriptor pd, out string name)
        {
            object dummy;
            return GetRelatedItemProperties(null, pd, out name, out dummy);
        }

		/// <summary>
		/// Returns the PropertyDescriptorCollection for the relation or nested collection.
		/// </summary>
		public static PropertyDescriptorCollection GetRelatedItemProperties(object parentItemList, PropertyDescriptor pd, out string name, out object arrayList)
		{
            arrayList = null;

            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
                {
                    if (mInfo == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Relation",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo = pi.GetGetMethod(true);

                    }

                    if (mInfo != null)
                    {
                        System.Data.DataRelation dr = mInfo.Invoke(pd, new object[0]) as System.Data.DataRelation;
                        if (dr != null)
                        {
                            name = dr.ChildTable.TableName;
                            return GetItemProperties(dr.ChildTable);
                        }
                    }
                }

                if (pd.GetType().FullName == "System.Data.DataTablePropertyDescriptor")
                {
                    if (mInfo2 == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Table",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo2 = pi.GetGetMethod(true);

                    }

                    if (mInfo2 != null)
                    {
                        System.Data.DataTable dt = mInfo2.Invoke(pd, new object[0]) as System.Data.DataTable;
                        name = dt.TableName;
                        return GetItemProperties(dt);
                    }
                }
            }

            if (parentItemList != null && pd.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)) // Support for ArrayList or other types of collection
            {
                IEnumerable list = parentItemList as IEnumerable;
                int n = 0;
                // Find first item in parent list that returns a list with 
                // at least one entry.
                foreach (object item in list)
                {
                    // do not loop through all record, only the first few- this is only a fallback
                    // for those cases where the collection has a nested ArrayList and the first
                    // few items do not have entries in that arraylist. We just search for the 
                    // first ArrayList that has at lest one entry and then we can extract PropertyDescriptors
                    // from that object.
                    if (++n > 20)
                        break;

                    IEnumerable arrayList1 = pd.GetValue(item) as IEnumerable;
                    if (arrayList1 != null && arrayList1.GetEnumerator().MoveNext())
                    {
                        name = pd.Name;
                        arrayList = arrayList1;
                        return GetItemProperties(arrayList1);
                    }
                }
            }
            
            if (!(pd.PropertyType.IsInterface || pd.PropertyType.IsAbstract) && IsComplexType(pd))
			{
				PropertyDescriptorCollection pdc = GetItemProperties(pd.PropertyType);
				if (pdc["IsMarshalByRef"] == null)
				{
					name = pd.PropertyType.Name;
					return pdc;
				}
			}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if (pd.PropertyType.IsGenericType)
            {
                PropertyDescriptorCollection pdc = GetItemProperties(pd.PropertyType);
                if (pdc["IsMarshalByRef"] == null)
                {
                    name = pd.PropertyType.Name;
                    return pdc;
                }
            }
#endif

            name = "";
			return null;
		}

		/// <summary>
		/// Indicates whether the specified PropertyDescriptor has nested properties.
		/// </summary>
		/// <param name="pd">The PropertyDescriptor to be checked.</param>
		/// <returns>True if nested properties are found; False otherwise.</returns>
 	    public static bool IsComplexType(PropertyDescriptor pd)
		{
            if (pd.ComponentType == typeof(Type))
                return false;

            Type t = pd.PropertyType;
            return IsComplexType(t);
        }

        /// <summary>
        /// Indicates whether the specified Type has nested properties.
        /// </summary>
        /// <param name="t">The Type to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        public static bool IsComplexType(Type t)
        {
            Type underlyingType = NullableHelper.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;
            
			if (t != typeof(object)
                && t != typeof(Decimal)
                && t != typeof(DateTime)
				&& t != typeof(Type)
                && t !=typeof(TimeSpan)
				//&& t != typeof(System.Drawing.Color)
				&& t != typeof(string)
				&& t != typeof(Guid)
                && t.BaseType != typeof(Enum)
				&& !t.IsPrimitive)
				return true;

            if (primitiveTypesTable != null && primitiveTypesTable.ContainsKey(t))
                return false;
			return false;
		}

        static Hashtable primitiveTypesTable = null;

        static Hashtable PrimitiveTypesTable
        {
            get
            {
                if (primitiveTypesTable == null)
                    primitiveTypesTable = new Hashtable();
                return ListUtil.primitiveTypesTable;
            }
        }

        /// <summary>
        /// Lets you register additional types that should return false when <see cref="IsComplexType"/> is called. 
        /// </summary>
        public static void RegisterNonComplexType(Type t)
        {
            PrimitiveTypesTable[t] = t;
        }
        
        
		/// <internalonly/>
		public static bool GetStandardValuesExclusive(PropertyDescriptor pd)
		{
			Type t = pd.PropertyType;
//			if (t == typeof(System.Drawing.Color))
//				return true;

			return pd.Converter.GetStandardValuesExclusive();
		}

		/// <internalonly/>
		public static string GetListName(object list)
		{
			if (list is ITypedList)
			{
				string s = ((ITypedList) list).GetListName(null);
				return s;
			}
			return null;
		}

		/// <internalonly/>
		public static System.Data.DataRelation GetRelatedDataRelation(PropertyDescriptor pd)
		{
			if (pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
			{
				if (mInfo == null)
				{
					Type t = pd.GetType();
					PropertyInfo pi = t.GetProperty("Relation",
						System.Reflection.BindingFlags.GetProperty
						| System.Reflection.BindingFlags.Public
						| System.Reflection.BindingFlags.IgnoreReturn
						| System.Reflection.BindingFlags.Instance
						| System.Reflection.BindingFlags.NonPublic);
					if (pi != null)
						mInfo = pi.GetGetMethod(true);

				}

				if (mInfo != null)
				{
					System.Data.DataRelation dr = mInfo.Invoke(pd, new object[0]) as System.Data.DataRelation;
					if (dr != null)
					{
						return dr;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the <see cref="System.Data.DataRelation.ChildTable"/> or DataTable of the <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public static IList GetRelatedList(PropertyDescriptor pd)
		{
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
                {
                    if (mInfo == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Relation",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo = pi.GetGetMethod(true);

                    }

                    if (mInfo != null)
                    {
                        System.Data.DataRelation dr = mInfo.Invoke(pd, new object[0]) as System.Data.DataRelation;
                        if (dr != null)
                            return dr.ChildTable.DefaultView;
                    }
                }

                if (pd.GetType().FullName == "System.Data.DataTablePropertyDescriptor")
                {
                    if (mInfo2 == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Table",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo2 = pi.GetGetMethod(true);

                    }

                    if (mInfo2 != null)
                        return (mInfo2.Invoke(pd, new object[0]) as System.Data.DataTable).DefaultView;
                }
            }

			return null;
		}

		/// <summary>
		/// Returns the <see cref="System.Data.DataColumn"/> of the <see cref="PropertyDescriptor"/> if it
		/// is a DataColumnPropertyDescriptor.
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public static DataColumn GetDataColumn(PropertyDescriptor pd)
		{
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (pd != null && pd.GetType().FullName == "System.Data.DataColumnPropertyDescriptor")
                {
                    if (mInfo3 == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Column",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo3 = pi.GetGetMethod(true);

                    }

                    if (mInfo3 != null)
                        return mInfo3.Invoke(pd, new object[0]) as System.Data.DataColumn;
                }
            }

			return null;
		}

        /// <summary>
        /// Determines and returns the DataTable object for the given list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
		public static DataTable GetDataTable(object list)
		{
			if (list is DataTable)
				return (DataTable) list;
			else if (list is DataView)
				return ((DataView) list).Table;
			return null;
		}


		/// <summary>
		/// Returns the <see cref="System.Data.DataRelation.ChildTable"/> or DataTable of the <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="pd"></param>
		/// <returns></returns>
		public static System.Data.DataTable GetRelatedDataTable(PropertyDescriptor pd)
		{
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
                {
                    if (mInfo == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Relation",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo = pi.GetGetMethod(true);

                    }

                    if (mInfo != null)
                    {
                        System.Data.DataRelation dr = mInfo.Invoke(pd, new object[0]) as System.Data.DataRelation;
                        if (dr != null)
                            return dr.ChildTable;
                    }
                }

                if (pd.GetType().FullName == "System.Data.DataTablePropertyDescriptor")
                {
                    if (mInfo2 == null)
                    {
                        Type t = pd.GetType();
                        PropertyInfo pi = t.GetProperty("Table",
                            System.Reflection.BindingFlags.GetProperty
                            | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.IgnoreReturn
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.NonPublic);
                        if (pi != null)
                            mInfo2 = pi.GetGetMethod(true);

                    }

                    if (mInfo2 != null)
                        return (mInfo2.Invoke(pd, new object[0]) as System.Data.DataTable);
                }
            }

			return null;
		}

		private static PropertyDescriptorCollection GetInstanceProperties(Type type, bool isCollection)
		{
            // fyi - in Whidbey there is now a ListBindingHelper.GetListItemProperties(list);
            
            object item = null;
			try
			{
				ConstructorInfo ci = type.GetConstructor(new Type[0]);
				if (ci != null)
					item = Activator.CreateInstance(type);
			}
			catch (MissingMethodException ex)
			{
				// no parameterless constructor found.
				// Now I have to fallback on meta data descriptors (without support for ICustomTypeDescriptor)
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                }
			}

			if (item != null)
			{
				if (isCollection)
					return GetItemProperties(item);
				else
					return TypeDescriptor.GetProperties(item, new Attribute[] { new BrowsableAttribute(true) }, false);
			}

			return TypeDescriptor.GetProperties(type, new Attribute[] { new BrowsableAttribute(true) });
		}

		/// <summary>
		/// Returns the properties for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static PropertyDescriptorCollection GetItemProperties(Type type)
		{
			if (type == null)

				return new PropertyDescriptorCollection(new PropertyDescriptor[0]);

			else if (typeof(ITypedList).IsAssignableFrom(type))

				return GetInstanceProperties(type, true);

			else if (typeof(System.Array).IsAssignableFrom(type))

				return GetInstanceProperties(type.GetElementType(), false);

			else
			{
				PropertyInfo[] propertyInfos = type.GetProperties();
				for (int index = 0; index < propertyInfos.Length; index++)
				{
					if ("Item".Equals(propertyInfos[index].Name)
						&& propertyInfos[index].PropertyType != typeof(object))
					{
						return GetInstanceProperties(propertyInfos[index].PropertyType, false);
					}
				}
			}

			return GetInstanceProperties(type, false);
		}

        /// <summary>
        /// Returns the type of the items in the list if the list is strong-typed.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Type GetListItemType(object list)
        {
            if (list != null)
                return GetListItemType(list.GetType());

            return null;
        }

		/// <summary>
		/// Returns the type of the items in the list if the list is strong-typed.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static Type GetListItemType(Type list)
		{
			PropertyInfo[] propertyInfos = list.GetProperties();
			for (int index = 0; index < propertyInfos.Length; index++)
			{
				if ("Item".Equals(propertyInfos[index].Name)
					&& propertyInfos[index].PropertyType != typeof(object))
				{
					return propertyInfos[index].PropertyType;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the properties for the object.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <returns></returns>
		public static PropertyDescriptorCollection GetItemProperties(object dataSource)
		{
			if (dataSource == null)
				return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			else
			{
				IEnumerable list = dataSource as IEnumerable;
				if (dataSource is IListSource)
					list = ((IListSource) dataSource).GetList();

				if (dataSource is ITypedList)

					return ((ITypedList)(dataSource)).GetItemProperties(null);

				else if (list is ITypedList)

					return ((ITypedList)(list)).GetItemProperties(null);

				else if (list != null)
				{
					PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                    IEnumerator enumerator = list.GetEnumerator();
                    
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == null)
                            continue;
                        object item = enumerator.Current;
                        pdc = TypeDescriptor.GetProperties(item, new Attribute[] { new BrowsableAttribute(true) });

                        if (pdc.Count == 0)
                        {
                            FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                            if (fields.Length > 0)
                            {
                                pdc = CreateFieldWrapperPropertyDescriptors(item.GetType(), fields);
                            }
                        }
                    }

					if (pdc.Count == 0)
						pdc = GetItemProperties(list.GetType());

					if (pdc.Count > 0)
						return pdc;
				}

				return GetItemProperties(dataSource.GetType());

			}
		}

        static PropertyDescriptorCollection CreateFieldWrapperPropertyDescriptors(Type type, FieldInfo[] fields)
        {
            PropertyDescriptor[] pds = new PropertyDescriptor[fields.Length];
            for (int n = 0; n < fields.Length; n++)
                pds[n] = new FieldInfoWrapperPropertyDescriptor(fields[n].Name, fields[n], null, type);
            return new PropertyDescriptorCollection(pds);
        }

		static Hashtable notfoundKeys;

		/// <summary>
		/// Returns the value for the ValueMember of the specified item.
		/// </summary>
		/// <param name="item">The row item.</param>
		/// <param name="dataSource">The list</param>
		/// <param name="valueMember">The name of the value member</param>
		/// <returns>The value of the ValueMember.</returns>
		public static object GetItemValue(object dataSource, string valueMember, object item)
		{
			if (valueMember == null || valueMember == "")
				return item;

			PropertyDescriptorCollection pdc = ListUtil.GetItemProperties(dataSource);
			PropertyDescriptor pd = pdc[valueMember];
			if (pd == null)
			{
				// try case-insensitive search.
				string lvalueMember = valueMember.ToLower();

				foreach (PropertyDescriptor p in pdc)
				{
					if (p.Name.ToLower() == lvalueMember)
					{
						pd = p;
						break;
					}
				}

				if (pd == null)
				{
					if (notfoundKeys == null)
						notfoundKeys = new Hashtable();
					if (!notfoundKeys.Contains(valueMember))
					{
						string source = ListUtil.GetListName(dataSource);
						if (source == null)
						{
							if (item != null)
								source = item.GetType().FullName;
							else
								source = dataSource.GetType().FullName;
						}

						//Console.WriteLine("GridComboBoxCellModel.GetItemValue: {0} was not found in {1}", valueMember, source);
						notfoundKeys.Add(valueMember, null);
					}
					return item;
				}
			}

			object value = pd.GetValue(item);// this.FilterItemOnProperty(item, this.ValueMember);
			return value;
		}

	}

    internal class FieldInfoWrapperPropertyDescriptor : PropertyDescriptor
    {
        FieldInfo field;
        Type componentType;

        internal FieldInfoWrapperPropertyDescriptor(string propertyName, FieldInfo field, Attribute[] attrArray, Type componentType)
            : base(propertyName, attrArray)
        {
            this.field = field;
            this.componentType = componentType;
        }

        public override bool CanResetValue(object comp)
        {
            return false;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override object GetValue(object comp)
        {
            return field.GetValue(comp);
        }

        public override void ResetValue(object comp)
        {
        }

        public override void SetValue(object comp, object value)
        {
            field.SetValue(comp, value);
        }

        public override Type ComponentType
        {
            get
            {
                return componentType;
            }
        }
        public override string DisplayName
        {
            get
            {
                return field.Name;
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public override Type PropertyType
        {
            get
            {
                return field.FieldType;
            }
        }
    }
}
