using System.Windows.Controls;
using UFUAModel;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseChannelSettings.xaml
    /// </summary>
    public partial class BaseDynamicSettingsNoMethod : UserControl
    {
        #region Declarations
        bool bLoaded, bFilled, bEditing;
        TagEntityReference original;
        #endregion

        /// <summary>
        /// basic channel settings 
        /// </summary>
        public BaseDynamicSettingsNoMethod()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;                 
                }
            };
        }
    }
}
