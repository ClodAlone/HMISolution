#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;

using Syncfusion.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [
      Serializable,
        TypeConverter(typeof(TreeNodePrimitiveConverter))
      ]
    public class TreeNodePrimitive : ICloneable
    {
        #region members

        private int m_index;

        private PredefinedPrimitiveTypes m_type = PredefinedPrimitiveTypes.Text;
        #endregion

        #region events

        internal event SyncfusionPropertyChangedEventHandler PropertyChanged;

        internal event SyncfusionPropertyChangedEventHandler PropertyChanging;
        #endregion

        #region Properties

        public int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                if (value != m_index)
                {
                    int oldValue = m_index;
                    SyncfusionPropertyChangedEventArgs args = new SyncfusionPropertyChangedEventArgs(
                      PropertyChangeEffect.None, "Index", oldValue, value);
                    OnPropertyChanging(args);

                    m_index = value;

                    args = new SyncfusionPropertyChangedEventArgs(
                      PropertyChangeEffect.None, "Index", oldValue, value);

                    OnPropertyChanged(args);
                }
            }
        }

        [DefaultValue(PredefinedPrimitiveTypes.Text)]
        public PredefinedPrimitiveTypes PrimitiveType
        {
            get
            {
                return m_type;
            }
            set
            {
                if (value != m_type)
                {
                    PredefinedPrimitiveTypes oldValue = m_type;
                    SyncfusionPropertyChangedEventArgs args = new SyncfusionPropertyChangedEventArgs(
                      PropertyChangeEffect.None, "PrimitiveType", oldValue, value);

                    OnPropertyChanging(args);

                    m_type = value;

                    args = new SyncfusionPropertyChangedEventArgs(
                      PropertyChangeEffect.None, "PrimitiveType", oldValue, value);

                    OnPropertyChanged(args);
                }
            }
        }
        #endregion

        #region Initialization
        public TreeNodePrimitive()
        {
        }

        public TreeNodePrimitive(int index, PredefinedPrimitiveTypes primitiveType)
        {
            m_type = primitiveType;
            m_index = index;
        }
        #endregion

        #region Implementation

        protected virtual void OnPropertyChanged(SyncfusionPropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected virtual void OnPropertyChanging(SyncfusionPropertyChangedEventArgs e)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }
        #endregion

        #region ICloneable Members
 
        public object Clone()
        {
            return new TreeNodePrimitive(m_index, m_type);
        }
        #endregion
    }
}