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
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

//using Syncfusion.Windows.Forms.HTMLUI;
//using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
using Syncfusion.HTMLUI.Base.Utility;
#endregion

namespace Syncfusion.HTMLUI.Base.Parser.HTML
{
  /// <summary>
  /// Class from HTMLParser (HTML->XHTML).
  /// </summary>
  internal class HTMLToXML
  {
    #region Internal Class
    /// <summary>
    /// Helper class for parsing HTML document.
    /// </summary>
    public class Context
    {
      #region Context Class members
      /// <summary>
      /// HTML element which is parsing.
      /// </summary>
      private HTMLElement _element;
      /// <summary>
      /// True if element is NULL.
      /// </summary>
      private bool    _xmlIsland = false;
      /// <summary>
      /// Name of the tag.
      /// </summary>
      private string  _fullElementName = "";
      #endregion

      #region Context Class Properties
      /// <summary>
      /// Returns the HTML element.
      /// </summary>
      public HTMLElement Element
      {
        get
        {
          return _element;
        }
      }

      /// <summary>
      /// Indicates whether element is NULL.
      /// </summary>
      public bool IsXmlIsland
      {
        get
        {
          return _xmlIsland;
        }
      }

      /// <summary>
      /// Returns the name of the element.
      /// </summary>
      public string FullName
      {
        get
        {
          return _fullElementName;
        }
      }
      #endregion

      #region Context Class Initialize/Finalize methods
      /// <summary>
      /// Hide default constructor from user hands.
      /// </summary>
      private Context()
      {
      }
      /// <summary>
      /// Constructor with an element instance as argument.
      /// </summary>
      /// <param name="element">Element object.</param>
      public Context( HTMLElement element )
        : this( element, null )
      {
      }
      /// <summary>
      /// Constructor with the specfied name of the element as argument.
      /// </summary>
      /// <param name="fullElementName">Name of the element.</param>
      public Context( string fullElementName )
        : this( null, fullElementName )
      {
      }
      /// <summary>
      /// Constructor with the name of the element and element instance as argument.
      /// </summary>
      /// <param name="element">Element object.</param>
      /// <param name="fullElementName">Name of the element.</param>
      public Context( HTMLElement element, string fullElementName )
      {
        _element = element;
        _fullElementName = ( element == null ) ? fullElementName : element.Name;
        _xmlIsland = ( element == null );
      }
      #endregion
    }
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether to raise event.
    /// </summary>
    private bool m_bSkipEvents;
    /// <summary>
    /// Corresponding for tokens of the document.
    /// </summary>
    private HTMLTokenizer m_parser;
    /// <summary>
    /// Writes XHTML document.
    /// </summary>
    private XmlWriter m_writer;
    /// <summary>
    /// Stack for holding open tags.
    /// </summary>
    private Stack m_contextStack = new Stack();
    /// <summary>
    /// Configurator.
    /// </summary>
    private HTMLConfig m_config;
    /// <summary>
    /// Root element of the document.
    /// </summary>
    private HTMLElement m_root;
    /// <summary>
    /// Flag indicates whether to skip bad situations or throw an exception.
    /// Default is skip.
    /// </summary>
    private bool m_skipError;
    /// <summary>
    /// Array of prefixes in the document.
    /// </summary>
    private ArrayList m_prefixes;
    #endregion

    #region Class Properties
    /// <summary>
    /// Indicates whether quiet mode is enabled.
    /// </summary>
    internal protected bool QuietMode
    {
      get
      {
        return m_bSkipEvents;
      }
      set
      {
        if( value != m_bSkipEvents )
        {
          m_bSkipEvents = value;
          OnQuietModeChanged();
        }
      }
    }
    /// <summary>
    /// Returns the element from the stack.
    /// </summary>
    public HTMLElement ContextElement
    {
      get
      {
        return ( ( Context )m_contextStack.Peek() ).Element;
      }
    }
    /// <summary>
    /// Flag indicates whether to skip bad situations or throw an exception.
    /// Default is skip.
    /// </summary>
    internal protected bool SkipError
    {
      get
      {
        return m_skipError;
      }
      set
      {
        if( m_skipError != value )
        {
          m_skipError = value;
        }
      }
    }
    /// <summary>
    /// Returns an array of prefixes that occurred in the document.
    /// </summary>
    internal ArrayList Prefixes
    {
      get
      {
        return m_prefixes;
      }
    }
    /// <summary>
    /// Returns the encoding used for data converting.
    /// </summary>
    internal Encoding Encoding
    {
      get
      {
        return m_parser.Encoding;
      }
    }
    #endregion

