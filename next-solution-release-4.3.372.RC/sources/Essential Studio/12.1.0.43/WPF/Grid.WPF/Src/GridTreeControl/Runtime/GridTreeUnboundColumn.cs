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
using System.Windows;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Syncfusion.Windows.Controls.Grid
{
    public sealed class GridTreeUnboundColumn : GridTreeColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridTreeUnboundColumn"/> class.
        /// </summary>
        public GridTreeUnboundColumn()
        {
            this.IsUnbound = true;
            if (this.MappingName == null || this.MappingName == string.Empty)
                this.MappingName = "UnboundMappingName";
        }

        #region Format

        [TypeConverter(typeof(GridDataFormatConverter))]
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(GridTreeUnboundColumn), new PropertyMetadata(string.Empty));

        #endregion

        #region Expression
        public string Expression
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        public static readonly DependencyProperty ExpressionProperty =
            DependencyProperty.Register("Expression", typeof(string), typeof(GridTreeUnboundColumn), new PropertyMetadata(string.Empty));

        #endregion

        #region CaseSensitive

        public bool CaseSensitive
        {
            get { return (bool)GetValue(CaseSensitiveProperty); }
            set { SetValue(CaseSensitiveProperty, value); }
        }

        public static readonly DependencyProperty CaseSensitiveProperty =
            DependencyProperty.Register("CaseSensitive", typeof(bool), typeof(GridTreeUnboundColumn), new PropertyMetadata(true));

        #endregion

        #region error support

        ExpressionError error = ExpressionError.None;
        internal ExpressionError Error
        {
            get { return error; }
        }
        /// <summary>
        /// Gets the error message, if any, associated with the parsing of the Expression string.
        /// </summary>
        [XmlIgnore]
        public string ErrorString
        {
            get
            {
                if (error != ExpressionError.ExceptionRaised)
                {
                    if (error != ExpressionError.None)
                    {
                        return error.ToString();
                    }
                    return "";
                }
                return CalulationExtensions.ErrorString;
            }
        }
        #endregion

        #region calculation access
        Delegate evaluator = null;
        internal object ComputedValue(object record)
        {
            evaluator = record.GetCompiledExpression(CaseSensitive, Expression, out error);
            if (evaluator != null)
                return evaluator.DynamicInvoke(record);
            return null;
        }
        #endregion
    }
}