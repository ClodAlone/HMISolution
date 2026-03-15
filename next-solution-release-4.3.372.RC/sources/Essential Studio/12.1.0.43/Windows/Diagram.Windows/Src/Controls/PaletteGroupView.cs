#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using Syncfusion.Diagram.Base.Wizard;
using Syncfusion.Windows.Forms.Diagram;
using Syncfusion.Windows.Forms.Diagram.Wizards;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Displays the symbol models belonging to a symbol palette in a GroupView.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is derived from the Syncfusion.Windows.Forms.Tools.GroupView
    /// class. A GroupView is a control that contains a list of icons and labels
    /// that can be hosted in a GroupBar (Outlook bar).
    /// </para>
    /// <para>
    /// This class provides an implementation that displays a list of symbol models
    /// that belong to a given symbol palette. Symbol models can be dragged from
    /// this control onto diagrams.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupBar"/>
    /// </remarks>
    [ToolboxItem(true)]
    [Designer(typeof(PaletteGroupViewDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [ToolboxBitmap(typeof(PaletteGroupView), "ToolboxIcons.PaletteGroupView.bmp")]
    [Description("GroupView control that presents a Diagram Symbol palette in the client area of a PaletteGroupBar.")]
    public class PaletteGroupView
        : GroupView
    {
        #region Class constants
        private const int WM_LBUTTONDBLCLK = 515;
        private const int WM_MOUSEMOVE = 512;
        #endregion

        #region Class members
        private SymbolPalette m_palette;
        private bool m_bBeginDrag;
        private bool m_bEditMode;
        private bool m_bShowDragNodeCue = true;
        private bool m_bDragNodeCueEnabled = true;
        private DiagramDragHelper dragHelper = null;
        private float m_fMagnification = 100f;
        private MouseButtons m_mouseButtonDowned = MouseButtons.None;
        private Diagram m_diagram;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="PaletteGroupView"/> class.
        /// </summary>
        /// <remarks>Added to facilitate opening palettes created using 2.x version.</remarks>
        static PaletteGroupView()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve +=
                    new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Core.Licensing.LicensedComponent(typeof(PaletteGroupView));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -=
                    new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            Syncfusion.Runtime.Serialization.AppStateSerializer.SetBindingInfo("Syncfusion.Diagram", DiagramBaseAssembly.Assembly);
            Syncfusion.Runtime.Serialization.AppStateSerializer.SetBindingInfo("Syncfusion.Diagram.Base", DiagramBaseAssembly.Assembly);
            Syncfusion.Runtime.Serialization.AppStateSerializer.SetBindingInfo("Syncfusion.Diagram.Windows", DiagramWindowsAssembly.Assembly);
            string controlnodebinding =
                "Syncfusion.Windows.Forms.Diagram.ControlNodeSyncfusion.Diagram.Base".ToLower();
            string activatestylebinding =
                "Syncfusion.Windows.Forms.Diagram.ActivateStyleSyncfusion.Diagram.Base".ToLower();
            if (!Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.ContainsKey(controlnodebinding))
            {
                Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.Add(
                    controlnodebinding.ToLower(), Assembly.GetExecutingAssembly());
            }

            if (!Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.ContainsKey(activatestylebinding))
            {
                Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.Add(
                    activatestylebinding.ToLower(), Assembly.GetExecutingAssembly());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupView"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public PaletteGroupView(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupView"/> class.
        /// </summary>
        public PaletteGroupView()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Core.Licensing.LicensedComponent(typeof(PaletteGroupView));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(this.GetType());
            
            // ctrlToolTip
            this.ctrlToolTip.Location = new Point(132, 174);
           
            // smImages
            this.ilSmall = new ImageList();
            this.ilSmall.ColorDepth = ColorDepth.Depth32Bit;
            this.ilSmall.ImageSize = new Size(16, 16);
            this.ilSmall.TransparentColor = Color.Transparent;
            Bitmap smallimage = resources.GetObject("SymbolModelSmall") as Bitmap;
            if (smallimage != null)
                this.ilSmall.Images.Add(smallimage);
           
            // lgImages
            this.ilLarge = new ImageList();
            this.ilLarge.ColorDepth = ColorDepth.Depth32Bit;
            this.ilLarge.ImageSize = new Size(32, 32);
            this.ilLarge.TransparentColor = Color.Transparent;
            Bitmap largeimage = resources.GetObject("SymbolModelLarge") as Bitmap;
            if (largeimage != null)
            {
                this.ilLarge.Images.Add(largeimage);
            }
            this.dragHelper = new DiagramDragHelper();
            this.ButtonView = true;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the diagram control.
        /// </summary>
        [Browsable(true)]
        [Description("Diagram associates with palette groupview.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Diagram Diagram
        {
            get
            {
                return m_diagram;
            }
            set
            {
                if (value != m_diagram)
                    m_diagram = value;
            }
        }      
        /// <summary>
        /// Gets or sets a value indicating whether dragged node cue is visible or not.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether the dragged node's cue will be shown besides the cursor.")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ShowDragNodeCue
        {
            get
            {
                return m_bShowDragNodeCue;
            }
            set
            {
                if (value != m_bShowDragNodeCue)
                    m_bShowDragNodeCue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dragged node cue is enabled or not.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether the dragged node's cue will be enabled.")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool DragNodeCueEnabled
        {
            get
            {
                return m_bDragNodeCueEnabled;
            }
            set
            {
                if (value != m_bDragNodeCueEnabled)
                    m_bDragNodeCueEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets drag cue's magnification (zoom) values on a scale of 1 to n.
        /// </summary>        
        [Browsable(true)]
        [Description("Specifies magnification value applied to dragged node's cue.")]
        [DefaultValue(100f)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Magnification
        {
            get
            {
                return m_fMagnification;
            }
            set
            {
                if (value != m_fMagnification)
                {
                    m_fMagnification = value;                   
                }
            }
        }

        /// <summary>
        /// Gets or sets the image list containing the large (32x32) images.
        /// </summary>
        /// <value>An ImageList type.</value>
        /// <remarks>
        /// <seealso cref="GroupView.SmallImageList"/>
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageList LargeImageList
        {
            get
            {
                if (m_palette != null)
                {
                    return m_palette.LargeImageList;
                }

                return null;
            }
            set 
            { 
            }
        }

        /// <summary>
        /// Gets or sets the image list containing the small (16x16) images.
        /// </summary>
        /// <value>An ImageList type.</value>
        /// <remarks>
        /// <seealso cref="GroupView.LargeImageList"/>
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageList SmallImageList
        {
            get
            {
                if (m_palette != null)
                {
                    return m_palette.SmallImageList;
                }

                return null;
            }
            set 
            { 
                ilSmall = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the symbols can be dragged from the palette
        /// onto a diagram.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this property is set to True, the palette is being edited and symbols
        /// cannot be dragged onto a diagram. The Symbol Designer sets this flag to True.
        /// The typical setting for an application is False, which means that the palette
        /// is not being edited and symbols may be dragged onto a diagram.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [Description("Specifies whether symbols can be dragged from the palette onto a diagram.")]
        [DefaultValue(false)]
        public bool EditMode
        {
            get { return m_bEditMode; }
            set { m_bEditMode = value; }
        }

        /// <summary>
        /// Gets or sets reference to the symbol palette displayed by this control.
        /// </summary>
        [Browsable(true)]
        [Editor(typeof(PaletteOpener), typeof(UITypeEditor))]
        [TypeConverter(typeof(SymbolPaletteConverter))]
        [Description("The symbol palette displayed by this control.")]
        [DefaultValue(null)]
        public SymbolPalette Palette
        {
            get 
            { 
                return m_palette; 
            }
            set
            {
                if (m_palette != value)
                {
                    this.GroupViewItems.Clear();

                    if ((value == null) && (m_palette != null))
                    {
                        m_palette.PaletteChildrenChanged -= new CollectionEEventHandler(OnPalette_ChildrenChangeComplete);
                        m_palette = null;
                        ilLarge = null;
                        ilSmall = null;
                    }
                    else
                    {
                        m_palette = value;
                        ilLarge = LargeImageList;
                        ilSmall = SmallImageList;
                        m_palette.PaletteChildrenChanged += new CollectionEEventHandler(OnPalette_ChildrenChangeComplete);

                        m_palette.UpdateIcons();

                        foreach (Node node in m_palette.Nodes)
                        {
                            AddNode(node);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected node.
        /// </summary>
        /// <value>The selected node.</value>
        public Node SelectedNode
        {
            get
            {
                Node nodeSel = null;

                // get selected item
                if (this.SelectedItem >= 0 && this.SelectedItem < this.GroupViewItems.Count)
                {
                    nodeSel = this.m_palette.Nodes[this.SelectedItem]; // this.GroupViewItems[this.SelectedItem].Tag as Node;
                }

                return nodeSel;
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="GroupViewItem"/> objects in the control.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="GroupView.GroupViewItemCollection"/> type.
        /// </value>
        [Browsable(false)]
        public new GroupView.GroupViewItemCollection GroupViewItems
        {
            get { return base.GroupViewItems; }
            set { base.GroupViewItems = value; }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Fired when the user selects a symbol model icon in a palette groupview component.
        /// </summary>
        public event NodeEventHandler NodeSelected;

        /// <summary>
        /// Occurs when the control is double-clicked.
        /// </summary>
        public new event EventHandler DoubleClick;
        #endregion

        #region Class public methods
        /// <summary>
        /// Loads a symbol palette from a file.
        /// </summary>
        /// <param name="filename">Name of file to load.</param>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// Deserializes a symbol palette from disk and loads it into this
        /// control.
        /// </remarks>
        public bool LoadPalette(string filename)
        {
            bool bSuccess = false;
            SymbolPalette symbolPalette = null;
            FileStream iStream;

            if (File.Exists(filename))
            {
                iStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                symbolPalette = GetPaletteFromStream(iStream);
            }

            if (symbolPalette != null)
            {
                bSuccess = LoadPalette(symbolPalette);
            }

            return bSuccess;
        }

        /// <summary>
        /// Loads the given symbol palette into this control.
        /// </summary>
        /// <param name="symbolPalette">SymbolPalette to load.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool LoadPalette(SymbolPalette symbolPalette)
        {
            if (symbolPalette == null) return false;

            this.Palette = symbolPalette;

            return true;
        }

        /// <summary>
        /// Loads a symbol palette from a resource file.
        /// </summary>
        /// <param name="assembly">Assembly containing the symbol palette.</param>
        /// <param name="baseName">Base name of resource.</param>
        /// <param name="resName">Name of resource.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool LoadPalette(Assembly assembly, string baseName, string resName)
        {
            bool bSuccess = false;
            SymbolPalette symbolPalette;
            System.Resources.ResourceManager resMgr = new System.Resources.ResourceManager(baseName, assembly);
            object resObj = resMgr.GetObject(resName);

            if (resObj != null && resObj.GetType() == typeof(byte[]))
            {
                MemoryStream strmMem = new MemoryStream((byte[])resObj);
                symbolPalette = GetPaletteFromStream(strmMem);

                if (symbolPalette != null)
                {
                    this.GroupViewItems.Clear();
                    bSuccess = LoadPalette(symbolPalette);
                }
            }
            return bSuccess;
        }

        /// <summary>
        /// Loads a symbol palette from memory.
        /// </summary>
        /// <param name="strmData">Array of bytes containing serialized symbol palette.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool LoadPalette(byte[] strmData)
        {
            bool bSuccess = false;
            MemoryStream strmMem = new MemoryStream(strmData);
            SymbolPalette symbolPalette = GetPaletteFromStream(strmMem);

            if (symbolPalette != null)
            {
                bSuccess = LoadPalette(symbolPalette);
            }

            return bSuccess;
        }

        /// <summary>
        /// Set the selected symbol model to the one matching the given symbol
        /// model name.
        /// </summary>
        /// <param name="node">Node to select.</param>
        public void SelectNode(Node node)
        {
            this.SelectedItem = m_palette.Nodes.IndexOf(node);
        }
        #endregion

        #region Class utility methods
        private Bitmap GetDragNodeCue()
        {
            int width = (int)Math.Ceiling((SelectedNode.Size.Width + SelectedNode.LineStyle.LineWidth) * Magnification / 100);
            int height = (int)Math.Ceiling((SelectedNode.Size.Height + SelectedNode.LineStyle.LineWidth) * Magnification / 100);
            Bitmap dragNodeCue = new Bitmap((int)MeasureUnitsConverter.ToPixelX(width,SelectedNode.MeasurementUnit), (int)MeasureUnitsConverter.ToPixelY(height,SelectedNode.MeasurementUnit));
            using (Graphics gfx = Graphics.FromImage(dragNodeCue))
            {                               
                Matrix mtx = new Matrix();
                mtx.Scale(Magnification / 100, Magnification / 100);
                gfx.MultiplyTransform(mtx, MatrixOrder.Append);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                this.SelectedNode.Draw(gfx, true);
            }            
            return dragNodeCue;
        }

        sealed class OldToNewDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                // For each assemblyName/typeName that you want to deserialize to
                // a different type, set typeToDeserialize to the desired type.
                string assem = Assembly.GetExecutingAssembly().FullName;

                if (assemblyName.IndexOf("Syncfusion.Diagram") != -1 && assemblyName.IndexOf("Version", 0) != -1)
                {
                    // find "Version" substring
                    int nIdxStart = assemblyName.IndexOf("Version", 0);
                    int nIdxEnd = assemblyName.IndexOf(",", nIdxStart);

                    int nIdxRplStart = assem.IndexOf("Version", 0);
                    int nIdxRplEnd = assem.IndexOf(",", nIdxRplStart);

                    // replace whole "Version" substring
                    assemblyName = assemblyName.Replace(
                        assemblyName.Substring(nIdxStart, nIdxRplStart + (nIdxEnd - nIdxStart)),
                        assem.Substring(nIdxRplStart, nIdxRplStart + (nIdxRplEnd - nIdxRplStart)));
                }

                return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            }
        }

        private SymbolPalette GetPaletteFromStream(Stream strmPalette)
        {
            SymbolPalette symbolPaletteToReturn = null;
            IFormatter formatter = new BinaryFormatter();
            ((BinaryFormatter)formatter).AssemblyFormat = FormatterAssemblyStyle.Simple;
            ((BinaryFormatter)formatter).TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
            ((BinaryFormatter)formatter).FilterLevel = TypeFilterLevel.Low;
            formatter.Binder = new OldToNewDeserializationBinder();

            try
            {
                symbolPaletteToReturn = (SymbolPalette)formatter.Deserialize(strmPalette);
            }
            catch (SerializationException)
            {
                // try soap formatter
                try
                {
                    formatter = new SoapFormatter();
                    ((SoapFormatter)formatter).AssemblyFormat = FormatterAssemblyStyle.Simple;
                    ((SoapFormatter)formatter).TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                    ((SoapFormatter)formatter).FilterLevel = TypeFilterLevel.Low;
                    formatter.Binder = new OldToNewDeserializationBinder();
                    
                    // rewind to begining
                    strmPalette.Position = 0;

                    symbolPaletteToReturn = (SymbolPalette)formatter.Deserialize(strmPalette);
                }
                catch (SerializationException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            finally
            {
                strmPalette.Close();
            }

            return symbolPaletteToReturn;
        }

        private void AddNode(Node node)
        {
            GroupViewItem gvItem = new GroupViewPaletteItem(node, Palette.Nodes.IndexOf(node), m_palette);
            this.GroupViewItems.Add(gvItem);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="bdispose">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <override/>
        protected override void Dispose(bool bdispose)
        {
            if (dragHelper != null)
            {
                dragHelper.DragWindow.Dispose();
            }
            base.Dispose(bdispose);
        }

        /// <summary>
        /// Called when a query continue drag event is received.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs args)
        {
            base.OnQueryContinueDrag(args);
            if (m_bDragNodeCueEnabled)
            {
                dragHelper.DragAction = args.Action;
                if (args.Action == DragAction.Drop)
                {
                    dragHelper.EndDrag();
                }
                else if (args.Action == DragAction.Cancel)
                {
                    dragHelper.CancelDrag();
                }
            }
        }

        /// <summary>
        /// Called when a mouse down event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// <para>
        /// If <see cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupView.EditMode"/>
        /// is enabled, then this method starts a drag-and-drop operation.
        /// </para>
        /// </remarks>
        protected override void OnMouseDown(MouseEventArgs evtArgs)
        {
            base.OnMouseDown(evtArgs);

            if (!m_bEditMode)
            {
                if (this.scrllBtnState != ScrollButtonState.DownScrollPressed && this.scrllBtnState != ScrollButtonState.UpScrollPressed)
                    m_bBeginDrag = GetItemAt(evtArgs.Location) != null;
                if (m_bBeginDrag)
                    m_mouseButtonDowned = evtArgs.Button;
            }
        }

        /// <summary>
        /// Called when a mouse move event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        /// <remarks>
        /// <para>
        /// If a drag-and-drop operation has been started with a mouse down, this
        /// method calls the System.Windows.Forms.Control.DoDragDrop method.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupView.OnMouseDown"/>
        /// </remarks>
        protected override void OnMouseMove(MouseEventArgs evtArgs)
        {
            base.OnMouseMove(evtArgs);

            if (m_bBeginDrag)
            {
                Node nodeSel = this.SelectedNode;

                if (nodeSel != null)
                {
                    if (this.m_bDragNodeCueEnabled)
                    {
                        int width = (int)Math.Ceiling((SelectedNode.Size.Width + SelectedNode.LineStyle.LineWidth) * Magnification / 100);
                        int height =  (int)Math.Ceiling((SelectedNode.Size.Height + SelectedNode.LineStyle.LineWidth) * Magnification / 100);
                        Bitmap dragNodeCue = new Bitmap(width,height);
                        if (m_bShowDragNodeCue)
                        {
                            dragNodeCue = GetDragNodeCue();
                            dragNodeCue.Tag = this.SelectedNode.Name;
                        }
                        if (dragNodeCue != null)
                        {
                            Point ptCur = Control.MousePosition;
                            dragHelper.DragWindow.AllowDrop = true;
                            dragHelper.DragWindow.SetOrigin(new Point(dragNodeCue.Width / 2, -5));
                            dragHelper.StartDrag(dragNodeCue, ptCur, DragDropEffects.Copy);
                        }
                    }
                    Node node = nodeSel.Clone() as Node;
                    DragDropData data = new DragDropData(node, dragHelper);
                    data.DragNodeCueEnabled = DragNodeCueEnabled;
                    DoDragDrop(data, DragDropEffects.Copy);
                    if (this.Diagram != null)
                    {
                        if (this.Diagram.Controller.ActiveTool is InsertNodeTool)
                            this.Diagram.Controller.ActivateTool("SelectTool");
                    }
                    m_bBeginDrag = false;
                    if (NodeSelected != null)
                        NodeSelected(this, new NodeEventArgs(this.SelectedNode));
                }
            }
        }

        /// <summary>
        /// Called when a give feedback event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs evtArgs)
        {
            base.OnGiveFeedback(evtArgs);
            if (m_bDragNodeCueEnabled)
            {
                if (m_bShowDragNodeCue && SelectedNode.Name != (string)dragHelper.DragWindow.DragBitmap.Tag)
                {
                    dragHelper.DragWindow.DragBitmap = GetDragNodeCue();
                    dragHelper.DragWindow.DragBitmap.Tag = SelectedNode.Name;
                }
                Point ptCur = Control.MousePosition;
                dragHelper.DoDrag(ptCur, DragDropEffects.Copy);
            }
        }        

        /// <summary>
        /// Called when a mouse up event is received.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected override void OnMouseUp(MouseEventArgs evtArgs)
        {
            base.OnMouseUp(evtArgs);
            m_bBeginDrag = false;
            dragHelper.EndDrag();
            if (this.Diagram != null)
            {
                if (GetItemAt(evtArgs.Location) != null)
                {
                    this.Diagram.Controller.NodeToInsert = (Node)this.SelectedNode.Clone();
                    this.Diagram.Controller.ActivateTool("InsertNodeTool");
                    this.Diagram.Controller.IsPaletteNodeClicked = GetItemAt(evtArgs.Location) != null;
                }
                else
                    this.Diagram.Controller.ActivateTool("SelectTool");
            }
        }

        /// <summary>
        /// Called when a item selected event is received.
        /// </summary>
        /// <param name="arg">Event arguments.</param>
        protected override void OnGroupViewItemSelected(EventArgs arg)
        {
            base.OnGroupViewItemSelected(arg);
            if (NodeSelected != null)
                NodeSelected(this, new NodeEventArgs(this.SelectedNode));
        }

        /// <summary>
        /// Overriden. WndProc.
        /// </summary>
        /// <param name="m">The message.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE && m_mouseButtonDowned == MouseButtons.Left)
            {
                m_mouseButtonDowned = MouseButtons.None;
                return;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                if (this.DoubleClick != null && m_bBeginDrag)
                {
                    this.DoubleClick(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Event handlers
        private void OnPalette_ChildrenChangeComplete(object sender, CollectionExEventArgs nodeEvtArgs)
        {
            if (nodeEvtArgs.ChangeType == CollectionExChangeType.Insert)
            {
                Node nodeTmp = nodeEvtArgs.Element as Node;

                if (nodeTmp != null)
                {
                    GroupViewItem gvItem = new GroupViewPaletteItem(nodeTmp, nodeEvtArgs.Index, m_palette);

                    if (nodeEvtArgs.Index == this.GroupViewItems.Count)
                        this.GroupViewItems.Add(gvItem);
                    else
                    {
                        this.GroupViewItems.Insert(nodeEvtArgs.Index, gvItem);

                        for (int i = nodeEvtArgs.Index + 1, length = this.GroupViewItems.Count; i < length; i++)
                        {
                            this.GroupViewItems[i].ImageIndex++;
                        }
                    }

                    this.SelectedItem = nodeEvtArgs.Index;
                    Refresh();
                }
            }
            else if (nodeEvtArgs.ChangeType == CollectionExChangeType.Remove)
            {
                int nIdxToDelete = -1;

                if (nodeEvtArgs.Index != -1)
                {
                    this.GroupViewItems.Remove(this.GroupViewItems[nodeEvtArgs.Index]);

                    if (nIdxToDelete != 0)
                        this.SelectedItem = nIdxToDelete - 1;

                    for (int i = nodeEvtArgs.Index; i < GroupViewItems.Count; i++)
                    {
                        GroupViewItems[i].ImageIndex = i;
                    }

                    Refresh();
                }
            }
        }
        #endregion

        #region Class internal declarations
        /// <summary>
        /// GroupViewItem derived class representing a symbol model in a group view.
        /// </summary>
        [TypeConverter(typeof(PaletteGroupViewEditor))]
        public class GroupViewPaletteItem
            : GroupViewItem
        {
            #region Class members
            private SymbolPalette m_palette;
            #endregion

            #region Class properties
            /// <summary>
            /// Gets or sets a value indicating whether to use default large icon.
            /// </summary>
            [PaletteProperty, Category("Icon"), DefaultValue(true), 
            Description("Gets or sets value indicates that use default large icon.")]
            public bool UseDefaultLargeIcon
            {
                get
                {
                    return !m_palette.IsUserDefinitedLargeIcon(this.ImageIndex);
                }
                set
                {
                    if (value == m_palette.IsUserDefinitedLargeIcon(this.ImageIndex))
                    {
                        if (value)
                        {
                            m_palette.SetUserLargeImage(this.ImageIndex, null);
                        }

                        this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("UseDefaultLargeIcon"));
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether to use default small icon.
            /// </summary>
            [PaletteProperty, Category("Icon"), DefaultValue(true), 
            Description("Gets or sets value indicates that use default small icon.")]
            public bool UseDefaultSmallIcon
            {
                get
                {
                    return !m_palette.IsUserDefinitedSmallIcon(this.ImageIndex);
                }
                set
                {
                    if (value == m_palette.IsUserDefinitedSmallIcon(this.ImageIndex))
                    {
                        if (value)
                        {
                            m_palette.SetUserSmallImage(this.ImageIndex, null);
                        }

                        this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("UseDefaultSmallIcon"));
                    }
                }
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [PaletteProperty, Description("Name of the palette item")]
            public string Name
            {
                get
                {
                    return m_palette.Nodes[this.ImageIndex].Name;
                }
                set
                {
                    Node node = m_palette.Nodes[this.ImageIndex];

                    if (node != null && node.Name != value)
                    {
                        m_palette.Nodes[this.ImageIndex].Name = value;
                        this.Text = value;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the large icon.
            /// </summary>
            /// <value>The large icon.</value>
            [PaletteProperty, Description("Gets or sets large image icon to display in symbol palette."),
            Category("Icon"), DefaultValue(null)]
            public Image LargeIcon
            {
                get
                {
                    if (UseDefaultLargeIcon)
                    {
                        return null;
                    }

                    return m_palette.LargeImageList.Images[this.ImageIndex];
                }
                set
                {
                    m_palette.SetUserLargeImage(this.ImageIndex, value);
                    this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("LargeIcon"));
                }
            }

            /// <summary>
            /// Gets or sets the small icon.
            /// </summary>
            /// <value>The small icon.</value>
            [PaletteProperty, Description("Gets or sets small image icon to display in symbol palette."),
            Category("Icon"), DefaultValue(null)]
            public Image SmallIcon
            {
                get
                {
                    if (UseDefaultSmallIcon)
                    {
                        return null;
                    }

                    return m_palette.SmallImageList.Images[this.ImageIndex];
                }
                set
                {
                    m_palette.SetUserSmallImage(this.ImageIndex, value);
                    this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SmallIcon"));
                }
            }

            #endregion

            #region Class initialize/finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="GroupViewPaletteItem"/> class.
            /// </summary>
            /// <param name="symModel">Symbol model to display in the group view.</param>
            public GroupViewPaletteItem(Node symModel)
                : base(symModel.Name, -1)
            {
                this.Tag = symModel;
                this.Text = symModel.Name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupViewPaletteItem"/> class.
            /// </summary>
            /// <param name="symModel">Symbol model to display in the group view.</param>
            /// <param name="imageIdx">The image index.</param>
            /// <param name="palette">The palette.</param>
            public GroupViewPaletteItem(Node symModel, int imageIdx, SymbolPalette palette)
                : base(symModel.Name, imageIdx)
            {
                this.Tag = symModel;
                this.Text = symModel.Name;
                m_palette = palette;
            }
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// Filter GroupViewPaletteItem properties to display in property grid.
    /// Show only properties marked with PalettePropertyAttribute.
    /// </summary>
    public class PaletteGroupViewEditor : TypeConverter
    {
        #region Class initialize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupViewEditor"/> class.
        /// </summary>
        public PaletteGroupViewEditor()
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Determines whether this object supports properties. By default, this is false.
        /// </summary>
        /// <param name="context">A type descriptor through which additional context can be provided.</param>
        /// <returns>This method returns true if the GetProperties object should be called to find the properties of this object; otherwise, false</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Retrieves the set of properties for this type to filter properties for GroupViewPaletteItem.
        /// </summary>
        /// <param name="context">A type descriptor through which additional context can be provided.</param>
        /// <param name="value">The value of the object to get the properties for.</param>
        /// <param name="attributes">An array of MemberAttribute objects that specify the attributes of the properties. </param>
        /// <returns>The set of properties that should be exposed for this data type. If no properties should be exposed, this may return null. The default implementation always returns null.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection res = TypeDescriptor.GetProperties(value, new Attribute[] { new PalettePropertyAttribute() });

            return res;
        }

        #endregion
    }

    /// <summary>
    /// Specifies whether a property is symbol model's property.
    /// </summary>
    public class PalettePropertyAttribute : Attribute
    {
        #region Class initialize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PalettePropertyAttribute"/> class.
        /// </summary>
        public PalettePropertyAttribute()
        { 
        }
        #endregion
    }

    #region PaletteGroupViewDesigner
    /// <summary>
    /// Palette GroupView designer.
    /// </summary>
    public class PaletteGroupViewDesigner : Syncfusion.Windows.Forms.Tools.Design.GroupViewDesigner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupViewDesigner"/> class.
        /// </summary>
        public PaletteGroupViewDesigner()
            : base()
        { 
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Gets the action lists.
        /// </summary>
        /// <value>The action lists.</value>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                for (int i = 0, length = base.ActionLists.Count; i < length; i++)
                {
                    if (!(base.ActionLists[i] is PaletteGroupViewActionList))
                        base.ActionLists[i] = new PaletteGroupViewActionList(this.Component);
                }

                return base.ActionLists;
            }
        }
#endif

        /// <summary>
        /// Called when the designer is initialized.
        /// </summary>
        [Obsolete]
        public override void OnSetComponentDefaults()
        {
            if (DiagramLoadBaseWizard.GetAutoRunWizard())
            {
                DiagramWindowsOnLoadWizard wizard = new DiagramWindowsOnLoadWizard(this.Control);
                wizard.ShowDialog();

                try
                {
                    ((PaletteGroupView)this.Component).LoadPalette(wizard.EDPPath[0]);
                }
                catch
                {
                }
            }

            base.OnSetComponentDefaults();
        }
    }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    /// <summary>
    /// Using to customize action list.
    /// </summary>
    public class PaletteGroupViewActionList : GroupViewActionList
    {
        private bool bLock = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupViewActionList"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        public PaletteGroupViewActionList(IComponent component)
            : base(component)
        { 
        }

        /// <summary>
        /// Initializes the action list.
        /// </summary>
        protected override void InitializeActionList()
        {
            base.InitializeActionList();

            if (!bLock)
            {
                bLock = true;

                DesignerActionItemCollection actionItems = base.GetSortedActionItems();
                string propertyToRemove = "Items Collection";
                string propertyCategory = "Misc";

                if (actionItems.Count > 0)
                {
                    DesignerActionItem itemToRemove = null;

                    foreach (DesignerActionItem item in actionItems)
                    {
                        if (propertyToRemove == item.DisplayName && item.Category == propertyCategory)
                        {
                            itemToRemove = item;
                            break;
                        }
                    }

                    if (itemToRemove != null)
                        actionItems.Remove(itemToRemove);
                }

                this.AddDesignerActionMethodItem("ShowWizard", "Show Wizard", "", "Show Diagram Wizard");
                bLock = false;
            }
        }

        private void ShowWizard()
        {
            DiagramWindowsOnLoadWizard wizard = new DiagramWindowsOnLoadWizard(this.Control);
            wizard.ShowDialog();
            try
            {
                ((PaletteGroupView)this.Component).LoadPalette(((DiagramLoadBaseWizard)wizard).EDPPath[0]);
            }
            catch
            {
            }
            this.InvalidateComponent(this.Component);
        }

        /// <summary>
        /// Invalidates current component
        /// </summary>
        /// <param name="component">The component.</param>
        private void InvalidateComponent(IComponent component)
        {
            if (component.Site != null)
            {
                IDesignerHost iDesignerHost = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                if (iDesignerHost != null)
                {
                    IComponentChangeService componentChangeService = iDesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                    if (componentChangeService != null)
                    {
                        componentChangeService.OnComponentChanged(component, null, null, null);
                    }
                }
            }
        }
    }
#endif
    #endregion
}
