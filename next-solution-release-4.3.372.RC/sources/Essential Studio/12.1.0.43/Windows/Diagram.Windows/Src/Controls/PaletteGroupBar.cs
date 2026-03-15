#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Displays a collection of symbol palettes that the symbol models contain
    /// in a GroupBar control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is derived from Syncfusion.Windows.Forms.Tools.GroupBar and
    /// provides an implementation for displaying symbol palettes and the symbol
    /// models they contain. Each symbol palette corresponds to a single panel
    /// (i.e. GroupView) inside of the GroupBar. Each entry in a panel corresponds
    /// to a symbol model inside of a symbol palette.
    /// </para>
    /// <para>
    /// The user interface looks and behaves like an Outlook bar. Each symbol
    /// palette is a list of symbols that have an icon and a label. If the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupBar.EditMode"/>
    /// flag is False, the PaletteGroupBar allows symbols to be dragged from
    /// this control onto diagrams.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupView"/>
    /// </remarks>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PaletteGroupBar), "ToolboxIcons.PaletteGroupBar.bmp")]
    [Description("GroupBar control that presents Diagram Symbol palettes in an Outlook-style control.")]
    public class PaletteGroupBar
        : GroupBar
    {
        #region Fields
        private IContainer components = null;
        private bool m_bEditMode = false;
        private bool m_bShowDragNodeCue = true;
        private bool m_bDragNodeCueEnabled = true;
        private float m_fMagnification = 100f;
        private Diagram m_diagram;
        #endregion

        #region Initialize\finalize methods
        // Added to facilitate opening palettes created using 2.x version.
        static PaletteGroupBar()
        {
            Syncfusion.Runtime.Serialization.AppStateSerializer.SetBindingInfo("Syncfusion.Diagram", DiagramBaseAssembly.Assembly);
            Syncfusion.Runtime.Serialization.AppStateSerializer.SetBindingInfo("Syncfusion.Diagram.Base", DiagramBaseAssembly.Assembly);
            Syncfusion.Runtime.Serialization.AppStateSerializer.SetBindingInfo("Syncfusion.Diagram.Windows", DiagramWindowsAssembly.Assembly);
            string controlnodebinding = "Syncfusion.Windows.Forms.Diagram.ControlNodeSyncfusion.Diagram.Base".ToLower();
            string activatestylebinding = "Syncfusion.Windows.Forms.Diagram.ActivateStyleSyncfusion.Diagram.Base".ToLower();
            if (Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.ContainsKey(controlnodebinding) == false)
                Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.Add(controlnodebinding.ToLower(), System.Reflection.Assembly.GetExecutingAssembly());
            if (Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.ContainsKey(activatestylebinding) == false)
                Syncfusion.Runtime.Serialization.AppStateSerializer.CustomBinder.TypeNamesVsAssembly.Add(activatestylebinding.ToLower(), System.Reflection.Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupBar"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public PaletteGroupBar(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteGroupBar"/> class.
        /// </summary>
        public PaletteGroupBar()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Core.Licensing.LicensedComponent(typeof(PaletteGroupBar));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        { }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the diagram control.
        /// </summary>
        [Browsable(true)]    
        [Description("Diagram associates with palette groupbar.")]
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
                {
                    m_diagram = value;
                    foreach (GroupBarItem groupBar in this.GroupBarItems)
                    {
                        PaletteGroupView paletteView = groupBar.Client as PaletteGroupView;
                        paletteView.Diagram = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether dragged node cue is visible or not
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
                {
                    m_bShowDragNodeCue = value;
                    foreach (GroupBarItem  groupBar in this.GroupBarItems)
                    {
                        PaletteGroupView paletteView = groupBar.Client as PaletteGroupView;
                        paletteView.ShowDragNodeCue = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dragged node cue is enabled or not
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
                {
                    m_bDragNodeCueEnabled = value;
                    foreach (GroupBarItem groupBar in this.GroupBarItems)
                    {
                        PaletteGroupView paletteView = groupBar.Client as PaletteGroupView;
                        paletteView.DragNodeCueEnabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets drag cue's magnification (zoom) values on a scale of 1 to n.
        /// </summary>        
        [Browsable(true)]
        [Description("Specifies magnification value applied to drag node cue.")]
        [DefaultValue(100)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
                    foreach (GroupBarItem groupBar in this.GroupBarItems)
                    {
                        PaletteGroupView paletteView = groupBar.Client as PaletteGroupView;
                        paletteView.Magnification = value;
                    }
                }
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
        [Category("Behavior")]
        [Description("Determines whether or not symbols can be dragged from the palette onto a diagram.")]
        public bool EditMode
        {
            get { return m_bEditMode; }
            set { m_bEditMode = value; }
        }

        /// <summary>
        /// Gets the currently selected symbol palette.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SymbolPalette CurrentSymbolPalette
        {
            get
            {
                SymbolPalette curSymbolPalette = null;

                if (this.SelectedItem >= 0 && this.SelectedItem < this.GroupBarItems.Count)
                {
                    curSymbolPalette = this.GroupBarItems[this.SelectedItem].Tag as SymbolPalette;
                }

                return curSymbolPalette;
            }
        }

        /// <summary>
        /// Gets the number of symbol palettes loaded in the GroupBar control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PaletteCount
        {
            get { return this.GroupBarItems.Count; }
        }

        /// <summary>
        /// Gets the currently selected symbol model.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Node SelectedNode
        {
            get
            {
                Node nodeToReturn = null;

                if (this.SelectedItem < this.VisibleGroupBarItems.Count)
                {
                    PaletteGroupView pgv = this.VisibleGroupBarItems[this.SelectedItem].Client as PaletteGroupView;

                    if (pgv != null)
                    {
                        nodeToReturn = pgv.SelectedNode;
                    }
                }

                return nodeToReturn;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Removes all symbol palettes from the GroupBar.
        /// </summary>
        public void Clear()
        {
            while (this.GroupBarItems.Count > 0)
            {
                this.GroupBarItems.Remove(this.GroupBarItems[0]);
            }
        }

        /// <summary>
        /// Adds an existing symbol palette to the GroupBar.
        /// </summary>
        /// <param name="symbolPalette">Symbol palette to add.</param>
        public void AddPalette(SymbolPalette symbolPalette)
        {
            GroupBarItem paletteBarItem = new GroupBarItem();
            paletteBarItem.Text = symbolPalette.Name;
            paletteBarItem.Tag = symbolPalette;

            PaletteGroupView paletteView = new PaletteGroupView();
            paletteView.SelectedItem = -1;
            paletteView.ShowDragNodeCue = m_bShowDragNodeCue;
            paletteView.DragNodeCueEnabled = m_bDragNodeCueEnabled;
            paletteView.EditMode = this.EditMode;
            paletteView.ButtonView = true;
            paletteView.BackColor = Color.Ivory;
            paletteView.Diagram = this.Diagram;
            paletteView.DoubleClick += new EventHandler(PaletteView_DoubleClick);
            paletteView.NodeSelected += new NodeEventHandler(PaletteView_NodeSelected);

            paletteView.LoadPalette(symbolPalette);
            paletteBarItem.Client = paletteView;
            this.GroupBarItems.Add(paletteBarItem);
        }

        /// <summary>
        /// Adds a new symbol palette to the GroupBar with the given name.
        /// </summary>
        /// <param name="paletteName">Name of symbol palette to create.</param>
        /// <returns>Symbol palette to be added to groupbar.</returns>
        public SymbolPalette AddPalette(string paletteName)
        {
            SymbolPalette symbolPalette = new SymbolPalette();
            symbolPalette.Name = paletteName;
            AddPalette(symbolPalette);
            return symbolPalette;
        }

        /// <summary>
        /// Adds a new symbol palette to the GroupBar after prompting the user for
        /// the name of the new symbol palette to create.
        /// </summary>
        /// <returns>Symbol palette to be added to groupbar.</returns>
        /// <remarks>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteAddDlg"/>
        /// </remarks>
        public SymbolPalette AddPalette()
        {
            SymbolPalette symbolPalette = null;
            PaletteAddDlg dlg = new PaletteAddDlg();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                symbolPalette = this.AddPalette(dlg.PaletteName);
            }

            return symbolPalette;
        }

        /// <summary>
        /// Loads a symbol palette from disk and adds it to the group bar.
        /// </summary>
        /// <param name="fileName">Name of symbol palette file to load.</param>
        /// <returns>PaletteGroupView created to hold the symbol palette.</returns>
        public PaletteGroupView LoadPalette(string fileName)
        {
            PaletteGroupView paletteView = new PaletteGroupView();
            paletteView.SelectedItem = -1;
            paletteView.EditMode = this.EditMode;
            paletteView.ShowDragNodeCue = m_bShowDragNodeCue;
            paletteView.DragNodeCueEnabled = m_bDragNodeCueEnabled;
            paletteView.ButtonView = true;
            paletteView.BackColor = Color.Ivory;
            paletteView.Diagram = this.Diagram;
            paletteView.DoubleClick += new EventHandler(PaletteView_DoubleClick);
            paletteView.NodeSelected+=new NodeEventHandler(PaletteView_NodeSelected);

            if (paletteView.LoadPalette(fileName))
            {
                // Load succeeded.
                GroupBarItem paletteBarItem = new GroupBarItem();
                SymbolPalette symbolPalette = paletteView.Palette;
                paletteBarItem.Text = symbolPalette.Name;
                paletteBarItem.Tag = symbolPalette;
                paletteBarItem.Client = paletteView;
                this.GroupBarItems.Add(paletteBarItem);
            }
            else
            {
                // Load failed.
                paletteView.Dispose();
                paletteView = null;
            }

            return paletteView;
        }

        /// <summary>
        /// Returns the symbol palette at the given index.
        /// </summary>
        /// <param name="paletteIdx">Zero-based index into the collection of symbol palettes loaded into the GroupBar control.</param>
        /// <returns>SymbolPalette object or NULL if paletteIdx parameter is out of range.</returns>
        public SymbolPalette GetPalette(int paletteIdx)
        {
            SymbolPalette symbolPalette = null;

            if (paletteIdx < GroupBarItems.Count)
            {
                symbolPalette = GroupBarItems[paletteIdx].Tag as SymbolPalette;
            }

            return symbolPalette;
        }

        /// <summary>
        /// Set the selected symbol model to the one matching the given symbol model name.
        /// </summary>
        /// <param name="node">Node to select.</param>
        public void SelectNode(Node node)
        {
            if (this.GroupBarItems.Count > 0)
            {
                PaletteGroupView curGroupView = this.GroupBarItems[0].Client as PaletteGroupView;

                if (curGroupView != null)
                {
                    curGroupView.SelectNode(node);
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Fired when the user selects a symbol model icon in a PaletteGroupBar component.
        /// </summary>
        public event NodeEventHandler NodeSelected;

        /// <summary>
        /// Fired when the user double clicks a symbol model icon in a PaletteGroupBar component.
        /// </summary>
        public event NodeEventHandler NodeDoubleClick;
        #endregion

        #region Event handlers
        private void PaletteView_NodeSelected(object sender, NodeEventArgs evtArgs)
        {
            if (NodeSelected != null)
                NodeSelected(this, new NodeEventArgs(this.SelectedNode));
        }

        private void PaletteView_DoubleClick(object sender, EventArgs e)
        {
            if (NodeDoubleClick != null)
                NodeDoubleClick(this, new NodeEventArgs(this.SelectedNode));
        }
        #endregion
    }
}
