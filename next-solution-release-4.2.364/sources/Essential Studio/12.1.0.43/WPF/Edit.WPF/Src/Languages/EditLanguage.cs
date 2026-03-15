// <copyright file="EditLanguage.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
    ///<remarks>
    /// Edit Format has formatname, font, fontsize and foreground properties.
    /// </remarks>
    /// <example>
    /// <para><b>XAML</b></para>
    /// <para></para>
    /// <para></para>
    /// <para>&lt;Window x:Class=&quot;SampleApplication.Window1&quot;</para>
    /// <para> xmlns=&quot;http://
    /// schemas.microsoft.com/winfx/2006/xaml/presentation&quot;</para>
    /// <para> xmlns:x=&quot;http:// schemas.microsoft.com/winfx/2006/xaml&quot;</para>
    /// <para>    Title=&quot;Window1&quot; Height=&quot;300&quot; Width=&quot;300&quot;
    /// xmlns:syncfusion=&quot;http:// schemas.syncfusion.com/wpf&quot;&gt;</para>
    /// <para>    &lt;Grid&gt;</para>
    /// <para>        &lt;syncfusion:EditFormats Name=&quot;KeywordFormat&quot;
    /// Font=&quot;Arial&quot; FontSize=&quot;12&quot; /&gt;</para>
    /// <para>    &lt;/Grid&gt;</para>
    /// <para>&lt;/Window&gt;</para>
    /// <para></para>
    /// <para></para>
    /// <para><b>C#</b></para>
    /// <para></para>
    /// <para>EditFormats EditFormats1 = new EditFormats();</para>
    /// <para>EditFormats1.FormatName = &quot;KeywordFormat&quot;</para>
    /// <para>EditFormats1.Font = &quot;Arial&quot;</para>
    /// <para>EditFormats1.FontSize = &quot;12&quot;</para>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class EditFormats : IFormat
    {
        #region Local Variables

        /// <summary>
        ///  variable of FormatName property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string formatName;

        /// <summary>
        /// instance for Font property.
        /// </summary>
        private FontFamily font = null;

        /// <summary>
        /// instance for FontSize property. By default it is set to double.NaN.
        /// </summary>
        /// <returns>
        /// <para>Type: System.Double</para>
        /// </returns>
        private double fontSize = double.NaN;

        /// <summary>
        /// instance for Foreground property. By default it is set to Brushes.Black.
        /// </summary>
        private Brush foreground = Brushes.Black;

        #endregion Local Variables

        #region IFormat Members

        /// <summary>
        /// Gets or sets name of the Format object. Name property is used to identify the text style in the Language class
        /// </summary>
        /// <remarks>Specifies the FormatName of the Lexem.</remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string FormatName
        {
            get
            {
                return formatName;
            }
            set
            {
                formatName = value;
            }
        }

        /// <summary>
        /// Gets or sets the FontFamily to be applied when this instance of Format is applied.
        /// </summary>
        /// <remarks>Specifies the Font name of the Lexem</remarks>
        public FontFamily FontFamily
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        /// <summary>
        /// Gets or sets the FontSize to be applied when this instance of Format is applied
        /// </summary>
        /// <remarks>Specifies the Font Size of the Lexem</remarks>
        /// <value>
        /// Type: System.Double
        /// </value>
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the Foreground to be applied when this instance of Format is applied
        /// </summary>
        /// <remarks>Specifies the Foreground color of the Lexem</remarks>
        public Brush Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                foreground = value;
            }
        }

        #endregion IFormat Members
    }

    /// <summary>
    /// Lexem class holds values related to a lexical element used in a Language
    /// configuration
    /// </summary>
    /// <remarks>
    /// Lexem class has Starttext, Endtext, Multiline, type, format properties of a
    /// Lexem.
    /// </remarks>
    /// <example>
    /// <para><b>XAML</b></para>
    /// <para></para>
    /// <para></para>
    /// <para>&lt;Window x:Class=&quot;SampleApplication.Window1&quot;</para>
    /// <para> xmlns=&quot;http://
    /// schemas.microsoft.com/winfx/2006/xaml/presentation&quot;</para>
    /// <para> xmlns:x=&quot;http:// schemas.microsoft.com/winfx/2006/xaml&quot;</para>
    /// <para>    Title=&quot;Window1&quot; Height=&quot;300&quot; Width=&quot;300&quot;
    /// xmlns:syncfusion=&quot;http:// schemas.syncfusion.com/wpf&quot;&gt;</para>
    /// <para>    &lt;Grid&gt;</para>
    /// <para>        &lt;syncfusion:Lexem StartText=&quot;/*&quot;
    /// EndText=&quot;*/&quot; FormatName=&quot;CommentType&quot; /&gt;</para>
    /// <para>    &lt;/Grid&gt;</para>
    /// <para>&lt;/Window&gt;</para>
    /// <para></para>
    /// <para></para>
    /// <para><b>C#</b></para>
    /// <para></para>
    /// <para>Lexem Lexem1 = new Lexem();</para>
    /// <para>Lexem1.StartText = &quot;/*&quot;</para>
    /// <para>Lexem1.EndText = &quot;*/&quot;</para>
    /// <para>Lexem1.FormatName = &quot;CommentType&quot;</para>
    /// </example>
    /// <seealso
    /// cref="Syncfusion.Windows.Edit.IBlock">Syncfusion.Windows.Edit.IBlock</seealso>
    /// <seealso
    /// cref="Syncfusion.Windows.Edit.IFormat">Syncfusion.Windows.Edit.IFormat</seealso>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Lexem : ILexem
    {
        #region local Variables

        /// <summary>
        ///  variable of StartText property.
        /// </summary>
        /// <remarks>Specifies the Start text of a Lexem</remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        private string starttext;

        /// <summary>
        ///  variable of EndText property.
        /// </summary>
        /// <remarks>Specifies the End text of a Lexem</remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        private string endtext;

        /// <summary>
        ///  variable of IsMultiline property.
        /// </summary>
        /// <remarks>Specifies the Multiline option of a Lexem</remarks>
        private bool multiline;

        /// <summary>
        ///  variable of LexemType property.
        /// </summary>
        private EditTokenType type;

        /// <summary>
        ///  variable of ContainsEndText property.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        private bool contains;

        /// <summary>
        ///  variable of FormatName property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string format;

        /// <summary>
        ///  variable of IsRegex property
        /// </summary>
        private bool isregex;

        private bool isalternatetext;

        private string intellisensetext;

        #endregion local Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Lexem"/> class.
        /// </summary>
        public Lexem()
        {
            SubLexems = new LexemCollection();
            CheckParentLexemType = false;
            ShowAlternateIntellisenseText = false;
            ExcludeItemInIntellisense = false;
            Indent = false;
            IsCollapsible = true;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the IsRegex is true or false. This denotes the Start and EndText properties contains Regex patterns. By default it is set to false.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsRegex
        {
            get
            {
                return isregex;
            }

            set
            {
                isregex = value;
            }
        }

        #endregion Properties

        #region IBlock Members

        /// <summary>
        /// Gets or sets the start text of a lexical element.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string StartText
        {
            get
            {
                return starttext;
            }

            set
            {
                starttext = value;
            }
        }

        /// <summary>
        /// Gets or sets the end text of a lexical element
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string EndText
        {
            get
            {
                return endtext;
            }

            set
            {
                endtext = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsMultiline is true or false. This property denotes whether this lexical element can last for multiple lines. By default it is set to false.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsMultiline
        {
            get
            {
                return multiline;
            }

            set
            {
                multiline = value;
            }
        }

        /// <summary>
        /// Gets or sets type of lexem.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.EditTokenType
        /// </value>
        public EditTokenType LexemType
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ContainsEndText is true or false. This denotes whether this lexical element contains end value to be captured. By default it is set to false.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool ContainsEndText
        {
            get
            {
                return contains;
            }

            set
            {
                contains = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool CheckParentLexemType
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public EditTokenType ParentLexemType { get; set; }

        #endregion IBlock Members

        #region IFormat Members

        /// <summary>
        /// Gets or sets the name of EditFormat object to be applied when this lexical element is captured
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string FormatName
        {
            get
            {
                return format;
            }

            set
            {
                format = value;
            }
        }

        #endregion IFormat Members

        #region ILexem Members

        /// <summary>
        /// Gets the SubLexems collection for applying nested coloring of lexical elements
        /// </summary>
        public IEnumerable SubLexems
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string IntellisenseDisplayText
        {
            get
            {
                if (this.ShowAlternateIntellisenseText)
                {
                    return intellisensetext;
                }
                else
                {
                    return starttext;
                }
            }
            set
            {
                intellisensetext = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ScopeLevel ScopeLevel
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowAlternateIntellisenseText
        {
            get
            {
                return isalternatetext;
            }
            set
            {
                isalternatetext = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ExcludeItemInIntellisense
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool Indent
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsCollapsible
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool EndBlockOnRecurrence
        {
            get;
            set;
        }

        #endregion ILexem Members
    }

    /// <summary>
    /// EditLanguage class is used to creating Language configurations for Syntax
    /// highlighting and Outlining
    /// </summary>
    /// <remarks>
    /// Edit Language class contains the format collection and lexem collection with
    /// language name, file extension, case sensitive, text foreground properties.
    /// </remarks>
    /// <example>
    /// <para><b>XAML</b></para>
    /// <para></para>
    /// <para>&lt;Window x:Class=&quot;SampleApplication.Window1&quot;</para>
    /// <para> xmlns=&quot;http://
    /// schemas.microsoft.com/winfx/2006/xaml/presentation&quot;</para>
    /// <para> xmlns:x=&quot;http:// schemas.microsoft.com/winfx/2006/xaml&quot;</para>
    /// <para>    Title=&quot;Window1&quot; Height=&quot;300&quot; Width=&quot;300&quot;
    /// xmlns:syncfusion=&quot;http:// schemas.syncfusion.com/wpf&quot;&gt;</para>
    /// <para>    &lt;Grid&gt;</para>
    /// <para>        &lt;syncfusion:EditLanguage Name=&quot;CSharp&quot;
    /// LanguageType=&quot;Procedural&quot; FileExtension=&quot;.cs;/&gt;</para>
    /// <para>    &lt;/Grid&gt;</para>
    /// <para>&lt;/Window&gt;</para>
    /// <para></para>
    /// <para><b>C#</b></para>
    /// <para></para>
    /// <para>EditLanguage EditLanguage1 = new EditLanguage();</para>
    /// <para>EditLanguage1.Name = &quot;CSharp&quot;;</para>
    /// <para>EditLanguage1.StartLine = &quot;Procedural&quot;;</para>
    /// <para>EditLanguage1.FileExtension = &quot;.cs&quot;;</para>
    /// </example>
    /// <seealso cref="Syncfusion.Windows.Edit.EditLanguage">Syncfusion.Windows.Edit.EditLanguage</seealso>
    /// <seealso cref="Syncfusion.Windows.Edit.FormatsCollection">Syncfusion.Windows.Edit.FormatsCollection</seealso>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Obsolete("This class is no longer in use due to modifications in language implementation, Kindly use create a class inheriting from LanguageBase or ProceduralLanguageBase or MarkupLanguageBase")]
    public class EditLanguage
    {
        #region local variables

        /// <summary>
        /// instance for Name property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string langname;

        /// <summary>
        /// instance for FileExtension property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string fileextension;

        /// <summary>
        /// instance for case sensitive property.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        private bool casesensitive;

        /// <summary>
        /// instance for TextForeground property.  default color is black.
        /// </summary>
        private Brush textforeground = Brushes.Black;

        /// <summary>
        /// instance for BlockStart property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string blockstart;

        /// <summary>
        /// instance for BlockEnd property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        private string blockend;

        /// <summary>
        /// instance for Formats property.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.FormatsCollection
        /// </value>
        private FormatsCollection formats;

        /// <summary>
        /// instance for Lexems property.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.LexemCollection
        /// </value>
        private LexemCollection lexems;

        /// <summary>
        /// instance for LanguageType property.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.EditLanguageType
        /// </value>
        private EditLanguageType langtype;

        #endregion local variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditLanguage"/> class.
        /// </summary>
        /// <remarks>
        /// EditLanguage constructor used to initializes the Format and Lexem collections.
        /// </remarks>
        public EditLanguage()
        {
            formats = new FormatsCollection();
            lexems = new LexemCollection();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets Name of the Language.
        /// </summary>
        /// <remarks>
        /// <para>Specifies the Name of the Language.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: System.String</para>
        /// </value>
        public string Name
        {
            get
            {
                return langname;
            }

            set
            {
                langname = value;
            }
        }

        /// <summary>
        /// Gets or sets File Extension supported by the language
        /// </summary>
        /// <remarks>
        /// <para>Specifies the file extension.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: System.String</para>
        /// </value>
        public string FileExtension
        {
            get
            {
                return fileextension;
            }

            set
            {
                if (value.TrimStart(' ').StartsWith("."))
                {
                    fileextension = value;
                }
                else
                {
                    fileextension = "." + value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Language has case sensitive or not
        /// </summary>
        /// <remarks>
        /// Specifies the particular language is case sensitive or not.
        /// </remarks>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool CaseSensitive
        {
            get
            {
                return casesensitive;
            }

            set
            {
                casesensitive = value;
            }
        }

        /// <summary>
        /// Gets or sets foreground brush to be applied when no Lexems are applicable for
        /// the text
        /// </summary>
        /// <remarks>
        /// <para>Specifies the particular language Text Foreground.</para>
        /// </remarks>
        /// <value>
        /// <para>TextForeground = Brushes.Green;</para>
        /// </value>
        public Brush TextForeground
        {
            get
            {
                return textforeground;
            }

            set
            {
                textforeground = value;
            }
        }

        /// <summary>
        /// Gets or sets BlockStart property. Specifies the start symbol that denotes start of a block of code
        /// </summary>
        /// <remarks>
        /// Specifies the Block start text.
        /// </remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockStart
        {
            get
            {
                return blockstart;
            }

            set
            {
                blockstart = value;
            }
        }

        /// <summary>
        /// Gets or sets BlockEnd property. Specifies the end symbol that denotes end of block of code
        /// </summary>
        /// <remarks>
        /// Specifies the Block End text.
        /// </remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockEnd
        {
            get
            {
                return blockend;
            }

            set
            {
                blockend = value;
            }
        }

        /// <summary>
        /// Gets a collection type property that contains the list of text styles to be applied to the language
        /// </summary>
        /// <remarks>This Format property returns the Format Collection to the language content.</remarks>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.FormatsCollection
        /// </value>
        public FormatsCollection Formats
        {
            get
            {
                return formats;
            }
        }

        /// <summary>
        /// Gets a Collection type property that contains the list of Lexical elements to be colored using the defined formats
        /// </summary>
        /// <remarks>This Lexems property returns the Lexems Collection.</remarks>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.LexemCollection
        /// </value>
        public LexemCollection Lexems
        {
            get
            {
                return lexems;
            }
        }

        /// <summary>
        /// Gets or sets a Specific type of language. For instance, Procedural or Markup
        /// </summary>
        /// <remarks>The LanguageType property used to get and sets the language types like Procedural or Markup.</remarks>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.EditLanguageType
        /// </value>
        public EditLanguageType LanguageType
        {
            get
            {
                return langtype;
            }

            set
            {
                langtype = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Helper method to retrieve format from the formats collection with name as its parameter
        /// </summary>
        /// <param name="fname">Gets the Formatname from the reporting source</param>
        /// <returns>EditFormat object from Formats collection of EditLanguage class</returns>
        /// <remarks>Edit format method used to get the format properties of the specified format name.</remarks>
        public EditFormats GetFormat(string fname)
        {
            if (this.Formats != null)
            {
                IEnumerable<EditFormats> formatsel = this.Formats.Where(format => format.FormatName == fname);
                foreach (EditFormats format in formatsel)
                {
                    return format;
                }
            }

            return null;
        }

        #endregion Methods
    }
}