#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace Syncfusion.Windows.Controls.Spreadsheet.Resources
{
    public class ResourceWrapper
    {
        ///BackStage
        const string _New = "New";
        const string _Open = "Open";
        const string _Print = "Print";
        const string _Save = "Save";
        const string _SaveAs = "SaveAs";
        const string _Info = "Info";
        const string _Exit = "Exit";

        ///Clipboard
        const string _Clipboard = "Clipboard";
        const string _Close = "Close";
        const string _Copy = "Copy";
        const string _Cut = "Cut";
        const string _Paste = "Paste";
        
        ///Font
        const string _Font = "Font";

        //Formatting
        const string _Alignment = "Alignment";
       
        const string _MergeCenter = "MergeCenter";
        const string _Styles = "Styles";
        const string _Cells = "Cells";

        //Tabs
        const string _File = "File";
        const string _Home = "Home";
        const string _Others = "Others";

        //Other - Tab
        const string _Picture = "Picture";
        const string _Illustrations = "Illustrations";
        const string _Hyperlink = "Hyperlink";
        const string _Links = "Links";
        const string _GridLines = "GridLines";
        const string _Headings = "Headings";
        const string _FormulaBar = "FormulaBar";
        const string _Show = "Show";
        const string _DataValidation = "DataValidation";
        const string _Data = "Data";
        const string _Freeze = "Freeze";
        const string _View = "View";
        const string _Themes = "Themes";
        const string _Comments = "Comments";
        const string _DeleteComments = "DeleteComments";
        const string _EditComments = "EditComments";

        //Home Tab - MergeCenter ComboBox
        const string _MergeAndCenter = "MergeAndCenter";
        const string _UnmergeCells = "UnmergeCells";

        //Home Tab - Styles
        const string _ConditionalFormatting = "ConditionalFormatting";
        const string _FormatAsTable = "FormatAsTable";
        const string _Normal = "Normal";
        const string _Bad = "Bad";

        //Home Tab - Cells
        const string _Insert = "Insert";
        const string _Delete = "Delete";
        const string _Format = "Format";

        //Home Tab - Styles 
        const string _HighlightCellsRules = "HighlightCellsRules";

        //Home Tab - Styles - HighlightCellsRules
        const string _GreaterThan = "GreaterThan";
        const string _LessThan = "LessThan";
        const string _Between = "Between";
        const string _EqualTo = "EqualTo";

        //Home Tab - Cells - Insert
        const string _InsertRow = "InsertRow";
        const string _InsertColumn = "InsertColumn";
        const string _InsertSheet = "InsertSheet";

        //Home Tab - Cells - Delete
        const string _DeleteRow = "DeleteRow";
        const string _DeleteColumn = "DeleteColumn";
        const string _DeleteSheet = "DeleteSheet";

        //Home Tab - Cells - Format
        const string _RowHeight = "RowHeight";
        const string _ColumnWidth = "ColumnWidth";
        const string _HideAndUnhide = "HideAndUnhide";
        const string _RenameSheet = "RenameSheet";

        //Home Tab - Cells - Format - HideAndUnhide
        const string _HideRows = "HideRows";
        const string _HideColumns = "HideColumns";
        const string _HideSheet = "HideSheet";
        const string _UnhideRows = "UnhideRows";
        const string _UnhideColumns = "UnhideColumns";
        const string _UnhideSheet = "UnhideSheet";

        //Others Tab - Freeze
        const string _FreezePanes = "FreezePanes";
        const string _FreezeTopRow = "FreezeTopRow";
        const string _FreezeTopColumn = "FreezeTopColumn";

        //Others Tab - Themes
        const string _Office = "Office";
        const string _Adjacency = "Adjacency";
        const string _Apex = "Apex";
        const string _Angels = "Angels";
        const string _Apothecary = "Apothecary";
        const string _Aspect = "Aspect";
        const string _Austin = "Austin";
        const string _Blacktie = "Blacktie";
        const string _Civic = "Civic";
        const string _Clarity = "Clarity";
        const string _Composite = "Composite";
        const string _Concourse = "Concourse";
        const string _Couture = "Couture";
        const string _Elemental = "Elemental";
        const string _Equity = "Equity";
        const string _Essential = "Essential";

        //Others Tab - Outline
        const string _Outline = "Outline";
        const string _Group = "Group";
        const string _Ungroup = "Ungroup";
        const string _IsSummaryRowBelow = "IsSummaryRowBelow";
        const string _IsSummaryColumnAtRight = "IsSummaryColumnAtRight";
        const string _Rows = "Rows";
        const string _Columns = "Columns";
        const string _Direction = "Direction";


        //OtersTab - Document settings

        const string _DocumentSettins = "DocumentSettings";

        //File Tab - Info
        const string _Permissions = "Permissions";

        //DataValidationWindow
        const string _ValidationCriteria = "ValidationCriteria";
        const string _Allow = "Allow";
        const string _Minimum = "Minimum";
        const string _Maximum = "Maximum";
        const string _NotBetween = "NotBetween";
        const string _NotEqualTo = "NotEqualTo";
        const string _GreaterThanOrEqualTo = "GreaterThanOrEqualTo";
        const string _LessThanOrEqualTo = "LessThanOrEqualTo";


        

        const string _AnyValue = "AnyValue";
        const string _WholeNumber = "WholeNumber";
        const string _Decimal = "Decimal";
        const string _List = "List";
        const string _DateAndTime = "DateAndTime";
        const string _TextLength = "TextLength";
        const string _Formula = "Formula";

        const string _Stop = "Stop";
        const string _Warning = "Warning";
        const string _Information = "Information";

        const string _Ok = "Ok";
        const string _Cancel = "Cancel";

        //Conditional Format Window items
        const string _LightRedFillwithDarkRedText = "LightRedFillwithDarkRedText";
        const string _YellowFillwithDarkYellowText = "YellowFillwithDarkYellowText";
        const string _GreenFillwithDarkGreenText = "GreenFillwithDarkGreenText";
        const string _LightRedFill = "LightRedFill";
        const string _RedText = "RedText";
        const string _RedBorder = "RedBorder";
        const string _GreaterThanWindowTitle = "GreaterThanWindowTitle";
        const string _LessWindowTitle = "LessWindowTitle";
        const string _BetweenWindowTitle = "BetweenWindowTitle";
        const string _NotBetweenWindowTitle = "NotBetweenWindowTitle";
        const string _EqualToWindowTitle = "EqualToWindowTitle";
        const string _GreaterThanWindowDescription = "GreaterThanWindowDescription";
        const string _LessThanWindowDescription = "LessThanWindowDescription";
        const string _BetweenWindowDescription = "BetweenWindowDescription";
        const string _NotBetweenWindowDescription = "NotBetweenWindowDescription";
        const string _EqualToWindowDescription = "EqualToWindowDescription";

        const string _And = "And";
        const string _With = "With";


        //DataValidation
        const string _Settings = "Settings";
        const string _Data_small = "Data_small";
        const string _InputMessage = "InputMessage";
        const string _Inputmessage_small = "Inputmessage_small";
        const string _Title = "Title";
        const string _ErrorAlert = "ErrorAlert";
        const string _ErrorMessage_small = "ErrorMessage_small";
        const string _ClearAll = "ClearAll";
        const string _InputmessageDescription = "InputmessageDescription";
        const string _ErrorAlertDescription = "ErrorAlertDescription";
        const string _ErrorMessage = "ErrorMessage";
        const string _DataValidationDescription = "DataValidationDescription";
        const string _DataValidationTitle = "DataValidationTitle";
        const string _Value = "Value";
        const string _EndDate = "EndDate";
        const string _StartDate = "StartDate";
        const string _Date = "Date";
        const string _Between_Lower = "Between_Lower";
        const string _Notbetween_Lower = "Notbetween_Lower";
        const string _GreaterThan_Lower = "GreaterThan_Lower";
        const string _GreaterThanOrEqualTo_Lower = "GreaterThanOrEqualTo_Lower";
        const string _Lessthan_Lower = "Lessthan_Lower";
        const string _LessThanOrEqualTo_Lower = "LessThanOrEqualTo_Lower";
        const string _Equalto_Lower = "Equalto_Lower";
        const string _NotEqualTo_Lower = "NotEqualTo_Lower";
        const string _Source = "Source";

        const string _ProtectSheet = "ProtectSheet";
        const string _ProtectWorkbook = "ProtectWorkbook";

        const string _SaveAsDifferentTypes = "SaveAsDifferentTypes";
        const string _SaveACopyOfTheItem = "SaveACopyOfTheItem";

       


        //...Text
        const string _LessThanOption = "LessThanOption";
        const string _GreaterThanOption = "GreaterThanOption";
        const string _UnHideSheetOption = "UnHideSheetOption";
        const string _RowHeightOption = "RowHeightOption";
        const string _ColumnWidthOption = "ColumnWidthOption";
        const string _BetweenOption = "BetweenOption";
        const string _EqualToOption = "EqualToOption";


        const string _AutoFitRowHeight = "AutoFitRowHeight";
        const string _AutoFitColumnWidth = "AutoFitColumnWidth";

        const string _Border = "Border";

        const string _TopAlign = "TopAlign";
        const string _MiddleAlign = "MiddleAlign";
        const string _BottomAlign = "BottomAlign";

        const string _LeftAlign = "LeftAlign";
        const string _CenterAlign = "CenterAlign";
        const string _RightAlign = "RightAlign";

        const string _IncreaseIndent = "IncreaseIndent";
        const string _DecreaseIndent = "DecreaseIndent";

        const string _AccountingNumberFormat = "AccountingNumberFormat";
        const string _PercentStyle = "PercentStyle";
        const string _CommaStyle = "CommaStyle";
        const string _IncreaseDecmial = "IncreaseDecmial";
        const string _DecreaseDecmial = "DecreaseDecmial";

        const string _TopBorder = "TopBorder";
        const string _BottomBorder = "BottomBorder";
        const string _LeftBorder = "LeftBorder";
        const string _RightBorder = "RightBorder";
        const string _AllBorder = "AllBorder";
        const string _NoBorder = "NoBorder";
        const string _TopandBottomBorder = "TopandBottomBorder";
        const string _TopandThickBottomBorder = "TopandThickBottomBorder";
        const string _OutSideBorder = "OutSideBorder";
        const string _ThickBoxBorder = "ThickBoxBorder";
        const string _ThickBottomBorder = "ThickBottomBorder";
        const string _UnProtectSheet = "UnProtectSheet";
        const string _Hide = "Hide";
        const string _Unhide = "Unhide";
        const string _UnFreezePanes = "UnFreezePanes";
        

       
        //Command Windows
        const string _FormatAsTableDescription = "FormatAsTableDescription";
        const string _ColumnWidthWindow = "ColumnWidthWindow";
        const string _RowHeightWindow = "RowHeightWindow";

        const string _ColumnWidthDescription = "ColumnWidthDescription";
        const string _RowHeightDescription = "RowHeightDescription";

        //Number Format Combo
        const string _Number = "Number";
        const string _General = "General";
        const string _Currency = "Currency";
        const string _Accounting = "Accounting";
        const string _ShortDate = "ShortDate";
        const string _LongDate = "LongDate";
        const string _Time = "Time";
        const string _Percentage = "Percentage";

        //document properties
        const string _WrapText = "WrapText";
        const string _DocumentProperties = "DocumentProperties";
        const string _DocumentPropertiesCompany = "DocumentProperty_Company";
        const string _DocumentMaskCompany = "DocumentMask_Company";
        const string _DocumentPropertiesTitle = "DocumentProperty_Title";
        const string _DocumentMaskTitle = "DocumentMask_Title";
        const string _DocumentPropertiesTag = "DocumentProperty_Tag";
        const string _DocumentMaskTag = "DocumentMask_Tag";
        const string _DocumentPropertiesComments = "DocumentProperty_Comments";
        const string _DocumentMaskComments = "DocumentMask_Comments";
        const string _DocumentPropertiesCataegories = "DocumentProperty_Cataegory";
        const string _DocumentMaskCategory = "DocumentMask_Category";
        const string _DocumentPropertiesSubject = "DocumentProperty_Subject";
        const string _DocumentMaskSubject = "DocumentMask_Subject";
        const string _DocumentPropertiesAppName = "DocumentProperty_AppName";
        const string _DocumentMaskAppName = "DocumentMask_AppName";
        const string _DocumentPropertiesRelatedPeople = "DocumentProperty_RelatedPeople";
        const string _DocumentPropertiesManager = "DocumentProperty_Manager";
        const string _DocumentMaskManager = "DocumentMask_Manager";
        const string _DocumentPropertiesAuthor = "DocumentProperty_Author";
        const string _DocumentMaskAuthor = "DocumentMask_Author";
        const string _ShowAllProperies = "ShowAllProperies";
        const string _ShowFewerProperties = "ShowFewerProperties";


      
       

      
        

       



        public ResourceWrapper()
        {
#if SILVERLIGHT
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
#endif
                CultureInfo ci = CultureInfo.CurrentUICulture;

                //BackStage
                newopt = SR.GetString(ci, _New);
                open = SR.GetString(ci, _Open);
                print = SR.GetString(ci, _Print);
                save = SR.GetString(ci, _Save);
                saveAs = SR.GetString(ci, _SaveAs);
                info = SR.GetString(ci, _Info);
                exit = SR.GetString(ci, _Exit);
                documentProperty_Title = SR.GetString(ci, _DocumentPropertiesTitle);
                documentMask_Title = SR.GetString(ci, _DocumentMaskTitle);
                documentProperty_Tag = SR.GetString(ci, _DocumentPropertiesTag);
                documentMask_Tag = SR.GetString(ci, _DocumentMaskTag);
                documentProperty_Comments = SR.GetString(ci, _DocumentPropertiesComments);
                documentMask_Comments = SR.GetString(ci, _DocumentMaskComments);
                documentProperty_Cataegory = SR.GetString(ci, _DocumentPropertiesCataegories);
                documentMask_Category = SR.GetString(ci, _DocumentMaskCategory);
                documentProperty_Subject = SR.GetString(ci, _DocumentPropertiesSubject);
                documentMask_Subject = SR.GetString(ci, _DocumentMaskSubject);
                documentProperty_AppName = SR.GetString(ci, _DocumentPropertiesAppName);
                documentMask_AppName = SR.GetString(ci, _DocumentMaskAppName);
                documentProperty_RelatedPeople = SR.GetString(ci, _DocumentPropertiesRelatedPeople);
                documentProperty_Manager = SR.GetString(ci, _DocumentPropertiesManager);
                documentMask_Manager = SR.GetString(ci, _DocumentMaskManager);
                documentPropertiesAuthor = SR.GetString(ci, _DocumentPropertiesAuthor);
                documentMask_Author = SR.GetString(ci, _DocumentMaskAuthor);
                showAllProperies = SR.GetString(ci, _ShowAllProperies);
                ShowFewerProperties = SR.GetString(ci, _ShowFewerProperties);
                documentProperties = SR.GetString(ci, _DocumentProperties);
                documentProperty_Company = SR.GetString(ci, _DocumentPropertiesCompany);
                documentMask_Company = SR.GetString(ci, _DocumentMaskCompany);

                //Clipboard
                clipboard = SR.GetString(ci, _Clipboard);
                close = SR.GetString(ci, _Close);
                copy = SR.GetString(ci, _Copy);
                cut = SR.GetString(ci, _Cut);
                paste = SR.GetString(ci, _Paste);

                //Font
                font = SR.GetString(ci, _Font);

                //Formatting
                alignment = SR.GetString(ci, _Alignment);
                mergeCenter = SR.GetString(ci, _MergeCenter);
                styles = SR.GetString(ci, _Styles);
                cells = SR.GetString(ci, _Cells);

                //Tabs
                file = SR.GetString(ci, _File);
                home = SR.GetString(ci, _Home);
                others = SR.GetString(ci, _Others);

                //Others - Tab
                picture = SR.GetString(ci, _Picture);
                illustrations = SR.GetString(ci, _Illustrations);
                hyperlink = SR.GetString(ci, _Hyperlink);
                links = SR.GetString(ci, _Links);
                gridLines = SR.GetString(ci, _GridLines);
                headings = SR.GetString(ci, _Headings);
                formulaBar = SR.GetString(ci, _FormulaBar);
                show = SR.GetString(ci, _Show);
                dataValidation = SR.GetString(ci, _DataValidation);
                data = SR.GetString(ci, _Data);
                freeze = SR.GetString(ci, _Freeze);
                view = SR.GetString(ci, _View);
                themes = SR.GetString(ci, _Themes);
                comments = SR.GetString(ci, _Comments);

                deleteComments = SR.GetString(ci, _DeleteComments);
                editComments = SR.GetString(ci, _EditComments);

                newComment = SR.GetString(ci, _NewComment);
                editComment = SR.GetString(ci, _EditComment);
                deleteComment = SR.GetString(ci, _DeleteComment);
                protectsheet = SR.GetString(ci, _ProtectSheet);
                protectWorkbook = SR.GetString(ci, _ProtectWorkbook);
                changes = SR.GetString(ci, _Changes);


                //Home Tab - MergeCenter ComboBox
                mergeAndCenter = SR.GetString(ci, _MergeAndCenter);
                unmergeCells = SR.GetString(ci, _UnmergeCells);
               
                //Home Tab_WrapText
                wrapText = SR.GetString(ci, _WrapText);

                //Home Tab - Styles
                conditionalFormatting = SR.GetString(ci, _ConditionalFormatting);
                formatAsTable = SR.GetString(ci, _FormatAsTable);
                normal = SR.GetString(ci, _Normal);
                bad = SR.GetString(ci, _Bad);

                //Home Tab - Cells
                insert = SR.GetString(ci, _Insert);
                delete = SR.GetString(ci, _Delete);
                format = SR.GetString(ci, _Format);
                deleteColumn = SR.GetString(ci, _DeleteColumn);
                deleteRow = SR.GetString(ci, _DeleteRow);
                deleteSheet = SR.GetString(ci, _DeleteSheet);
                //Home Tab - Styles 
                highlightCellsRules = SR.GetString(ci, _HighlightCellsRules);

                //Home Tab - Styles - HighlightCellsRules
                greaterThan = SR.GetString(ci, _GreaterThan);
                lessThan = SR.GetString(ci, _LessThan);
                between = SR.GetString(ci, _Between);
                equalTo = SR.GetString(ci, _EqualTo);

                //Home Tab - Cells - Insert
                insertRow = SR.GetString(ci, _InsertRow);
                insertColumn = SR.GetString(ci, _InsertColumn);
                insertSheet = SR.GetString(ci, _InsertSheet);

                //Home Tab - Cells - Format
                rowHeight = SR.GetString(ci, _RowHeight);
                columnWidth = SR.GetString(ci, _ColumnWidth);
                hideAndUnhide = SR.GetString(ci, _HideAndUnhide);
                renameSheet = SR.GetString(ci, _RenameSheet);

                //Home Tab - Cells - Format - HideAndUnhide
                hideRows = SR.GetString(ci, _HideRows);
                hideColumns = SR.GetString(ci, _HideColumns);
                hideSheet = SR.GetString(ci, _HideSheet);
                unhideRows = SR.GetString(ci, _UnhideRows);
                unhideColumns = SR.GetString(ci, _UnhideColumns);
                unhideSheet = SR.GetString(ci, _UnhideSheet);

                //Others Tab - Freeze
                freezePanes = SR.GetString(ci, _FreezePanes);
                freezeTopRow = SR.GetString(ci, _FreezeTopRow);
                freezeTopColumn = SR.GetString(ci, _FreezeTopColumn);

                //Others Tab - Themes

                office = SR.GetString(ci, _Office);
                adjacency = SR.GetString(ci, _Adjacency);
                apex = SR.GetString(ci, _Apex);
                angels = SR.GetString(ci, _Angels);
                apothecary = SR.GetString(ci, _Apothecary);
                aspect = SR.GetString(ci, _Aspect);
                austin = SR.GetString(ci, _Austin);
                blacktie = SR.GetString(ci, _Blacktie);
                civic = SR.GetString(ci, _Civic);
                clarity = SR.GetString(ci, _Clarity);
                composite = SR.GetString(ci, _Composite);
                concourse = SR.GetString(ci, _Concourse);
                couture = SR.GetString(ci, _Couture);
                elemental = SR.GetString(ci, _Elemental);
                equity = SR.GetString(ci, _Equity);
                essential = SR.GetString(ci, _Essential);

                //Others Tab - Outline
                outline = SR.GetString(ci, _Outline);
                group = SR.GetString(ci, _Group);
                ungroup = SR.GetString(ci, _Ungroup);
                isSummaryRowBelow = SR.GetString(ci, _IsSummaryRowBelow);
                isSummaryColumnAtRight = SR.GetString(ci, _IsSummaryColumnAtRight);
                rows = SR.GetString(ci, _Rows);
                columns = SR.GetString(ci, _Columns);
                direction = SR.GetString(ci, _Direction);

                //File Tab - Info - TextBlock
                permissions = SR.GetString(ci, _Permissions);
                encryptwithPassword = SR.GetString(ci, _EncryptwithPassword);
                protectCurrentSheet = SR.GetString(ci, _ProtectCurrentSheet);
                permissionsDescription = SR.GetString(ci, _PermissionsDescription);
                name = SR.GetString(ci, _Name);
                properties = SR.GetString(ci, _Properties);
                version = SR.GetString(ci, _Version);
                author = SR.GetString(ci, _Author);
                blankWorkbook = SR.GetString(ci, _BlankWorkbook);
                blankWorkbookDescription = SR.GetString(ci, _BlankWorkbookDescription);
                availableTemplate = SR.GetString(ci, _AvailableTemplate);

                //DataValidation Window
                validationCriteria = SR.GetString(ci, _ValidationCriteria);
                allow = SR.GetString(ci, _Allow);
                minimum = SR.GetString(ci, _Minimum);
                maximum = SR.GetString(ci, _Maximum);
                notBetween = SR.GetString(ci, _NotBetween);
                notEqualTo = SR.GetString(ci, _NotEqualTo);
                greaterThanOrEqualTo = SR.GetString(ci, _GreaterThanOrEqualTo);
                lessThanOrEqualTo = SR.GetString(ci, _LessThanOrEqualTo);

                anyValue = SR.GetString(ci, _AnyValue);
                wholeNumber = SR.GetString(ci, _WholeNumber);
                decimal_1 = SR.GetString(ci, _Decimal);
                list = SR.GetString(ci, _List);
                dateAndTime = SR.GetString(ci, _DateAndTime);
                textLength = SR.GetString(ci, _TextLength);
                formula = SR.GetString(ci, _Formula);

                stop = SR.GetString(ci, _Stop);
                information = SR.GetString(ci, _Information);
                warning = SR.GetString(ci, _Warning);

                ok = SR.GetString(ci, _Ok);
                cancel = SR.GetString(ci, _Cancel);

                //Conditional format window item
                lightRedFillwithDarkRedText = SR.GetString(ci, _LightRedFillwithDarkRedText);
                yellowFillwithDarkYellowText = SR.GetString(ci, _YellowFillwithDarkYellowText);
                greenFillwithDarkGreenText = SR.GetString(ci, _GreenFillwithDarkGreenText);
                lightRedFill = SR.GetString(ci, _LightRedFill);
                redText = SR.GetString(ci, _RedText);
                redBorder = SR.GetString(ci, _RedBorder);
                greaterThanWindowTitle = SR.GetString(ci, _GreaterThanWindowTitle);
                greaterThanDescription = SR.GetString(ci, _GreaterThanWindowDescription);
                lessThanWindowTitle = SR.GetString(ci, _LessWindowTitle);
                lessWindowDescription = SR.GetString(ci, _LessThanWindowDescription);
                betweenWindowTitle = SR.GetString(ci, _BetweenWindowTitle);
                betweenWindowDescription = SR.GetString(ci, _BetweenWindowDescription);
                notBetweenWindowTitle = SR.GetString(ci, _NotBetweenWindowTitle);
                notBetweenWindowDescription = SR.GetString(ci, _NotBetweenWindowDescription);
                equalToWindowTitle = SR.GetString(ci, _EqualToWindowTitle);
                equalToWindowDescription = SR.GetString(ci, _EqualToWindowDescription);
                and = SR.GetString(ci, _And);
                with = SR.GetString(ci, _With);

                //DataValidation
                settings = SR.GetString(ci, _Settings);
                data_small = SR.GetString(ci, _Data_small);
                inputMessage = SR.GetString(ci, _InputMessage);
                inputmessage_small = SR.GetString(ci, _Inputmessage_small);
                title = SR.GetString(ci, _Title);
                errorAlert = SR.GetString(ci, _ErrorAlert);
                clearAll = SR.GetString(ci, _ClearAll);
                inputmessageDescription = SR.GetString(ci, _InputmessageDescription);
                errorAlertDescription = SR.GetString(ci, _ErrorAlertDescription);
                errorMessage = SR.GetString(ci, _ErrorMessage);
                errorMessage_small = SR.GetString(ci, _ErrorMessage_small);
                dataValidationDescription = SR.GetString(ci, _DataValidationDescription);
                dataValidationTitle = SR.GetString(ci, _DataValidationTitle);
                value_1 = SR.GetString(ci, _Value);
                endDate = SR.GetString(ci, _EndDate);
                startDate = SR.GetString(ci, _StartDate);
                date = SR.GetString(ci, _Date);
                between_Lower = SR.GetString(ci, _Between_Lower);
                notbetween_Lower = SR.GetString(ci, _Notbetween_Lower);
                greaterThan_Lower = SR.GetString(ci, _GreaterThan_Lower);
                greaterThanOrEqualTo_Lower = SR.GetString(ci, _GreaterThanOrEqualTo_Lower);
                lessthan_Lower = SR.GetString(ci, _Lessthan_Lower);
                lessThanOrEqualTo_Lower = SR.GetString(ci, _LessThanOrEqualTo_Lower);
                equalto_Lower = SR.GetString(ci, _Equalto_Lower);
                notEqualTo_Lower = SR.GetString(ci, _NotEqualTo_Lower);
                source = SR.GetString(ci, _Source);

                protectWorkbook = SR.GetString(ci, _ProtectWorkbook);
                // protectSheet = SR.GetString(ci, _ProtectSheet);
                saveAsDifferentTypes = SR.GetString(ci, _SaveAsDifferentTypes);
                saveACopyOfTheItem = SR.GetString(ci, _SaveACopyOfTheItem);

                //...Text
                lessThanOption = SR.GetString(ci, _LessThanOption);
                greaterThanOption = SR.GetString(ci, _GreaterThanOption);
                unHideSheetOption = SR.GetString(ci, _UnHideSheetOption);
                rowHeightOption = SR.GetString(ci, _RowHeightOption);
                columnWidthOption = SR.GetString(ci, _ColumnWidthOption);
                equalToOption = SR.GetString(ci, _EqualToOption);
                betweenOption = SR.GetString(ci, _BetweenOption);

                autoFitRowHeight = SR.GetString(ci, _AutoFitRowHeight);
                autoFitColumnWidth = SR.GetString(ci, _AutoFitColumnWidth);
                border = SR.GetString(ci, _Border);

                topAlign = SR.GetString(ci, _TopAlign);
                middleAlign = SR.GetString(ci, _MiddleAlign);
                bottomAlign = SR.GetString(ci, _BottomAlign);

                leftAlign = SR.GetString(ci, _LeftAlign);
                centerAlign = SR.GetString(ci, _CenterAlign);
                rightAlign = SR.GetString(ci, _RightAlign);

                increaseIndent = SR.GetString(ci, _IncreaseIndent);
                decreaseIndent = SR.GetString(ci, _DecreaseIndent);

                accountingNumberFormat = SR.GetString(ci, _AccountingNumberFormat);
                percentStyle = SR.GetString(ci, _PercentStyle);
                commaStyle = SR.GetString(ci, _CommaStyle);
                increaseDecmial = SR.GetString(ci, _IncreaseDecmial);
                decreaseDecmial = SR.GetString(ci, _DecreaseDecmial);



                topBorder = SR.GetString(ci, _TopBorder);
                bottomBorder = SR.GetString(ci, _BottomBorder);
                leftBorder = SR.GetString(ci, _LeftBorder);
                rightBorder = SR.GetString(ci, _RightBorder);
                allBorder = SR.GetString(ci, _AllBorder);
                noBorder = SR.GetString(ci, _NoBorder);
                topandBottomBorder = SR.GetString(ci, _TopandBottomBorder);
                topandThickBottomBorder = SR.GetString(ci, _TopandThickBottomBorder);

                outSideBorder = SR.GetString(ci, _OutSideBorder);
                thickBottomBorder = SR.GetString(ci, _ThickBottomBorder);
                thickBoxBorder = SR.GetString(ci, _ThickBoxBorder);
                unProtectSheet = SR.GetString(ci, _UnProtectSheet);

                unFreezePanes = SR.GetString(ci, _UnFreezePanes);

                //Window
                formatAsTableDescription = SR.GetString(ci, _FormatAsTableDescription);
                columnWidthWindow = SR.GetString(ci, _ColumnWidthWindow);
                rowHeightWindow = SR.GetString(ci, _RowHeightWindow);

                columnWidthDescription = SR.GetString(ci, _ColumnWidthDescription);
                rowHeightDescription = SR.GetString(ci, _RowHeightDescription);
                password = SR.GetString(ci, "Password");
                address = SR.GetString(ci, "Address");
                texttodisplay = SR.GetString(ci, "Texttodisplay");
                comment = SR.GetString(ci, "Comment");
                structure = SR.GetString(ci, "Structure");
                windows = SR.GetString(ci, "Windows");

                general = SR.GetString(ci, _General);
                number = SR.GetString(ci, _Number);
                accounting = SR.GetString(ci, _Accounting);
                currency = SR.GetString(ci, _Currency);
                shortDate = SR.GetString(ci, _ShortDate);
                longDate = SR.GetString(ci, _LongDate);
                time = SR.GetString(ci, _Time);
                percentage = SR.GetString(ci, _Percentage);
#if SILVERLIGHT
            }
#endif
        }

        private string structure;
        public string Structure
        {
            get { return structure; }
            set { structure = value; }
        }

        private string windows;
        public string Windows
        {
            get { return windows; }
            set { windows = value; }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string texttodisplay;
        public string Texttodisplay
        {
            get { return texttodisplay; }
            set { texttodisplay = value; }
        }
        //BackStage
        private string newopt;
        public string New
        {
            get { return newopt; }
            set { newopt = value; }
        }

        private string open;
        public string Open
        {
            get { return open; }
            set { open = value; }
        }

        private string print;
        public string Print
        {
            get { return print; }
            set { print = value; }
        }

        private string save;
        public string Save
        {
            get { return save; }
            set { save = value; }
        }

        private string saveAs;
        public string SaveAs
        {
            get { return saveAs; }
            set { saveAs = value; }
        }

        private string info;
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        private string exit;
        public string Exit
        {
            get { return exit; }
            set { exit = value; }
        }


        #region DocumentProperties


        private string documentProperty_Title;
        private string documentMask_Title;
        private string documentProperty_Tag;
        private string documentMask_Tag;
        private string documentProperty_Comments;
        private string documentMask_Comments;
        private string documentProperty_Cataegory;
        private string documentMask_Category;
        private string documentProperty_Subject;
        private string documentMask_Subject;
        private string documentMask_AppName;
        private string documentProperty_AppName;
        private string documentProperty_RelatedPeople;
        private string documentProperty_Manager;
        private string documentMask_Manager;
        private string documentPropertiesAuthor;
        private string documentMask_Author;
        private string showAllProperies;
        private string showFewerProperties;
        private string documentProperties;
        private string documentProperty_Company;
        private string documentMask_Company;
        
        #region Document properties declaration

        /// <summary>
        /// Gets or sets the document properties title.
        /// </summary>
        /// <value>The document properties title.</value>
        public string DocumentProperty_Title
        {
            get { return documentProperty_Title; }
            set { documentProperty_Title = value; }
        }

       
        /// <summary>
        /// Gets or sets the document mask title.
        /// </summary>
        /// <value>The document mask title.</value>
        public string DocumentMask_Title
        {
            get { return documentMask_Title; }
            set { documentMask_Title = value; }
        }

      
        /// <summary>
        /// Gets or sets the document properties tag.
        /// </summary>
        /// <value>The document properties tag.</value>
        public string DocumentProperty_Tag
        {
            get { return documentProperty_Tag; }
            set { documentProperty_Tag = value; }
        }

      
        /// <summary>
        /// Gets or sets the document mask tag.
        /// </summary>
        /// <value>The document mask tag.</value>
        public string DocumentMask_Tag
        {
            get { return documentMask_Tag; }
            set { documentMask_Tag = value; }
        }

        
        /// <summary>
        /// Gets or sets the document properties comments.
        /// </summary>
        /// <value>The document properties comments.</value>
        public string DocumentProperty_Comments
        {
            get { return documentProperty_Comments; }
            set { documentProperty_Comments = value; }

        }

       
        /// <summary>
        /// Gets or sets the document mask comments.
        /// </summary>
        /// <value>The document mask comments.</value>
        public string DocumentMask_Comments
        {
            get { return documentMask_Comments; }
            set { documentMask_Comments = value; }
        }

       
        /// <summary>
        /// Gets or sets the document properties cataegories.
        /// </summary>
        /// <value>The document properties cataegories.</value>
        public string DocumentProperties_Cataegory
        {
            get { return documentProperty_Cataegory; }
            set { documentProperty_Cataegory = value; }
        }

        
        /// <summary>
        /// Gets or sets the document mask category.
        /// </summary>
        /// <value>The document mask category.</value>
        public string DocumentMask_Category
        {
            get { return documentMask_Category; }
            set { documentMask_Category = value; }
        }

       
        /// <summary>
        /// Gets or sets the document properties subject.
        /// </summary>
        /// <value>The document properties subject.</value>
        public string DocumentProperty_Subject
        {
            get { return documentProperty_Subject; }
            set { documentProperty_Subject = value; }
        }

       
        /// <summary>
        /// Gets or sets the document mask subject.
        /// </summary>
        /// <value>The document mask subject.</value>
        public string DocumentMask_Subject
        {
            get { return documentMask_Subject; }
            set { documentMask_Subject = value; }
        }

       
        /// <summary>
        /// Gets or sets the name of the document properties app.
        /// </summary>
        /// <value>The name of the document properties app.</value>
        public string DocumentProperty_AppName
        {
            get { return documentProperty_AppName; }
            set { documentProperty_AppName = value; }
        }

        
        /// <summary>
        /// Gets or sets the name of the document mask app.
        /// </summary>
        /// <value>The name of the document mask app.</value>
        public string DocumentMask_AppName
        {
            get { return documentMask_AppName; }
            set { documentMask_AppName = value; }
        }

       
        /// <summary>
        /// Gets or sets the document properties related people.
        /// </summary>
        /// <value>The document properties related people.</value>
        public string DocumentProperty_RelatedPeople
        {
            get { return documentProperty_RelatedPeople; }
            set { documentProperty_RelatedPeople = value; }
        }

        
        /// <summary>
        /// Gets or sets the document properties manager.
        /// </summary>
        /// <value>The document properties manager.</value>
        public string DocumentProperty_Manager
        {
            get { return documentProperty_Manager; }
            set { documentProperty_Manager = value; }
        }

       
        /// <summary>
        /// Gets or sets the document mask manager.
        /// </summary>
        /// <value>The document mask manager.</value>
        public string DocumentMask_Manager
        {
            get { return documentMask_Manager; }
            set { documentMask_Manager = value; }
        }

        
        /// <summary>
        /// Gets or sets the document properties author.
        /// </summary>
        /// <value>The document properties author.</value>
        public string DocumentProperty_Author
        {
            get { return documentPropertiesAuthor; }
            set { documentPropertiesAuthor = value; }
        }

       
        /// <summary>
        /// Gets or sets the document mask author.
        /// </summary>
        /// <value>The document mask author.</value>
        public string DocumentMask_Author
        {
            get { return documentMask_Author; }
            set { documentMask_Author = value; }
        }


        
        /// <summary>
        /// Gets or sets the show all properies.
        /// </summary>
        /// <value>The show all properies.</value>
        public string ShowAllProperies
        {
            get { return showAllProperies; }
            set { showAllProperies = value; }
        }

       
        /// <summary>
        /// Gets or sets the show fewer properties.
        /// </summary>
        /// <value>The show fewer properties.</value>
        public string ShowFewerProperties
        {
            get { return showFewerProperties; }
            set { showFewerProperties = value; }
        }

       
        /// <summary>
        /// Gets or sets the document properties.
        /// </summary>
        /// <value>The document properties.</value>
        public string DocumentProperties
        {
            get { return documentProperties; }
            set { documentProperties = value; }
        }

       
        /// <summary>
        /// Gets or sets the document properties company.
        /// </summary>
        /// <value>The document properties company.</value>
        public string DocumentProperty_Company
        {
            get { return documentProperty_Company; }
            set { documentProperty_Company = value; }
        }

       
        /// <summary>
        /// Gets or sets the document mask company.
        /// </summary>
        /// <value>The document mask company.</value>
        public string DocumentMask_Company
        {
            get { return documentMask_Company; }
            set { documentMask_Company = value; }
        }

        #endregion

        #endregion


        private string wrapText;
        /// <summary>
        /// Gets or sets the wrap text.
        /// </summary>
        /// <value>The wrap text.</value>
        public string WrapText
        {
            get { return wrapText; }
            set { wrapText = value; }
        }

        ///Clipboard
        private string clipboard;
        public string Clipboard
        {
            get { return clipboard; }
            set { clipboard = value; }
        }

        private string close;
        public string Close
        {
            get { return close; }
            set { close = value; }
        }

        private string copy;
        public string Copy
        {
            get { return copy; }
            set { copy = value; }
        }

        private string cut;
        public string Cut
        {
            get { return cut; }
            set { cut = value; }
        }

        private string paste;
        public string Paste
        {
            get { return paste; }
            set { paste = value; }
        }

        ///Font
        private string font;
        public string Font
        {
            get { return font; }
            set { font = value; }
        }

        //Formatting
        private string alignment;
        public string Alignment
        {
            get { return alignment; }
            set { alignment = value; }
        }

        private string number;
        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        private string mergeCenter;
        public string MergeCenter
        {
            get { return mergeCenter; }
            set { mergeCenter = value; }
        }

        private string styles;
        public string Styles
        {
            get { return styles; }
            set { styles = value; }
        }

        private string cells;
        public string Cells
        {
            get { return cells; }
            set { cells = value; }
        }

        //Tabs
        private string home;
        public string Home
        {
            get { return home; }
            set { home = value; }
        }

        private string file;
        public string File
        {
            get { return file; }
            set { file = value; }
        }

        private string others;
        public string Others
        {
            get { return others; }
            set { others = value; }
        }

        //Tab - Others
        private string picture;
        public string Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        private string illustrations;
        public string Illustrations
        {
            get { return illustrations; }
            set { illustrations = value; }
        }

        private string hyperlink;
        public string Hyperlink
        {
            get { return hyperlink; }
            set { hyperlink = value; }
        }

        private string links;
        public string Links
        {
            get { return links; }
            set { links = value; }
        }

        private string gridLines;
        public string GridLines
        {
            get { return gridLines; }
            set { gridLines = value; }
        }

        private string headings;
        public string Headings
        {
            get { return headings; }
            set { headings = value; }
        }

        private string formulaBar;
        public string FormulaBar
        {
            get { return formulaBar; }
            set { formulaBar = value; }
        }

        private string show;
        public string Show
        {
            get { return show; }
            set { show = value; }
        }

        private string dataValidation;
        public string DataValidation
        {
            get { return dataValidation; }
            set { dataValidation = value; }
        }

        private string data;
        public string Data
        {
            get { return data; }
            set { data = value; }
        }



        private string documentSettings;
        public string DocumentSettings
        {
            get { return documentSettings; }
            set { documentSettings = value; }
        }

        private string freeze;
        public string Freeze
        {
            get { return freeze; }
            set { freeze = value; }
        }

        private string view;
        public string View
        {
            get { return view; }
            set { view = value; }
        }

        private string themes;
        public string Themes
        {
            get { return themes; }
            set { themes = value; }
        }

        private string deleteComments;
        public string DeleteComments
        {
            get { return deleteComments; }
            set { deleteComments = value; }
        }

        private string editComments;
        public string EditComments
        {
            get { return editComments; }
            set { editComments = value; }
        }

        private string comments;
        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        const string _NewComment = "NewComment";
        private string newComment;
        public string NewComment
        {
            get { return newComment; }
            set { newComment = value; }
        }

        const string _EditComment = "EditComment";
        private string editComment;
        public string EditComment
        {
            get { return editComment; }
            set { editComment = value; }
        }

        const string _DeleteComment = "DeleteComment";
        private string deleteComment;
        public string DeleteComment
        {
            get { return deleteComment; }
            set { deleteComment = value; }
        }

        //const string _ProtectSheet = "ProtectSheet";
        private string protectsheet;
        public string ProtectSheet
        {
            get { return protectsheet; }
            set { protectsheet = value; }
        }

        //const string _ProtectWorkbook = "ProtectWorkbook";
        private string protectWorkbook;
        public string ProtectWorkbook
        {
            get { return protectWorkbook; }
            set { protectWorkbook = value; }
        }

        const string _Changes = "Changes";
        private string changes;
        public string Changes
        {
            get { return changes; }
            set { changes = value; }
        }
        //Home Tab - MergeCenter ComboBox
        
        private string mergeAndCenter;
        public string MergeAndCenter
        {
            get { return mergeAndCenter; }
            set { mergeAndCenter = value; }
        }

        private string unmergeCells;
        public string UnmergeCells
        {
            get { return unmergeCells; }
            set { unmergeCells = value; }
        }

        //Home Tab - Styles

        private string conditionalFormatting;
        public string ConditionalFormatting 
        {
            get { return conditionalFormatting; }
            set { conditionalFormatting = value; }
        }

        private string formatAsTable;
        public string FormatAsTable 
        {
            get { return formatAsTable; }
            set { formatAsTable = value; }
        }

        private string normal;
        public string Normal 
        {
            get { return normal; }
            set { normal = value; }
        }

        private string bad;
        public string Bad 
        {
            get { return bad; }
            set { bad = value; }
        }

        //Home Tab - Cells

        private string insert;
        public string Insert
        {
            get { return insert; }
            set { insert = value; }
        }

        private string delete;
        public string Delete
        {
            get { return delete; }
            set { delete = value; }
        }

        private string format;
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        //Home Tab - Styles 

        private string highlightCellsRules;
        public string HighlightCellsRules
        {
            get { return highlightCellsRules; }
            set { highlightCellsRules = value; }
        }

        //Home Tab - Styles - HighlightCellsRules

        private string greaterThan;
        public string GreaterThan
        {
            get { return greaterThan; }
            set { greaterThan = value; }
        }

        private string lessThan;
        public string LessThan
        {
            get { return lessThan; }
            set { lessThan = value; }
        }

        private string between;
        public string Between
        {
            get { return between; }
            set { between = value; }
        }

        private string equalTo;
        public string EqualTo
        {
            get { return equalTo; }
            set { equalTo = value; }
        }

        //Home Tab - Cells - Insert

        private string insertRow;
        public string InsertRow
        {
            get { return insertRow; }
            set { insertRow = value; }
        }

        private string insertColumn;
        public string InsertColumn
        {
            get { return insertColumn; }
            set { insertColumn = value; }
        }

        private string insertSheet;
        public string InsertSheet
        {
            get { return insertSheet; }
            set { insertSheet = value; }
        }

        //Home Tab - Cells - Delete

        private string deleteRow;
        public string DeleteRow
        {
            get { return deleteRow; }
            set { deleteRow = value; }
        }

        private string deleteColumn;
        public string DeleteColumn
        {
            get { return deleteColumn; }
            set { deleteColumn = value; }
        }

        private string deleteSheet;
        public string DeleteSheet
        {
            get { return deleteSheet; }
            set { deleteSheet = value; }
        }

        //Home Tab - Cells - Format

        private string rowHeight;
        public string RowHeight
        {
            get { return rowHeight; }
            set { rowHeight = value; }
        }

        private string columnWidth;
        public string ColumnWidth
        {
            get { return columnWidth; }
            set { columnWidth = value; }
        }

        private string hideAndUnhide;
        public string HideAndUnhide
        {
            get { return hideAndUnhide; }
            set { hideAndUnhide = value; }
        }

        private string renameSheet;
        public string RenameSheet
        {
            get { return renameSheet; }
            set { renameSheet = value; }
        }

        //Home Tab - Cells - Format - HideAndUnhide

        private string hideRows;
        public string HideRows
        {
            get { return hideRows; }
            set { hideRows = value; }
        }

        private string hideColumns;
        public string HideColumns
        {
            get { return hideColumns; }
            set { hideColumns = value; }
        }

        private string hideSheet;
        public string HideSheet
        {
            get { return hideSheet; }
            set { hideSheet = value; }
        }

        private string unhideRows;
        public string UnhideRows
        {
            get { return unhideRows; }
            set { unhideRows = value; }
        }

        private string unhideColumns;
        public string UnhideColumns
        {
            get { return unhideColumns; }
            set { unhideColumns = value; }
        }

        private string unhideSheet;
        public string UnhideSheet
        {
            get { return unhideSheet; }
            set { unhideSheet = value; }
        }

        //Others Tab - Freeze

        private string freezePanes;
        public string FreezePanes
        {
            get { return freezePanes; }
            set { freezePanes = value; }
        }

        private string freezeTopRow;
        public string FreezeTopRow
        {
            get { return freezeTopRow; }
            set { freezeTopRow = value; }
        }

        private string freezeTopColumn;
        public string FreezeTopColumn
        {
            get { return freezeTopColumn; }
            set { freezeTopColumn = value; }
        }

        //Others Tab - Themes
        
        private string office;
        public string Office
        {
            get { return office; }
            set { office = value; }
        }

        private string adjacency;
        public string Adjacency
        {
            get { return adjacency; }
            set { adjacency = value; }
        }

        private string apex;
        public string Apex
        {
            get { return apex; }
            set { apex = value; }
        }

        private string angels;
        public string Angels
        {
            get { return angels; }
            set { angels = value; }
        }

        private string apothecary;
        public string Apothecary
        {
            get { return apothecary; }
            set { apothecary = value; }
        }

        private string aspect;
        public string Aspect
        {
            get { return aspect; }
            set { aspect = value; }
        }

        private string austin;
        public string Austin
        {
            get { return austin; }
            set { austin = value; }
        }

        private string blacktie;
        public string Blacktie
        {
            get { return blacktie; }
            set { blacktie = value; }
        }

        private string civic;
        public string Civic
        {
            get { return civic; }
            set { civic = value; }
        }

        private string clarity;
        public string Clarity
        {
            get { return clarity; }
            set { clarity = value; }
        }

        private string composite;
        public string Composite
        {
            get { return composite; }
            set { composite = value; }
        }

        private string concourse;
        public string Concourse
        {
            get { return concourse; }
            set { concourse = value; }
        }

        private string couture;
        public string Couture
        {
            get { return couture; }
            set { couture = value; }
        }

        private string elemental;
        public string Elemental
        {
            get { return elemental; }
            set { elemental = value; }
        }

        private string equity;
        public string Equity
        {
            get { return equity; }
            set { equity = value; }
        }
        
        private string essential;
        public string Essential
        {
            get { return essential; }
            set { essential = value; }
        }


        //Others Tab - Outline

        private string outline;
        public string Outline
        {
            get { return outline; }
            set { outline = value; }
        }

        private string group;
        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        private string ungroup;
        public string Ungroup
        {
            get { return ungroup; }
            set { ungroup = value; }
            
        }

        private string isSummaryRowBelow;
        public string IsSummaryRowBelow
        {
            get { return isSummaryRowBelow; }
            set { isSummaryRowBelow = value; }
        }

        private string isSummaryColumnAtRight;
        public string IsSummaryColumnAtRight
        {
            get { return isSummaryColumnAtRight; }
            set { isSummaryColumnAtRight = value; }
        }

        private string rows;
        public string Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        private string columns;
        public string Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private string direction;
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        //File - Tab - Info Text

        private string permissions;
        public string Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }

        const string _ProtectCurrentSheet = "ProtectCurrentSheet";
        private string protectCurrentSheet;
        public string ProtectCurrentSheet
        {
            get { return protectCurrentSheet; }
            set { protectCurrentSheet = value; }
        }
        const string _EncryptwithPassword = "EncryptwithPassword";
        private string encryptwithPassword;
        public string EncryptwithPassword
        {
            get { return encryptwithPassword; }
            set { encryptwithPassword = value; }
        }
        const string _PermissionsDescription = "PermissionsDescription";
        public string permissionsDescription;
        public string PermissionsDescription
        {
            get { return permissionsDescription; }
            set { permissionsDescription = value; }
        }

        const string _AvailableTemplate = "AvailableTemplate";
        public string availableTemplate;
        public string AvailableTemplate
        {
            get { return availableTemplate; }
            set { availableTemplate = value; }
        }

        const string _Properties = "Properties";
        public string properties;
        public string Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        const string _Name = "Name";
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        const string _Version = "Version";
        private string version;
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        const string _Author = "Author";
        private string author;
        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        const string _BlankWorkbook = "BlankWorkbook";
        private string blankWorkbook;
        public string BlankWorkbook
        {
            get { return blankWorkbook; }
            set { blankWorkbook = value; }
        }

        const string _BlankWorkbookDescription = "BlankWorkbookDescription";
        private string blankWorkbookDescription;
        public string BlankWorkbookDescription
        {
            get { return blankWorkbookDescription; }
            set { blankWorkbookDescription = value; }
        }



        //DataValidation Window
        private string validationCriteria;
        public string ValidationCriteria
        {
            get { return validationCriteria; }
            set { validationCriteria = value; }
        }

        private string allow;
        public string Allow
        {
            get { return allow; }
            set { allow = value; }
        }

        private string minimum;
        public string Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        private string maximum;
        public string Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        private string notBetween;
        public string NotBetween
        {
            get { return notBetween; }
            set { notBetween = value; }
        }

        private string notEqualTo;
        public string NotEqualTo
        {
            get { return notEqualTo; }
            set { notEqualTo = value; }
        }

        private string greaterThanOrEqualTo;
        public string GreaterThanOrEqualTo
        {
            get { return greaterThanOrEqualTo; }
            set { greaterThanOrEqualTo = value; }
        }

        private string lessThanOrEqualTo;
        public string LessThanOrEqualTo
        {
            get { return lessThanOrEqualTo; }
            set { lessThanOrEqualTo = value; }
        }

        private string anyValue;
        public string AnyValue
        {
            get { return anyValue; }
            set { anyValue = value; }
        }

        private string wholeNumber;
        public string WholeNumber
        {
            get { return wholeNumber; }
            set { wholeNumber = value; }
        }

        private string decimal_1;
        public string Decimal
        {
            get { return decimal_1; }
            set { decimal_1 = value; }
        }

        private string list;
        public string List
        {
            get { return list; }
            set { list = value; }
        }

        private string dateAndTime;
        public string DateAndTime
        {
            get { return dateAndTime; }
            set { dateAndTime = value; }
        }

        private string textLength;
        public string TextLength
        {
            get { return textLength; }
            set { textLength = value; }
        }

        private string formula;
        public string Formula
        {
            get { return formula; }
            set { formula = value; }
        }

        private string stop;
        public string Stop
        {
            get { return stop; }
            set { stop = value; }
        }

        private string warning;
        public string Warning
        {
            get { return warning; }
            set { warning = value; }
        }

        private string information;
        public string Information
        {
            get { return information; }
            set { information = value; }
        }

        private string ok;
        public string Ok
        {
            get
            {
                return ok;
            }
            set
            {
                ok = value;
            }
        }

        private string cancel;
        public string Cancel
        {
            get
            {
                return cancel;
            }
            set
            {
                cancel = value;
            }

        }

        //Conditional Format Window elements

        private string lightRedFillwithDarkRedText;
        public string LightRedFillwithDarkRedText
        {
            get
            {
                return lightRedFillwithDarkRedText;
            }
            set
            {
                lightRedFillwithDarkRedText = value;
            }
        }

        private string yellowFillwithDarkYellowText;
        public string YellowFillwithDarkYellowText
        {
    
            get
            {
                return yellowFillwithDarkYellowText;
            }
            set
            {
                yellowFillwithDarkYellowText = value;
            }
        }

        private string greenFillwithDarkGreenText;
        public string GreenFillwithDarkGreenText
        {
            get
            {
                return greenFillwithDarkGreenText;
            }
            set
            {
                greenFillwithDarkGreenText = value;
            }
        }

        private string lightRedFill;
        public string LightRedFill
        {
            get
            {
                return lightRedFill;
            }
            set
            {
                lightRedFill = value;
            }
        }

        private string redText;
        public string RedText
        {
            get
            {
                return redText;
            }
            set
            {
                redText = value;
            }
        }

        private string redBorder;
        public string RedBorder
        {
            get
            {
                return redBorder;
            }
            set
            {
                redBorder = value;
            }
        }

        private string greaterThanWindowTitle;
        public string GreaterThanWindowTitle
        {
            get { return greaterThanWindowTitle; }
            set { greaterThanWindowTitle = value; }
        }

        private string greaterThanDescription;
        public string GreaterThanWindowDescription
        {
            get
            {
                return greaterThanDescription;
            }
            set
            {
                greaterThanDescription = value;
            }
        }

        private string lessThanWindowTitle;
        public string LessThanWindowTitle
        {
            get { return lessThanWindowTitle; }
            set { lessThanWindowTitle = value; }
        }

        private string lessWindowDescription;
        public string LessThanWindowDescription
        {
            get
            {
                return lessWindowDescription;
            }
            set
            {
                lessWindowDescription = value;
            }
        }

        private string betweenWindowTitle;
        public string BetweenWindowTitle
        {
            get { return betweenWindowTitle; }
            set { betweenWindowTitle = value; }
        }

        private string betweenWindowDescription;
        public string BetweenWindowDescription
        {
            get
            {
                return betweenWindowDescription;
            }
            set
            {
                betweenWindowDescription = value;
            }
        }

        private string notBetweenWindowTitle;
        public string NotBetweenWindowTitle
        {
            get { return notBetweenWindowTitle; }
            set { notBetweenWindowTitle = value; }
        }

        private string notBetweenWindowDescription;
        public string NotBetweenWindowDescription
        {
            get
            {
                return notBetweenWindowDescription;
            }
            set
            {
                notBetweenWindowDescription = value;
            }
        }

        private string equalToWindowTitle;
        public string EqualToWindowTitle
        {
            get { return equalToWindowTitle; }
            set { equalToWindowTitle = value; }
        }

        private string equalToWindowDescription;
        public string EqualToWindowDescription
        {
            get
            {
                return equalToWindowDescription;
            }
            set
            {
                equalToWindowDescription = value;
            }
        }

        private string and;
        public string And
        {
            get
            {
                return and;
            }
            set
            {
                and = value;
            }
        }

        private string with;
        public string With
        {
            get
            {
                return with;
            }
            set
            {
                with = value;
            }
        }

        //DataValidation Screen
        private string settings;
        public string Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        private string data_small;
        public string Data_small
        {
            get { return data_small; }
            set { data_small = value; }
        }

        private string inputMessage;
        public string InputMessage
        {
            get
            {
                return inputMessage;
            }
            set
            {
                inputMessage = value;
            }
        }

        private string inputmessage_small;
        public string Inputmessage_small
        {
            get { return inputmessage_small; }
            set { inputmessage_small = value; }
        }

        private string startDate;
        public string StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        private string endDate;
        public string EnddDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        private string value_1;
        public string Value
        {
            get { return value_1; }
            set { value_1 = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string errorAlert;
        public string ErrorAlert
        {
            get
            {
                return errorAlert;
            }
            set
            {
                errorAlert = value;
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public string errorMessage_small;
        public string ErrorMessage_small
        {
            get { return errorMessage_small; }
            set { errorMessage_small = value; }
        }


        private string dataValidationDescription;
        public string DataValidationDescription
        {
            get { return dataValidationDescription; }
            set { dataValidationDescription = value; }
        }

        private string dataValidationTitle;
        public string DataValidationTitle
        {
            get { return dataValidationTitle; }
            set { dataValidationTitle = value; }
        }

        private string clearAll;
        public string ClearAll
        {
            get
            {
                return clearAll;
            }
            set
            {
                clearAll = value;
            }
        }

        private string inputmessageDescription;
        public string InputmessageDescription
        {
            get { return inputmessageDescription; }
            set { inputmessageDescription = value; }
        }

        private string errorAlertDescription;
        public string ErrorAlertDescription
        {
            get { return errorAlertDescription; }
            set { errorAlertDescription = value; }
        }

        private string between_Lower;
        public string Between_Lower
        {
            get { return between_Lower; }
            set { between_Lower = value; }
        }

        private string notbetween_Lower;
        public string Notbetween_Lower
        {
            get { return notbetween_Lower; }
            set { notbetween_Lower = value; }
        }

        private string greaterThan_Lower;
        public string GreaterThan_Lower
        {
            get { return greaterThan_Lower; }
            set { greaterThan_Lower = value; }
        }

        private string greaterThanOrEqualTo_Lower;
        public string GreaterThanOrEqualTo_Lower
        {
            get { return greaterThanOrEqualTo_Lower; }
            set { greaterThanOrEqualTo_Lower = value; }
        }

        private string lessthan_Lower;
        public string Lessthan_Lower
        {
            get { return lessthan_Lower; }
            set { lessthan_Lower = value; }
        }

        private string lessThanOrEqualTo_Lower;
        public string LessThanOrEqualTo_Lower
        {
            get { return lessThanOrEqualTo_Lower; }
            set { lessThanOrEqualTo_Lower = value; }
        }

        private string equalto_Lower;
        public string Equalto_Lower
        {
            get { return equalto_Lower; }
            set { equalto_Lower = value; }
        }

        private string notEqualTo_Lower;
        public string NotEqualTo_Lower
        {
            get { return notEqualTo_Lower; }
            set { notEqualTo_Lower = value; }
        }

        private string source;
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        

        //private string protectSheet;
        //public string ProtectSheet
        //{
        //    get
        //    {
        //        return protectSheet;
        //    }
        //    set
        //    {
        //        protectSheet = value;
        //    }
        //}

        private string saveAsDifferentTypes;
        public string SaveAsDifferentTypes
        {
            get
            {
                return saveAsDifferentTypes;
            }
            set
            {
                saveAsDifferentTypes = value;
            }

        }

        private string saveACopyOfTheItem;
        public string SaveACopyOfTheItem
        {
            get
            {
                return saveACopyOfTheItem;
            }
            set
            {
                saveACopyOfTheItem = value;
            }
        }

        private string greaterThanOption;
        public string GreaterThanOption
        {
            get
            {
                return greaterThanOption;
            }
            set
            {
                greaterThanOption = value;
            }
        }


        private string lessThanOption;
        public string LessThanOption
        {
            get
            {
                return lessThanOption;
            }
            set
            {
                lessThanOption = value;
            }
        }

        private string unHideSheetOption;
        public string UnHideSheetOption
        {
            get
            {
                return unHideSheetOption;
            }
            set
            {
                unHideSheetOption = value;
            }
        }

        private string rowHeightOption;
        public string RowHeightOption
        {
            get
            {
                return rowHeightOption;
            }
            set
            {
                rowHeightOption = value;
            }
        }

        private string columnWidthOption;
        public string ColumnWidthOption
        {
            get
            {
                return columnWidthOption;
            }
            set
            {
                columnWidthOption = value;
            }
        }

        private string betweenOption;
        public string BetweenOption
        {
            get
            {
                return betweenOption;
            }
            set
            {
                betweenOption = value;
            }
        }

        private string equalToOption;
        public string EqualToOption
        {
            get
            {
                return equalToOption;
            }
            set
            {
                equalToOption = value;
            }
        }

        private string autoFitRowHeight;
        public string AutoFitRowHeight
        {
            get
            {
                return autoFitRowHeight;
            }
            set
            {
                autoFitRowHeight = value;
            }
        }

        private string autoFitColumnWidth;
        public string AutoFitColumnWidth
        {
            get
            {
                return autoFitColumnWidth;
            }
            set
            {
                autoFitColumnWidth = value;
            }
        }

        private string border;
        public string Border
        {
            get
            {
                return border;
            }
            set
            {
                border = value;
            }
        }

        private string topAlign;
        public string TopAlign
        {
            get
            {
                return topAlign;
            }
            set
            {
                topAlign = value;
            }
        }

        private string middleAlign;
        public string MiddleAlign
        {
            get
            {
                return middleAlign;
            }
            set
            {
                middleAlign = value;
            }
        }

        private string bottomAlign;
        public string BottomAlign
        {
            get
            {
                return bottomAlign;
            }
            set
            {
                bottomAlign = value;
            }
        }


        private string leftAlign;
        public string LeftAlign
        {
            get
            {
                return leftAlign;
            }
            set
            {
                leftAlign = value;
            }
        }

        private string centerAlign;
        public string CenterAlign
        {
            get
            {
                return centerAlign;
            }
            set
            {
                centerAlign = value;
            }
        }

        private string rightAlign;
        public string RightAlign
        {
            get
            {
                return rightAlign;
            }
            set
            {
                rightAlign = value;
            }
        }

        private string decreaseIndent;
        public string DecreaseIndent
        {
            get
            {
                return decreaseIndent;
            }
            set
            {
                decreaseIndent = value;
            }
        }

        private string increaseIndent;
        public string IncreaseIndent
        {
            get
            {
                return increaseIndent;
            }
            set
            {
                increaseIndent = value;
            }
        }

           private string accountingNumberFormat;
        public string AccountingNumberFormat
        {
            get
            {
                return accountingNumberFormat;
            }
            set
            {
                accountingNumberFormat = value;
            }
        }

         private string percentStyle;
        public string PercentStyle
        {
            get
            {
                return percentStyle;
            }
            set
            {
                percentStyle = value;
            }
        }

         private string commaStyle;
        public string CommaStyle
        {
            get
            {
                return commaStyle;
            }
            set
            {
                commaStyle = value;
            }
        }

        private string increaseDecmial;
        public string IncreaseDecmial
        {
            get
            {
                return increaseDecmial;
            }
            set
            {
                increaseDecmial = value;
            }
        }

        private string decreaseDecmial;
        public string DecreaseDecmial
        {
            get
            {
                return decreaseDecmial;
            }
            set
            {
                decreaseDecmial = value;
            }
        }


        private string topBorder;
        public string TopBorder
        {
            get
            {
                return topBorder;
            }
            set
            {
                topBorder = value;
            }
        }
        private string bottomBorder;
        public string BottomBorder
        {
            get
            {
                return bottomBorder;
            }
            set
            {
                bottomBorder = value;
            }
        }
        private string leftBorder;
        public string LeftBorder
        {
            get
            {
                return leftBorder;
            }
            set
            {
                leftBorder = value;
            }
        }
        private string rightBorder;
        public string RightBorder
        {
            get
            {
                return rightBorder;
            }
            set
            {
                rightBorder = value;
            }
        }
        private string noBorder;
        public string NoBorder
        {
            get
            {
                return noBorder;
            }
            set
            {
                noBorder = value;
            }
        }
        private string allBorder;
        public string AllBorder
        {
            get
            {
                return allBorder;
            }
            set
            {
                allBorder = value;
            }
        }
        private string topandBottomBorder;
        public string TopandBottomBorder
        {
            get
            {
                return topandBottomBorder;
            }
            set
            {
                topandBottomBorder = value;
            }
        }
        private string topandThickBottomBorder;
        public string TopandThickBottomBorder
        {
            get
            {
                return topandThickBottomBorder;
            }
            set
            {
                topandThickBottomBorder = value;
            }
        }

        private string formatAsTableDescription;
        public string FormatAsTableDescription
        {
            get { return formatAsTableDescription; }
            set { formatAsTableDescription = value; }
        }



        private string outSideBorder;
        public string OutSideBorder
        {
            get
            {
                return outSideBorder;
            }
            set
            {
                outSideBorder = value;
            }
        }

        private string columnWidthWindow;
        public string ColumnWidthWindow
        {
            get { return columnWidthWindow; }
            set { columnWidthWindow = value; }
        }



        private string thickBoxBorder;
        public string ThickBoxBorder
        {
            get
            {
                return thickBoxBorder;
            }
            set
            {
                thickBoxBorder = value;
            }
        }

        private string thickBottomBorder;
        public string ThickBottomBorder
        {
            get
            {
                return thickBottomBorder;
            }
            set
            {
                thickBottomBorder = value;
            }
        }

        private string unProtectSheet;
        public string UnProtectSheet
        {
            get
            {
                return unProtectSheet;
            }
            set
            {
                unProtectSheet = value;
            }
        }

        private string unFreezePanes;
        public string UnFreezePanes
        {
            get
            {
                return unFreezePanes;
            }
            set
            {
                unFreezePanes = value;
            }
        }

        private string rowHeightWindow;
        public string RowHeightWindow
        {
            get { return rowHeightWindow; }
            set { rowHeightWindow = value; }
        }

        private string columnWidthDescription;
        public string ColumnWidthDescription
        {
            get { return columnWidthDescription; }
            set { columnWidthDescription = value; }
        }

        private string rowHeightDescription;
        public string RowHeightDescription
        {
            get { return rowHeightDescription; }
            set { rowHeightDescription = value; }
        }

        private string general;
        public string General
        {
            get { return general; }
            set { general = value; }
        }

        private string currency;
        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        private string accounting;
        public string Accounting
        {
            get { return accounting; }
            set { accounting = value; }
        }

        private string shortDate;
        public string ShortDate
        {
            get { return shortDate; }
            set { shortDate = value; }
        }

        private string longDate;
        public string LongDate
        {
            get { return longDate; }
            set { longDate = value; }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        private string percentage;
        public string Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }

    }

    public static class SpreadsheetResourceWrapper
    {

        public static string General
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "General"); }
        }

        public static string Group
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Group"); }
        }

        public static string Ungroup
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Ungroup"); }
        }

        public static string Settings
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Settings"); }
        }

        public static string DataValidation
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "DataValidation"); }
        }

        public static string Number
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Number"); }
        }

        public static string Accounting
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Accounting"); }
        }

        public static string Currency
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Currency"); }
        }

        public static string ShortDate
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ShortDate"); }
        }

        public static string LongDate
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "LongDate"); }
        }

        public static string Time
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Time"); }
        }

        public static string Percentage
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Percentage"); }
        }

        public static string UnFreezePanes
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnFreezePanes"); }
        }

        public static string FreezePanes
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "FreezePanes"); }
        }

        public static string UnHideSheetOption
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnHideSheetOption"); }
        }

        public static string RowHeightOption
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "RowHeightOption"); }
        }

        public static string ColumnWidthOption
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ColumnWidthOption"); }
        }

        public static string AnyValue
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "AnyValue"); }
        }

        public static string WholeNumber
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "WholeNumber"); }
        }

        public static string Decimal
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Decimal"); }
        }

        public static string List
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "List"); }
        }

        public static string DateAndTime
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "DateAndTime"); }
        }

        public static string TextLength
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "TextLength"); }
        }

        public static string Between_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Between_Lower"); }
        }

        public static string Notbetween_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Notbetween_Lower"); }
        }

        public static string Equalto_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Equalto_Lower"); }
        }

        public static string NotEqualTo_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NotEqualTo_Lower"); }
        }

        public static string GreaterThan_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThan_Lower"); }
        }

        public static string Lessthan_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Lessthan_Lower"); }
        }

        public static string GreaterThanOrEqualTo_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThanOrEqualTo_Lower"); }
        }

        public static string LessThanOrEqualTo_Lower
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "LessThanOrEqualTo_Lower"); }
        }

        public static string Formula
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Formula"); }
        }

        public static string Source
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Source"); }
        }

        public static string Minimum
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Minimum"); }
        }

        public static string Maximum
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Maximum"); }
        }

        public static string Value
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Value"); }
        }

        public static string StartDate
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "StartDate"); }
        }

        public static string EndDate
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EndDate"); }
        }

        public static string Date
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Date"); }
        }

        public static string MinimumMaximumErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "MinimumMaximumErrorText"); }
        }

        public static string MinimumErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "MinimumErrorText"); }
        }

        public static string MaximumErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "MaximumErrorText"); }
        }

        public static string ValueErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValueErrorText"); }
        }

        public static string EndDateStartDateErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EndDateStartDateErrorText"); }
        }

        public static string StartDateErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "StartDateErrorText"); }
        }

        public static string EndDateErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EndDateErrorText"); }
        }

        public static string SourceErrorText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "SourceErrorText"); }
        }

        public static string ErrorMessageTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ErrorMessageTitle"); }
        }

        public static string RowHeightWindow
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "RowHeightWindow"); }
        }

        public static string RowHeightDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "RowHeightDescription"); }
        }

        public static string ColumnWidthWindow
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ColumnWidthWindow"); }
        }

        public static string ColumnWidthDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ColumnWidthDescription"); }
        }

        public static string RowHeight
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "RowHeight"); }
        }

        public static string ColumnWidth
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ColumnWidth"); }
        }

        public static string GreaterThan
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThan"); }
        }

        public static string LessThan
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "LessThan"); }
        }

        public static string Between
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Between"); }
        }

        public static string NotBetween
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NotBetween"); }
        }

        public static string EqualTo
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EqualTo"); }
        }

        public static string GreaterThanWindowDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThanWindowDescription"); }
        }

        public static string GreaterThanWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "GreaterThanWindowTitle"); }
        }

        public static string LessThanWindowDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "LessThanWindowDescription"); }
        }

        public static string LessThanWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "LessThanWindowTitle"); }
        }

        public static string BetweenWindowDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "BetweenWindowDescription"); }
        }

        public static string BetweenWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "BetweenWindowTitle"); }
        }

        public static string NotBetweenWindowDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NotBetweenWindowDescription"); }
        }

        public static string NotBetweenWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NotBetweenWindowTitle"); }
        }

        public static string EqualToWindowDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EqualToWindowDescription"); }
        }

        public static string EqualToWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EqualToWindowTitle"); }
        }

        public static string EncryptCommandWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EncryptCommandWindowTitle"); }
        }

        public static string EncryptCommandWindowDescriptionText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EncryptCommandWindowDescriptionText"); }
        }

        public static string EncryptCommandWindowDescription 
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EncryptCommandWindowDescription"); }
        }

        public static string FormatAsTableDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "FormatAsTableDescription"); }
        }

        public static string FormatAsTableWindowTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "FormatAsTableWindowTitle"); }
        }

        public static string Password
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Password"); }
        }

        public static string InsertHyperlink
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "InsertHyperlink"); }
        }

        public static string NewComment
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "NewComment"); }
        }

        public static string EditComment
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "EditComment"); }
        }

        public static string ProtectSheetDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectSheetDescription"); }
        }

        public static string ProtectSheetDescriptionText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectSheetDescriptionText"); }
        }

        public static string ProtectSheetTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectSheetTitle"); }
        }

        public static string UnProtectSheetDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnProtectSheetDescription"); }
        }

        public static string UnProtectSheetDescriptionText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnProtectSheetDescriptionText"); }
        }

        public static string UnProtectSheetTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnProtectSheet"); }
        }

        public static string ProtectWorkbookDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectWorkbookDescription"); }
        }

        public static string ProtectWorkbookDescriptionText
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectWorkbookDescriptionText"); }
        }

        public static string ProtectWorkbookTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectWorkbookTitle"); }
        }

        public static string Unhide
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Unhide"); }
        }

        public static string UnhideSheetDescription
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnhideSheetDescription"); }
        }

        public static string IsSummaryRowBelow
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "IsSummaryRowBelow"); }
        }

        public static string IsSummaryColumnAtRight
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "IsSummaryColumnAtRight"); }
        }

        public static string MessageBoxCaption
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "MessageBoxCaption"); }
        }
        /// <summary>
        /// /
        /// </summary>
        public static string ValidationMessage_BuildInStyleNotSupportedInExcelVersion
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_BuildInStyleNotSupportedInExcelVersion"); }
        }
        public static string ValidationMessage_EnteredInvalidData
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_EnteredInvalidData"); }
        }
        public static string ValidationMessage_EnterValidRange
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_EnterValidRange"); }
        }
        public static string ValidationMessage_SelectCellToInsertHyperlink
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_SelectCellToInsertHyperlink"); }
        }
        public static string ValidationMessage_Error
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_Error"); }
        }
        public static string ValidationMessage_SelectCellToInsertComment
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_SelectCellToInsertComment"); }
        }
        public static string ValidationMessage_SheetAlreadyPotected
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_SheetAlreadyPotected"); }
        }
        public static string ValidationMessage_SheetNotFound
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_SheetNotFound"); }
        }
        public static string ValidationMessage_DeleteSheet
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_DeleteSheet"); }
        }

        public static string ValidationMessage_DeleteNonEmptySheet
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_DeleteNonEmptySheet"); }
        }

        public static string ValidationMessage_PasteOnMergedCells
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_PasteOnMergedCells"); }
        }

        public static string ValidationMessage_ReadOnlyCells
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_ReadOnlyCells"); }
        }

        public static string ValidationMessage_NotEnoughSpaceToPaste
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_NotEnoughSpaceToPaste"); }
        }

        public static string ValidationMessage_HideSheet
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_HideSheet"); }
        }
        public static string ValidationMessage_DiscardChanges
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_DiscardChanges"); }
        }
        public static string ValidationMessage_InCorrectPassword
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_InCorrectPassword"); }
        }
        public static string ValidationMessage_ValueIsNoValid
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationMessage_ValueIsNoValid"); }
        }
        public static string Warning
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Warning"); }
        }
        public static string ValidationFailed
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ValidationFailed"); }
        }

        public static string UnprotectWorkbookTitle
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnprotectWorkbookTitle"); }
        }

        public static string Hide
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Hide"); }
        }

        public static string Insert
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Insert"); }
        }

        public static string ProtectSheet
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "ProtectSheet"); }
        }

        public static string UnProtectSheet
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "UnProtectSheet"); }
        }

        public static string Delete
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "Delete"); }
        }

        public static string File
        {
            get { return SR.GetString(CultureInfo.CurrentUICulture, "File"); }
        }
    }
}
