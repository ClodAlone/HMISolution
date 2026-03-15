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

#if WINRT
namespace Syncfusion.UI.Xaml.Reports
#elif MVC
namespace Syncfusion.Reports.Mvc
#else
namespace Syncfusion.Windows.Reports
#endif
{
    #region Report Parameter Definition
    public sealed class ReportParameterInfoCollection : List<ReportParameterInfo>
    {
        public ReportParameterInfo this[string name]
        {
            get
            {
                if (this != null)
                {
                    var parameters = from param in this
                                     where param.Name.Equals(name)
                                     select param;

                    if (parameters.Count() > 0)
                    {
                        return parameters.First();
                    }
                }

                return null;
            }
        }
    }

    public sealed class ReportParameterInfo
    {
        public ReportParameterInfo()
        {
            this.Labels = new List<string>();
            this.Values = new List<string>();
            this.ValidValues = new List<ValidValue>();
        }
        public string Name { get; set; }
        public ParamType DataType { get; set; }
        public bool AllowBlank { get; set; }
        public bool MultiValue { get; set; }
        public IList<ValidValue> ValidValues { get; set; }
        public IList<string> Values { get; set; }
        public IList<string> Labels { get; set; }
        public string Prompt { get; set; }
        public bool Nullable { get; set; }
        public bool Hidden { get; set; }
    }

    public sealed class ReportParameter
    {
        public string Name { get; set; }

        public List<string> Values { get; set; }
        public List<string> Labels { get; set; }
        public bool Nullable { get; set; }
        public string Prompt { get; set; }

        public ReportParameter()
        {
            this.Values = new List<string>();
            this.Labels = new List<string>();
        }
    }

    public class ValidValue
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    #endregion

    #region enums
    public enum ParamType
    {
        Boolean,
        DateTime,
        Float,
        Integer,
        String
    }
    #endregion
}
