#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Summary description for ChartWizardImages.
    /// </summary>
    sealed class ChartWizardResources
    {
        #region Members
        private readonly static Image c_wizardBackImage;
        private readonly static Image c_loadTemplateBackImage;

        private readonly static Image c_tabHeaderNormal;
        private readonly static Image c_tabHeaderSelected;

        private readonly static Image c_buttonCloseNormal;
        private readonly static Image c_buttonCloseSelected;

        private readonly static Image c_buttonNormal;
        private readonly static Image c_buttonSelected;

        private readonly static Image c_buttonNextNormal;
        private readonly static Image c_buttonNextSelected;

        private readonly static Image c_buttonPrevNormal;
        private readonly static Image c_buttonPrevSelected;

        private readonly static Image c_buttonTLNormal;
        private readonly static Image c_buttonTCNormal;
        private readonly static Image c_buttonTRNormal;

        private readonly static Image c_buttonCLNormal;
        private readonly static Image c_buttonCCNormal;
        private readonly static Image c_buttonCRNormal;

        private readonly static Image c_buttonBLNormal;
        private readonly static Image c_buttonBCNormal;
        private readonly static Image c_buttonBRNormal;

        private readonly static Image c_buttonTLSelected;
        private readonly static Image c_buttonTCSelected;
        private readonly static Image c_buttonTRSelected;

        private readonly static Image c_buttonCLSelected;
        private readonly static Image c_buttonCCSelected;
        private readonly static Image c_buttonCRSelected;

        private readonly static Image c_buttonBLSelected;
        private readonly static Image c_buttonBCSelected;
        private readonly static Image c_buttonBRSelected;

        private readonly static Icon c_wizardIcon;
        #endregion

        #region Properties

        #region Images properties
        /// <summary>
        /// Gets the wizard back image.
        /// </summary>
        /// <value>The wizard back image.</value>
        public static Image WizardBackImage
        {
            get
            {
                return c_wizardBackImage;
            }
        }

        /// <summary>
        /// Gets the load template back image.
        /// </summary>
        /// <value>The load template back image.</value>
        public static Image LoadTemplateBackImage
        {
            get
            {
                return c_loadTemplateBackImage;
            }
        }

        /// <summary>
        /// Gets the tab header normal image.
        /// </summary>
        /// <value>The tab header normal image.</value>
        public static Image ButtonAlternativeNormal
        {
            get
            {
                return c_buttonCloseNormal;
            }
        }

        /// <summary>
        /// Gets the tab header normal image.
        /// </summary>
        /// <value>The tab header normal image.</value>
        public static Image ButtonAlternativeSelected
        {
            get
            {
                return c_buttonCloseSelected;
            }
        }

        /// <summary>
        /// Gets the tab header normal image.
        /// </summary>
        /// <value>The tab header normal image.</value>
        public static Image TabHeaderNormalImage
        {
            get
            {
                return c_tabHeaderNormal;
            }
        }

        /// <summary>
        /// Gets the tab header selected image.
        /// </summary>
        /// <value>The tab header selected image.</value>
        public static Image TabHeaderSelectedImage
        {
            get
            {
                return c_tabHeaderSelected;
            }
        }

        /// <summary>
        /// Gets the button normal image.
        /// </summary>
        /// <value>The button normal image.</value>
        public static Image ButtonNormalImage
        {
            get
            {
                return c_buttonNormal;
            }
        }

        /// <summary>
        /// Gets the button selected image.
        /// </summary>
        /// <value>The button selected image.</value>
        public static Image ButtonSelectedImage
        {
            get
            {
                return c_buttonSelected;
            }
        }

        /// <summary>
        /// Gets the button normal image.
        /// </summary>
        /// <value>The button normal image.</value>
        public static Image ButtonNextNormalImage
        {
            get
            {
                return c_buttonNextNormal;
            }
        }

        /// <summary>
        /// Gets the button selected image.
        /// </summary>
        /// <value>The button selected image.</value>
        public static Image ButtonNextSelectedImage
        {
            get
            {
                return c_buttonNextSelected;
            }
        }

        /// <summary>
        /// Gets the button normal image.
        /// </summary>
        /// <value>The button normal image.</value>
        public static Image ButtonPrevNormalImage
        {
            get
            {
                return c_buttonPrevNormal;
            }
        }

        /// <summary>
        /// Gets the button selected image.
        /// </summary>
        /// <value>The button selected image.</value>
        public static Image ButtonPrevSelectedImage
        {
            get
            {
                return c_buttonPrevSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN top left image.
        /// </summary>
        /// <value>The BTTN top left image.</value>
        public static Image ButtonTLNormalImage
        {
            get
            {
                return c_buttonTLNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN top center image.
        /// </summary>
        /// <value>The BTTN top center image.</value>
        public static Image ButtonTCNormalImage
        {
            get
            {
                return c_buttonTCNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN top right image.
        /// </summary>
        /// <value>The BTTN top right image.</value>
        public static Image ButtonTRNormalImage
        {
            get
            {
                return c_buttonTRNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN center left image.
        /// </summary>
        /// <value>The BTTN center left image.</value>
        public static Image ButtonCLNormalImage
        {
            get
            {
                return c_buttonCLNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN center center image.
        /// </summary>
        /// <value>The BTTN center center image.</value>
        public static Image ButtonCCNormalImage
        {
            get
            {
                return c_buttonCCNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN center right image.
        /// </summary>
        /// <value>The BTTN center right image.</value>
        public static Image ButtonCRNormalImage
        {
            get
            {
                return c_buttonCRNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN bottom left image.
        /// </summary>
        /// <value>The BTTN bottom left image.</value>
        public static Image ButtonBLNormalImage
        {
            get
            {
                return c_buttonBLNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN bottom center image.
        /// </summary>
        /// <value>The BTTN bottom center image.</value>
        public static Image ButtonBCNormalImage
        {
            get
            {
                return c_buttonBCNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN bottom right image.
        /// </summary>
        /// <value>The BTTN bottom right image.</value>
        public static Image ButtonBRNormalImage
        {
            get
            {
                return c_buttonBRNormal;
            }
        }

        /// <summary>
        /// Gets the BTTN top left image.
        /// </summary>
        /// <value>The BTTN top left image.</value>
        public static Image ButtonTLSelectedImage
        {
            get
            {
                return c_buttonTLSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN top center image.
        /// </summary>
        /// <value>The BTTN top center image.</value>
        public static Image ButtonTCSelectedImage
        {
            get
            {
                return c_buttonTCSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN top right image.
        /// </summary>
        /// <value>The BTTN top right image.</value>
        public static Image ButtonTRSelectedImage
        {
            get
            {
                return c_buttonTRSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN center left image.
        /// </summary>
        /// <value>The BTTN center left image.</value>
        public static Image ButtonCLSelectedImage
        {
            get
            {
                return c_buttonCLSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN center center image.
        /// </summary>
        /// <value>The BTTN center center image.</value>
        public static Image ButtonCCSelectedImage
        {
            get
            {
                return c_buttonCCSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN center right image.
        /// </summary>
        /// <value>The BTTN center right image.</value>
        public static Image ButtonCRSelectedImage
        {
            get
            {
                return c_buttonCRSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN bottom left image.
        /// </summary>
        /// <value>The BTTN bottom left image.</value>
        public static Image ButtonBLSelectedImage
        {
            get
            {
                return c_buttonBLSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN bottom center image.
        /// </summary>
        /// <value>The BTTN bottom center image.</value>
        public static Image ButtonBCSelectedImage
        {
            get
            {
                return c_buttonBCSelected;
            }
        }

        /// <summary>
        /// Gets the BTTN bottom right image.
        /// </summary>
        /// <value>The BTTN bottom right image.</value>
        public static Image ButtonBRSelectedImage
        {
            get
            {
                return c_buttonBRSelected;
            }
        }

        /// <summary>
        /// Gets the wizard icon.
        /// </summary>
        /// <value>The wizard icon.</value>
        public static Icon WizardIcon
        {
            get
            {
                return c_wizardIcon;
            }
        }
        #endregion

        #region Text properties
        /// <summary>
        /// Gets caption of the Chart Type button.
        /// </summary>
        public static string ButtonChartTypeText
        {
            get
            {
                return "Chart Type";
            }
        }

        /// <summary>
        /// Gets caption of the Series button.
        /// </summary>
        public static string ButtonSeriesText
        {
            get
            {
                return "Series";
            }
        }

        /// <summary>
        /// Gets caption of the Appearance button.
        /// </summary>
        public static string ButtonAppearanceText
        {
            get
            {
                return "Appearance";
            }
        }

        /// <summary>
        /// Gets caption of the Axes button.
        /// </summary>
        public static string ButtonAxesText
        {
            get
            {
                return "Axes";
            }
        }

        /// <summary>
        /// Gets caption of the Legend button.
        /// </summary>
        public static string ButtonLegendText
        {
            get
            {
                return "Legend";
            }
        }

        /// <summary>
        /// Gets caption of the ToolBar button.
        /// </summary>
        public static string ButtonToolBarText
        {
            get
            {
                return "ToolBar";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the series type is setting.
        /// </summary>
        public static string ChartTypeTitle
        {
            get
            {
                return "Select Chart Type group from a combobox then choose the ChartType.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the data source is setting.
        /// </summary>
        public static string DataSourceTitle
        {
            get
            {
                return "2a) Select an existing datasource or create a new one, and go to the 'Series Data' Tab.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the palette is setting.
        /// </summary>
        public static string PaletteTitle
        {
            get
            {
                return "Select color palette of the series.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the appearance is setting.
        /// </summary>
        public static string BorderAndBackgroundTitle
        {
            get
            {
                return "Select Border Style and Back Color of the Chart.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the title of chart is setting.
        /// </summary>
        public static string TitleTitle
        {
            get
            {
                return "Chart Title Customization.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the axes is editing.
        /// </summary>
        public static string AxesTitle
        {
            get
            {
                return "Control the appearance of the ChartControl Axes.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the legend is editing.
        /// </summary>
        public static string LegendTitle
        {
            get
            {
                return "Control the appearance of the ChartControl Legend.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the toolbar is editing.
        /// </summary>
        public static string ToolBarTitle
        {
            get
            {
                return "Control the appearance of the ChartControl ToolBar.";
            }
        }

        /// <summary>
        /// Gets the points title.
        /// </summary>
        /// <value>The points title.</value>
        public static string PointsTitle
        {
            get
            {
                return "Points labels are used to display the text of style ( like YValues of points ) You can change their color, font, angle, position.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the toolbar is editing.
        /// </summary>
        public static string SeriesPointsTitle
        {
            get
            {
                return "1) Generate custom points for the series.";
            }
        }

        /// <summary>
        /// Gets text of the wizard title when the toolbar is editing.
        /// </summary>
        public static string SeriesDataTitle
        {
            get
            {
                return "2b) Assign the chart series to the selected data source members. Note: Series will be databound, when you run the application.";
            }
        }

        /// <summary>
        /// Gets the remove points message.
        /// </summary>
        /// <value>The remove points message.</value>
        public static string RemovePointsMessage
        {
            get
            {
                return "This series already has points collection. Do you want to remove them?";
            }
        }

        /// <summary>
        /// Gets the remove binding message.
        /// </summary>
        /// <value>The remove binding message.</value>
        public static string RemoveBindingMessage
        {
            get
            {
                return "This series already has data source binding. Do you want to remove them?";
            }
        }

        /// <summary>
        /// Gets the series incompatible message.
        /// </summary>
        /// <value>The series incompatible message.</value>
        public static string SeriesIncompatibleMessage
        {
            get
            {
                return "This type isn't compatible with other series. Do you want to use it?";
            }
        }

        /// <summary>
        /// Gets the model change title.
        /// </summary>
        /// <value>The model change title.</value>
        public static string ModelChangeTitle
        {
            get
            {
                return "The another model is present.";
            }
        }

        /// <summary>
        /// Gets the null template selected text.
        /// </summary>
        /// <value>The null template selected text.</value>
        public static string NullTemplateSelectedText
        {
            get
            {
                return "Invalid Template selected.";
            }
        }

        /// <summary>
        /// Gets the null template selected title.
        /// </summary>
        /// <value>The null template selected title.</value>
        public static string NullTemplateSelectedTitle
        {
            get
            {
                return "Invalid Template selected.";
            }
        }
        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="ChartWizardResources"/> class.
        /// </summary>
        static ChartWizardResources()
        {
            Type type = typeof(ChartWizardResources);

            c_wizardBackImage = new Bitmap(type, "Designer.Resources.WizardBackground.png");
            c_loadTemplateBackImage = new Bitmap(type, "Designer.Resources.LoadTemplateBackground.png");

            c_tabHeaderNormal = new Bitmap(type, "Designer.Resources.TabHeaderNormal.png");
            c_tabHeaderSelected = new Bitmap(type, "Designer.Resources.TabHeaderSelected.png");

            c_buttonNormal = new Bitmap(type, "Designer.Resources.ButtonNormal.png");
            c_buttonSelected = new Bitmap(type, "Designer.Resources.ButtonSelected.png");

            c_buttonCloseNormal = new Bitmap(type, "Designer.Resources.ButtonCloseNormal.png");
            c_buttonCloseSelected = new Bitmap(type, "Designer.Resources.ButtonCloseSelected.png");

            c_buttonNextNormal = new Bitmap(type, "Designer.Resources.ButtonNextNormal.png");
            c_buttonNextSelected = new Bitmap(type, "Designer.Resources.ButtonNextSelected.png");

            c_buttonPrevNormal = new Bitmap(type, "Designer.Resources.ButtonPrevNormal.png");
            c_buttonPrevSelected = new Bitmap(type, "Designer.Resources.ButtonPrevSelected.png");

            c_buttonTLNormal = new Bitmap(type, "Designer.Resources.ButtonTLNormal.png");
            c_buttonTCNormal = new Bitmap(type, "Designer.Resources.ButtonTCNormal.png");
            c_buttonTRNormal = new Bitmap(type, "Designer.Resources.ButtonTRNormal.png");

            c_buttonCLNormal = new Bitmap(type, "Designer.Resources.ButtonCLNormal.png");
            c_buttonCCNormal = new Bitmap(type, "Designer.Resources.ButtonCCNormal.png");
            c_buttonCRNormal = new Bitmap(type, "Designer.Resources.ButtonCRNormal.png");

            c_buttonBLNormal = new Bitmap(type, "Designer.Resources.ButtonBLNormal.png");
            c_buttonBCNormal = new Bitmap(type, "Designer.Resources.ButtonBCNormal.png");
            c_buttonBRNormal = new Bitmap(type, "Designer.Resources.ButtonBRNormal.png");

            c_buttonTLSelected = new Bitmap(type, "Designer.Resources.ButtonTLSelected.png");
            c_buttonTCSelected = new Bitmap(type, "Designer.Resources.ButtonTCSelected.png");
            c_buttonTRSelected = new Bitmap(type, "Designer.Resources.ButtonTRSelected.png");

            c_buttonCLSelected = new Bitmap(type, "Designer.Resources.ButtonCLSelected.png");
            c_buttonCCSelected = new Bitmap(type, "Designer.Resources.ButtonCCSelected.png");
            c_buttonCRSelected = new Bitmap(type, "Designer.Resources.ButtonCRSelected.png");

            c_buttonBLSelected = new Bitmap(type, "Designer.Resources.ButtonBLSelected.png");
            c_buttonBCSelected = new Bitmap(type, "Designer.Resources.ButtonBCSelected.png");
            c_buttonBRSelected = new Bitmap(type, "Designer.Resources.ButtonBRSelected.png");

            c_wizardIcon = new Icon(type, "Designer.Resources.WizardIcon.ico");
        }
        #endregion
    }
}
