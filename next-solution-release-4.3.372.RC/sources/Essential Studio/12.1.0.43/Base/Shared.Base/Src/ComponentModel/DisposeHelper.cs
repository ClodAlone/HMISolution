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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Windows.Forms;
#endregion

namespace Syncfusion.ComponentModel
{
	/// <summary></summary>
	public sealed class DisposeHelper
	{
		/// <summary></summary>
		/// <param name="list"></param>
		public static void Dispose( ref ImageList list )
		{
			if( list != null )
			{
				list.Images.Clear();
				list.Dispose();
				list = null;
			}
		}
		/// <summary>Clear collection and then reset it reference to NULL.</summary>
		/// <param name="collection"></param>
		public static void Dispose( ref IList collection )
		{
			if( collection != null )
			{
				collection.Clear();
				collection = null;
			}
		}
		/// <summary></summary>
		/// <param name="dictionary"></param>
		public static void Dispose( ref IDictionary dictionary )
		{
			if( dictionary != null )
			{
				dictionary.Clear();
				dictionary = null;
			}
		}
		/// <summary></summary>
		/// <param name="window"></param>
		public static void Dispose( ref NativeWindow window )
		{
			if( window != null )
			{
				try
				{
					window.ReleaseHandle();
				}
				catch( Exception ex )
				{
					Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );	
				}

				window = null;
			}
		}
		/// <summary></summary>
		/// <param name="value"></param>
		public static void Dispose( ref IDisposable value )
		{
			if( value != null )
			{
				value.Dispose();
				value = null;
			}
		}

		/// <summary></summary>
		/// <param name="component"></param>
		public static void Dispose( ref Component component )
		{
			if( component != null )
			{
				component.Dispose();
				component = null;
			}
		}
    /// <summary></summary>
    /// <param name="tooltip"></param>
    public static void Dispose( ref ToolTipAdv tooltip )
    {
      if( tooltip != null )
      {
        tooltip.Dispose();
        tooltip = null;
      }
    }
    /// <summary></summary>
    /// <param name="dragHelper"></param>
    public static void Dispose( ref DragHelper dragHelper )
    {
      if( dragHelper != null )
      {
        dragHelper.Dispose();
        dragHelper = null;
      }
    }
    /// <summary></summary>
		/// <param name="pen"/>
		public static void Dispose( ref Pen pen )
		{
			if( pen != null )
			{
				pen.Dispose();
				pen = null;
			}
		}

		/// <summary></summary>
		/// <param name="brush"/>
		public static void Dispose( ref Brush brush )
		{
			if( brush != null )
			{
				brush.Dispose();
				brush = null;
			}
		}

    /// <summary></summary>
    /// <param name="graphics"/>
    public static void Dispose( ref Graphics graphics )
    {
      if( graphics != null )
      {
        graphics.Dispose();
        graphics = null;
      }
    }

    /// <summary></summary>
		/// <param name="path"/>
		public static void Dispose( ref GraphicsPath path )
		{
			if( path != null )
			{
				path.Dispose();
				path = null;
			}
		}

		/// <summary></summary>
		/// <param name="format"/>
		public static void Dispose( ref StringFormat format )
		{
			if( format != null )
			{
				format.Dispose();
				format = null;
			}
		}
    /// <summary></summary>
    /// <param name="collection"/>
    public static void Dispose( ref ArrayListExt collection )
    {
      if( collection != null )
      {
        if( collection is IDisposable )
        {
          (( IDisposable )collection).Dispose();
        }
        
        collection.Clear();
        collection = null;
      }
    }
    /// <summary>Dispose array items and then clear collection and reset it reference to NULL.</summary>
		/// <param name="collection"></param>
		public static void DisposeItems( ref IList collection )
		{
			if( collection != null )
			{
				foreach( object item in collection )
				{
					if( item is IDisposable )
					{
						(( IDisposable )item).Dispose();
					}
				}

				collection.Clear();
				collection = null;
			}
		}
	}

	public sealed class AmbientHelper
	{
		/// <summary>
		/// Gets the value of a property from some object
		/// </summary>
		/// <param name="instance">Object from which we want to take the value of property</param>
		/// <param name="nameOfProperty">Name of the property from which we want to take value</param>
		/// <param name="retType">Type of value? which to return</param>
		/// <param name="retValue">Value which we want to take</param>
		public static bool GetAmbientValue( object instance, string nameOfProperty, Type retType, out object retValue )
		{
			retValue = null;

			if( instance != null && nameOfProperty != null && nameOfProperty.Length != 0 )
			{
				Type type = instance.GetType();
				PropertyInfo propInfo = type.GetProperty( nameOfProperty );

				if( propInfo != null )
				{
					object value;
					
					try
					{
						value = propInfo.GetValue( instance, new object[ ]{} );
					}
					catch( TargetException ex )
					{
						Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );
						return false;
					}
					catch( TargetParameterCountException ex1 )
					{
						Debug.WriteLine( ex1.Message + Environment.NewLine + ex1.StackTrace );
						return false;
					}

					// value != null ???
					if( value.GetType() == retType )
					{
						retValue = value;
						return true;
					}
				}
			}

			return false;
		}

	}
}