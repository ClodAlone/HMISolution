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
using System.Diagnostics;
using System.Text;
using System.Reflection;

namespace Syncfusion.ComponentModel
{
	/// <summary>
	/// Specifies whether a property should be shown in the ToString result.
	/// </summary>
	/// <seealso cref="SyncfusionEventArgs"/>
	[
	AttributeUsageAttribute(AttributeTargets.Property)
	]
	public sealed class TracePropertyAttribute : Attribute 
	{
		// Fields
		private bool traceProperty;
        
		/// <summary>
		///   <para>Specifies that a property should be shown in the ToString result. 
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly TracePropertyAttribute Yes;

		/// <summary>
		///   <para>Specifies that a property should not be shown in the ToString result. 
		///   This <see langword="static" /> field is Read-only.</para>
		/// </summary>
		public static readonly TracePropertyAttribute No;

		/// <summary>
		/// The default value for TracePropertyAttribute. (No)
		/// </summary>
		public static readonly TracePropertyAttribute Default;
        
		// Constructors
        
		static TracePropertyAttribute()
		{
			TracePropertyAttribute.Yes = new TracePropertyAttribute(true);
			TracePropertyAttribute.No = new TracePropertyAttribute(false);
			TracePropertyAttribute.Default = TracePropertyAttribute.No;
		} 
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="TracePropertyAttribute" /> class.</para>
		/// </summary>
		/// <param name="traceProperty">
		///   <see langword="True" /> if a property should be shown in ToString result; <see langword="False" /> otherwise. The default is <see langword="True" />.</param>
		public TracePropertyAttribute(bool traceProperty)
		{
			this.traceProperty = traceProperty;
		} 
        
		// Methods
        
		/// <override/>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(TracePropertyAttribute.Default);
		} 
        
		/// <summary>
		/// Overridden. See <see cref="System.Attribute.GetHashCode"/>.
		/// </summary>
		public override int GetHashCode()
		{
			return this.traceProperty.GetHashCode();
		} 
        
		/// <override/>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			TracePropertyAttribute attr = obj as TracePropertyAttribute;
			if (attr != null)
				return (attr.TraceProperty == this.traceProperty);
			return false;
		} 
        
		/// <summary>
		///   <para>Indicates whether a property is shown in the ToString result.</para>
		/// </summary>
		public bool TraceProperty 
		{ 
			get
			{
				return this.traceProperty;
			} 
		}
	}

	/// <summary>
	/// Helper class for creating a string concatenating the string representation of all properties in an object.
	/// </summary>
	/// <remarks>
	/// The static <see cref="TraceProperties.ToString"/> method of this helper class will loop through any property
	/// in a given object and check if the <see cref="TracePropertyAttribute"/> has been set. If it has
	/// been set, the string representation of the property will be appended to the resulting string.
	/// </remarks>
	public class TraceProperties
	{
		/// <summary>
		/// Indicates whether the <see cref="TracePropertyAttribute"/> has been set for the property.
		/// </summary>
		/// <param name="info">A <see cref="PropertyInfo"/>.</param>
		/// <returns>True if property has a <see cref="TracePropertyAttribute"/>; False otherwise.</returns>
		public static bool IsTraceProperty(PropertyInfo info)
		{
			TracePropertyAttribute attr = TracePropertyAttribute.Default;
			if (info.IsDefined(typeof(TracePropertyAttribute), true))
			{
				object[] array = info.GetCustomAttributes(typeof(TracePropertyAttribute), true);
				attr = (TracePropertyAttribute) array[0];
			}
			return attr.TraceProperty;
		}

		/// <summary>
		/// This method will loop through any property in a given object and append the
		/// string representation of the property if the <see cref="TracePropertyAttribute"/>
		/// has been set.
		/// </summary>
		[DebuggerStepThrough()]
		public static string ToString(object obj)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(obj.GetType().Name);
			sb.Append(" { ");
			bool comma = false;
		
			PropertyInfo[] properties = obj.GetType().GetProperties();
			if (properties != null)
			{
				foreach (PropertyInfo info in properties)
				{
					TypeConverter tc = TypeDescriptor.GetConverter(info.PropertyType);
					if (info.CanRead && tc.CanConvertTo(typeof(string))
						&& TraceProperties.IsTraceProperty(info))
					{
						if (!comma)
							comma = true;
						else
							sb.Append(", ");
						string s = (string) tc.ConvertTo(info.GetValue(obj, null), typeof(string));
						sb.Append(info.Name);
						sb.Append("=");
						if (info.PropertyType == typeof(string))
							sb.Append("\"" + s + "\"");
						else
							sb.Append(s);
					}
				}
			}
			sb.Append(" }");
		
			return sb.ToString();
		}
	}

	/// <summary>
	/// This is a base class for events of the Syncfusion libraries. It supports writing
	/// properties in its ToString() method. 
	/// </summary>
	public class SyncfusionEventArgs : EventArgs
	{
		/// <summary>
		/// This method will loop through all properties in a derived class and append the
		/// string representation of the property if the <see cref="TracePropertyAttribute"/>
		/// has been set.
		/// </summary>
		[DebuggerStepThrough()]
		public override string ToString()
		{
			return TraceProperties.ToString(this);
		}
	}

	/// <summary>
	/// Provides data for a cancellable event. 
	/// </summary>
	public class SyncfusionCancelEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Overloaded. Initializes a new instance of the SyncfusionCancelEventArgs class.
		/// </summary>
		public SyncfusionCancelEventArgs()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SyncfusionCancelEventArgs class with the Cancel property set to the given value.
		/// </summary>
		public SyncfusionCancelEventArgs(bool cancel)
			: base(cancel)
		{
		}

		/// <override/>
		[DebuggerStepThrough()]
		public override string ToString()
		{
			return TraceProperties.ToString(this);
		}
	}

	/// <summary>
	/// Provides data for an event that indicates success or failure. 
	/// </summary>
	public class SyncfusionSuccessEventArgs : SyncfusionEventArgs
	{
		bool success;

		/// <summary>
		/// Overloaded. Initializes a new instance of the SyncfusionSuccessEventArgs class with the Success property set to True.
		/// </summary>
		public SyncfusionSuccessEventArgs()
			: this(true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SyncfusionSuccessEventArgs class with the Success property set to the given value.
		/// </summary>
		/// <param name="success"> Indicates whether an operation was successful.</param>
		public SyncfusionSuccessEventArgs(bool success)
		{
			this.success = success;
		}

		/// <summary>
		/// Indicates whether an operation was successful.
		/// </summary>
		[TraceProperty(true)]
		public bool Success
		{
			get
			{
				return success;
			}
		}
	}

	/// <summary>
	/// Provides data for a event that can be handled by a subscriber and overrides the event's default behavior.
	/// </summary>
	public class SyncfusionHandledEventArgs : SyncfusionEventArgs
	{
		bool handled;

		/// <summary>
		/// Overloaded. Initializes a new instance of the SyncfusionHandledEventArgs class with the Handled property set to False.
		/// </summary>
		public SyncfusionHandledEventArgs() 
		{
			handled = false;
		}

		/// <summary>
		/// Initializes a new instance of the SyncfusionHandledEventArgs class with the Handled property set to the given value.
		/// </summary>
		public SyncfusionHandledEventArgs(bool handled) 
		{
			this.handled = handled;
		}

		/// <summary>
		/// Indicates whether the event has been handled and no further processing of the event should happen.
		/// </summary>
		[TraceProperty(true)]
		public bool Handled
		{
			get
			{
				return handled;
			}
			set
			{
				handled = value;
			}
		}
	}
}
