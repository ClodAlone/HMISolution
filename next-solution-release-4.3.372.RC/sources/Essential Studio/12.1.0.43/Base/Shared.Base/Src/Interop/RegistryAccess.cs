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
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using System.Windows.Forms;
using System.Collections;

namespace Syncfusion.Win32
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class RegistryAccessHelper
	{
		public static void PersistValueAt(RegistryKey key, string subkey, string valueName, string source, bool bThrowException)
		{
			PersistValueAtCommon(key, subkey, valueName, source, bThrowException);
		}

		[CLSCompliant(false)] 
		public static void PersistValueAt(RegistryKey key, string subkey, string valueName, ulong source, bool bThrowException)
		{
			PersistValueAtCommon(key, subkey, valueName, source, bThrowException);
		}

		private static void PersistValueAtCommon(RegistryKey key, string subkey, string valueName, object source, bool bThrowException)
		{
			if((key == null || subkey == null)
				&& bThrowException)
			{

				System.ArgumentNullException e = new System.ArgumentNullException();
				throw e;
			}

			try
			{
				RegistryKey regKey = key;
				regKey = regKey.CreateSubKey(subkey);
				if(regKey != null)
					regKey.SetValue(valueName, source);
			}
			catch{if(bThrowException) throw;}
		}

		public static void PersistValueAt(RegistryKey key, string subkey, string valueName, object source, bool bThrowException)
		{
			// Store value as binary
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, source);
			PersistValueAtCommon(key, subkey, valueName, stream.ToArray(), bThrowException);
		}

		public static void PersistValueAt(RegistryKey key, string subkey, string valueName, bool source, bool bThrowException)
		{
			PersistValueAtCommon(key, subkey, valueName, source.ToString(), bThrowException);
		}

		private static void GetValueFromCommon(RegistryKey key, string subkey, string valueName, out object destination, object defaultValue, bool bThrowException)
		{
			destination = defaultValue;
			if((key == null || subkey == null)
				&& bThrowException)
			{

				System.ArgumentNullException e = new System.ArgumentNullException();
				throw e;
			}
			
			try
			{
				RegistryKey regKey = key;
				regKey = regKey.OpenSubKey(subkey);
				if(regKey != null)
					destination = regKey.GetValue(valueName, defaultValue);
			}
			catch{if(bThrowException) throw;}
		}

		public static void GetValueFrom(RegistryKey key, string subkey, string valueName, out string destination, string defaultValue, bool bThrowException)
		{
			destination = defaultValue;
			object stringValue;
			GetValueFromCommon(key, subkey, valueName, out stringValue, defaultValue, bThrowException);
			if(stringValue != null)
				destination = (string)stringValue;
		}

		[CLSCompliant(false)] 
		public static void GetValueFrom(RegistryKey key, string subkey, string valueName, out ulong destination, ulong defaultValue, bool bThrowException)
		{
			destination = defaultValue;
			object ulongValue;
			GetValueFromCommon(key, subkey, valueName, out ulongValue, defaultValue, bThrowException);
			if(ulongValue != null)
				destination = (ulong)ulongValue;
		}

		public static void GetValueFrom(RegistryKey key, string subkey, string valueName, out bool destination, bool defaultValue, bool bThrowException)
		{
			destination = defaultValue;
			object boolValue;
			GetValueFromCommon(key, subkey, valueName, out boolValue, defaultValue.ToString(), bThrowException);
			if(boolValue != null)
				destination = Boolean.TrueString == (string)boolValue;
		}

		public static void GetValueFrom(RegistryKey key, string subkey, string valueName, out object destination, object defaultValue, bool bThrowException)
		{
			destination = defaultValue;
			object binaryObject;
			GetValueFromCommon(key, subkey, valueName, out binaryObject, null, bThrowException);
			if(binaryObject != null)
			{
				byte[] binaryData = (byte[])binaryObject;
				// Convert the binary data to its object
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();

				stream.Write(binaryData, 0, binaryData.Length);
				stream.Position = 0;
				try
				{
					destination = formatter.Deserialize(stream);
				}
				catch{if(bThrowException) throw;}
			}

			if(destination == null)
				destination = defaultValue;
		}
	}
	/// <summary>
	/// A structure that represents a location in the registry.
	/// </summary>
	/// <remarks>
	/// This is a simple structure that refers to a Registry location
	/// through the root RegistryKey and the subkey.
	/// </remarks>
	[Syncfusion.Documentation.DocumentationExclude()]
	public struct RegistryEntry
	{
		private string registrySubKey;
		private RegistryKey registryKey;

		/// <summary>
		/// Creates a new instance of the RegistryEntry class and
		/// initializes it with the root RegistryKey and subkey.
		/// </summary>
		/// <param name="key">The root RegistryKey.</param>
		/// <param name="subkey">The subkey string.</param>
		public RegistryEntry(RegistryKey key, string subkey)
		{
			this.registryKey = key;
			this.registrySubKey = subkey;
		}

		/// <summary>
		/// Gets / sets the root RegistryKey.
		/// </summary>
		/// <value>A RegistryKey value.</value>
		public RegistryKey SourceRegistryKey
		{
			get{return this.registryKey;}
			set{this.registryKey = value;}
		}

		/// <summary>
		/// Gets /sets the subkey under the root RegistryKey.
		/// </summary>
		/// <value>A string value representing the subkey.</value>
		public string SourceRegistrySubKey
		{
			get	{return this.registrySubKey;}
			set {this.registrySubKey = value;}
		}
	}
}
