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
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// This class used to store the Seralizable Property
    /// </summary>
#if !SILVERLIGHT    
    [Serializable]
#endif
   public class GridTreeSerializableProperty:GridModel
    {       
        public bool AllowAutoSizingNodeColumn
        {
            get;
            set;
        }

        public bool AllowDragColumns
        {
            get;
            set;
        }

        public bool AllowSort
        {
            get;
            set;
        }

        public List<SortState> SortStates
        {
            get;
            set;
        }

        public string SortProperty
        {
            get;
            set;
        }

        public bool EnableHotRowMarker
        {
            get;
            set;
        }

        public bool EnableNodeSelection
        {
            get;
            set;
        }

        public bool EnableRenderCheckIfGlyphNeeded
        {
            get;
            set;
        }

        public bool EnableSelections
        {
            get;
            set;
        }

        public GridTreeStartUpExpandState ExpandStateAtStartUp
        {
            get;
            set;
        }

        public bool FreezeExpandColumn
        {
            get;
            set;
        }

        public bool HideEmptyChildGlyphs
        {
            get;
            set;
        }

        public bool IgnoreResetOnListChanged
        {
            get;
            set;
        }

        public bool ReadOnly
        {
            get;
            set;
        }       
             
        public bool ShowColumnHeaders
        {
            get;
            set;
        }

        public bool ShowExpandColumnBorders
        {
            get;
            set;
        }

        public bool ShowRowHeader
        {
            get;
            set;
        }

        public bool SupportNodeImages
        {
            get;
            set;
        }

        public bool SupportRowSizing
        {
            get;
            set;
        }       

        public bool TrackSelectionOnCollectionChange
        {
            get;
            set;
        }

        public VisualStyle VisualStyle
        {
            get;
            set;
        }
    }
}
