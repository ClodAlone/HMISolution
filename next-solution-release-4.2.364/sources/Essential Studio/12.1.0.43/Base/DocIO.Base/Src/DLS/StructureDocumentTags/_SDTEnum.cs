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

#endregion
namespace Syncfusion.DocIO.DLS
{
    #region Enum Structure Document Type
    /// <summary>
    /// 
    /// </summary>
    internal enum StructureDocumentType
    {
        None,
        ComboBox,
        DropDownList,
        Picture,
        RichText,
        Text,
        //Currently no implementation handled for equations.
        Equation,
        CheckBox,
        DatePicker
    }
    #endregion

    #region Enum Lock settings
    /// <summary>
    /// 
    /// </summary>
    internal enum LockSettings
    {
        UnLocked,
        ContentLocked,
        SDTContentLocked,
        SDTLocked
    }
    #endregion

    #region Enum CalendarType
    internal enum CalendarType
    {
        Gregorian,
        GregorianArabic,
        GregorianMiddleEastFrench,
        GregorianEnglish,
        GregorianTransliteratedEnglish,
        GregorianTransliteratedFrench,
        Hebrew,
        Hijri,
        Japan,
        Korean,
        Taiwan,
        Thai,
        Saka,
        None,
    }
    #endregion

    #region Enum Content repeating type
    /// <summary>
    /// 
    /// </summary>
    internal enum ContentRepeatingType
    {
        None,
        RepeatingSection,
        RepeatingSectionItem
    }
    #endregion
}

   
