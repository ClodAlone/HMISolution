#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Threading;

#if ( WINRT )
using Windows.UI.Xaml.Media;
namespace Syncfusion.XlsIO.Implementation.WINRT
#elif WP
using System.Windows.Threading;
using System.Windows.Controls;
namespace Syncfusion.XlsIO.Implementation.WP
#else
using System.Windows.Threading;
using System.Windows.Controls;
namespace Syncfusion.XlsIO.Implementation.Silverlight
#endif

{
  public class Font: IDisposable
  {
    public string Name;
    public bool Bold;
    public bool Italic;
    public float Size;
    public bool Underline;
    public bool Strikeout;
    public byte CharSet;
    public void Dispose()
    {
        Name = null;
    }
    internal static Font FromLogFont(LOGFONT logFont)
    {
      Font result = new Font();
      result.Name = Encoding.UTF8.GetString(logFont.lfFaceName, 0, logFont.lfFaceName.Length);
      result.Bold = logFont.lfWidth > 400;
      result.Italic = logFont.lfItalic > 0;
      result.Size = logFont.lfHeight;
      return result;
    }
#if (SILVERLIGHT) || ( WINRT ) || (WP)
    public float GetHeight()
    {
#if ( WINRT )
        Windows.Foundation.Size size = new Windows.Foundation.Size(double.MaxValue,
            double.MaxValue);
        UIDispatcher.Initialize();
        Action action=new Action(
            delegate
            {
                Windows.UI.Xaml.Controls.TextBlock text = new Windows.UI.Xaml.Controls.TextBlock();

                 text.Text = "0123456789";
                
                 text.FontFamily = new FontFamily(Name);
                 text.FontSize = Size;
                 text.FontStyle = (Italic) ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                 text.FontWeight = (Bold) ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal;
                 text.Measure(size);
                 size = text.DesiredSize;
            });
        UIDispatcher.Execute(action);
       
        return (float)size.Height;
#else
        float result=(float)0.0;

      if( ColorExtension.IsBackgroundThread )
      {
        ManualResetEvent threadCompleteEvent = new ManualResetEvent( false );
        DispatcherOperation operation = System.Windows.Deployment.Current.Dispatcher.BeginInvoke(
          delegate
          {
              result = Measure("0123456789",Name,Bold,Italic,(float)Size);
            threadCompleteEvent.Set();
          } );

        threadCompleteEvent.WaitOne();
        threadCompleteEvent.Close();
      }
      else
      {
          result = Measure("0123456789", Name, Bold, Italic,(float) Size);
      }

      return result;
#endif
    }
#endif
#if  (SILVERLIGHT || WP)
    private float Measure(string strValue, string FontName, bool Bold, bool Italic, float size)
    {
        TextBlock textBlock = new TextBlock()
        {
            Text = strValue,
            FontFamily = new System.Windows.Media.FontFamily(FontName),
            FontSize = ApplicationImpl.ConvertUnitsStatic(size * 96, MeasureUnits.Point, MeasureUnits.Inch),
            FontWeight = Bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
            FontStyle = Italic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal,
            TextWrapping = System.Windows.TextWrapping.NoWrap,
            FontStretch = System.Windows.FontStretches.Expanded,
        };
        return (float)textBlock.ActualHeight;
    }

#endif
    internal void ToLogFont( LOGFONT logFont )
    {
      logFont.lfFaceName = Encoding.UTF8.GetBytes( Name );

      if( Bold )
        logFont.lfWidth = 700;

      if( Italic )
        logFont.lfItalic = 1;

      logFont.lfHeight = ( int )Size;
    }

    internal Font()
    {
    }

    internal Font( string FontName, float size, FontStyle fontstyle, GraphicsUnit unit, byte charSet )
    {
      Name = FontName;
      CharSet = charSet;
      Size = size;

      Bold = ( ( fontstyle & FontStyle.Bold ) != 0 );
      Italic = ( ( fontstyle & FontStyle.Italic ) != 0 );
      Underline = ( ( fontstyle & FontStyle.Underline ) != 0 );
      Strikeout = ( ( fontstyle & FontStyle.Strikeout ) != 0 );
    }
  }
}
