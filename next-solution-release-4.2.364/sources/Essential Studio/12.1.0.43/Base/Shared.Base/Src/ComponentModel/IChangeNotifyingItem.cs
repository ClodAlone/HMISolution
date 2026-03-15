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
using System.Collections;
using System.ComponentModel;

namespace Syncfusion.ComponentModel
{
	/// <summary>
	/// Specifies the expected effect of the change in property of an object / Control.
	/// </summary>
	/// <remarks>
	/// <para>Used by the <see cref="Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs"/> class.</para>
	/// </remarks>
	public enum PropertyChangeEffect 
	{
		/// <summary>
		/// The Control needs a repaint due to change in property's value.
		/// </summary>
		NeedRepaint, 
		/// <summary>
		/// The Control needs to be laid out due to change in a property's value.
		/// </summary>
		NeedLayout, 
		/// <summary>
		/// No effect when there is change in a property's value.
		/// </summary>
		None
	};

	/// <summary>
	/// Provides data for the <see cref="SyncfusionPropertyChangedEventHandler"/> delegate.
	/// </summary>
	public class SyncfusionPropertyChangedEventArgs : PropertyChangedEventArgs
	{
		private PropertyChangeEffect propertyChangeType;
		/// <summary>
		/// Gets / sets the <see cref="PropertyChangeEffect"/> of this change in property value.
		/// </summary>
		/// <value>The <see cref="PropertyChangeEffect"/>.</value>
		public PropertyChangeEffect PropertyChangeEffect
		{
			get{return propertyChangeType;}
			set{propertyChangeType = value;}
		}

		private object oldValue;
		/// <summary>
		/// The old value of the property before it changes.
		/// </summary>
		/// <remarks>
		/// The object representing the old value. This can be cast to
		/// the type of the property.
		/// </remarks>
		public object OldValue
		{
			get{return oldValue;}
			set{oldValue = value;}
		}

		private object newValue;
		/// <summary>
		/// The new value of the property after it changes.
		/// </summary>
		/// <remarks>
		/// The object representing the new value. This can be cast to
		/// the type of the property.
		/// </remarks>
		public object NewValue
		{
			get{return newValue;}
			set{newValue = value;}
		}

		/// <summary>
		/// Creates an instance of the SyncfusionPropertyChangedEventArgs class.
		/// </summary>
		/// <param name="propertyChangeType">A PropertyChangeEffect value.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="oldValue">The old value cast into an object.</param>
		/// <param name="newValue">The new value cast into an object.</param>
		public SyncfusionPropertyChangedEventArgs(PropertyChangeEffect propertyChangeType,
			string propertyName, object oldValue, object newValue)
			:base(propertyName)
		{
			this.propertyChangeType = propertyChangeType;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}

	/// <summary>
	/// Represents the method that will handle the PropertyChanged event of
	/// certain classes.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="SyncfusionPropertyChangedEventArgs"/> object that 
	/// contains the event data.</param>
	public delegate void SyncfusionPropertyChangedEventHandler(object sender, SyncfusionPropertyChangedEventArgs e);

	/// <summary>
	/// A class implements this interface to let it be known that it provides
	/// a <see cref="PropertyChanged"/> event.
	/// </summary>
	/// <remarks>
	/// The <see cref="Syncfusion.Collections.ArrayListExt"/> class makes use of this interface when the items
	/// in its list implement it. The <see cref="Syncfusion.Collections.ArrayListExt"/> listens to this event and
	/// forwards the event args using its own <see cref="E:Syncfusion.Collections.ArrayListExt.ItemPropertyChanged"/> event.
	/// </remarks>
	public interface IChangeNotifyingItem
	{
		/// <summary>
		/// Occurs when one of the object's property changes.
		/// </summary>
		/// <remarks>
		/// This event provides a generic way of notifying changes
		/// in an object's property, along with the old value, new value
		/// and the PropertyChangeEffect.
		/// </remarks>
		event SyncfusionPropertyChangedEventHandler PropertyChanged;
	}
	/// <summary>
	/// Used internally to expose the DesignMode property of certain component-derived
	/// classes.
	/// </summary>
	public interface IDesignable
	{
		/// <summary>
		/// Indicates whether the component is in design-mode.
		/// </summary>
		bool DesignMode {get;}
	}
}
