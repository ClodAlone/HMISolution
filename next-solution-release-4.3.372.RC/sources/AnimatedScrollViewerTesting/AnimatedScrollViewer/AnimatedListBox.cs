using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimatedScrollViewer
{
    [TemplatePart(Name = "PART_AnimatedScrollViewer", Type = typeof(AnimatedScrollViewer))]

    public class AnimatedListBox : ListBox
    {
        #region PART holders
        AnimatedScrollViewer _scrollViewer;
        #endregion
        
        static AnimatedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedListBox), new FrameworkPropertyMetadata(typeof(AnimatedListBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AnimatedScrollViewer scrollViewerHolder = base.GetTemplateChild("PART_AnimatedScrollViewer") as AnimatedScrollViewer;
            if (scrollViewerHolder != null)
            {
                _scrollViewer = scrollViewerHolder;
            }

            this.SelectionChanged += new SelectionChangedEventHandler(AnimatedListBox_SelectionChanged);
            this.Loaded += new RoutedEventHandler(AnimatedListBox_Loaded);
            this.LayoutUpdated += new EventHandler(AnimatedListBox_LayoutUpdated);
        }

        void AnimatedListBox_LayoutUpdated(object sender, EventArgs e)
        {
            updateScrollPosition(sender);
        }

        void AnimatedListBox_Loaded(object sender, RoutedEventArgs e)
        {
            updateScrollPosition(sender);
        }

        void AnimatedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateScrollPosition(sender);
        }

        public void updateScrollPosition(object sender)
        {
            AnimatedListBox thisLB = (AnimatedListBox)sender;

            if (thisLB != null)
            {
                if (thisLB.ScrollToSelectedItem)
                {
                    double scrollTo = 0;
                    for (int i = 0; i < (thisLB.SelectedIndex + thisLB.SelectedIndexOffset); i++)
                    {
                        ListBoxItem tempItem = thisLB.ItemContainerGenerator.ContainerFromItem(thisLB.Items[i]) as ListBoxItem;

                        if (tempItem != null)
                        {
                            scrollTo += tempItem.ActualHeight;
                        }
                    }

                    _scrollViewer.TargetVerticalOffset = scrollTo;
                }
            }
        }



        #region ScrollToSelectedItem (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public bool ScrollToSelectedItem
        {
            get { return (bool)GetValue(ScrollToSelectedItemProperty); }
            set { SetValue(ScrollToSelectedItemProperty, value); }
        }
        public static readonly DependencyProperty ScrollToSelectedItemProperty =
            DependencyProperty.Register("ScrollToSelectedItem", typeof(bool), typeof(AnimatedListBox),
            new PropertyMetadata(false));
        #endregion

        #region SelectedIndexOffset (DependencyProperty)

        /// <summary>
        /// Use this property to choose the scroll to an item that is not selected, but is X above or below the selected item
        /// </summary>
        public int SelectedIndexOffset
        {
            get { return (int)GetValue(SelectedIndexOffsetProperty); }
            set { SetValue(SelectedIndexOffsetProperty, value); }
        }
        public static readonly DependencyProperty SelectedIndexOffsetProperty =
            DependencyProperty.Register("SelectedIndexOffset", typeof(int), typeof(AnimatedListBox),
              new PropertyMetadata(0));

        #endregion
    }
}
