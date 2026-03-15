using System;
using System.Windows.Controls;
using System.Windows;
using Utilities;

namespace UFSolution
{
    public class ToolbarButton : Button
    {
        public ToolbarButton()
        {
        }

        static ToolbarButton()
        {
            DescriptionProperty = DependencyProperty.Register(
                "Description", typeof(string), typeof(ToolbarButton),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
        }

        #region Dependency Properties
        public static DependencyProperty DescriptionProperty;
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Listen for the OnPropertyChanged event and overwrite the tooltip with the tooltip
        /// in our ScribbleCommand (if that's what we're attached to).
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (0 == string.Compare(e.Property.Name, "Command"))
            {
                GeneralCommand newCommand = e.NewValue as GeneralCommand;
                if (null != newCommand)
                {
                    ToolTip = newCommand.Tooltip;
                    Description = newCommand.Description;
                }
            }
        }
    }
}
