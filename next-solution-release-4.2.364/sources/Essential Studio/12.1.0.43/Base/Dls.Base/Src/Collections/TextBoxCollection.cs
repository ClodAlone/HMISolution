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
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS.Collections
{
	/// <summary>
	/// Summary description for TextBoxCollection.
	/// </summary>
	public class TextBoxCollection
    : EntityCollectionBase,
      ITextBoxCollection
	{
    #region Class properties
    /// <summary>
    /// Gets section by index.
    /// </summary>
    public ITextBox this[ int index ]
    {
      get
      {
        return ( ITextBox )List[ index ];
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public TextBoxCollection( IDocument doc )
      : base( doc )
    {}
    #endregion

    #region Class public methods
    /// <summary>
    /// Adds textbox to collection.
    /// </summary>
    /// <param name="textBox"></param>
    /// <returns></returns>
    public int Add( ITextBox textBox )
    {
      return List.Add( textBox );
    }
    #endregion

    #region IDLSXmlEntityCollection implementation
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object CreateItem( IXDLSContentReader reader )
    {
      return (Document as Document).CreateParagraphItemImpl( ParagraphItemType.TextBox );
    }
    /// <summary>
    /// Gets name of xml tag
    /// </summary>
    public override string TagItemName
    {
      get
      {
        return XDLSConstants.TextBoxesTag;
      }
    }
    #endregion
	}
}