    #region Class Events
    /// <summary>
    /// Event that is to be raised when mode is changed.
    /// </summary>
    public event EventHandler QuietModeChanged;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Hides private constructor from end user.
    /// </summary>
    private HTMLToXML()
    {
    }
    /// <summary>
    /// Creates a class with default configuration.
    /// Class gets input stream and output writer.
    /// </summary>
    /// <param name="input">Input stream.</param>
    /// <param name="output">Output writer.</param>
    public HTMLToXML( TokenStream input, XmlWriter output )
      : this( input, output, HTMLConfig.MainConfig, null )
    {
    }
    /// <summary>
    /// Main constructor which allows the user to control all aspects of class creation.
    /// </summary>
    public HTMLToXML( TokenStream reader, XmlWriter writer,
      HTMLConfig config, HTMLElement contextElement )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( config == null )
        throw new ArgumentNullException( "config" );

      m_parser = new HTMLTokenizer( reader );
      m_writer = writer;
      m_config = config;
      m_skipError = true;

      if( contextElement != null )
      {
        m_root = contextElement;
      }
      else
      {
        m_root = new HTMLElement( "the:root" );
      }

      m_contextStack.Push( new Context( m_root ) );
    }

    #endregion

    #region Class Overrides
    /// <summary>
    /// Quiet mode property changed.
    /// </summary>
    protected virtual void OnQuietModeChanged()
    {
      RaiseQuietModeChangedEvent();
    }
    /// <summary>
    /// Raises quiet mode event.
    /// </summary>
    protected virtual void RaiseQuietModeChangedEvent()
    {
      if( QuietModeChanged != null )
      {
        QuietModeChanged( this, EventArgs.Empty );
      }
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Returns a list of elements that MUST BE opened to allow current element
    /// at curent position.
    /// </summary>
    /// <param name="probe">Element object.</param>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="probed"></param>
    /// <returns>List of elements that MUST BE opened to allow current element
    /// at curent position.</returns>
    private Stack CheckSubElements( HTMLElement probe, string elementName, IDictionary probed )
    {
      // This checks if it possible to open some elements under
      // probe to allow elementName to be correct at this position.
      if( probed == null )
      {
        probed = new SortedListEx();
      }

      probed.Add( elementName, elementName );

      foreach( HTMLElement opt in probe.Optional.Values )
      {
        HTMLElement sub = ( HTMLElement )opt.Children[ elementName ];
        Stack res = new Stack( 1 );
        res.Push( opt );
        return res;
      }

      // If it is not possible, we try to do the same with one more with deep traversal.
      foreach( HTMLElement opt in probe.Optional.Values )
      {
        if( !probed.Contains( opt.Name ) )
        {
          if( opt.IsOptionalUsed )
          {
            Stack res = CheckSubElements( opt, elementName, probed );

            if( res != null )
            {
              res.Push( opt );
              return res;
            }
          }
        }
      }
      return null;
    }
    /// <summary>
    /// Indicates whether to check tag and adjust if needed.
    /// </summary>
    /// <param name="elementName"></param>
    /// <param name="tryClose"></param>
    /// <param name="probedToOpen"></param>
    /// <param name="resElement"></param>
    /// <param name="writtenElements"></param>
    /// <returns></returns>
    private bool iCheckAndAdjust( string elementName, bool tryClose, IDictionary probedToOpen, out HTMLElement resElement, ref Stack writtenElements )
    {
      if( elementName == null )
        throw new ArgumentNullException( "elementName" );

      HTMLElement res = m_config.Elements[ elementName ] as HTMLElement;
      Context currentContext = m_contextStack.Peek() as Context;

      Debug.Assert( res != null );
      Debug.Assert( !currentContext.IsXmlIsland );
      Debug.Assert( currentContext.Element != null );

      // Start adjusting.
      while( true )
      {
        // Check if current context is ok for the given element.
        HTMLElement subElement = currentContext.Element.Children[ elementName ] as HTMLElement;

        if( subElement != null )
        { // We found an adjustment path.
          resElement = subElement;
          return true;
        }

        // Current context is invalid.
        // Trying to open new elements.
        if( currentContext.Element.IsOptionalUsed )
        {
          if( probedToOpen == null ) probedToOpen = new SortedListEx();

          foreach( HTMLElement subElementToOpen in currentContext.Element.Optional.Values )
          {
            if( !probedToOpen.Contains( subElementToOpen.Name ) )
            {
              writtenElements.Push( subElementToOpen );
              m_contextStack.Push( new Context( subElementToOpen ) );
              probedToOpen.Add( subElementToOpen.Name, subElementToOpen );

              bool attemptResult = iCheckAndAdjust( elementName, false, probedToOpen, out resElement, ref writtenElements );
              if( attemptResult )
              {
                // WrittenElements contain all elements closed and open.
                // m_contextStack is set.
                return true;
              }

              // If no way found, attemt the next after clean up.
              m_contextStack.Pop();
              writtenElements.Pop();
              probedToOpen.Remove( subElementToOpen.Name );
            }
          }
        }

        // If we cannot find any way with new element, let's try to close if anything.
        if( tryClose )
        {
          object undoContextStack;

          if( m_contextStack.Count > 1 &&
            Utilities.StrEquals( currentContext.Element.TagEnd, "o" ) )
          {
            undoContextStack = m_contextStack.Pop();
            writtenElements.Push( null );

            bool attemptResult = iCheckAndAdjust( elementName, tryClose,
              probedToOpen, out resElement, ref writtenElements );

            if( attemptResult )
            {
              // WrittenElements contain all elements closed and open.
              // m_contextStack is set.
              return true;
            }

            m_contextStack.Push( undoContextStack );
            writtenElements.Pop();
          }
        };
        break;
      }

      resElement = res;
      return false;
    }
    /// <summary>
    /// Checks if the specified elementName is allowed in the current context
    /// and adjusts it as necessary.
    /// Function returns element describing the elementName or NULL if there is no such element.
    /// </summary>
    private HTMLElement CheckAndAdjust( string elementName )
    {
      if( elementName == null )
        throw new ArgumentNullException( "elementName" );

      if( elementName.Length == 0 )
        throw new ArgumentException( "elementName - string can not be empty" );

      elementName = "html:" + elementName;
      HTMLElement res = m_config.Elements[ elementName ] as HTMLElement;

      // Unknown element. This probably means that an XML island starts here.
      if( res == null ) return res;

      Context c = ( Context )m_contextStack.Peek(); // take a current context

      // If we are in XML island, treat any element as unknown.
      if( c.IsXmlIsland ) return null;

      HTMLElement resElement;
      SortedList probedToOpen = null;
      Stack writtenElements = new Stack(); // to opt

      bool attemptResult = iCheckAndAdjust( elementName, true, probedToOpen,
        out resElement, ref writtenElements );

      if( attemptResult )
      {
        foreach( HTMLElement e in writtenElements )
        {
          if( e == null )
          {
            m_writer.WriteEndElement();
          }
          else
          {
            m_writer.WriteStartElement( e.SubName );
          }
        }
      }

      return resElement;
    }

    /// <summary>
    /// Checks and adjusts tag for validity.
    /// </summary>
    /// <param name="fullElementName"></param>
    private void CheckAndAdjustEndTag( string fullElementName )
    {
      if( fullElementName == null )
        throw new ArgumentNullException( "fullElementName" );

      if( fullElementName.Length == 0 )
        throw new ArgumentException( "fullElementName - string cannot be empty" );

      // Remove unneeded whitespaces in name for comparing.
      fullElementName = fullElementName.Trim();

      Context c;
      Stack undoContextStack = null;

      for( ;; )
      {
        c = ( Context )m_contextStack.Peek();

        if( string.Compare( c.FullName, fullElementName, true, CultureInfo.InvariantCulture ) == 0 )
        {
          // We found the way. Accept it.
          if( undoContextStack != null )
          {
            while( undoContextStack.Count > 0 )
            {
              Context elementToValidate = ( Context )undoContextStack.Pop();

              if( !elementToValidate.IsXmlIsland &&
                !Utilities.StrEquals(  elementToValidate.Element.TagEnd, "o" )
                )
              {
                Debug.WriteLine( string.Format( "***!!! Auto-closing the <{0}> element because open tag found for the <{1}> element !!!***", elementToValidate.FullName, fullElementName ) );
                //m_writer.WriteComment( string.Format( "***!!! Auto-closing the <{0}> element because open tag found for the <{1}> element !!!***", elementToValidate.FullName, fullElementName ) );
              }

              m_writer.WriteEndElement();
            }
          }

          m_writer.WriteEndElement();
          m_contextStack.Pop();
          return;
        }
        else
        {
          if( undoContextStack == null ) undoContextStack = new Stack();
          if( !c.IsXmlIsland && Utilities.StrEquals(  c.Element.TagEnd, "o" ) )
          {
            undoContextStack.Push( m_contextStack.Pop() );
          }
          else if( m_contextStack.Count > 1 )
          {
            undoContextStack.Push( m_contextStack.Pop() ); // do the same - HANDLE if necessary
          }
          else
          {
            break;
          }
        }
      } // for

      // IGNORE the element's end !!!
      if( undoContextStack != null )
      {
        while( undoContextStack.Count > 0 )
        {
          m_contextStack.Push( undoContextStack.Pop() );
        }
      }
      Debug.WriteLine( string.Format( "***!!! Ending tag for element <{0}> is ignored !!!***", fullElementName ) );
      //m_writer.WriteComment( string.Format( "***!!! Ending tag for element <{0}> is ignored !!!***", fullElementName ) );
    }

    /// <summary>
    ///  Recognizes token and invokes the corresponding method.
    /// </summary>
    /// <param name="tok"></param>
    /// <param name="attributeName"></param>
    /// <param name="writtenAttributes"></param>
    /// <param name="inStartTag"></param>
    private void RecognizeToken( ParserToken tok, ref string attributeName, StringCollection writtenAttributes, ref bool inStartTag )
    {
      switch( tok )
      {
        case ParserToken.ProcessingInstruction:
        case ParserToken.None:
          break;

        case ParserToken.MarkedSection:
          m_writer.WriteElementString( "marked-section", m_parser.TokenString );
          break;

        case ParserToken.StartTag:
        {
          writtenAttributes.Clear();
          HTMLElement el = CheckAndAdjust( m_parser.TokenString );
          m_writer.WriteStartElement( m_parser.TokenString );

          string elementName = "html:" + m_parser.TokenString;
          if( m_config.Elements.Contains( elementName ) )
          {
            HTMLElement res = m_config.Elements[ elementName ] as HTMLElement;
            if( el == null && ( res.TagEnd == null ||
              res.TagEnd == "f"  || res.TagEnd == "o" ) ) el = res;
          }
          
          CheckPrefix( m_parser.TokenString );

          m_contextStack.Push( new Context( el, "html:" + m_parser.TokenString ) );
          inStartTag = true;
        }
          break;

        case ParserToken.StartTagClosedEnd:
        {
          m_writer.WriteEndElement();
          m_contextStack.Pop();
          writtenAttributes.Clear();
          inStartTag = false;
        }
          break;

        case ParserToken.EndTag:
        {
          string fullElementName = "html:" + m_parser.TokenString;
          CheckAndAdjustEndTag( fullElementName );
        }
          break;

        case ParserToken.StartTagEnd:
        {
          Context c = ( Context )m_contextStack.Peek();

          if( c.Element != null )
          {
            if( c.Element.TagEnd == "f" )
            {
              m_writer.WriteEndElement();
              m_contextStack.Pop();
            }

            if( c.Element.IsCDATA )
            {
              Debug.WriteLine( "CDATA section here" );

              m_parser.Position = ParserPosition.CData;
              m_parser.CDataElement = c.Element.SubName;
              //m_parser.SwitchToCData( c.Element.SubName );
            }
          }

          writtenAttributes.Clear();

          //m_contextStack.Pop();

          inStartTag = false;
        }
          break;

        case ParserToken.Attribute:
        {
          attributeName = m_parser.TokenString;
        }
          break;

        case ParserToken.AttributeValue:
        {
          if( !writtenAttributes.Contains( attributeName ) )
          {
            // If attribute name is empty, throw exception or skip.
            if( !this.SkipError && ( attributeName == null || attributeName.Length == 0 ) )
            {
              throw new ArgumentNullException( "attributeName" );
            }
            else if( attributeName != null && attributeName.Length > 0 )
            {
              m_writer.WriteAttributeString( attributeName, m_parser.TokenString );
              writtenAttributes.Add( attributeName );

              if( m_contextStack.Count > 0 )
              {
                Context context = m_contextStack.Peek() as Context;

                if( Utilities.StrEquals( attributeName, "content" ) &&
                  Utilities.StrEquals( context.FullName, "html:meta" )
                  )
                {
                  DetectFileEncoding( m_parser );
                }
              }
            }
          }

          attributeName = "";
        }
          break;

        case ParserToken.Comment:
        {
          string commentText = m_parser.TokenString;

          if( commentText.IndexOf( "--" ) != -1 )
          {
            commentText = commentText.Replace( "--", "- - " );
          }

          if( commentText.Length > 0 && commentText[ commentText.Length - 1 ] == '-' )
          {
            commentText += " ";
          }

          m_writer.WriteComment( commentText );
        }
          break;

        case ParserToken.Space:
        {
          if( !inStartTag &&
            m_parser.TokenString != null &&
            m_parser.TokenString.Length > 0 )
          {
            Context c = ( Context )m_contextStack.Peek();
            if( c.Element != null && c.Element.PreserveWhiteSpaces )
            {
              m_writer.WriteWhitespace( m_parser.TokenString );
            }
            else
            {
              string remainder = Utilities.DeleteWhiteSpace( m_parser.TokenString );
              if( remainder.Length > 0 )
              {
                m_writer.WriteWhitespace( remainder );
              }
            }
          }
        }
          break;

        case ParserToken.Text:
        {
          Context c = ( Context )m_contextStack.Peek();
          string text = m_parser.TokenString;
          if( c == null || ( c.Element != null && !c.Element.PreserveWhiteSpaces
            && !c.Element.IsCDATA ) )
          {
            text = Utilities.DeleteWhiteSpace( text );
          }

          if( !( c.Element != null && c.Element.IsCDATA ) )
          {
            m_writer.WriteString( text );
          }
          else
          {
            m_writer.WriteCData( text );
            //m_writer.WriteString( text );
            CheckAndAdjustEndTag( c.Element.Name );
          }

        }
          break;

        default:
          throw new ParseException(
            string.Format( "token: {0}, attr-name:{1}", tok, attributeName ) );
      }
    }
    /// <summary>
    /// Detects the encoding of the file.
    /// </summary>
    /// <param name="parser">String for encoding recognizing.</param>
    protected void DetectFileEncoding( HTMLTokenizer parser )
    {
      if( parser == null )
        throw new ArgumentNullException( "parser" );
      
      if( parser.Encoding is UnicodeEncoding ) return;
      
      string value = parser.TokenString;
      Encoding ret = Utilities.DetectEncoding( value );
      
      if( ret != null )
      {
        parser.Encoding = ret;
      }
    }
    /// <summary>
    /// Main method. Converts HTML document into XHTML document.
    /// </summary>
    /// <returns>Converts HTML to XHTML.</returns>
    public  bool Convert()
    {
      bool inStartTag = false;
      string attributeName = string.Empty;
      StringCollection writtenAttributes = new StringCollection();

      m_parser.Entities = m_config.Entities;

      while( !m_parser.IsEof )
      {
        ParserToken tok = m_parser.ReadToken();

        if( attributeName.Length > 0 &&
          tok != ParserToken.AttributeValue &&
          tok != ParserToken.Space )
        {
          if( !writtenAttributes.Contains( attributeName ) )
          {
            m_writer.WriteAttributeString( attributeName, attributeName );
            writtenAttributes.Add( "true" );
          }

          if( tok != ParserToken.Space ) attributeName = string.Empty;
        }

        RecognizeToken( m_parser.Token, ref attributeName, writtenAttributes, ref inStartTag );
      }

      while( m_contextStack.Count > 1 )
      {
        Context c = ( Context )m_contextStack.Peek();
        CheckAndAdjustEndTag( c.FullName );
      }

      return true;
    }
    /// <summary>
    /// Checks if the specified tag name holds a prefix.
    /// Also adds prefix to collection.
    /// </summary>
    /// <param name="value">Name of the tag.</param>
    private void CheckPrefix( string value )
    {
      if( value == null || value.Length == 0 ) return;

      string[] arrNames = value.Split( ':' );
      string prefix = arrNames[ 0 ];
      
      if( arrNames.Length == 2 && prefix != null && prefix.Length > 0 )
      {
        if( m_prefixes == null )
        {
          m_prefixes = new ArrayList();
        }

        if( !m_prefixes.Contains( prefix ) )
        {
          m_prefixes.Add( prefix );
        }
      }
    }
    #endregion
  }
}
