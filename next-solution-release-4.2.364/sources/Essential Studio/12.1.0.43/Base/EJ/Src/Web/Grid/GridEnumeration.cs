#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript
{
    [DataContract]
    public enum SelectionType
    {
        [EnumMember(Value = "single")]
        Single,
        [EnumMember(Value = "multiple")]
        Multiple
    }
    [DataContract]
    public enum EditingType
    {
        [EnumMember(Value = "stringedit")]
        StringEdit,
        [EnumMember(Value = "booleanedit")]
        BooleanEdit,
        [EnumMember(Value = "numericedit")]
        NumericEdit,
        [EnumMember(Value = "datepicker")]
        Datepicker,
        [EnumMember(Value = "datetimepicker")]
        DateTimePicker,
        [EnumMember(Value = "dropdownedit")]
        DropdownEdit
    }
    [DataContract]
    public enum FilterType
    {
        [EnumMember(Value = "filterbar")]
        FilterBar,
        [EnumMember(Value = "menu")]
        Menu
    }
    [DataContract]
    public enum FilterBarMode
    {
        [EnumMember(Value = "immediate")]
        Immediate,
        [EnumMember(Value = "onenter")]
        OnEnter
    }
    [DataContract]
    public enum EditMode
    {
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "dialog")]
        Dialog,
        [EnumMember(Value = "dialogtemplate")]
        DialogTemplate,
        [EnumMember(Value = "externalform")]
        ExternalForm,
        [EnumMember(Value = "externalformtemplate")]
        ExternalFormTemplate,
        [EnumMember(Value = "inlineform")]
        InlineForm,
        [EnumMember(Value = "inlineformtemplate")]
        InlineTemplateForm,
        [EnumMember(Value = "batch")]
        Batch
    }
    [DataContract]
    public enum EditEvent
    {
        [EnumMember(Value = "doubleclick")]
        DoubleClick,
        [EnumMember(Value = "none")]
        None
    }
    [DataContract]
    public enum FormPosition
    {
        [EnumMember(Value = "bottomLeft")]
        BottomLeft,
        [EnumMember(Value = "topRight")]
        TopRight
    }
    [DataContract]
    public enum SummaryType
    {
        [EnumMember(Value = "average")]
        Average,
        [EnumMember(Value = "minimum")]
        Minimum,
        [EnumMember(Value = "maximum")]
        Maximum,
        [EnumMember(Value = "count")]
        Count,
        [EnumMember(Value = "sum")]
        Sum,
        [EnumMember(Value = "truecount")]
        Truecount,
        [EnumMember(Value = "falsecount")]
        Falsecount,
        [EnumMember(Value = "custom")]
        Custom
    }
    [DataContract]
    public enum UnboundType
    {
        [EnumMember(Value = "edit")]
        Edit,
        [EnumMember(Value = "save")]
        Save,
        [EnumMember(Value = "delete")]
        Delete,
        [EnumMember(Value = "cancel")]
        Cancel
    }
    [DataContract]
    public enum ToolBarItems
    {
        [EnumToString("add")]
        Add,
        [EnumToString("edit")]
        Edit,
        [EnumToString("delete")]
        Delete,
        [EnumToString("update")]
        Update,
        [EnumToString("cancel")]
        Cancel,
        [EnumToString("search")]
        Search
    }
   }
