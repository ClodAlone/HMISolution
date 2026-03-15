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

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Syncfusion.Styles
{
	/// <internalonly/>
	[DebuggerStepThrough()]
	public class StyleInfoObjectStore : IDisposable
	{
		private ObjectEntry[] objEntries;
		private static int currentKey;

		/// <internalonly/>
		public StyleInfoObjectStore()
		{
		}

		/// <internalonly/>
		public void Dispose()
		{
			if (objEntries != null)
			{
				for (int index = 0; index < objEntries.Length; index++)
				{
					_Dispose(objEntries[index].Value1);
					_Dispose(objEntries[index].Value2);
					_Dispose(objEntries[index].Value3);
					_Dispose(objEntries[index].Value4);
					objEntries[index].Value1 = null;
					objEntries[index].Value2 = null;
					objEntries[index].Value3 = null;
					objEntries[index].Value4 = null;
				}
				objEntries = null;
			}
			GC.SuppressFinalize(this);
		}

		void _Dispose(object obj)
		{
			if (obj is StyleInfoSubObjectBase)
				((StyleInfoSubObjectBase) obj).Dispose();
		}

		/// <internalonly/>
		public bool ContainsObject(int key)  
		{
			bool exists;
			this.GetObject(key, out exists);
			return exists;
		}
		
		/// <internalonly/>
		public static int CreateKey()  
		{
			return currentKey++;
		}

		/// <internalonly/>
		public object GetObject(int key, out bool found)  
		{
			object obj = null;
			int index;
			short element;
			short entryKey = this.SplitKey(key, out element);
			found = false;
			if (this.LocateObjectEntry(entryKey, element, out index))
			{
				if ( ((1 << (element & (short)0x1f)) & this.objEntries[index].Mask) != 0 ) 
				{
					found = true;
					switch (element) 
					{
						case 0:
							obj = this.objEntries[index].Value1;
							break;
						case 1:
							obj = this.objEntries[index].Value2;
							break;
						case 2:
							obj = this.objEntries[index].Value3;
							break;
						case 3:
							obj = this.objEntries[index].Value4;
							break;
					}
				}
			}

			return obj;
		}


		/// <internalonly/>
		public object GetObject(int key)  
		{
			bool exists;
			return this.GetObject(key, out exists);
		}

		private bool LocateObjectEntry(short entryKey, short element, out int index)  
		{
			if (this.objEntries != null)
			{
				int upperBound = (((int) this.objEntries.Length) - 1);
				int lowerBound = 0;
				int divide = 0;
				short key;
				
				do
				{
					divide = ((upperBound + lowerBound)/2);
					key = this.objEntries[divide].Key;
					if (key == entryKey)
					{
						index = divide;
						return true;
					}
					else if (entryKey < key) 
						upperBound = (divide - 1);
					else
						lowerBound = (divide + 1);
				}
				while (upperBound >= lowerBound);

				index = divide;
				if (entryKey > this.objEntries[divide].Key) 
					index++;
				return false;
			}
			index = 0;
			return false;
		}

		/// <internalonly/>
		public void SetObject(int key, object value)  
		{
			int index;
			short element;
			short entryKey = this.SplitKey(key, out element);
			ObjectEntry[] objEntries;
			if (!this.LocateObjectEntry(entryKey,element,out index)) 
			{
				if (this.objEntries != null)
				{
					objEntries = new ObjectEntry[this.objEntries.Length + 1];
					if (index > 0) 
						Array.Copy(this.objEntries, 0, objEntries, 0, index);
					
					if (this.objEntries.Length - index > 0) 
						Array.Copy(this.objEntries, index, objEntries, (index + 1), this.objEntries.Length - index);
					
					this.objEntries = objEntries;
				}
				else
					this.objEntries = new ObjectEntry[1];
				this.objEntries[index].Key = entryKey;
			}

			switch(element) 
			{
				case 0:
					this.objEntries[index].Value1 = value;
					break;
				case 1:
					this.objEntries[index].Value2 = value;
					break;
				case 2:
					this.objEntries[index].Value3 = value;
					break;
				case 3:
					this.objEntries[index].Value4 = value;
					break;
			}

			this.objEntries[index].Mask = (short) ((ushort) this.objEntries[index].Mask | (1 << (element & 0x1f)));
		}

		/// <internalonly/>
		public void RemoveObject(int key)  
		{
			int index;
			short element;
			short entryKey = this.SplitKey(key, out element);
			ObjectEntry[] objEntries;
			if (this.LocateObjectEntry(entryKey,element,out index)) 
			{
				this.objEntries[index].Mask = (short) (this.objEntries[index].Mask & ~((1 << element) & 0x1f)) ;

				if ((this.objEntries[index].Mask & 0x1f) == 0)
				{
					if (this.objEntries.Length == 1)
						this.objEntries = null;
					else
					{
						objEntries = new ObjectEntry[this.objEntries.Length - 1];
						if (index > 0) 
							Array.Copy(this.objEntries, 0, objEntries, 0, index);
					
						int length = this.objEntries.Length - index - 1;
						if (length > 0) 
							Array.Copy(this.objEntries, index+1, objEntries, index, length);
						this.objEntries = objEntries;
					}			
				}
			}
		}

		private short SplitKey(int key, out short element)  
		{
			element = (short) (key & 0x03);
			return (short) (key & ~0x03);
		}

			[StructLayout(LayoutKind.Sequential)] 
		private struct ObjectEntry 
		{
			public short Key;
			public short Mask;
			public object Value1;
			public object Value2;
			public object Value3;
			public object Value4;
		} // end of struct ObjectEntry
	}
}
