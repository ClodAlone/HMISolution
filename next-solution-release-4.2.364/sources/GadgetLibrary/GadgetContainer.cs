using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GadgetLibrary
{
    public enum OptionButtonTypes
    {
        Settings,
        Resize,
        Other,
        None
    }


    [TemplatePart(Name = "PART_Gadget", Type = typeof (ContentControl))]
    [TemplatePart(Name = "PART_ControlBarGrid", Type = typeof (Grid))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof (Button))]
    [TemplatePart(Name = "PART_OptionButton", Type = typeof (Button))]
    public class GadgetContainer : Control, IGadgetContainer
    {
        public static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent(
            "Close", RoutingStrategy.Bubble,
            typeof (RoutedEventHandler), typeof (GadgetContainer));

        public static readonly RoutedEvent GadgetChangedEvent = EventManager.RegisterRoutedEvent(
            "GadgetChanged", RoutingStrategy.Bubble,
            typeof (RoutedPropertyChangedEventHandler<IGadget>), typeof (GadgetContainer));

        public static readonly DependencyProperty GadgetProperty =
            DependencyProperty.Register(
                "Gadget", typeof (IGadget), typeof (GadgetContainer),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGadgetChanged)));

        public static readonly DependencyProperty OptionButtonTypeProperty =
            DependencyProperty.Register(
                "OptionButtonType", typeof (OptionButtonTypes), typeof (GadgetContainer),
                new FrameworkPropertyMetadata(OptionButtonTypes.Settings));

        public static readonly RoutedEvent ShowOptionsEvent = EventManager.RegisterRoutedEvent(
            "ShowOptions", RoutingStrategy.Bubble,
            typeof (RoutedEventHandler), typeof (GadgetContainer));

        private Button _CloseButton;
        private Grid _ControlBarGrid;
        private Storyboard _FadeInStoryboard;
        private Storyboard _FadeOutStoryboard;
        private Button _OptionButton;

        static GadgetContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (GadgetContainer), new FrameworkPropertyMetadata(typeof (GadgetContainer)));
        }

        public IGadget Gadget
        {
            get { return (IGadget) GetValue(GadgetProperty); }
            set { SetValue(GadgetProperty, value); }
        }

        public OptionButtonTypes OptionButtonType
        {
            get { return (OptionButtonTypes) GetValue(OptionButtonTypeProperty); }
            set { SetValue(OptionButtonTypeProperty, value); }
        }

        public bool CanClose = true;
        #region IGadgetContainer Members

        public SnapRegions SnapRegion { get; set; }

        #endregion

        public override void OnApplyTemplate()
        {
            _ControlBarGrid = Template.FindName("PART_ControlBarGrid", this) as Grid;
            _FadeInStoryboard = Template.Resources["FadeInStoryboard"] as Storyboard;
            _FadeOutStoryboard = Template.Resources["FadeOutStoryboard"] as Storyboard;

            MouseEnter += GadgetContainer_MouseEnter;
            MouseLeave += GadgetContainer_MouseLeave;
            PreviewMouseLeftButtonDown += GadgetContainer_PreviewMouseLeftButtonDown;

            _CloseButton = Template.FindName("PART_CloseButton", this) as Button;

            if (_CloseButton != null)
            {
                if (CanClose)
                    _CloseButton.AddHandler(MouseLeftButtonUpEvent, new RoutedEventHandler(OnCloseButtonClick), true);
                else
                    _CloseButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            _OptionButton = Template.FindName("PART_OptionButton", this) as Button;

            if (_OptionButton != null)
            {
                if (OptionButtonType == OptionButtonTypes.None)
                    _OptionButton.Visibility = System.Windows.Visibility.Collapsed;
                else
                    _OptionButton.AddHandler(MouseLeftButtonUpEvent, new RoutedEventHandler(OnOptionButtonClick), true);
            }
        }


        private void GadgetContainer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ControlBarGrid.Opacity = 1;
        }

        private void GadgetContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            _FadeOutStoryboard.Begin(_ControlBarGrid, true);
        }

        private void GadgetContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            _FadeOutStoryboard.Stop(_ControlBarGrid);

            if (_ControlBarGrid.Opacity != 1)
            {
                _FadeInStoryboard.Begin(_ControlBarGrid);
            }
        }

        private void OnOptionButtonClick(object sender, RoutedEventArgs e)
        {
            e.RoutedEvent = ShowOptionsEvent;
            e.Source = sender;
            e.Handled = false;
            OnShowOptions(e);
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            e.RoutedEvent = CloseEvent;
            e.Source = sender;
            e.Handled = false;
            OnClose(e);
        }

        public event RoutedEventHandler ShowOptions
        {
            add { AddHandler(ShowOptionsEvent, value); }
            remove { RemoveHandler(ShowOptionsEvent, value); }
        }

        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        protected virtual void OnShowOptions(RoutedEventArgs args)
        {
            Gadget.OnShowOptions(OptionButtonType);
            RaiseEvent(args);
        }

        protected virtual void OnClose(RoutedEventArgs args)
        {
            RaiseEvent(args);
        }

        private static void OnGadgetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = (GadgetContainer) obj;

            var e = new RoutedPropertyChangedEventArgs<IGadget>(
                (IGadget) args.OldValue, (IGadget) args.NewValue, GadgetChangedEvent);
            control.OnGadgetChanged(e);
        }

        protected virtual void OnGadgetChanged(RoutedPropertyChangedEventArgs<IGadget> args)
        {
            RaiseEvent(args);
        }

        public event RoutedPropertyChangedEventHandler<IGadget> GadgetChanged
        {
            add { AddHandler(GadgetChangedEvent, value); }
            remove { RemoveHandler(GadgetChangedEvent, value); }
        }
    }
}