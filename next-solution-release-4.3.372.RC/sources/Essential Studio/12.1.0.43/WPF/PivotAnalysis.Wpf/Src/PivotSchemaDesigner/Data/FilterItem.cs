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
using System.ComponentModel;
using System.Reflection;



#if !SILVERLIGHT
using Syncfusion.Windows.Controls.PivotGrid;
using System.Xml.Serialization;
using Syncfusion.PivotAnalysis.Base;


namespace Syncfusion.Windows.Controls.PivotSchemaDesigner
#else
using System.Xml.Serialization;
using Syncfusion.PivotAnalysis.Base.Silverlight;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// This is a collection of FilterItemElement and it generates the filter expression
    /// based on the selected FilterItemElement
    /// </summary>

#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

#if !SILVERLIGHT
    [Serializable]
#else
#endif
    public class GridFilter
    {
        /// <summary>
        /// Initializes the <see cref="GridFilter"/> class.
        /// </summary>
        public GridFilter()
        {
            this.FilterItems = new List<FilterItemElement>();
        }

#if SILVERLIGHT
        [XmlIgnore]
        public object FilterProperty
        {
            get;
            set;
        }
#else
        /// <summary>
        /// gets or sets the filter properties
        /// </summary>
        [XmlIgnore]
        public PropertyDescriptor FilterProperty
        {
            get;
            set;
        }
#endif
        /// <summary>
        /// gets or sets a value indicating whether the item located at filter header area
        /// </summary>
        public bool IsFilterHeaderArea { get; set; }
        /// <summary>
        /// Gets or sets the name of filter item that present at schema designer
        /// Name and Header may be same of different but the value set at header will be display at schema designer's filter area
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the header of filter item that present at schema designer 
        /// Name and Header may be same of different but the value set at header will be display at schema designer's filter area
        /// </summary>
      
        public string Header { get; set; }
        /// <summary> 
        /// Gets or sets the list of filter items
        /// </summary>
        public List<FilterItemElement> FilterItems { get; set; }
        /// <summary>
        /// Gets or sets filteritem instance of "All" checkbox
        /// </summary>
        public FilterItemElement AllFilterItem { get; set; }
        /// <summary>
        /// gets or sets the list of filtered item 
        /// </summary>
        public List<string> FilteredValues { get; set; }
    }

}