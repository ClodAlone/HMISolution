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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

#if  (SILVERLIGHT) || ( WINRT ) || (WP)
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser
{
  /// <summary>
  /// This class contains information about all known biff records.
  /// Used for registering the biff record type, creating new biff records
  /// and extracting them from a stream.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BiffRecordFactory
  {
    #region Class constants
    /// <summary>
    /// Default size for the internal dictionaries
    /// </summary>
    private const int DEF_RESERVE_SIZE = 200;
    #endregion

    #region Class members
    /// <summary>
    /// code-to-constructor pair
    /// </summary>
    private static Dictionary<int, BiffRecordRaw> m_dict = new Dictionary<int, BiffRecordRaw>( DEF_RESERVE_SIZE );
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initialize internal dictionary by Records
    /// </summary>
    static BiffRecordFactory()
    {
      FillFactory();
    }
    /// <summary>
    /// Fills factory without using reflection.
    /// </summary>
    private static void FillFactory()
    {
      BiffRecordRaw record;

      m_dict[ ( int )TBIFFRecord.RecalcId ] = new RecalcIdRecord();
      m_dict[ ( int )TBIFFRecord.UnkBegin ] = new UnknownBeginRecord();
      m_dict[ ( int )TBIFFRecord.Selection ] = new SelectionRecord();
      m_dict[ ( int )TBIFFRecord.PrintHeaders ] = new PrintHeadersRecord();
      m_dict[ ( int )TBIFFRecord.PivotName ] = new PivotNameRecord();
      m_dict[ ( int )TBIFFRecord.DCON ] = new DataConsolidationInfoRecord();
      m_dict[ ( int )TBIFFRecord.Label ] = new LabelRecord();
      m_dict[ ( int )TBIFFRecord.EOF ] = new EOFRecord();
      m_dict[ ( int )TBIFFRecord.DateWindow1904 ] = new DateWindow1904Record();
      m_dict[ ( int )TBIFFRecord.ChartSiIndex ] = new ChartSiIndexRecord();
      m_dict[ ( int )TBIFFRecord.ChartSeries ] = new ChartSeriesRecord();
      m_dict[ ( int )TBIFFRecord.ChartPie ] = new ChartPieRecord();
      m_dict[ ( int )TBIFFRecord.ChartFormatLink ] = new ChartFormatLinkRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxis ] = new ChartAxisRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxesUsed ] = new ChartAxesUsedRecord();
      m_dict[ ( int )TBIFFRecord.WriteAccess ] = new WriteAccessRecord();
      m_dict[ ( int )TBIFFRecord.Table ] = new TableRecord();
      m_dict[ ( int )TBIFFRecord.Protect ] = new ProtectRecord();
      m_dict[ ( int )TBIFFRecord.RowColumnFieldId ] = new RowColumnFiledIdRecord();
      m_dict[ ( int )TBIFFRecord.LineItemArray ] = new LineItemArrayRecord();
      m_dict[ ( int )TBIFFRecord.MulRK ] = new MulRKRecord();
      m_dict[ ( int )TBIFFRecord.MSODrawing ] = new MSODrawingRecord();

      record = new MarginRecord();
      record.SetRecordCode( ( int )TBIFFRecord.LeftMargin );
      m_dict[ ( int )TBIFFRecord.LeftMargin ] = record;

      record = new MarginRecord();
      record.SetRecordCode( ( int )TBIFFRecord.BottomMargin );
      m_dict[ ( int )TBIFFRecord.BottomMargin ] = record;

      record = new MarginRecord();
      record.SetRecordCode( ( int )TBIFFRecord.RightMargin );
      m_dict[ ( int )TBIFFRecord.RightMargin ] = record;

      record = new MarginRecord();
      record.SetRecordCode( ( int )TBIFFRecord.TopMargin );
      m_dict[ ( int )TBIFFRecord.TopMargin ] = record;

      m_dict[ ( int )TBIFFRecord.DVal ] = new DValRecord();
      m_dict[ ( int )TBIFFRecord.DefaultRowHeight ] = new DefaultRowHeightRecord();
      m_dict[ ( int )TBIFFRecord.CustomProperty ] = new CustomPropertyRecord();
      m_dict[ ( int )TBIFFRecord.ChartWrapper ] = new ChartWrapperRecord();
      m_dict[ ( int )TBIFFRecord.ChartSertocrt ] = new ChartSertocrtRecord();
      m_dict[ ( int )TBIFFRecord.ChartObjectLink ] = new ChartObjectLinkRecord();
      m_dict[ ( int )TBIFFRecord.ChartGelFrame ] = new ChartGelFrameRecord();
      m_dict[ ( int )TBIFFRecord.ChartChart ] = new ChartChartRecord();
      m_dict[ ( int )TBIFFRecord.Chart3DDataFormat ] = new Chart3DDataFormatRecord();
      m_dict[ ( int )TBIFFRecord.BookBool ] = new BookBoolRecord();
      m_dict[ ( int )TBIFFRecord.UnkMacrosDisable ] = new UnkMacrosDisable();
      m_dict[ ( int )TBIFFRecord.Formula ] = new FormulaRecord();
      m_dict[ ( int )TBIFFRecord.FilePass ] = new FilePassRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxisLineFormat ] = new ChartAxisLineFormatRecord();
      m_dict[ ( int )TBIFFRecord.BOF ] = new BOFRecord();
      m_dict[ ( int )TBIFFRecord.WSBool ] = new WSBoolRecord();
      m_dict[ ( int )TBIFFRecord.VerticalPageBreaks ] = new VerticalPageBreaksRecord();
      m_dict[ ( int )TBIFFRecord.SQLDataTypeId ] = new SQLDataTypeIdRecord();
      m_dict[ ( int )TBIFFRecord.PivotString ] = new PivotStringRecord();
      m_dict[ ( int )TBIFFRecord.Note ] = new NoteRecord();
      m_dict[ ( int )TBIFFRecord.CondFMT ] = new CondFMTRecord();
      m_dict[ ( int )TBIFFRecord.ChartUnits ] = new ChartUnitsRecord();
      m_dict[ ( int )TBIFFRecord.ChartSerParent ] = new ChartSerParentRecord();
      m_dict[ ( int )TBIFFRecord.ChartSerAuxTrend ] = new ChartSerAuxTrendRecord();
      m_dict[ ( int )TBIFFRecord.ChartRadar ] = new ChartRadarRecord();
      m_dict[ ( int )TBIFFRecord.ChartPos ] = new ChartPosRecord();
      m_dict[ ( int )TBIFFRecord.ChartDataLabels ] = new ChartDataLabelsRecord();
      m_dict[ ( int )TBIFFRecord.ChartAttachedLabel ] = new ChartAttachedLabelRecord();
      m_dict[ ( int )TBIFFRecord.ChartAttachedLabelLayout] = new ChartAttachedLabelLayoutRecord();
      m_dict[ ( int )TBIFFRecord.PlotAreaLayout] = new ChartPlotAreaLayoutRecord();
      m_dict[ ( int )TBIFFRecord.BoolErr ] = new BoolErrRecord();
      m_dict[ ( int )TBIFFRecord.SaveRecalc ] = new SaveRecalcRecord();
      m_dict[ ( int )TBIFFRecord.ExternalSourceInfo ] = new ExternalSourceInfoRecord();
      m_dict[ ( int )TBIFFRecord.Password ] = new PasswordRecord();
      m_dict[ ( int )TBIFFRecord.OBJ ] = new OBJRecord();
      m_dict[ ( int )TBIFFRecord.HLink ] = new HLinkRecord();
      m_dict[ ( int )TBIFFRecord.ChartBoppop ] = new ChartBoppopRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxisParent ] = new ChartAxisParentRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxisDisplayUnits ] = new ChartAxisDisplayUnitsRecord();
      m_dict[ ( int )TBIFFRecord.CF ] = new CFRecord();
      m_dict[ ( int )TBIFFRecord.CFEx] = new CFExRecord();
      m_dict[ ( int )TBIFFRecord.CF12] = new CF12Record();
      m_dict[ ( int )TBIFFRecord.CondFMT12] = new CondFmt12Record();
      m_dict[ ( int )TBIFFRecord.CalCount ] = new CalcCountRecord();
      m_dict[ ( int )TBIFFRecord.WindowOne ] = new WindowOneRecord();
      m_dict[ ( int )TBIFFRecord.Setup ] = new PrintSetupRecord();
      m_dict[ ( int )TBIFFRecord.Index ] = new IndexRecord();
      m_dict[ ( int )TBIFFRecord.MSODrawingGroup ] = new MSODrawingGroupRecord();
      m_dict[ ( int )TBIFFRecord.HeaderFooterImage ] = new HeaderFooterImageRecord();
      m_dict[ ( int )TBIFFRecord.Format ] = new FormatRecord();
      m_dict[ ( int )TBIFFRecord.ChartShtprops ] = new ChartShtpropsRecord();
      m_dict[ ( int )TBIFFRecord.ChartBoppCustom ] = new ChartBoppCustomRecord();
      m_dict[ ( int )TBIFFRecord.XCT ] = new XCTRecord();
      m_dict[ ( int )TBIFFRecord.UseSelFS ] = new UseSelFSRecord();
      m_dict[ ( int )TBIFFRecord.RString ] = new RStringRecord();
      m_dict[ ( int )TBIFFRecord.OleSize ] = new OleSizeRecord();
      m_dict[ ( int )TBIFFRecord.DCONBIN ] = new DConBinRecord();
      m_dict[ ( int )TBIFFRecord.Codepage ] = new CodepageRecord();
      m_dict[ ( int )TBIFFRecord.ChartDefaultText ] = new ChartDefaultTextRecord();
      m_dict[ ( int )TBIFFRecord.ChartDataFormat ] = new ChartDataFormatRecord();
      m_dict[ ( int )TBIFFRecord.ChartArea ] = new ChartAreaRecord();
      m_dict[ ( int )TBIFFRecord.WriteProtection ] = new WriteProtection();
      m_dict[ ( int )TBIFFRecord.ProtectionRev4 ] = new ProtectionRev4Record();
      m_dict[ ( int )TBIFFRecord.ViewExtendedInfo ] = new ViewExtendedInfoRecord();
      m_dict[ ( int )TBIFFRecord.PivotViewSource ] = new PivotViewSourceRecord();
      m_dict[ ( int )TBIFFRecord.PivotBoolean ] = new PivotBooleanRecord();
      m_dict[ ( int )TBIFFRecord.CacheDataEx ] = new CacheDataExRecord();
      m_dict[ ( int )TBIFFRecord.PasswordRev4 ] = new PasswordRev4Record();
      m_dict[ ( int )TBIFFRecord.MulBlank ] = new MulBlankRecord();
      m_dict[ ( int )TBIFFRecord.LabelSST ] = new LabelSSTRecord();
      m_dict[ ( int )TBIFFRecord.DSF ] = new DSFRecord();
      m_dict[ ( int )TBIFFRecord.DCONNAME ] = new DConNameRecord();
      m_dict[ ( int )TBIFFRecord.ChartText ] = new ChartTextRecord();
      m_dict[ ( int )TBIFFRecord.ChartPlotArea ] = new ChartPlotAreaRecord();
      m_dict[ ( int )TBIFFRecord.ChartLineFormat ] = new ChartLineFormatRecord();
      m_dict[ ( int )TBIFFRecord.ChartLegend ] = new ChartLegendRecord();
      m_dict[ ( int )TBIFFRecord.RefMode ] = new RefModeRecord();
      m_dict[ ( int )TBIFFRecord.Precision ] = new PrecisionRecord();
      m_dict[ ( int )TBIFFRecord.PivotViewItem ] = new PivotViewItemRecord();
      m_dict[ ( int )TBIFFRecord.PivotViewFields ] = new PivotViewFieldsRecord();
      m_dict[ ( int )TBIFFRecord.PivotNamePair ] = new PivotNamePairRecord();
      m_dict[ ( int )TBIFFRecord.ObjectProtect ] = new ObjectProtectRecord();
      m_dict[ ( int )TBIFFRecord.HorizontalPageBreaks ] = new HorizontalPageBreaksRecord();
      m_dict[ ( int )TBIFFRecord.HideObj ] = new HideObjRecord();
      m_dict[ ( int )TBIFFRecord.ExternSheet ] = new ExternSheetRecord();
      m_dict[ ( int )TBIFFRecord.ChartScatter ] = new ChartScatterRecord();
      m_dict[ ( int )TBIFFRecord.ChartPieFormat ] = new ChartPieFormatRecord();
      m_dict[ ( int )TBIFFRecord.ChartMarkerFormat ] = new ChartMarkerFormatRecord();
      m_dict[ ( int )TBIFFRecord.ChartBegDispUnit ] = new ChartBegDispUnitRecord();
      m_dict[ ( int )TBIFFRecord.ChartBar ] = new ChartBarRecord();
      m_dict[ ( int )TBIFFRecord.ChartAreaFormat ] = new ChartAreaFormatRecord();
      m_dict[ ( int )TBIFFRecord.ChartAI ] = new ChartAIRecord();
      m_dict[ ( int )TBIFFRecord.Backup ] = new BackupRecord();
      m_dict[ ( int )TBIFFRecord.WindowProtect ] = new WindowProtectRecord();
      m_dict[ ( int )TBIFFRecord.TextObject ] = new TextObjectRecord();
      m_dict[ ( int )TBIFFRecord.SupBook ] = new SupBookRecord();
      m_dict[ ( int )TBIFFRecord.RuleData ] = new RuleDataRecord();
      m_dict[ ( int )TBIFFRecord.PageItem ] = new PageItemRecord();
      m_dict[ ( int )TBIFFRecord.Name ] = new NameRecord();
      m_dict[ ( int )TBIFFRecord.HasBasic ] = new HasBasicRecord();
      m_dict[ ( int )TBIFFRecord.FileSharing ] = new FileSharingRecord();
      m_dict[ ( int )TBIFFRecord.DBCell ] = new DBCellRecord();
      m_dict[ ( int )TBIFFRecord.ColumnInfo ] = new ColumnInfoRecord();
      m_dict[(int)TBIFFRecord.DxGCol] = new DxGCol();
      m_dict[ ( int )TBIFFRecord.ChartLegendxn ] = new ChartLegendxnRecord();
      m_dict[ ( int )TBIFFRecord.ChartEndDispUnit ] = new ChartEndDispUnitRecord();
      m_dict[ ( int )TBIFFRecord.Bitmap ] = new BitmapRecord();
      m_dict[ ( int )TBIFFRecord.SharedFormula2 ] = new SharedFormulaRecord();
      m_dict[ ( int )TBIFFRecord.PivotIndexList ] = new PivotIndexListRecord();
      m_dict[ ( int )TBIFFRecord.DV ] = new DVRecord();
      m_dict[ ( int )TBIFFRecord.ChartIfmt ] = new ChartIfmtRecord();
      m_dict[ ( int )TBIFFRecord.ChartFbi ] = new ChartFbiRecord();
      m_dict[ ( int )TBIFFRecord.ChartDat ] = new ChartDatRecord();
      m_dict[ ( int )TBIFFRecord.Begin ] = new BeginRecord();
      m_dict[ ( int )TBIFFRecord.Unknown ] = new UnknownRecord();
      m_dict[ ( int )TBIFFRecord.SheetLayout ] = new SheetLayoutRecord();
      m_dict[ ( int )TBIFFRecord.Row ] = new RowRecord();
      m_dict[ ( int )TBIFFRecord.PivotError ] = new PivotErrorRecord();
      m_dict[ ( int )TBIFFRecord.DataItem ] = new DataItemRecord();
      m_dict[ ( int )TBIFFRecord.InterfaceEnd ] = new InterfaceEndRecord();
      m_dict[ ( int )TBIFFRecord.Guts ] = new GutsRecord();
      m_dict[ ( int )TBIFFRecord.Gridset ] = new GridsetRecord();
      m_dict[ ( int )TBIFFRecord.FilterMode ] = new FilterModeRecord();
      m_dict[ ( int )TBIFFRecord.Country ] = new CountryRecord();
      m_dict[ ( int )TBIFFRecord.ChartSeriesText ] = new ChartSeriesTextRecord();
      m_dict[ ( int )TBIFFRecord.BoundSheet ] = new BoundSheetRecord();
      m_dict[ ( int )TBIFFRecord.WindowTwo ] = new WindowTwoRecord();
      m_dict[ ( int )TBIFFRecord.TabId ] = new TabIdRecord();
      m_dict[ ( int )TBIFFRecord.PrinterSettings ] = new PrinterSettingsRecord();
      m_dict[ ( int )TBIFFRecord.PivotSourceInfo ] = new PivotSourceInfoRecord();
      m_dict[ ( int )TBIFFRecord.PivotDouble ] = new PivotDoubleRecord();
      m_dict[ ( int )TBIFFRecord.PivotDateTime ] = new PivotDateTimeRecord();
      m_dict[ ( int )TBIFFRecord.PageItemNameCount ] = new PageItemNameCountRecord();
      m_dict[ ( int )TBIFFRecord.MMS ] = new MMSRecord();

      record = new HeaderFooterRecord();
      record.SetRecordCode( ( int )TBIFFRecord.Header );
      m_dict[ ( int )TBIFFRecord.Header ] = record;

      record = new HeaderFooterRecord();
      record.SetRecordCode( ( int )TBIFFRecord.Footer );
      m_dict[ ( int )TBIFFRecord.Footer ] = record;

      m_dict[ ( int )TBIFFRecord.FnGroupCount ] = new FnGroupCountRecord();
      m_dict[ ( int )TBIFFRecord.ExtSST ] = new ExtSSTRecord();
      m_dict[ ( int )TBIFFRecord.ExtSSTInfoSub ] = new ExtSSTInfoSubRecord();
      m_dict[ ( int )TBIFFRecord.ExternCount ] = new ExternCountRecord();
      m_dict[ ( int )TBIFFRecord.ExtendedFormat ] = new ExtendedFormatRecord();
      m_dict[ ( int ) TBIFFRecord.ExtendedFormatCRC] = new ExtendedFormatCRC();
      m_dict[ ( int ) TBIFFRecord.ExtendedXFRecord] = new ExtendedXFRecord();
      m_dict[ ( int ) TBIFFRecord.ExtendedFormatCRC] = new ExtendedFormatCRC();
      m_dict[ ( int ) TBIFFRecord.ExtendedXFRecord] = new ExtendedXFRecord();
      m_dict[ ( int )TBIFFRecord.End ] = new EndRecord();
      m_dict[ ( int )TBIFFRecord.Delta ] = new DeltaRecord();
      m_dict[ ( int )TBIFFRecord.DefaultColWidth ] = new DefaultColWidthRecord();
      m_dict[ ( int )TBIFFRecord.DCON ] = new DCONRecord();
      m_dict[ ( int )TBIFFRecord.Continue ] = new ContinueRecord();
      m_dict[ ( int )TBIFFRecord.CodeName ] = new CodeNameRecord();
      m_dict[ ( int )TBIFFRecord.ChartSeriesList ] = new ChartSeriesListRecord();
      m_dict[ ( int )TBIFFRecord.ChartPlotGrowth ] = new ChartPlotGrowthRecord();
      m_dict[ ( int )TBIFFRecord.ChartLine ] = new ChartLineRecord();
      m_dict[ ( int )TBIFFRecord.Blank ] = new BlankRecord();
      m_dict[ ( int )TBIFFRecord.String ] = new StringRecord();
      m_dict[ ( int )TBIFFRecord.SST ] = new SSTRecord();

      record = new SheetCenterRecord();
      record.SetRecordCode( ( int )TBIFFRecord.HCenter );
      m_dict[ ( int )TBIFFRecord.HCenter ] = record;
      
      record = new SheetCenterRecord();
      record.SetRecordCode( ( int )TBIFFRecord.VCenter );
      m_dict[ ( int )TBIFFRecord.VCenter ] = record;

      m_dict[ ( int )TBIFFRecord.QuickTip ] = new QuickTipRecord();
      m_dict[ ( int )TBIFFRecord.PrintGridlines ] = new PrintGridlinesRecord();
      m_dict[ ( int )TBIFFRecord.StreamId ] = new StreamIdRecord();
      m_dict[ ( int )TBIFFRecord.PivotViewDefinition ] = new PivotViewDefinitionRecord();
      m_dict[ ( int )TBIFFRecord.PivotFormat ] = new PivotFormatRecord();
      m_dict[ ( int )TBIFFRecord.CRN ] = new CRNRecord();
      m_dict[ ( int )TBIFFRecord.PrintedChartSize ] = new PrintedChartSizeRecord();
      m_dict[ ( int )TBIFFRecord.ChartTick ] = new ChartTickRecord();
      m_dict[ ( int )TBIFFRecord.ChartRadarArea ] = new ChartRadarAreaRecord();
      m_dict[ ( int )TBIFFRecord.ChartFrame ] = new ChartFrameRecord();
      m_dict[ ( int )TBIFFRecord.ChartFontx ] = new ChartFontxRecord();
      m_dict[ ( int )TBIFFRecord.ChartChartFormat ] = new ChartChartFormatRecord();
      m_dict[ ( int )TBIFFRecord.UnkMarker ] = new UnknownMarkerRecord();
      m_dict[ ( int )TBIFFRecord.Template ] = new TemplateRecord();
      m_dict[ ( int )TBIFFRecord.Style ] = new StyleRecord();
      m_dict[ ( int )TBIFFRecord.RK ] = new RKRecord();
      m_dict[ ( int )TBIFFRecord.RefreshAll ] = new RefreshAllRecord();
      m_dict[ ( int )TBIFFRecord.RangeProtection ] = new RangeProtectionRecord();
      m_dict[ ( int )TBIFFRecord.SelectionInfo ] = new SelectionInfoRecord();
      m_dict[ ( int )TBIFFRecord.RuleFilter ] = new RuleFilterRecord();
      m_dict[ ( int )TBIFFRecord.PivotEmpty ] = new PivotEmptyRecord();
      m_dict[ ( int )TBIFFRecord.PageItemIndexes ] = new PageItemIndexesRecord();
      m_dict[ ( int )TBIFFRecord.Number ] = new NumberRecord();
      m_dict[ ( int )TBIFFRecord.LabelRanges ] = new LabelRangesRecord();
      m_dict[ ( int )TBIFFRecord.InterfaceHdr ] = new InterfaceHdrRecord();
      m_dict[ ( int )TBIFFRecord.DCONRef ] = new DConRefRecord();
      m_dict[ ( int )TBIFFRecord.ChartValueRange ] = new ChartValueRangeRecord();
      m_dict[ ( int )TBIFFRecord.ChartSerFmt ] = new ChartSerFmtRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxisOffset ] = new ChartAxisOffsetRecord();
      m_dict[ ( int )TBIFFRecord.Array ] = new ArrayRecord();
      m_dict[ ( int )TBIFFRecord.WindowZoom ] = new WindowZoomRecord();
      m_dict[ ( int )TBIFFRecord.ScenProtect ] = new ScenProtectRecord();
      m_dict[ ( int )TBIFFRecord.PivotViewFieldsEx ] = new PivotViewFieldsExRecord();
      m_dict[ ( int )TBIFFRecord.PivotField ] = new PivotFieldRecord();
      m_dict[ ( int )TBIFFRecord.ParsedExpression ] = new ParsedExpressionRecord();
      m_dict[ ( int )TBIFFRecord.Pane ] = new PaneRecord();
      m_dict[ ( int )TBIFFRecord.Palette ] = new PaletteRecord();
      m_dict[ ( int )TBIFFRecord.Font ] = new FontRecord();
      m_dict[ ( int )TBIFFRecord.Dimensions ] = new DimensionsRecord();
      m_dict[ ( int )TBIFFRecord.ChartSurface ] = new ChartSurfaceRecord();
      m_dict[ ( int )TBIFFRecord.ChartSerAuxErrBar ] = new ChartSerAuxErrBarRecord();
      m_dict[ ( int )TBIFFRecord.ChartDropBar ] = new ChartDropBarRecord();
      m_dict[ ( int )TBIFFRecord.ChartChartLine ] = new ChartChartLineRecord();
      m_dict[ ( int )TBIFFRecord.CalcMode ] = new CalcModeRecord();
      m_dict[ ( int )TBIFFRecord.AutoFilter ] = new AutoFilterRecord();
      m_dict[ ( int )TBIFFRecord.Sort ] = new SortRecord();
      m_dict[ ( int )TBIFFRecord.SheetProtection ] = new SheetProtectionRecord();
      m_dict[ ( int )TBIFFRecord.PivotFormula ] = new PivotFormulaRecord();
      m_dict[ ( int )TBIFFRecord.CacheData ] = new CacheDataRecord();
      m_dict[ ( int )TBIFFRecord.MergeCells ] = new MergeCellsRecord();
      m_dict[ ( int )TBIFFRecord.Iteration ] = new IterationRecord();
      m_dict[ ( int )TBIFFRecord.ExternName ] = new ExternNameRecord();
      m_dict[ ( int )TBIFFRecord.ChartSbaseref ] = new ChartSbaserefRecord();
      m_dict[ ( int )TBIFFRecord.ChartPicf ] = new ChartPicfRecord();
      m_dict[ ( int )TBIFFRecord.ChartCatserRange ] = new ChartCatserRangeRecord();
      m_dict[ ( int )TBIFFRecord.ChartAxcext ] = new ChartAxcextRecord();
      m_dict[ ( int )TBIFFRecord.ChartAlruns ] = new ChartAlrunsRecord();
      m_dict[ ( int )TBIFFRecord.Chart3D ] = new Chart3DRecord();
      m_dict[ ( int )TBIFFRecord.AutoFilterInfo ] = new AutoFilterInfoRecord();
      m_dict[ ( int )TBIFFRecord.ImageData ] = new ImageDataRecord();
      m_dict[ ( int )TBIFFRecord.ChartMlFrt ] = new UnknownRecord();
      m_dict[(int)TBIFFRecord.Compatibility] = new CompatibilityRecord();
      m_dict[(int)TBIFFRecord.DConn] = new UnknownRecord();
      m_dict[(int)TBIFFRecord.HeaderFooter] = new HeaderAndFooterRecord();

      record = new PageLayoutView();
      record.SetRecordCode((int)TBIFFRecord.PageLayoutView);
      m_dict[(int)TBIFFRecord.PageLayoutView] = record;
     
    }
