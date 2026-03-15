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
using System.Collections;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.ItemModel;

namespace Syncfusion.RDL.Data
{
    /// <summary>
    /// IPageModeler Properties
    /// Contains various positional properties
    /// </summary>
    internal interface IReportItemModeler
    {
        event ReportItemLoadedHandler ReportItemLoaded;

        event ReportItemEvaluatedHanlder ReportItemEvaluated;

        List<DataField> DataSetFields 
        { 
            get;
            set;
        }

        List<FieldValue> FieldValues
        {
            get;
            set;
        }

        Dictionary<string, object> RowNumbers
        {
            get;
            set;
        }

        Dictionary<string, int> GroupLevels
        {
            get;
            set;
        }

        string DataSetName
        {
            get;
            set;
        }

        object DataSource
        {
            get;
            set;
        }

        ReportModel Model
        {
            get;
            set;
        }

        Guid GUID
        {
            get;
            set;
        }

        bool IsLoaded
        {
            get;
            set;
        }

        bool IsSubReportChild
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        bool CanGrow
        {
            get;
            set;
        }

        bool Hidden
        {
            get;
            set;
        }

        ModelType ModelType
        {
            get;
            set;
        }

        IReportItemModeler ContainerModel
        {
            get;
            set;
        }

        ReportModelContentCollection ReportItemModelers
        {
            get;
            set;
        }

        RDL.DOM.BreakLocation PageBreak
        {
            get;
            set;
        }

        bool KeepTogether
        {
            get;
            set;
        }

        ReportItem ReportItem
        {
            get;
            set;
        }

        double Left
        {
            get;
            set;
        }

        double Top
        {
            get;
            set;
        }

        double Width
        {
            get;
            set;
        }

        double Height
        {
            get;
            set;
        }

        List<ExpFilter> ExpFilters 
        { 
            get; 
            set;
        }

        LayoutReportItemModel PrintPageInfo
        {
            get;
            set;
        }

        LayoutReportItemModel PageInfo
        {
            get;
            set;
        }

        LayoutReportItemModel FlowLayoutInfo
        {
            get;
            set;
        }

        bool IsTablixChild
        {
            get;
            set;
        }
        
        bool IsTablixInnerChild
        {
            get;
            set;
        }

        bool HasRowNumber
        {
            get;
            set;
        }

        string ToggleItem
        {
            get; 
            set;
        }

        DocumentData DocumentNodeRefer
        {
            get;
            set;
        }

        string DocumentMapLable
        {
            get;
            set;
        }

        void Evaluate();

        void UpdateSize();

        void Load();

        void DisposeEvalObjects();

        void DisposeReportItemObj();

        void UpdatePageNo(int pageNo);

        IReportItemModeler GetModel();

        TextboxModel GetTextBoxModel(string itemName);

        void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode);

