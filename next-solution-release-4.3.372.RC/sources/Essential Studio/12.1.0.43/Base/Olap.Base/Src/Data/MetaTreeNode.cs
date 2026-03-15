//-------------------------------------------------------------------------------------------------
// <copyright file="MetaTreeNode.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;


#if !SILVERLIGHT
using Syncfusion.Olap.Reports;
using Syncfusion.Linq;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// This is just a wrapper class on top of OLAP object model, This class contain 
    /// information about the Cube, Measure, Dimension, Hierarchies , Level and Members 
    /// in an Hierarchical format.
    /// </summary>
    /// <remarks>
    /// Each node in the meta tree will contain a reference to the unwrapped object model of its parent, in the 
    /// property collection member.
    /// </remarks>
    [Serializable]
    public class MetaTreeNode : ICloneable, INotifyPropertyChanged
    {
#else
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Manager;
using Syncfusion.Linq;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// This is just a wrapper class on top of OLAP object model, This class contain 
    /// information about the Cube, Measure, Dimension, Hierarchies , Level and Members 
    /// in an Hierarchical format.
    /// </summary>
    /// <remarks>
    /// Each node in the meta tree will contain a reference to the unwrapped object model of its parent, in the 
    /// property collection member.
    /// </remarks>
    [DataContract]
    public class MetaTreeNode : INotifyPropertyChanged 
    {
#endif
        #region Private Variables

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        string _Caption;
       
        /// <summary>
        /// Gets or sets Node checked type.
        /// </summary>
        /// <value>MetaTreeNodeCheckedType .</value>
        private MetaTreeNodeCheckedType _NodeCheckedType;

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        PropertyCollection _Properties;

        bool? _IsSelected;
        private int checkedState;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaTreeNode"/> class.
        /// </summary>
        public MetaTreeNode()
        {
            this.Name = string.Empty;
            this._Caption = string.Empty;
            this.Description = string.Empty;
            this.UniqueName = string.Empty;
            this.ChildNodes = new MetaTreeNodeCollection(this);
            this.Properties = new PropertyCollection();
            //// Node selection and check state maintenance
            this.IsSelected = true;
            this.AcceptIsSelectedChanges(false);

            this.LevelDepth = 0;
            this.DisableUpdateReport = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaTreeNode"/> class.
        /// </summary>
        /// <param name="name">The name of the MetaTree node.</param>
        /// <param name="caption">The caption of the MetaTree node</param>
        /// <param name="description">The description of the MetaTree node</param>
        public MetaTreeNode(string name, string caption, string description)
        {
            this.Name = name;
            this._Caption = caption;
            this.Description = description;
            this.UniqueName = string.Empty;
            this.ChildNodes = new MetaTreeNodeCollection(this);
            this.Properties = new PropertyCollection();

            //// Node selection and check state maintenance
            this.IsSelected = true;
            this.AcceptIsSelectedChanges(false);
            this.LevelDepth = 0;
            this.DisableUpdateReport = false;
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the state of the checked.
        /// </summary>
        /// <value>The state of the checked.</value>
        public int CheckedState
        {
            get
            {
                return checkedState;
            }

            set
            {
                checkedState = value;
            }
        }

        /// <summary>
        /// Gets or sets the is selected.
        /// </summary>
        /// <value>The is selected.</value>
        public bool? IsSelected
        {
            get
            {
                return _IsSelected;
            }

            set
            {
                this.SetIsChecked(value, true, true);
            }
        }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get
            {
                if (_Caption == string.Empty || _Caption == null)
                {
                    return "(Blank)";
                }

                return _Caption;
            }

            set
            {
                _Caption = value;
            }
        }

        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>The child nodes.</value>
        public MetaTreeNodeCollection ChildNodes { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has valid children.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has valid children; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidChildren
        {
            get
            {
                if (this.ChildNodes.Count > 0)
                {
                    if ((this.ChildNodes[0].Name.Length > 0 || this.ChildNodes[0].Caption.Length > 0) && this.ChildNodes[0].NodeType != MetaTreeNodeType.None)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether this instance has child members.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has child members; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildMembers
        {
            get;
            set;
        }
#endif

        ///// <summary>
        ///// Gets or sets the is selected.
        ///// </summary>
        ///// <value>The is selected.</value>
        //public bool? IsSelected
        //{
        //    get
        //    {
        //        return _IsSelected;
        //    }

        //    set
        //    {
        //        _IsSelected = value;
        //        this.OnPropertyChanged("IsSelected");
        //        ////SetIsChecked(value, true, true);
        //    }
        //}

        /// <summary>
        /// Gets or sets the level depth.
        /// </summary>
        /// <value>The level depth.</value>
        public int LevelDepth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable update report].
        /// </summary>
        /// <value><c>true</c> if [disable update report]; otherwise, <c>false</c>.</value>
        public bool DisableUpdateReport { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The MetaTree node name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the node checked.
        /// </summary>
        /// <value>The type of the node checked.</value>
        public MetaTreeNodeCheckedType NodeCheckedType
        {
            get
            {
                return _NodeCheckedType;
            }

            set
            {
                _NodeCheckedType = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public MetaTreeNodeType NodeType { get; set; }

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public MetaTreeNode ParentNode { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public PropertyCollection Properties
        {
            get
            {
                if (_Properties == null)
                {
                    this._Properties = new PropertyCollection();
                }

                return _Properties;
            }

            set
            {
                _Properties = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name.</value>
        public string UniqueName { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Accepts the is selected changes.
        /// </summary>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        public void AcceptIsSelectedChanges(bool updateParent)
        {
            if (updateParent)
            {
                MetaTreeNode rootNode = GetRootNode();
                rootNode.AcceptIsSelectedChanges(false);
            }
            else
            {
                if (this.IsSelected == null)
                {
                    this.CheckedState = 1;
                    this.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                }
                else if (this.IsSelected == true)
                {
                    this.CheckedState = 2;
                    this.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                }
                else
                {
                    this.CheckedState = 3;
                    this.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
                }

                if (this.ChildNodes.Count > 0)
                {
                    this.ChildNodes.AcceptIsSelectedChanges(updateParent);
                }
            }

        }

        /// <summary>
        /// Reverts the is selected changed.
        /// </summary>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        public void RevertIsSelectedChanged(bool updateParent)
        {
            if (updateParent)
            {
                MetaTreeNode rootNode = GetRootNode();
                rootNode.RevertIsSelectedChanged(false);
            }
            else
            {
                if (CheckedState == 1)
                {
                    this._IsSelected = null;
                }
                else if (CheckedState == 2)
                {
                    this._IsSelected = true;
                }
                else if (CheckedState == 3)
                {
                    this._IsSelected = false;
                }
                if (this.ChildNodes.Count > 0)
                {
                    this.ChildNodes.RevertIsSelectedChanged(updateParent);
                }
            }
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <remarks>
        /// ParentNode property will not be cloned, because cloning of current object cannot hold the
        /// reference to the parent object
        /// </remarks>
        public object Clone()
        {
            MetaTreeNode metaTreeCloneObj = new MetaTreeNode();
            metaTreeCloneObj.Name = this.Name;
            metaTreeCloneObj.Caption = this.Caption;
            metaTreeCloneObj.Description = this.Description;
            metaTreeCloneObj.UniqueName = this.UniqueName;
            metaTreeCloneObj.NodeType = this.NodeType;

            metaTreeCloneObj._IsSelected = this.IsSelected;
            metaTreeCloneObj.CheckedState = this.CheckedState;
            metaTreeCloneObj._NodeCheckedType = this.NodeCheckedType;


            /*
             * Parent Property will not be cloned
             */

            //// Cloning child nodes
            foreach (MetaTreeNode metaTreeNode in this.ChildNodes)
            {
                MetaTreeNode metaTreeChildClone = (MetaTreeNode)metaTreeNode.Clone();
                metaTreeChildClone.ParentNode = metaTreeCloneObj;
                metaTreeCloneObj.ChildNodes.Add(metaTreeChildClone);
            }

            //// Cloning Properties
            foreach (Property property in this.Properties)
            {
#if !SILVERLIGHT
                metaTreeCloneObj.Properties.Add(property.Clone());
#else
                metaTreeCloneObj.Properties.Add(property);
#endif
            }

            return metaTreeCloneObj;
        }



        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <returns></returns>
        public MetaTreeNode GetRootNode()
        {
            if (this.ParentNode != null)
            {
                return this.ParentNode.GetRootNode();
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Sets the is Selected property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _IsSelected)
                return;
            _IsSelected = value;
            if (updateChildren && _IsSelected.HasValue)
            {
                if (this.ChildNodes != null)
                    this.ChildNodes.ForEach(i => i.SetIsChecked(_IsSelected, true, false));
            }

            if (updateParent && this.ParentNode != null)
            {
                ParentNode.VerifyCheckState();
            }

            this.OnPropertyChanged("IsSelected");
        }

        /// <summary>
        /// Verifies the state of the check.
        /// </summary>
        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.ChildNodes.Count; ++i)
            {
                bool? current = this.ChildNodes[i].IsSelected;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            this.SetIsChecked(state, false, true);
        }


        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if (this.Caption.Length > 0)
            {
                return this.Caption;
            }

            return this.Name;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="prop">The property name</param>
        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        void UpdateReport()
        {
            Items measureItems = null;
            //measureItems = GetMeasuresAxis();
            if (this.NodeType == MetaTreeNodeType.KPI_Value ||
                this.NodeType == MetaTreeNodeType.KPI_Goal ||
                this.NodeType == MetaTreeNodeType.KPI_Status ||
                this.NodeType == MetaTreeNodeType.KPI_Trend)
            {
                UpdateKPIElements(measureItems);
            }
            else if (this.NodeType == MetaTreeNodeType.Measure)
            {
                UpdateMeasureElements(measureItems);
            }
            //OlapDataManager.NotifyReportChanged(OlapDataManager.CurrentReport);
        }

        private void UpdateMeasureElements(Items measureItems)
        {
            if (measureItems != null)
            {
                MeasureElement measureElement = new MeasureElement();
                measureElement.UniqueName = this.UniqueName;
                measureElement.Properties.Add(new Property(PropertyConstants.MeasrueNodeName, this));
                MeasureElements availableMeasures = (MeasureElements)GetMeasureElement(measureItems);
                if (availableMeasures != null)
                {
                    if (CheckedState == 2)
                    {
                        if (!FindMeasureElement(availableMeasures, this.UniqueName))
                        {
                            availableMeasures.Elements.Add(measureElement);
                            //OlapDataManager.NotifyElementModified();
                        }
                    }
                    else
                    {
                        int index = FindIndexOfMeasureElement(availableMeasures, this.UniqueName);
                        if (index >= 0)
                        {
                            availableMeasures.Elements.RemoveAt(index);
                            if (availableMeasures.Elements.Count == 0)
                            {
                                Item measureItem = FindMeasureItem(measureItems);
                                if (measureItem != null)
                                {
                                    measureItems.Remove(measureItem);
                                }
                            }
                            //OlapDataManager.NotifyElementModified();
                        }
                    }
                }
                else
                {
                    AddMeasureToDefaultAxis();
                }
            }
            else
            {
                AddMeasureToDefaultAxis();
                //OlapDataManager.NotifyElementModified(AxisPosition.Categorical);
            }
        }

        private void UpdateKPIElements(Items measureItems)
        {
            bool showKPIGoal = false, showKPIValue = false, showKPIStatus = false, showKPITrend = false;
            switch (this.NodeType)
            {
                case MetaTreeNodeType.KPI_Goal:
                    {
                        showKPIGoal = true;
                        break;
                    }

                case MetaTreeNodeType.KPI_Value:
                    {
                        showKPIValue = true;
                        break;
                    }

                case MetaTreeNodeType.KPI_Status:
                    {
                        showKPIStatus = true;
                        break;
                    }

                case MetaTreeNodeType.KPI_Trend:
                    {
                        showKPITrend = true;
                        break;
                    }
            }

            if (measureItems != null)
            {
                KpiElement kpiElement = new KpiElement();
                kpiElement.Name = this.ParentNode.UniqueName;
                KpiElements availableKpis = (KpiElements)GetKPIElement(measureItems);
                kpiElement.ShowKPIGoal = showKPIGoal;
                kpiElement.ShowKPIStatus = showKPIStatus;
                kpiElement.ShowKPITrend = showKPITrend;
                kpiElement.ShowKPIValue = showKPIValue;
                if (availableKpis != null)
                {
                    if (CheckedState == 2)
                    {
                        if (!FindKpiElement(availableKpis, this.ParentNode.UniqueName))
                        {
                            availableKpis.Elements.Add(kpiElement);
                            //OlapDataManager.NotifyElementModified();
                        }
                        else
                        {
                            KpiElement element = (KpiElement)FindKPIElement(measureItems, kpiElement);
                            if (element != null)
                            {
                                if (this.NodeType == MetaTreeNodeType.KPI_Goal)
                                {
                                    element.ShowKPIGoal = showKPIGoal;
                                }
                                else if (this.NodeType == MetaTreeNodeType.KPI_Status)
                                {
                                    element.ShowKPIStatus = showKPIStatus;
                                }
                                else if (this.NodeType == MetaTreeNodeType.KPI_Trend)
                                {
                                    element.ShowKPITrend = showKPITrend;
                                }
                                else if (this.NodeType == MetaTreeNodeType.KPI_Value)
                                {
                                    element.ShowKPIValue = showKPIValue;
                                }
                            }
                            //element = kpiElement;
                        }
                    }
                    else
                    {
                        KpiElement element = (KpiElement)FindKPIElement(measureItems, kpiElement);
                        if (element != null)
                        {
                            if (this.NodeType == MetaTreeNodeType.KPI_Goal)
                            {
                                element.ShowKPIGoal = !showKPIGoal;
                            }
                            else if (this.NodeType == MetaTreeNodeType.KPI_Status)
                            {
                                element.ShowKPIStatus = !showKPIStatus;
                            }
                            else if (this.NodeType == MetaTreeNodeType.KPI_Trend)
                            {
                                element.ShowKPITrend = !showKPITrend;
                            }
                            else if (this.NodeType == MetaTreeNodeType.KPI_Value)
                            {
                                element.ShowKPIValue = !showKPIValue;
                            }

                            if (!element.ShowKPIGoal && !element.ShowKPIStatus && !element.ShowKPITrend && !element.ShowKPIValue)
                            {
                                if (element != null)
                                {
                                    availableKpis.Elements.Remove(element);
                                    if (availableKpis.Elements.Count == 0)
                                    {
                                        Item kpiItem = FindKPIItem(measureItems);
                                        if (kpiItem != null)
                                        {
                                            measureItems.Remove(kpiItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    AddKpiToDefaultAxis(showKPIGoal, showKPIValue, showKPIStatus, showKPITrend);
                }
            }
            else
            {
                AddKpiToDefaultAxis(showKPIGoal, showKPIValue, showKPIStatus, showKPITrend);
                //OlapDataManager.NotifyElementModified(AxisPosition.Categorical);
            }
        }

        private void AddMeasureToDefaultAxis()
        {
            MeasureElement measureElement = new MeasureElement();
            measureElement.UniqueName = this.UniqueName;
            measureElement.Properties.Add(new Property(PropertyConstants.MeasrueNodeName, this));
            MeasureElements availableMeasures = new MeasureElements();
            availableMeasures.Elements.Add(measureElement);
            //this.OlapDataManager.CurrentReport.CategoricalElements.Add(new Item { ElementValue = availableMeasures });
        }

        private void AddKpiToDefaultAxis(bool showKPIGoal, bool showKPIValue, bool showKPIStatus, bool showKPITrend)
        {
            KpiElement kpiElement = new KpiElement();
            kpiElement.Name = this.ParentNode.UniqueName;
            kpiElement.ShowKPIGoal = showKPIGoal;
            kpiElement.ShowKPIStatus = showKPIStatus;
            kpiElement.ShowKPITrend = showKPITrend;
            kpiElement.ShowKPIValue = showKPIValue;
            kpiElement.Properties.Add(new Property(PropertyConstants.KPI, this.ParentNode));
            KpiElements availableKpis = new KpiElements();
            availableKpis.Elements.Add(kpiElement);
            //this.OlapDataManager.CurrentReport.CategoricalElements.Add(new Item { ElementValue = availableKpis });
        }

        //private Items GetMeasuresAxis()
        //{
        //    Items tempItems = null;
        //    if (GetMeasureElement(this.OlapDataManager.CurrentReport.CategoricalElements) is MeasureElements ||
        //        GetKPIElement(this.OlapDataManager.CurrentReport.CategoricalElements) is KpiElements)
        //    {
        //        tempItems = this.OlapDataManager.CurrentReport.CategoricalElements;
        //    }
        //    else if (GetMeasureElement(this.OlapDataManager.CurrentReport.SeriesElements) is MeasureElements ||
        //            GetKPIElement(this.OlapDataManager.CurrentReport.CategoricalElements) is KpiElements)
        //    {
        //        tempItems = this.OlapDataManager.CurrentReport.SeriesElements;
        //    }
        //    else if (GetMeasureElement(this.OlapDataManager.CurrentReport.SlicerElements) is MeasureElements ||
        //        GetKPIElement(this.OlapDataManager.CurrentReport.CategoricalElements) is KpiElements)
        //    {
        //        tempItems = this.OlapDataManager.CurrentReport.SlicerElements;
        //    }

        //    return tempItems;
        //}

        private Element GetKPIElement(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is KpiElements)
                {
                    return item.ElementValue;
                }
            }

            return null;
        }

        private Element GetMeasureElement(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is MeasureElements)
                {
                    return item.ElementValue;
                }
            }

            return null;
        }

        private Element FindKPIElement(Items items, KpiElement kpiElement)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is KpiElements)
                {
                    KpiElements kpiElements = (KpiElements)item.ElementValue;
                    foreach (KpiElement element in kpiElements.Elements)
                    {
                        if (element.Name == kpiElement.Name)
                        {
                            return element;
                        }
                    }
                }
            }

            return null;
        }

        private Item FindKPIItem(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is KpiElements)
                {
                    return item;
                }
            }

            return null;
        }

        private Item FindMeasureItem(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is MeasureElements)
                {
                    return item;
                }
            }

            return null;
        }

        private bool FindKpiElement(Element element, string uniqueName)
        {
            if (element is KpiElements)
            {
                KpiElements kpiElements = (KpiElements)element;
                foreach (KpiElement kpiElement in kpiElements.Elements)
                {
                    if (kpiElement.Name.ToLower() == uniqueName.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool FindMeasureElement(Element element, string uniqueName)
        {
            if (element is MeasureElements)
            {
                MeasureElements measureElements = (MeasureElements)element;
                foreach (MeasureElement measureElement in measureElements.Elements)
                {
                    if (measureElement.UniqueName.ToLower() == uniqueName.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int FindIndexOfMeasureElement(Element element, string uniqueName)
        {
            if (element is MeasureElements)
            {
                MeasureElements measureElements = (MeasureElements)element;
                for (int i = 0; i < measureElements.Elements.Count; i++)
                {
                    MeasureElement measureElement = measureElements.Elements[i];
                    if (measureElement.UniqueName.ToLower() == uniqueName.ToLower())
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        #endregion
    }
}