//    /// <summary>
//    /// Fills factory using reflection.
//    /// </summary>
//    private static void FillFactoryReflection()
//    {
//#if DEBUG
//      DateTime now = DateTime.Now;
//#endif

//      //Assembly asm = Assembly.GetExecutingAssembly();
//      Type[] types = ApplicationImpl.AssemblyTypes;//asm.GetTypes();

//      for( int i = 0, len = types.Length; i < len; i++ )
//      {
//        Type type = types[ i ];
//        object[] attribs = type.GetCustomAttributes( typeof( BiffAttribute ), false );
//        int iAttributeCount = ( attribs != null ) ? attribs.Length : 0;

//        if( iAttributeCount > 0 )
//        {
//          Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, type.FullName,
//            "Records Found" );

//          object o = Activator.CreateInstance( type );

//          // This if was add to optimize performance, since most of records have only one BiffAttribute.
//          if( iAttributeCount == 1 )
//          {
//            BiffAttribute attrib = ( BiffAttribute )attribs[ 0 ];
//            int iCode = ( int )attrib.Code;
//            m_dict[ iCode ] = ( BiffRecordRaw )o;
//            //Console.WriteLine( "m_dict[ ( int )TBIFFRecord.{0} ] = new {1}();", attrib.Code, type.Name );
//          }
//          else
//          {