        void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode);
    }

    internal abstract class ReportItemModeler : IReportItemModeler
    {
        #region IReportItemModeler Members

        public event ReportItemLoadedHandler ReportItemLoaded;

        public event ReportItemEvaluatedHanlder ReportItemEvaluated;

        public List<DataField> DataSetFields
        {
            get;
            set;
        }

        public List<FieldValue> FieldValues
        {
            get;
            set;
        }

        public Dictionary<string, object> RowNumbers
        {
            get;
            set;
        }

        public Dictionary<string, int> GroupLevels
        {
            get;
            set;
        }

        public string DataSetName
        {
            get;
            set;
        }

        public object DataSource
        {
            get;
            set;
        }

        public ReportModel Model
        {
            get;
            set;
        }

        public Guid GUID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool CanGrow
        {
            get;
            set;
        }

        public bool IsLoaded
        {
            get;
            set;
        }

        public bool IsSubReportChild
        {
            get;
            set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        public String WritingMode
        {
            get;
            set;
        }


        public ModelType ModelType
        {
            get;
            set;
        }

        public IReportItemModeler ContainerModel
        {
            get;
            set;
        }

        public ReportModelContentCollection ReportItemModelers
        {
            get;
            set;
        }

        public RDL.DOM.BreakLocation PageBreak
        {
            get;
            set;
        }

        public bool KeepTogether
        {
            get;
            set;
        }

        public ReportItem ReportItem
        {
            get;
            set;
        }

        public double Left
        {
            get;
            set;
        }

        public double Top
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        public List<ExpFilter> ExpFilters
        {
            get;
            set;
        }

        public LayoutReportItemModel PrintPageInfo
        {
            get;
            set;
        }

        public LayoutReportItemModel PageInfo
        {
            get;
            set;
        }

        public LayoutReportItemModel FlowLayoutInfo
        {
            get;
            set;
        }

        public bool IsTablixChild
        {
            get;
            set;
        }

        public bool IsTablixInnerChild
        {
            get;
            set;
        }

        public bool HasRowNumber
        {
            get;
            set;
        }

        public string ToggleItem
        {
            get; 
            set;
        }

        public DocumentData DocumentNodeRefer
        {
            get;
            set;
        }

        public string DocumentMapLable
        {
            get; 
            set;
        }

        public virtual void Load()
        {
            this.RaiseReportItemLoaded(null);
        }

        public virtual void UpdateSize()
        {        
        }

        public virtual void DisposeEvalObjects()
        {
        }

        public virtual void DisposeReportItemObj()
        {
        }

        public virtual void Evaluate()
        {
            this.RaiseReportItemEvaluated(null);
        }

        public virtual IReportItemModeler GetModel()
        {
            return null;
        }

        public virtual TextboxModel GetTextBoxModel(string itemName)
        {
            try
            {
                var model = this.Model.BodyReportItemModels.Where(t => (t.Name == itemName && t.ModelType == ModelType.TextBoxModel));
                var txtmodel = model.First() as TextboxModel;
                txtmodel.AddToggleItems(this.Name);
                this.Model.isContainsToggle = true;
                return txtmodel;
            }
            catch 
            {
                return null;
            }
        }

        public virtual void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
        }

        public virtual void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
        }

        #endregion

        public void RaiseReportItemLoaded(Exception e)
        {
            if (this.ReportItemLoaded != null)
            {
                this.ReportItemLoaded(this, new ReportItemLoadedEventArgs() { Exception = e });
            }
        }

        public void RaiseReportItemEvaluated(Exception e)
        {
            if (this.ReportItemEvaluated != null)
            {
                this.ReportItemEvaluated(this, new ReportItemEvaluatedEventArgs() { Exception = e });
            }
        }


        public virtual void UpdatePageNo(int pageNo)
        {

        }
    }

    internal class ReportModelContentCollection:List<IReportItemModeler>
    {
        public event ReportItemLoadedHandler ReportItemLoaded;

        public event ReportItemEvaluatedHanlder ReportItemEvaluated;

        int itemCount = 0;
        int itemIndex = -1;

        public void Load()
        {
            itemCount = this.Count;
            itemIndex = -1;

            if (itemCount > 0)
            {
                itemIndex++;
                this.UpdateReportItemModel();
            }
            else
            {
                this.RaiseReportItemLoaded(null);
            }
        }

        public void UpdateReportItemModel()
        {
            if (!this[itemIndex].IsLoaded)
            {
                this[itemIndex].ReportItemLoaded += ReportModelContentCollection_ReportItemLoaded;
                this[itemIndex].Load();
            }
            else
            {
                itemIndex++;

                if (itemIndex == itemCount)
                {
                    this.RaiseReportItemLoaded(null);
                }
                else
                {
                    UpdateReportItemModel();
                }
            }
        }

        void ReportModelContentCollection_ReportItemLoaded(object sender, ReportItemLoadedEventArgs e)
        {
            this[itemIndex].ReportItemLoaded -= ReportModelContentCollection_ReportItemLoaded;
            var item = this[itemIndex];

            if (item.ModelType == ModelType.SubReportModel)
            {   
                if (item.ReportItemModelers != null && item.ReportItemModelers.Count > 0)
                {
                    itemCount += item.ReportItemModelers.Count;
                    this.InsertRange(itemIndex + 1, item.ReportItemModelers);
                }                
            }

            itemIndex++;

            if (itemIndex == itemCount)
            {
                this.RaiseReportItemLoaded(null);
            }
            else
            {
                UpdateReportItemModel();
            }
        }

        public void Evaluate()
        {
            try
            {
                itemCount = this.Count;
                itemIndex = -1;
                
                if (itemCount > 0)
                {
                    itemIndex++;
                    this.EvaluateReportItemModel();
                }
                else
                {
                    this.RaiseReportItemEvaluated(null);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Getting following exception while evaluate expressions : " + e.Message);
            }
        }

        public void EvaluateReportItemModel()
        {
            if (this[itemIndex].IsSubReportChild)
            {
                itemIndex++;

                if (itemIndex == itemCount)
                {
                    this.RaiseReportItemEvaluated(null);
                }
                else
                {
                    this.EvaluateReportItemModel();
                }
            }
            else
            {
                this[itemIndex].ReportItemEvaluated += ReportModelContentCollection_ReportItemEvaluated;
                this[itemIndex].Evaluate();
            }           
        }

        void ReportModelContentCollection_ReportItemEvaluated(object sender, ReportItemEvaluatedEventArgs e)
        {
            this[itemIndex].ReportItemEvaluated -= ReportModelContentCollection_ReportItemEvaluated;

            itemIndex++;
            if (itemIndex == itemCount)
            {
                this.RaiseReportItemEvaluated(null);
            }
            else
            {
                this.EvaluateReportItemModel();
            }
        }

        public void UpdateSize()
        {
            foreach (var reportItemModel in this)
            {
                reportItemModel.UpdateSize();
            }
        }

        void RaiseReportItemLoaded(Exception e)
        {
            if (this.ReportItemLoaded != null)
            {
                this.ReportItemLoaded(this, new ReportItemLoadedEventArgs() { Exception = e });
            }
        }

        void RaiseReportItemEvaluated(Exception e)
        {
            if (this.ReportItemEvaluated != null)
            {
                this.ReportItemEvaluated(this, new ReportItemEvaluatedEventArgs() { Exception = e });
            }
        }
    }
}