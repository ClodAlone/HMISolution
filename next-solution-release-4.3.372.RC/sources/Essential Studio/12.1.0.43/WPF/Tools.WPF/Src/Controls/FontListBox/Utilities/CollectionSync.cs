// <copyright file="CollectionSync.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Used for font collections tracing and synchronization.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CollectionSync : IDisposable
    {
        /// <summary>
        /// Stores the target collection
        /// </summary>
        private WeakReference m_targetCollection;

        /// <summary>
        /// Stores the sourceCollection
        /// </summary>
        private ICollection m_sourceCollection;

        /// <summary>
        /// Stores the mainCollection
        /// </summary>
        private ICollection m_mainCollection;

        /// <summary>
        /// Stores the fontPurposeType
        /// </summary>
        private FontFamilyRecordType m_fontPurposeType;

        /// <summary>
        /// Initializes a new instance of the CollectionSync class.
        /// </summary>
        /// <param name="target">FontFamilyRecord target collection.</param>
        /// <param name="source">FontFamily collection.</param>
        /// <param name="purpose">Purpose of the font family.</param>
        /// <param name="mainCollection">Main collection.</param>
        public CollectionSync(FontFamilyRecordCollection target, ICollection source, FontFamilyRecordType purpose, ICollection mainCollection)
        {
            m_targetCollection = new WeakReference(target);
            m_sourceCollection = source;

            m_fontPurposeType = purpose;

            m_mainCollection = mainCollection;

            INotifyCollectionChanged colChange = (INotifyCollectionChanged)source;
            colChange.CollectionChanged -= ColChange_CollectionChanged;
            colChange.CollectionChanged += ColChange_CollectionChanged;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the <see cref="System.Collections.ICollection"/> collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance
        /// containing the event data.</param>
        public void ColChange_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!m_targetCollection.IsAlive)
            {
                Dispose();
                return;
            }

            FontFamilyRecordCollection target = (FontFamilyRecordCollection)m_targetCollection.Target;

            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US" /*CultureInfo.CurrentUICulture.IetfLanguageTag*/);

            int lastThemePosition = -1;

            if (m_mainCollection != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (FontFamily font in e.NewItems)
                        {
                            if (font != null)
                            {
                                FontFamilyRecord record = new FontFamilyRecord();
                                record.Family = font;
                                record.Type = m_fontPurposeType;
                                record.Name = font.FamilyNames[userLanguage];

                                bool bOperation = ((FontCollection)m_mainCollection).ContainsName(font);

                                if (bOperation)
                                {
                                    if (!target.Contains(record))
                                    {
                                        // for correct keyboard navigation.
                                        switch (record.Type)
                                        {
                                            case FontFamilyRecordType.Theme:

                                                if (font is ThemeFontFamily)
                                                {
                                                    ThemeFontFamily themeFont = (ThemeFontFamily)font;
                                                    record.Purpose = themeFont.Purpose;
                                                }

                                                target.Add(record);
                                                break;

                                            case FontFamilyRecordType.RecentlyUsed:
                                                if (lastThemePosition == -1)
                                                {
                                                    lastThemePosition = 0;
                                                    while (target[lastThemePosition].Type == FontFamilyRecordType.Theme)
                                                    {
                                                        lastThemePosition++;
                                                    }
                                                }

                                                target.Insert(lastThemePosition, record);
                                                lastThemePosition++;
                                                break;

                                            case FontFamilyRecordType.Common:
                                                target.Add(record);
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (FontFamily font in e.OldItems)
                        {
                            FontFamilyRecord record = new FontFamilyRecord();
                            record.Family = font;
                            record.Type = m_fontPurposeType;
                            record.Name = font.FamilyNames[userLanguage];

                            bool bOperation = ((FontCollection)m_mainCollection).ContainsName(font);

                            if (bOperation)
                            {
                                target.Remove(record);
                            }
                        }

                        break;

                    case NotifyCollectionChangedAction.Reset:
                        foreach (FontFamily font in m_mainCollection)
                        {
                            FontFamilyRecord record = new FontFamilyRecord();
                            record.Family = font;
                            record.Type = m_fontPurposeType;
                            record.Name = font.FamilyNames[userLanguage];

                            while (target.Contains(record))
                            {
                                target.Remove(record);
                            }
                        }

                        break;

                    case NotifyCollectionChangedAction.Move:

                        int oldIndex = e.NewStartingIndex;
                        int newIndex = e.OldStartingIndex;

                        if (m_sourceCollection.Count > 1 && newIndex != oldIndex)
                        {
                            newIndex = 1;
                        }

                        FontFamily oldFont = ((FontCollection)m_sourceCollection)[oldIndex];
                        FontFamily newFont = ((FontCollection)m_sourceCollection)[newIndex];

                        FontFamilyRecord oldRecord = new FontFamilyRecord();
                        oldRecord.Family = oldFont;
                        oldRecord.Type = m_fontPurposeType;
                        oldRecord.Name = oldFont.FamilyNames[userLanguage];

                        FontFamilyRecord newRecord = new FontFamilyRecord();
                        newRecord.Family = newFont;
                        newRecord.Type = m_fontPurposeType;
                        newRecord.Name = newFont.FamilyNames[userLanguage];

                        int oldTargetItemPos = target.IndexOf(oldRecord);
                        int newTargetItemPos = target.IndexOf(newRecord);

                        if (oldTargetItemPos != -1 && newTargetItemPos != -1 && oldTargetItemPos != newTargetItemPos)
                        {
                            target.Move(oldTargetItemPos, newTargetItemPos);
                        }

                        break;

                    case NotifyCollectionChangedAction.Replace:
                        MessageBox.Show("NotifyCollectionChangedAction.Replace");
                        break;
                }
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void Dispose()
        {
            INotifyCollectionChanged colChange = (INotifyCollectionChanged)m_sourceCollection;
            colChange.CollectionChanged -= ColChange_CollectionChanged;
            m_targetCollection = null;
            m_sourceCollection = null;
        }
    }
}