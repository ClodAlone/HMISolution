#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Holds the range and options for a covered cell. A covered cell is a cell that
    /// spans over neighbouring cells. All cells in this range are treated as one single cell.
    /// </summary>
    public class CoveredCellInfo : CellSpanInfo, IXmlSerializable
    {
        bool spanWholeRow; 
        bool spanWholeColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoveredCellInfo"/> class.
        /// </summary>
        public CoveredCellInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoveredCellInfo"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        /// <param name="clipRows">if set to <c>true</c> allow estimates for out of view rows.</param>
        /// <param name="clipColumns">if set to <c>true</c> allow estimates for out of view columns.</param>
        public CoveredCellInfo(int top, int left, int bottom, int right, bool clipRows, bool clipColumns)
            : base(top, left, bottom, right, clipRows, clipColumns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoveredCellInfo"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        public CoveredCellInfo(int top, int left, int bottom, int right)
            : base(top, left, bottom, right)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the covered cell spans the whole row.
        /// </summary>
        /// <value><c>true</c> if the covered cell spans the whole row; otherwise, <c>false</c>.</value>
        public bool SpanWholeRow
        {
            get { return spanWholeRow; }
            set { spanWholeRow = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the covered cell spans the whole column.
        /// </summary>
        /// <value><c>true</c> if the covered cell spans the whole column; otherwise, <c>false</c>.</value>
        public bool SpanWholeColumn
        {
            get { return spanWholeColumn; }
            set { spanWholeColumn = value; }
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = obj as CoveredCellInfo;
            if (other == null)
                return false;

            return other.spanWholeRow == spanWholeRow
                && other.spanWholeColumn == spanWholeColumn;
        }

        /// <summary>
        /// Returns a string describing the state of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ( Top = {1} Left = {2} Bottom = {3} Right = {4} SpanWholeRow = {5} SpanWholeColumn = {6} ClipRows = {7} ClipColumns = {8} )",
               GetType().Name, Top, Left, Bottom, Right, SpanWholeRow, SpanWholeColumn, ClipRows, ClipColumns);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            var text = reader.ReadString();

            var pdc = TypeDescriptor.GetProperties(this);
            text = "<Nodes>" + text + "</Nodes>";
            var doc = new XmlDocument();
            doc.LoadXml(text);

            foreach (PropertyDescriptor pd in pdc)
            {
                string nodeName = "//" + pd.Name;
                var node = doc.SelectNodes(nodeName)[0];
                if (node == null) continue;

                var val = 0;
                int.TryParse(node.InnerXml, out val);
                if (pd.Name == "Left")
                    left = val;
                else if (pd.Name  == "Right")
                    right = val;
                else if (pd.Name == "Top")
                    top = val;
                else if (pd.Name == "Bottom")
                    bottom = val;
                else
                {
                    throw new NotImplementedException("Exception while deserializing" + pd.Name);
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var pdc = TypeDescriptor.GetProperties(this);
            var text = "";
            foreach (PropertyDescriptor pd in pdc)
            {
                if (pd.Name.Equals("Left") || pd.Name.Equals("Right") || pd.Name.Equals("Top") ||
                    pd.Name.Equals("Bottom"))
                {
                    var localValue = pd.GetValue(this);
                    if (localValue != null)
                    {
                        text += "<" + pd.Name + ">";
                        text += localValue;
                        text += "</" + pd.Name + ">";
                    }
                }
            }
            writer.WriteString(text);
        }
    }


}
