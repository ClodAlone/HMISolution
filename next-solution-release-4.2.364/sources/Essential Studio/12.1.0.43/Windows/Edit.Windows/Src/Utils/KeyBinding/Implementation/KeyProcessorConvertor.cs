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
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Syncfusion.Windows.Forms.Edit;

namespace Syncfusion.Shared.Utils.KeyBinding.Implementation
{
  /// <summary>
  /// Manages conversions of the KeyProcessor class.
  /// </summary>
  public class KeyProcessorConverter
    : TypeConverter
  {
    /// <summary>
    /// Creates and initializes class instance.
    /// </summary>
    public KeyProcessorConverter()
      : base(){}
    /// <summary>
    /// Returns whether this converter can convert an object of one type to the type of this converter.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="sourceType">A Type that represents the type you want to convert from.</param>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
    {
      if( sourceType == typeof( byte[] ) ) return true;
      
      return base.CanConvertFrom( context, sourceType );
    }
    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="destinationType">A Type that represents the type you want to convert to.</param>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
    {
      if( destinationType == typeof( byte[] ) ) return true;
      
      return base.CanConvertTo( context, destinationType );
    }
    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="culture">The CultureInfo to use as the current culture.</param>
    /// <param name="value">The Object to convert.</param>
    /// <returns>An Object that represents the converted value.</returns>
    public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
    {
      if( value is byte[] )
      {
        MemoryStream ms = new MemoryStream( (byte[])value );
        return new KeyProcessor( ms );
      } 
      
      return base.ConvertFrom( context, culture, value );
    }
    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="culture">A CultureInfo object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
    /// <param name="value">The Object to convert.</param>
    /// <param name="destinationType">The Type to convert the value parameter to.</param>
    /// <returns>An Object that represents the converted value.</returns>
    public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
    {
      if( destinationType == typeof( byte[] ) )
      {
        if( value != null )
        {
          MemoryStream stream = new MemoryStream();
          ( (KeyProcessor)value ).SaveBindingsToXML( stream );                

          return stream.ToArray();
        }
        
        return new byte[0];
      }

      return base.ConvertTo( context, culture, value, destinationType );
    }           
  }
}