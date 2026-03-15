using DevExpress.Xpf.LayoutControl;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using UFInterfaces.PropertyControl;
using UFRecipeLayout.Helpers;
using UFRecipeSettings.UFRecipeModel;
using UFUAModel.Extensions;
using Utilities.WPF;

namespace UFRecipeLayout.LayoutItemControls
{
    public class RecipeCommandButtonLayoutItem : RecipeLayoutItem
    {
        #region Dependency Properties

        #region ShowIcon
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(RecipeCommandButtonLayoutItem), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowIconChanged), new CoerceValueCallback(OnCoerceShowIcon)));

        private static object OnCoerceShowIcon(DependencyObject o, object value)
        {
            RecipeCommandButtonLayoutItem control = o as RecipeCommandButtonLayoutItem;
            if (control != null)
                return control.OnCoerceShowIcon((bool)value);
            else
                return value;
        }

        private static void OnShowIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeCommandButtonLayoutItem control = o as RecipeCommandButtonLayoutItem;
            if (control != null)
                control.OnShowIconChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowIcon(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowIconChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                var content = Content as ICommandUI;
                if (content != null)
                {
                    (content as ICommandUI).ShowIcon(newValue);
                }
            }
        }

        public bool ShowIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowIconProperty);
            }
            set
            {
                SetValue(ShowIconProperty, value);
            }
        }

        #endregion

        #region Caption
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(RecipeCommandButtonLayoutItem), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnCaptionChanged), new CoerceValueCallback(OnCoerceCaption)));

        private static object OnCoerceCaption(DependencyObject o, object value)
        {
            RecipeCommandButtonLayoutItem control = o as RecipeCommandButtonLayoutItem;
            if (control != null)
                return control.OnCoerceCaption((string)value);
            else
                return value;
        }

        private static void OnCaptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeCommandButtonLayoutItem control = o as RecipeCommandButtonLayoutItem;
            if (control != null)
                control.OnCaptionChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceCaption(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCaptionChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                var content = Content as ICommandUI;
                if (content != null)
                {
                    (content as ICommandUI).SetCaption(newValue);
                }
            }
        }

        public string Caption
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        #endregion
        
        #endregion
    }
}
