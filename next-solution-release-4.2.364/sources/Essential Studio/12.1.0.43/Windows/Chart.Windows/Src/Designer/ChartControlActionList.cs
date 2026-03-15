#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if SyncfusionFramework2_0
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Provides the UI editor of chart appearance styles.
    /// </summary>
    class ChartAppearanceStylesEditor : ChartDropDownUIEditor
    {
        #region Implementation
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns>Returns ChartAppearanceStyles IList collection.</returns>
        protected override IList GetValues()
        {
            return new string[]{
                                    ChartAppearanceStyles.NoneFormat,
                                    ChartAppearanceStyles.BlackFormat,
                                    ChartAppearanceStyles.ContrastFormat,
                                    ChartAppearanceStyles.DefaultFormat,
                                    ChartAppearanceStyles.GainsBoroFormat,
                                    ChartAppearanceStyles.GradientFormat,
                                    ChartAppearanceStyles.LightOliveFormat,
                                    ChartAppearanceStyles.LinenFormat,
                                    ChartAppearanceStyles.MistyRoseFormat,
                                    ChartAppearanceStyles.PaleYellowFormat,
                                    ChartAppearanceStyles.PinkOverlayFormat,
                                    ChartAppearanceStyles.SolidColorFormat,
                                    ChartAppearanceStyles.TriColorFormat
                              };
        }
        #endregion
    }

    /// <summary>
    /// This class contains the action items of <see cref="ChartControl"/>.
    /// </summary>
    ///<internalonly/>
    [DocumentationExclude()]
    public class ChartControlActionList : SyncActionListBase<ChartControl>
    {
        #region Class constants
        /// <summary>
        /// Store chart category.
        /// </summary>
        private const string c_chartCategory = "Chart";

        /// <summary>
        /// Store back interior category.
        /// </summary>
        private const string c_backInteriorCategory = "Back Style";

        /// <summary>
        /// Store palette category.
        /// </summary>
        private const string c_paletteCategory = "Palette";

        /// <summary>
        /// Store legend category.
        /// </summary>
        private const string c_legendCategory = "Legend";

        /// <summary>
        /// Store appearance category.
        /// </summary>
        private const string c_appearance = "Appearance";

        /// <summary>
        /// Store axis title category.
        /// </summary>
        private const string c_axisTitle = "Axis title";

        /// <summary>
        /// Store default filter for open template dialog.
        /// </summary>
        private const string c_templateFileFilter = "*.xml | *.xml";

        /// <summary>
        /// Store default question on reset chart settings.
        /// </summary>
        private const string c_resetMesaage = "Are you sure want to reset settings?";

        #endregion

        #region Class members
        private DesignerActionUIService m_uiService;
        private string m_appearanceStyles = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartControlActionList"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        public ChartControlActionList(IComponent component)
            : base(component)
        {
            m_uiService = this.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the action list.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Chart for Windows Forms");

            // Title
            this.AddDesignerActionPropertyItem("Text", "Title", c_chartCategory, "Specifies the title for the chart.");

            this.AddDesignerActionPropertyItem("TextPosition", "Title Position", c_chartCategory, "Specifies the position of the title for the chart.");

            // Back style
            this.AddDesignerActionPropertyItem("BackInterior", "Back Interior", c_backInteriorCategory, string.Empty);

            this.AddDesignerActionPropertyItem("AreaBackInterior", "Area Back Interior", c_backInteriorCategory, string.Empty);

            this.AddDesignerActionPropertyItem("ChartBackInterior", "Chart Back Interior", c_backInteriorCategory, string.Empty);

            // palette
            this.AddDesignerActionPropertyItem("AppearanceStyles", "Appearance Style", c_paletteCategory, "Select a appearance style to be used.");
            ////this.AddDesignerActionPropertyItem("Palette", "Palette", c_paletteCategory, "Select a palette to be used.");
            this.AddDesignerActionPropertyItem("AllowGradientPalette", "Allow Gradient Palette", c_paletteCategory, "Allow Gradient Palette.");

            if (this.Palette == ChartColorPalette.Custom)
            {
                // this.AddDesignerActionPropertyItem("CustomPalette", "Custom Palette", c_paletteCategory, "Edit a custom palette.");
            }

            // Legend
            this.AddDesignerActionPropertyItem("ShowLegend", "Show Legend", c_legendCategory, "Specifies if the chart legend is to be displayed.");

            if (this.ShowLegend)
            {
                this.AddDesignerActionPropertyItem("LegendPosition", "Legend Position", c_legendCategory, "Specifies the position of the legend.");

                this.AddDesignerActionPropertyItem("LegendAlignment", "Legend Alignment", c_legendCategory, "Specifies the legend alignment.");
            }

            // appearance
            this.AddDesignerActionPropertyItem("ShowToolbar", "Show Toolbar", c_appearance, "Specifies if the toolbar is to be displayed.");
            this.AddDesignerActionPropertyItem("ShowContextMenu", "Show ContextMenu", c_appearance, "Specifies if the context menu is to be displayed.");

            // axis title
            this.AddDesignerActionPropertyItem("XAxisTitle", "X Axis Title", c_axisTitle, "Specifies the title of the priary X axis.");

            this.AddDesignerActionPropertyItem("YAxisTitle", "Y Axis Title", c_axisTitle, "Specifies the title of the priary Y axis.");

            // Methods
            this.AddDesignerActionMethodItem("ChartWizard", "Chart Wizard", "Methods", "Use the Chart Wizard to make changes to chart appearance.");

            this.AddDesignerActionMethodItem("LoadTemplate", "Load Template", "Methods", "Load template from a file.");

            this.AddDesignerActionMethodItem("SaveTemplate", "Save Template", "Methods", "Save template to file.");

            this.AddDesignerActionMethodItem("ResetTemplate", "Reset Template", "Methods", "Reset current template.");
        }

        /// <summary>
        /// Refreshes the actions.
        /// </summary>
        protected void RefreshActions()
        {
            if (m_uiService != null)
            {
                m_uiService.Refresh(this.Component);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Reset template.
        /// </summary>
        public void ResetTemplate()
        {
            if (this.Control != null)
            {
                if (MessageBox.Show(c_resetMesaage, "Reset...", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ChartTemplate template = new ChartTemplate(typeof(ChartControl));
                    template.Reset(this.Control as ChartControl);
                }
            }
        }

        /// <summary>
        /// Save template.
        /// </summary>
        public void SaveTemplate()
        {
            bool templateAll = false;
            string msg = "Do you want store only the appearance?";

            if(MessageBox.Show(msg,"Template Storing Option...", MessageBoxButtons.YesNo)==DialogResult.No)
            {
                templateAll = true;
            }

            if (this.Control != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = c_templateFileFilter;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ChartTemplate.StoreAllProperties = templateAll;
                    ChartTemplate template = new ChartTemplate(typeof(ChartControl));                    
                    template.Scan(this.Control as ChartControl);
                    template.Save(sfd.FileName);
                }
            }
        }

        /// <summary>
        /// Load  template and apply it to the current <see cref="ChartControl"/>.
        /// </summary>
        public void LoadTemplate()
        {
            if (this.Control != null)
            {
                ChartTemplateWizard templateWizard = new ChartTemplateWizard();

                if (templateWizard.ShowDialog() == DialogResult.OK)
                {
                    if (templateWizard.Template != null)
                    {
                        templateWizard.Template.Apply(this.Control as ChartControl);
                    }
                    else
                    {
                        MessageBox.Show(ChartWizardResources.NullTemplateSelectedText,
                                        ChartWizardResources.NullTemplateSelectedTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Calls the <see cref="ChartControl.DisplayWizard"/> method.
        /// </summary>
        public void ChartWizard()
        {
            if (this.Control != null)
            {
                ChartControl control = this.Control as ChartControl;
                control.DisplayWizard();
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Y axis title.This is duplicate of PrimaryYAxis.Title property.
        /// </summary>
        /// <value>The Y axis title.</value>
        public string YAxisTitle
        {
            get
            {
                string res = string.Empty;
                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    res = c.PrimaryYAxis.Title;
                }

                return res;
            }

            set
            {
                if (this.Control != null)
                {
                    (this.Control as ChartControl).PrimaryYAxis.Title = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the X axis title. This is duplicate of PrimaryXAxis.Title property.
        /// </summary>
        /// <value>The X axis title.</value>
        public string XAxisTitle
        {
            get
            {
                string res = string.Empty;
                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    res = c.PrimaryXAxis.Title;
                }

                return res;
            }

            set
            {
                if (this.Control != null)
                {
                    (this.Control as ChartControl).PrimaryXAxis.Title = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the legend alignment. This is duplicate of <see cref="ChartControl.LegendAlignment"/> property.
        /// </summary>
        /// <value>The legend alignment.</value>
        public ChartAlignment LegendAlignment
        {
            get
            {
                if (this.Control != null)
                {
                    ChartControl c = this.Control;
                    return c.LegendAlignment;
                }

                return ChartAlignment.Center;
            }

            set
            {
                if (this.Control != null)
                {
                    SetValue("LegendAlignment", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the chart back interior. This is duplicate of <see cref="ChartControl.ChartInterior"/> property.
        /// </summary>
        /// <value>The chart back interior.</value>
        [Editor(typeof(BrushInfoEditor), typeof(UITypeEditor))]
        public BrushInfo ChartBackInterior
        {
            get
            {
                BrushInfo br = new BrushInfo();
                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    br = c.ChartInterior;
                }

                return br;
            }

            set
            {
                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    c.ChartInterior = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the area back interior. This is duplicate of <see cref="ChartArea.BackInterior"/> property.
        /// </summary>
        /// <value>The area back interior.</value>
        [Editor(typeof(BrushInfoEditor), typeof(UITypeEditor))]
        public BrushInfo AreaBackInterior
        {
            get
            {
                BrushInfo br = new BrushInfo();
                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    br = c.ChartArea.BackInterior;
                }

                return br;
            }

            set
            {
                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    c.ChartArea.BackInterior = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the back interior. This is duplicate of <see cref="ChartControl.BackInterior"/> property.
        /// </summary>
        /// <value>The back interior.</value>
        [Editor(typeof(BrushInfoEditor), typeof(UITypeEditor))]
        public BrushInfo BackInterior
        {
            get
            {
                BrushInfo br = new BrushInfo();

                if (this.Control != null)
                {
                    ChartControl c = this.Control as ChartControl;
                    br = c.BackInterior;
                }

                return br;
            }

            set
            {
                SetValue("BackInterior", value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing. This is duplicate of <see cref="ChartControl.Spacing"/> property.
        /// </summary>
        /// <value>The spacing.</value>
        public float Spacing
        {
            get
            {
                float spacing = 0;
                if (this.Control != null)
                {
                    ChartControl control = this.Control as ChartControl;
                    spacing = control.Spacing;
                }

                return spacing;
            }

            set
            {
                SetValue("Spacing", value);
            }
        }
        
        /// <summary>
        /// Gets or sets the appearance styles.
        /// </summary>
        /// <value>The appearance styles.</value>
        [Editor(typeof(ChartAppearanceStylesEditor), typeof(UITypeEditor))]
        public string AppearanceStyles
        {
            get
            {
                return m_appearanceStyles;
            }

            set
            {
                m_appearanceStyles = value;
                ChartAppearanceStyles.ApplyFormat(this.Control, m_appearanceStyles);

                if (this.Control.Site != null)
                {
                    IComponentChangeService changeService = this.Control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                    if (changeService != null)
                    {
                        changeService.OnComponentChanged(this.Control, null, null, null);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the palette. This is duplicate of <see cref="ChartControl.Palette"/> property.
        /// </summary>
        /// <value>The palette.</value>
        [Editor(typeof(ChartColorPaletteEditor), typeof(UITypeEditor))]
        public ChartColorPalette Palette
        {
            get
            {
                ChartColorPalette palette = ChartColorPalette.Colorful;
                if (this.Control != null)
                {
                    palette = Control.Palette;
                }

                return palette;
            }

            set
            {
                SetValue("Palette", value);
                this.RefreshActions();
            }
        }

        /// <summary>
        /// Gets or sets the custom palette. This is duplicate of <see cref="ChartControl.CustomPalette"/> property.
        /// </summary>
        /// <value>The custom palette.</value>
        [Editor(typeof(ColorsUIEditor), typeof(UITypeEditor))]
        public Color[] CustomPalette
        {
            get
            {
                return this.Control == null ? null : this.Control.CustomPalette;
            }

            set
            {
                SetValue("CustomPalette", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow gradient palette. This is duplicate of <see cref="ChartControl.AllowGradientPalette"/> property.
        /// </summary>
        /// <value>
        ///      <c>true</c> if [allow gradient palette]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowGradientPalette
        {
            get
            {
                return this.Control == null ? false : this.Control.AllowGradientPalette;
            }

            set
            {
                SetValue("AllowGradientPalette", value);
            }
        }

        /// <summary>
        /// Gets or sets the text position. This is duplicate of <see cref="ChartControl.TextPosition"/> property.
        /// </summary>
        /// <value>The text position.</value>
        public ChartTextPosition TextPosition
        {
            get
            {
                ChartTextPosition txtPos = ChartTextPosition.Top;

                if (this.Control != null)
                {
                    ChartControl control = this.Control as ChartControl;
                    txtPos = control.TextPosition;
                }

                return txtPos;
            }

            set
            {
                SetValue("TextPosition", value);
            }
        }

        /// <summary>
        /// Gets or sets the text. This is duplicate of <see cref="ChartControl.Text"/> property.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                string text = String.Empty;
                if (this.Control != null)
                {
                    ChartControl control = this.Control as ChartControl;
                    text = control.Text;
                }

                return text;
            }

            set
            {
                SetValue("Text", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show legend]. This is duplicate of <see cref="ChartControl.ShowLegend"/> property.
        /// </summary>
        /// <value><c>true</c> if [show legend]; otherwise, <c>false</c>.</value>
        public bool ShowLegend
        {
            get
            {
                bool showLegend = true;
                if (this.Control != null && this.Control.Legend != null)
                {
                    showLegend = Control.ShowLegend;
                }

                return showLegend;
            }

            set
            {
                SetValue("ShowLegend", value);
                this.RefreshActions();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show the toolbar or not. This is duplicate of <see cref="ChartControl.ShowToolbar"/> property.
        /// </summary>
        /// <value><c>true</c> if [show toolbar]; otherwise, <c>false</c>.</value>
        public bool ShowToolbar
        {
            get
            {
                bool showToolbar = false;
                if (this.Control != null)
                {
                    showToolbar = Control.ShowToolbar;
                }

                return showToolbar;
            }

            set
            {
                SetValue("ShowToolbar", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show the context menu. This is duplicate of <see cref="ChartControl.ShowContextMenu"/> property.
        /// </summary>
        /// <value><c>true</c> if [show context menu]; otherwise, <c>false</c>.</value>
        public bool ShowContextMenu
        {
            get
            {
                bool showContextMenu = false;
                if (this.Control != null)
                {
                    showContextMenu = Control.ShowContextMenu;
                }

                return showContextMenu;
            }

            set
            {
                SetValue("ShowContextMenu", value);
            }
        }

        /// <summary>
        /// Gets or sets the legend position. This is duplicate of <see cref="ChartControl.LegendPosition"/> property.
        /// </summary>
        /// <value>The legend position.</value>
        public ChartDock LegendPosition
        {
            get
            {
                ChartDock legendPosition = ChartDock.Right;
                if (this.Control != null)
                {
                    legendPosition = Control.LegendPosition;
                }

                return legendPosition;
            }

            set
            {
                SetValue("LegendPosition", value);
            }
        }
        #endregion
    }
}
#endif