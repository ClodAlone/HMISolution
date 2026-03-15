#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
using System.ComponentModel;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.Silverlight.Client.Olap.Resources;
using System.Globalization;

namespace Syncfusion.Silverlight.Tools.Olap
{
    /// <summary>
    /// Helper class for calculated member editor.
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class CalcMemberEditor : Syncfusion.Windows.Tools.Controls.WindowControl
    {

        #region Private Members
        private int _flag;
        #endregion

        #region Properties

        private DragDropManager DragDropManager
        {
            get
            {
                if (this.CalcMeasureTreeView.OlapDataManager != null)
                    return this.CalcMeasureTreeView.OlapDataManager.DragDropManager;
                else
                    return null;
            }
            set
            {
                if (this.CalcMeasureTreeView.OlapDataManager != null)
                {
                    this.CalcMeasureTreeView.OlapDataManager.DragDropManager = value;
                }
            }
        }

        internal string CurrentCubeName { get; set; }

        internal bool AutoExecute { get; set; }

        internal SplitButton Host { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcMemberEditor"/> class.
        /// </summary>
        public CalcMemberEditor()
        {
            InitializeComponent();
            this.Title = Syncfusion.Silverlight.Client.Olap.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title");
            #region Wire the events

            this.Closing -= new Windows.Tools.Controls.ClosedEventHandler(CalcMeasureEditor_Closing);
            this.Closing += new Windows.Tools.Controls.ClosedEventHandler(CalcMeasureEditor_Closing);
            this.MouseLeftButtonUp -= new MouseButtonEventHandler(CalcMeasureEditor_MouseLeftButtonUp);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(CalcMeasureEditor_MouseLeftButtonUp);
            this.ExpressionText.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.ExpressionText.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.CalcMeasureTreeView.MouseMove -= new MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.CalcMeasureTreeView.MouseMove += new MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.CalcMeasureTreeView.NodeClicked -= new NodeClickedHandler(CalcMeasureTreeView_NodeClicked);
            this.CalcMeasureTreeView.NodeClicked += new NodeClickedHandler(CalcMeasureTreeView_NodeClicked);
            
            #endregion

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcMemberEditor"/> class.
        /// </summary>
        /// <param name="host">The host as <see cref="SplitButton"/>.</param>        
        public CalcMemberEditor(SplitButton host)
        {
            InitializeComponent();
            this.Title = Syncfusion.Silverlight.Client.Olap.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title");
            this.Host = host;
            if (this.Host != null && this.Host.MetaTreeNode != null)
            {
                Property prop = this.Host.MetaTreeNode.Properties.FindByName(PropertyConstants.CalculatedMemberNodeName);
                if (prop != null && prop.Value is CalculatedMember)
                {
                    CalculatedMember calcMember = prop.Value as CalculatedMember;
                    this.CaptionText.Text = calcMember.Name ?? string.Empty;
                    this.ExpressionText.Text = calcMember.Expression ?? string.Empty;
                    this.MemberTypeBox.SelectedIndex = (calcMember.Type == TypeOfMember.Measure) ? 0 : 1;
                    if (calcMember.FormatString != null)
                    {
                        if (calcMember.FormatString.Equals("Standard", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.FormatBox.SelectedIndex = 0;
                        }
                        else if (calcMember.FormatString.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.FormatBox.SelectedIndex = 1;
                        }
                        else if (calcMember.FormatString.Equals("Percent", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.FormatBox.SelectedIndex = 2;
                        }
                        else
                        {
                            this.FormatText.Text = calcMember.FormatString;
                            this.FormatBox.SelectedIndex = 3;
                        }
                    }
                }
            }

            #region Wire the events

            this.Closing -= new Windows.Tools.Controls.ClosedEventHandler(CalcMeasureEditor_Closing);
            this.Closing += new Windows.Tools.Controls.ClosedEventHandler(CalcMeasureEditor_Closing);
            this.MouseLeftButtonUp -= new MouseButtonEventHandler(CalcMeasureEditor_MouseLeftButtonUp);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(CalcMeasureEditor_MouseLeftButtonUp);
            this.ExpressionText.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.ExpressionText.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.CalcMeasureTreeView.MouseMove -= new MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.CalcMeasureTreeView.MouseMove += new MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.CalcMeasureTreeView.NodeClicked -= new NodeClickedHandler(CalcMeasureTreeView_NodeClicked);
            this.CalcMeasureTreeView.NodeClicked += new NodeClickedHandler(CalcMeasureTreeView_NodeClicked);

            #endregion

        }
        #endregion

        #region Actions of wired events

        private void CalcMeasureTreeView_NodeClicked(object sender, NodeClickedEventArgs e)
        {
            if ((sender as CDTreeViewItem) != null)
            {
                _flag = 1;
            }
        }

        private void CalcMeasureTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_flag == 1)
            {
                _flag = 2;
                SetVisibility(Visibility.Visible);
            }
        }

        private void ExpressionText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DragDropManager != null)
            {
                this.ExpressionText.Text += this.DragDropManager.SelectedNode.UniqueName;
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }

            if (_flag == 2)
            {
                _flag = 0;
                SetVisibility(Visibility.Collapsed);
            }
        }

        private void CalcMeasureEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _flag = 0;
            SetVisibility(Visibility.Collapsed);
        }

