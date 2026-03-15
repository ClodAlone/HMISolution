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
using System.Linq.Expressions;

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotSchemaDesigner
#else
using System.Reflection;

namespace Syncfusion.Silverlight.Controls.PivotSchemaDesigner
#endif
{
#if SyncfusionFramework4_0 
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotTableField : INotifyPropertyChanged
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Silverlight.Controls.PivotSchemaDesigner.PivotTableField">PivotTableField</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public PivotTableField()
        {

        }

        /// <summary>
        /// Gets or sets the field header.
        /// </summary>
        /// <value>
        /// The field header.
        /// </value>
        public string FieldHeader
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total header.
        /// </summary>
        /// <value>
        /// The total header.
        /// </value>
        public string TotalHeader
        {
            get;
            set;
        }
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string FieldName
        {
            get
            {
                return this.FieldPropertyDescriptor.Name;
            }
        }

        PropertyDescriptor _FieldPropertyDescriptor;
        /// <summary>
        /// Gets or sets the property descriptor.
        /// </summary>
        public PropertyDescriptor FieldPropertyDescriptor
        {
            get
            {
                return _FieldPropertyDescriptor;
            }
            set
            {
                _FieldPropertyDescriptor = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        public Type DataType
        {
            get
            {
                return this.FieldPropertyDescriptor.PropertyType;
            }
        }
     
        /// <summary>
        /// Gets or sets the summary type for calculations use.
        /// </summary>
        public Syncfusion.PivotAnalysis.Base.SummaryType SummaryType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the summary for calculations use.
        /// </summary>

        public Syncfusion.PivotAnalysis.Base.SummaryBase Summary
        {
            get;
            set;
        }
#else

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName
        {
            get
            {
                return this.FieldPropertyDescriptor.Name;
            }
        }

        /// <summary>
        /// Gets or sets the type of the summary.
        /// </summary>
        /// <value>
        /// The type of the summary.
        /// </value>
        internal Syncfusion.PivotAnalysis.Base.Silverlight.SummaryType SummaryType
        {
            get;
            set;
        }

        PropertyInfo _FieldPropertyDescriptor;
        /// <summary>
        /// Gets or sets the field property descriptor.
        /// </summary>
        /// <value>
        /// The field property descriptor.
        /// </value>
        public PropertyInfo FieldPropertyDescriptor
        {
            get
            {
                return _FieldPropertyDescriptor;
            }
            set
            {
                _FieldPropertyDescriptor = value;
            }
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public Type DataType
        {
            get
            {
                return this.FieldPropertyDescriptor.GetType();
            }
        }
#endif 
        /// <summary>
        /// Gets or sets the format for this pivot field values.
        /// </summary>
        public string Format { get; set; }

        bool _IsSelected;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                this.OnPropertyChanged(sp => sp.IsSelected);
            }
        }

        private bool allowRunTimeGroupByField = true;
        /// <summary>
        /// Gets or sets the value to enable/disable grouping for this pivot item. Default value is true.
        /// </summary>
        public bool AllowRunTimeGroupByField
        {
            get
            {
                return allowRunTimeGroupByField;
            }
            set
            {
                if (allowRunTimeGroupByField != value)
                {
                    allowRunTimeGroupByField = value;
                    this.OnPropertyChanged(sp => sp.AllowRunTimeGroupByField);
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// An event that notifies user whenever property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<R>(Expression<Func<PivotTableField, R>> expr)
        {
            OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
