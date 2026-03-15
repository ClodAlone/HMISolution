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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.Entities;
using System.Reflection;

#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#else
using System.Drawing;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using Image = System.Drawing.Image;
using DataRow = System.Data.DataRow;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
#if SILVERLIGHT || WP
  /// <summary>
  /// Represents the mail merge functionality. 
  /// </summary>
  public class MailMerge
  {
    #region Fields
    private WordDocument m_doc;
    private GroupSelector m_groupSelector;
    private WSectionCollection m_contentSections;
    private string[] m_names;
    private string[] m_values;
    private bool m_bClearFields = true;
    private bool m_bBeginGroupFound = false;
    private bool m_bEndGroupFound = false;
    private bool m_bRemoveEmptyPara;
    private bool m_bRemoveEmptyGroup;
    /// <summary>
    /// Fields used for nested mail merge.
    /// </summary>
    //private DbConnection m_conn;
    private MailMergeDataSet m_curDataSet;
    private Dictionary<string, IRowsEnumerator> m_nestedEnums;
    private Regex m_varCmdRegex;
    private Stack<GroupSelector> m_groupSelectors;
    private List<DictionaryEntry> m_commands;
    private bool m_bIsNested;
    private Dictionary<string, string> m_mappedFields;
    private MailMergeDataSet m_dataSet = null;
    private int m_mergedRecordCount = 0;
    private Dictionary<string, bool> m_clearFieldsState;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether [clear fields].
    /// </summary>
    /// <value><c>true</c> if it clears the fields, set to <c>true</c>.</value>
    public bool ClearFields
    {
      get
      {
        return m_bClearFields;
      }
      set
      {
        m_bClearFields = value;
      }
    }
    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    protected WordDocument Document
    {
      get
      {
        return m_doc;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to remove paragraphs which contain empty merge fields
    /// (merge field with no data after mail merge).
    /// </summary>
    /// <value>
    /// Set "true" to remove empty paragraphs; otherwise, "false".
    /// </value>
    public bool RemoveEmptyParagraphs
    {
      get
      {
        return m_bRemoveEmptyPara;
      }
      set
      {
        m_bRemoveEmptyPara = value;
      }
    }
     /// <summary>
     /// Gets or sets a value indicating whether to remove groups which contain empty merge fields
     /// (merge field with no data after mail merge).
     /// </summary>
     /// <value>
     /// Set "true" to remove the empty groups; By default the value is false.
     /// </value>
     public bool RemoveEmptyGroup
     {
         get
         {
             return m_bRemoveEmptyGroup;
         }
         set
         {
             m_bRemoveEmptyGroup = value;
         }
     }
    /// <summary>
    /// Gets the enumerators which are used during mailmerge.
    /// </summary>
    /// <value>The nested enums.</value>
    private Dictionary<string, IRowsEnumerator> NestedEnums
    {
      get
      {
        if( m_nestedEnums == null )
        {
          m_nestedEnums = new Dictionary<string, IRowsEnumerator>();
        }
        return m_nestedEnums;
      }
    }
    /// <summary>
    /// Gets the current data set for nested mail merge.
    /// </summary>
    /// <value>The current data set.</value>
    private MailMergeDataSet CurrentDataSet
    {
      get
      {
        if( m_curDataSet == null )
        {
          m_curDataSet = new MailMergeDataSet();
        }
        return m_curDataSet;
      }
    }
    /// <summary>
    /// Gets the variable command regulat expression.
    /// </summary>
    /// <value>The variable command regex.</value>
    private Regex VariableCommandRegex
    {
      get
      {
        if( m_varCmdRegex == null )
        {
          m_varCmdRegex = new Regex( "%([^\"%]+)%" );
        }
        return m_varCmdRegex;
      }
    }
    /// <summary>
    /// Gets the group selectors.
    /// </summary>
    /// <value>The group selectors.</value>
    private Stack<GroupSelector> GroupSelectors
    {
      get
      {
        if( m_groupSelectors == null )
        {
          m_groupSelectors = new Stack<GroupSelector>();
        }
        return m_groupSelectors;
      }
    }
    /// <summary>
    /// Gets the collection of mapped fields. Mapped fields represent mapping between
    /// fields names in the data source and mail merge fields in the document. The keys of
    /// the collection are merge field names and the values are field names in the data source.
    /// </summary>
    /// <value>The collection of mapped fields.</value>
    public Dictionary<string, string> MappedFields
    {
      get
      {
        if( m_mappedFields == null )
        {
          m_mappedFields = new Dictionary<string, string>();
        }
        return m_mappedFields;
      }
    }
    /// <summary>
    /// Gets the previous state of the clear fields.
    /// </summary>
    /// <value>The previous state of the clear fields.</value>
    private Dictionary<string, bool> ClearFieldsState
    {
        get
        {
            if (m_clearFieldsState == null)
            {
                m_clearFieldsState = new Dictionary<string, bool>();
            }
            return m_clearFieldsState;
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs during mail merge when a text merge field is encountered in the document
    /// </summary>
    public event MergeFieldEventHandler MergeField;
    /// <summary>
    /// Occurs during mail merge when an image merge field is encountered in the document
    /// </summary>
    public event MergeImageFieldEventHandler MergeImageField;
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="MailMerge"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    internal MailMerge( WordDocument document )
    {
      m_doc = document;
      m_contentSections = new WSectionCollection();
      m_groupSelector = new GroupSelector(new GroupSelector.GroupFound(OnGroupFound));
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Performs mail merge operation.
    /// </summary>
    /// <param name="fieldNames">array of fields names</param>
    /// <param name="fieldValues">array of fields values</param>
    public void Execute( string[] fieldNames, string[] fieldValues )
    {
      Document.IsMailMerge = true;

      if( fieldNames == null )
      {
        throw new ArgumentNullException( "fieldNames" );
      }
      if( fieldValues == null )
      {
        throw new ArgumentNullException( "fieldValues" );
      }

      m_names = fieldNames;
      m_values = fieldValues;

      if( m_names.Length > 0 )
      {
        IWSection sec = null;
        for( int i = 0, cnt = Document.Sections.Count; i < cnt; i++ )
        {
          sec = Document.Sections[ i ];
          ExecuteForSection( sec, null );
        }
      }
      Document.IsMailMerge = false;
    }
    /// <summary>
    /// Performs mail merge operation.
    /// </summary>
    /// <param name="dataSource">IEnumerable data source</param>
    public void Execute( IEnumerable  dataSource )
    {
      if( dataSource == null )
        throw new ArgumentNullException( "datasource" );

      MailMergeDataTable table = new MailMergeDataTable( string.Empty, dataSource );
      ExecuteGroup( table );
    }
    /// <summary>
    /// Performs mail merge operation.
    /// </summary>
    /// <param name="datatable">MailMergeDataTable</param>
    public void ExecuteGroup( MailMergeDataTable dataSource )
    {
      if( dataSource == null )
        throw new ArgumentNullException( "datasource" );

      if( dataSource.GroupName == string.Empty )
        Execute( new DataTableEnumerator( dataSource ) );
      else
        ExecuteGroup( new DataTableEnumerator( dataSource ) );
    }
    /// <summary>
    /// Performs mail merge operation.
    /// </summary>
    /// <param name="dataSource">MailMergeDataSet</param>
    /// <param name="commands">Commands list</param>
    public void ExecuteNestedGroup( MailMergeDataSet dataSource, List<DictionaryEntry> commands )
    {
      if( dataSource == null || dataSource.DataSet.Count == 0 )
        throw new ArgumentException( "dataSet is empty" );

      if( commands == null || commands.Count == 0 )
        throw new ArgumentException( "commands list is empty" );

      RemoveSpellChecking();
      m_dataSet = dataSource;
      m_commands = commands;
      DictionaryEntry entry = ( DictionaryEntry )commands[ 0 ];
      Document.IsMailMerge = true;
      m_bIsNested = true;
      ExecuteNestedGroup( ( string )entry.Key );

      if( m_nestedEnums != null )
      {
        m_nestedEnums.Clear();
        m_nestedEnums = null;
      }
      if( m_dataSet != null )
      {
        m_dataSet.Clear();
        m_dataSet = null;
      }
      Document.IsMailMerge = false;
      m_bIsNested = false;
    }
    /// <summary>
    /// Returns a collection of mergefield names found in the document.
    /// </summary>
    /// <returns></returns>
    public string[] GetMergeFieldNames()
    {
      List<string> fieldsArray = new List<string>();

      GetMergeFieldNamesImpl( fieldsArray, null );

      return fieldsArray.ToArray();
    }
    /// <summary>
    /// Gets the merge field names.
    /// </summary>
    /// <param name="groupName">Name of the group.</param>
    /// <returns></returns>
    public string[] GetMergeFieldNames( string groupName )
    {
      List<string> fieldsArray = new List<string>();

      GetMergeFieldNamesImpl( fieldsArray, groupName );

      return fieldsArray.ToArray();
    }
    /// <summary>
    /// Gets the merge field names.
    /// </summary>
    /// <param name="groupName">Name of the group.</param>
    /// <returns></returns>
    public string[] GetMergeGroupNames()
    {
      List<string> groupNames = new List<string>();
      Stack<EntityEntry> entDeep = new Stack<EntityEntry>();
      entDeep.Push( new EntityEntry( Document ) );

      do
      {
        EntityEntry ent = entDeep.Peek();

        // If we have child - add it to stack
        if( ent.Current != null && ent.Current.IsComposite )
        {
          ICompositeEntity ce = ent.Current as ICompositeEntity;

          if( ce.ChildEntities.Count > 0 )
          {
            entDeep.Push( new EntityEntry( ce.ChildEntities[ 0 ] ) );
            continue;
          }
        }

        // Process leafs
        if( ent.Current != null )
        {
          if( ent.Current.EntityType == EntityType.MergeField )
          {
            WMergeField mf = ent.Current as WMergeField;
            if( IsBeginGroup( mf ) )
            {
              groupNames.Add( mf.FieldName );
            }
          }
        }

        // Move next or drop
        while( !ent.Fetch() )
        {
          entDeep.Pop();

          if( entDeep.Count == 0 )
            break;

          ent = entDeep.Peek();
        }

      }
      while( entDeep.Count > 0 );

      return groupNames.ToArray();
    }
    /// <summary>
    /// Gets the merge group names2.
    /// </summary>
    /// <returns></returns>
    private string[] GetMergeGroupNames2()
    {
      List<string> groupNames = new List<string>();
      Stack<IEnumerator> enumStack = new Stack<IEnumerator>();
      enumStack.Push( Document.ChildEntities.GetEnumerator() );

      do
      {
        IEnumerator en = enumStack.Peek();

        // If we have child - add it to stack
        if( en.MoveNext() )
        {
          ICompositeEntity ce = en.Current as ICompositeEntity;

          if( ce != null && ce.ChildEntities.Count > 0 )
          {
            enumStack.Push( ce.ChildEntities.GetEnumerator() );
            continue;
          }
        }

        // Process leafs
        if( en.Current != null )
        {
          Entity ent = en.Current as Entity;

          if( ent != null && ent.EntityType == EntityType.MergeField )
          {
            WMergeField mf = ent as WMergeField;
            if( IsBeginGroup( mf ) )
            {
              groupNames.Add( mf.FieldName );
            }
          }
        }

        // Move next or drop
        while( !en.MoveNext() )
        {
          enumStack.Pop();

          if( enumStack.Count == 0 )
            break;

          en = enumStack.Peek();
        }

      }
      while( enumStack.Count > 0 );

      return groupNames.ToArray();
    }
    /// <summary>
    /// Gets the merge field names.
    /// </summary>
    /// <param name="fieldsArray">The fields array.</param>
    /// <param name="groupName">Name of the group.</param>
    private void GetMergeFieldNamesImpl( List<string> fieldsArray, string groupName )
    {
      WSection section = null;
      TextBodyItem paragraph = null;
      for( int index = 0, cnt = Document.Sections.Count; index < cnt; index++ )
      {
        section = Document.Sections[ index ];
        for( int j = 0, parCnt = section.Body.Items.Count; j < parCnt; j++ )
        {
          paragraph = section.Body.Items[ j ];
          GetFieldNamesForParagraph( fieldsArray, paragraph, groupName );
        }
        // Executes for headers/footers
        for( int i = 0; i < 6; i++ )
        {
          for( int j = 0, parCnt = section.HeadersFooters[ i ].Items.Count; j < parCnt; j++ )
          {
            paragraph = section.HeadersFooters[ i ].Items[ j ];
            GetFieldNamesForParagraph( fieldsArray, paragraph, groupName );
          }
        }
      }
    }
    #endregion

    #region Implementation / with groups
    /// <summary>
    /// Called when group found by GroupSelector.
    /// </summary>
    private void OnGroupFound( IRowsEnumerator rowsEnum )
    {
      GroupSelector gs = m_groupSelector;
      bool isFieldsHidded = false;
      if (m_bClearFields && gs.GroupSelection != null && gs.GroupSelection.TextBody.Owner != null && gs.GroupSelection.TextBody.Owner is WSection && rowsEnum.RowsCount == 0)
      {
          int startFieldIndex = gs.BeginGroupField.Owner.GetIndexInOwnerCollection();
          int endFieldIndex = gs.EndGroupField.Owner.GetIndexInOwnerCollection();
          WTextBody textbody = gs.BeginGroupField.Owner.Owner as WTextBody;
          for (int i = startFieldIndex; i <= endFieldIndex; i++)
          {
              if (textbody.Items[i] is WParagraph)
              {
                  HideFields(textbody.Items[i] as WParagraph);
              }
              else if (textbody.Items[i] is WTable)
              {
                  WTable table = textbody.Items[i] as WTable;
                  for (int j = 0, rowCnt = table.Rows.Count; j < rowCnt; j++)
                  {
                      HideFields(table.Rows[j]);
                  }
              }
          }
          isFieldsHidded = true;
          if (rowsEnum.RowsCount == 0 && RemoveEmptyGroup)
          {
              EmptyGroup(gs);
          }
      }
      else
      {
          if (rowsEnum.RowsCount == 0 && RemoveEmptyGroup)
          {
              EmptyGroup(gs);
          }
          HideField(gs.BeginGroupField);
          HideField(gs.EndGroupField);
      }
      if( m_bIsNested )
      {
        m_groupSelector.BeginGroupField.FieldName = string.Empty;
        m_groupSelector.EndGroupField.FieldName = string.Empty;
      }

      if (!isFieldsHidded && !CheckSelection(rowsEnum))
        return;

      if( gs.GroupSelection != null )
      {
        OnBodyGroupFound( rowsEnum );
      }
      else if( gs.RowSelection != null )
      {
        OnRowGroupFound( rowsEnum );
      }
    }
    /// <summary>
    /// Remove items between Empty Group
    /// </summary>
    /// <param name="gs">The group selector</param>
    private void EmptyGroup(GroupSelector gs)
    {
        if (gs.BeginGroupField.OwnerParagraph.IsInCell)
        {
            if (gs.BeginGroupField.Prefix == "TableStart")
            {
                EmptyGroupInTable(gs);
            }
            else
            {
                EmptyGroupInTableCell(gs);
            }
        }
        else
        {
            EmptyGroupInTextbody(gs);
        }
        if (gs.GroupSelection != null)
        {
            //Set group selection start and end index to be the same after emptying the group.
            gs.GroupSelection.ItemEndIndex = gs.GroupSelection.ItemStartIndex;
        }
        if (gs.RowSelection != null)
        {
            //Set row selection start and end index to be the same after emptying the group.
            gs.RowSelection.EndRowIndex = gs.RowSelection.StartRowIndex;
        }
    }
    /// <summary>
    /// Remove items between Empty group present in Text body.
    /// </summary>
    /// <param name="gs">The group selector<</param>
    private void EmptyGroupInTextbody(GroupSelector gs)
    {
        int startIndex = gs.BeginGroupField.GetIndexInOwnerCollection();
        int startParaIndex = (gs.BeginGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();
        int endIndex = gs.EndGroupField.GetIndexInOwnerCollection();
        int endParaIndex = (gs.EndGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();

        WTextBody textbody = gs.BeginGroupField.OwnerParagraph.Owner as WTextBody;
        if (startParaIndex == endParaIndex)
        {
            //Remove items between start and end index
            RemoveItems((textbody.Items[startParaIndex] as WParagraph), startIndex, endIndex);
        }
        else if ((textbody.Items[startParaIndex] as WParagraph).Items.Count > 1 && startIndex > 0)
        {
            //Removes items in paragraph.
            RemoveItems((textbody.Items[startParaIndex] as WParagraph), startIndex, (textbody.Items[startParaIndex] as WParagraph).Items.Count);
            //Removes items in textbody.
            RemoveItems(textbody, startParaIndex + 1, endParaIndex);

            //Removes paragraph if the end index is the last item of paragraph
            if (endIndex == (textbody.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                textbody.Items.RemoveAt(startParaIndex + 1);
            else if ((textbody.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
            {
                //Removes items in paragraph
                RemoveItems((textbody.Items[startParaIndex + 1] as WParagraph), 0, endIndex + 1);
            }
        }
        else
        {
            //Removes items in textbody.
            RemoveItems(textbody, startParaIndex, endParaIndex);
            //Removes paragraph if the end index is the last item of paragraph
            if (endIndex == (textbody.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                textbody.Items.RemoveAt(startParaIndex + 1);
            else if ((textbody.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
            {
                //Removes items in paragraph
                RemoveItems((textbody.Items[startParaIndex + 1] as WParagraph), 0, endIndex + 1);
            }
        }
    }
    /// <summary>
    /// Removes items present between empty group present in Table.
    /// </summary>
    /// <param name="gs">The group selector</param>
    private void EmptyGroupInTable(GroupSelector gs)
    {
        int startIndex = gs.BeginGroupField.GetIndexInOwnerCollection();
        int endIndex = gs.EndGroupField.GetIndexInOwnerCollection();
        int startParaIndex = (gs.BeginGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();
        int startCellIndex = (gs.BeginGroupField.Owner.Owner as WTableCell).GetCellIndex();
        int endCellIndex = (gs.EndGroupField.Owner.Owner as WTableCell).GetCellIndex();
        int startRowIndex = (gs.BeginGroupField.Owner.Owner.Owner as WTableRow).GetRowIndex();
        int endRowIndex = (gs.EndGroupField.Owner.Owner.Owner as WTableRow).GetRowIndex();
        int endParaIndex = (gs.EndGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();

        WTable table = gs.BeginGroupField.Owner.Owner.Owner.Owner as WTable;
        RemoveItemsAfterTableStart(gs, table, startIndex, startParaIndex, startCellIndex, endCellIndex, startRowIndex, endRowIndex);
        if (startRowIndex != endRowIndex)
            RemoveItemsAtTableEnd(gs, table, endIndex, endParaIndex, startCellIndex, endCellIndex, startRowIndex, endRowIndex);
        else
        {
            //Removes items if start row index and end rom index are same.
            if (startIndex == 0 && startParaIndex == 0)
            {
                //Removes the row if end para index and end index are the last element of cell and paragraph respectively.
                if (endParaIndex == table.Rows[startRowIndex].Cells[startCellIndex].Items.Count - 1 && endIndex == (table.Rows[startRowIndex].Cells[startCellIndex].Items[endParaIndex] as WParagraph).Items.Count - 1)
                {
                    table.Rows.RemoveAt(startRowIndex);
                }
                else
                {
                    //Remove items in paragraph
                    RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex].Items[endParaIndex], 0, endIndex);
                    //Remove items in cell
                    RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex], 0, endParaIndex);
                }
            }
        }
    }
    /// <summary>
    /// Removes items present in Group after Table Start field
    /// </summary>
    /// <param name="gs">The group selector</param>
    /// <param name="table">Table</param>
    /// <param name="startIndex">Start index</param>
    /// <param name="startParaIndex">Start para index</param>
    /// <param name="startCellIndex">Start cell index</param>
    /// <param name="endCellIndex">End cell index</param>
    /// <param name="startRowIndex">Start row index</param>
    /// <param name="endRowIndex">End row index</param>
    private void RemoveItemsAfterTableStart(GroupSelector gs, WTable table, int startIndex, int startParaIndex, int startCellIndex, int endCellIndex, int startRowIndex, int endRowIndex)
    {
        if (startIndex == 0)
        {
            if (startParaIndex == 0)
            {
                if (startCellIndex == 0)
                {
                    if (startRowIndex != endRowIndex)
                    {
                        //Remove items in table
                        RemoveItems(table, startRowIndex, endRowIndex);
                    }
                    else if (startCellIndex == endCellIndex)
                    {
                        //Removes the cell start cell index and end cell index are the same.
                        table.Rows[startRowIndex].Cells.RemoveAt(startCellIndex);
                    }
                    else
                        //Remove items in row
                        RemoveItems(table.Rows[startRowIndex], startCellIndex, endCellIndex);
                }
                else
                {
                    if (startRowIndex != endRowIndex)
                    {
                        if (startCellIndex == table.Rows[startRowIndex].Cells.Count - 1)
                        {
                            table.Rows[startRowIndex].Cells.RemoveAt(startCellIndex);
                        }
                        else
                            //Remove items in row
                            RemoveItems(table.Rows[startRowIndex], startCellIndex, table.Rows[startRowIndex].Cells.Count - 1);

                        //Remove items in table
                        if (startRowIndex + 1 < endRowIndex)
                            RemoveItems(table, startRowIndex + 1, endRowIndex);
                    }
                    else
                    {
                        //Removes the cell start cell index and end cell index are the same.
                        if (startCellIndex == endCellIndex)
                        {
                            table.Rows[startRowIndex].Cells.RemoveAt(startCellIndex);
                        }
                        else
                            //Remove items in row
                            RemoveItems(table.Rows[startRowIndex], startCellIndex, endCellIndex);
                    }
                }
            }
            else
                //Remove items in cell
                RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex], startParaIndex, table.Rows[startRowIndex].Cells[startCellIndex].Items.Count);
        }
        else
        {
            //Remove items in paragraph
            RemoveItems((table.Rows[startRowIndex].Cells[startCellIndex].Items[startParaIndex] as WParagraph), startIndex, (table.Rows[startRowIndex].Cells[startCellIndex].Items[startParaIndex] as WParagraph).Items.Count);
            //Remove items in cell
            RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex], startParaIndex + 1, table.Rows[startRowIndex].Cells[startCellIndex].Items.Count);
            if (startRowIndex == endRowIndex)
                //Remove items in row till end cell index if start and end row index are same.
                RemoveItems(table.Rows[startRowIndex], startCellIndex + 1, endCellIndex);
            else
                //Remove items in row 
                RemoveItems(table.Rows[startRowIndex], startCellIndex + 1, table.Rows[startRowIndex].Cells.Count);
            //Remove items in table
            RemoveItems(table, startRowIndex + 1, endRowIndex);
        }
    }
    /// <summary>
    /// Removes Items present in group at table end
    /// </summary>
    /// <param name="gs">The group selector</param>
    /// <param name="table">Table</param>
    /// <param name="endIndex">End index</param>
    /// <param name="endParaIndex">End para index</param>
    /// <param name="startCellIndex">Start cell index</param>
    /// <param name="endCellIndex">End cell index</param>
    /// <param name="startRowIndex">Start row index</param>
    /// <param name="endRowIndex">End row index</param>
    private void RemoveItemsAtTableEnd(GroupSelector gs, WTable table, int endIndex, int endParaIndex, int startCellIndex, int endCellIndex, int startRowIndex, int endRowIndex)
    {
        if ((startRowIndex != endRowIndex) && (endIndex == (table.Rows[startRowIndex + 1].Cells[endCellIndex].Items[endParaIndex] as WParagraph).Items.Count - 1))
        {
            if (endParaIndex == table.Rows[startRowIndex + 1].Cells[endCellIndex].Items.Count - 1)
            {
                if (endCellIndex == table.Rows[startRowIndex + 1].Cells.Count - 1)
                {
                    //Remove the row if end cell index is the last cell in row
                    table.Rows.RemoveAt(startRowIndex + 1);
                }
                else
                {
                    //Remove items in row
                    RemoveItems(table.Rows[startRowIndex + 1], 0, endCellIndex + 1);
                }
            }
            else
            {
                //Remove items in cell
                RemoveItems(table.Rows[startRowIndex + 1].Cells[endCellIndex], 0, endParaIndex + 1);
                //Remove items in row
                RemoveItems(table.Rows[startRowIndex + 1], 0, endCellIndex);
            }
        }
        else
        {
            //Remove items in paragraph
            RemoveItems((table.Rows[startRowIndex + 1].Cells[endCellIndex].Items[endParaIndex] as WParagraph), 0, endIndex + 1);
            //Remove items in cell
            RemoveItems(table.Rows[startRowIndex + 1].Cells[endCellIndex], 0, endParaIndex);
            //Remove items in row
            RemoveItems(table.Rows[startRowIndex + 1], 0, endCellIndex);
        }
    }
    /// <summary>
    /// Remove items between empty group present in table cell.
    /// </summary>
    /// <param name="gs">The group selector</param>
    private void EmptyGroupInTableCell(GroupSelector gs)
    {
        int startIndex = gs.BeginGroupField.GetIndexInOwnerCollection();
        int startParaIndex = (gs.BeginGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();
        int endIndex = gs.EndGroupField.GetIndexInOwnerCollection();
        int endParaIndex = (gs.EndGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();

        WTableCell cell = gs.BeginGroupField.OwnerParagraph.Owner as WTableCell;
        if ((cell.Items[startParaIndex] as WParagraph).Items.Count > 1 && startIndex > 0)
        {
            //Remove items in paragraph
            RemoveItems((cell.Items[startParaIndex] as WParagraph), startIndex, (cell.Items[startParaIndex] as WParagraph).Items.Count);
            //Remove items in cell
            RemoveItems(cell, startParaIndex + 1, endParaIndex);
            //Remove paragraph if end index is the last item in paragraph
            if (endIndex == (cell.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                cell.Items.RemoveAt(startParaIndex + 1);
            else if ((cell.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
            {
                //Remove items in paragraph
                RemoveItems((cell.Items[startParaIndex + 1] as WParagraph), 0, endIndex);
            }
        }
        else
        {
            //Remove items in cell
            RemoveItems(cell, startParaIndex, endParaIndex);
            if (startParaIndex != endParaIndex)
            {
                //Remove paragraph if end index is the last item in paragraph
                if (endIndex == (cell.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                    cell.Items.RemoveAt(startParaIndex + 1);
                else if ((cell.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
                {
                    //Remove items in paragraph
                    RemoveItems((cell.Items[startParaIndex + 1] as WParagraph), 0, (cell.Items[startParaIndex + 1] as WParagraph).Items.Count + 1);
                }
            }
            else
            {
                //Remove paragraph if end index is the last item in paragraph
                if (endIndex == (cell.Items[startParaIndex] as WParagraph).Items.Count - 1)
                    cell.Items.RemoveAt(startParaIndex);
                else if ((cell.Items[startParaIndex] as WParagraph).Items.Count > 0)
                {
                    //Remove items in paragraph
                    RemoveItems((cell.Items[startParaIndex] as WParagraph), 0, endIndex + 1);
                }
            }
        }
    }
    /// <summary>
    /// Removes items from start index to end index.
    /// </summary>
    /// <param name="ent">Entity</param>
    /// <param name="startIndex">Start index</param>
    /// <param name="endIndex">End index</param>
    private void RemoveItems(Entity ent, int startIndex, int endIndex)
    {
        switch (ent.EntityType)
        {
            case EntityType.Paragraph:
                {
                    for (int i = startIndex; i < endIndex; i++)
                        (ent as WParagraph).Items.RemoveAt(startIndex);
                }
                break;
            case EntityType.TableCell:
                {
                    for (int i = startIndex; i < endIndex; i++)
                        (ent as WTableCell).Items.RemoveAt(startIndex);
                }
                break;
            case EntityType.TableRow:
                {
                    for (int i = startIndex; i < endIndex; i++)
                        (ent as WTableRow).Cells.RemoveAt(startIndex);
                }
                break;
            case EntityType.Table:
                {
                    for (int i = startIndex; i < endIndex; i++)
                        (ent as WTable).Rows.RemoveAt(startIndex);
                }
                break;
            case EntityType.TextBody:
                {
                    for (int i = startIndex; i < endIndex; i++)
                        (ent as WTextBody).Items.RemoveAt(startIndex);
                }
                break;
        }
    }
    /// <summary>
    /// Called when body group found.
    /// </summary>
    /// <param name="rowsEnum">The rows enum.</param>
    private void OnBodyGroupFound( IRowsEnumerator rowsEnum )
    {
      GroupSelector gs = m_groupSelector;

      // Execute for text selection
      TextBodyPart bodyPart = new TextBodyPart();
      TextBodySelection bodySel = gs.GroupSelection;
      bodyPart.Copy( bodySel );
      rowsEnum.Reset();

      while( rowsEnum.NextRow() )
      {
        if( ( m_commands != null && m_commands.Count > 0 ) ||
          ( m_dataSet != null && m_dataSet.DataSet.Count > 0 ) )
        {
          UpdateEnum( gs.GroupName, rowsEnum );
        }
        // Count items number before mail merge.
        int beforeItemsNum = bodySel.TextBody.Items.Count;

        ExecuteGroupForSelection( bodySel.TextBody,
          bodySel.ItemStartIndex, bodySel.ItemEndIndex,
          bodySel.ParagraphItemStartIndex, bodySel.ParagraphItemEndIndex,
          rowsEnum );

        bodySel.ItemEndIndex += bodySel.TextBody.Items.Count - beforeItemsNum;

        if( rowsEnum.IsLast )
        {
          if( m_bIsNested )
          {
            NestedEnums.Remove( gs.GroupName );
          }
          break;
        }

        // Correct item end index
        if (bodySel.ItemStartIndex == bodySel.ItemEndIndex)
            bodyPart.PasteAt(bodySel.TextBody, bodySel.ItemEndIndex, bodySel.ParagraphItemEndIndex + 1);
        else
            bodyPart.PasteAt(bodySel.TextBody, bodySel.ItemEndIndex, bodySel.ParagraphItemEndIndex);

        int pItemEndShift = 0;
        if( bodyPart.BodyItems.Count == 1 && bodyPart.BodyItems[ 0 ] is WParagraph )
          pItemEndShift = ( bodyPart.BodyItems[ 0 ] as WParagraph ).Items.Count - 1;

        bodySel.ShiftStartToEnd( bodyPart.BodyItems.Count - 1, pItemEndShift );
      }
    }
    /// <summary>
    /// Called when row group found.
    /// </summary>
    /// <param name="rowsEnum">The rows enum.</param>
    private void OnRowGroupFound( IRowsEnumerator rowsEnum )
    {
      GroupSelector gs = m_groupSelector;

      // Execute for row selection
      WTable table = gs.RowSelection.Table;
      int startIndex = gs.RowSelection.StartRowIndex;
      int endIndex = gs.RowSelection.EndRowIndex;
      int rowsCntBefore = table.Rows.Count;
      int rowIndex = startIndex;
      int shift = 0;

      if( m_bIsNested )
      {
        VerifyNestedGroups( startIndex, endIndex, table );
      }
      int count = endIndex - startIndex + 1;

      WTableRow[] tableRows = new WTableRow[ count ];

      int index = 0;
      for( int i = startIndex; i <= endIndex; i++ )
      {
        tableRows[ index ] = table.Rows[ i ].Clone();
        index += 1;
      }

      rowsEnum.Reset();

      while( rowsEnum.NextRow() )
      {
        if( ( m_dataSet != null && m_dataSet.DataSet.Count > 0 ) ||
          m_commands != null && m_commands.Count > 0 )
        {
          UpdateEnum( gs.GroupName, rowsEnum );
        }

        shift = ExecuteGroupForRowSelection( table, rowIndex, count, rowsEnum );

        if (rowsEnum.IsLast)
        {
            if (m_bIsNested)
                NestedEnums.Remove(gs.GroupName);
            break;
        }

        rowIndex += shift;
        for( int j = 0; j < count; j++ )
        {
          table.Rows.Insert( rowIndex + j, tableRows[ j ].Clone() );
        }
      }

      gs.RowSelection.StartRowIndex = rowIndex;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowsEnum"></param>
    private void ExecuteGroup( IRowsEnumerator rowsEnum )
    {
      Document.IsMailMerge = true;
      RemoveSpellChecking();

      WSection sec = null;
      for( int i = 0, cnt = Document.Sections.Count; i < cnt; i++ )
      {
        sec = Document.Sections[ i ];
        ExecuteGroup( sec, rowsEnum );
      }

      Document.IsMailMerge = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="section"></param>
    /// <param name="rowsEnum"></param>
    private void ExecuteGroup( WSection section, IRowsEnumerator rowsEnum )
    {
      m_groupSelector.ProcessGroups( section.Body, rowsEnum );

      // Executes for headers / footers
      for( int i = 0; i < 6; i++ )
      {
        WTextBody headerFooter = section.HeadersFooters[ i ];

        if( headerFooter.Items.Count > 0 )
        {
          m_groupSelector.ProcessGroups( headerFooter, rowsEnum );
        }
      }
    }
    /// <summary>
    /// Executes the group for selection.
    /// </summary>
    /// <param name="textBody">The text body.</param>
    /// <param name="itemStart">The item start.</param>
    /// <param name="itemEnd">The item end.</param>
    /// <param name="pItemStart">The p item start.</param>
    /// <param name="pItemEnd">The p item end.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void ExecuteGroupForSelection( WTextBody textBody, int itemStart, int itemEnd, int pItemStart, 
      int pItemEnd, IRowsEnumerator rowsEnum )
    {
      if( itemEnd < 0 )
      {
        itemEnd = textBody.Items.Count - 1;
      }

      for( int i = itemStart, end = itemEnd; i <= end; i++ )
      {
        TextBodyItem bodyItem = textBody.Items[ i ];

        switch( bodyItem.EntityType )
        {
          case EntityType.Table:
            {
              WTable table = bodyItem as WTable;

              for( int j = 0, len = table.Rows.Count; j < len; j++ )
              {
                ExecuteGroupForRowSelection( table, j, 1, rowsEnum );
              }
            }
            break;
          case EntityType.Paragraph:
            {
              WParagraph para = bodyItem as WParagraph;

              int start = ( i == itemStart ) ? pItemStart : 0;
              int endj = ( i == end && pItemEnd > -1 ) ? pItemEnd : para.Items.Count - 1;

              for( int j = start; j <= endj; j++ )
              {
                WField field = para.Items[ j ] as WField;
                if( field == null )
                  continue;

                if( field is WMergeField )
                {
                  WMergeField mergeField = field as WMergeField;
                  if( !IsBeginGroup( mergeField ) && !IsEndGroup( mergeField ) )
                  {
                    if( mergeField.Prefix.StartsWith( "Image" ) )
                    {
                      UpdateImageFieldValue( mergeField, para, rowsEnum );
                    }
                    else
                    {
                      UpdateFieldValue( mergeField, rowsEnum );
                    }
                  }
                  else if( m_bIsNested )
                  {
                    if( IsBeginGroup( mergeField ) && !NestedEnums.ContainsKey( mergeField.FieldName ) )
                    {
                      // Process nested groups
                      string tableName = mergeField.FieldName;

                      if( tableName == string.Empty )
                        continue;

                      if (mergeField.Prefix == "TableStart")
                      {
                          ClearFieldsState.Add(tableName, ClearFields);
                          ClearFields = false;
                          continue;
                      }
                      // Get the new table for mail merge and get enumerator for that table.
                      // GetEnum method also adds created (just filled) table to the current dataset
                      IRowsEnumerator tblRowsEnum = GetEnum( tableName );
                      if( tblRowsEnum == null )
                        continue;

                      int beforeItemsCount = textBody.Items.Count;
                      GroupSelectors.Push( m_groupSelector );

                      m_groupSelector = new GroupSelector( new GroupSelector.GroupFound( OnGroupFound ) );
                      m_groupSelector.ProcessGroups( textBody, tblRowsEnum );
                      // Define the number of selected items
                      int selItems = m_groupSelector.SelectedBodyItemsCount;
                      if( selItems == -1 )
                      {
                        throw new Exception( "Group \"" + tableName + "\" is missing in the source document." );
                      }
                      else if( selItems > 0 )
                      {
                        // Define shift made by mail merge
                        int shift = textBody.Items.Count - beforeItemsCount;
                        i += shift + selItems - 1;
                        end += shift;
                        itemEnd = end;
                      }
                      else
                      {
                        HideField( mergeField );
                      }
                      // Remove filled before table from 
                      CurrentDataSet.RemoveDataTable( tableName );
                      // Pop previous GroupSelector with selected before items
                      m_groupSelector = GroupSelectors.Pop();
                      break;
                    }
                  }
                  else if ((IsBeginGroup(mergeField) || IsEndGroup(mergeField))
                      && m_bClearFields)
                  {
                      HideField(mergeField);
                  }
                }
                else if( field is WIfField )
                {
                  UpdateIfFieldValue( field as WIfField, rowsEnum );
                }
                else if (field.FieldType == FieldType.FieldNext)
                {
                    if (rowsEnum != null && !rowsEnum.IsEnd)
                    {
                        rowsEnum.NextRow();
                    }
                    // Erase NEXT field
                    HideField(field);
                }
                else if (field.FieldType == FieldType.FieldNextIf)
                {
                    if (field.UpdateNextIfField()
                        && rowsEnum != null
                        && !rowsEnum.IsEnd)
                        rowsEnum.NextRow();
                    // Removes NEXTIF field
                    para.Items.Remove(field);
                    endj = para.Items.Count - 1;
                    j--;
                }
                else if (field.FieldType == FieldType.FieldMergeRec
                    || field.FieldType == FieldType.FieldMergeSeq)
                {
                    // Converts MergeSequence and MergeRecord field to text.
                    int mergeRecordCount = 1;
                    if (rowsEnum != null)
                        mergeRecordCount += rowsEnum.CurrentRowIndex;
                    ConvertToText(field, mergeRecordCount.ToString());
                }
              }

              if( m_bRemoveEmptyPara )
              {
                RemoveEmptyPara( para );
              }
            }
            break;
        }
      }
    }
    /// <summary>
    /// Executes the group for row selection.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="startRowIndex">Start index of the row.</param>
    /// <param name="count">The count.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    /// <returns>The number of rows that took part in mail merge</returns>
    private int ExecuteGroupForRowSelection( WTable table, int startRowIndex, int count, IRowsEnumerator rowsEnum )
    {
      int rowsCntBefore = table.Rows.Count;
      int endRowIndex = startRowIndex + count - 1;
      WTableRow row = null;
      WTableCell cell = null;

      for( int j = startRowIndex; j <= endRowIndex; j++ )
      {
        row = table.Rows[ j ];
        for( int i = 0, cnt = row.Cells.Count; i < cnt; i++ )
        {
          cell = row.Cells[ i ];
          ExecuteGroupForSelection( cell, 0, -1, 0, -1, rowsEnum );
        }
      }

      int shift = 0;
      if( m_bIsNested )
      {
        // Execute nested 
        string nestedGr = FindNestedGroup( startRowIndex, endRowIndex, table );
        int rowsCnt = table.Rows.Count;
        if (nestedGr != null
            && ClearFieldsState.ContainsKey(nestedGr))
        {
            ClearFields = ClearFieldsState[nestedGr];
            ClearFieldsState.Remove(nestedGr);
        }
        while( nestedGr != null )
        {
          shift = table.Rows.Count - rowsCnt;
          endRowIndex += shift;
          startRowIndex += shift;
          rowsCnt = table.Rows.Count;
          IRowsEnumerator tblRowsEnum = GetEnum( nestedGr );
          if( tblRowsEnum != null )
          {
            // Store current group selector
            GroupSelectors.Push( m_groupSelector );
            // Create new one based new row enumerator
            m_groupSelector = new GroupSelector( new GroupSelector.GroupFound( OnGroupFound ) );
            m_groupSelector.ProcessGroups( table, startRowIndex, endRowIndex, tblRowsEnum );

            CurrentDataSet.RemoveDataTable( nestedGr );
            // Pop previous GroupSelector with selected before items
            m_groupSelector = GroupSelectors.Pop();
          }

          // Get nested group name and check whether it was already mail merged.
          string prevNestedGr = nestedGr;
          nestedGr = FindNestedGroup( startRowIndex, endRowIndex, table );
          if( prevNestedGr == nestedGr )
            break;
        }
      }

      shift = table.Rows.Count - rowsCntBefore;
      return count + shift;
    }
    #endregion

    #region Implementation / nested
    /// <summary>
    /// Executes the nested group.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    private void ExecuteNestedGroup( string tableName )
    {
      IRowsEnumerator rowsEnum = GetEnum( tableName );

      if( rowsEnum == null )
        return;

      WSection section = null;
      for( int i = 0, cnt = Document.Sections.Count; i < cnt; i++ )
      {
        section = Document.Sections[ i ];
        ExecuteGroup( section, rowsEnum );
      }
      UpdateMergedRecordCount();
      // Removes temporary filled table from current dataset.
      CurrentDataSet.RemoveDataTable( tableName );
    }
    /// <summary>
    /// Gets the enumerator for nested mail merge.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns></returns>
    private IRowsEnumerator GetEnum( string tableName )
    {
      MailMergeDataTable table = GetDataTable( tableName );
      if( table == null )
        return null;

      CurrentDataSet.Add( table );
      IRowsEnumerator rowsEnum = new DataTableEnumerator( table );
      rowsEnum.Reset();

      return rowsEnum;
    }
    /// <summary>
    /// Updates the enum.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void UpdateEnum( string tableName, IRowsEnumerator rowsEnum )
    {
      if( !NestedEnums.ContainsKey( tableName ) )
      {
        NestedEnums.Add( tableName, rowsEnum );
      }
      else
      {
        NestedEnums[ tableName ] = rowsEnum;
      }
    }
    /// <summary>
    /// Gets the data table.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="commands">The commands.</param>
    /// <returns></returns>
    private MailMergeDataTable GetDataTable( string tableName )
    {
      MailMergeDataTable table = m_dataSet.GetDataTable( tableName );
      if( table == null )
        return null;

      string command = GetCommand( tableName );
      if( command == string.Empty )
      {
        return table;
      }
      else
      {
        return table.Select( command );
      }
    }
    /// <summary>
    /// Gets the command.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="commands">The commands.</param>
    /// <returns></returns>
    private string GetCommand( string tableName )
    {
      DictionaryEntry entry = new DictionaryEntry( string.Empty, string.Empty );
      bool found = false;
      for( int i = 0, cnt = m_commands.Count; i < cnt; i++ )
      {
        entry = ( DictionaryEntry )m_commands[ i ];
        if( tableName == ( string )entry.Key )
        {
          found = true;
          break;
        }
      }

      // If entry with specified table name is found, get the command.
      if( found )
      {
        string command = ( string )entry.Value;
        if( command.IndexOf( "%" ) == -1 )
        {
          return command;
        }
        else
        {
          return UpdateVarCmd( command );
        }
      }
      return null;
    }
    /// <summary>
    /// Updates the variable part of the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns></returns>
    private string UpdateVarCmd( string command )
    {
      MatchCollection matches = VariableCommandRegex.Matches( command );
      if( matches.Count == 0 )
        return null;

      char[] splitter = new char[ 1 ] { '.' };
      string newCmd = null;
      string varCmd = null;
      string[] cmdParts = null;

      for( int i = 0, cnt = matches.Count; i < cnt; i++ )
      {
        varCmd = matches[ i ].Value.Replace( "%", string.Empty );
        cmdParts = varCmd.Split( splitter );
        if( cmdParts.Length != 2 )
          throw new ArgumentException( "String value between '%' symbols (variable command) is not valid." );

        IRowsEnumerator rowsEnum = null;
        if( NestedEnums.ContainsKey( cmdParts[ 0 ] ) )
        {
          rowsEnum = NestedEnums[ cmdParts[ 0 ] ];
        }

        if( rowsEnum == null )
          return string.Empty;

        newCmd = ( rowsEnum.GetCellValue( cmdParts[ 1 ] ) ).ToString();
        command = command.Replace( "%" + varCmd + "%", newCmd );
      }

      return command;
    }
    /// <summary>
    /// Checks the table nested groups.
    /// </summary>
    /// <param name="startRow">The start row.</param>
    /// <param name="endRow">The end row.</param>
    /// <param name="table">The table.</param>
    private void VerifyNestedGroups( int startRow, int endRow, WTable table )
    {
      Dictionary<string, WMergeField> startGrNames = new Dictionary<string, WMergeField>();
      Dictionary<string, WMergeField> endGrNames = new Dictionary<string, WMergeField>();

      WTableRow row = null;
      WMergeField mergeFld = null;
      for( int i = startRow; i <= endRow; i++ )
      {
        row = table.Rows[ i ] as WTableRow;
        foreach( WTableCell cell in row.Cells )
          foreach( WParagraph par in cell.Paragraphs )
            foreach( ParagraphItem item in par.Items )
              if( item is WMergeField )
              {
                mergeFld = item as WMergeField;
                if( IsBeginGroup( mergeFld ) && !mergeFld.ConvertedToText )
                  startGrNames.Add( mergeFld.FieldName, mergeFld );
                else if( IsEndGroup( mergeFld ) && !mergeFld.ConvertedToText )
                  endGrNames.Add( mergeFld.FieldName, mergeFld );
              }
      }

      // Check whether there are only start or end groups
      if( startGrNames.Count == 0 )
      {
        if( endGrNames.Count > 0 )
          foreach( string key in endGrNames.Keys )
            //ClearGroupFld( entry.Value as WMergeField );
            throw new Exception( "GroupEnd field \"" + key + "\" doesn't have GroupStart field equivalent." );
      }
      else if( endGrNames.Count == 0 )
      {
        if( startGrNames.Count > 0 )
          foreach( string key in startGrNames.Keys )
            //ClearGroupFld( entry.Value as WMergeField );
            throw new Exception( "GroupStart field \"" + key + "\" doesn't have GroupEnd field equivalent." );
      }

      // Chack whether there is equivalent for start group in end groups collection.
      foreach( string startGrName in startGrNames.Keys )
      {
        if( !endGrNames.ContainsKey( startGrName ) )
          //ClearGroupFld( dEntry.Value as WMergeField );
          throw new Exception( "GroupStart field \"" + startGrName + "\" doesn't have GroupEnd field equivalent." );
        else
          // If there is an equivalent in endGroupName, remove equivalent field from collection.
          endGrNames.Remove( startGrName );
      }
      // Hide invalid group fields 
      if( endGrNames.Count > 0 )
      {
        foreach( string key in endGrNames.Keys )
        {
          //ClearGroupFld( ent.Value as WMergeField );
          throw new Exception( "GroupEnd field \"" + key + "\" doesn't have GroupStart field equivalent." );
        }
      }

      startGrNames.Clear();
      endGrNames.Clear();
    }
    /// <summary>
    /// Clears the group FLD.
    /// </summary>
    /// <param name="field">The field.</param>
    private void ClearGroupFld( WMergeField field )
    {
      HideField( field );
      field.FieldName = string.Empty;
    }
    /// <summary>
    /// Finds the table groups.
    /// </summary>
    /// <param name="startRow">The start row.</param>
    /// <param name="endRow">The end row.</param>
    /// <param name="table">The table.</param>
    /// <returns>The founded nested group name.</returns>
    private string FindNestedGroup(int startRow, int endRow, WTable table)
    {
        WTableRow row = null;
        for (int i = startRow; i <= endRow; i++)
        {
            row = table.Rows[i];
            foreach (WTableCell cell in row.Cells)
                foreach (WParagraph par in cell.Paragraphs)
                    foreach (ParagraphItem item in par.Items)
                        if (item is WMergeField)
                        {
                            WMergeField mergeFld = item as WMergeField;
                            if (IsBeginGroup(mergeFld) && mergeFld.FieldName != string.Empty)
                            {
                                string grName = (item as WMergeField).FieldName;
                                return (grName == string.Empty) ? null : grName;
                            }
                        }
        }

        return null;
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Sends MergeField event.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    /// <returns></returns>
    protected MergeFieldEventArgs SendMergeField( IWMergeField field, object value, IRowsEnumerator rowsEnum )
    {
      MergeFieldEventArgs args = new MergeFieldEventArgs(
        Document as IWordDocument,
        rowsEnum.TableName, rowsEnum.CurrentRowIndex,
        field, value
        );

      if( MergeField != null )
      {
        MergeField( this, args );
      }

      return args;
    }
    /// <summary>
    /// 
    /// </summary>
    protected MergeImageFieldEventArgs SendMergeImageField( IWMergeField field, object bmp, 
      IRowsEnumerator rowsEnum )
    {
      MergeImageFieldEventArgs args = null;
      if( rowsEnum != null )
      {
        args = new MergeImageFieldEventArgs(
          Document as IWordDocument,
          rowsEnum.TableName, rowsEnum.CurrentRowIndex,
          field, bmp
          );
      }
      else
      {
        args = new MergeImageFieldEventArgs(
          Document as IWordDocument, null, int.MaxValue, field, bmp
          );
      }

      if( MergeImageField != null )
      {
        MergeImageField( this, args );
      }

      return args;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowsEnum"></param>
    private void Execute( IRowsEnumerator rowsEnum )
    {
      Document.IsMailMerge = true;
      RemoveSpellChecking();

      if( rowsEnum == null )
        throw new ArgumentNullException( "rowsEnum" );

      int rowsCount = rowsEnum.RowsCount;
      int lastSecIndex = 0;

      if( rowsCount > 1 )
      {
        CopyContent( Document );
      }

      IWSectionCollection sections = Document.Sections;
      rowsEnum.Reset();

      if( rowsEnum.RowsCount == 0 && m_bClearFields )
      {
        for( int j = 0, len = sections.Count; j < len; j++ )
        {
          ExecuteForSection( sections[ j ], null );
        }
      }
      else
      {
        while( rowsEnum.NextRow() )
        {
          for( int j = lastSecIndex, len = sections.Count; j < len; j++ )
          {
            ExecuteForSection( sections[ j ], rowsEnum );
          }

          lastSecIndex = sections.Count;
          if( !rowsEnum.IsLast )
          {
            // Begin from second row - appends copied sections
            AppendCopiedContent( Document );
          }
        }
      }
      Document.IsMailMerge = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="rowsEnum"></param>
    private void ExecuteForSection( IWSection sec, IRowsEnumerator rowsEnum )
    {
        ExecuteForTextBody(sec.Body.Items, rowsEnum);

      // Executes for headers/footers
      for( int i = 0; i < 6; i++ )
      {
        BodyItemCollection paragraphs = ( BodyItemCollection )sec.HeadersFooters[ i ].ChildEntities;

        if( paragraphs.Count > 0 )
        {
            ExecuteForTextBody(paragraphs, rowsEnum);
        }
      }
    }
    /// <summary>
    /// Executes for text body.
    /// </summary>
    /// <param name="bodyItems">The body items.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void ExecuteForTextBody(BodyItemCollection bodyItems, IRowsEnumerator rowsEnum)
    {
        ITextBodyItem item = null;
        for (int index = 0; index < bodyItems.Count; index++)
        {
            item = bodyItems[index];

            //Execute merge field names from textbody item.
            ExecuteForTextBodyItem(item, rowsEnum);

        }
    }    
    /// <summary>
    /// Executes for text body item.
    /// </summary>
    /// <param name="bodyItems">The body items.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void ExecuteForTextBodyItem(ITextBodyItem item, IRowsEnumerator rowsEnum)
    {
        if (item is IWParagraph)
        {
            WParagraph paragraph = item as WParagraph;
            ExecuteForParagraph(paragraph, rowsEnum);
        }
        else if (item is IWTable)
        {
            IWTable table = item as IWTable;

            if (table != null)
            {
                ExecuteForTable(table, rowsEnum);
            }
        }
        else if (item is StructureDocumentTagBlock)
        {
            foreach (ITextBodyItem textBodyItem in (item as StructureDocumentTagBlock).SDTContent.TextBody.ChildEntities)
            {
                //Iterates the child items of SDTInlineTextBodyItem.
                ExecuteForTextBodyItem(textBodyItem, rowsEnum);
            }
        }
    }

    /// <summary>
    /// Executes for paragraph.
    /// </summary>
    /// <param name="paragraph">The paragraph.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void ExecuteForParagraph(WParagraph paragraph, IRowsEnumerator rowsEnum)
    {
        bool paraItemCollectionChanged = false;
        for (int i = 0, len = paragraph.Items.Count; i < len; i++)
        {
            ParagraphItem pItem = paragraph[i];

            //Execute  merge field names from paragraph item.
            ExecuteForParagraphItems(pItem, paragraph, rowsEnum, ref paraItemCollectionChanged);

            if (paraItemCollectionChanged)
            {
                len = paragraph.Items.Count;
                i--;
                paraItemCollectionChanged = false;
            }
        }

        if (m_bRemoveEmptyPara)
        {
            RemoveEmptyPara(paragraph);
        }
    }
    /// <summary>
    /// Executes for paragraph items.
    /// </summary>
    /// <param name="paragraph">The paragraph.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    /// <param name="pItem">The Paragraph item.</param>
    /// <param name="paraItemCollectionChanged">The indicate whether the paragraph item collection changed or not.</param>
    private void ExecuteForParagraphItems(ParagraphItem pItem, WParagraph paragraph, IRowsEnumerator rowsEnum, ref bool paraItemCollectionChanged)
    {
        WMergeField mergeField = pItem as WMergeField;

        if (mergeField != null)
        {
            if (mergeField.Prefix.StartsWith("Image"))
            {
                UpdateImageFieldValue(mergeField, paragraph, rowsEnum);
            }
            else
            {
                if (!mergeField.ConvertedToText)
                    UpdateFieldValue(mergeField, rowsEnum);
            }
        }
        else if (pItem is WField)
        {
            WField field = pItem as WField;
            if (field.FieldType == FieldType.FieldNext)
            {
                if (rowsEnum != null && !rowsEnum.IsEnd)
                {
                    rowsEnum.NextRow();
                }
                // Erase NEXT field
                HideField(field);
            }
            else if (field.FieldType == FieldType.FieldNextIf)
            {
                if (field.UpdateNextIfField()
                    && rowsEnum != null
                    && !rowsEnum.IsEnd)
                    rowsEnum.NextRow();
                // Removes NEXTIF field
                paragraph.Items.Remove(field);
                paraItemCollectionChanged = true;
            }
            else if (field.FieldType == FieldType.FieldIf)
            {
                UpdateIfFieldValue(field as WIfField, rowsEnum);
            }
            else if (field.FieldType == FieldType.FieldMergeRec
                || field.FieldType == FieldType.FieldMergeSeq)
            {
                // Converts MergeSequence and MergeRecord field to text.
                int mergeRecordCount = 1;
                if (rowsEnum != null)
                    mergeRecordCount += rowsEnum.CurrentRowIndex;
                ConvertToText(field, mergeRecordCount.ToString());
            }
        }
        else if (pItem is WTextBox)
        {
            WTextBox textBox = pItem as WTextBox;
            ExecuteForTextBody((BodyItemCollection)textBox.TextBoxBody.ChildEntities, rowsEnum);
        }
        else if (pItem is Shape)
        {
            Shape shape = pItem as Shape;
            ExecuteForTextBody((BodyItemCollection)shape.TextBody.ChildEntities, rowsEnum);
        }
        else if (pItem is StructureDocumentTagInline)
        {
            foreach (ParagraphItem paraItem in (pItem as StructureDocumentTagInline).SDTContent.ParagraphItems)
            {
                //Iterates the child items of StructureDocumentTagInline.
                ExecuteForParagraphItems(paraItem, paragraph, rowsEnum, ref paraItemCollectionChanged);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <param name="rowsEnum"></param>
    private void ExecuteForTable( IWTable table, IRowsEnumerator rowsEnum )
    {
      WTableRow tableRow = null;
      WTableCell cell = null;
      for( int i = 0, rowLen = table.Rows.Count; i < rowLen; i++ )
      {
        tableRow = table.Rows[ i ];
        for( int j = 0, cellLen = tableRow.Cells.Count; j < cellLen; j++ )
        {
          cell = tableRow.Cells[ j ];
          ExecuteForTextBody((BodyItemCollection)cell.ChildEntities, rowsEnum);
        }
      }
    }
    /// <summary>
    /// Converts to text.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="text">The text.</param>
    private void ConvertToText(WField field, string text)
    {
        WTextRange textRange = new WTextRange(Document);

        WParagraph paragraph = field.OwnerParagraph;
        int index = field.GetIndexInOwnerCollection();
        paragraph.Items.Remove(field);
        paragraph.Items.Insert(index, textRange);
        // Updates the character format.
        textRange.CharacterFormat.ImportContainer(field.CharacterFormat);
        textRange.CharacterFormat.CopyProperties(field.CharacterFormat);
        if (textRange.CharacterFormat.Sprms.HasSprm(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec))
            textRange.CharacterFormat.Sprms.RemoveValue(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec);
        // Updates the text.
        textRange.Text = text;
    }
    /// <summary>
    /// Updates the merged record count.
    /// </summary>
    private void UpdateMergedRecordCount()
    {
        for (int i = 0; i < Document.Fields.Count; i++)
        {
            WField field = Document.Fields[i];
            if (field.FieldType == FieldType.FieldMergeRec
                || field.FieldType == FieldType.FieldMergeSeq)
            {
                // Updates MergeSequence and MergeRecord field.
                ConvertToText(field, m_mergedRecordCount.ToString());
                i--;
            }
        }
    }
    /// <summary>
    /// Updates the field value.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void UpdateFieldValue( IWMergeField field, IRowsEnumerator rowsEnum )
    {
      if( rowsEnum == null )
      {
        UpdateFieldValue( field );
      }
      else
      {
        string columnName = null;
        bool fieldUpdated = false;
        object value = null;

        // Get column name for the collection of mapped fields
        columnName = GetMappedColName( field.FieldName );
        if( columnName != null )
        {
          value = rowsEnum.GetCellValue( columnName );
        }

        if( value == null )
        {
          for( int i = 0, len = rowsEnum.ColumnNames.Length; i < len; i++ )
          {
            columnName = rowsEnum.ColumnNames[ i ];
            string fieldNameUpper = field.FieldName.ToUpper();
            string columnNameUpper = columnName.ToUpper();

            if( fieldNameUpper == columnNameUpper
              || fieldNameUpper == "\"" + columnNameUpper + "\"" )
            {
              // If there is no more data then stop merging
              value = rowsEnum.GetCellValue( columnName );
              break;
            }
          }
        }

        if( value != null )
        {
          if( !( field as WMergeField ).ConvertedToText )
          {
            // Send MergeField event
            MergeFieldEventArgs args = SendMergeField( field, value, rowsEnum );
            field.Text = args.Text;
            ( field as WMergeField ).ConvertedToText = true;
          }
          fieldUpdated = true;
        }

        if( ( !fieldUpdated && m_bClearFields ) )
        {
          HideField( field );
        }
      }
    }
    /// <summary>
    /// Updates if field value.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="rowsEnum">The rows enum.</param>
    private void UpdateIfFieldValue( WIfField field, IRowsEnumerator rowsEnum )
    {
      if( field.MergeFields.Count == 0 )
        return;

      if( rowsEnum == null )
        return;

      string columnName = null;
      for( int i = 0, len = rowsEnum.ColumnNames.Length; i < len; i++ )
      {
        columnName = rowsEnum.ColumnNames[ i ];
        string columnNameUpper = columnName.ToUpper();
        string fieldNameUpper = string.Empty;
        PseudoMergeField mergeField = null;

        for( int j = 0, cnt = field.MergeFields.Count; j < cnt; j++ )
        {
          mergeField = field.MergeFields[ j ];
          if( mergeField.Name == null )
            continue;

          fieldNameUpper = mergeField.Name.ToUpper();
          if( fieldNameUpper == columnNameUpper )
          {
            // If there is no more data then stop merging
            object value = rowsEnum.GetCellValue( columnName );
            mergeField.Value = value.ToString();
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="field"></param>
    /// <param name="paragraph"></param>
    /// <param name="rowsEnum"></param>
    private void UpdateImageFieldValue( IWMergeField field, IWParagraph paragraph,
      IRowsEnumerator rowsEnum )
    {
      if( rowsEnum == null && MergeImageField == null )
      {
        UpdateFieldValue( field );
      }
      else
      {
        MergeImageFieldEventArgs args = null;
        if( rowsEnum == null && MergeImageField != null )
        {
            string fieldName = field.FieldName;
            object value = null;
            for (int i = 0; i < m_names.Length; i++)
            {
                if (fieldName != null)
                {
                    if (m_names[i].ToUpper() == fieldName.ToUpper())
                    {
                        value = m_values[i];
                        break;
                    }
                }
            }
            if (value != null)
            {
                Image img = GetImage(value);
                if (img != null)
                {
                    value = img;
                }
            }
            // Send MergeField event
            args = SendMergeImageField(field, value, rowsEnum);
            UpdateMergedPicture(field, paragraph, args);
        }
        else
        {
          bool fieldUpdated = false;
          string fieldName = field.FieldName;
          string columnName = null;
          object value = null;

          // Get column name from the collection of mapped fields 
          columnName = GetMappedColName( fieldName );
          if( columnName != null )
          {
            value = rowsEnum.GetCellValue( columnName );
          }

          if( value == null )
          {
            for( int i = 0, len = rowsEnum.ColumnNames.Length; i < len; i++ )
            {
              columnName = rowsEnum.ColumnNames[ i ];
              if( columnName.ToUpper() == fieldName.ToUpper() )
              {
                value = rowsEnum.GetCellValue( columnName );
                break;
              }
            }
          }

          if( value != null )
          {
            Image img = GetImage( value );
            if( img != null )
            {
              value = img;
            }
            // Send MergeField event
            args = SendMergeImageField( field, value, rowsEnum );
            UpdateMergedPicture( field, paragraph, args );
            fieldUpdated = true;
          }

          if( !fieldUpdated && m_bClearFields )
          {
            HideField( field );
          }
        }
      }
    }
    /// <summary>
    /// Updates the merged picture.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="paragraph">The paragraph.</param>
    /// <param name="args">The <see cref="Syncfusion.DocIO.DLS.MergeImageFieldEventArgs"/> instance containing the event data.</param>
    private void UpdateMergedPicture( IWMergeField field, IWParagraph paragraph, MergeImageFieldEventArgs args )
    {
      if( args.UseText )
      {
        field.Text = args.Text;
      }
      else
      {
          if (!args.Skip)
          {
              IWPicture picture;
              int index = paragraph.Items.IndexOf(field);
              paragraph.Items.RemoveAt(index);
              picture = (IWPicture)Document.CreateParagraphItem(ParagraphItemType.Picture);
              paragraph.Items.Insert(index, picture);
              if (args.Image != null)
                  picture.LoadImage(args.Image.ImageData);
          }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="field"></param>
    private void UpdateFieldValue( IWMergeField field )
    {
      if( m_bClearFields && m_values == null )
      {
        field.Text = "";
        ( field as WMergeField ).ConvertedToText = true;
      }
      else
      {
        int valIndex = -1;
        string name = GetMappedColName( field.FieldName );
        // Find value index according to the name.
        if( name != null )
        {
          for( int j = 0; j < m_names.Length; j++ )
          {
            if( m_names[ j ].ToUpper() == name.ToUpper() )
            {
              valIndex = j;
              break;
            }
          }
        }

        if( valIndex == -1 )
        {
          for( int i = 0; i < m_names.Length; i++ )
          {
            if( m_names[ i ].ToUpper() == field.FieldName.ToUpper() )
            {
              valIndex = i;
              break;
            }
          }
        }

        if( valIndex != -1 )
        {
          // throwing event 
          MergeFieldEventArgs args = new MergeFieldEventArgs(
            Document as IWordDocument,
            "", valIndex,
            field, m_values[ valIndex ]
            );

          if( MergeField != null )
          {
            MergeField( this, args );
          }

          field.Text = args.Text;
          ( field as WMergeField ).ConvertedToText = true;
        }
        else if( m_bClearFields )
        {
          HideField( field );
        }
      }
    }
    /// <summary>
    /// Copies all document sections to clipboard. 
    /// </summary>
    /// <param name="document"></param>
    private void CopyContent( WordDocument document )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      m_contentSections.Clear();
      document.Sections.CloneTo( m_contentSections );
    }
    /// <summary>
    /// Appends copied sections to end of document. 
    /// </summary>
    /// <param name="document"></param>
    private void AppendCopiedContent( WordDocument document )
    {
      if( document == null )
        throw new ArgumentNullException( "document" );

      IWSection sec = null;
      for( int i = 0, cnt = m_contentSections.Count; i < cnt; i++ )
      {
        sec = m_contentSections[ i ];
        document.Sections.Add( sec.Clone() );
      }
    }
    /// <summary>
    /// Gets the bitmap.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    private Image GetImage( object data )
    {
      Type objType = data.GetType();
      if( objType == typeof( byte[] ) )
      {
        MemoryStream memStream = new MemoryStream( ( byte[] )data );
        try
        {
          return new Image( memStream );
        }
        catch
        {
          return null;
        }
      }
      else if( objType == typeof( WPicture ) )
      {
        WPicture picture = ( WPicture )data;
        return picture.Image;
      }
      else if( objType == typeof( Image ) )
      {
        return data as Image;
      }

      return null;
    }
    /// <summary>
    /// Gets the field names for paragraph.
    /// </summary>
    /// <param name="fieldsArray">The fields array.</param>
    /// <param name="paragraph">The paragraph.</param>
    /// <param name="groupName">Name of the group.</param>
    private void GetFieldNamesForParagraph( List<string> fieldsArray, TextBodyItem paragraph, string groupName )
    {
      if (paragraph is StructureDocumentTagBlock)
        {
            GetFiledNamesForSDTBlockItems(fieldsArray, paragraph as StructureDocumentTagBlock, groupName);
            return;
        }
        if (paragraph is IWTable)
        {
            WTable table = paragraph as WTable;
            WTableRow row = null;
            WTableCell cell = null;
            TextBodyItem cellItem = null;

            for (int rowIndex = 0, rowCnt = table.Rows.Count; rowIndex < rowCnt; rowIndex++)
            {
                row = table.Rows[rowIndex];
                for (int cellIndex = 0, cellCnt = row.Cells.Count; cellIndex < cellCnt; cellIndex++)
                {
                    cell = row.Cells[cellIndex];
                    for (int parIndex = 0, parCnt = cell.ChildEntities.Count; parIndex < parCnt; parIndex++)
                    {
                        cellItem = cell.Items[parIndex];// Paragraphs[ parIndex ];
                        GetFieldNamesForParagraph(fieldsArray, cellItem, groupName);
                    }
                }
            }
        }
        else
        {
            for (int i = 0, count = (paragraph as WParagraph).Items.Count; i < count; i++)
            {
                ParagraphItem pItem = (paragraph as WParagraph)[i];

                //Gets merge field names from paragraph item.
                GetFieldNamesForParagraphItems(fieldsArray, pItem, groupName);

            }
        }
    }
    /// <summary>
    /// Gets the field names from paragraph item.
    /// </summary>
    ///<param name="fieldsArray">The fields array.</param>
    /// <param name="pStructureDocumentTagBlocklockContent">Paragraph item</param>
    /// <param name="groupName">Name of the group.</param>
    private void GetFieldNamesForParagraphItems(List<string> fieldsArray, ParagraphItem item, string groupName)
    {
        if (item is WMergeField)
        {
            WMergeField mField = (item as WMergeField);

            if (mField.FieldName == groupName)
            {
                if (!m_bBeginGroupFound && IsBeginGroup(mField))
                {
                    m_bBeginGroupFound = true;
                    m_bEndGroupFound = false;

                }
                if (!m_bEndGroupFound && IsEndGroup(mField))
                {
                    m_bEndGroupFound = true;
                    m_bBeginGroupFound = false;
                }
            }
            else if (groupName == null || (m_bBeginGroupFound && !m_bEndGroupFound))
            {
                fieldsArray.Add(mField.FieldName);
            }
        }
        else if (item is WTextBox)
        {
            WTextBox textBox = item as WTextBox;
            int tbxCount = textBox.TextBoxBody.Items.Count;
            for (int index = 0; index < tbxCount; index++)
            {
                GetFieldNamesForParagraph(fieldsArray, textBox.TextBoxBody.Items[index], groupName);
            }
        }
        else if (item is Shape)
        {
            Shape shape = item as Shape;
            int tbxCount = shape.TextBody.Items.Count;
            for (int index = 0; index < tbxCount; index++)
            {
                GetFieldNamesForParagraph(fieldsArray, shape.TextBody.Items[index], groupName);
            }
        }
        else if (item is StructureDocumentTagInline)
        {
            foreach (ParagraphItem paraItem in (item as StructureDocumentTagInline).SDTContent.ParagraphItems)
            {
                //Iterates the child items of StructureDocumentTagInline.
                GetFieldNamesForParagraphItems(fieldsArray, paraItem, groupName);
            }
        }

    }
    /// <summary>
    /// Iterate StructureDocumentTagBlock items to get the fields name. 
    /// </summary>
    ///<param name="pFieldsArray">The fields array.</param>
    /// <param name="structureDocumentTagBlocklockContent">StructureDocumentTagBlocklockContent</param>
    /// <param name="groupName">Name of the group.</param>
    private void GetFiledNamesForSDTBlockItems(List<string> fieldsArray, StructureDocumentTagBlock structureDocumentTagBlocklockContent, string groupName)
    {
        for (int i = 0; i < structureDocumentTagBlocklockContent.SDTContent.TextBody.ChildEntities.Count; i++)
        {
            GetFieldNamesForParagraph(fieldsArray, structureDocumentTagBlocklockContent.SDTContent.TextBody.ChildEntities[i] as TextBodyItem, groupName);
        }

    }



    /// <summary>
    /// Determines whether [is start prefix] [the specified field].
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    /// 	<c>true</c> if it specifies a begining of the group field, set to <c>true</c>.
    /// </returns>
    private static bool IsBeginGroup( WMergeField field )
    {
      string prefix = field.Prefix;
      return ( prefix == "TableStart" || prefix == "BeginGroup" );
    }
    /// <summary>
    /// Determines whether [is end group] [the specified field].
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    /// 	If it is a end of the group, set to <c>true</c>.
    /// </returns>
    private static bool IsEndGroup( WMergeField field )
    {
      string prefix = field.Prefix;
      return ( prefix == "TableEnd" || prefix == "EndGroup" );
    }
    /// <summary>
    /// Checks the selection.
    /// </summary>
    /// <param name="rowsEnum">The rows enum.</param>
    /// <returns></returns>
    private bool CheckSelection( IRowsEnumerator rowsEnum )
    {
        if (rowsEnum.RowsCount > 0)
            return true;

        if (!m_bClearFields && !m_bRemoveEmptyPara)
            return true;

        GroupSelector gs = m_groupSelector;
        if (gs.GroupSelection != null)
        {
            if (m_bClearFields)
                HideFields(gs.GroupSelection.TextBody.Items);
            if (m_bRemoveEmptyPara)
            {
                RemoveEmptyPara(gs.GroupSelection.TextBody.Items);
            }
        }
        else if (gs.RowSelection != null)
        {
            int startIndex = gs.RowSelection.StartRowIndex;
            int endIndex = gs.RowSelection.EndRowIndex;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (gs.RowSelection.Table.Rows.Count > startIndex)
                {
                    if (m_bClearFields)
                        HideFields(gs.RowSelection.Table.Rows[i]);
                    if (m_bRemoveEmptyPara)
                    {
                        RemoveEmptyPara(gs.RowSelection.Table.Rows[i]);
                    }
                }
            }
        }

      return false;
    }
    /// <summary>
    /// Hides the fields.
    /// </summary>
    /// <param name="sections">The sections.</param>
    private void HideFields( IWSectionCollection sections )
    {
      for( int i = 0, len = sections.Count; i < len; i++ )
      {
        ExecuteForSection( sections[ i ], null );
      }
    }
    /// <summary>
    /// Hides the fields.
    /// </summary>
    /// <param name="row">The row.</param>
    private void HideFields( WTableRow row )
    {
      WTableCell cell = null;

      for( int i = 0, cnt = row.Cells.Count; i < cnt; i++ )
      {
        cell = row.Cells[ i ];
        HideFields( cell.Items );
      }
    }
    /// <summary>
    /// Hides the fields.
    /// </summary>
    /// <param name="items">The body items.</param>
    private void HideFields( BodyItemCollection items )
    {
      TextBodyItem item = null;

      for( int i = 0, cnt = items.Count; i < cnt; i++ )
      {
        item = items[ i ];
        if( item is WParagraph )
        {
          HideFields( item as WParagraph );
        }
        else if( item is WTable )
        {
          WTable table = item as WTable;
          for( int j = 0, rowCnt = table.Rows.Count; j < rowCnt; j++ )
          {
            HideFields( table.Rows[ j ] );
          }
        }
      }
    }
    /// <summary>
    /// Hides the fields.
    /// </summary>
    /// <param name="para">The paragraph.</param>
    private void HideFields( WParagraph para )
    {
      WField field = null;
      for( int i = 0, cnt = para.Items.Count; i < cnt; i++ )
      {
        if( para.Items[ i ] is WField )
        {
          field = para.Items[ i ] as WField;
          if( field.FieldType == FieldType.FieldMergeField || field.FieldType == FieldType.FieldNext )
          {
            HideField( field );
          }
        }
        else if( para.Items[ i ] is WTextBox )
        {
          HideFields( ( para.Items[ i ] as WTextBox ).TextBoxBody.Items );
        }
        else if( para.Items[ i ] is Shape )
        {
          HideFields( ( para.Items[ i ] as Shape ).TextBody.Items );
        }
      }
    }
    /// <summary>
    /// Hides the field.
    /// </summary>
    /// <param name="field">The field.</param>
    private void HideField( IWField field )
    {
      if( ( field as WField ).ConvertedToText )
        return;

      field.Text = string.Empty;
      ( field as WField ).ConvertedToText = true;
    }
    /// <summary>
    /// Removes the empty paragraph.
    /// </summary>
    /// <param name="para">The paragraph.</param>
    private void RemoveEmptyPara( WParagraph para )
    {
      if( para.Items.Count > 0 && para.Items[ 0 ] is WMergeField )
        {
            if (IsEmptyParagraph(para))
            {
                WTableCell parentCell = para.Owner as WTableCell;
                if (parentCell != null && parentCell.ChildEntities.Count == 1)
                    para.ChildEntities.Clear();
                else
                    para.RemoveEmpty = true;
            }
        }     
    }
    /// <summary>
    /// Checks whether the paragraph is empty.
    /// </summary>
    /// <param name="para">Paragraph</param>
    /// <returns>Returns true, if the paragraph is empty.</returns>
    private bool IsEmptyParagraph(WParagraph para)
    {
        bool IsEmpty = true;

        for (int i = 0; i < para.Items.Count; i++)
        {
            ParagraphItem item = para.Items[i];
            switch (item.EntityType)
            {
                case EntityType.MergeField:
                    WMergeField mergeField = para.Items[i] as WMergeField;
                    if (!(mergeField.ConvertedToText && mergeField.Text == string.Empty))
                    {
                        IsEmpty = false;
                    }
                    break;
                case EntityType.Picture:
                    IsEmpty = false;
                    break;
                default:
                    IsEmpty = false;
                    break;
            }
        }
        return IsEmpty;
    }
    /// <summary>
    /// Removes the empty paragraph.
    /// </summary>
    /// <param name="row">The row.</param>
    private void RemoveEmptyPara( WTableRow row )
    {
      WTableCell cell = null;

      for( int i = 0, cnt = row.Cells.Count; i < cnt; i++ )
      {
        cell = row.Cells[ i ];
        RemoveEmptyPara( cell.Items );
      }
    }
    /// <summary>
    /// Removes the empty paragraphs.
    /// </summary>
    /// <param name="items">The items.</param>
    private void RemoveEmptyPara( BodyItemCollection items )
    {
      TextBodyItem item = null;

      for( int i = 0, cnt = items.Count; i < cnt; i++ )
      {
        item = items[ i ];
        if( item is WParagraph )
        {
          if( RemoveEmptyPara( items, i ) )
          {
            i -= 1;
            cnt -= 1;
          }
        }
        else if( item is WTable )
        {
          WTable table = item as WTable;
          for( int j = 0, rowCnt = table.Rows.Count; j < rowCnt; j++ )
          {
            RemoveEmptyPara( table.Rows[ j ] );
          }
        }
      }
    }
    /// <summary>
    /// Removes the empty paragraph.
    /// </summary>
    /// <param name="paragraphs">The paragraphs.</param>
    /// <param name="paraIndex">Index of the paragraph.</param>
    /// <returns></returns>
    private bool RemoveEmptyPara( BodyItemCollection paragraphs, int paraIndex )
    {
      WParagraph para = paragraphs[ paraIndex ] as WParagraph;

      if( para.Items.Count > 0 && para.Text == string.Empty )
      {
        if( para.Items[ 0 ] is WMergeField )
        {
          paragraphs.Remove( para );
          return true;
        }
      }

      return false;
    }
    /// <summary>
    /// Gets the  name of the mapped column name.
    /// </summary>
    /// <param name="fieldName">Name of the merge field.</param>
    /// <returns></returns>
    private string GetMappedColName( string fieldName )
    {
      if( m_mappedFields != null && m_mappedFields.ContainsKey( fieldName ) )
      {
        return m_mappedFields[ fieldName ];
      }
      return null;
    }
    /// <summary>
    /// Removes the spell checking.
    /// </summary>
    private void RemoveSpellChecking()
    {
      if( Document.GrammarSpellingData != null )
      {
        Document.GrammarSpellingData.PlcfgramData = null;
        Document.GrammarSpellingData.PlcfsplData = null;
      }
    }
    #endregion

    #region Internal declarations
    /// <summary>
    /// 
    /// </summary>
    internal class GroupSelector
    {
    #region Fields
      /// <summary>
      /// The internal fields.
      /// </summary>
      private TextBodySelection m_groupSelection;
      private TableRowSelection m_rowSelection;
      private WTextBody m_groupTextBody;
      private WTextBody m_body;
      private WMergeField m_beginGroupField = null;
      private WMergeField m_endGroupField = null;
      private int m_bodyItemIndex = 0;
      private int m_bodyItemStartIndex = -1;
      private int m_paragraphItemIndex = -1;
      private int m_paragraphItemStartIndex = -1;
      private int m_rowIndex = -1;
      private int m_startRowIndex = -1;
      private string m_groupName;
      private GroupFound SendGroupFound;
      private IRowsEnumerator m_rowsEnum;
      private int m_selBodyItemsCnt = -1;
    #endregion

    #region Properties
      /// <summary>
      /// Gets the group selection.
      /// </summary>
      /// <value>The group selection.</value>
      internal TextBodySelection GroupSelection
      {
        get
        {
          return m_groupSelection;
        }
      }
      /// <summary>
      /// Gets the row selection.
      /// </summary>
      /// <value>The row selection.</value>
      internal TableRowSelection RowSelection
      {
        get
        {
          return m_rowSelection;
        }
      }
      /// <summary>
      /// Gets the begin group field.
      /// </summary>
      /// <value>The begin group field.</value>
      internal WMergeField BeginGroupField
      {
        get
        {
          return m_beginGroupField;
        }
      }
      /// <summary>
      /// Gets or sets the end group field.
      /// </summary>
      /// <value>The end group field.</value>
      internal WMergeField EndGroupField
      {
        get
        {
          return m_endGroupField;
        }
        set
        {
          m_endGroupField = value;
        }
      }
      /// <summary>
      /// Gets or sets the index of the body item.
      /// </summary>
      /// <value>The index of the body item.</value>
      internal int BodyItemIndex
      {
        get
        {
          return m_bodyItemIndex;
        }
        set
        {
          m_bodyItemIndex = value;
        }
      }
      /// <summary>
      /// Gets a value indicating whether group is found.
      /// </summary>
      /// <value>
      /// 	If group is found, set to <c>true</c>.
      /// </value>
      internal bool IsGroupFound
      {
        get
        {
          return ( m_endGroupField != null );
        }
      }
      /// <summary>
      /// Gets or set the name of the group.
      /// </summary>
      /// <value>The name of the group.</value>
      internal string GroupName
      {
        get
        {
          return m_groupName;
        }
        set
        {
          m_groupName = value;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      internal int SelectedBodyItemsCount
      {
        get
        {
          return m_selBodyItemsCnt;
        }
      }
    #endregion

    #region Constructor
      /// <summary>
      /// Initializes a new instance of the <see cref="GroupSelector"/> class.
      /// </summary>
      /// <param name="onGroupFound">The on group found.</param>
      internal GroupSelector( GroupFound onGroupFound )
      {
        SendGroupFound += onGroupFound;
      }
    #endregion

    #region Methods
      /// <summary>
      /// Initialize the process.
      /// </summary>
      private void InitProcess( IRowsEnumerator rowsEnum )
      {
        m_groupSelection = null;
        m_rowSelection = null;
        m_beginGroupField = null;
        m_endGroupField = null;
        m_bodyItemIndex = 0;
        m_bodyItemStartIndex = -1;
        m_paragraphItemIndex = -1;
        m_paragraphItemStartIndex = -1;
        m_rowIndex = -1;
        m_selBodyItemsCnt = -1;

        m_rowsEnum = rowsEnum;
        m_groupName = m_rowsEnum.TableName;
      }
      /// <summary>
      /// Processes the groups.
      /// </summary>
      /// <param name="body">The body.</param>
      /// <param name="rowsEnum">The rows enum.</param>
      internal void ProcessGroups( WTextBody body, IRowsEnumerator rowsEnum )
      {
        InitProcess( rowsEnum );
        m_groupTextBody = m_body = body;
        FindInBodyItems( m_body.Items );
      }
      /// <summary>
      /// Processes the groups.
      /// </summary>
      /// <param name="table">The table.</param>
      /// <param name="startRow">The start row.</param>
      /// <param name="endRow">The end row.</param>
      /// <param name="rowsEnum">The rows enumerator.</param>
      internal void ProcessGroups( WTable table, int startRow, int endRow, IRowsEnumerator rowsEnum )
      {
        InitProcess( rowsEnum );
        FindInTable( table, startRow, endRow );
      }
      /// <summary>
      /// Finds inside body items.
      /// </summary>
      /// <param name="bodyItems">The body items.</param>
      private void FindInBodyItems( BodyItemCollection bodyItems )
      {
        for( int i = 0, cnt = bodyItems.Count; i < cnt; i++ )
        {
          TextBodyItem bodyItem = ( TextBodyItem )bodyItems[ i ];
          m_bodyItemIndex = i;

          switch( bodyItem.EntityType )
          {
            case EntityType.Paragraph:
              WParagraph para = ( WParagraph )bodyItem;
              for( int j = 0; j < para.Items.Count; j++ )
              {
                ParagraphItem item = para.Items[ j ];

                m_paragraphItemIndex = j;
                if( item is WTextBox )
                {
                  m_bodyItemIndex = 0;
                  FindInBodyItems( ( item as WTextBox ).TextBoxBody.Items );
                  m_bodyItemIndex = i;
                }
                else if( item is Shape )
                {
                  m_bodyItemIndex = 0;
                  FindInBodyItems( ( item as Shape ).TextBody.Items );
                  m_bodyItemIndex = i;
                }
                else
                {
                  CheckItem( item );
                }

                if( IsGroupFound )
                {
                  if( m_groupSelection != null )
                  {
                    i = m_groupSelection.ItemEndIndex;
                    //Updates the paragraph item end index (Mail group defined as single paragraph).
                    if (m_groupSelection.ItemStartIndex == m_groupSelection.ItemEndIndex)
                        j = m_groupSelection.ParagraphItemEndIndex;
                    cnt = bodyItems.Count;
                    ClearSelection();
                  }
                  else
                  {
                    break;
                  }
                }
              }
              break;
            case EntityType.Table:
              {
                WTable table = ( WTable )bodyItem;
                FindInTable( table, 0, table.Rows.Count - 1 );
              }
              break;

            default:
              throw new Exception();
          }
        }
      }
      /// <summary>
      /// Finds the group in the table.
      /// </summary>
      /// <param name="table">The table.</param>
      /// <param name="startRow">The start row.</param>
      /// <param name="endRow">The end row.</param>
      private void FindInTable( WTable table, int startRow, int endRow )
      {
        int rowsCntBefore = table.Rows.Count;
        for( int k = startRow; k <= endRow; k++ )
        {
          WTableRow row = table.Rows[ k ];
          m_rowIndex = k;

          for( int l = 0, cntl = row.Cells.Count; l < cntl; l++ )
          {
            WTableCell cell = row.Cells[ l ];
            FindInBodyItems( cell.Items );

            if( IsGroupFound )
            {
              endRow += table.Rows.Count - rowsCntBefore;
              k = m_rowSelection.StartRowIndex;
              ClearSelection();
              break;
            }
          }
        }
      }
      /// <summary>
      /// Clears the selection.
      /// </summary>
      private void ClearSelection()
      {
        m_groupSelection = null;
        m_rowSelection = null;
        m_beginGroupField = null;
        m_endGroupField = null;
      }
      /// <summary>
      /// Checks the item.
      /// </summary>
      /// <param name="item">The item.</param>
      private void CheckItem( ParagraphItem item )
      {
        if( item.EntityType == EntityType.MergeField )
        {
          WMergeField field = item as WMergeField;

          if( field.FieldName == m_groupName )
          {
            if( m_beginGroupField == null )
            {
              if( IsBeginGroup( field ) )
              {
                StartSelection( field );
              }
            }
            else
            {
              if( IsEndGroup( field ) )
              {
                EndSelection( field );

                if( SendGroupFound != null )
                {
                  SendGroupFound( m_rowsEnum );
                }
              }
            }
          }
        }
      }
      /// <summary>
      /// Starts the selection.
      /// </summary>
      private void StartSelection( WMergeField field )
      {
        m_beginGroupField = field;
        m_groupTextBody = field.OwnerParagraph.OwnerTextBody;
        m_bodyItemStartIndex = m_bodyItemIndex;
        m_paragraphItemStartIndex = m_paragraphItemIndex;
        m_startRowIndex = m_rowIndex;
      }
      /// <summary>
      /// Ends the selection.
      /// </summary>
      private void EndSelection( WMergeField field )
      {
        m_endGroupField = field;
        WTextBody textBody = field.OwnerParagraph.OwnerTextBody;
        m_selBodyItemsCnt = m_bodyItemIndex - m_bodyItemStartIndex + 1;

        if( textBody == m_groupTextBody )
        {
          m_groupSelection = new TextBodySelection( textBody,
            m_bodyItemStartIndex,
            m_bodyItemIndex,
            m_paragraphItemStartIndex,
            m_paragraphItemIndex
            );
        }
        else if( textBody.EntityType == EntityType.TableCell &&
          m_groupTextBody.EntityType == EntityType.TableCell &&
          ( m_groupTextBody.Owner as WTableRow ).OwnerTable == ( textBody.Owner as WTableRow ).OwnerTable )
        {
          UpdateEndSelection( textBody as WTableCell );
          m_rowSelection = new TableRowSelection( textBody.Owner.Owner as WTable,
            m_startRowIndex, m_rowIndex );
        }
        else
        {
          throw new MailMergeException();
        }
      }
      /// <summary>
      /// Updates end of row selection in case cell has vertical merge.
      /// </summary>
      /// <param name="cell">The cell.</param>
      private void UpdateEndSelection( WTableCell cell )
      {
        WTableRow row = cell.OwnerRow;
        bool hasMerge = false;

        foreach( WTableCell tblCell in row.Cells )
        {
          if( cell.CellFormat.VerticalMerge != CellMerge.None )
          {
            hasMerge = true;
            break;
          }
        }

        if( !hasMerge )
          return;

        while( row.NextSibling != null )
        {
          row = row.NextSibling as WTableRow;
          hasMerge = false;

          foreach( WTableCell tblCell in row.Cells )
          {
            if( cell.CellFormat.VerticalMerge != CellMerge.None )
            {
              hasMerge = true;
              break;
            }
          }

          if( hasMerge )
            m_rowIndex += 1;
          else
            break;
        }
      }
    #endregion

    #region Internal declarations
      /// <summary>
      /// 
      /// </summary>
      /// <param name="rowsEnum"></param>
      internal delegate void GroupFound( IRowsEnumerator rowsEnum );
    #endregion
    }
    /// <summary>
    /// Represents a row selection.
    /// </summary>
    internal class TableRowSelection
    {
    #region Fields
      internal WTable Table;
      internal int StartRowIndex;
      internal int EndRowIndex;
    #endregion

    #region Constructor
      /// <summary>
      /// Initializes a new instance of the <see cref="TableRowSelection"/> class.
      /// </summary>
      /// <param name="table">The table.</param>
      /// <param name="startRowIndex">Start index of the row.</param>
      /// <param name="endRowIndex">End index of the row.</param>
      internal TableRowSelection( WTable table, int startRowIndex, int endRowIndex )
      {
        Table = table;
        StartRowIndex = startRowIndex;
        EndRowIndex = endRowIndex;
        ValidateIndexes();
      }
      /// <summary>
      /// Validates the indexes.
      /// </summary>
      private void ValidateIndexes()
      {
        if( StartRowIndex < 0 || StartRowIndex >= Table.Rows.Count )
          throw new ArgumentOutOfRangeException( "StartRowIndex" );

        if( EndRowIndex < 0 || EndRowIndex >= Table.Rows.Count )
          throw new ArgumentOutOfRangeException( "EndRowIndex" );
      }
    #endregion
    }
    #endregion
  }
  /// <summary>
  /// Represents the Method that handles MergeField event
  /// </summary>
  public delegate void MergeFieldEventHandler( object sender, MergeFieldEventArgs args );
  /// <summary>
  /// Represents the Method that handles MergeImageField event
  /// </summary>
  public delegate void MergeImageFieldEventHandler( object sender, MergeImageFieldEventArgs args );
  /// <summary>
  /// Provides data during MergeField event.
  /// </summary>
  public class MergeFieldEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// Represents the document.
    /// </summary>
    private IWordDocument m_doc = null;
    /// <summary>
    /// Represents the Merge field.
    /// </summary>
    private IWMergeField m_field;
    /// <summary>
    /// Represents the Field Value
    /// </summary>
    private object m_fieldValue;
    /// <summary>
    /// Represents the Row Index.
    /// </summary>
    private int m_rowIndex;
    /// <summary>
    /// Represents the Table Name
    /// </summary>
    private string m_tableName;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets the document.
    /// </summary>
    public IWordDocument Document
    {
      get
      {
        return m_doc;
      }
    }
    /// <summary>
    /// Gets the Merge field Name.
    /// </summary>
    public string FieldName
    {
      get
      {
        return m_field.FieldName;
      }
    }
    /// <summary>
    /// Gets the Merge Field Value
    /// </summary>
    public object FieldValue
    {
      get
      {
        return m_fieldValue;
      }
    }
    /// <summary>
    /// Gets the Table Name
    /// </summary>
    public string TableName
    {
      get
      {
        return m_tableName;
      }
    }
    /// <summary>
    /// Gets the Row Index.
    /// </summary>
    public int RowIndex
    {
      get
      {
        return m_rowIndex;
      }
    }
    /// <summary>
    /// Gets the Character Format of the field.
    /// </summary>
    public WCharacterFormat CharacterFormat
    {
      get
      {
        return m_field.CharacterFormat;
      }
    }
    /// <summary>
    /// Gets the Text value
    /// </summary>
    public string Text
    {
      get
      {
        if( FieldValue == null )
        {
          return "";
        }

        return FieldValue.ToString();
      }
      set
      {
        m_fieldValue = value;
      }
    }
    /// <summary>
    /// Gets the current Merge field.
    /// </summary>
    public IWMergeField CurrentMergeField
    {
      get
      {
        return m_field;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Provides data during the MergeField event.
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="tableName"></param>
    /// <param name="rowIndex"></param>
    /// <param name="field"></param>
    /// <param name="value"></param>
    public MergeFieldEventArgs( IWordDocument doc, string tableName, int rowIndex,
                                IWMergeField field, object value )
    {
      m_doc = doc;
      m_field = field;
      m_fieldValue = value;
      m_rowIndex = rowIndex;
      m_tableName = tableName;
    }
    #endregion

  }
  /// <summary>
  /// Provides data during MergeImageField event.
  /// </summary>
  public class MergeImageFieldEventArgs : MergeFieldEventArgs
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private bool m_useText = false;
    /// <summary>
    /// 
    /// </summary>
    private Image m_image = null;
    /// <summary>
    /// 
    /// </summary>
    private Stream m_imageStream = null;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSkip;
    #endregion

    #region Class properties
    /// <summary>
    /// Setting to specify if Text should be used.
    /// </summary>
    public bool UseText
    {
      get
      {
        return m_useText;
      }
    }
    /// <summary>
    /// Gets / Sets Image Stream.
    /// </summary>
    public Stream ImageStream
    {
      get
      {
        return m_imageStream;
      }
      set
      {
        m_imageStream = value;
        LoadImage( m_imageStream );
      }
    }
#if !(SILVERLIGHT || WP)
    /// <summary>
    /// Gets / Sets Image.
    /// </summary>
    public Image Image
    {
      get
      {
        return m_image;
      }
      set
      {
        m_image = value;
      }
    }
#else
    /// <summary>
    /// Gets / Sets Image.
    /// </summary>
    internal Image Image
    {
      get
      {
        return m_image;
      }
      set
      {
        m_image = value;
      }
    }

#endif

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="MergeImageFieldEventArgs"/> is skip.
    /// </summary>
    /// <value><c>true</c> if skip; otherwise, <c>false</c>.</value>
    public bool Skip
    {
      get
      {
        return m_bSkip;
      }
      set
      {
        m_bSkip = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
#if !(SILVERLIGHT || WP)
    /// <summary>
    /// Provides data during MergeImageField event.
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="tableName"></param>
    /// <param name="rowIndex"></param>
    /// <param name="field"></param>
    /// <param name="image"></param>
    public MergeImageFieldEventArgs( IWordDocument doc, string tableName, int rowIndex,
      IWMergeField field, Image image )
      : base( doc, tableName, rowIndex, field, null )
    {
      m_image = image;
    }
#endif
    public MergeImageFieldEventArgs( IWordDocument doc, string tableName, int rowIndex,
      IWMergeField field, object obj )
      : base( doc, tableName, rowIndex, field, obj )
    {
      m_image = obj as Image;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    private void LoadImage( Stream stream )
    {
      m_image = new Image( stream );
    }
    #endregion
  }

  /// <summary>
  /// The EntityEntry class maintains the Entries of Entity class.
  /// </summary>
  public class EntityEntry
  {
    #region Fields
    /// <summary>
    /// Current entity.
    /// </summary>
    public Entity Current;
    /// <summary>
    /// Index value.
    /// </summary>
    public int Index;
    #endregion

    #region Constructor
    /// <summary>
    /// The base class for DLS entities.
    /// </summary>
    /// <param name="ent"></param>
    /// <param name="index"></param>
    public EntityEntry( Entity ent )
    {
      Current = ent;
      Index = 0;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Fetches the entries element.
    /// </summary>
    /// <returns>if contains entry, it will return true. Otherwise, it returns false.</returns>
    public bool Fetch()
    {
      if( Current != null && Current.Owner != null && Current.Owner.IsComposite )
      {
        ICompositeEntity ce = Current.Owner as ICompositeEntity;

        if( ce.ChildEntities.Count > Index + 1 )
        {
          Index += 1;
          Current = ce.ChildEntities[ Index ];
          return true;
        }

      }

      Current = null;
      Index = -1;
      return false;
    }
    #endregion
  }
#else
    /// <summary>
    /// Represents the mail merge functionality. 
    /// </summary>
    public class MailMerge
    {
        #region Fields
        private WordDocument m_doc;
        private GroupSelector m_groupSelector;
        private WSectionCollection m_contentSections;
        private string[] m_names;
        private string[] m_values;
        private bool m_bClearFields = true;
        //private bool m_bClearFields;
#if SILVERLIGHT || WP
        m_bClearFields = true;
#endif
        private bool m_bBeginGroupFound = false;
        private bool m_bEndGroupFound = false;
        private bool m_bRemoveEmptyPara;
        private bool m_bRemoveEmptyGroup;
        /// <summary>
        /// Fields used for nested mail merge.
        /// </summary>
        private DbConnection m_conn;
        private DataSet m_curDataSet;
        private Dictionary<string, IRowsEnumerator> m_nestedEnums;
        private Regex m_varCmdRegex;
        private Stack<GroupSelector> m_groupSelectors;
        private ArrayList m_commands;
        private bool m_bIsNested;
        private Dictionary<string, string> m_mappedFields;
        private bool m_isSqlConnection;
        private DataSet m_dataSet = null;
        private MailMergeDataSet m_dataSetDocIO;
        private List<DictionaryEntry> m_commandsDocIO;
        private MailMergeDataSet m_curDataSetDocIO;
        private int m_mergedRecordCount = 0;
        private Dictionary<string, bool> m_clearFieldsState;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [clear fields].
        /// </summary>
        /// <value><c>true</c> if it clears the fields, set to <c>true</c>.</value>
        public bool ClearFields
        {
            get
            {
                return m_bClearFields;
            }
            set
            {
                m_bClearFields = value;
            }
        }
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        protected WordDocument Document
        {
            get
            {
                return m_doc;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to remove paragraphs which contain empty merge fields
        /// (merge field with no data after mail merge).
        /// </summary>
        /// <value>
        /// Set "true" to remove empty paragraphs; otherwise, "false".
        /// </value>
        public bool RemoveEmptyParagraphs
        {
            get
            {
                return m_bRemoveEmptyPara;
            }
            set
            {
                m_bRemoveEmptyPara = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to remove groups which contain empty merge fields
        /// (merge field with no data after mail merge).
        /// </summary>
        /// <value>
        /// Set "true" to remove the empty groups; By default the value is false.
        /// </value>
        public bool RemoveEmptyGroup
        {
            get
            {
                return m_bRemoveEmptyGroup;
            }
            set
            {
                m_bRemoveEmptyGroup = value;
            }
        }
        /// <summary>
        /// Gets the enumerators which are used during mailmerge.
        /// </summary>
        /// <value>The nested enums.</value>
        private Dictionary<string, IRowsEnumerator> NestedEnums
        {
            get
            {
                if (m_nestedEnums == null)
                {
                    m_nestedEnums = new Dictionary<string, IRowsEnumerator>();
                }
                return m_nestedEnums;
            }
        }
        /// <summary>
        /// Gets the current data set for nested mail merge.
        /// </summary>
        /// <value>The current data set.</value>
        private DataSet CurrentDataSet
        {
            get
            {
                if (m_curDataSet == null)
                {
                    m_curDataSet = new DataSet();
                }
                return m_curDataSet;
            }
        }
        /// <summary>
        /// Gets the variable command regulat expression.
        /// </summary>
        /// <value>The variable command regex.</value>
        private Regex VariableCommandRegex
        {
            get
            {
                if (m_varCmdRegex == null)
                {
                    m_varCmdRegex = new Regex("%([^\"%]+)%");
                }
                return m_varCmdRegex;
            }
        }
        /// <summary>
        /// Gets the group selectors.
        /// </summary>
        /// <value>The group selectors.</value>
        private Stack<GroupSelector> GroupSelectors
        {
            get
            {
                if (m_groupSelectors == null)
                {
                    m_groupSelectors = new Stack<GroupSelector>();
                }
                return m_groupSelectors;
            }
        }
        /// <summary>
        /// Gets the collection of mapped fields. Mapped fields represent mapping between
        /// fields names in the data source and mail merge fields in the document. The keys of
        /// the collection are merge field names and the values are field names in the data source.
        /// </summary>
        /// <value>The collection of mapped fields.</value>
        public Dictionary<string, string> MappedFields
        {
            get
            {
                if (m_mappedFields == null)
                {
                    m_mappedFields = new Dictionary<string, string>();
                }
                return m_mappedFields;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private MailMergeDataSet CurrentDataSetDocIO
        {
            get
            {
                if (m_curDataSetDocIO == null)
                {
                    m_curDataSetDocIO = new MailMergeDataSet();
                }
                return m_curDataSetDocIO;
            }
        }
        /// <summary>
        /// Gets the previous state of the clear fields.
        /// </summary>
        /// <value>The previous state of the clear fields.</value>
        private Dictionary<string, bool> ClearFieldsState
        {
            get
            {
                if (m_clearFieldsState == null)
                {
                    m_clearFieldsState = new Dictionary<string, bool>();
                }
                return m_clearFieldsState;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs during mail merge when a text merge field is encountered in the document
        /// </summary>
        public event MergeFieldEventHandler MergeField;
        /// <summary>
        /// Occurs during mail merge when an image merge field is encountered in the document
        /// </summary>
        public event MergeImageFieldEventHandler MergeImageField;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MailMerge"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        internal MailMerge(WordDocument document)
        {
            m_doc = document;
            m_contentSections = new WSectionCollection();
            m_groupSelector = new GroupSelector(new GroupSelector.GroupFound(OnGroupFound));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Performs mail merge operation.
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <param name="fieldValues"></param>
        public void Execute(string[] fieldNames, string[] fieldValues)
        {
            Document.IsMailMerge = true;

            if (fieldNames == null)
            {
                throw new ArgumentNullException("fieldNames");
            }
            if (fieldValues == null)
            {
                throw new ArgumentNullException("fieldValues");
            }

            m_names = fieldNames;
            m_values = fieldValues;

            if (m_names.Length > 0)
            {
                IWSection sec = null;
                for (int i = 0, cnt = Document.Sections.Count; i < cnt; i++)
                {
                    sec = Document.Sections[i];
                    ExecuteForSection(sec, null);
                }
            }
            Document.IsMailMerge = false;
        }
        ///<summary>
        ///Performs mail merge from a DataRow into the document
        ///</summary>
        ///<param name="row"></param>
        public void Execute(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            Execute(new DataTableEnumerator(row));
        }
        /// <summary>
        /// Performs mail merge operation.
        /// </summary>
        /// <param name="dataSource">IEnumerable data source</param>
        public void Execute(IEnumerable dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("datasource");

            MailMergeDataTable table = new MailMergeDataTable(string.Empty, dataSource);
            ExecuteGroup(table);
        }
        /// <summary>
        /// Performs mail merge from a DataTable
        /// </summary>
        /// <param name="table"></param>
        public void Execute(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            Execute(new DataTableEnumerator(table));
        }
        ///<summary>
        /// Performs mail merge from a DataView
        ///</summary>
        ///<param name="dataView"></param>
        public void Execute(DataView dataView)
        {
            if (dataView == null)
                throw new ArgumentNullException("dataView");

            Execute(new DataViewEnumerator(dataView));
        }
        /// <summary>
        /// Performs mail merge from
        /// </summary>
        /// <param name="dataReader"></param>
        public void Execute(OleDbDataReader dataReader)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");

            Execute(new DataReaderEnumerator(dataReader as IDataReader));
        }
        /// <summary>
        /// Performs mail merge from a DataView
        /// </summary>
        /// <param name="dataReader"></param>
        public void Execute(IDataReader dataReader)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");

            Execute(new DataReaderEnumerator(dataReader));
        }
        /// <summary>
        /// Performs Mail Merge within a Group from a DataTable.
        /// </summary>
        /// <param name="table"></param>
        public void ExecuteGroup(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            ExecuteGroup(new DataTableEnumerator(table));
        }
        /// <summary>
        /// Performs Mail Merge within a Group from a DataView.
        /// </summary>
        /// <param name="dataView"></param>
        public void ExecuteGroup(DataView dataView)
        {
            if (dataView == null)
                throw new ArgumentNullException("dataView");

            ExecuteGroup(new DataViewEnumerator(dataView));
        }
        /// <summary>
        /// Performs Mail Merge within a Group from a DataReader.
        /// </summary>
        /// <param name="dataReader"></param>
        public void ExecuteGroup(IDataReader dataReader)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");

            ExecuteGroup(new DataReaderEnumerator(dataReader));
        }
        /// <summary>
        /// Performs mail merge operation.
        /// </summary>
        /// <param name="dataSource">MailMergeDataSet</param>
        /// <param name="commands">Commands list</param>
        public void ExecuteNestedGroup(MailMergeDataSet dataSource, List<DictionaryEntry> commands)
        {
            if (dataSource == null || dataSource.DataSet.Count == 0)
                throw new ArgumentException("dataSet is empty");

            if (commands == null || commands.Count == 0)
                throw new ArgumentException("commands list is empty");

            RemoveSpellChecking();
            m_dataSetDocIO = dataSource;
            m_commandsDocIO = commands;
            DictionaryEntry entry = (DictionaryEntry)commands[0];
            Document.IsMailMerge = true;
            m_bIsNested = true;
            ExecuteNestedGroup((string)entry.Key);

            if (m_nestedEnums != null)
            {
                m_nestedEnums.Clear();
                m_nestedEnums = null;
            }
            if (m_dataSet != null)
            {
                m_dataSet.Clear();
                m_dataSet = null;
            }
            Document.IsMailMerge = false;
            m_bIsNested = false;
        }
        /// <summary>
        /// Executes nested mailmerge within a Group for the specified data.
        /// </summary>
        /// <param name="conn">The Connection.</param>
        /// <param name="commands">The commands.</param>
        public void ExecuteNestedGroup(DbConnection conn, ArrayList commands)
        {
            if (conn == null)
                throw new ArgumentException("conn");

            if (commands == null)
                throw new ArgumentException("commands");

            RemoveSpellChecking();
            m_conn = conn;
            m_commands = commands;
            DictionaryEntry entry = (DictionaryEntry)commands[0];
            Document.IsMailMerge = true;
            m_bIsNested = true;
            ExecuteNestedGroup((string)entry.Key);

            if (m_nestedEnums != null)
            {
                m_nestedEnums.Clear();
                m_nestedEnums = null;
            }

            if (m_curDataSet != null)
            {
                m_curDataSet.Clear();
                m_curDataSet = null;
            }
            Document.IsMailMerge = false;
            m_bIsNested = false;
        }
        /// <summary>
        /// Executes the nested group.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="commands">The commands.</param>
        /// <param name="isSqlConnection">if it is a SQL connection, set to <c>true</c>.</param>
        public void ExecuteNestedGroup(DbConnection conn, ArrayList commands, bool isSqlConnection)
        {
            m_isSqlConnection = isSqlConnection;
            ExecuteNestedGroup(conn, commands);
        }
        /// <summary>
        /// Executes the nested group.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="commands">The commands.</param>
        public void ExecuteNestedGroup(DataSet dataSet, ArrayList commands)
        {
            if (dataSet == null)
                throw new ArgumentException("dataSet");

            if (commands == null)
                throw new ArgumentException("commands");

            RemoveSpellChecking();
            m_dataSet = dataSet.Copy();
            m_commands = commands;
            DictionaryEntry entry = (DictionaryEntry)commands[0];
            Document.IsMailMerge = true;
            m_bIsNested = true;
            ExecuteNestedGroup((string)entry.Key);

            if (m_nestedEnums != null)
            {
                m_nestedEnums.Clear();
                m_nestedEnums = null;
            }
            if (m_dataSet != null)
            {
                m_dataSet.Clear();
                m_dataSet = null;
            }
            Document.IsMailMerge = false;
            m_bIsNested = false;
        }
        /// <summary>
        /// Returns a collection of mergefield names found in the document.
        /// </summary>
        /// <returns></returns>
        public string[] GetMergeFieldNames()
        {
            ArrayList fieldsArray = new ArrayList();

            GetMergeFieldNamesImpl(fieldsArray, null);

            return (string[])fieldsArray.ToArray(typeof(string));
        }
        /// <summary>
        /// Gets the merge field names.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        public string[] GetMergeFieldNames(string groupName)
        {
            ArrayList fieldsArray = new ArrayList();

            GetMergeFieldNamesImpl(fieldsArray, groupName);

            return (string[])fieldsArray.ToArray(typeof(string));
        }
        /// <summary>
        /// Gets the merge field names.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        public string[] GetMergeGroupNames()
        {
            ArrayList groupNames = new ArrayList();
            Stack<EntityEntry> entDeep = new Stack<EntityEntry>();
            entDeep.Push(new EntityEntry(Document));

            do
            {
                EntityEntry ent = entDeep.Peek();

                // If we have child - add it to stack
                if (ent.Current != null && ent.Current.IsComposite)
                {
                    ICompositeEntity ce = ent.Current as ICompositeEntity;

                    if (ce.ChildEntities.Count > 0)
                    {
                        entDeep.Push(new EntityEntry(ce.ChildEntities[0]));
                        continue;
                    }
                }

                // Process leafs
                if (ent.Current != null)
                {
                    if (ent.Current.EntityType == EntityType.MergeField)
                    {
                        WMergeField mf = ent.Current as WMergeField;
                        if (IsBeginGroup(mf))
                        {
                            groupNames.Add(mf.FieldName);
                        }
                    }
                }

                // Move next or drop
                while (!ent.Fetch())
                {
                    entDeep.Pop();

                    if (entDeep.Count == 0)
                        break;

                    ent = entDeep.Peek();
                }

            }
            while (entDeep.Count > 0);

            return (string[])groupNames.ToArray(typeof(string));
        }
        /// <summary>
        /// Gets the merge group names2.
        /// </summary>
        /// <returns></returns>
        private string[] GetMergeGroupNames2()
        {
            ArrayList groupNames = new ArrayList();
            Stack<IEnumerator> enumStack = new Stack<IEnumerator>();
            enumStack.Push(Document.ChildEntities.GetEnumerator());

            do
            {
                IEnumerator en = enumStack.Peek();

                // If we have child - add it to stack
                if (en.MoveNext())
                {
                    ICompositeEntity ce = en.Current as ICompositeEntity;

                    if (ce != null && ce.ChildEntities.Count > 0)
                    {
                        enumStack.Push(ce.ChildEntities.GetEnumerator());
                        continue;
                    }
                }

                // Process leafs
                if (en.Current != null)
                {
                    Entity ent = en.Current as Entity;

                    if (ent != null && ent.EntityType == EntityType.MergeField)
                    {
                        WMergeField mf = ent as WMergeField;
                        if (IsBeginGroup(mf))
                        {
                            groupNames.Add(mf.FieldName);
                        }
                    }
                }

                // Move next or drop
                while (!en.MoveNext())
                {
                    enumStack.Pop();

                    if (enumStack.Count == 0)
                        break;

                    en = enumStack.Peek();
                }

            }
            while (enumStack.Count > 0);

            return (string[])groupNames.ToArray(typeof(string[]));
        }
        /// <summary>
        /// Gets the merge field names.
        /// </summary>
        /// <param name="fieldsArray">The fields array.</param>
        /// <param name="groupName">Name of the group.</param>
        private void GetMergeFieldNamesImpl(ArrayList fieldsArray, string groupName)
        {
            WSection section = null;
            TextBodyItem paragraph = null;
            for (int index = 0, cnt = Document.Sections.Count; index < cnt; index++)
            {
                section = Document.Sections[index];
                for (int j = 0, parCnt = section.Body.Items.Count; j < parCnt; j++)
                {
                    paragraph = section.Body.Items[j];
                    GetFieldNamesForParagraph(fieldsArray, paragraph, groupName);
                }
                // Executes for headers/footers
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0, parCnt = section.HeadersFooters[i].Items.Count; j < parCnt; j++)
                    {
                        paragraph = section.HeadersFooters[i].Items[j];
                        GetFieldNamesForParagraph(fieldsArray, paragraph, groupName);
                    }
                }
            }
        }
        #endregion

        #region Implementation / with groups
        /// <summary>
        /// Called when group found by GroupSelector.
        /// </summary>
        private void OnGroupFound(IRowsEnumerator rowsEnum)
        {
            bool isFieldsHidded = false;
            GroupSelector gs = m_groupSelector;
            if (m_bClearFields && gs.GroupSelection != null && gs.GroupSelection.TextBody.Owner != null && gs.GroupSelection.TextBody.Owner is WSection && rowsEnum.RowsCount == 0)
            {
                int startFieldIndex = gs.BeginGroupField.Owner.GetIndexInOwnerCollection();
                int endFieldIndex = gs.EndGroupField.Owner.GetIndexInOwnerCollection();
                WTextBody textbody = gs.BeginGroupField.Owner.Owner as WTextBody;
                for (int i = startFieldIndex; i <= endFieldIndex; i++)
                {
                    if (textbody.Items[i] is WParagraph)
                    {
                        HideFields(textbody.Items[i] as WParagraph);
                    }
                    else if (textbody.Items[i] is WTable)
                    {
                        WTable table = textbody.Items[i] as WTable;
                        for (int j = 0, rowCnt = table.Rows.Count; j < rowCnt; j++)
                        {
                            HideFields(table.Rows[j]);
                        }
                    }
                }
                isFieldsHidded = true;
                if (rowsEnum.RowsCount == 0 && RemoveEmptyGroup)
                {
                    EmptyGroup(gs);
                }
            }
            else
            {
                if (rowsEnum.RowsCount == 0 && RemoveEmptyGroup)
                {
                    EmptyGroup(gs);
                }
                HideField(gs.BeginGroupField, true);
                HideField(gs.EndGroupField, true);
            }

            if (m_bIsNested)
            {
                m_groupSelector.BeginGroupField.FieldName = string.Empty;
                m_groupSelector.EndGroupField.FieldName = string.Empty;
            }

            if (!isFieldsHidded && !CheckSelection(rowsEnum))
                return;

            if (gs.GroupSelection != null)
            {
                OnBodyGroupFound(rowsEnum);
            }
            else if (gs.RowSelection != null)
            {
                OnRowGroupFound(rowsEnum);
            }
        }
        /// <summary>
        /// Remove items between Empty Group
        /// </summary>
        /// <param name="gs">The group selector</param>
        private void EmptyGroup(GroupSelector gs)
        {
            if (gs.BeginGroupField.OwnerParagraph.IsInCell)
            {
                if (gs.BeginGroupField.Prefix == "TableStart")
                {
                    EmptyGroupInTable(gs);
                }
                else
                {
                    EmptyGroupInTableCell(gs);
                }
            }
            else
            {
                EmptyGroupInTextbody(gs);
            }
            if (gs.GroupSelection != null)
            {
                //Set group selection start and end index to be the same after emptying the group.
                gs.GroupSelection.ItemEndIndex = gs.GroupSelection.ItemStartIndex;
            }
            if (gs.RowSelection != null)
            {
                //Set row selection start and end index to be the same after emptying the group.
                gs.RowSelection.EndRowIndex = gs.RowSelection.StartRowIndex;
            }
        }
        /// <summary>
        /// Remove items between Empty group present in Text body.
        /// </summary>
        /// <param name="gs">The group selector<</param>
        private void EmptyGroupInTextbody(GroupSelector gs)
        {
            int startIndex = gs.BeginGroupField.GetIndexInOwnerCollection();
            int startParaIndex = (gs.BeginGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();
            int endIndex = gs.EndGroupField.GetIndexInOwnerCollection();
            int endParaIndex = (gs.EndGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();

            WTextBody textbody = gs.BeginGroupField.OwnerParagraph.Owner as WTextBody;
            if (startParaIndex == endParaIndex)
            {
                //Remove items between start and end index
                RemoveItems((textbody.Items[startParaIndex] as WParagraph), startIndex, endIndex);
            }
            else if ((textbody.Items[startParaIndex] as WParagraph).Items.Count > 1 && startIndex > 0)
            {
                //Removes items in paragraph.
                RemoveItems((textbody.Items[startParaIndex] as WParagraph), startIndex, (textbody.Items[startParaIndex] as WParagraph).Items.Count);
                //Removes items in textbody.
                RemoveItems(textbody, startParaIndex + 1, endParaIndex);

                //Removes paragraph if the end index is the last item of paragraph
                if (endIndex == (textbody.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                    textbody.Items.RemoveAt(startParaIndex + 1);
                else if ((textbody.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
                {
                    //Removes items in paragraph
                    RemoveItems((textbody.Items[startParaIndex + 1] as WParagraph), 0, endIndex + 1);
                }
            }
            else
            {
                //Removes items in textbody.
                RemoveItems(textbody, startParaIndex, endParaIndex);
                //Removes paragraph if the end index is the last item of paragraph
                if (endIndex == (textbody.Items[startParaIndex] as WParagraph).Items.Count - 1)
                    textbody.Items.RemoveAt(startParaIndex);
                else if ((textbody.Items[startParaIndex] as WParagraph).Items.Count > 0)
                {
                    //Removes items in paragraph
                    RemoveItems((textbody.Items[startParaIndex] as WParagraph), 0, endIndex + 1);
                }
            }
        }
        /// <summary>
        /// Removes items present between empty group present in Table.
        /// </summary>
        /// <param name="gs">The group selector</param>
        private void EmptyGroupInTable(GroupSelector gs)
        {
            //Remove go back bookmarks from selection
            RemoveGoBackBookmark(gs);
            int startIndex = gs.BeginGroupField.GetIndexInOwnerCollection();
            int endIndex = gs.EndGroupField.GetIndexInOwnerCollection();
            int startParaIndex = (gs.BeginGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();
            int startCellIndex = (gs.BeginGroupField.Owner.Owner as WTableCell).GetCellIndex();
            int endCellIndex = (gs.EndGroupField.Owner.Owner as WTableCell).GetCellIndex();
            int startRowIndex = (gs.BeginGroupField.Owner.Owner.Owner as WTableRow).GetRowIndex();
            int endRowIndex = (gs.EndGroupField.Owner.Owner.Owner as WTableRow).GetRowIndex();
            int endParaIndex = (gs.EndGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();

            WTable table = gs.BeginGroupField.Owner.Owner.Owner.Owner as WTable;
            RemoveItemsAfterTableStart(gs, table, startIndex, startParaIndex, startCellIndex, endCellIndex, startRowIndex, endRowIndex);
            if (startRowIndex != endRowIndex)
                RemoveItemsAtTableEnd(gs, table, endIndex, endParaIndex, startCellIndex, endCellIndex, startRowIndex, endRowIndex);
            else
            {
                //Removes items if start row index and end rom index are same.
                if (startIndex == 0 && startParaIndex == 0)
                {
                    //Removes the row if end para index and end index are the last element of cell and paragraph respectively.
                    if (endParaIndex == table.Rows[startRowIndex].Cells[startCellIndex].Items.Count - 1 && endIndex == (table.Rows[startRowIndex].Cells[startCellIndex].Items[endParaIndex] as WParagraph).Items.Count - 1)
                    {
                        table.Rows.RemoveAt(startRowIndex);
                    }
                    else
                    {
                        //Remove items in paragraph
                        RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex].Items[endParaIndex], 0, endIndex);
                        //Remove items in cell
                        RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex], 0, endParaIndex);
                    }
                }
            }
        }
        /// <summary>
        /// Removes GoBack bookmark from selected group
        /// </summary>
        /// <param name="gs"></param>
        private void RemoveGoBackBookmark(GroupSelector gs)
        {
            //Remove Bookmark start
            for (int i = 0; i < gs.BeginGroupField.OwnerParagraph.Items.Count; i++)
            {
                ParagraphItem item = gs.BeginGroupField.OwnerParagraph.Items[i] as ParagraphItem;
                if ((item is BookmarkStart && (item as BookmarkStart).Name.ToLower() == "_goback") ||
                    (item is BookmarkEnd && (item as BookmarkEnd).Name.ToLower() == "_goback"))
                { item.RemoveSelf(); i--; }
            }
            //Remove Bookmark end
            for (int i = 0; i < gs.EndGroupField.OwnerParagraph.Items.Count; i++)
            {
                ParagraphItem item = gs.EndGroupField.OwnerParagraph.Items[i] as ParagraphItem;
                if ((item is BookmarkStart && (item as BookmarkStart).Name.ToLower() == "_goback") ||
                    (item is BookmarkEnd && (item as BookmarkEnd).Name.ToLower() == "_goback"))
                { item.RemoveSelf(); i--; }
            }
        }
        /// <summary>
        /// Removes items present in Group after Table Start field
        /// </summary>
        /// <param name="gs">The group selector</param>
        /// <param name="table">Table</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="startParaIndex">Start para index</param>
        /// <param name="startCellIndex">Start cell index</param>
        /// <param name="endCellIndex">End cell index</param>
        /// <param name="startRowIndex">Start row index</param>
        /// <param name="endRowIndex">End row index</param>
        private void RemoveItemsAfterTableStart(GroupSelector gs, WTable table, int startIndex, int startParaIndex, int startCellIndex, int endCellIndex, int startRowIndex, int endRowIndex)
        {
            if (startIndex == 0)
            {
                if (startParaIndex == 0)
                {
                    if (startCellIndex == 0)
                    {
                        if (startRowIndex != endRowIndex)
                        {
                            //Remove items in table
                            RemoveItems(table, startRowIndex, endRowIndex);
                        }
                        else if (startCellIndex == endCellIndex)
                        {
                            //Removes the cell start cell index and end cell index are the same.
                            table.Rows[startRowIndex].Cells.RemoveAt(startCellIndex);
                        }
                        else
                            //Remove items in row
                            RemoveItems(table.Rows[startRowIndex], startCellIndex, endCellIndex);
                    }
                    else
                    {
                        if (startRowIndex != endRowIndex)
                        {
                            if (startCellIndex == table.Rows[startRowIndex].Cells.Count - 1)
                            {
                                table.Rows[startRowIndex].Cells.RemoveAt(startCellIndex);
                            }
                            else
                                //Remove items in row
                                RemoveItems(table.Rows[startRowIndex], startCellIndex, table.Rows[startRowIndex].Cells.Count - 1);

                            //Remove items in table
                            if (startRowIndex + 1 < endRowIndex)
                                RemoveItems(table, startRowIndex + 1, endRowIndex);
                        }
                        else
                        {
                            //Removes the cell start cell index and end cell index are the same.
                            if (startCellIndex == endCellIndex)
                            {
                                table.Rows[startRowIndex].Cells.RemoveAt(startCellIndex);
                            }
                            else
                                //Remove items in row
                                RemoveItems(table.Rows[startRowIndex], startCellIndex, endCellIndex);
                        }
                    }
                }
                else
                    //Remove items in cell
                    RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex], startParaIndex, table.Rows[startRowIndex].Cells[startCellIndex].Items.Count);
            }
            else
            {
                //Remove items in paragraph
                RemoveItems((table.Rows[startRowIndex].Cells[startCellIndex].Items[startParaIndex] as WParagraph), startIndex, (table.Rows[startRowIndex].Cells[startCellIndex].Items[startParaIndex] as WParagraph).Items.Count);
                //Remove items in cell
                RemoveItems(table.Rows[startRowIndex].Cells[startCellIndex], startParaIndex + 1, table.Rows[startRowIndex].Cells[startCellIndex].Items.Count);
                if (startRowIndex == endRowIndex)
                    //Remove items in row till end cell index if start and end row index are same.
                    RemoveItems(table.Rows[startRowIndex], startCellIndex + 1, endCellIndex);
                else
                    //Remove items in row 
                    RemoveItems(table.Rows[startRowIndex], startCellIndex + 1, table.Rows[startRowIndex].Cells.Count);
                //Remove items in table
                RemoveItems(table, startRowIndex + 1, endRowIndex);
            }
        }
        /// <summary>
        /// Removes Items present in group at table end
        /// </summary>
        /// <param name="gs">The group selector</param>
        /// <param name="table">Table</param>
        /// <param name="endIndex">End index</param>
        /// <param name="endParaIndex">End para index</param>
        /// <param name="startCellIndex">Start cell index</param>
        /// <param name="endCellIndex">End cell index</param>
        /// <param name="startRowIndex">Start row index</param>
        /// <param name="endRowIndex">End row index</param>
        private void RemoveItemsAtTableEnd(GroupSelector gs, WTable table, int endIndex, int endParaIndex, int startCellIndex, int endCellIndex, int startRowIndex, int endRowIndex)
        {
            if ((startRowIndex != endRowIndex) && (endIndex == (table.Rows[startRowIndex + 1].Cells[endCellIndex].Items[endParaIndex] as WParagraph).Items.Count - 1))
            {
                if (endParaIndex == table.Rows[startRowIndex + 1].Cells[endCellIndex].Items.Count - 1)
                {
                    if (endCellIndex == table.Rows[startRowIndex + 1].Cells.Count - 1)
                    {
                        //Remove the row if end cell index is the last cell in row
                        table.Rows.RemoveAt(startRowIndex + 1);
                    }
                    else
                    {
                        //Remove items in row
                        RemoveItems(table.Rows[startRowIndex + 1], 0, endCellIndex + 1);
                    }
                }
                else
                {
                    //Remove items in cell
                    RemoveItems(table.Rows[startRowIndex + 1].Cells[endCellIndex], 0, endParaIndex + 1);
                    //Remove items in row
                    RemoveItems(table.Rows[startRowIndex + 1], 0, endCellIndex);
                }
            }
            else
            {
                //Remove items in paragraph
                RemoveItems((table.Rows[startRowIndex + 1].Cells[endCellIndex].Items[endParaIndex] as WParagraph), 0, endIndex + 1);
                //Remove items in cell
                RemoveItems(table.Rows[startRowIndex + 1].Cells[endCellIndex], 0, endParaIndex);
                //Remove items in row
                RemoveItems(table.Rows[startRowIndex + 1], 0, endCellIndex);
            }
        }
        /// <summary>
        /// Remove items between empty group present in table cell.
        /// </summary>
        /// <param name="gs">The group selector</param>
        private void EmptyGroupInTableCell(GroupSelector gs)
        {
            int startIndex = gs.BeginGroupField.GetIndexInOwnerCollection();
            int startParaIndex = (gs.BeginGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();
            int endIndex = gs.EndGroupField.GetIndexInOwnerCollection();
            int endParaIndex = (gs.EndGroupField as ParagraphItem).OwnerParagraph.GetIndexInOwnerCollection();

            WTableCell cell = gs.BeginGroupField.OwnerParagraph.Owner as WTableCell;
            if ((cell.Items[startParaIndex] as WParagraph).Items.Count > 1 && startIndex > 0)
            {
                //Remove items in paragraph
                RemoveItems((cell.Items[startParaIndex] as WParagraph), startIndex, (cell.Items[startParaIndex] as WParagraph).Items.Count);
                //Remove items in cell
                RemoveItems(cell, startParaIndex + 1, endParaIndex);
                //Remove paragraph if end index is the last item in paragraph
                if (endIndex == (cell.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                    cell.Items.RemoveAt(startParaIndex + 1);
                else if ((cell.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
                {
                    //Remove items in paragraph
                    RemoveItems((cell.Items[startParaIndex + 1] as WParagraph), 0, endIndex);
                }
            }
            else
            {
                //Remove items in cell
                RemoveItems(cell, startParaIndex, endParaIndex);
                if (startParaIndex != endParaIndex)
                {
                    //Remove paragraph if end index is the last item in paragraph
                    if (endIndex == (cell.Items[startParaIndex + 1] as WParagraph).Items.Count - 1)
                        cell.Items.RemoveAt(startParaIndex + 1);
                    else if ((cell.Items[startParaIndex + 1] as WParagraph).Items.Count > 0)
                    {
                        //Remove items in paragraph
                        RemoveItems((cell.Items[startParaIndex + 1] as WParagraph), 0, (cell.Items[startParaIndex + 1] as WParagraph).Items.Count + 1);
                    }
                }
                else
                {
                    //Remove paragraph if end index is the last item in paragraph
                    if (endIndex == (cell.Items[startParaIndex] as WParagraph).Items.Count - 1)
                        cell.Items.RemoveAt(startParaIndex);
                    else if ((cell.Items[startParaIndex] as WParagraph).Items.Count > 0)
                    {
                        //Remove items in paragraph
                        RemoveItems((cell.Items[startParaIndex] as WParagraph), 0, endIndex + 1);
                    }
                }
            }
        }
        /// <summary>
        /// Removes items from start index to end index.
        /// </summary>
        /// <param name="ent">Entity</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        private void RemoveItems(Entity ent, int startIndex, int endIndex)
        {
            switch (ent.EntityType)
            {
                case EntityType.Paragraph:
                    {
                        for (int i = startIndex; i < endIndex; i++)
                            (ent as WParagraph).Items.RemoveAt(startIndex);
                    }
                    break;
                case EntityType.TableCell:
                    {
                        for (int i = startIndex; i < endIndex; i++)
                            (ent as WTableCell).Items.RemoveAt(startIndex);
                    }
                    break;
                case EntityType.TableRow:
                    {
                        for (int i = startIndex; i < endIndex; i++)
                            (ent as WTableRow).Cells.RemoveAt(startIndex);
                    }
                    break;
                case EntityType.Table:
                    {
                        for (int i = startIndex; i < endIndex; i++)
                            (ent as WTable).Rows.RemoveAt(startIndex);
                    }
                    break;
                case EntityType.TextBody:
                    {
                        for (int i = startIndex; i < endIndex; i++)
                            (ent as WTextBody).Items.RemoveAt(startIndex);
                    }
                    break;
            }
        }
        /// <summary>
        /// Called when body group found.
        /// </summary>
        /// <param name="rowsEnum">The rows enum.</param>
        private void OnBodyGroupFound(IRowsEnumerator rowsEnum)
        {
            GroupSelector gs = m_groupSelector;

            // Execute for text selection
            TextBodyPart bodyPart = new TextBodyPart();
            TextBodySelection bodySel = gs.GroupSelection;
            bodyPart.Copy(bodySel);
            rowsEnum.Reset();

            RemoveBookMarks(bodyPart);
            while (rowsEnum.NextRow())
            {
                if (m_conn != null || m_dataSet != null || m_dataSetDocIO != null)
                {
                    UpdateEnum(gs.GroupName, rowsEnum);
                }
                // Count items number before mail merge.
                int beforeItemsNum = bodySel.TextBody.Items.Count;

                ExecuteGroupForSelection(bodySel.TextBody,
                  bodySel.ItemStartIndex, bodySel.ItemEndIndex,
                  bodySel.ParagraphItemStartIndex, bodySel.ParagraphItemEndIndex,
                  rowsEnum);
                m_mergedRecordCount++;
                bodySel.ItemEndIndex += bodySel.TextBody.Items.Count - beforeItemsNum;

                if (rowsEnum.IsLast)
                {
                    if (m_bIsNested)
                    {
                        NestedEnums.Remove(gs.GroupName);
                    }
                    break;
                }

                // Correct item end index
                if (bodySel.ItemStartIndex == bodySel.ItemEndIndex)
                    bodyPart.PasteAt(bodySel.TextBody, bodySel.ItemEndIndex, bodySel.ParagraphItemEndIndex + 1);
                else
                    bodyPart.PasteAt(bodySel.TextBody, bodySel.ItemEndIndex, bodySel.ParagraphItemEndIndex);

                int pItemEndShift = 0;
                if (bodyPart.BodyItems.Count > 0 && bodyPart.BodyItems[bodyPart.BodyItems.Count - 1] is WParagraph)
                    pItemEndShift = (bodyPart.BodyItems[bodyPart.BodyItems.Count - 1] as WParagraph).Items.Count - 1;

                bodySel.ShiftStartToEnd(bodyPart.BodyItems.Count - 1, pItemEndShift);
            }
        }
        /// <summary>
        /// Removes the bookmark from the TextbodyPart
        /// </summary>
        /// <param name="txtBodyPart"></param>
        private void RemoveBookMarks(TextBodyPart txtBodyPart)
        {
            for (int i = 0; i < txtBodyPart.BodyItems.Count; i++)
            {
                DeleteBoookmarks(txtBodyPart.BodyItems[i]);
            }
        }
        /// <summary>
        /// Delete bookmarks
        /// </summary>
        /// <param name="entity"></param>
        private void DeleteBoookmarks(IEntity entity)
        {
            if (entity.IsComposite)
            {
                for (int i = (entity as ICompositeEntity).ChildEntities.Count - 1; i >= 0; i--)
                {
                    DeleteBoookmarks((entity as ICompositeEntity).ChildEntities[i]);
                }
            }
            else
            {
                if ((entity.EntityType == EntityType.BookmarkStart) || (entity.EntityType == EntityType.BookmarkEnd))
                {
                    WParagraph ownerPara = entity.Owner as WParagraph;
                    ownerPara.Items.Remove(entity);
                }
            }
        }
        /// <summary>
        /// Called when row group found.
        /// </summary>
        /// <param name="rowsEnum">The rows enum.</param>
        private void OnRowGroupFound(IRowsEnumerator rowsEnum)
        {
            GroupSelector gs = m_groupSelector;

            // Execute for row selection
            WTable table = gs.RowSelection.Table;
            int startIndex = gs.RowSelection.StartRowIndex;
            int endIndex = gs.RowSelection.EndRowIndex;
            int rowsCntBefore = table.Rows.Count;
            int rowIndex = startIndex;
            int shift = 0;

            if (m_bIsNested)
            {
                VerifyNestedGroups(startIndex, endIndex, table);
            }
            int count = endIndex - startIndex + 1;

            WTableRow[] tableRows = new WTableRow[count];

            int index = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                tableRows[index] = table.Rows[i].Clone();
                index += 1;
            }

            rowsEnum.Reset();

            while (rowsEnum.NextRow())
            {
                if (m_conn != null || m_dataSet != null || m_dataSetDocIO != null)
                {
                    UpdateEnum(gs.GroupName, rowsEnum);
                }

                shift = ExecuteGroupForRowSelection(table, rowIndex, count, rowsEnum);
                m_mergedRecordCount++;
                if (rowsEnum.IsLast)
                {
                    if (m_bIsNested)
                        NestedEnums.Remove(gs.GroupName);
                    break;
                }

                rowIndex += shift;
                for (int j = 0; j < count; j++)
                {
                    table.Rows.Insert(rowIndex + j, tableRows[j].Clone());
                }
            }

            gs.RowSelection.StartRowIndex = rowIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowsEnum"></param>
        private void ExecuteGroup(IRowsEnumerator rowsEnum)
        {
            Document.IsMailMerge = true;
            RemoveSpellChecking();

            WSection sec = null;
            for (int i = 0, cnt = Document.Sections.Count; i < cnt; i++)
            {
                sec = Document.Sections[i];
                ExecuteGroup(sec, rowsEnum);
            }
            UpdateMergedRecordCount();
            Document.IsMailMerge = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="rowsEnum"></param>
        private void ExecuteGroup(WSection section, IRowsEnumerator rowsEnum)
        {
            m_groupSelector.ProcessGroups(section.Body, rowsEnum);

            // Executes for headers / footers
            for (int i = 0; i < 6; i++)
            {
                WTextBody headerFooter = section.HeadersFooters[i];

                if (headerFooter.Items.Count > 0)
                {
                    m_groupSelector.ProcessGroups(headerFooter, rowsEnum);
                }
            }
        }
        /// <summary>
        /// Executes the group for selection.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <param name="itemStart">The item start.</param>
        /// <param name="itemEnd">The item end.</param>
        /// <param name="pItemStart">The p item start.</param>
        /// <param name="pItemEnd">The p item end.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void ExecuteGroupForSelection(WTextBody textBody, int itemStart, int itemEnd, int pItemStart, int pItemEnd, IRowsEnumerator rowsEnum)
        {
            if (itemEnd < 0)
            {
                itemEnd = textBody.Items.Count - 1;
            }

            for (int i = itemStart, end = itemEnd; i <= end; i++)
            {
                TextBodyItem bodyItem = textBody.Items[i];

                switch (bodyItem.EntityType)
                {
                    case EntityType.Table:
                        {
                            WTable table = bodyItem as WTable;

                            for (int j = 0; j < table.Rows.Count; j++)
                            {
                                ExecuteGroupForRowSelection(table, j, 1, rowsEnum);
                            }
                        }
                        break;
                    case EntityType.Paragraph:
                        {
                            WParagraph para = bodyItem as WParagraph;

                            int start = (i == itemStart) ? pItemStart : 0;
                            int endj = (i == end && pItemEnd > -1) ? pItemEnd : para.Items.Count - 1;

                            for (int j = start; j <= endj; j++)
                            {
                                if (para.Items.Count > j && para.Items[j].EntityType == EntityType.TextBox)
                                {
                                    WTextBox txtBox = para.Items[j] as WTextBox;
                                    ExecuteGroupForSelection(txtBox.TextBoxBody, 0, -1, 0, -1, rowsEnum);
                                    continue;
                                }
                                if (para.Items.Count > j && para.Items[j].EntityType == EntityType.AutoShape)
                                {
                                    Shape shape = para.Items[j] as Shape;
                                    ExecuteGroupForSelection(shape.TextBody, 0, -1, 0, -1, rowsEnum);
                                    continue;
                                }
                                WField field = (para.Items.Count > j) ? para.Items[j] as WField : null;
                                if (field == null)
                                    continue;
                                if (field is WMergeField)
                                {
                                    WMergeField mergeField = field as WMergeField;
                                    if (!IsBeginGroup(mergeField) && !IsEndGroup(mergeField))
                                    {
                                        if (mergeField.Prefix.StartsWith("Image"))
                                        {
                                            UpdateImageFieldValue(mergeField, para, rowsEnum);
                                        }
                                        else
                                        {
                                            if (!mergeField.ConvertedToText)
                                                UpdateFieldValue(mergeField, rowsEnum);
                                        }
                                    }
                                    else if (m_bIsNested)
                                    {
                                        if (IsBeginGroup(mergeField) && !NestedEnums.ContainsKey(mergeField.FieldName))
                                        {
                                            // Process nested groups
                                            string tableName = mergeField.FieldName;

                                            if (tableName == string.Empty)
                                                continue;

                                            //if (mergeField.Prefix == "TableStart")
                                            //{
                                            //    ClearFieldsState.Add(tableName, ClearFields);
                                            //    ClearFields = false;
                                            //    //continue;
                                            //}
                                            // Get the new table for mail merge and get enumerator for that table.
                                            // GetEnum method also adds created (just filled) table to the current dataset
                                            IRowsEnumerator tblRowsEnum = GetEnum(tableName);
                                            if (tblRowsEnum == null)
                                                continue;

                                            int beforeItemsCount = textBody.Items.Count;
                                            GroupSelectors.Push(m_groupSelector);

                                            m_groupSelector = new GroupSelector(new GroupSelector.GroupFound(OnGroupFound));
                                            m_groupSelector.ProcessGroups(textBody, tblRowsEnum);
                                            // Define the number of selected items
                                            int selItems = m_groupSelector.SelectedBodyItemsCount;
                                            //Check for Table mail merge group if group end is not available in the same text body
                                            if (mergeField.Prefix == "TableStart" && selItems == -1)
                                            {
                                                //Get owner table
                                                Entity entity = GetTableEntity(mergeField);
                                                if (entity is WTable)
                                                {
                                                    //Process table group for mail merging
                                                    m_groupSelector.ProcessGroups(entity as WTable,
                                                        mergeField.OwnerParagraph.Owner.Owner.GetIndexInOwnerCollection(), //start row index
                                                        (entity as WTable).Rows.Count - 1, //end row index - last row of the table
                                                        tblRowsEnum); // data
                                                }
                                                else //if the owner is not a table and there is no selected body items
                                                    throw new ApplicationException("Group \"" + tableName + "\" is missing in the source document.");
                                            }
                                            else if (selItems == -1)
                                            {
                                                throw new ApplicationException("Group \"" + tableName + "\" is missing in the source document.");
                                            }
                                            else if (selItems > 0)
                                            {
                                                // Define shift made by mail merge
                                                int shift = textBody.Items.Count - beforeItemsCount;
                                                i += shift + selItems - 1;
                                                end += shift;
                                                itemEnd = end;
                                            }
                                            else
                                            {
                                                HideField(mergeField, false);
                                            }
                                            // Remove filled before table from 
                                            if (m_curDataSet != null)
                                                CurrentDataSet.Tables.Remove(tableName);
                                            else if (m_curDataSetDocIO != null)
                                                CurrentDataSetDocIO.RemoveDataTable(tableName);
                                            // Pop previous GroupSelector with selected before items
                                            m_groupSelector = GroupSelectors.Pop();
                                            break;
                                        }
                                    }
                                    else if ((IsBeginGroup(mergeField) || IsEndGroup(mergeField))
                                        && m_bClearFields)
                                    {
                                        HideField(mergeField, true);
                                    }
                                }
                                else if (field is WIfField)
                                {
                                    UpdateIfFieldValue(field as WIfField, rowsEnum);
                                }
                                else if (field.FieldType == FieldType.FieldNext)
                                {
                                    if (rowsEnum != null && !rowsEnum.IsEnd)
                                    {
                                        rowsEnum.NextRow();
                                    }
                                    // Erase NEXT field
                                    HideField(field, true);
                                }
                                else if (field.FieldType == FieldType.FieldNextIf)
                                {
                                    if (field.UpdateNextIfField()
                                        && rowsEnum != null
                                        && !rowsEnum.IsEnd)
                                        rowsEnum.NextRow();
                                    // Removes NEXTIF field
                                    para.Items.Remove(field);
                                    endj = para.Items.Count - 1;
                                    j--;
                                }
                                else if (field.FieldType == FieldType.FieldMergeRec
                                    || field.FieldType == FieldType.FieldMergeSeq)
                                {
                                    // Converts MergeSequence and MergeRecord field to text.
                                    int mergeRecordCount = 1;
                                    if (rowsEnum != null)
                                        mergeRecordCount += rowsEnum.CurrentRowIndex;
                                    if (!(field.OwnerParagraph.IsInCell && ClearFieldsState.Count > 0 &&
                                        !ClearFieldsState.ContainsKey(rowsEnum.TableName)))
                                        ConvertToText(field, mergeRecordCount.ToString());
                                }
                            }

                            if (m_bRemoveEmptyPara)
                            {
                                RemoveEmptyPara(para);
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Get Table object if the given entity is available within the table
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Entity GetTableEntity(Entity entity)
        {
            Entity baseEntity = entity;
            do
            {
                if (baseEntity.Owner == null)
                    return baseEntity;
                baseEntity = baseEntity.Owner;
            }
            while (!(baseEntity is WTable));

            return baseEntity;
        }
        /// <summary>
        /// Executes the group for row selection.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="count">The count.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        /// <returns>The number of rows that took part in mail merge</returns>
        private int ExecuteGroupForRowSelection(WTable table, int startRowIndex, int count, IRowsEnumerator rowsEnum)
        {
            int rowsCntBefore = table.Rows.Count;
            int endRowIndex = startRowIndex + count - 1;
            WTableRow row = null;
            WTableCell cell = null;
            //Get row count
            int rowCount = table.Rows.Count;
            for (int j = startRowIndex;
                //If endrow index is greater than row count, then use row count as the limit
                j <= (table.Rows.Count > endRowIndex ? endRowIndex : table.Rows.Count - 1);
                j++)
            {
                row = table.Rows[j];
                //Get cell count
                int cellCount = row.Cells.Count;
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cell = row.Cells[i];
                    ExecuteGroupForSelection(cell, 0, -1, 0, -1, rowsEnum);
                    //Update cell count and index i if there is any change in the cell count due to Empty group functionality
                    if (cellCount > row.Cells.Count)
                    {
                        i--;
                        cellCount = row.Cells.Count;
                    }
                }
                //Update row count and index j if there is any change in the row count due to Empty group functionality
                if (rowCount > table.Rows.Count)
                {
                    j--;
                    rowCount = table.Rows.Count;
                }
            }
            //Update end row index if the row count is lesser than the end row index
            endRowIndex = table.Rows.Count > endRowIndex ? endRowIndex : table.Rows.Count - 1;
            int shift = 0;
            if (m_bIsNested)
            {
                // Execute nested 
                string nestedGr = FindNestedGroup(startRowIndex, endRowIndex, table);
                int rowsCnt = table.Rows.Count;
                if (nestedGr != null
                    && ClearFieldsState.ContainsKey(nestedGr))
                {
                    ClearFields = ClearFieldsState[nestedGr];
                    ClearFieldsState.Remove(nestedGr);
                }
                while (nestedGr != null)
                {
                    shift = table.Rows.Count - rowsCnt;
                    endRowIndex += shift;
                    startRowIndex += shift;
                    rowsCnt = table.Rows.Count;
                    IRowsEnumerator tblRowsEnum = GetEnum(nestedGr);
                    if (tblRowsEnum != null)
                    {
                        // Store current group selector
                        GroupSelectors.Push(m_groupSelector);
                        // Create new one based new row enumerator
                        m_groupSelector = new GroupSelector(new GroupSelector.GroupFound(OnGroupFound));
                        m_groupSelector.ProcessGroups(table, startRowIndex, endRowIndex, tblRowsEnum);

                        if (m_curDataSet != null)
                            CurrentDataSet.Tables.Remove(nestedGr);
                        else if (m_curDataSetDocIO != null)
                            CurrentDataSetDocIO.RemoveDataTable(nestedGr);
                        // Pop previous GroupSelector with selected before items
                        m_groupSelector = GroupSelectors.Pop();
                    }

                    // Get nested group name and check whether it was already mail merged.
                    string prevNestedGr = nestedGr;
                    nestedGr = FindNestedGroup(startRowIndex, endRowIndex, table);
                    if (prevNestedGr == nestedGr)
                        break;
                }
            }

            shift = table.Rows.Count - rowsCntBefore;
            return count + shift;
        }
        #endregion

        #region Implementation / nested
        /// <summary>
        /// Executes the nested group.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        private void ExecuteNestedGroup(string tableName)
        {
            IRowsEnumerator rowsEnum = GetEnum(tableName);

            if (rowsEnum == null)
                return;

            WSection section = null;
            for (int i = 0, cnt = Document.Sections.Count; i < cnt; i++)
            {
                section = Document.Sections[i];
                ExecuteGroup(section, rowsEnum);
            }
            UpdateMergedRecordCount();
            // Removes temporary filled table from current dataset.
            if (m_curDataSet != null)
                CurrentDataSet.Tables.Remove(tableName);
            else if (m_curDataSetDocIO != null)
                CurrentDataSetDocIO.RemoveDataTable(tableName);
        }
        /// <summary>
        /// Gets the enumerator for nested mail merge.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private IRowsEnumerator GetEnum(string tableName)
        {
            IRowsEnumerator rowsEnum = null;

            object obj = GetDataTable(tableName);
            if (obj is DataTable)
            {
                DataTable table = obj as DataTable;
                CurrentDataSet.Tables.Add(table);
                rowsEnum = new DataTableEnumerator(table);
                rowsEnum.Reset();
            }
            else if (obj is MailMergeDataTable)
            {
                MailMergeDataTable table = obj as MailMergeDataTable;
                CurrentDataSetDocIO.Add(table);
                rowsEnum = new DataTableEnumerator(table);
                rowsEnum.Reset();
            }
            return rowsEnum;
        }
        /// <summary>
        /// Updates the enum.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void UpdateEnum(string tableName, IRowsEnumerator rowsEnum)
        {
            if (!NestedEnums.ContainsKey(tableName))
            {
                NestedEnums.Add(tableName, rowsEnum);
            }
            else
            {
                NestedEnums[tableName] = rowsEnum;
            }
        }
        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        private object GetDataTable(string tableName)
        {
            if (m_conn != null)
                return GetDataTableConn(tableName);
            else if (m_dataSetDocIO != null)
                return GetDataTable(tableName, m_dataSetDocIO);
            else
                return GetDataTableDSet(tableName);
        }
        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        private MailMergeDataTable GetDataTable(string tableName, MailMergeDataSet dataSet)
        {
            MailMergeDataTable table = dataSet.GetDataTable(tableName);
            if (table == null)
                return null;

            string command = GetCommand(tableName);
            if (command == string.Empty)
            {
                return table;
            }
            else
            {
                return table.Select(command);
            }
        }
        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        private string GetCommand(string tableName)
        {
            DictionaryEntry entry = new DictionaryEntry(string.Empty, string.Empty);
            bool found = false;
            if (m_commands != null)
            {
                for (int i = 0, cnt = m_commands.Count; i < cnt; i++)
                {
                    entry = (DictionaryEntry)m_commands[i];
                    if (tableName == (string)entry.Key)
                    {
                        found = true;
                        break;
                    }
                }
            }
            else if (m_commandsDocIO != null)
            {
                for (int i = 0, cnt = m_commandsDocIO.Count; i < cnt; i++)
                {
                    entry = (DictionaryEntry)m_commandsDocIO[i];
                    if (tableName == (string)entry.Key)
                    {
                        found = true;
                        break;
                    }
                }
            }

            // If entry with specified table name is found, get the command.
            if (found)
            {
                string command = (string)entry.Value;
                if (command.IndexOf("%") == -1)
                {
                    return command;
                }
                else
                {
                    return UpdateVarCmd(command);
                }
            }
            return null;
        }
        /// <summary>
        /// Updates the variable part of the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private string UpdateVarCmd(string command)
        {
            MatchCollection matches = VariableCommandRegex.Matches(command);
            if (matches.Count == 0)
                return null;

            char[] splitter = new char[1] { '.' };
            string newCmd = null;
            string varCmd = null;
            string[] cmdParts = null;

            for (int i = 0, cnt = matches.Count; i < cnt; i++)
            {
                varCmd = matches[i].Value.Replace("%", string.Empty);
                cmdParts = varCmd.Split(splitter);
                if (cmdParts.Length != 2)
                    throw new ArgumentException("String value between '%' symbols (variable command) is not valid.");

                IRowsEnumerator rowsEnum = null;
                if (NestedEnums.ContainsKey(cmdParts[0]))
                {
                    rowsEnum = NestedEnums[cmdParts[0]];
                }

                if (rowsEnum == null)
                    return string.Empty;

                newCmd = (rowsEnum.GetCellValue(cmdParts[1])).ToString();
                // Check string value
                if (m_dataSet != null && m_dataSet.Tables.Contains(cmdParts[0]))
                {
                    DataTable table = m_dataSet.Tables[cmdParts[0]];
                    if (table.Columns.Contains(cmdParts[1]))
                    {
                        if (table.Columns[cmdParts[1]].DataType.Name == "String")
                        {
                            newCmd = newCmd.Replace("'", "''");
                            newCmd = "'" + newCmd + "'";
                        }
                    }
                }
                command = command.Replace("%" + varCmd + "%", newCmd);

                //if (m_conn != null)
                //    command = command.Replace("%" + varCmd + "%", newCmd);
                //else
                //    command = cmdParts[1] + " = " + newCmd;
            }

            return command;
        }
        /// <summary>
        /// Checks the table nested groups.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="table">The table.</param>
        private void VerifyNestedGroups(int startRow, int endRow, WTable table)
        {
            Dictionary<string, WMergeField> startGrNames = new Dictionary<string, WMergeField>();
            Dictionary<string, WMergeField> endGrNames = new Dictionary<string, WMergeField>();

            WTableRow row = null;
            WMergeField mergeFld = null;
            for (int i = startRow; i <= endRow; i++)
            {
                row = table.Rows[i] as WTableRow;
                foreach (WTableCell cell in row.Cells)
                    foreach (WParagraph par in cell.Paragraphs)
                        foreach (ParagraphItem item in par.Items)
                            if (item is WMergeField)
                            {
                                mergeFld = item as WMergeField;
                                if (IsBeginGroup(mergeFld) && !mergeFld.ConvertedToText)
                                    startGrNames.Add(mergeFld.FieldName, mergeFld);
                                else if (IsEndGroup(mergeFld) && !mergeFld.ConvertedToText)
                                    endGrNames.Add(mergeFld.FieldName, mergeFld);
                            }
            }

            // Check whether there are only start or end groups
            if (startGrNames.Count == 0)
            {
                if (endGrNames.Count > 0)
                    foreach (string key in endGrNames.Keys)
                        //ClearGroupFld( entry.Value as WMergeField );
                        throw new ApplicationException("GroupEnd field \"" + key + "\" doesn't have GroupStart field equivalent.");
            }
            else if (endGrNames.Count == 0)
            {
                if (startGrNames.Count > 0)
                    foreach (string key in startGrNames.Keys)
                        //ClearGroupFld( entry.Value as WMergeField );
                        throw new ApplicationException("GroupStart field \"" + key + "\" doesn't have GroupEnd field equivalent.");
            }

            // Chack whether there is equivalent for start group in end groups collection.
            foreach (string startGrName in startGrNames.Keys)
            {
                if (!endGrNames.ContainsKey(startGrName))
                    //ClearGroupFld( dEntry.Value as WMergeField );
                    throw new ApplicationException("GroupStart field \"" + startGrName + "\" doesn't have GroupEnd field equivalent.");
                else
                    // If there is an equivalent in endGroupName, remove equivalent field from collection.
                    endGrNames.Remove(startGrName);
            }
            // Hide invalid group fields 
            if (endGrNames.Count > 0)
            {
                foreach (string key in endGrNames.Keys)
                {
                    //ClearGroupFld( ent.Value as WMergeField );
                    throw new ApplicationException("GroupEnd field \"" + key + "\" doesn't have GroupStart field equivalent.");
                }
            }

            startGrNames.Clear();
            endGrNames.Clear();
        }
        /// <summary>
        /// Clears the group FLD.
        /// </summary>
        /// <param name="field">The field.</param>
        private void ClearGroupFld(WMergeField field)
        {
            HideField(field, true);
            field.FieldName = string.Empty;
        }
        /// <summary>
        /// Finds the table groups.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="table">The table.</param>
        /// <returns>The founded nested group name.</returns>
        private string FindNestedGroup(int startRow, int endRow, WTable table)
        {
            WTableRow row = null;
            for (int i = startRow; i <= endRow; i++)
            {
                row = table.Rows[i];
                foreach (WTableCell cell in row.Cells)
                    foreach (WParagraph par in cell.Paragraphs)
                        foreach (ParagraphItem item in par.Items)
                            if (item is WMergeField)
                            {
                                WMergeField mergeFld = item as WMergeField;
                                if (IsBeginGroup(mergeFld) && mergeFld.FieldName != string.Empty)
                                {
                                    string grName = (item as WMergeField).FieldName;
                                    return (grName == string.Empty) ? null : grName;
                                }
                            }
            }

            return null;
        }
        /// <summary>
        /// Gets the data table using connent.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private DataTable GetDataTableConn(string tableName)
        {
            DataTable table = null;
            string command = GetCommand(tableName);

            if (command == null)
                command = "Select * from " + tableName;
            else if (command == string.Empty)
                return null;

            table = new DataTable(tableName);

            DbCommand dbCommand = null;
            DbDataAdapter da = null;

            if (m_isSqlConnection)
            {
                dbCommand = new SqlCommand(command, m_conn as SqlConnection);
                da = new SqlDataAdapter(dbCommand as SqlCommand);
            }
            else
            {
                dbCommand = new OleDbCommand(command, m_conn as OleDbConnection);
                da = new OleDbDataAdapter(dbCommand as OleDbCommand);
            }
            try
            {
                da.Fill(table);
            }
            catch
            {
                return table;
            }

            return table;
        }
        /// <summary>
        /// Gets the data table using data set.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private DataTable GetDataTableDSet(string tableName)
        {
            DataTable table = m_dataSet.Tables[tableName];
            if (table == null)
                return null;

            string command = GetCommand(tableName);
            if (command == null)
                return null;

            DataRow[] rows = null;

            try
            {
                rows = table.Select(command);
            }
            catch
            {
                return null;
            }

            DataTable newTable = new DataTable(tableName);
            foreach (DataColumn column in table.Columns)
            {
                DataColumn newColumn = newTable.Columns.Add(column.ColumnName);
                //Updates the data type of the column.
                newColumn.DataType = column.DataType;
            }
            foreach (DataRow row in rows)
            {
                DataRow newRow = newTable.NewRow();
                newRow.ItemArray = row.ItemArray;
                newRow.RowError = row.RowError;
                newTable.Rows.Add(newRow);
            }

            return newTable;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sends MergeField event.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        /// <returns></returns>
        protected MergeFieldEventArgs SendMergeField(IWMergeField field, object value, IRowsEnumerator rowsEnum)
        {
            MergeFieldEventArgs args = new MergeFieldEventArgs(
              Document as IWordDocument,
              rowsEnum.TableName, rowsEnum.CurrentRowIndex,
              field, value
              );

            if (MergeField != null)
            {
                MergeField(this, args);
            }

            return args;
        }
        /// <summary>
        /// 
        /// </summary>
        protected MergeImageFieldEventArgs SendMergeImageField(IWMergeField field, object bmp, IRowsEnumerator rowsEnum)
        {
            MergeImageFieldEventArgs args = null;
            if (rowsEnum != null)
            {
                args = new MergeImageFieldEventArgs(
                  Document as IWordDocument,
                  rowsEnum.TableName, rowsEnum.CurrentRowIndex,
                  field, bmp
                  );
            }
            else
            {
                args = new MergeImageFieldEventArgs(
                  Document as IWordDocument, null, int.MaxValue, field, bmp
                  );
            }

            if (MergeImageField != null)
            {
                MergeImageField(this, args);
            }

            return args;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowsEnum"></param>
        private void Execute(IRowsEnumerator rowsEnum)
        {
            Document.IsMailMerge = true;
            RemoveSpellChecking();

            if (rowsEnum == null)
                throw new ArgumentNullException("rowsEnum");

            int rowsCount = rowsEnum.RowsCount;
            int lastSecIndex = 0;

            if (rowsCount > 1)
            {
                CopyContent(Document);
            }

            IWSectionCollection sections = Document.Sections;
            rowsEnum.Reset();

            if (rowsEnum.RowsCount == 0 && m_bClearFields)
            {
                for (int j = 0, len = sections.Count; j < len; j++)
                {
                    ExecuteForSection(sections[j], null);
                }
            }
            else
            {
                while (rowsEnum.NextRow())
                {
                    for (int j = lastSecIndex, len = sections.Count; j < len; j++)
                    {
                        ExecuteForSection(sections[j], rowsEnum);
                    }

                    lastSecIndex = sections.Count;
                    if (!rowsEnum.IsLast)
                    {
                        // Begin from second row - appends copied sections
                        AppendCopiedContent(Document);
                    }
                }
            }
            Document.IsMailMerge = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="rowsEnum"></param>
        private void ExecuteForSection(IWSection sec, IRowsEnumerator rowsEnum)
        {
            ExecuteForTextBody(sec.Body.Items, rowsEnum);

            // Executes for headers/footers
            for (int i = 0; i < 6; i++)
            {
                BodyItemCollection paragraphs = (BodyItemCollection)sec.HeadersFooters[i].ChildEntities;

                if (paragraphs.Count > 0)
                {
                    ExecuteForTextBody(paragraphs, rowsEnum);
                }
            }
        }
        /// <summary>
        /// Executes for text body.
        /// </summary>
        /// <param name="bodyItems">The body items.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void ExecuteForTextBody(BodyItemCollection bodyItems, IRowsEnumerator rowsEnum)
        {
            ITextBodyItem item = null;
            for (int index = 0; index < bodyItems.Count; index++)
            {
                item = bodyItems[index];

                //Execute merge field names from textbody item.
                ExecuteForTextBodyItem(item, rowsEnum);

            }
        }
        /// <summary>
        /// Executes for text body item.
        /// </summary>
        /// <param name="bodyItems">The body items.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void ExecuteForTextBodyItem(ITextBodyItem item, IRowsEnumerator rowsEnum)
        {
            if (item is IWParagraph)
            {
                WParagraph paragraph = item as WParagraph;
                ExecuteForParagraph(paragraph, rowsEnum);
            }
            else if (item is IWTable)
            {
                IWTable table = item as IWTable;

                if (table != null)
                {
                    ExecuteForTable(table, rowsEnum);
                }
            }
            else if (item is StructureDocumentTagBlock)
            {
                foreach (ITextBodyItem textBodyItem in (item as StructureDocumentTagBlock).SDTContent.TextBody.ChildEntities)
                {
                    //Iterates the child items of SDTInlineTextBodyItem.
                    ExecuteForTextBodyItem(textBodyItem, rowsEnum);
                }
            }
        }
        /// <summary>
        /// Executes for paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void ExecuteForParagraph(WParagraph paragraph, IRowsEnumerator rowsEnum)
        {
            bool paraItemCollectionChanged = false;
            for (int i = 0, len = paragraph.Items.Count; i < len; i++)
            {
                ParagraphItem pItem = paragraph[i];

                //Execute  merge field names from paragraph item.
                ExecuteForParagraphItems(pItem, paragraph, rowsEnum, ref paraItemCollectionChanged);

                if (paraItemCollectionChanged)
                {
                    len = paragraph.Items.Count;
                    i--;
                    paraItemCollectionChanged = false;
                }
            }

            if (m_bRemoveEmptyPara)
            {
                RemoveEmptyPara(paragraph);
            }
        }
        /// <summary>
        /// Executes for paragraph items.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        /// <param name="pItem">The Paragraph item.</param>
        /// <param name="paraItemCollectionChanged">The indicate whether the paragraph item collection changed or not.</param>
        private void ExecuteForParagraphItems(ParagraphItem pItem, WParagraph paragraph, IRowsEnumerator rowsEnum, ref bool paraItemCollectionChanged)
        {
            WMergeField mergeField = pItem as WMergeField;

            if (mergeField != null)
            {
                if (mergeField.Prefix.StartsWith("Image"))
                {
                    UpdateImageFieldValue(mergeField, paragraph, rowsEnum);
                }
                else
                {
                    if (!mergeField.ConvertedToText)
                        UpdateFieldValue(mergeField, rowsEnum);
                }
            }
            else if (pItem is WField)
            {
                WField field = pItem as WField;
                if (field.FieldType == FieldType.FieldNext)
                {
                    if (rowsEnum != null && !rowsEnum.IsEnd)
                    {
                        rowsEnum.NextRow();
                    }
                    // Erase NEXT field
                    HideField(field, true);
                }
                else if (field.FieldType == FieldType.FieldNextIf)
                {
                    if (field.UpdateNextIfField()
                        && rowsEnum != null
                        && !rowsEnum.IsEnd)
                        rowsEnum.NextRow();
                    // Removes NEXTIF field
                    paragraph.Items.Remove(field);
                    paraItemCollectionChanged = true;
                }
                else if (field.FieldType == FieldType.FieldIf)
                {
                    UpdateIfFieldValue(field as WIfField, rowsEnum);
                }
                else if (field.FieldType == FieldType.FieldMergeRec
                    || field.FieldType == FieldType.FieldMergeSeq)
                {
                    // Converts MergeSequence and MergeRecord field to text.
                    int mergeRecordCount = 1;
                    if (rowsEnum != null)
                        mergeRecordCount += rowsEnum.CurrentRowIndex;
                    ConvertToText(field, mergeRecordCount.ToString());
                }
            }
            else if (pItem is WTextBox)
            {
                WTextBox textBox = pItem as WTextBox;
                ExecuteForTextBody((BodyItemCollection)textBox.TextBoxBody.ChildEntities, rowsEnum);
            }
            else if (pItem is Shape)
            {
                Shape shape = pItem as Shape;
                ExecuteForTextBody((BodyItemCollection)shape.TextBody.ChildEntities, rowsEnum);
            }
            else if (pItem is StructureDocumentTagInline)
            {
                foreach (ParagraphItem paraItem in (pItem as StructureDocumentTagInline).SDTContent.ParagraphItems)
                {
                    //Iterates the child items of StructureDocumentTagInline.
                    ExecuteForParagraphItems(paraItem, paragraph, rowsEnum, ref paraItemCollectionChanged);
                }
            }
        }
        /// <summary>
        /// Executes for table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void ExecuteForTable(IWTable table, IRowsEnumerator rowsEnum)
        {
            WTableRow tableRow = null;
            WTableCell cell = null;
            for (int i = 0, rowLen = table.Rows.Count; i < rowLen; i++)
            {
                tableRow = table.Rows[i];
                for (int j = 0, cellLen = tableRow.Cells.Count; j < cellLen; j++)
                {
                    cell = tableRow.Cells[j];
                    ExecuteForTextBody((BodyItemCollection)cell.ChildEntities, rowsEnum);
                }
            }
        }
        /// <summary>
        /// Converts to text.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="text">The text.</param>
        private void ConvertToText(WField field, string text)
        {
            WTextRange textRange = new WTextRange(Document);
            WParagraph paragraph = field.OwnerParagraph;
            int index = field.GetIndexInOwnerCollection();
            paragraph.Items.Remove(field);
            paragraph.Items.Insert(index, textRange);
            // Updates the character format.
            textRange.CharacterFormat.ImportContainer(field.CharacterFormat);
            textRange.CharacterFormat.CopyProperties(field.CharacterFormat);
            if (textRange.CharacterFormat.Sprms.HasSprm(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec))
                textRange.CharacterFormat.Sprms.RemoveValue(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec);
            // Updates the text.
            textRange.Text = text;
        }
        /// <summary>
        /// Updates the merged record count.
        /// </summary>
        private void UpdateMergedRecordCount()
        {
            for (int i = 0; i < Document.Fields.Count; i++)
            {
                WField field = Document.Fields[i];
                if (field.FieldType == FieldType.FieldMergeRec
                    || field.FieldType == FieldType.FieldMergeSeq)
                {
                    // Updates MergeSequence and MergeRecord field.
                    ConvertToText(field, m_mergedRecordCount.ToString());
                    i--;
                }
            }
        }
        /// <summary>
        /// Updates the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void UpdateFieldValue(IWMergeField field, IRowsEnumerator rowsEnum)
        {
            if (rowsEnum == null)
            {
                UpdateFieldValue(field);
            }
            else
            {
                string columnName = null;
                bool fieldUpdated = false;
                object value = null;

                // Get column name for the collection of mapped fields
                columnName = GetMappedColName(field.FieldName);
                if (columnName != null)
                {
                    value = rowsEnum.GetCellValue(columnName);
                }

                if (value == null)
                {
                    for (int i = 0, len = rowsEnum.ColumnNames.Length; i < len; i++)
                    {
                        columnName = rowsEnum.ColumnNames[i];
                        string fieldNameUpper = field.FieldName.ToUpper();
                        string columnNameUpper = columnName.ToUpper();

                        if (fieldNameUpper == columnNameUpper
                          || fieldNameUpper == "\"" + columnNameUpper + "\"")
                        {
                            // If there is no more data then stop merging
                            value = rowsEnum.GetCellValue(columnName);
                            break;
                        }
                    }
                }

                if (value != null)
                {
                    if (!(field as WMergeField).ConvertedToText)
                    {
                        // Send MergeField event
                        MergeFieldEventArgs args = SendMergeField(field, value, rowsEnum);
                        field.Text = args.Text;
                        (field as WMergeField).ConvertedToText = true;
                    }
                    fieldUpdated = true;
                }

                if ((!fieldUpdated && m_bClearFields))
                {
                    HideField(field, true);
                }
            }
        }
        /// <summary>
        /// Updates if field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="rowsEnum">The rows enum.</param>
        private void UpdateIfFieldValue(WIfField field, IRowsEnumerator rowsEnum)
        {
            if (field.MergeFields.Count == 0)
                return;

            if (rowsEnum == null)
                return;

            string columnName = null;
            for (int i = 0, len = rowsEnum.ColumnNames.Length; i < len; i++)
            {
                columnName = rowsEnum.ColumnNames[i];
                string columnNameUpper = columnName.ToUpper();
                string fieldNameUpper = string.Empty;
                PseudoMergeField mergeField = null;

                for (int j = 0, cnt = field.MergeFields.Count; j < cnt; j++)
                {
                    mergeField = field.MergeFields[j];
                    if (mergeField.Name == null)
                        continue;

                    fieldNameUpper = mergeField.Name.ToUpper();
                    if (fieldNameUpper == columnNameUpper)
                    {
                        // If there is no more data then stop merging
                        object value = rowsEnum.GetCellValue(columnName);
                        mergeField.Value = value.ToString();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="paragraph"></param>
        /// <param name="rowsEnum"></param>
        private void UpdateImageFieldValue(IWMergeField field, IWParagraph paragraph,
          IRowsEnumerator rowsEnum)
        {
            if (rowsEnum == null && MergeImageField == null)
            {
                UpdateFieldValue(field);
            }
            else
            {
                MergeImageFieldEventArgs args = null;
                if (rowsEnum == null && MergeImageField != null)
                {
                    string fieldName = field.FieldName;
                    object value = null;
                    // Get field value
                    for (int i = 0; i < m_names.Length; i++)
                    {
                        if (fieldName != null)
                        {
                            if (m_names[i].ToUpper() == fieldName.ToUpper())
                            {
                                value = m_values[i];
                                break;
                            }
                        }
                    }
                    if (value != null)
                    {
                        Bitmap bmp = GetBitmap(value);
                        if (bmp != null)
                        {
                            value = bmp;
                        }
                    }
                    // Send MergeField event
                    args = SendMergeImageField(field, value, rowsEnum);
                    UpdateMergedPicture(field, paragraph, args);
                }
                else
                {
                    bool fieldUpdated = false;
                    string fieldName = field.FieldName;
                    string columnName = null;
                    object value = null;

                    // Get column name from the collection of mapped fields 
                    columnName = GetMappedColName(fieldName);
                    if (columnName != null)
                    {
                        value = rowsEnum.GetCellValue(columnName);
                    }

                    if (value == null)
                    {
                        for (int i = 0, len = rowsEnum.ColumnNames.Length; i < len; i++)
                        {
                            columnName = rowsEnum.ColumnNames[i];
                            if (columnName.ToUpper() == fieldName.ToUpper())
                            {
                                value = rowsEnum.GetCellValue(columnName);
                                break;
                            }
                        }
                    }

                    if (value != null)
                    {
                        Bitmap bmp = GetBitmap(value);
                        if (bmp != null)
                        {
                            value = bmp;
                        }
                        // Send MergeField event
                        args = SendMergeImageField(field, value, rowsEnum);
                        UpdateMergedPicture(field, paragraph, args);
                        fieldUpdated = true;
                    }

                    if (!fieldUpdated && m_bClearFields)
                    {
                        HideField(field, true);
                    }
                }
            }
        }
        /// <summary>
        /// Updates the merged picture.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="args">The <see cref="Syncfusion.DocIO.DLS.MergeImageFieldEventArgs"/> instance containing the event data.</param>
        private void UpdateMergedPicture(IWMergeField field, IWParagraph paragraph, MergeImageFieldEventArgs args)
        {
            if (args.UseText)
            {
                field.Text = args.Text;
            }
            else
            {
                if (!args.Skip)
                {
                    IWPicture picture;
                    int index = paragraph.Items.IndexOf(field);
                    paragraph.Items.RemoveAt(index);
                    picture = (IWPicture)Document.CreateParagraphItem(ParagraphItemType.Picture);
                    paragraph.Items.Insert(index, picture);
                    if (args.Image != null)
                        (picture as WPicture).LoadImage(args.Image);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        private void UpdateFieldValue(IWMergeField field)
        {
            if (m_bClearFields && m_values == null)
            {
                field.Text = "";
                (field as WMergeField).ConvertedToText = true;
            }
            else
            {
                int valIndex = -1;
                string name = GetMappedColName(field.FieldName);
                // Find value index according to the name.
                if (name != null)
                {
                    for (int j = 0; j < m_names.Length; j++)
                    {
                        if (m_names[j].ToUpper() == name.ToUpper())
                        {
                            valIndex = j;
                            break;
                        }
                    }
                }

                if (valIndex == -1)
                {
                    for (int i = 0; i < m_names.Length; i++)
                    {
                        if (m_names[i].ToUpper() == field.FieldName.ToUpper())
                        {
                            valIndex = i;
                            break;
                        }
                    }
                }

                if (valIndex != -1)
                {
                    // throwing event 
                    MergeFieldEventArgs args = new MergeFieldEventArgs(
                      Document as IWordDocument,
                      "", valIndex,
                      field, m_values[valIndex]
                      );

                    if (MergeField != null)
                    {
                        MergeField(this, args);
                    }

                    field.Text = args.Text;
                    (field as WMergeField).ConvertedToText = true;
                }
                else if (m_bClearFields)
                {
                    HideField(field, true);
                }
            }
        }
        /// <summary>
        /// Copies all document sections to clipboard. 
        /// </summary>
        /// <param name="document"></param>
        private void CopyContent(WordDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_contentSections.Clear();
            document.Sections.CloneTo(m_contentSections);
        }
        /// <summary>
        /// Appends copied sections to end of document. 
        /// </summary>
        /// <param name="document"></param>
        private void AppendCopiedContent(WordDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            IWSection sec = null;
            for (int i = 0, cnt = m_contentSections.Count; i < cnt; i++)
            {
                sec = m_contentSections[i];
                document.Sections.Add(sec.Clone());
            }
        }
        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private Bitmap GetBitmap(object data)
        {
            if (data.GetType() == typeof(byte[]))
            {
                MemoryStream memStream = new MemoryStream((byte[])data);
                try
                {
                    return new Bitmap(memStream);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the field names for paragraph.
        /// </summary>
        /// <param name="fieldsArray">The fields array.</param>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="groupName">Name of the group.</param>
        private void GetFieldNamesForParagraph(ArrayList fieldsArray, TextBodyItem paragraph, string groupName)
        {
            if (paragraph is StructureDocumentTagBlock)
            {
                GetFiledNamesForSDTBlockItems(fieldsArray, paragraph as StructureDocumentTagBlock, groupName);
                return;
            }
            if (paragraph is IWTable)
            {
                WTable table = paragraph as WTable;
                WTableRow row = null;
                WTableCell cell = null;
                TextBodyItem cellItem = null;

                for (int rowIndex = 0, rowCnt = table.Rows.Count; rowIndex < rowCnt; rowIndex++)
                {
                    row = table.Rows[rowIndex];
                    for (int cellIndex = 0, cellCnt = row.Cells.Count; cellIndex < cellCnt; cellIndex++)
                    {
                        cell = row.Cells[cellIndex];
                        for (int parIndex = 0, parCnt = cell.ChildEntities.Count; parIndex < parCnt; parIndex++)
                        {
                            cellItem = cell.Items[parIndex];// Paragraphs[ parIndex ];
                            GetFieldNamesForParagraph(fieldsArray, cellItem, groupName);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0, count = (paragraph as WParagraph).Items.Count; i < count; i++)
                {
                    ParagraphItem pItem = (paragraph as WParagraph)[i];

                    //Gets merge field names from paragraph item.
                    GetFieldNamesForParagraphItems(fieldsArray, pItem, groupName);

                }
            }
        }
        /// <summary>
        /// Gets the field names from paragraph item.
        /// </summary>
        ///<param name="fieldsArray">The fields array.</param>
        /// <param name="pStructureDocumentTagBlocklockContent">Paragraph item</param>
        /// <param name="groupName">Name of the group.</param>
        private void GetFieldNamesForParagraphItems(ArrayList fieldsArray, ParagraphItem item, string groupName)
        {
            if (item is WMergeField)
            {
                WMergeField mField = (item as WMergeField);

                if (mField.FieldName == groupName)
                {
                    if (!m_bBeginGroupFound && IsBeginGroup(mField))
                    {
                        m_bBeginGroupFound = true;
                        m_bEndGroupFound = false;

                    }
                    if (!m_bEndGroupFound && IsEndGroup(mField))
                    {
                        m_bEndGroupFound = true;
                        m_bBeginGroupFound = false;
                    }
                }
                else if (groupName == null || (m_bBeginGroupFound && !m_bEndGroupFound))
                {
                    fieldsArray.Add(mField.FieldName);
                }
            }
            else if (item is WTextBox)
            {
                WTextBox textBox = item as WTextBox;
                int tbxCount = textBox.TextBoxBody.Items.Count;
                for (int index = 0; index < tbxCount; index++)
                {
                    GetFieldNamesForParagraph(fieldsArray, textBox.TextBoxBody.Items[index], groupName);
                }
            }
            else if (item is Shape)
            {
                Shape shape = item as Shape;
                int tbxCount = shape.TextBody.Items.Count;
                for (int index = 0; index < tbxCount; index++)
                {
                    GetFieldNamesForParagraph(fieldsArray, shape.TextBody.Items[index], groupName);
                }
            }
            else if (item is StructureDocumentTagInline)
            {
                foreach (ParagraphItem paraItem in (item as StructureDocumentTagInline).SDTContent.ParagraphItems)
                {
                    //Iterates the child items of StructureDocumentTagInline.
                    GetFieldNamesForParagraphItems(fieldsArray, paraItem, groupName);
                }
            }

        }
        /// <summary>
        /// Iterate StructureDocumentTagBlock items to get the fields name. 
        /// </summary>
        ///<param name="pFieldsArray">The fields array.</param>
        /// <param name="structureDocumentTagBlocklockContent">StructureDocumentTagBlocklockContent</param>
        /// <param name="groupName">Name of the group.</param>
        private void GetFiledNamesForSDTBlockItems(ArrayList fieldsArray, StructureDocumentTagBlock structureDocumentTagBlocklockContent, string groupName)
        {
            for (int i = 0; i < structureDocumentTagBlocklockContent.SDTContent.TextBody.ChildEntities.Count; i++)
            {
                GetFieldNamesForParagraph(fieldsArray, structureDocumentTagBlocklockContent.SDTContent.TextBody.ChildEntities[i] as TextBodyItem, groupName);
            }

        }
        /// <summary>
        /// Determines whether [is start prefix] [the specified field].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        /// 	<c>true</c> if it specifies a begining of the group field, set to <c>true</c>.
        /// </returns>
        private static bool IsBeginGroup(WMergeField field)
        {
            string prefix = field.Prefix;
            return (prefix == "TableStart" || prefix == "BeginGroup");
        }
        /// <summary>
        /// Determines whether [is end group] [the specified field].
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        /// 	If it is a end of the group, set to <c>true</c>.
        /// </returns>
        private static bool IsEndGroup(WMergeField field)
        {
            string prefix = field.Prefix;
            return (prefix == "TableEnd" || prefix == "EndGroup");
        }
        /// <summary>
        /// Checks the selection.
        /// </summary>
        /// <param name="rowsEnum">The rows enum.</param>
        /// <returns></returns>
        private bool CheckSelection(IRowsEnumerator rowsEnum)
        {
            if (rowsEnum.RowsCount > 0)
                return true;

            if (!m_bClearFields && !m_bRemoveEmptyPara)
                return true;

            GroupSelector gs = m_groupSelector;
            if (gs.GroupSelection != null)
            {
                if (m_bClearFields)
                    HideFields(gs.GroupSelection.TextBody.Items);
                if (m_bRemoveEmptyPara)
                {
                    RemoveEmptyPara(gs.GroupSelection.TextBody.Items);
                }
            }
            else if (gs.RowSelection != null)
            {
                int startIndex = gs.RowSelection.StartRowIndex;
                int endIndex = gs.RowSelection.EndRowIndex;
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (gs.RowSelection.Table.Rows.Count > startIndex)
                    {
                        if (m_bClearFields)
                            HideFields(gs.RowSelection.Table.Rows[i]);
                        if (m_bRemoveEmptyPara)
                        {
                            RemoveEmptyPara(gs.RowSelection.Table.Rows[i]);
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Hides the fields.
        /// </summary>
        /// <param name="sections">The sections.</param>
        private void HideFields(IWSectionCollection sections)
        {
            for (int i = 0, len = sections.Count; i < len; i++)
            {
                ExecuteForSection(sections[i], null);
            }
        }
        /// <summary>
        /// Hides the fields.
        /// </summary>
        /// <param name="row">The row.</param>
        private void HideFields(WTableRow row)
        {
            WTableCell cell = null;

            for (int i = 0, cnt = row.Cells.Count; i < cnt; i++)
            {
                cell = row.Cells[i];
                HideFields(cell.Items);
            }
        }
        /// <summary>
        /// Hides the fields.
        /// </summary>
        /// <param name="items">The body items.</param>
        private void HideFields(BodyItemCollection items)
        {
            TextBodyItem item = null;

            for (int i = 0, cnt = items.Count; i < cnt; i++)
            {
                item = items[i];
                if (item is WParagraph)
                {
                    HideFields(item as WParagraph);
                }
                else if (item is WTable)
                {
                    WTable table = item as WTable;
                    for (int j = 0, rowCnt = table.Rows.Count; j < rowCnt; j++)
                    {
                        HideFields(table.Rows[j]);
                    }
                }
            }
        }
        /// <summary>
        /// Hides the fields.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void HideFields(WParagraph para)
        {
            WField field = null;
            for (int i = 0, cnt = para.Items.Count; i < cnt; i++)
            {
                if (para.Items[i] is WField)
                {
                    field = para.Items[i] as WField;
                    if (field.FieldType == FieldType.FieldMergeField || field.FieldType == FieldType.FieldNext)
                    {
                        HideField(field, true);
                    }
                }
                else if (para.Items[i] is WTextBox)
                {
                    HideFields((para.Items[i] as WTextBox).TextBoxBody.Items);
                }
                else if (para.Items[i] is Shape)
                {
                    HideFields((para.Items[i] as Shape).TextBody.Items);
                }
            }
        }
        /// <summary>
        /// Hides the field.
        /// </summary>
        /// <param name="field">The field.</param>
        private void HideField(IWField field, bool hide)
        {
            if ((field as WField).ConvertedToText)
                return;

            field.Text = string.Empty;
            if (field.FieldType == FieldType.FieldNext && hide)
            {
                (field as WField).ConvertedToText = true;
                return;
            }
            if ((field as WMergeField) != null)
            {
                if (IsBeginGroup(field as WMergeField) || IsEndGroup(field as WMergeField) || hide)
                    (field as WMergeField).ConvertedToText = true;
            }
        }
        /// <summary>
        /// Removes the empty paragraph.
        /// </summary>
        /// <param name="para">The paragraph.</param>
        private void RemoveEmptyPara(WParagraph para)
        {
            if (para.Items.Count > 0 && para.Items[0] is WMergeField)
            {
                if (IsEmptyParagraph(para))
                {
                    WTableCell parentCell = para.Owner as WTableCell;
                    if (parentCell != null && parentCell.ChildEntities.Count == 1)
                        para.ChildEntities.Clear();
                    else
                        para.RemoveEmpty = true;
                }
            }
        }
        /// <summary>
        /// Checks whether the paragraph is empty.
        /// </summary>
        /// <param name="para">Paragraph</param>
        /// <returns>Returns true, if the paragraph is empty.</returns>
        private bool IsEmptyParagraph(WParagraph para)
        {
            bool IsEmpty = true;

            for (int i = 0; i < para.Items.Count; i++)
            {
                ParagraphItem item = para.Items[i];
                switch (item.EntityType)
                {
                    case EntityType.MergeField:
                        WMergeField mergeField = para.Items[i] as WMergeField;
                        if (!(mergeField.ConvertedToText && mergeField.Text == string.Empty))
                        {
                            IsEmpty = false;
                        }
                        break;
                    case EntityType.Picture:
                        IsEmpty = false;
                        break;
                    default:
                        IsEmpty = false;
                        break;
                }
            }
            return IsEmpty;
        }
        /// <summary>
        /// Removes the empty paragraph.
        /// </summary>
        /// <param name="row">The row.</param>
        private void RemoveEmptyPara(WTableRow row)
        {
            WTableCell cell = null;

            for (int i = 0, cnt = row.Cells.Count; i < cnt; i++)
            {
                cell = row.Cells[i];
                RemoveEmptyPara(cell.Items);
            }
        }
        /// <summary>
        /// Removes the empty paragraphs.
        /// </summary>
        /// <param name="items">The items.</param>
        private void RemoveEmptyPara(BodyItemCollection items)
        {
            TextBodyItem item = null;

            for (int i = 0, cnt = items.Count; i < cnt; i++)
            {
                item = items[i];
                if (item is WParagraph)
                {
                    if (RemoveEmptyPara(items, i))
                    {
                        i -= 1;
                        cnt -= 1;
                    }
                }
                else if (item is WTable)
                {
                    WTable table = item as WTable;
                    for (int j = 0, rowCnt = table.Rows.Count; j < rowCnt; j++)
                    {
                        RemoveEmptyPara(table.Rows[j]);
                    }
                }
            }
        }
        /// <summary>
        /// Removes the empty paragraph.
        /// </summary>
        /// <param name="paragraphs">The paragraphs.</param>
        /// <param name="paraIndex">Index of the paragraph.</param>
        /// <returns></returns>
        private bool RemoveEmptyPara(BodyItemCollection paragraphs, int paraIndex)
        {
            WParagraph para = paragraphs[paraIndex] as WParagraph;

            if (para.Items.Count > 0 && para.Text == string.Empty)
            {
                if (para.Items[0] is WMergeField)
                {
                    paragraphs.Remove(para);
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Gets the  name of the mapped column name.
        /// </summary>
        /// <param name="fieldName">Name of the merge field.</param>
        /// <returns></returns>
        private string GetMappedColName(string fieldName)
        {
            if (m_mappedFields != null && m_mappedFields.ContainsKey(fieldName))
            {
                return m_mappedFields[fieldName];
            }
            return null;
        }
        /// <summary>
        /// Removes the spell checking.
        /// </summary>
        private void RemoveSpellChecking()
        {
            if (Document.GrammarSpellingData != null)
            {
                Document.GrammarSpellingData.PlcfgramData = null;
                Document.GrammarSpellingData.PlcfsplData = null;
            }
        }
        #endregion

        #region Internal declarations
        /// <summary>
        /// 
        /// </summary>
        internal class GroupSelector
        {
            #region Fields
            /// <summary>
            /// The internal fields.
            /// </summary>
            private TextBodySelection m_groupSelection;
            private TableRowSelection m_rowSelection;
            private WTextBody m_groupTextBody;
            private WTextBody m_body;
            private WMergeField m_beginGroupField = null;
            private WMergeField m_endGroupField = null;
            private int m_bodyItemIndex = 0;
            private int m_bodyItemStartIndex = -1;
            private int m_paragraphItemIndex = -1;
            private int m_paragraphItemStartIndex = -1;
            private int m_rowIndex = -1;
            private int m_startRowIndex = -1;
            private string m_groupName;
            private GroupFound SendGroupFound;
            private IRowsEnumerator m_rowsEnum;
            private int m_selBodyItemsCnt = -1;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the group selection.
            /// </summary>
            /// <value>The group selection.</value>
            internal TextBodySelection GroupSelection
            {
                get
                {
                    return m_groupSelection;
                }
            }
            /// <summary>
            /// Gets the row selection.
            /// </summary>
            /// <value>The row selection.</value>
            internal TableRowSelection RowSelection
            {
                get
                {
                    return m_rowSelection;
                }
            }
            /// <summary>
            /// Gets the begin group field.
            /// </summary>
            /// <value>The begin group field.</value>
            internal WMergeField BeginGroupField
            {
                get
                {
                    return m_beginGroupField;
                }
            }
            /// <summary>
            /// Gets or sets the end group field.
            /// </summary>
            /// <value>The end group field.</value>
            internal WMergeField EndGroupField
            {
                get
                {
                    return m_endGroupField;
                }
                set
                {
                    m_endGroupField = value;
                }
            }
            /// <summary>
            /// Gets or sets the index of the body item.
            /// </summary>
            /// <value>The index of the body item.</value>
            internal int BodyItemIndex
            {
                get
                {
                    return m_bodyItemIndex;
                }
                set
                {
                    m_bodyItemIndex = value;
                }
            }
            /// <summary>
            /// Gets a value indicating whether group is found.
            /// </summary>
            /// <value>
            /// 	If group is found, set to <c>true</c>.
            /// </value>
            internal bool IsGroupFound
            {
                get
                {
                    return (m_endGroupField != null);
                }
            }
            /// <summary>
            /// Gets the name of the group.
            /// </summary>
            /// <value>The name of the group.</value>
            internal string GroupName
            {
                get
                {
                    return m_groupName;
                }

            }
            /// <summary>
            /// 
            /// </summary>
            internal int SelectedBodyItemsCount
            {
                get
                {
                    return m_selBodyItemsCnt;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="GroupSelector"/> class.
            /// </summary>
            /// <param name="onGroupFound">The on group found.</param>
            internal GroupSelector(GroupFound onGroupFound)
            {
                SendGroupFound += onGroupFound;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Initialize the process.
            /// </summary>
            private void InitProcess(IRowsEnumerator rowsEnum)
            {
                m_groupSelection = null;
                m_rowSelection = null;
                m_beginGroupField = null;
                m_endGroupField = null;
                m_bodyItemIndex = 0;
                m_bodyItemStartIndex = -1;
                m_paragraphItemIndex = -1;
                m_paragraphItemStartIndex = -1;
                m_rowIndex = -1;
                m_selBodyItemsCnt = -1;

                m_rowsEnum = rowsEnum;
                m_groupName = m_rowsEnum.TableName;
            }
            /// <summary>
            /// Processes the groups.
            /// </summary>
            /// <param name="body">The body.</param>
            /// <param name="rowsEnum">The rows enum.</param>
            internal void ProcessGroups(WTextBody body, IRowsEnumerator rowsEnum)
            {
                InitProcess(rowsEnum);
                m_groupTextBody = m_body = body;
                FindInBodyItems(m_body.Items);
            }
            /// <summary>
            /// Processes the groups.
            /// </summary>
            /// <param name="table">The table.</param>
            /// <param name="startRow">The start row.</param>
            /// <param name="endRow">The end row.</param>
            /// <param name="rowsEnum">The rows enumerator.</param>
            internal void ProcessGroups(WTable table, int startRow, int endRow, IRowsEnumerator rowsEnum)
            {
                InitProcess(rowsEnum);
                FindInTable(table, startRow, endRow);
            }
            /// <summary>
            /// Finds inside body items.
            /// </summary>
            /// <param name="bodyItems">The body items.</param>
            private void FindInBodyItems(BodyItemCollection bodyItems)
            {
                for (int i = 0, cnt = bodyItems.Count; i < cnt; i++)
                {
                    TextBodyItem bodyItem = (TextBodyItem)bodyItems[i];
                    m_bodyItemIndex = i;

                    switch (bodyItem.EntityType)
                    {
                        case EntityType.Paragraph:
                            WParagraph para = (WParagraph)bodyItem;
                            for (int j = 0; j < para.Items.Count; j++)
                            {
                                ParagraphItem item = para.Items[j];

                                m_paragraphItemIndex = j;
                                if (item is WTextBox)
                                {
                                    m_bodyItemIndex = 0;
                                    FindInBodyItems((item as WTextBox).TextBoxBody.Items);
                                    m_bodyItemIndex = i;
                                }
                                else if (item is Shape)
                                {
                                    m_bodyItemIndex = 0;
                                    FindInBodyItems((item as Shape).TextBody.Items);
                                    m_bodyItemIndex = i;
                                }
                                else
                                {
                                    CheckItem(item);
                                }

                                if (IsGroupFound)
                                {
                                    if (m_groupSelection != null)
                                    {
                                        i = m_groupSelection.ItemEndIndex;
                                        //Updates the paragraph item end index (Mail group defined as single paragraph).
                                        if (m_groupSelection.ItemStartIndex == m_groupSelection.ItemEndIndex)
                                            j = m_groupSelection.ParagraphItemEndIndex;
                                        cnt = bodyItems.Count;
                                        ClearSelection();
                                    }
                                    else if (m_rowSelection.StartRowIndex == m_rowSelection.EndRowIndex)
                                    {
                                        cnt = bodyItems.Count;
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case EntityType.Table:
                            {
                                WTable table = (WTable)bodyItem;
                                FindInTable(table, 0, table.Rows.Count - 1);
                            }
                            break;
                        case EntityType.AlternateChunk:
                            //TO DO
                            break;
                        case EntityType.StructureDocumentTag:
                            //TO DO
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }
            /// <summary>
            /// Finds the group in the table.
            /// </summary>
            /// <param name="table">The table.</param>
            /// <param name="startRow">The start row.</param>
            /// <param name="endRow">The end row.</param>
            private void FindInTable(WTable table, int startRow, int endRow)
            {
                int rowsCntBefore = table.Rows.Count;
                for (int k = startRow; k <= endRow; k++)
                {
                    WTableRow row = table.Rows[k];
                    m_rowIndex = k;

                    for (int l = 0, cntl = row.Cells.Count; l < cntl; l++)
                    {
                        WTableCell cell = row.Cells[l];
                        FindInBodyItems(cell.Items);

                        if (IsGroupFound)
                        {
                            endRow += table.Rows.Count - rowsCntBefore;
                            k = m_rowSelection.StartRowIndex;
                            ClearSelection();
                            break;
                        }
                    }
                }
            }
            /// <summary>
            /// Clears the selection.
            /// </summary>
            private void ClearSelection()
            {
                m_groupSelection = null;
                m_rowSelection = null;
                m_beginGroupField = null;
                m_endGroupField = null;
            }
            /// <summary>
            /// Checks the item.
            /// </summary>
            /// <param name="item">The item.</param>
            private void CheckItem(ParagraphItem item)
            {
                if (item.EntityType == EntityType.MergeField)
                {
                    WMergeField field = item as WMergeField;

                    if (field.FieldName == m_groupName)
                    {
                        if (m_beginGroupField == null)
                        {
                            if (IsBeginGroup(field))
                            {
                                StartSelection(field);
                            }
                        }
                        else
                        {
                            if (IsEndGroup(field))
                            {
                                EndSelection(field);

                                if (SendGroupFound != null)
                                {
                                    SendGroupFound(m_rowsEnum);
                                }
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Starts the selection.
            /// </summary>
            private void StartSelection(WMergeField field)
            {
                m_beginGroupField = field;
                m_groupTextBody = field.OwnerParagraph.OwnerTextBody;
                m_bodyItemStartIndex = m_bodyItemIndex;
                m_paragraphItemStartIndex = m_paragraphItemIndex;
                m_startRowIndex = m_rowIndex;
            }
            /// <summary>
            /// Ends the selection.
            /// </summary>
            private void EndSelection(WMergeField field)
            {
                m_endGroupField = field;
                WTextBody textBody = field.OwnerParagraph.OwnerTextBody;
                m_selBodyItemsCnt = m_bodyItemIndex - m_bodyItemStartIndex + 1;

                if (textBody == m_groupTextBody)
                {
                    m_groupSelection = new TextBodySelection(textBody,
                      m_bodyItemStartIndex,
                      m_bodyItemIndex,
                      m_paragraphItemStartIndex,
                      m_paragraphItemIndex
                      );
                }
                else if (textBody.EntityType == EntityType.TableCell &&
                  m_groupTextBody.EntityType == EntityType.TableCell &&
                  (m_groupTextBody.Owner as WTableRow).OwnerTable == (textBody.Owner as WTableRow).OwnerTable)
                {
                    UpdateEndSelection(textBody as WTableCell);
                    m_rowSelection = new TableRowSelection(textBody.Owner.Owner as WTable,
                      m_startRowIndex, m_rowIndex);
                }
                else
                {
                    throw new MailMergeException();
                }
            }
            /// <summary>
            /// Updates end of row selection in case cell has vertical merge.
            /// </summary>
            /// <param name="cell">The cell.</param>
            private void UpdateEndSelection(WTableCell cell)
            {
                WTableRow row = cell.OwnerRow;
                bool hasMerge = false;

                foreach (WTableCell tblCell in row.Cells)
                {
                    if (cell.CellFormat.VerticalMerge != CellMerge.None)
                    {
                        hasMerge = true;
                        break;
                    }
                }

                if (!hasMerge)
                    return;

                while (row.NextSibling != null)
                {
                    row = row.NextSibling as WTableRow;
                    hasMerge = false;

                    foreach (WTableCell tblCell in row.Cells)
                    {
                        if (cell.CellFormat.VerticalMerge != CellMerge.None)
                        {
                            hasMerge = true;
                            break;
                        }
                    }

                    if (hasMerge)
                        m_rowIndex += 1;
                    else
                        break;
                }
            }
            #endregion

            #region Internal declarations
            /// <summary>
            /// 
            /// </summary>
            /// <param name="rowsEnum"></param>
            internal delegate void GroupFound(IRowsEnumerator rowsEnum);
            #endregion
        }
        /// <summary>
        /// Represents a row selection.
        /// </summary>
        internal class TableRowSelection
        {
            #region Fields
            internal WTable Table;
            internal int StartRowIndex;
            internal int EndRowIndex;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="TableRowSelection"/> class.
            /// </summary>
            /// <param name="table">The table.</param>
            /// <param name="startRowIndex">Start index of the row.</param>
            /// <param name="endRowIndex">End index of the row.</param>
            internal TableRowSelection(WTable table, int startRowIndex, int endRowIndex)
            {
                Table = table;
                StartRowIndex = startRowIndex;
                EndRowIndex = endRowIndex;
                ValidateIndexes();
            }
            /// <summary>
            /// Validates the indexes.
            /// </summary>
            private void ValidateIndexes()
            {
                if (StartRowIndex < 0 || StartRowIndex >= Table.Rows.Count)
                    throw new ArgumentOutOfRangeException("StartRowIndex");

                if (EndRowIndex < 0 || EndRowIndex >= Table.Rows.Count)
                    throw new ArgumentOutOfRangeException("EndRowIndex");
            }
            #endregion
        }
        #endregion

        #region Silverlight methods
        /// <summary>
        /// Performs mail merge operation.
        /// </summary>
        /// <param name="datatable">MailMergeDataTable</param>
        public void ExecuteGroup(MailMergeDataTable dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("datasource");

            if (dataSource.GroupName == string.Empty)
                Execute(new DataTableEnumerator(dataSource));
            else
                ExecuteGroup(new DataTableEnumerator(dataSource));
        }
        #endregion
    }
    /// <summary>
    /// Represents the Method that handles MergeField event
    /// </summary>
    public delegate void MergeFieldEventHandler(object sender, MergeFieldEventArgs args);
    /// <summary>
    /// Represents the Method that handles MergeImageField event
    /// </summary>
    public delegate void MergeImageFieldEventHandler(object sender, MergeImageFieldEventArgs args);
    /// <summary>
    /// Provides data during MergeField event.
    /// </summary>
    public class MergeFieldEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Represents the document.
        /// </summary>
        private IWordDocument m_doc = null;
        /// <summary>
        /// Represents the Merge field.
        /// </summary>
        private IWMergeField m_field;
        /// <summary>
        /// Represents the Field Value
        /// </summary>
        private object m_fieldValue;
        /// <summary>
        /// Represents the Row Index.
        /// </summary>
        private int m_rowIndex;
        /// <summary>
        /// Represents the Table Name
        /// </summary>
        private string m_tableName;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        public IWordDocument Document
        {
            get
            {
                return m_doc;
            }
        }
        /// <summary>
        /// Gets the Merge field Name.
        /// </summary>
        public string FieldName
        {
            get
            {
                return m_field.FieldName;
            }
        }
        /// <summary>
        /// Gets the Merge Field Value
        /// </summary>
        public object FieldValue
        {
            get
            {
                return m_fieldValue;
            }
        }
        /// <summary>
        /// Gets the Table Name
        /// </summary>
        public string TableName
        {
            get
            {
                return m_tableName;
            }
        }
        /// <summary>
        /// Gets the Row Index.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return m_rowIndex;
            }
        }
        /// <summary>
        /// Gets the Character Format of the field.
        /// </summary>
        public WCharacterFormat CharacterFormat
        {
            get
            {
                return m_field.CharacterFormat;
            }
        }
        /// <summary>
        /// Gets the Text value
        /// </summary>
        public string Text
        {
            get
            {
                if (FieldValue == null)
                {
                    return "";
                }

                return FieldValue.ToString();
            }
            set
            {
                m_fieldValue = value;
            }
        }
        /// <summary>
        /// Gets the current Merge field.
        /// </summary>
        public IWMergeField CurrentMergeField
        {
            get
            {
                return m_field;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Provides data during the MergeField event.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tableName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public MergeFieldEventArgs(IWordDocument doc, string tableName, int rowIndex,
                                    IWMergeField field, object value)
        {
            m_doc = doc;
            m_field = field;
            m_fieldValue = value;
            m_rowIndex = rowIndex;
            m_tableName = tableName;
        }
        #endregion

    }
    /// <summary>
    /// Provides data during MergeImageField event.
    /// </summary>
    public class MergeImageFieldEventArgs : MergeFieldEventArgs
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private bool m_useText = false;
        /// <summary>
        /// 
        /// </summary>
        private Image m_image = null;
        /// <summary>
        /// 
        /// </summary>
        private string m_imageFileName = "";
        /// <summary>
        /// 
        /// </summary>
        private Stream m_imageStream = null;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bSkip;
        #endregion

        #region Class properties
        /// <summary>
        /// Setting to specify if Text should be used.
        /// </summary>
        public bool UseText
        {
            get
            {
                return m_useText;
            }
        }
        /// <summary>
        /// Gets / Sets Image File Name.
        /// </summary>
        public string ImageFileName
        {
            get
            {
                return m_imageFileName;
            }
            set
            {
                m_imageFileName = value;
                LoadImage(m_imageFileName);
            }
        }
        /// <summary>
        /// Gets / Sets Image Stream.
        /// </summary>
        public Stream ImageStream
        {
            get
            {
                return m_imageStream;
            }
            set
            {
                m_imageStream = value;
                LoadImage(m_imageStream);
            }
        }
        /// <summary>
        /// Gets / Sets Image.
        /// </summary>
        public Image Image
        {
            get
            {
                return m_image;
            }
            set
            {
                m_image = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MergeImageFieldEventArgs"/> is skip.
        /// </summary>
        /// <value><c>true</c> if skip; otherwise, <c>false</c>.</value>
        public bool Skip
        {
            get
            {
                return m_bSkip;
            }
            set
            {
                m_bSkip = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Provides data during MergeImageField event.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tableName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="field"></param>
        /// <param name="image"></param>
        public MergeImageFieldEventArgs(IWordDocument doc, string tableName, int rowIndex,
          IWMergeField field, Image image)
            : base(doc, tableName, rowIndex, field, null)
        {
            m_image = image;
        }
        public MergeImageFieldEventArgs(IWordDocument doc, string tableName, int rowIndex,
          IWMergeField field, object obj)
            : base(doc, tableName, rowIndex, field, obj)
        {
            m_image = obj as Image;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void LoadImage(string name)
        {
            m_image = new Bitmap(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void LoadImage(Stream stream)
        {
            m_image = new Bitmap(stream);
        }
        #endregion
    }

    /// <summary>
    /// The EntityEntry class maintains the Entries of Entity class.
    /// </summary>
    public class EntityEntry
    {
        #region Fields
        /// <summary>
        /// Current entity.
        /// </summary>
        public Entity Current;
        /// <summary>
        /// Index value.
        /// </summary>
        public int Index;
        #endregion

        #region Constructor
        /// <summary>
        /// The base class for DLS entities.
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="index"></param>
        public EntityEntry(Entity ent)
        {
            Current = ent;
            Index = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fetches the entries element.
        /// </summary>
        /// <returns>if contains entry, it will return true. Otherwise, it returns false.</returns>
        public bool Fetch()
        {
            if (Current != null && Current.Owner != null && Current.Owner.IsComposite)
            {
                ICompositeEntity ce = Current.Owner as ICompositeEntity;

                if (ce.ChildEntities.Count > Index + 1)
                {
                    Index += 1;
                    Current = ce.ChildEntities[Index];
                    return true;
                }

            }

            Current = null;
            Index = -1;
            return false;
        }
        #endregion
    }
#endif
}
