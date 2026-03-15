#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


namespace Syncfusion.Windows.Chart.Olap
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using Syncfusion.Olap.Data;
    using Syncfusion.Olap.Engine;
    /// <summary>
    /// Representing OlapLabelPresenter
    /// </summary>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapLabelPresenter : ContentControl
    {
        private LabelExpanderState m_labelExpanderState;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapLabelPresenter"/> class.
        /// </summary>
        /// <param name="representedMember">The represented member.</param>
        public OlapLabelPresenter(PivotCellDescriptor representedMember)
        {
            Content = representedMember;
            Member member = representedMember.Tag as Member;
            if (member == null)
            {
                this.m_labelExpanderState = LabelExpanderState.Hidden;
            }
            else
                this.m_labelExpanderState = member.HasChildMembers ?
                  member.DrilledDown ?
                  (LabelExpanderState.Expanded) : (LabelExpanderState.Collapsed) :
                  LabelExpanderState.Hidden;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the state of the expander.
        /// </summary>
        /// <value>The state of the expander.</value>
        public LabelExpanderState ExpanderState
        {
            get
            {
                return m_labelExpanderState;
            }
        }

        #endregion
    }
}
