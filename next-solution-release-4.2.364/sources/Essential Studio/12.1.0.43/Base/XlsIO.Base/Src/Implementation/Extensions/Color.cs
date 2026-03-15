#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

#if WINDOWS_UWP
using Windows.UI;
using System.Threading;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
#endif

#if !WINDOWS_UWP
#if  (SILVERLIGHT)
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
#elif WP
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Threading;
#else
using System.Drawing;
using System.IO;
using System.Globalization;
#endif
#else
using System.IO;
using System.Globalization;
using Windows.UI;
#endif

namespace Syncfusion.XlsIO.Implementation
{
#if WINDOWS_UWP
     
    public static class StreamExtension
    {
        public static void Close(this Stream stream)
        {
            stream.Dispose();
        }
        public static byte[] GetBuffer(this MemoryStream stream)
        {
            return stream.ToArray();
        }
    }
    public enum BindingFlags
    {
        NonPublic = 1,
        Instance = 2,

    }
    public static class TypeExtension
    {
  /// <summary>
        /// Searches for a public instance constructor whose parameters match the types in the specified array.
        /// </summary>
        /// <param name="type">Represents the property type.</param>
        /// <param name="types">An array of Type objects representing the number, order, and type of the parameters for the desired constructor.</param>
        /// <returns>An object representing the public instance constructor whose parameters match the types in the parameter type array, if found; otherwise, null.</returns>
        public static ConstructorInfo GetConstructor(this Type type,Type[] types)
        {
            bool isMatch = true;
            IEnumerator<ConstructorInfo> declaredConstructors = type.GetTypeInfo().DeclaredConstructors.GetEnumerator();
            while (declaredConstructors.MoveNext())
            {
                isMatch = true;
                ParameterInfo[] paramsInfo= declaredConstructors.Current.GetParameters();
                for(int i=0;i<types.Length && i<paramsInfo.Length;i++)
                {
                    if (!paramsInfo[i].Equals(types[i]))
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    return declaredConstructors.Current;
            }
            return null;
        }
       
        /// <summary>
        /// Retrieves the collection of custome attribute of the specified type that are applied 
        /// to a specific member, and optionally inspect the ancestors of that member.
        /// </summary>
        /// <param name="type">Type of the Property</param>
        /// <param name="attributeType">Type of the attribute to search for.</param>
        /// <param name="inherit">True to inspect the ancestor of that member otherwise false.</param>
        /// <returns></returns>
        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            IEnumerable<Attribute> attributes = type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
            return (object[])attributes.ToArray<Attribute>();
        }
        public static FieldInfo[] GetFields(this Type type,BindingFlags flags)
        {
            bool isStatic = !((flags & BindingFlags.Instance) != 0);
            bool isNonPublic=(flags& BindingFlags.NonPublic)!=0;
            FieldInfo[] fields = type.GetTypeInfo().DeclaredFields.ToArray<FieldInfo>();
            List<FieldInfo> resultantFeilds=new List<FieldInfo>();
            foreach (FieldInfo field in fields)
            {
                if(field.IsStatic==isStatic && field.IsPrivate==isNonPublic && field.IsPublic==!isNonPublic)
                 resultantFeilds.Add(field);
            }
            
            return resultantFeilds.ToArray<FieldInfo>();
        }
        public static bool IsSubclassOf(this Type type, Type parentType)
        {
            return type.GetTypeInfo().IsSubclassOf(parentType);
        }
        public static Type GetInterface(this Type type, string interfaceName, bool ignoreCase)
        {
            IEnumerator<Type> interfaces= type.GetTypeInfo().ImplementedInterfaces.GetEnumerator();
            while(interfaces.MoveNext())
            {
                Type interfaceType = interfaces.Current;
                if (interfaceType.Name.Equals(interfaceName, (ignoreCase) ? StringComparison.CurrentCultureIgnoreCase :
                    StringComparison.CurrentCulture))
                {
                    return interfaceType;
                }
            }
            return null;
        }
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.ToArray<PropertyInfo>();
        }
        public static PropertyInfo GetProperty(this Type type,string propertyName)
        {
            PropertyInfo[] properties= type.GetTypeInfo().DeclaredProperties.ToArray<PropertyInfo>();
            foreach (PropertyInfo property in properties)
                if (property.Name == propertyName)
                    return property;
            return null;
        }
        public static object[] GetCustomAttributes(this PropertyInfo property, bool inherit)
        {
            return property.GetCustomAttributes(inherit).ToArray<object>();
        }
    }


    public static class HelperMethods
    {
        public static string ToUpper(this string strValue, CultureInfo culture)
        {
            if (CultureInfo.InvariantCulture.Equals(culture))
                return strValue.ToUpperInvariant();
            else
                return strValue.ToUpper();
        }
        public static string ToLower(this string strValue, CultureInfo culture)
        {
            if (CultureInfo.InvariantCulture.Equals(culture))
                return strValue.ToLowerInvariant();
            else
                return strValue.ToLower();
        }
        public static T[] ToArray<T>(this IEnumerable<T> enumObject)
        {
            IEnumerator<T> enumeratorObject= enumObject.GetEnumerator();
            IList<T> listObjects = new List<T>();
            while (enumeratorObject.MoveNext())
                listObjects.Add(enumeratorObject.Current);
            T[] arrValues = new T[listObjects.Count];
            listObjects.CopyTo(arrValues,0);
            return arrValues;
        }
        
        
    }
