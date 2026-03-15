#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Collections;
using System.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
    # region LayoutGroup

    [ToolboxItem(false)]
    public partial class LayoutGroup : GradientPanel
    {
        # region Members

        /// <summary>
        /// Indicates first item in the row
        /// </summary>
        bool isFirst = true;

        /// <summary>
        /// Indicates the row
        /// </summary>
        int row = 0;

        /// <summary>
        /// Indicates the column
        /// </summary>
        int colum = 0;

        /// <summary>
        /// Indicates the items
        /// </summary>
        private ImageStreamerCollection items = new ImageStreamerCollection();

        /// <summary>
        /// Indicates to set Items
        /// </summary>
        private bool setItems = true;

        /// <summary>
        /// Indicates the text 
        /// </summary>
        private string text = string.Empty;

        /// <summary>
        /// Indicates to dispose
        /// </summary>
        internal bool canDispose = false;

        /// <summary>
        /// Indicates the timer
        /// </summary>
        Timer t = new Timer();

        # endregion

        # region Constructor

        public LayoutGroup()
        {
           InitializeComponent();
           this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
           this.DoubleBuffered = true;
           this.BackColor = Color.Transparent;
           t.Tick += new EventHandler(t_Tick);
           this.BorderStyle = BorderStyle.None;
           this.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        # endregion

        # region Properities

        /// <summary>
        /// 
        /// </summary>
        public bool SetItem
        {
            get
            {
                return setItems;
            }
            set
            {
                setItems = value;
            }

        }

        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        [
         Description("Gets or sets the ImageStreamer collection."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ImageStreamerCollection Items
        {
            get
            {

                for (int i = 0; i < items.Count; i++)
                {
                    this.Controls.Add(items[i]);
                }
                if (items.Count == 0)
                {
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        items.Add(this.Controls[i]);
                    }
                }

                return items;

            }
            set
            {
                items = value;
            }
        }

        # endregion

        # region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.DesignMode)
                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(0, 0, this.Width, this.Height));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            t.Interval = 50;
            setWidth();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levent"></param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            canDispose = false;
            ArrangeControl();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="se"></param>
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            setMinimumwidth();
            setWidth();
        }

        # endregion

        # region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void t_Tick(object sender, EventArgs e)
        {
            setMinimumwidth();
            setWidth();
            t.Stop();
        }
        
        /// <summary>
        /// 
        /// </summary>
        internal void setMinimumwidth()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        internal void setWidth()
        {

        }

        /// <summary>
        ///  Arrange the controls
        /// </summary>
        internal  void ArrangeControl()
        {
            int minRowWidth = 250;
            if (this.Controls.Count == 0 && canDispose )
            {
                this.Parent.Controls.Remove(this);
                this.Dispose();
            }
            foreach (Control ctrl in this.Controls)
            {
                if (this.Controls.Count ==1)
                {
                    minRowWidth = ctrl.Width + 10;
                    
                }
                int nc = 0;
                if (row * 120 + 10 > this.Height - 200)
                {
                    row = 0;
                    colum++;
                }
                if (ctrl.Width == 120)
                {
                    if (isFirst)
                    {
                        ctrl.Location = new Point(colum * 250 + 5 + nc, row * 125);
                        isFirst = false;
                        nc = 0;
                    }
                    else
                    {

                        ctrl.Location = new Point(colum * 250 + 10 + 120, row * 125);
                        isFirst = true;
                        row++;
                    }
                }
                else
                {
                    if (!isFirst)
                        row++;
                    if (row * 120 + 10 > this.Height - 200)
                    {
                        row = 0;
                        colum++;
                    }

                    ctrl.Location = new Point(colum * 250 + 5 + nc, row * 125);
                    isFirst = true;
                    row++;
                    nc = 0;
                }
            }
            isFirst = true;
            row = 0;
            this.Width =10 + (colum+1) * minRowWidth  + (colum) * 5;
            setMinimumwidth();
            colum = 0;
         }

       /// <summary>
       /// Get the container
       /// </summary>
       /// <param name="item"></param>
       /// <returns></returns>
        private Panel getContainer(ImageStreamer item)
        {
            Panel panel= new Panel ();
            panel.BackColor = Color.Transparent ;
            panel.BorderStyle = BorderStyle.None;
            panel.Controls.Add(item);
            return panel;
        }

        # endregion

    }

    # endregion

    # region Collection

    public class ImageStreamerCollection : ArrayListExt
    {
        // Fields
        internal IComparer comparer = null;
        /// <summary>
        /// Creates a new instance of the collection.
        /// </summary>
        public ImageStreamerCollection()
        {
        }

        public event CollectionChangeEventHandler BeforeRemoving;

        public override void RemoveAt(int index)
        {
            OnBeforeRemoving(index);

            base.RemoveAt(index);
        }

        protected virtual void OnBeforeRemoving(int index)
        {
            ImageStreamer removingNode = this[index];

            CollectionChangeEventArgs e =
                new CollectionChangeEventArgs(CollectionChangeAction.Remove, removingNode);

            RaiseBeforeRemoving(e);
        }

        protected void RaiseBeforeRemoving(CollectionChangeEventArgs e)
        {
            if (this.BeforeRemoving != null)
            {
                BeforeRemoving(this, e);
            }
        }

        /// </override>
        protected override void OnCollectionChanged(CollectionChangeEventArgs args)
        {
            base.OnCollectionChanged(args);
        }

        /// <summary>
        /// Gets / sets a reference to the TreeNodeAdv at the specified index location in the
        /// collection.
        /// In C#, this property is the indexer for the TreeNodeAdvCollection class.
        /// </summary>
        /// <param name="index">The location of the TreeNodeAdv in the collection.</param>
        /// <value>The reference to the TreeNodeAdv.</value>
        public new ImageStreamer this[int index]
        {
            get
            {
                return (ImageStreamer)base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Adds a <see cref="TreeNodeAdv"/> to the collection.
        /// </summary>
        /// <param name="node">The <see cref="TreeNodeAdv"/> to add.</param>
        /// <returns>The position of the added node in the list.</returns>
        public virtual int Add(ImageStreamer node)
        {
            return base.Add(node);
        }
        /// <summary>
        /// Adds an array of TreeNodeAdv objects to the collection.
        /// </summary>
        /// <param name="items">An array of <see cref="TreeNodeAdv"/> objects to add to the collection.</param>
        public void AddRange(ImageStreamer[] items)
        {
            base.AddRange(items);
        }


        /// </override>

        public override void Sort()
        {
            Sort(SortOrder.Ascending);
        }

        /// <summary>
        /// Sorts the collection using the specified sort order.
        /// </summary>
        /// <param name="order">One of the <see cref="SortOrder"/> entries.</param>
        public virtual void Sort(SortOrder order)
        {
            if (order == SortOrder.None) return;
            if (this.comparer != null)
                this.Sort(comparer);
            else
                base.Sort();
            if (order == SortOrder.Descending)
                this.Reverse();
        }
    }
    # endregion

}
