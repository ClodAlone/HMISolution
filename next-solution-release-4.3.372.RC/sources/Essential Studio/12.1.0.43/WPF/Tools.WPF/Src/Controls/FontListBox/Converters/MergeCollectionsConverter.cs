// <copyright file="MergeCollectionsConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Used for <see cref="System.Collections.ICollection"/> collections merging.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MergeCollectionsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Merges <see cref="System.Collections.ICollection"/> collections into one collection.
        /// </summary>
        /// <param name="values">Collections which should be merged.</param>
        /// <param name="targetType">Type of the merged collection.</param>
        /// <param name="parameter">Parameter is not used.</param>
        /// <param name="culture">Culture is not used.</param>
        /// <returns>
        /// Merged collection.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ICollection theme = (ICollection)values[0];
            ICollection recently = (ICollection)values[1];
            ICollection all = (ICollection)values[2];

            FontFamilyRecordCollection colResult = new FontFamilyRecordCollection();

            if (theme != null)
            {
                AddToCollection(colResult, theme, FontFamilyRecordType.Theme);
                AttachToCollection(all, colResult, FontFamilyRecordType.Common, null);
            }

            if (recently != null)
            {
                AddToCollection(colResult, recently, FontFamilyRecordType.RecentlyUsed);
                AttachToCollection(recently, colResult, FontFamilyRecordType.RecentlyUsed, all);
            }

            if (all != null)
            {
                AddToCollection(colResult, all, FontFamilyRecordType.Common);
                AttachToCollection(theme, colResult, FontFamilyRecordType.Theme, all);
            }

            return colResult;
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Traces add action in the collection.
        /// </summary>
        /// <param name="colResult">Result collection.</param>
        /// <param name="collection">Collection that should be traced.</param>
        /// <param name="fontPurposeType">Purpose for FontFamilyRecord.</param>
        private void AddToCollection(FontFamilyRecordCollection colResult, ICollection collection, FontFamilyRecordType fontPurposeType)
        {
            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");

            foreach (FontFamily fontFamily in collection)
            {
                FontFamilyRecord record = new FontFamilyRecord();
                record.Family = fontFamily;
                record.Type = fontPurposeType;

                if (fontPurposeType == FontFamilyRecordType.Theme && fontFamily is ThemeFontFamily)
                {
                    record.Purpose = ((ThemeFontFamily)fontFamily).Purpose;
                }

                record.Name = fontFamily.FamilyNames[userLanguage];

                colResult.Add(record);
            }
        }

        /// <summary>
        /// Attaches to collection for tracing.
        /// </summary>
        /// <param name="collection">FontFamily collection.</param>
        /// <param name="target">FontFamilyRecord target collection.</param>
        /// <param name="purpose">Purpose of the font family.</param>
        /// <param name="mainCollection">Main collection.</param>
        private void AttachToCollection(ICollection collection, FontFamilyRecordCollection target, FontFamilyRecordType purpose, ICollection mainCollection)
        {
            INotifyCollectionChanged colChange = collection as INotifyCollectionChanged;

            if (colChange != null)
            {
                CollectionSync sync = new CollectionSync(target, collection, purpose, mainCollection);
            }
        }
    }
}