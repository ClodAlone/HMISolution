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
using System.Xml;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Utils;

#endregion

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
  /// <summary>
  /// Virtual lexem configuration.
  /// </summary>
  /// <remarks>
  /// Virtual lexem configuration is such kind of <see cref="IConfigLexem"/>
  /// implementation, when just link to real configuration is kept
  /// in memory. Virtual configurations does not support collapsing.
  /// </remarks>
  public class VirtualConfigLexem : IConfigLexem
  {
    #region Class members
    /// <summary>
    /// Config, this instance is linked to.
    /// </summary>
    private IConfigLexem m_parent;
    #endregion

    #region Class Properties
		/// <summary>
		/// Gets name of the format to be used in collapsed state.
		/// </summary>
		public string TypeCollapsed
		{
			get
			{
				return m_parent.TypeCollapsed;
			}
		}
		/// <summary>
    /// GET link to the virtual config for current lexem.
    /// </summary>
    /// <remarks>
    /// Virtual configs does not support collapsed state.
    /// </remarks>
    [XmlIgnore]
    public IConfigLexem VirtualConfig
    {
      get
      {
        return this;
      }
    }
    /// <summary>
    /// Parent of the virtual lexem configuration.
    /// </summary>
    [XmlIgnore]
    public IConfigLexem Parent
    {
      get
      {
        return m_parent;
      }
    }

    /// <summary>
    /// Begin symbol or word for lexem.
    /// </summary>
    [ XmlAttribute ]
    public string BeginBlock
    {
      get
      {
        return m_parent.BeginBlock;
      }
    }

    /// <summary>
    ///  If lexem has begin symbol and end symbol then use this property for
    /// setting end symbol. if lemex is "keyword" then this property must be
    /// set to null value.
    /// </summary>
    [ DefaultValue( null ) ]
    [ XmlAttribute ]
    public string EndBlock
    {
      get
      {
        return m_parent.EndBlock;
      }
    }

    /// <summary>
    /// If lexem can be divided on multi lines or has some special rules
    /// which can continue lexem then us this setting.
    /// </summary>
    [ DefaultValue( null ) ]
    [ XmlAttribute ]
    public string ContinueBlock
    {
      get
      {
        return m_parent.ContinueBlock;
      }
    }

    /// <summary>
    /// If many lexems has the same begin string then on parsing
    /// must be controlled order in which lexem parser will try to
    /// interpret input as lexem...
    /// </summary>
    [ DefaultValue( 0 ) ]
    [ XmlAttribute ]
    public int Priority
    {
      get
      {
        return m_parent.Priority;
      }
    }

    /// <summary>
    /// Format which must be used for coloring. If format is Custom then
    /// used FormatName property for format identification
    /// </summary>
    [ DefaultValue( typeof( FormatType ), "Text" ) ]
    [ XmlAttribute ]
    public FormatType Type
    {
      get
      {
        return m_parent.Type;
      }
    }

    /// <summary>
    /// FormatName which must be used for coloring.
    /// </summary>
    [ DefaultValue( "Text" ) ]
    [ XmlAttribute ]
    public string FormatName
    {
      get
      {
        return m_parent.FormatName;
      }
    }

    /// <summary>
    /// Is BeginBlock property contains Regular expression or not
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool IsBeginRegex
    {
      get
      {
        return m_parent.IsBeginRegex;
      }
    }

    /// <summary>
    /// Is EndBlock property contains Regular expression or not
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool IsEndRegex
    {
      get
      {
        return m_parent.IsEndRegex;
      }
    }

    /// <summary>
    /// Is ContinueBlock property contains Regular expression or not
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool IsContinueRegex
    {
      get
      {
        return m_parent.IsEndRegex;
      }
    }

    /// <summary>
    /// this flag indicate must parser parse lexem internals or not. For
    /// complex constructions data between begin and end blocks can have own
    /// formats.
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool IsComplex
    {
      get
      {
        return m_parent.IsComplex;
      }
    }

    /// <summary>
    /// sub-lexems which must be skipped if they found after BeginBlock
    /// string till EndBock string.
    /// </summary>
    [ XmlArrayItem( ElementName= "SubLexems", IsNullable=true )
    , XmlArrayItem( ElementName = "lexem", IsNullable = true, Type = typeof( ConfigLexem ) )
    , XmlArray ]
    public ArrayList SubLexems
    {
      get
      {
        return m_parent.SubLexems;
      }
    }
    /// <summary>
    /// List of references.
    /// </summary>
    [ XmlArrayItem( ElementName= "References", IsNullable=true )
    , XmlArrayItem( ElementName = "reference", IsNullable = true, Type = typeof( ReferenceConfig ) )
    , XmlArray ]
    public ArrayList References
    {
      get
      {
        return m_parent.References;
      }
    }
    /// <summary>
    /// Parent of the config.
    /// </summary>
    [XmlIgnore]
    public IConfigLexem ParentConfig
    {
      get
      {
        return m_parent.ParentConfig;
      }
    }

    /// <summary>
    /// This flag indicates, whether parser should look for lexem`s config just in local array,
    /// or also look in parents
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool OnlyLocalSublexems
    {
      get
      {
        return m_parent.OnlyLocalSublexems;
      }
    }

    /// <summary>
    /// GET, SET format by Type and FormatName.
    /// </summary>
    [XmlIgnore]
    public ISnippetFormat Format
    {
      get
      {
        return m_parent.Format;
      }
    }
    /// <summary>
    /// Is BeginBlock property contains Regular expression or not
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool IsCollapsable
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Condition, needed to pass check. Format: name=ON|OFF
    /// </summary>
    [XmlAttribute]
    [ DefaultValue( "" ) ]
    public string Condition
    {
      get
      {
        return m_parent.Condition;
      }
    }
    /// <summary>
    /// Static unique ID of configuration node.
    /// </summary>
    [ DefaultValue( -1 ) ]
    [ XmlAttribute ]
    public int ID
    { 
      get
      {
        return m_parent.ID;
      }
    }

    /// <summary>
    /// Language, lexem belongs to.
    /// </summary>
    [XmlIgnore]
    public IConfigLanguage Language
    { 
      get
      {
        return m_parent.Language;
      }
    }
    /// <summary>
    /// GET sign whether the end-block is just the way to exit higher by stack, or it is real ending of lexem.
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool IsPseudoEnd
    {
      get
      {
        return m_parent.IsPseudoEnd;
      }
    }
    /// <summary>
    /// GET sign of auto-indenting after lexem with such config.
    /// </summary>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool Indent
    {
      get
      {
        return m_parent.Indent;
      }
    }
    /// <summary>
    /// GET ID of the lexem configuration, that follows right after current 
    /// one is parsed. Such lexem must be complex and "OnlyLocalSublexems",
    /// without beginblock and with endblock.
    /// </summary>
    [ DefaultValue( -1 ) ]
    [ XmlAttribute ]
    public int NextID
    { 
      get
      {
        return m_parent.NextID;
      }
    }
    /// <summary>
    /// GET sign of dropping down context choice list after entering text of the current lexem.
    /// </summary>
    /// <remarks>
    /// Can be set only on non-complex lexems.
    /// </remarks>
    [ DefaultValue( false ) ]
    [ XmlAttribute ]
    public bool DropContextChoiceList
    {
      get
      {
        return m_parent.DropContextChoiceList;
      }
    }
    /// <summary>
    /// GET value, that shows whether context prompt should be shown after typing text of the current lexem.
    /// </summary>
    /// <remarks>
    /// Can be set only on non-complex lexems.
    /// </remarks>
    [ XmlAttribute ]
    public bool DropContextPrompt
    {
      get
      {
        return m_parent.DropContextPrompt;
      }
    }
    /// <summary>
    /// Gets whether content divider should be shown below lexem.
    /// </summary>
    [ XmlIgnore ]
    public bool ContentDivider
    {
      get
      {
        return m_parent.ContentDivider;
      }
    }
    /// <summary>
    /// Gets whether IndentationGuideline should be shown.
    /// </summary>
    [ XmlIgnore ]
    public bool IndentationGuideline
    {
      get
      {
        return m_parent.IndentationGuideline;
      }
    }
    /// <summary>
    /// Gets value indicating if lexem should be used if there are more than one config found on one priority level.
    /// </summary>
    public bool DefaultInGroup
    { 
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Gets value indicating if custom control should be used instead of the simple lexem rendering.
    /// </summary>
    [XmlIgnore]
    public bool UseCustomControl
    {
      get
      {
        return m_parent.UseCustomControl;
      }
    }
    /// <summary>
    /// Gets value indicating whether autoreplace triggers can be used.
    /// </summary>
    /// <value></value>
		[XmlIgnore]
		public bool AllowTriggers
		{
			get
			{
				return m_parent.AllowTriggers;
			}
		}
		#endregion

    #region Class Public Methods
    /// <summary>
    /// Checks string to the equalization to end block.
    /// If end block is regular expression, input string will be checked by RegExp
    /// </summary>
    /// <param name="str">String to be checked</param>
    /// <returns>True if it can be treated as end block.</returns>
    public bool IsEqualToEnd(string str)
    {
      if( str == null )
        throw new ArgumentNullException( "str" );

      if( str == string.Empty )
        throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

      return m_parent.IsEqualToBegin( str );
    }

    /// <summary>
    /// Checks string to the equalization to continue block.
    /// If continue block is regular expression, input string will be checked by RegExp
    /// </summary>
    /// <param name="str">String to be checked</param>
    /// <returns>True if it can be treated as continue block.</returns>
    public bool IsEqualToContinue(string str)
    {
      if( str == null )
        throw new ArgumentNullException( "str" );

      if( str == string.Empty )
        throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

      return m_parent.IsEqualToContinue( str );
    }

    /// <summary>
    /// Checks string to the equalization to begin block.
    /// If begin block is regular expression, input string will be checked by RegExp
    /// </summary>
    /// <param name="str">String to be checked</param>
    /// <returns>True if it can be treated as begin block.</returns>
    public bool IsEqualToBegin(string str)
    {
      if( str == null )
        throw new ArgumentNullException( "str" );

      if( str == string.Empty )
        throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

      return m_parent.IsEqualToEnd( str );
    }

    /// <summary>
    /// Searches for configs in sub-lexems.
    /// Current lexem config is not tested for equalization.
    /// If config was not found in sub-lexems,
    /// it will be searched in parent.
    /// </summary>
    /// <param name="str">String to find.</param>
    /// <returns>List of config lexems.</returns>
    public IList FindConfigs( string str )
    {
      if( str == null )
        throw new ArgumentNullException( "str" );

      if( str == string.Empty )
        throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

      return m_parent.FindConfigs( str );
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="parent">Parent config.</param>
    public VirtualConfigLexem( IConfigLexem parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parent = parent;
    }
    #endregion
  }
}