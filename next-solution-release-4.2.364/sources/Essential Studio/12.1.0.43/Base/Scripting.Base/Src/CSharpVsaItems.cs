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

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;

using Microsoft.Vsa;
#endregion

namespace Syncfusion.Scripting
{
	/// <summary>
	/// Implements an interface for a collection of IVsaItem objects, 
	/// which can be addressed either by name or by index.
	/// </summary>
	public class CSharpVsaItems : IVsaItems
	{
		#region Class members
		/// <summary>
		/// Keeps the name-to-index hashtable for searched item by name.
		/// </summary>
		private Hashtable m_hasItemsToIndex = new Hashtable();

		/// <summary>
		/// Keeps the vsa items.
		/// </summary>
		private ArrayList m_items = new ArrayList();

		/// <summary>
		/// Reference to script enginew
		/// </summary>
		private CSharpScriptEngine m_engine;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Construct a CSharpVsaItems given a script engine.
		/// </summary>
		public CSharpVsaItems( CSharpScriptEngine engine )
		{
			if (engine == null)
			{
				throw new ArgumentNullException("engine");
			}

			m_engine = engine;
		}
		#endregion

		#region IVsaItems implementations
		/// <summary>
		/// Gets an item from the collection by its index value.
		/// </summary>
		public IVsaItem this[int index]
		{
			get
			{
				if (m_engine.IsClosed)
				{
					throw new VsaException(VsaError.EngineClosed);
				}

				if (!m_engine.IsInitialized)
				{
					throw new VsaException(VsaError.EngineNotInitialized);
				}

				if (index < 0 || index > m_items.Count - 1)
				{
					throw new VsaException(VsaError.ItemNotFound);
				}

				if (index < 0 || index > m_items.Count - 1)
				{
					throw new VsaException(VsaError.ItemNotFound);
				}

				return m_items[index] as IVsaItem;
			}
		}

		/// <summary>
		/// Gets an item from the collection by its name.
		/// </summary>
		public IVsaItem this[string name]
		{
			get
			{
				if (m_engine.IsClosed)
				{
					throw new VsaException(VsaError.EngineClosed);
				}

				if (!m_engine.IsInitialized)
				{
					throw new VsaException(VsaError.EngineNotInitialized);
				}

				if (!m_hasItemsToIndex.ContainsKey(name))
				{
					throw new VsaException(VsaError.ItemNotFound);
				}

				int index = (int)m_hasItemsToIndex[name];
				return m_items[index] as IVsaItem;
			}
		}

		/// <summary>
		/// Gets the number of items in the specified collection.
		/// </summary>
		public int Count
		{
			get
			{
				if (m_engine.IsClosed)
				{
					throw new VsaException(VsaError.EngineClosed);
				}

				if (!m_engine.IsInitialized)
				{
					throw new VsaException(VsaError.EngineNotInitialized);
				}

				return m_items.Count;
			}
		}

		/// <summary>
		///  Removes an item from the collection.
		/// </summary>
		/// <param name="index"></param>
		public void Remove( int index )
		{
			if (m_engine.IsClosed)
			{
				throw new VsaException(VsaError.EngineClosed);
			}

			if (m_engine.IsRunning)
			{
				throw new VsaException(VsaError.EngineRunning);
			}

			if (!m_engine.IsInitialized)
			{
				throw new VsaException(VsaError.EngineNotInitialized);
			}

			m_items.RemoveAt(index);
		}

		/// <summary>
		/// Overloaded. Removes an item from the collection by item name.
		/// </summary>
		/// <param name="name"></param>
		void IVsaItems.Remove( string name )
		{
			if (m_engine.IsClosed)
			{
				throw new VsaException(VsaError.EngineClosed);
			}

			if (m_engine.IsRunning)
			{
				throw new VsaException(VsaError.EngineRunning);
			}

			if (!m_engine.IsInitialized)
			{
				throw new VsaException(VsaError.EngineNotInitialized);
			}

			if (!m_hasItemsToIndex.ContainsKey(name))
			{
				throw new VsaException(VsaError.ItemNotFound);
			}

			int index = (int)m_hasItemsToIndex[name];
			m_items.RemoveAt(index);
		}

		/// <summary>
		/// Creates a new instance of one of the IVsaItem types, 
		/// as defined in the VsaItemType enumeration.
		/// </summary>
		/// <param name="name">The name to associate with the new item</param>
		/// <param name="itemType">The type of item created, as defined in the VsaItemType enumeration</param>
		/// <param name="itemFlag">The optional flag to specify the initial content of a Code item. </param>
		/// <returns>Returns a reference to the IVsaItem object created</returns>
		public IVsaItem CreateItem( string name, VsaItemType itemType, VsaItemFlag itemFlag )
		{
			IVsaItem item = null;

			switch (itemType)
			{
				case VsaItemType.AppGlobal:
					item = new CSharpVsaGlobalItem(m_engine, name);
					break;
				case VsaItemType.Code:
					item = new CSharpVsaCodeItem(m_engine, name, itemFlag);
					break;
				case VsaItemType.Reference:
					if (ContainsName(name))
						item = (IVsaItem)m_items[(int)m_hasItemsToIndex[name]];
					else
						item = new CSharpVsaReferenceItem(m_engine, name);
					break;
			}

			int index = m_items.Add(item);
			m_hasItemsToIndex[item.Name] = index;

			return item;
		}
		#endregion

		#region IEnumerable implementations
		/// <summary>
		/// Gets the VsaItems enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_items.GetEnumerator();
		}
		#endregion

		#region Class helper methods
		/// <summary>
		/// Checks if contains item with specified name.
		/// </summary>
		/// <param name="name">Item name.</param>
		/// <returns>tru if item exists or false if not</returns>
		public bool ContainsName( string name )
		{
			return m_hasItemsToIndex.ContainsKey(name);
		}
		#endregion
	}
}