#endif
    public static class DateTimeExtension
    {
#if WINDOWS_UWP
        /// <summary>
        /// Returns a DateTime equivalent to the specified OLE Automation Date.
        /// </summary>
        /// <param name="doubleOLEValue">An OLE Automation Date value.</param>
        /// <returns>An object that represents the same date and time as d.</returns>
        public static DateTime FromOADate(double doubleOLEValue)
        {
            if (doubleOLEValue < -657435.0 || doubleOLEValue > 2958465.99999999)
                throw new ArgumentException("Not an valid OLE value.");

            double doubleSecondsPerDay = 24 * 60 * 60;
            string[] strSplitedValue = doubleOLEValue.ToString().Split('.');
            int integralPart = Convert.ToInt32(strSplitedValue[0]);
            double mantisaPart = Convert.ToDouble("." + strSplitedValue[1]);
            int totalSeconds = (int)(mantisaPart * doubleSecondsPerDay);
            DateTime MinOLEDate = DateTime.Parse("1899-12-30 12:0:0 AM");
            DateTime oleDateFromValue = MinOLEDate.AddDays(integralPart);
            oleDateFromValue = oleDateFromValue.AddSeconds(totalSeconds);
            return oleDateFromValue;
        }

        /// <summary>
        /// Converts the value of this instance to the equivalent OLE Automation date.
        /// </summary>
        /// <param name="inDateTime"></param>
        /// <returns>A double-precision floating-point number that contains an OLE Automation date equivalent to the value of this instance.</returns>
        public static double ToOADate(this DateTime inDateTime)
        {
            DateTime MinOLEDate = DateTime.Parse("1899-12-30 12:0:0 AM");
            DateTime MaxOLEDate = DateTime.Parse("9999-12-31 12:0:0 AM");

            if (inDateTime < MinOLEDate || inDateTime > MaxOLEDate)
                throw new ArgumentException("Not an Valid OLE Date.");

            double doubleSecondsPerDay = 24 * 60 * 60;


            TimeSpan dateDiff = inDateTime - MinOLEDate;
            double doubleOLEValue = (double)dateDiff.Days;
            doubleOLEValue += (double)dateDiff.TotalSeconds / doubleSecondsPerDay;

            return doubleOLEValue;

        }

#endif
    }
  /// <summary>
  /// This class contains some extension to the Color class in  (SILVERLIGHT).
  /// </summary>
  public static class ColorExtension
  {
#if  (SILVERLIGHT || WINDOWS_UWP || WP)
    public static int ToArgb(this Color color)
    {
      return ( color.A << 24 ) + ( color.R << 16 ) + ( color.G << 8 ) + color.B;
    }
#endif
    /// <summary>
    /// Contains black color.
    /// </summary>
    public static Color Black = Color.FromArgb( 255, 0, 0, 0 );
    /// <summary>
    /// Contains white color.
    /// </summary>
    public static Color White = Color.FromArgb( 255, 255, 255, 255 );
    /// <summary>
    /// Contains empty color.
    /// </summary>
    public static Color Empty = Color.FromArgb( 0, 0, 0, 0 );
    /// <summary>
    /// Contains red color.
    /// </summary>
    public static Color Red = Color.FromArgb( 255, 255, 0, 0 );
    /// <summary>
    /// Contains blue color.
    /// </summary>
    public static Color Blue = Color.FromArgb( 255, 0, 0, 255 );
    /// <summary>
    /// Contains dark gray color.
    /// </summary>
    public static Color DarkGray = Color.FromArgb( 255, 0x80, 0x80, 0x80 );
    /// <summary>
    /// Contains yellow color.
    /// </summary>
    public static Color Yellow = Color.FromArgb( 255, 255, 255, 0 );
    /// <summary>
    /// Contains cyan color.
    /// </summary>
    public static Color Cyan = Color.FromArgb( 255, 0, 255, 255 );
    /// <summary>
    /// Contains magenta color.
    /// </summary>
    public static Color Magenta = Color.FromArgb( 255, 255, 0, 255 );
    /// <summary>
    /// Contains gray color.
    /// </summary>
    public static Color Gray = Color.FromArgb( 255, 192, 192, 192 );
    /// <summary>
    /// Chart foreground color.
    /// </summary>
    public static Color ChartForeground = 
#if  SILVERLIGHT || WINDOWS_UWP || WP
      White;
#else
      SystemColors.WindowText;
#endif
    /// <summary>
    /// Chart background color.
    /// </summary>
    public static Color ChartBackground =
#if  SILVERLIGHT || WINDOWS_UWP || WP
      Black;
#else
      SystemColors.Window;
#endif
    /// <summary>
    /// Chart neutral color.
    /// </summary>
    public static Color ChartNeutral = Black;
    /// <summary>
    /// Converts Int32 value into Color.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted value.</returns>
    public static Color FromArgb( int value )
    {
#if  (SILVERLIGHT || WINDOWS_UWP || WP)
      byte b = ( byte )value;
      value = value >> 8;

      byte g = ( byte )value;
      value = value >> 8;

      byte r = ( byte )value;
      value = value >> 8;

      byte a = ( byte )value;

      return Color.FromArgb( a, r, g, b );
#else
      return Color.FromArgb( value );
#endif
    }
    public static Color FromName( string name )
    {
#if WINDOWS_UWP
            return Colors.Black;
#elif (SILVERLIGHT || WP)
      if( m_dictSystemColors.Count == 0 )
        InitSystemColors();

      return m_dictSystemColors[ name.ToLower() ];
#else
            return Color.FromName( name );
#endif
    }

#if  (SILVERLIGHT || WP)
    private static Dictionary<string, Color> m_dictSystemColors = new Dictionary<string, Color>();
    internal static bool IsBackgroundThread
    {
      get
      {
        bool result = false;

        try
        {
          System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock();
          textBlock = null;
        }
        catch
        {
          result = true;
        }

        return result;
      }
    }
    public static void InitSystemColors()
    {
      //GetColorDelegate method = delegate { return SystemColors.ActiveBorderColor; };
      bool multiThreading = IsBackgroundThread;

      if( IsBackgroundThread )
      {
        ManualResetEvent threadCompleteEvent = new ManualResetEvent( false );
        DispatcherOperation operation = System.Windows.Deployment.Current.Dispatcher.BeginInvoke(
          delegate
          {
            AddColors();
            threadCompleteEvent.Set();
          } );

        threadCompleteEvent.WaitOne();
        threadCompleteEvent.Close();
      }
      else
      {
        AddColors();
      }
    }
    private static void AddColors()
    {
      m_dictSystemColors.Add( "activeborder", SystemColors.ActiveBorderColor );
      m_dictSystemColors.Add( "activecaption", SystemColors.ActiveCaptionColor );
      m_dictSystemColors.Add( "activecaptiontext", SystemColors.ActiveCaptionTextColor );
      m_dictSystemColors.Add( "appworkspace", SystemColors.AppWorkspaceColor );
      m_dictSystemColors.Add( "control", SystemColors.ControlColor );
      m_dictSystemColors.Add( "controldark", SystemColors.ControlDarkColor );
      m_dictSystemColors.Add( "controldarkdark", SystemColors.ControlDarkDarkColor );
      m_dictSystemColors.Add( "controllight", SystemColors.ControlLightColor );
      m_dictSystemColors.Add( "controllightlight", SystemColors.ControlLightLightColor );
      m_dictSystemColors.Add( "controltext", SystemColors.ControlTextColor );
      m_dictSystemColors.Add( "desktop", SystemColors.DesktopColor );
      m_dictSystemColors.Add( "graytext", SystemColors.GrayTextColor );
      m_dictSystemColors.Add( "highlight", SystemColors.HighlightColor );
      m_dictSystemColors.Add( "highlighttext", SystemColors.HighlightTextColor );
      m_dictSystemColors.Add( "inactiveborder", SystemColors.InactiveBorderColor );
      m_dictSystemColors.Add( "inactivecaption", SystemColors.InactiveCaptionColor );
      m_dictSystemColors.Add( "inactivecaptiontext", SystemColors.InactiveCaptionTextColor );
      m_dictSystemColors.Add( "info", SystemColors.InfoColor );
      m_dictSystemColors.Add( "infotext", SystemColors.InfoTextColor );
      m_dictSystemColors.Add( "menu", SystemColors.MenuColor );
      m_dictSystemColors.Add( "menutext", SystemColors.MenuTextColor );
      m_dictSystemColors.Add( "scrollbar", SystemColors.ScrollBarColor );
      m_dictSystemColors.Add( "window", SystemColors.WindowColor );
      m_dictSystemColors.Add( "windowframe", SystemColors.WindowFrameColor );
      m_dictSystemColors.Add( "windowtext", SystemColors.WindowTextColor );
    }
#endif
  }
#if WINDOWS_UWP
  public sealed class DBNull 
  {
      private static DBNull m_soleInstance;
      public static DBNull Value
      {
          get
          {
              if(m_soleInstance==null)
                m_soleInstance=new DBNull();
              return m_soleInstance;
          }
      }
  }
#endif
}