        private void SetVisibility(System.Windows.Visibility visibility)
        {
            this.CalcMeasureTreeViewBorder.Visibility = visibility;
            this.CaptionTextBorder.Visibility = visibility;
            this.MemberTypeBorder.Visibility = visibility;
            if (this.MemberTypeText.Visibility == System.Windows.Visibility.Visible)
                this.MemberTypeTextBorder.Visibility = visibility;
            this.FormatBoxBorder.Visibility = visibility;
            if (this.FormatText.Visibility == System.Windows.Visibility.Visible)
                this.FormatTextBorder.Visibility = visibility;
        }

        /// <summary>
        /// Handles the Click event of the OK Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            bool canPass = true;
            try
            {
                if (this.Host != null && this.Host.Parent != null)
                {
                    ////Code here for modifying the existing calculated member at the specified axis.
                    if (!string.IsNullOrEmpty(this.CaptionText.Text))                    
                    {
                        var calcMemberItem = this.CalcMeasureTreeView.OlapDataManager.CurrentReport.CalculatedMembers.List.Select(i => i).Where(j => (j.ElementValue is CalculatedMember) && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                        if (calcMemberItem != null)
                        {
                            CalculatedMember calcMember = calcMemberItem.ElementValue as CalculatedMember;
                            if (!string.IsNullOrEmpty(this.ExpressionText.Text.Trim()))
                            {
                                calcMember.Name = this.CaptionText.Text.Trim();
                                calcMember.Expression = this.ExpressionText.Text.Trim();
                            }
                            else
                            {
                                ////Show aleart message as Expression text should be required for each calculated member.
                                canPass = false;
                                Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Exception_ExpressionTextShouldBeNonEmpty"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title"), Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                            }

                            calcMember.FormatString = this.FormatBox.SelectedIndex == 3 ? this.FormatText.Text.Trim() : (this.FormatBox.SelectedItem as ComboBoxItem).Content.ToString();
                            if (this.MemberTypeBox.SelectedIndex == 0)
                            {
                                MeasureElement measureElement = new MeasureElement { Name = "CalculatedMeasure" };
                                calcMember.AddElement(measureElement);
                            }
                            else if (this.MemberTypeText.SelectedItem != null && this.MemberTypeBox.SelectedItem.ToString() != string.Empty)
                            {
                                DimensionElement dimensionElement = new DimensionElement { Name = this.MemberTypeText.SelectedItem.ToString() };
                                dimensionElement.AddLevel(this.MemberTypeText.SelectedItem.ToString(), this.MemberTypeText.SelectedItem.ToString());
                                calcMember.AddElement(dimensionElement);
                            }
                            else
                            {
                                ////Show alert message as Parent dimension name should be specified for calculated member.
                                canPass = false;
                                Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture,"Exception_PleaseSelectTheParentDimension"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title"), Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                            }

                            if (this.Host.Parent.Axis == AxisPosition.Categorical)
                            {
                                var reportItem = this.CalcMeasureTreeView.OlapDataManager.CurrentReport.CategoricalElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                                if (reportItem != null)
                                {
                                    reportItem.ElementValue = calcMember;
                                }
                            }
                            else if (this.Host.Parent.Axis == AxisPosition.Series)
                            {
                                var reportItem = this.CalcMeasureTreeView.OlapDataManager.CurrentReport.SeriesElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                                if (reportItem != null)
                                {
                                    reportItem.ElementValue = calcMember;
                                }
                            }
                            else if (this.Host.Parent.Axis == AxisPosition.Slicer)
                            {
                                var reportItem = this.CalcMeasureTreeView.OlapDataManager.CurrentReport.SlicerElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                                if (reportItem != null)
                                {
                                    reportItem.ElementValue = calcMember;
                                }
                            }

                            this.Host.MetaTreeNode.Caption = this.Host.MetaTreeNode.Description = this.Host.MetaTreeNode.Name = calcMember.Name;
                            this.Host.MetaTreeNode.UniqueName = calcMember.UniqueName;
                            this.Host.MetaTreeNode.Properties.Clear();
                            this.Host.MetaTreeNode.Properties.Add(new Property(PropertyConstants.CalculatedMemberNodeName, calcMember));
                        }

                        if (canPass)
                        {
                            if (this.AutoExecute)
                                this.CalcMeasureTreeView.OlapDataManager.NotifyElementChanged(this.Host.Parent.Axis);
                            else
                                this.CalcMeasureTreeView.OlapDataManager.RefreshAxisElementBuilder();
                        }
                    }
                    else
                    {
                        ////Show alert message as Caption text should be specified.
                        canPass = false;
                        Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture,"Exception_CaptionTextShouldBeNonEmpty"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title"), Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                    }
                }
                else
                {
                    ////Code here for creating the new calculated members add these items into Categorical axis by default.
                    if (!string.IsNullOrEmpty(this.CaptionText.Text) && !this.CalcMeasureTreeView.OlapDataManager.CurrentReport.CalculatedMembers.List.Any(i => i.ElementValue.Name == this.CaptionText.Text.Trim()))
                    {
                        ////Add the calculated member into report for specified information.
                        CalculatedMember calcMember = new CalculatedMember();
                        if (!string.IsNullOrEmpty(this.ExpressionText.Text.Trim()))
                        {
                            calcMember.Name = this.CaptionText.Text.Trim();
                            calcMember.Expression = this.ExpressionText.Text.Trim();
                        }
                        else
                        {
                            ////Show aleart message as Expression text should be required for each calculated member.
                            canPass = false;
                            Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Exception_ExpressionTextShouldBeNonEmpty"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title"), Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                        }

                        if (this.FormatBox.SelectedIndex > 0)
                        {
                            calcMember.FormatString = this.FormatBox.SelectedIndex == 3 ? this.FormatText.Text.Trim() : (this.FormatBox.SelectedItem as ComboBoxItem).Content.ToString();
                        }
                        if (this.MemberTypeBox.SelectedIndex == 0)
                        {
                            MeasureElement measureElement = new MeasureElement { Name = "CalculatedMeasure" };
                            calcMember.AddElement(measureElement);
                        }
                        else if (this.MemberTypeText.SelectedItem != null && this.MemberTypeBox.SelectedItem.ToString() != string.Empty)
                        {
                            DimensionElement dimensionElement = new DimensionElement { Name = this.MemberTypeText.SelectedItem.ToString() };
                            dimensionElement.AddLevel(this.MemberTypeText.SelectedItem.ToString(), this.MemberTypeText.SelectedItem.ToString());
                            calcMember.AddElement(dimensionElement);
                        }
                        else
                        {
                            ////Show alert message as Parent dimension name should be specified for calculated member.
                            canPass = false;
                            Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Exception_PleaseSelectTheParentDimension"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title"), Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                        }

                        if (canPass)
                        {
                            this.CalcMeasureTreeView.OlapDataManager.CurrentReport.CalculatedMembers.Add(calcMember);
                            if (this.CalcMeasureTreeView.OlapDataManager.CurrentReport.SeriesElements.List.Any(i => i.ElementValue is MeasureElements))
                            {
                                this.CalcMeasureTreeView.OlapDataManager.CurrentReport.SeriesElements.Add(calcMember);
                            }
                            else if (this.CalcMeasureTreeView.OlapDataManager.CurrentReport.SlicerElements.List.Any(i => i.ElementValue is MeasureElements))
                            {
                                this.CalcMeasureTreeView.OlapDataManager.CurrentReport.SlicerElements.Add(calcMember);
                            }
                            else
                            {
                                this.CalcMeasureTreeView.OlapDataManager.CurrentReport.CategoricalElements.Add(calcMember);
                            }

                            if (this.AutoExecute)
                                this.CalcMeasureTreeView.OlapDataManager.NotifyElementChanged();
                            else
                                this.CalcMeasureTreeView.OlapDataManager.RefreshAxisElementBuilder();
                        }
                    }
                    else
                    {
                        ////Show alert window with message as either caption text is empty or already exist.
                        canPass = false;
                        Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Exception_EitherTheCaptionTextIsEmptyOrAlreadyExist"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMemberEditor_Title"), Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);

                    }
                }

                if (canPass)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(ex.Message,SR.GetString(CultureInfo.CurrentUICulture, "Exception_UpdateError"),Syncfusion.Windows.Tools.Controls.DialogIcon.Error, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the Click event of the Cancel Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FormatBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FormatText != null)
            {
                if (this.FormatBox.SelectedIndex == 3)
                {
                    this.FormatText.Visibility = Visibility.Visible;
                    this.FormatText.Focus();
                }
                else
                {
                    this.FormatText.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void MemberTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MemberTypeText != null)
            {
                if (this.MemberTypeBox.SelectedIndex == 1)
                {
                    this.MemberTypeText.Visibility = Visibility.Visible;
                    this.MemberTypeText.Focus();
                }
                else
                {
                    this.MemberTypeText.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        void CalcMeasureEditor_Closing(object sender, Windows.Tools.Controls.ClosedEventArgs e)
        {
            if (this.DragDropManager != null)
            {
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
        }
        #endregion

        #region DragDrop Handler

        /// <summary>
        /// Mouse move event handler for drag and drop support
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.DragDropManager != null)
            {
                Point point = e.GetPosition(null);
                this.DragDropManager.DragDropPopup.Child.Visibility = System.Windows.Visibility.Visible;
                this.DragDropManager.DragDropPopup.HorizontalOffset = point.X - (this.DragDropManager.DragDropPopup.ActualWidth + 5);
                this.DragDropManager.DragDropPopup.VerticalOffset = point.Y + 15;
            }
        }

        /// <summary>
        /// Mouse left button up event handler for drag and drop support
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.DragDropManager != null)
            {
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
        }


        #endregion
    }
}

