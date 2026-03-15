#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Input;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
        // Summary:
        //     Selects a member from a list of candidates, and performs type conversion
        //     from actual argument type to formal argument type.
    internal class Binder
    {
        // Summary:
        //     Initializes a new instance of the System.Reflection.Binder class.
        protected Binder()
        {
        }
    }
    internal enum BindingFlags
    {
        // Summary:
        //     No binding flag.
        Default = 0,
        //
        // Summary:
        //     The case of the member name should not be considered when binding.
        IgnoreCase = 1,
        //
        // Summary:
        //     Only members declared at the level of the supplied type's hierarchy should
        //     be considered. Inherited members are not considered.
        DeclaredOnly = 2,
        //
        // Summary:
        //     Instance members should be included in the search.
        Instance = 4,
        //
        // Summary:
        //     Static members should be included in the search.
        Static = 8,
        //
        // Summary:
        //     Public members should be included in the search.
        Public = 16,
        //
        // Summary:
        //     Non-public members should be included in the search.
        NonPublic = 32,
        //
        // Summary:
        //     Public and protected static members up the hierarchy should be returned.
        //     Private static members in inherited classes are not returned. Static members
        //     include fields, methods, events, and properties. Nested types are not returned.
        FlattenHierarchy = 64,
        //
        // Summary:
        //     A method is to be invoked. This must not be a constructor or a type initializer.
        InvokeMethod = 256,
        //
        // Summary:
        //     Reflection should create an instance of the specified type. This flag calls
        //     the constructor that matches the given arguments. The supplied member name
        //     is ignored. If the type of lookup is not specified, (Instance | Public) will
        //     apply. It is not possible to call a type initializer.
        CreateInstance = 512,
        //
        // Summary:
        //     The value of the specified field should be returned.
        GetField = 1024,
        //
        // Summary:
        //     The value of the specified field should be set.
        SetField = 2048,
        //
        // Summary:
        //     The value of the specified property should be returned.
        GetProperty = 4096,
        //
        // Summary:
        //     The value of the specified property should be set. For COM properties, specifying
        //     this binding flag is equivalent to specifying PutDispProperty and PutRefDispProperty.
        SetProperty = 8192,
        //
        // Summary:
        //     The PROPPUT member on a COM object should be invoked. PROPPUT specifies a
        //     property-setting function that uses a value. Use PutDispProperty if a property
        //     has both PROPPUT and PROPPUTREF and you need to distinguish which one is
        //     called.
        PutDispProperty = 16384,
        //
        // Summary:
        //     The PROPPUTREF member on a COM object should be invoked. PROPPUTREF specifies
        //     a property-setting function that uses a reference instead of a value. Use
        //     PutRefDispProperty if a property has both PROPPUT and PROPPUTREF and you
        //     need to distinguish which one is called.
        PutRefDispProperty = 32768,
        //
        // Summary:
        //     Types of the supplied arguments must exactly match the types of the corresponding
        //     formal parameters. Reflection throws an exception if the caller supplies
        //     a non-null Binder object, because that implies that the caller is supplying
        //     BindToXXX implementations that will pick the appropriate method. The default
        //     binder ignores this flag, whereas custom binders can implement the semantics
        //     of this flag.
        ExactBinding = 65536,
        //
        // Summary:
        //     Not implemented.
        SuppressChangeType = 131072,
        //
        // Summary:
        //     The set of members whose parameter count matches the number of supplied arguments
        //     should be returned. This binding flag is used for methods with parameters
        //     that have default values and methods with variable arguments (varargs). This
        //     flag should only be used with the System.Type.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[],System.Reflection.ParameterModifier[],System.Globalization.CultureInfo,System.String[])
        //     method. Parameters with default values are used only in calls where trailing
        //     arguments are omitted. They must be the last arguments.
        OptionalParamBinding = 262144,
        //
        // Summary:
        //     Used in COM interop to specify that the return value of the member can be
        //     ignored.
        IgnoreReturn = 16777216,
    }
    internal static class StreamExtension
    {
        internal static FieldInfo[] GetFields(this Type type, BindingFlags flags)
        {
            bool isStatic = !((flags & BindingFlags.Instance) != 0);
            bool isNonPublic = (flags & BindingFlags.NonPublic) != 0;
            FieldInfo[] fields = type.GetTypeInfo().DeclaredFields.ToArray<FieldInfo>();
            List<FieldInfo> resultantFeilds = new List<FieldInfo>();
            foreach (FieldInfo field in fields)
            {
                if (field.IsStatic == isStatic && field.IsPrivate == isNonPublic && field.IsPublic == !isNonPublic)
                    resultantFeilds.Add(field);
            }
            return resultantFeilds.ToArray<FieldInfo>();
        }

        internal static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.ToArray<PropertyInfo>();
        }
        internal static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            PropertyInfo[] properties = type.GetTypeInfo().DeclaredProperties.ToArray<PropertyInfo>();
            foreach (PropertyInfo property in properties)
                if (property.Name == propertyName)
                    return property;
            return null;
        }
        internal static MethodInfo GetMethod(this Type type, string methodName)
        {
            MethodInfo[] methods = type.GetTypeInfo().DeclaredMethods.ToArray<MethodInfo>();
            foreach (MethodInfo method in methods)
                if (method.Name == methodName)
                    return method;
            return null;
        }
        internal static Object InvokeMember(this Type type, String name, BindingFlags invokeAttr, Binder binder, Object target, Object[] args)
        {
            return null;
        } 
        internal static T[] ToArray<T>(this IEnumerable<T> enumObject)
        {
            IEnumerator<T> enumeratorObject = enumObject.GetEnumerator();
            IList<T> listObjects = new List<T>();
            while (enumeratorObject.MoveNext())
                listObjects.Add(enumeratorObject.Current);
            T[] arrValues = new T[listObjects.Count];
            listObjects.CopyTo(arrValues, 0);
            return arrValues;
        }
    }
    internal static class DrawingExtensions
    {
        private const int SizeOfArgb = 4;
        private const float StepFactor = 2f;

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        /// <param name="color">The color.</param>
        internal static void FillRectangle(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            bmp.FillRectangle(x1, y1, x2, y2, col);
        }

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        /// <param name="color">The color.</param>
        internal static void FillRectangle(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] pixels = bmp.GetBitmapContext().Pixels;

            // Check boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 >= w) { x1 = w - 1; }
            if (y1 >= h) { y1 = h - 1; }
            if (x2 >= w) { x2 = w - 1; }
            if (y2 >= h) { y2 = h - 1; }


            // Fill first line
            int startY = y1 * w;
            int startYPlusX1 = startY + x1;
            int endOffset = startY + x2;
            for (int x = startYPlusX1; x <= endOffset; x++)
            {
                pixels[x] = color;
            }

            // Copy first line
            int len = (x2 - x1) * SizeOfArgb;
            int srcOffsetBytes = startYPlusX1 * SizeOfArgb;
            int offset2 = y2 * w + x1;
            for (int y = startYPlusX1 + w; y < offset2; y += w)
            {
                System.Buffer.BlockCopy(pixels, srcOffsetBytes, pixels, y * SizeOfArgb, len);
            }
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        internal static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            bmp.DrawLine(x1, y1, x2, y2, col);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        internal static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color)
        {
            DrawLine(bmp.GetBitmapContext().Pixels, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA. 
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="pixels">An array containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        internal static void DrawLine(int[] pixels, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color)
        {
            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;
            int len = pixels.Length;

            const int PRECISION_SHIFT = 8;
            const int PRECISION_VALUE = 1 << PRECISION_SHIFT;

            // Determine slope (absoulte value)
            int lenX, lenY;
            int incy1;
            if (dy >= 0)
            {
                incy1 = PRECISION_VALUE;
                lenY = dy;
            }
            else
            {
                incy1 = -PRECISION_VALUE;
                lenY = -dy;
            }

            int incx1;
            if (dx >= 0)
            {
                incx1 = 1;
                lenX = dx;
            }
            else
            {
                incx1 = -1;
                lenX = -dx;
            }

            if (lenX > lenY)
            { // x increases by +/- 1
                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / lenX;
                int y = y1 << PRECISION_SHIFT;

                // Walk the line!
                for (int i = 0; i < lenX; i++)
                {
                    // Check boundaries
                    y1 = y >> PRECISION_SHIFT;
                    if (x1 >= 0 && x1 < pixelWidth && y1 >= 0 && y1 < pixelHeight)
                    {
                        var i2 = y1 * pixelWidth + x1;
                        pixels[i2] = color;
                    }
                    x1 += incx1;
                    y += incy;
                }
            }
            else
            {
                // Prevent divison by zero
                if (lenY == 0)
                {
                    return;
                }

                // Init steps and start
                // since y increases by +/-1, we can safely add (*h) before the for() loop, since there is no fractional value for y
                int incx = (dx << PRECISION_SHIFT) / lenY;
                int x = x1 << PRECISION_SHIFT;
                int y = y1 << PRECISION_SHIFT;
                int index = (x1 + y1 * pixelWidth) << PRECISION_SHIFT;

                // Walk the line!
                var inc = incy1 * pixelWidth + incx;
                for (int i = 0; i < lenY; i++)
                {
                    x1 = x >> PRECISION_SHIFT;
                    y1 = y >> PRECISION_SHIFT;
                    if (x1 >= 0 && x1 < pixelWidth && y1 >= 0 && y1 < pixelHeight)
                    {
                        pixels[index >> PRECISION_SHIFT] = color;
                    }
                    x += incx;
                    y += incy1;
                    index += inc;
                }
            }
        }
        /// <summary>
        /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
        /// </summary>
        /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
        /// <param name="bmp"></param>
        /// <returns></returns>
        internal static BitmapContext GetBitmapContext(this WriteableBitmap bmp)
        {
            return new BitmapContext(bmp);
        }
    }

    /// <summary>
    /// Read Write Mode for the BitmapContext.
    /// </summary>
    internal enum ReadWriteMode
    {
        /// <summary>
        /// On Dispose of a BitmapContext, do not Invalidate
        /// </summary>
        ReadOnly,

        /// <summary>
        /// On Dispose of a BitmapContext, invalidate the bitmap
        /// </summary>
        ReadWrite
    }
    internal struct BitmapContext : IDisposable
    {
        private readonly WriteableBitmap writeableBitmap;
        private readonly ReadWriteMode mode;
        private readonly static IDictionary<WriteableBitmap, int> UpdateCountByBmp = new Dictionary<WriteableBitmap, int>();
        private readonly static IDictionary<WriteableBitmap, int[]> PixelCacheByBmp = new Dictionary<WriteableBitmap, int[]>();
        private int length;
        private int[] pixels;
        /// <summary>
      /// The Bitmap
      /// </summary>
      public WriteableBitmap WriteableBitmap { get { return writeableBitmap; } }

      /// <summary>
      /// Width of the bitmap
      /// </summary>
      public int Width { get { return writeableBitmap.PixelWidth; } }

      /// <summary>
      /// Height of the bitmap
      /// </summary>
      public int Height { get { return writeableBitmap.PixelHeight; } }

      /// <summary>
      /// Creates an instance of a BitmapContext, with default mode = ReadWrite
      /// </summary>
      /// <param name="writeableBitmap"></param>
      public BitmapContext(WriteableBitmap writeableBitmap)
         : this(writeableBitmap, ReadWriteMode.ReadWrite)
      {
      }

      /// <summary>
      /// Creates an instance of a BitmapContext, with specified ReadWriteMode
      /// </summary>
      /// <param name="writeableBitmap"></param>
      /// <param name="mode"></param>
      public BitmapContext(WriteableBitmap writeableBitmap, ReadWriteMode mode)
      {
          this.writeableBitmap = writeableBitmap;
          this.mode = mode;
          // Ensure the bitmap is in the dictionary of mapped Instances
          if (!UpdateCountByBmp.ContainsKey(writeableBitmap))
          {
              // Set UpdateCount to 1 for this bitmap 
              UpdateCountByBmp.Add(writeableBitmap, 1);
              length = writeableBitmap.PixelWidth * writeableBitmap.PixelHeight;
              pixels = new int[length];
              CopyPixels();
              PixelCacheByBmp.Add(writeableBitmap, pixels);
          }
          else
          {
              // For previously contextualized bitmaps increment the update count
              IncrementRefCount(writeableBitmap);
              pixels = PixelCacheByBmp[writeableBitmap];
              length = pixels.Length;
          }
      }
      private void CopyPixels()
      {
          var data = writeableBitmap.PixelBuffer.ToArray();
          for (var i = 0; i < length; i++)
          {
              pixels[i] = (data[i * 4 + 3] << 24) | (data[i * 4 + 2] << 16) | (data[i * 4 + 1] << 8) | data[i * 4 + 0];
          }
      }
      /// <summary>
      /// Gets the Pixels array 
      /// </summary>        
      public int[] Pixels { get { return pixels; } }

      /// <summary>
      /// Gets the length of the Pixels array 
      /// </summary>
      public int Length { get { return length; } }

      ///// <summary>
      ///// Performs a Copy operation from source BitmapContext to destination BitmapContext
      ///// </summary>
      ///// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
      //public static void BlockCopy(BitmapContext src, int srcOffset, BitmapContext dest, int destOffset, int count)
      //{
      //    Buffer.BlockCopy(src.Pixels, srcOffset, dest.Pixels, destOffset, count);
      //}

      ///// <summary>
      ///// Performs a Copy operation from source Array to destination BitmapContext
      ///// </summary>
      ///// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
      //public static void BlockCopy(Array src, int srcOffset, BitmapContext dest, int destOffset, int count)
      //{
      //    Buffer.BlockCopy(src, srcOffset, dest.Pixels, destOffset, count);
      //}

      ///// <summary>
      ///// Performs a Copy operation from source BitmapContext to destination Array
      ///// </summary>
      ///// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
      //public static void BlockCopy(BitmapContext src, int srcOffset, Array dest, int destOffset, int count)
      //{
      //    Buffer.BlockCopy(src.Pixels, srcOffset, dest, destOffset, count);
      //}

      /// <summary>
      /// Clears the BitmapContext, filling the underlying bitmap with zeros
      /// </summary>
      public void Clear()
      {
          var pixels = Pixels;
          Array.Clear(pixels, 0, pixels.Length);
      }

      /// <summary>
      /// Disposes this instance if the underlying platform needs that.
      /// </summary>
      public void Dispose()
      {
          // Decrement the update count. If it hits zero
          if (DecrementRefCount(writeableBitmap) == 0)
          {
              // Remove this bitmap from the update map 
              UpdateCountByBmp.Remove(writeableBitmap);
              PixelCacheByBmp.Remove(writeableBitmap);

              // Copy data back
              if (mode == ReadWriteMode.ReadWrite)
              {
                  using (var stream = writeableBitmap.PixelBuffer.AsStream())
                  {
                      var buffer = new byte[length * 4];
                      var b = 0;
                      for (var i = 0; i < length; i++, b += 4)
                      {
                          var p = pixels[i];
                          buffer[b + 3] = (byte)((p >> 24) & 0xff);
                          buffer[b + 2] = (byte)((p >> 16) & 0xff);
                          buffer[b + 1] = (byte)((p >> 8) & 0xff);
                          buffer[b + 0] = (byte)(p & 0xff);
                      }
                      stream.Write(buffer, 0, length * 4);
                  }
                  writeableBitmap.Invalidate();
              }
          }
      }
      private static void IncrementRefCount(WriteableBitmap target)
      {
          UpdateCountByBmp[target]++;
      }

      private static int DecrementRefCount(WriteableBitmap target)
      {
          int current = UpdateCountByBmp[target];
          current--;
          UpdateCountByBmp[target] = current;
          return current;
      }
    }
}
