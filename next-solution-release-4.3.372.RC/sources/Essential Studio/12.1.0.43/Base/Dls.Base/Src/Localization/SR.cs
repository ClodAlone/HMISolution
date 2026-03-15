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
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using System.Diagnostics;

// Tip:
// To simplify rebuilding this class while working on your project
// you should add the following External Tool in Developer Studio:
// 
//     Title: Resgen SR.TXT
//     Command: C:\WINNT\system32\CMD.EXE
//     Arguments: /c resgen SR.txt & srgen @srgen.ini & echo Done.
//     Initial Directory: $(ProjectDir)
//     Please enable "X Use Output for Window."
// 
// Additionally you should add a file srgen.ini to project that 
// contains command line arguments for your project.
//
// Example: 
// SRGen.ini
// sr.txt namespace:Samples.SRGenSample classname:SR

namespace Syncfusion.DLS.Localization
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.DLS.Localization.SR.resources
    /// </summary>
	sealed class SR 
	{
		// Fields
		private ResourceManager resources;
		private static SR loader = null;

		// Strings 
		internal const string Testing = "Testing";


		private SR()  
		{
			this.resources = new ResourceManager(this.GetType());
		}

		// Methods
		private static SR GetLoader()  
		{
			lock(typeof(SR))
			{
				if (SR.loader == null)
					SR.loader = new SR();
				return SR.loader;
			}
		}

		// Methods

		public static string GetString(CultureInfo culture, string name, params object[] args)  
		{
			SR sr = SR.GetLoader();
			string value;
			
			if (sr == null) 
				return null;

			try
			{
				value = sr.resources.GetString(name, culture);
				if (value != null && args != null && args.Length > 0) 
					return String.Format(value,args);
			
				return value;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return name;
			}
		}


		public static string GetString(string name)  
		{
			return SR.GetString(null, name);
		}


		public static string GetString(string name, params object[] args)  
		{
			return SR.GetString(null, name,args);
		}


		public static string GetString(CultureInfo culture, string name)  
		{
			SR sr = SR.GetLoader();
			if (sr == null) 
				return null;
			return sr.resources.GetString(name, culture);
		}


		public static object GetObject(CultureInfo culture, string name)  
		{
			SR sr = SR.GetLoader();
			if (sr == null) 
				return null;
			return sr.resources.GetObject(name, culture);
		}


		public static object GetObject(string name)  
		{
			return SR.GetObject(null, name);
		}



		public static bool GetBoolean(CultureInfo culture, string name)  
		{
			bool value;
			SR sr = SR.GetLoader();
			object obj;
			value = false;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Boolean) 
					value = ((bool) obj);
			}
			return value;
		}


		public static bool GetBoolean(string name)  
		{
			return SR.GetBoolean(name);
		}


		public static byte GetByte(CultureInfo culture, string name)  
		{
			byte value;
			SR sr = SR.GetLoader();
			object obj;
			value = (byte)0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Byte) 
					value = ((byte) obj);
			}
			return value;
		}


		public static byte GetByte(string name)  
		{
			return SR.GetByte(null, name);
		}


		public static char GetChar(CultureInfo culture, string name)  
		{
			char value;
			SR sr = SR.GetLoader();
			object obj;
			value = (char)0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Char)
					value = (char) obj;
			}
			return value;
		}


		public static char GetChar(string name)  
		{
			return SR.GetChar(null, name);
		}


		public static double GetDouble(CultureInfo culture, string name)  
		{
			double value;
			SR sr = SR.GetLoader();
			object obj;
			value = 0.0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Double) 
					value = ((double) obj);
			}
			return value;
		}


		public static double GetDouble(string name)  
		{
			return SR.GetDouble(null, name);
		}


		public static float GetFloat(CultureInfo culture, string name)  
		{
			float value;
			SR sr = SR.GetLoader();
			object obj;
			value = 0.0f;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Single) 
					value = ((float)obj);
			}
			return value;
		}


		public static float GetFloat(string name)  
		{
			return SR.GetFloat(null, name);
		}


		public static int GetInt(string name)  
		{
			return SR.GetInt(null, name);
		}


		public static int GetInt(CultureInfo culture, string name)  
		{
			int value;
			SR sr = SR.GetLoader();
			object obj;
			value = 0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Int32) 
					value = ((int) obj);
			}
			return value;
		}


		public static long GetLong(string name)  
		{
			return SR.GetLong(null, name);
		}


		public static long GetLong(CultureInfo culture, string name)  
		{
			Int64 value;
			SR sr = SR.GetLoader();
			object obj;
			value = ((Int64) 0);
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Int64)
					value = ((Int64) obj);
			}
			return value;
		}

		public static short GetShort(CultureInfo culture, string name)  
		{
			short value;
			SR sr = SR.GetLoader();
			object obj;
			value = (short)0;
			if (sr != null)
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Int16)
					value = ((short) obj);
			}
			return value;
		}


		public static short GetShort(string name)  
		{
			return SR.GetShort(null, name);
		}
	}

	/// <summary>
	/// Specifies the category in which the property or event will be displayed in a visual designer.
	/// </summary>
	/// <remarks>
	/// This is a localized version of CategoryAttribute. The localized string will be loaded from the 
	/// assembly manifest Syncfusion.DLS.Localization.SR.resources
	/// </remarks>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)] 
	sealed class SRCategoryAttribute : CategoryAttribute
	{
		public SRCategoryAttribute(string category)
			: base(category)
		{
		} 
        
		protected override string GetLocalizedString(string value)
		{
			return SR.GetString(value);
		} 
	} 

	/// <summary>
	/// Specifies a description for a property or event.
	/// </summary>
	/// <remarks>
	/// This is a localized version of DescriptionAttribute. The localized string will be loaded from the 
	/// assembly manifest Syncfusion.DLS.Localization.SR.resources
	/// </remarks>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)] 
	sealed class SRDescriptionAttribute : DescriptionAttribute
	{
		private bool replaced = false;
        
		public SRDescriptionAttribute(string description)
			: base(description)
		{
		} 
        
		public override string Description 
		{ 
			get
			{
				if (!this.replaced) 
				{
					this.replaced = true;
					this.DescriptionValue = SR.GetString(base.Description);
				}
				return base.Description;
			} 
		}
	}	
} // end of namespace Syncfusion.DLS.Localization