//            BiffRecordRaw record = ( BiffRecordRaw )o;

//            for( int j = 0; j < iAttributeCount; j++ )
//            {
//              BiffAttribute attrib = ( BiffAttribute )attribs[ j ];
//              int iCode = ( int )attrib.Code;
//              record.SetRecordCode( iCode );
//              m_dict[ iCode ] = record;// as BiffRecordRaw;
//              record = ( BiffRecordRaw )record.Clone();
//              //Console.WriteLine( "record = new {0}();", type.Name );
//              //Console.WriteLine( "record.SetRecordCode( {0} );", iCode );
//              //Console.WriteLine( "m_dict[ ( int )TBIFFRecord.{0} ] = record;", attrib.Code );
//            }
//          }
//        }
//      }

//#if DEBUG
//      TimeSpan diff = DateTime.Now.Subtract( now );
//      Debug.WriteLine( diff, "Class Extract Performance" );
//#endif
//    }
    #endregion

    #region Extracting Record class according to it code
    /// <summary>
    /// Create empty record by specified type.
    /// </summary>
    /// <param name="type">Type of the record that should be created.</param>
    /// <returns>Created record if succeeded, null otherwise.</returns>
    public static BiffRecordRaw GetRecord( TBIFFRecord type )
    {
      int iCode = ( int )type;
      return GetRecord( iCode );
    }
    /// <summary>
    /// Create empty record by specified type.
    /// </summary>
    /// <param name="type">Type of the record that should be created.</param>
    /// <returns>Created record if succeeded, null otherwise.</returns>
    public static BiffRecordRaw GetRecord( int type )
    {
      object value = ( m_dict.ContainsKey( type ) )
        ? m_dict[ type ]
        : null;

      ICloneable toClone = null;

      if( value != null )
      {
        toClone = value as ICloneable;
      }
      else if( m_dict.ContainsKey( ( int )TBIFFRecord.Unknown ) )
      {
        UnknownRecord record = ( UnknownRecord )m_dict[ ( int )TBIFFRecord.Unknown ];
        record.RecordCode = type;
        toClone = record;
      }

      if( toClone != null )
      {
        return toClone.Clone() as BiffRecordRaw;
      }

      return null;
    }
    /// <summary>
    /// Extracts unknown record from the stream
    /// </summary>
    /// <param name="stream">Stream that contains needed record</param>
    /// <returns>Extracted unknown record</returns>
    public static BiffRecordRaw GetUntypedRecord( Stream stream )
    {
      int size;
      UnknownRecord raw = new UnknownRecord( stream, out size );

      return raw;
    }
    /// <summary>
    /// Extracts unknown record from the stream
    /// </summary>
    /// <param name="reader">Reader that contains record</param>
    /// <returns>Extracted unknown record</returns>
    public static BiffRecordRaw GetUntypedRecord( BinaryReader reader )
    {
      int size;
      UnknownRecord raw = new UnknownRecord( reader, out size );

      return raw;
    }
    /// <summary>
    /// Extracts specified record from the reader
    /// </summary>
    /// <param name="type">Type of the record to be extracted</param>
    /// <param name="reader">Reader that contains record</param>
    /// <param name="provider">Object that provider access to the record data.</param>
    /// <param name="decryptor">
    /// Decryptor used to parse encrypted records.
    /// This argument can be null when no decryption is required.
    /// </param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Extracted record</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When specified reader is null
    /// </exception>
    public static BiffRecordRaw GetRecord( int type, BinaryReader reader,
      DataProvider provider, IDecryptor decryptor, byte[] arrBuffer )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      BiffRecordRaw retValue = GetRecord( type );

      // if record exists then infill it
      if( retValue != null )
      {
        retValue.FillRecord( reader, provider, decryptor, arrBuffer );
      }

      return retValue;
    }
    /// <summary>
    /// Extracts specified record from the reader.
    /// </summary>
    /// <param name="type">Type of the record to be extracted.</param>
    /// <param name="reader">Reader that contains record.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Extracted record.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When specified reader is null.
    /// </exception>
    public static BiffRecordRaw GetRecord( TBIFFRecord type, BinaryReader reader,
      byte[] arrBuffer )
    {
      return GetRecord( ( int )type, reader, arrBuffer );
    }
    /// <summary>
    /// Extracts specified record from the reader.
    /// </summary>
    /// <param name="type">Type of the record to be extracted.</param>
    /// <param name="reader">Reader that contains record.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Extracted record.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When specified reader is null.
    /// </exception>
    public static BiffRecordRaw GetRecord( int type, BinaryReader reader,
      byte[] arrBuffer )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      BiffRecordRaw retValue = GetRecord( type );

      // if record exists then infill it
      if( retValue != null )
      {
        retValue.FillRecord( reader, null, null, arrBuffer );
      }

      return retValue;
    }
    /// <summary>
    /// Extracts record from the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader that contains record to extract.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Extracted record.</returns>
    public static BiffRecordRaw GetRecord( BinaryReader reader, DataProvider provider,
      byte[] arrBuffer )
    {
      return GetRecord( ExtractRecordType( reader ), reader, provider,
        null, arrBuffer );
    }
    /// <summary>
    /// Extracts record from the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader that contains record to extract.</param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
    /// <returns>Extracted record.</returns>
    public static BiffRecordRaw GetRecord( BinaryReader reader, DataProvider provider,
      IDecryptor decryptor, byte[] arrBuffer )
    {
      return GetRecord( ExtractRecordType( reader ), reader, provider,
        decryptor, arrBuffer );
    }
    /// <summary>
    /// Extracts record from array of bytes.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's start.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Extracted record.</returns>
    public static BiffRecordRaw GetRecord( DataProvider provider, int iOffset,
      ExcelVersion version )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      int recordType = provider.ReadInt16( iOffset );
      iOffset += 2;

      BiffRecordRaw result = GetRecord( recordType );

      int iLength = provider.ReadInt16( iOffset );
      result.Length = iLength;
      iOffset += 2;

      result.ParseStructure( provider, iOffset, iLength, version );

      return result;
    }

    /// <summary>
    /// Extracts from the BinaryReader type of the next record.
    /// </summary>
    /// <param name="reader">BinaryReader that contains record to extract.</param>
    /// <returns>Type of the next record in the BinaryReader.</returns>
    /// <exception cref="System.ArgumentNullException">When reader is null.</exception>
    /// <exception cref="System.ApplicationException">
    /// When code of the extracted record is zero.
    /// </exception>
    public static int   ExtractRecordType( BinaryReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      Stream stream = reader.BaseStream;
      long lPos = stream.Position;
      int iCode = reader.ReadInt16();
      stream.Position = lPos;

      if( iCode == 0 )
        throw new ApplicationException( "Cannot find record identifier in stream!" );

      return iCode;
    }

    /// <summary>
    /// Extract from stream Next Record type.
    /// </summary>
    /// <param name="stream">Stream that contains record to extract.</param>
    /// <returns>Extracted record.</returns>
    public static int   ExtractRecordType( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( stream.CanSeek == false || stream.CanRead == false )
        throw new ApplicationException( "Stream must permit seeking and reading operations" );

      long lPos = stream.Position;
      int retValue = ( stream.ReadByte() & 0xff ) + ( ( stream.ReadByte() & 0xff ) << 8 );

      if( retValue == 0 )
        throw new ApplicationException( "Cannot find record identifier in stream!" );

      if( !m_dict.ContainsKey( retValue ) )
      {
        retValue = ( int )TBIFFRecord.Unknown;
      }

      return retValue;
    }
    #endregion

    #region Enhance methods
    ///// <summary>
    ///// Registers biff record type in the inner collections.
    ///// </summary>
    ///// <param name="type">Type of the record to register.</param>
    ///// <exception cref="System.ArgumentNullException">If type is null.</exception>
    ///// <exception cref="System.ArgumentException">
    ///// When specified by type class is not derived from BiffRecordRaw class
    ///// or if type does not contain BiffAttribute.
    ///// </exception>
    //private static void InnerRegisterRecord( Type type )
    //{
    //  if( type == null )
    //    throw new ArgumentNullException( "type" );

    //  if( type.IsSubclassOf( typeof( BiffRecordRaw ) ) == false )
    //    throw new ArgumentException( "class must be derived from BiffRecordRaw class", "type" );

    //  object[] arrAttribs = type.GetCustomAttributes( typeof( BiffAttribute ), true );

    //  if( arrAttribs.Length > 0 )
    //  {
    //    Debug.WriteLine( type.FullName, "Records Found" );

    //    BiffAttribute attrib = ( BiffAttribute )arrAttribs[ 0 ];

    //    // get default and with space reserve constructors
    //    int iCode = ( int )attrib.Code;
    //    m_dict[ iCode ] = Activator.CreateInstance( type ) as BiffRecordRaw;
    //  }
    //  else
    //  {
    //    throw new ArgumentException( "Type does not contain any BiffAttribute " +
    //      "specification. Type: " + type.FullName, "type" );
    //  }
    //}
    ///// <summary>
    ///// Used for adding additional BiffRecords in runtime
    ///// </summary>
    ///// <param name="type">BiffRecord typeof information</param>
    //public static void AddCustomRecord( Type type )
    //{
    //  InnerRegisterRecord( type );
    //}
    ///// <summary>
    ///// Register multiple classes on one call
    ///// </summary>
    ///// <param name="types">array of Biff Records type information</param>
    //public static void AddCustomRecord( Type[] types )
    //{
    //  for( int i = 0, len = types.Length; i < len; i++ )
    //  {
    //    InnerRegisterRecord( types[ i ] );
    //  }
    //}
    ///// <summary>
    ///// Remove from Biff Factory registration of specified type.
    ///// </summary>
    ///// <param name="type">
    ///// Type of class which can be produced by BiffFactory on demand
    ///// </param>
    //public static void RemoveCustomRecord( Type type )
    //{
    //  if( type == null )
    //    throw new ArgumentNullException( "type" );

    //  object[] attribs = type.GetCustomAttributes( typeof( BiffAttribute ), true );

    //  if( attribs.Length > 0 )
    //  {
    //    BiffAttribute attrib = ( BiffAttribute )attribs[ 0 ];

    //    int iCode = ( int )attrib.Code;
    //    m_dict.Remove( iCode );
    //  }
    //}
    ///// <summary>
    ///// Remove registration of BiffRecords from Biff Factory
    ///// </summary>
    ///// <param name="types">Array of BiffRecords types</param>
    //public static void RemoveCustomRecord( Type[] types )
    //{
    //  if( types == null )
    //    throw new ArgumentNullException( "types" );

    //  for( int i = 0, len = types.Length; i < len; i++ )
    //  {
    //    RemoveCustomRecord( types[ i ] );
    //  }
    //}
    #endregion
  }
}
